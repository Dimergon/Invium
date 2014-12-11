﻿/*
* PinCushion, a password manager in C#
* Copyright (c) 2013, 2014 Armin Altorffer
*
* This file is part of PinCushion.
*
* PinCushion is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* PinCushion is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with PinCushion.  If not, see <http://www.gnu.org/licenses/>.
*/

/*
* Main UI of PinCushion, houses most of its actual functionality.
*/
namespace PinCushion
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using System.Windows.Forms;
	using System.Xml;

	#if BuildForMono
	using Gtk;
	#endif

	public partial class MainForm : Form
	{
		/* 
		 * Quit after Max_Idle seconds when idle...
		 * Clear clipboard after CC_Time seconds...
		 */
		private const short MaxIdle = 300;
		private const short CCTime = 120;

		// Used in loading and saving...
		private const string XMLBody = "body";
		private const string XMLencrypt = "encrypted";
		private const string XMLSalt = "salt";
		private const string XMLProfile = "profile";
		private const string XMLService = "service";
		private const string XMLAccount = "account";
		private const string XMLName = "name";
		private const string XMLPassword = "password";
		private const string XMLCommand = "command";
		private const string XMLDescendant = "descendant";

		// Main container
		private List<Profile> profiles = new List<Profile> ();

		// Used in the automatic timeout functionality, PinCushion will quit after a certain period of inactivity.
		private DateTime timeout = DateTime.Now.AddSeconds (MaxIdle);
		private bool notimeout = false;

		// Used to clear the clipboard after copying a password
		private DateTime clipboardTimeout = DateTime.Now.AddSeconds (CCTime);

		// Used in auto saving upon changes
		private PinCushionPassword_Class pinCushionPassword = new PinCushionPassword_Class ();

		// Remind the user of an unsaved password...
		private bool unsavedPassword = false;
		private int[] unsavedPasswordIndeces = { 0, 0, 0 };

		public MainForm ()
		{
			this.InitializeComponent ();

			// Initialize the tray icon
			this.tray.BalloonTipTitle = this.Text = string.Format ("{0} v{1}", System.Windows.Forms.Application.ProductName, System.Windows.Forms.Application.ProductVersion);
			this.tray.BalloonTipIcon = ToolTipIcon.Info;

			/*
			 * Main form right click menu items...
			 */

			// disable timeout
			((ToolStripMenuItem)this.mainFormRightclick.Items [0]).Checked = this.notimeout;

			// languages
			foreach (string s in Program.Language.Traylangs) {
				((ToolStripMenuItem)this.mainFormRightclick.Items [1]).DropDownItems.Add (s, null, this.ChangeLanguage);
			}

			// Mark the active language
			foreach (ToolStripMenuItem t in ((ToolStripMenuItem)this.mainFormRightclick.Items[1]).DropDownItems) {
				if (t.Text == Program.Language.Trayfeedback) {
					t.Checked = true;
				}
			}

			// import functionality...
			((ToolStripMenuItem)this.mainFormRightclick.Items [2]).Enabled = false;
		}

		// Used in refreshing
		private enum RefreshLevel
		{
			None,
			Password,
			Account,
			Service,
			Profile
		}

		/*
		* Load all data
		*/
		public void DoLoad (string file, bool importing = false)
		{
			/* Actually load the data
			*
			* I experimented with various other approaches but XmlDocument + Xpath proved the least cumbersome.
			*/
			Stopwatch load_timer = new Stopwatch ();
			XmlDocument x_d = new XmlDocument ();
			x_d.Load (file);

			// Check for encryption
			bool do_decrypt = true;
			if (x_d.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLencrypt)) == null) {
				do_decrypt = false;
			}

			// User validation
			string salt = string.Empty, input_password = string.Empty;

			if (do_decrypt) {
				XmlNode salt_node = x_d.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLSalt));
				XmlNode password_node = x_d.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLPassword));
				if (salt_node == null) {
					throw new PinCushionException (Program.Language.FatalNoSaltNode);
				}

				if (password_node == null) {
					throw new PinCushionException (Program.Language.FatalNoPasswordNode);
				}

				salt = salt_node.InnerText;
				string password_hash = password_node.InnerText;

				// This InputBox will ask for the password, of either log on or of the import
				if (this.Inputbox (InputBoxMode.Singlepassword, ref input_password, importing ? Program.Language.ImportAuth : Program.Language.Login, importing ? Program.Language.ImportPassword : Program.Language.AuthPassword) == DialogResult.Cancel) {
					throw new PinCushionException (importing ? Program.Language.ImportCancel : Program.Language.AuthFailCancel);
				}

				if (Crypto.Hash (input_password, salt) != password_hash) {
					throw new PinCushionException (Program.Language.AuthFailIncorrect);
				}
			}

			// Used when re-encrypting
			bool do_recrypt_save = false;

			/*
			* - Securely store the password in case of initial log on (for (auto)saving purposes)
			* - Ask for a new password in case of an unencrypted main data file (to re-enable encryption)
			*
			* Skip if importing
			*/
			if (!importing) {
				if (!do_decrypt) {
					if (this.Inputbox (InputBoxMode.Doublepassword, ref input_password, string.Format (Program.Language.PinCushionReencryptTitle, System.Windows.Forms.Application.ProductName), string.Format (Program.Language.PinCushionReencryptPrompt, System.Windows.Forms.Application.ProductName), Program.Language.PinCushionReencryptConfirmation) == DialogResult.Cancel) {
						throw new PinCushionException (Program.Language.FatalNeedPasswordReencrypt);
					} else {
						do_recrypt_save = true;
					}
				}

				this.pinCushionPassword.Password = input_password;
			}

			load_timer.Start ();
			short p_count, s_count, a_count;
			p_count = s_count = a_count = 0;

			/*
			 * Main Loop, will also decrypt if data is encrypted
			 */
			foreach (XmlNode x_n_p in x_d.SelectNodes(string.Format("{0}::{1}", XMLDescendant, XMLProfile))) {
				p_count++;
				XmlNode p_name_node = x_n_p.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLName));
				string p_name = do_decrypt ? Crypto.Decrypt (p_name_node.InnerText, input_password, salt) : p_name_node.InnerText;

				// Check for duplicates, importing requires this step.
				if (this.profiles.Find (delegate(Profile x) {
					return x.Name == p_name;
				}) != null) {
					p_name += DateTime.Now.ToString ();
				}

				Profile p = new Profile (p_name);
				foreach (XmlNode x_n_s in x_n_p.SelectNodes(string.Format("{0}::{1}", XMLDescendant, XMLService))) {
					s_count++;
					XmlNode s_name_node = x_n_s.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLName));
					XmlNode s_command_node = x_n_s.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLCommand));
					string s_command = string.Empty;
					if (s_command_node != null) {
						s_command = do_decrypt ? Crypto.Decrypt (s_command_node.InnerText, input_password, salt) : s_command_node.InnerText;
					}

					Service s = new Service (do_decrypt ? Crypto.Decrypt (s_name_node.InnerText, input_password, salt) : s_name_node.InnerText, s_command);
					foreach (XmlNode x_n_a in x_n_s.SelectNodes(string.Format("{0}::{1}", XMLDescendant, XMLAccount))) {
						a_count++;
						XmlNode a_name_node = x_n_a.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLName));
						XmlNode a_password_node = x_n_a.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLPassword));
						Account a = new Account (
							            do_decrypt ? Crypto.Decrypt (a_name_node.InnerText, input_password, salt) : a_name_node.InnerText,
							            do_decrypt ? Crypto.Decrypt (a_password_node.InnerText, input_password, salt) : a_password_node.InnerText);
						s.ServiceAccounts.Add (a);
					}

					p.Profileservices.Add (s);
				}

				this.profiles.Add (p);
			}

			// Done, inform user of the time it took to load all data
			load_timer.Stop ();
			this.tray.BalloonTipText = string.Format (Program.Language.LoadStats, p_count, s_count, a_count, load_timer.ElapsedMilliseconds);
			this.tray.ShowBalloonTip (int.MaxValue);

			// and save all data in case data was unencrypted, to encrypt it again
			if (do_recrypt_save) {
				this.DoSave ();
			}
		}

		/*
		* Save all data
		*/
		public void DoSave ()
		{
			// Create the required streams and write the start
			using (StreamWriter s_w = File.CreateText (Program.DataFile)) {
				XmlWriterSettings x_w_s = new XmlWriterSettings ();
				x_w_s.Encoding = Encoding.UTF8;
				XmlWriter x_w = XmlWriter.Create (s_w, x_w_s);
				x_w.WriteStartElement (XMLBody);

				// encryption releated data
				string salt = Password.GenSalt ();
				string password_hash = Crypto.Hash (this.pinCushionPassword.Password, salt);
				if (this.encrypt.Checked) {
					x_w.WriteElementString (XMLencrypt, Password.GenSalt ());
					x_w.WriteElementString (XMLSalt, salt);
					x_w.WriteElementString (XMLPassword, password_hash);
				}

				/*
				 * Main Loop, will also encrypt if specified
				 */
				foreach (Profile p in this.profiles) {
					x_w.WriteStartElement (XMLProfile);
					string p_name = this.encrypt.Checked ? Crypto.Encrypt (p.Name, this.pinCushionPassword.Password, salt) : p.Name;
					x_w.WriteElementString (XMLName, p_name);
					foreach (Service s in p.Profileservices) {
						x_w.WriteStartElement (XMLService);
						string s_name = this.encrypt.Checked ? Crypto.Encrypt (s.Name, this.pinCushionPassword.Password, salt) : s.Name;
						x_w.WriteElementString (XMLName, s_name);
						if (s.Command != string.Empty) {
							string s_command = this.encrypt.Checked ? Crypto.Encrypt (s.Command, this.pinCushionPassword.Password, salt) : s.Command;
							x_w.WriteElementString (XMLCommand, s_command);
						}

						foreach (Account a in s.ServiceAccounts) {
							x_w.WriteStartElement (XMLAccount);
							string a_name = this.encrypt.Checked ? Crypto.Encrypt (a.Name, this.pinCushionPassword.Password, salt) : a.Name;
							string a_password = this.encrypt.Checked ? Crypto.Encrypt (a.Password, this.pinCushionPassword.Password, salt) : a.Password;
							x_w.WriteElementString (XMLName, a_name);
							x_w.WriteElementString (XMLPassword, a_password);
							x_w.WriteEndElement ();
						}

						x_w.WriteEndElement ();
					}

					x_w.WriteEndElement ();
				}

				// Done
				x_w.WriteEndElement ();
				x_w.Close ();
				s_w.Close ();
			}
		}

		/*
		 * This event is raised whenever a different profile is selected.
		 */
		private void ProfileSelection_SelectedIndexChanged (object sender, EventArgs e)
		{
			this.NotIdle ();

			this.RefreshControls (RefreshLevel.Service);
		}

		/*
		 * This event is raised whenever a different service is selected.
		 */
		private void ServiceSelection_SelectedIndexChanged (object sender, EventArgs e)
		{
			this.NotIdle ();

			this.RefreshControls (RefreshLevel.Account);
		}

		/*
		 * This event is raised whenever a different account is selected.
		 */
		private void AccountSelection_SelectedIndexChanged (object sender, EventArgs e)
		{
			this.NotIdle ();

			this.RefreshControls (RefreshLevel.Password);
		}

		/*
		* Add a profile.
		*
		* Steps:
		* - Get a name.
		* - Abort if name is already taken.
		* - Create new instance.
		* - Refresh controls.
		*/
		private void AddProfile_Click (object sender, EventArgs e)
		{
			this.NotIdle ();

			try {
				string input = string.Empty;
				if (this.Inputbox (InputBoxMode.Normal, ref input, Program.Language.AddProfileTitle, Program.Language.AddProfilePrompt) == DialogResult.OK) {
					if (this.profiles.Find (delegate(Profile p) {
						return p.Name == input;
					}) != null) {
						MessageBox.Show (Program.Language.ProfileExists);
					} else {
						this.profiles.Add (new Profile (input));
						this.profiles.Sort (delegate(Profile p, Profile q) {
							return p.Name.CompareTo (q.Name);
						});
						this.DoSave ();
						this.RefreshControls (RefreshLevel.Profile);
						this.profileSelection.SelectedIndex = this.profileSelection.FindStringExact (input, 0);
					}
				}
			} catch (ArgumentOutOfRangeException) {
				MessageBox.Show (Program.Language.AddProfileError);
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		* Remove a profile.
		*
		* Steps:
		* - Remove the profile.
		* - Refresh controls.
		*/
		private void RemoveProfile_Click (object sender, EventArgs e)
		{
			this.NotIdle ();

			try {
				if (MessageBox.Show (Program.Language.ConfirmQuestion, Program.Language.ConfirmCaption, MessageBoxButtons.YesNo) == DialogResult.Yes) {
					this.profiles.Remove (this.profiles [this.profileSelection.SelectedIndex]);
					this.profiles.Sort (delegate(Profile p, Profile q) {
						return p.Name.CompareTo (q.Name);
					});
					this.DoSave ();
					this.RefreshControls (RefreshLevel.Profile);
				}
			} catch (ArgumentOutOfRangeException) {
				MessageBox.Show (Program.Language.RemoveProfileError);
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		* Rename a profile.
		*
		* Steps:
		* - Get a new name.
		* - Abort rename if the new name is either empty or already taken.
		* - Rename.
		* - Refresh controls.
		*/
		private void RenameProfile_Click (object sender, EventArgs e)
		{
			this.NotIdle ();

			try {
				string input = string.Empty;
				if (this.Inputbox (InputBoxMode.Normal, ref input, Program.Language.RenameProfileTitle, Program.Language.RenameProfilePrompt) == DialogResult.OK) {
					if (this.profiles.Find (delegate(Profile p) {
						return p.Name == input;
					}) != null) {
						MessageBox.Show (Program.Language.ProfileExists);
					} else {
						this.profiles [this.profileSelection.SelectedIndex].Name = input;
						this.profiles.Sort (delegate(Profile p, Profile q) {
							return p.Name.CompareTo (q.Name);
						});
						this.DoSave ();
						this.RefreshControls (RefreshLevel.Profile);
						this.profileSelection.SelectedIndex = this.profileSelection.FindStringExact (input, 0);
					}
				}
			} catch (ArgumentOutOfRangeException) {
				MessageBox.Show (Program.Language.RenameProfileError);
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		* Add a new service to the currently selected profile.
		*
		* Steps:
		* - Get a name.
		* - Abort if name is already taken.
		* - Create new instance.
		* - Refresh controls.
		*/
		private void AddService_Click (object sender, EventArgs e)
		{
			this.NotIdle ();

			try {
				string input = string.Empty;
				if (this.Inputbox (InputBoxMode.Normal, ref input, Program.Language.AddServiceTitle, Program.Language.AddServicePrompt) == DialogResult.OK) {
					if (this.profiles [this.profileSelection.SelectedIndex].Profileservices.Find (delegate(Service s) {
						return s.Name == input;
					}) != null) {
						MessageBox.Show (string.Format (Program.Language.ServiceExists, this.profiles [this.profileSelection.SelectedIndex].Name));
					} else {
						this.profiles [this.profileSelection.SelectedIndex].Profileservices.Add (new Service (input, string.Empty));
						this.profiles [this.profileSelection.SelectedIndex].Profileservices.Sort (delegate(Service s, Service t) {
							return s.Name.CompareTo (t.Name);
						});
						this.DoSave ();
						this.RefreshControls (RefreshLevel.Service);
						this.serviceSelection.SelectedIndex = this.serviceSelection.FindStringExact (input, 0);
					}
				}
			} catch (ArgumentOutOfRangeException) {
				MessageBox.Show (Program.Language.AddServiceError);
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		* Remove a service from a profile.
		*
		* Steps:
		* - Remove the service.
		* - Refresh controls.
		*/
		private void RemoveService_Click (object sender, EventArgs e)
		{
			this.NotIdle ();

			try {
				if (MessageBox.Show (Program.Language.ConfirmQuestion, Program.Language.ConfirmCaption, MessageBoxButtons.YesNo) == DialogResult.Yes) {
					this.profiles [this.profileSelection.SelectedIndex].Profileservices.Remove (this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex]);
					this.profiles [this.profileSelection.SelectedIndex].Profileservices.Sort (delegate(Service s, Service t) {
						return s.Name.CompareTo (t.Name);
					});
					this.DoSave ();
					this.RefreshControls (RefreshLevel.Service);
				}
			} catch (ArgumentOutOfRangeException) {
				MessageBox.Show (Program.Language.RemoveServiceError);
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		* Rename the specified service within the specified profile.
		*
		* Steps:
		* - Get a new name.
		* - Ensure the new name is not already taken by another service within that profile and is not empty either.
		* - Rename.
		* - Refresh controls.
		*/
		private void RenameService_Click (object sender, EventArgs e)
		{
			this.NotIdle ();

			try {
				string input = string.Empty;
				if (this.Inputbox (InputBoxMode.Normal, ref input, Program.Language.RenameServiceTitle, Program.Language.RenameServicePrompt) == DialogResult.OK) {
					if (this.profiles [this.profileSelection.SelectedIndex].Profileservices.Find (delegate(Service s) {
						return s.Name == input;
					}) != null) {
						MessageBox.Show (string.Format (Program.Language.ServiceExists, this.profiles [this.profileSelection.SelectedIndex].Name));
					} else {
						this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Name = input;
						this.profiles [this.profileSelection.SelectedIndex].Profileservices.Sort (delegate(Service s, Service t) {
							return s.Name.CompareTo (t.Name);
						});
						this.DoSave ();
						this.RefreshControls (RefreshLevel.Service);
						this.serviceSelection.SelectedIndex = this.serviceSelection.FindStringExact (input, 0);
					}
				}
			} catch (ArgumentOutOfRangeException) {
				MessageBox.Show (Program.Language.RenameServiceError);
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		* Add an account.
		*
		* Steps:
		* - Get a new name and password.
		* - Abort if name is already taken.
		* - Create new instance.
		* - Refresh controls.
		*/
		private void AddAccount_Click (object sender, EventArgs e)
		{
			this.NotIdle ();

			try {
				string account = string.Empty;
				string password = string.Empty;
				if (this.Inputbox (InputBoxMode.Normal, ref account, Program.Language.AddAccountTitle, Program.Language.AddAccountPrompt) == DialogResult.OK) {
					if (this.Inputbox (InputBoxMode.Doublepassword, ref password, Program.Language.AddAccountPasswordTitle, Program.Language.AddAccountPasswordPrompt, Program.Language.AddAccountPasswordConfirmation) == DialogResult.OK) {
						if (this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts.Find (delegate(Account a) {
							return a.Name == account;
						}) != null) {
							MessageBox.Show (string.Format (Program.Language.AccountExists, this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Name, this.profiles [this.profileSelection.SelectedIndex].Name));
						} else {
							this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts.Add (new Account (account, password));
							this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts.Sort (delegate(Account a, Account b) {
								return a.Name.CompareTo (b.Name);
							});
							this.DoSave ();
							this.RefreshControls (RefreshLevel.Account);
							this.accountSelection.SelectedIndex = this.accountSelection.FindStringExact (account, 0);
						}
					}
				}
			} catch (ArgumentOutOfRangeException) {
				MessageBox.Show (Program.Language.AddAccountError);
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		* Remove an account.
		*
		* Steps:
		* - Ask for confirmation.
		* - Remove account.
		* - Refresh controls.
		*/
		private void RemoveAccount_Click (object sender, EventArgs e)
		{
			this.NotIdle ();

			try {
				if (MessageBox.Show (Program.Language.ConfirmQuestion, Program.Language.ConfirmCaption, MessageBoxButtons.YesNo) == DialogResult.Yes) {
					this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts.Remove (this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts [this.accountSelection.SelectedIndex]);
					this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts.Sort (delegate(Account a, Account b) {
						return a.Name.CompareTo (b.Name);
					});
					this.DoSave ();
					this.RefreshControls (RefreshLevel.Account);
				}
			} catch (ArgumentOutOfRangeException) {
				MessageBox.Show (Program.Language.RemoveAccountError);
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		* Rename an account.
		*
		* Steps:
		* - Get a new name.
		* - Ensure the specified name is not already in use for specified profile and service.
		* - Rename.
		* - Refresh controls.
		*/
		private void RenameAccount_Click (object sender, EventArgs e)
		{
			this.NotIdle ();

			try {
				string input = string.Empty;
				if (this.Inputbox (InputBoxMode.Normal, ref input, Program.Language.RenameAccountTitle, Program.Language.RenameAccountPrompt) == DialogResult.OK) {
					if (this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts.Find (delegate(Account a) {
						return a.Name == input;
					}) != null) {
						MessageBox.Show (string.Format (Program.Language.AccountExists, this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Name, this.profiles [this.profileSelection.SelectedIndex].Name));
					} else {
						this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts [this.accountSelection.SelectedIndex].Name = input;
						this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts.Sort (delegate(Account a, Account b) {
							return a.Name.CompareTo (b.Name);
						});
						this.DoSave ();
						this.RefreshControls (RefreshLevel.Account);
						this.accountSelection.SelectedIndex = this.accountSelection.FindStringExact (input, 0);
					}
				}
			} catch (ArgumentOutOfRangeException) {
				MessageBox.Show (Program.Language.RenameAccountError);
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		* Generate a random password but do not store it yet. Just show it.
		*/
		private void GeneratePassword_Click (object sender, EventArgs e)
		{
			this.NotIdle ();

			try {
				string password = Password.Generate (ref this.profiles, this.passwordStrength.Value);
				this.accountPassword.Text = password;
				this.Copy2Clipboard (password);
				this.unsavedPassword = true;
				this.unsavedPasswordIndeces [0] = this.profileSelection.SelectedIndex;
				this.unsavedPasswordIndeces [1] = this.serviceSelection.SelectedIndex;
				this.unsavedPasswordIndeces [2] = this.accountSelection.SelectedIndex;
				this.tray.BalloonTipText = string.Format (Program.Language.TrayGenerateReminder, this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Name);
				this.tray.ShowBalloonTip (int.MaxValue);
			} catch (ArgumentOutOfRangeException) {
				MessageBox.Show (Program.Language.GeneratePasswordError);
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		* Set a manual password.
		*
		* Steps:
		* - Show a dialogue, asking for a new password (uses confirmation)
		* - (or just take the freshly generated password for a new password)
		* - Set the password or just stick it in the text box in case it's an account to be added.
		* - Refresh.
		*/
		private void SetPassword_Click (object sender, EventArgs e)
		{
			// this next call needs to be here to avoid an unnecessary popup (see NotIdle)
			this.unsavedPassword = false;

			this.NotIdle ();

			try {
				if (this.accountPassword.Text == this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts [this.accountSelection.SelectedIndex].Password) {
					/*
					 * Show the popup in case the shown password matches the listed password, implying we want to manually set a new password.
					 */
					string input_password = string.Empty;
					if (this.Inputbox (InputBoxMode.Doublepassword, ref input_password, Program.Language.NewPasswordTitle, Program.Language.NewPasswordPrompt, Program.Language.NewPasswordConfirmation) == DialogResult.OK) {
						string current = this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts [this.accountSelection.SelectedIndex].Name;
						this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts [this.accountSelection.SelectedIndex].Password = input_password;
						this.DoSave ();
						this.RefreshControls (RefreshLevel.Account);
						this.accountSelection.SelectedIndex = this.accountSelection.FindStringExact (current, 0);
					}
				} else {
					/*
					 * Apparently we generated a new password, just set the listed password to whatever was generated
					 */
					string current = this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts [this.accountSelection.SelectedIndex].Name;
					this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts [this.accountSelection.SelectedIndex].Password = this.accountPassword.Text;
					this.DoSave ();
					this.RefreshControls (RefreshLevel.Account);
					this.accountSelection.SelectedIndex = this.accountSelection.FindStringExact (current, 0);
				}
			} catch (ArgumentOutOfRangeException) {
				MessageBox.Show (Program.Language.SetPasswordError);
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		* Refresh all controls.
		*
		* Steps:
		* - Clear the levels below the specified ignore level.
		* - Repopulate first selection below specified ignore level.
		* - Lock underlying levels.
		*/
		private void RefreshControls (RefreshLevel level = RefreshLevel.Profile)
		{
			// Populate controls...
			try {
				switch (level) {
				case RefreshLevel.Profile:
					this.profileSelection.Items.Clear ();
					this.profileSelection.Text = string.Empty;
					this.profileSelection.SelectedIndex = -1;
					this.serviceSelection.Items.Clear ();
					this.serviceSelection.Text = string.Empty;
					this.serviceSelection.SelectedIndex = -1;
					this.accountSelection.Items.Clear ();
					this.accountSelection.Text = string.Empty;
					this.accountSelection.SelectedIndex = -1;
					this.accountPassword.Text = string.Empty;
					foreach (Profile p in this.profiles) {
						this.profileSelection.Items.Add (p.Name);
					}

					this.profileSelection.Select ();
					break;
				case RefreshLevel.Service:
					this.serviceSelection.Items.Clear ();
					this.serviceSelection.Text = string.Empty;
					this.serviceSelection.SelectedIndex = -1;
					this.accountSelection.Items.Clear ();
					this.accountSelection.Text = string.Empty;
					this.accountSelection.SelectedIndex = -1;
					this.accountPassword.Text = string.Empty;
					foreach (Service s in this.profiles[this.profileSelection.SelectedIndex].Profileservices) {
						this.serviceSelection.Items.Add (s.Name);
					}

					this.profileSelection.Select ();
					break;
				case RefreshLevel.Account:
					// Enable/disable the execute rightclick item
					if (this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Command == string.Empty) {
						this.serviceRightclick.Items [0].Text = Program.Language.NoExecute;
						this.serviceRightclick.Items [0].Enabled = false;
					} else {
						this.serviceRightclick.Items [0].Text = string.Format (Program.Language.Execute, this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Command);
						this.serviceRightclick.Items [0].Enabled = true;
					}

					this.accountSelection.Items.Clear ();
					this.accountSelection.Text = string.Empty;
					this.accountSelection.SelectedIndex = -1;
					this.accountPassword.Text = string.Empty;
					foreach (Account a in this.profiles[this.profileSelection.SelectedIndex].Profileservices[this.serviceSelection.SelectedIndex].ServiceAccounts) {
						this.accountSelection.Items.Add (a.Name);
					}

					this.serviceSelection.Select ();
					break;
				case RefreshLevel.Password:
					this.accountPassword.Text = this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts [this.accountSelection.SelectedIndex].Password;
					this.accountSelection.Select ();
					break;
				default:
					break;
				}

				// Enable/Disable/readOnly
				this.addProfile.Enabled = !this.readOnly.Checked;
				this.removeProfile.Enabled = this.profileSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.renameProfile.Enabled = this.profileSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.serviceSelection.Enabled = this.profileSelection.SelectedItem == null ? false : true;
				this.addService.Enabled = this.profileSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.removeService.Enabled = this.serviceSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.renameService.Enabled = this.serviceSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.serviceRightclick.Items [0].Text = this.serviceSelection.SelectedItem == null ? Program.Language.NoExecute : this.serviceRightclick.Items [0].Text;
				this.serviceRightclick.Items [0].Enabled = this.serviceSelection.SelectedItem == null ? false : this.serviceRightclick.Items [0].Enabled;
				this.serviceRightclick.Items [1].Enabled = this.serviceSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.accountSelection.Enabled = this.serviceSelection.SelectedItem == null ? false : true;
				this.accountPassword.TabStop = this.serviceSelection.SelectedItem == null ? false : true;
				this.addAccount.Enabled = this.serviceSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.removeAccount.Enabled = this.accountSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.renameAccount.Enabled = this.accountSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.setPassword.Enabled = this.accountSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.generatePassword.Enabled = this.accountSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.encrypt.Enabled = !this.readOnly.Checked;
				this.passwordStrength.Enabled = this.accountSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.mainFormRightclick.Items [2].Enabled = !this.readOnly.Checked;
				this.setPinCushionPassword.Enabled = !this.readOnly.Checked;
				this.passwordStrengthDescription.Text = Password.PasswordLength [this.passwordStrength.Value].ToString ();
				this.passwordStrengthDescription.Text += " aA0";
				this.copyTextToClipboardRightclick.Items [0].Enabled = this.accountSelection.SelectedItem == null ? false : true;
				if (this.passwordStrength.Value > 1) {
					this.passwordStrengthDescription.Text += "!";
				}

				if (this.passwordStrength.Value > 5) {
					this.passwordStrengthDescription.Text += "#";
				}

				// The main form's title...
				if (this.profileSelection.SelectedItem != null) {
					this.Text = string.Format ("{0} v{1} | {2}", System.Windows.Forms.Application.ProductName, System.Windows.Forms.Application.ProductVersion, string.Format (Program.Language.Stats, this.profiles [this.profileSelection.SelectedIndex].Name, this.profiles [this.profileSelection.SelectedIndex].Profileservices.Count));
				} else {
					this.Text = string.Format ("{0} v{1}", System.Windows.Forms.Application.ProductName, System.Windows.Forms.Application.ProductVersion);
				}
			} catch (ArgumentOutOfRangeException) {
				// If for whatever reason we end up here, just try again but this time... from the top.
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		* This event is raised whenever the read only checkbox is changed.
		*
		* Refresh controls if this happens.
		*/
		private void ReadOnly_CheckedChanged (object sender, EventArgs e)
		{
			this.NotIdle ();

			this.RefreshControls (RefreshLevel.None);
		}

		/*
		* Load the data.
		*
		* Steps:
		* - Load all the data into an XML document, but only if it exists.
		* - Request the password if the data is encrypted.
		* - Loop through all child profiles.
		*      - Loop through all child services of each profile.
		*          - Loop through all child accounts of each service.
		* - Refresh controls.
		*/
		private void MainForm_Load (object sender, EventArgs e)
		{
			// Start by localizing the UI
			this.Localize ();

			// Load the data but only if there actually is data to load
			if (File.Exists (Program.DataFile)) {
				try {
					this.DoLoad (Program.DataFile);
				} catch (PinCushionException ex) {
					// We end up here in case of data corruption or in case an incorrect password was specified.
					MessageBox.Show (ex.Message);
					Program.PinCushionExit ();
				} catch (XmlException) {
					// Most likely merely an empty XML file.
				}
			} else {
				// First run, let's grab a password for PinCushion before proceeding.
				string input_password = string.Empty;
				if (this.Inputbox (InputBoxMode.Doublepassword, ref input_password, string.Format (Program.Language.FirstRunTitle, System.Windows.Forms.Application.ProductName), string.Format (Program.Language.FirstRunPrompt, System.Windows.Forms.Application.ProductName), Program.Language.PinCushionSaveConfirmation) == DialogResult.Cancel) {
					Program.PinCushionExit ();
				}

				this.pinCushionPassword.Password = input_password;
			}

			// Done
			this.RefreshControls (RefreshLevel.Profile);
			this.readOnly.Checked = this.profiles.Count == 0 ? false : true;
		}

		/*
		* Keeps track of idle time; using Application.Idle will not work reliably because of MessageBoxes.
		*/
		private void IdleTimer_Tick (object sender, EventArgs e)
		{
			// Check for maximum idle time, quit if it expired...
			if (!this.notimeout && DateTime.Now >= this.timeout) {
				/*
				 * The call to NotIdle is just here to inform the user of an unsaved password.
				 * See NotIdle();
				 */
				this.NotIdle ();
				Program.PinCushionExit ();
			}

			// Clear the clipboard but only if we placed something on it.
			if (Program.ClipboardClearEnabled && DateTime.Now >= this.clipboardTimeout) {
				#if BuildForMono
				((Gtk.Clipboard)Gtk.Clipboard.Get (Gdk.Selection.Clipboard)).Clear ();
				((Gtk.Clipboard)Gtk.Clipboard.Get (Gdk.Selection.Clipboard)).Store ();
				#else
				System.Windows.Forms.Clipboard.Clear();
				#endif
				Program.ClipboardClearEnabled = false;
			}
		}

		/*
		* Not idle
		*
		* Used by several controls and in various other places to reset the idle timer.
		*/
		private void NotIdle (object sender = null, EventArgs e = null)
		{
			this.timeout = DateTime.Now.AddSeconds (MaxIdle);

			/*
			 * After several trials, I have found that this is the best spot to have this check
			 * for an unsaved password; this function gets called before any other action
			 * such as data manipulation takes place. Hence, it is the safest spot.
			 */
			if (this.unsavedPassword) {
				if (MessageBox.Show (string.Format (Program.Language.UnsavedPasswordPrompt, this.profiles [this.unsavedPasswordIndeces [0]].Profileservices [this.unsavedPasswordIndeces [1]].Name), Program.Language.UnsavedPasswordTitle, MessageBoxButtons.YesNo) == DialogResult.Yes) {
					this.profiles [this.unsavedPasswordIndeces [0]].Profileservices [this.unsavedPasswordIndeces [1]].ServiceAccounts [this.unsavedPasswordIndeces [2]].Password = this.accountPassword.Text;
					this.DoSave ();
				}
			}

			this.unsavedPassword = false;
		}

		/*
		* Localize the UI
		*/
		private void Localize ()
		{
			this.accountLabel.Text = Program.Language.Account;
			this.serviceLabel.Text = Program.Language.Service;
			this.profileLabel.Text = Program.Language.Profile;
			this.passwordLabel.Text = Program.Language.Password;
			this.addProfile.Text = this.addService.Text = this.addAccount.Text = Program.Language.Add;
			this.removeProfile.Text = this.removeService.Text = this.removeAccount.Text = Program.Language.Remove;
			this.renameProfile.Text = this.renameService.Text = this.renameAccount.Text = Program.Language.Rename;
			this.serviceRightclick.Items [0].Text = Program.Language.NoExecute;
			this.serviceRightclick.Items [1].Text = Program.Language.SetExecute;
			this.setPassword.Text = Program.Language.Set;
			this.setPinCushionPassword.Text = string.Format (Program.Language.PinCushionPassword, System.Windows.Forms.Application.ProductName);
			this.generatePassword.Text = Program.Language.Generate;
			this.readOnly.Text = Program.Language.ReadOnly;
			this.encrypt.Text = Program.Language.Encrypt;
			this.copyTextToClipboardRightclick.Items [0].Text = Program.Language.CopyToClipboard;
			this.showPassword.Text = Program.Language.ShowPassword;
			this.passwordStrengthLabel.Text = Program.Language.PasswordStrength;
			this.mainFormRightclick.Items [0].Text = Program.Language.DisableIdleTimeout;
			this.mainFormRightclick.Items [1].Text = Program.Language.Language;
			this.mainFormRightclick.Items [2].Text = Program.Language.Import;
		}

		/*
		* Copy the control's text to the clipboard...
		*/
		private void CopyTextToClipboardToolStripMenuItem_Click (object sender, EventArgs e)
		{
			this.NotIdle ();

			this.Copy2Clipboard (((Control)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl).Text);
		}

		/*
		* Toggle the password characters
		*/
		private void ShowPassword_CheckedChanged (object sender, EventArgs e)
		{
			this.NotIdle ();

			this.accountPassword.UseSystemPasswordChar = !this.showPassword.Checked;
		}

		/*
		* Toggle the timeout
		*/
		private void DisableIdleTimeoutToolStripMenuItem_Click (object sender, EventArgs e)
		{
			this.NotIdle ();

			this.notimeout = !this.notimeout;
			((ToolStripMenuItem)this.mainFormRightclick.Items [0]).Checked = this.notimeout;
		}

		/*
		* Execute the specified command for the selected service
		*/
		private void ExecuteToolStripMenuItem_Click (object sender, EventArgs e)
		{
			this.NotIdle ();

			try {
				if (this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Command != string.Empty) {
					Process.Start (this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Command);
				}
			} catch (ArgumentOutOfRangeException) {
				MessageBox.Show (Program.Language.RunExecuteError);
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		* Set the command to execute for the given service
		*/
		private void SetexecuteToolStripMenuItem_Click (object sender, EventArgs e)
		{
			this.NotIdle ();

			try {
				string command = string.Empty;
				if (this.Inputbox (InputBoxMode.Normal, ref command, Program.Language.SetExecuteTitle, Program.Language.SetExecutePrompt) == DialogResult.OK) {
					this.profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Command = command;
					this.DoSave ();
					this.RefreshControls (RefreshLevel.Account);
				}
			} catch (ArgumentOutOfRangeException) {
				MessageBox.Show (Program.Language.SetExecuteError);
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		* Change the language
		*/
		private void ChangeLanguage (object sender, EventArgs e)
		{
			this.NotIdle ();

			Program.Language.LoadLocalization (sender.ToString ());
			foreach (ToolStripMenuItem t in ((ToolStripMenuItem)this.mainFormRightclick.Items[1]).DropDownItems) {
				t.Checked = false;
			}

			((ToolStripMenuItem)sender).Checked = true;
			this.Localize ();
		}

		/*
		* Import a data file
		*/
		private void ImportToolStripMenuItem_Click (object sender, EventArgs e)
		{
			this.NotIdle ();

			OpenFileDialog f = new OpenFileDialog ();
			f.Filter = string.Format ("{0}|{1}", System.Windows.Forms.Application.ProductName, Program.ImportFilter);
			if (f.ShowDialog () == DialogResult.OK) {
				try {
					this.DoLoad (f.FileName, true);
					this.DoSave ();
					this.RefreshControls (RefreshLevel.Profile);
				} catch (PinCushionException ex) {
					MessageBox.Show (ex.Message);
				} catch (XmlException) {
					// Most likely an empty XML file, just ignore.
				}
			}
		}

		/*
		* Manually set a new password for PinCushion
		*/
		private void SetPinCushionPassword_Click (object sender, EventArgs e)
		{
			this.NotIdle ();

			string input_password = string.Empty;
			if (this.Inputbox (InputBoxMode.Doublepassword, ref input_password, string.Format (Program.Language.PinCushionReencryptTitle, System.Windows.Forms.Application.ProductName), string.Format (Program.Language.PinCushionReencryptPrompt, System.Windows.Forms.Application.ProductName), Program.Language.PinCushionReencryptConfirmation) == DialogResult.OK) {
				this.pinCushionPassword.Password = input_password;
				this.DoSave ();
			}
		}

		/*
		* Copy something to the clipboard and activate the timeout
		*/
		private void Copy2Clipboard (string message)
		{
			#if BuildForMono
			((Gtk.Clipboard)Gtk.Clipboard.Get (Gdk.Selection.Clipboard)).Text = message;
			((Gtk.Clipboard)Gtk.Clipboard.Get (Gdk.Selection.Clipboard)).Store ();
			#else
			System.Windows.Forms.Clipboard.SetText(message);
			#endif
			Program.ClipboardClearEnabled = true;
			this.clipboardTimeout = DateTime.Now.AddSeconds (CCTime);
		}

		/*
		* Update the password strength description
		*/
		private void PasswordStrength_ValueChanged (object sender, EventArgs e)
		{
			this.NotIdle ();

			this.RefreshControls (RefreshLevel.None);
		}
	}
}