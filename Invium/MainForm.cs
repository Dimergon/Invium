/*
* Invium, a password manager in C#
* Copyright (c) 2013, 2014 Armin Altorffer
*
* This file is part of Invium.
*
* Invium is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
*
* Invium is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with Invium.  If not, see <http://www.gnu.org/licenses/>.
*/

/*
* Main UI of Invium, houses most of its actual functionality.
*/
namespace Invium
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Threading;
	using System.Windows.Forms;
	using System.Xml;

	#if BuildForMono
	using Gtk;
	#endif

	public partial class MainForm : Form
	{
		/* 
		 * Quit after Max_Idle seconds when idle.
		 * Clear clipboard after CC_Time seconds.
		 * Autosave (if applicable) after (MaxIdle - AutoSave) seconds of idle.
		 */
		private const short MaxIdle = 300;
		private const short CCTime = 120;

		// Used in the automatic timeout functionality, Invium will quit after a certain period of inactivity.
		private DateTime timeout = DateTime.Now.AddSeconds (MaxIdle);
		private bool notimeout = false;

		// Used to clear the clipboard after copying a password
		private DateTime clipboardTimeout = DateTime.Now.AddSeconds (CCTime);

		// Make sure we save in case there is data to save
		private bool saveOnClose = false;

		// Heartbeat
		private Thread heartbeat;
		// Heartbeat lock -- used to interrupt/block ProcessTick()
		// this is done to avoid closing the program when idle in case
		// of very large data files (taking more than 300 seconds to load or save).
		private object heartbeatlock = new object ();

		public MainForm ()
		{
			this.InitializeComponent ();

			// Initialize the tray icon
			this.tray.BalloonTipTitle = this.Text = string.Format ("{0} v{1}", Program.ProductName, Program.ProductVersion);
			this.tray.BalloonTipIcon = ToolTipIcon.Info;

			/*
			 * Main form right click menu items...
			 */

			// disable timeout
			((ToolStripMenuItem)this.mainFormCM.Items [0]).Checked = this.notimeout;

			// languages
			foreach (string s in Program.Language.Traylangs) {
				((ToolStripMenuItem)this.mainFormCM.Items [1]).DropDownItems.Add (s, null, this.ChangeLanguage);
			}

			// Mark the active language
			foreach (ToolStripMenuItem t in ((ToolStripMenuItem)this.mainFormCM.Items[1]).DropDownItems) {
				if (t.Text == Program.Language.Trayfeedback) {
					t.Checked = true;
				}
			}

			// import functionality
			((ToolStripMenuItem)this.mainFormCM.Items [2]).Enabled = false;

			// merge functionality
			((ToolStripMenuItem)this.mainFormCM.Items [3]).Enabled = false;

			// Create a pulse.
			this.heartbeat = new Thread (new ThreadStart (delegate() {
				while (true) {
					this.ProcessTick ();
					Thread.Sleep (1000);
				}
			}));
			this.heartbeat.IsBackground = true;
			this.heartbeat.Start ();
			while (!this.heartbeat.IsAlive) {
			}
		}

		// Used in refreshing
		private enum RefreshLevel
		{
			None,
			Account,
			Service,
			Profile
		}

		/*
		 * Capture key combinations
		 */
		protected override bool ProcessCmdKey (ref Message msg, Keys keydata)
		{
			switch (keydata) {
			case Keys.Control | Keys.Q:
			case Keys.Control | Keys.Shift | Keys.W:
			case Keys.Alt | Keys.F4:
			case Keys.Control | Keys.Shift | Keys.X:
				// Some common Quit combinations
				this.Close ();
				return true;
			case Keys.Control | Keys.R:
				// Run the command of the selected service
				this.ExecuteCommand ();
				return true;
			case Keys.Control | Keys.Shift | Keys.S:
				// Hard save
				this.NotIdle ();
				new Storage ().DoSave (this.encrypt.Checked, this.heartbeatlock);
				this.saveOnClose = false;
				return true;
			case Keys.Control | Keys.C:
				// Copy contents of current control to clipboard
				this.NotIdle ();
				this.Copy2Clipboard (this.ActiveControl.Text);
				return true;
			default:
				// anything else gets processed normally.
				return base.ProcessCmdKey (ref msg, keydata);
			}
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
		private void AddProfile ()
		{
			this.NotIdle ();

			try {
				string userinput = string.Empty;
				if (new InputBox ().ShowMe (InputBox.Mode.Normal, ref userinput, Program.Language.AddProfileTitle, Program.Language.AddProfilePrompt) == DialogResult.OK) {
					if (Program.Profiles.Find (delegate(Profile p) {
						return p.Name == userinput;
					}) != null) {
						MessageBox.Show (Program.Language.ProfileExists);
					} else {
						Program.Profiles.Add (new Profile (userinput));
						Program.Profiles.Sort (delegate(Profile p, Profile q) {
							return p.Name.CompareTo (q.Name);
						});
						this.saveOnClose = true;
						this.RefreshControls (RefreshLevel.Profile);
						this.profileSelection.SelectedIndex = this.profileSelection.FindStringExact (userinput, 0);
						userinput = string.Empty;
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
		private void RemoveProfile ()
		{
			this.NotIdle ();

			try {
				if (MessageBox.Show (Program.Language.ConfirmQuestion, Program.Language.ConfirmCaption, MessageBoxButtons.YesNo) == DialogResult.Yes) {
					Program.Profiles.Remove (Program.Profiles [this.profileSelection.SelectedIndex]);
					Program.Profiles.Sort (delegate(Profile p, Profile q) {
						return p.Name.CompareTo (q.Name);
					});
					this.saveOnClose = true;
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
		private void RenameProfile ()
		{
			this.NotIdle ();

			try {
				string userinput = string.Empty;
				if (new InputBox ().ShowMe (InputBox.Mode.Normal, ref userinput, Program.Language.RenameProfileTitle, Program.Language.RenameProfilePrompt) == DialogResult.OK) {
					if (Program.Profiles.Find (delegate(Profile p) {
						return p.Name == userinput;
					}) != null) {
						MessageBox.Show (Program.Language.ProfileExists);
					} else {
						Program.Profiles [this.profileSelection.SelectedIndex].Name = userinput;
						Program.Profiles.Sort (delegate(Profile p, Profile q) {
							return p.Name.CompareTo (q.Name);
						});
						this.saveOnClose = true;
						this.RefreshControls (RefreshLevel.Profile);
						this.profileSelection.SelectedIndex = this.profileSelection.FindStringExact (userinput, 0);
						userinput = string.Empty;
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
		private void AddService ()
		{
			this.NotIdle ();

			try {
				string userinput = string.Empty;
				if (new InputBox ().ShowMe (InputBox.Mode.Normal, ref userinput, Program.Language.AddServiceTitle, Program.Language.AddServicePrompt) == DialogResult.OK) {
					if (Program.Profiles [this.profileSelection.SelectedIndex].Profileservices.Find (delegate(Service s) {
						return s.Name == userinput;
					}) != null) {
						MessageBox.Show (string.Format (Program.Language.ServiceExists, Program.Profiles [this.profileSelection.SelectedIndex].Name));
					} else {
						Program.Profiles [this.profileSelection.SelectedIndex].Profileservices.Add (new Service (userinput, string.Empty));
						Program.Profiles [this.profileSelection.SelectedIndex].Profileservices.Sort (delegate(Service s, Service t) {
							return s.Name.CompareTo (t.Name);
						});
						this.saveOnClose = true;
						this.RefreshControls (RefreshLevel.Service);
						this.serviceSelection.SelectedIndex = this.serviceSelection.FindStringExact (userinput, 0);
						userinput = string.Empty;
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
		private void RemoveService ()
		{
			this.NotIdle ();

			try {
				if (MessageBox.Show (Program.Language.ConfirmQuestion, Program.Language.ConfirmCaption, MessageBoxButtons.YesNo) == DialogResult.Yes) {
					Program.Profiles [this.profileSelection.SelectedIndex].Profileservices.Remove (Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex]);
					Program.Profiles [this.profileSelection.SelectedIndex].Profileservices.Sort (delegate(Service s, Service t) {
						return s.Name.CompareTo (t.Name);
					});
					this.saveOnClose = true;
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
		private void RenameService ()
		{
			this.NotIdle ();

			try {
				string userinput = string.Empty;
				if (new InputBox ().ShowMe (InputBox.Mode.Normal, ref userinput, Program.Language.RenameServiceTitle, Program.Language.RenameServicePrompt) == DialogResult.OK) {
					if (Program.Profiles [this.profileSelection.SelectedIndex].Profileservices.Find (delegate(Service s) {
						return s.Name == userinput;
					}) != null) {
						MessageBox.Show (string.Format (Program.Language.ServiceExists, Program.Profiles [this.profileSelection.SelectedIndex].Name));
					} else {
						Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Name = userinput;
						Program.Profiles [this.profileSelection.SelectedIndex].Profileservices.Sort (delegate(Service s, Service t) {
							return s.Name.CompareTo (t.Name);
						});
						this.saveOnClose = true;
						this.RefreshControls (RefreshLevel.Service);
						this.serviceSelection.SelectedIndex = this.serviceSelection.FindStringExact (userinput, 0);
						userinput = string.Empty;
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
		private void AddAccount ()
		{
			this.NotIdle ();

			try {
				string account = string.Empty;
				string password = string.Empty;
				if (new InputBox ().ShowMe (InputBox.Mode.Normal, ref account, Program.Language.AddAccountTitle, Program.Language.AddAccountPrompt) == DialogResult.OK) {
					if (new InputBox ().ShowMe (InputBox.Mode.Doublepassword, ref password, Program.Language.AddAccountPasswordTitle, Program.Language.AddAccountPasswordPrompt, Program.Language.ConfirmPassword) == DialogResult.OK) {
						if (Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts.Find (delegate(Account a) {
							return a.Name == account;
						}) != null) {
							MessageBox.Show (string.Format (Program.Language.AccountExists, Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Name, Program.Profiles [this.profileSelection.SelectedIndex].Name));
						} else {
							Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts.Add (new Account (account, password));
							Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts.Sort (delegate(Account a, Account b) {
								return a.Name.CompareTo (b.Name);
							});
							this.saveOnClose = true;
							this.RefreshControls (RefreshLevel.Account);
							this.accountSelection.SelectedIndex = this.accountSelection.FindStringExact (account, 0);
							account = password = string.Empty;
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
		private void RemoveAccount ()
		{
			this.NotIdle ();

			try {
				if (MessageBox.Show (Program.Language.ConfirmQuestion, Program.Language.ConfirmCaption, MessageBoxButtons.YesNo) == DialogResult.Yes) {
					Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts.Remove (Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts [this.accountSelection.SelectedIndex]);
					Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts.Sort (delegate(Account a, Account b) {
						return a.Name.CompareTo (b.Name);
					});
					this.saveOnClose = true;
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
		private void RenameAccount ()
		{
			this.NotIdle ();

			try {
				string userinput = string.Empty;
				if (new InputBox ().ShowMe (InputBox.Mode.Normal, ref userinput, Program.Language.RenameAccountTitle, Program.Language.RenameAccountPrompt) == DialogResult.OK) {
					if (Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts.Find (delegate(Account a) {
						return a.Name == userinput;
					}) != null) {
						MessageBox.Show (string.Format (Program.Language.AccountExists, Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Name, Program.Profiles [this.profileSelection.SelectedIndex].Name));
					} else {
						Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts [this.accountSelection.SelectedIndex].Name = userinput;
						Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts.Sort (delegate(Account a, Account b) {
							return a.Name.CompareTo (b.Name);
						});
						this.saveOnClose = true;
						this.RefreshControls (RefreshLevel.Account);
						this.accountSelection.SelectedIndex = this.accountSelection.FindStringExact (userinput, 0);
						userinput = string.Empty;
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
		private void GeneratePassword ()
		{
			this.NotIdle ();

			try {
				string password = new Password ().Generate (ref Program.Profiles, this.passwordStrength.Value);
				this.accountPassword.Text = password;
				this.Copy2Clipboard (password);
				this.tray.BalloonTipText = string.Format (Program.Language.TrayGenerateReminder, Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Name);
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
		private void SetPassword ()
		{
			this.NotIdle ();

			try {
				if (this.accountPassword.Text == Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts [this.accountSelection.SelectedIndex].Password) {
					/*
					 * Show the popup in case the shown password matches the listed password, implying we want to manually set a new password.
					 */
					string userinput = string.Empty;
					if (new InputBox ().ShowMe (InputBox.Mode.Doublepassword, ref userinput, Program.Language.NewPasswordTitle, Program.Language.NewPasswordPrompt, Program.Language.NewPasswordConfirmation) == DialogResult.OK) {
						string current = Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts [this.accountSelection.SelectedIndex].Name;
						Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts [this.accountSelection.SelectedIndex].Password = userinput;
						this.saveOnClose = true;
						this.RefreshControls (RefreshLevel.Account);
						this.accountSelection.SelectedIndex = this.accountSelection.FindStringExact (current, 0);
						userinput = string.Empty;
					}
				} else {
					/*
					 * Apparently we generated a new password, just set the listed password to whatever was generated
					 */
					string current = Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts [this.accountSelection.SelectedIndex].Name;
					Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts [this.accountSelection.SelectedIndex].Password = this.accountPassword.Text;
					this.saveOnClose = true;
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

					this.profileSelection.Items.AddRange (Program.Profiles.ConvertAll<string> (delegate (Profile x) {
						return x.Name;
					}).ToArray ());

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

					this.serviceSelection.Items.AddRange (Program.Profiles [this.profileSelection.SelectedIndex].Profileservices.ConvertAll<string> (delegate (Service x) {
						return x.Name;
					}).ToArray ());

					this.profileSelection.Select ();
					break;
				case RefreshLevel.Account:
					this.accountSelection.Items.Clear ();
					this.accountSelection.Text = string.Empty;
					this.accountSelection.SelectedIndex = -1;
					this.accountPassword.Text = string.Empty;

					this.accountSelection.Items.AddRange (Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts.ConvertAll<string> (delegate (Account x) {
						return x.Name;
					}).ToArray ());

					this.serviceSelection.Select ();
					break;
				default:
					break;
				}

				// Service context menu
				if (this.serviceSelection.SelectedItem != null) {
					this.serviceCM.Items [0].Text = Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Command == string.Empty ? Program.Language.NoExecute : Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Command;
					this.serviceCM.Items [0].Enabled = Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Command == string.Empty ? false : true;
				}

				// Enable/Disable/readOnly
				this.addProfile.Enabled = !this.readOnly.Checked;
				this.removeProfile.Enabled = this.profileSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.renameProfile.Enabled = this.profileSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.serviceSelection.Enabled = this.profileSelection.SelectedItem == null ? false : true;
				this.addService.Enabled = this.profileSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.removeService.Enabled = this.serviceSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.renameService.Enabled = this.serviceSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.serviceCM.Items [1].Enabled = this.serviceSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.serviceCM.Items [2].Enabled = this.serviceSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.accountSelection.Enabled = this.serviceSelection.SelectedItem == null ? false : true;
				this.addAccount.Enabled = this.serviceSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.removeAccount.Enabled = this.accountSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.renameAccount.Enabled = this.accountSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.setPassword.Enabled = this.accountSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.generatePassword.Enabled = this.accountSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.passwordStrength.Enabled = this.accountSelection.SelectedItem == null ? false : !this.readOnly.Checked;
				this.encrypt.Enabled = !this.readOnly.Checked;
				this.mainFormCM.Items [2].Enabled = !this.readOnly.Checked;
				this.mainFormCM.Items [3].Enabled = !this.readOnly.Checked;
				this.setMasterPassword.Enabled = !this.readOnly.Checked;
				if (this.profileSelection.SelectedItem != null) {
					this.Text = string.Format ("{0} v{1} | {2}", Program.ProductName, Program.ProductVersion, string.Format (Program.Language.Stats, Program.Profiles [this.profileSelection.SelectedIndex].Name, Program.Profiles [this.profileSelection.SelectedIndex].Profileservices.Count));
				} else {
					this.Text = string.Format ("{0} v{1}", Program.ProductName, Program.ProductVersion);
				}
			} catch (ArgumentOutOfRangeException) {
				// If for whatever reason we end up here, just try again but this time... from the top.
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		* Load the data, if it exists.
		*/
		private void MainForm_Load (object sender, EventArgs e)
		{
			// Start by localizing the UI
			this.Localize ();

			// Load the data but only if there actually is data to load
			if (File.Exists (Program.DataFile)) {
				try {
					new Storage ().DoLoad (Program.DataFile, this.heartbeatlock, false);
				} catch (InviumException ex) {
					// We end up here in case of data corruption or in case an incorrect password was specified.
					MessageBox.Show (ex.Message);
					Program.InviumExit ();
				} catch (XmlException) {
					// Most likely merely an empty XML file.
				}
			} else {
				// First run, let's grab a password for Invium before proceeding.
				string input_password = string.Empty;
				if (new InputBox ().ShowMe (InputBox.Mode.Doublepassword, ref input_password, Program.Language.FirstRunTitle, Program.Language.FirstRunPrompt, Program.Language.ConfirmPassword) == DialogResult.Cancel) {
					Program.InviumExit ();
				}

				Program.MasterPassword.Password = input_password;
				input_password = string.Empty;
			}

			// Done
			this.RefreshControls (RefreshLevel.Profile);
			this.readOnly.Checked = Program.Profiles.Count == 0 ? false : true;
			this.readOnly.Enabled = !Program.ForcedReadOnly;
		}

		/*
		* Process the heartbeat.
		*/
		private void ProcessTick ()
		{
			lock (this.heartbeatlock) {
				// Check for maximum idle time, quit if it expired...
				if (!this.notimeout && DateTime.Now >= this.timeout) {
					this.Close ();
				}

				// Clear the clipboard but only if we placed something on it.
				if (Program.ClipboardClearEnabled && DateTime.Now >= this.clipboardTimeout) {
					#if BuildForMono
					((Gtk.Clipboard)Gtk.Clipboard.Get (Gdk.Selection.Clipboard)).Clear ();
					((Gtk.Clipboard)Gtk.Clipboard.Get (Gdk.Selection.Clipboard)).Store ();
					#else
					Clipboard.Clear ();
					#endif
					Program.ClipboardClearEnabled = false;
				}
			}
		}

		/*
		* Not idle
		*
		* Used by several controls and in various other places to reset the idle timer.
		*/
		private void NotIdle ()
		{
			this.timeout = DateTime.Now.AddSeconds (MaxIdle);
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
			this.serviceCM.Items [0].Text = Program.Language.NoExecute;
			this.serviceCM.Items [1].Text = Program.Language.SetExecute;
			this.serviceCM.Items [2].Text = Program.Language.CloneService;
			this.setPassword.Text = Program.Language.Set;
			this.setMasterPassword.Text = Program.Language.MasterPassword;
			this.generatePassword.Text = Program.Language.Generate;
			this.readOnly.Text = Program.Language.ReadOnly;
			this.encrypt.Text = Program.Language.Encrypt;
			this.copyTextCM.Items [0].Text = Program.Language.CopyToClipboard;
			this.showPassword.Text = Program.Language.ShowPassword;
			this.passwordStrengthLabel.Text = Program.Language.PasswordStrength;
			this.mainFormCM.Items [0].Text = Program.Language.DisableIdleTimeout;
			this.mainFormCM.Items [1].Text = Program.Language.Language;
			this.mainFormCM.Items [2].Text = Program.Language.Import;
			this.mainFormCM.Items [3].Text = Program.Language.ProfileMerge;
		}

		/*
		* Execute the specified command for the selected service
		*/
		private void ExecuteCommand ()
		{
			this.NotIdle ();

			try {
				if (Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Command != string.Empty) {
					Process.Start (Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Command);
				}
			} catch (ArgumentOutOfRangeException) {
				MessageBox.Show (Program.Language.RunExecuteError);
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		* Set the command to execute for the given service
		*/
		private void SetCommand ()
		{
			this.NotIdle ();

			try {
				string userinput = string.Empty;
				if (new InputBox ().ShowMe (InputBox.Mode.Normal, ref userinput, Program.Language.SetExecuteTitle, Program.Language.SetExecutePrompt) == DialogResult.OK) {
					Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Command = userinput;
					this.saveOnClose = true;
					int currentAccount = this.accountSelection.SelectedIndex;
					this.RefreshControls (RefreshLevel.Account);
					this.accountSelection.SelectedIndex = currentAccount;
					userinput = string.Empty;
				}
			} catch (ArgumentOutOfRangeException) {
				MessageBox.Show (Program.Language.SetExecuteError);
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		 * Clone a service to a different profile
		 *
		 * Steps:
		 * - Get a destination profile from the user
		 * - Check its existence
		 * - Loop through all accounts of the source service
		 * - Add them to a new service
		 * - Add this new service to the destination profile
		 * - (under a new name if destination profile already had a service of the same name)
		 */
		private void CloneService ()
		{
			this.NotIdle ();

			try {
				string userinput = string.Empty;

				if (new InputBox ().ShowMe (InputBox.Mode.Normal, ref userinput, Program.Language.CloneServiceTitle, string.Format (Program.Language.CloneServicePrompt, Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Name)) == DialogResult.OK) {
					int destination_profile = Program.Profiles.FindIndex (delegate (Profile p) {
						return p.Name == userinput;
					});
					if (destination_profile == -1) {
						MessageBox.Show (Program.Language.CloneServiceNoSuchProfile);
					} else {
						bool renamed = false;
						string destination_service = Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Name;
						if (Program.Profiles [destination_profile].Profileservices.Find (delegate (Service s) {
							return s.Name == destination_service;
						}) != null) {
							renamed = true;
							destination_service += new Password ().GenSalt ();
						}

						Service new_service = new Service (destination_service, Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Command);
						foreach (Account a in Program.Profiles[this.profileSelection.SelectedIndex].Profileservices[this.serviceSelection.SelectedIndex].ServiceAccounts) {
							new_service.ServiceAccounts.Add (new Account (a.Name, a.Password));
						}

						Program.Profiles [destination_profile].Profileservices.Add (new_service);
						Program.Profiles [destination_profile].Profileservices.Sort (delegate (Service a, Service b) {
							return a.Name.CompareTo (b.Name);
						});

						this.saveOnClose = true;
						this.RefreshControls (RefreshLevel.Service);
						userinput = string.Empty;
						if (renamed) {
							MessageBox.Show (string.Format (Program.Language.CloneServiceRename, Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].Name, Program.Profiles [destination_profile].Name, destination_service));
						} else {
							MessageBox.Show (string.Format (Program.Language.CloneServiceDone, Program.Profiles [destination_profile].Name, destination_service));
						}
					}
				}
			} catch (ArgumentOutOfRangeException) {
				MessageBox.Show (Program.Language.CloneServiceError);
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		 * Merge profiles
		 *
		 * Steps:
		 * - Get a list of profiles to merge, from the user.
		 * - Check existence of all of them, abort if not all exist.
		 * - Get a destination profile name from the user.
		 * - Check existence, abort if it exists.
		 * - Create new profile instance.
		 * - Loop through all specified profiles, copy their services/accounts to new instances.
		 * - (rename the service in case of duplicates)
		 * - Add all these new instances to the new profile instance.
		 * - Add new profile instance to main container.
		 */
		private void MergeProfiles ()
		{
			this.NotIdle ();

			try {
				List<int> profilestocopy = new List<int> ();
				string userinput_source = string.Empty;
				string userinput_destination = string.Empty;

				if (new InputBox ().ShowMe (InputBox.Mode.Normal, ref userinput_source, Program.Language.MergeProfilesSourceTitle, Program.Language.MergeProfilesSourcePrompt) == DialogResult.OK) {
					if (new InputBox ().ShowMe (InputBox.Mode.Normal, ref userinput_destination, Program.Language.MergeProfilesDestinationTitle, Program.Language.MergeProfilesDestinationPrompt) == DialogResult.OK) {
						if (Program.Profiles.Find (x => x.Name == userinput_destination) != null) {
							MessageBox.Show (Program.Language.MergeProfilesDestinationExists);
							return;
						}

						foreach (string s in userinput_source.Split (new char[] { ',' })) {
							if (Program.Profiles.Find (x => x.Name == s.Trim ()) == null) {
								MessageBox.Show (Program.Language.MergeProfilesSourceDoesNotExist);
								return;
							} else {
								profilestocopy.Add (Program.Profiles.FindIndex (x => x.Name == s.Trim ()));
							}
						}

						bool renamed = false;
						Profile newprofile = new Profile (userinput_destination);
						foreach (int i in profilestocopy) {
							foreach (Service s in Program.Profiles[i].Profileservices) {
								string service_name = s.Name;
								if (newprofile.Profileservices.Find (x => x.Name == s.Name) != null) {
									renamed = true;
									service_name += new Password ().GenSalt ();
								}

								Service newservice = new Service (service_name, s.Command);
								foreach (Account a in s.ServiceAccounts) {
									newservice.ServiceAccounts.Add (new Account (a.Name, a.Password));
								}

								newprofile.Profileservices.Add (newservice);
							}
						}

						newprofile.Profileservices.Sort (delegate(Service s, Service t) {
							return s.Name.CompareTo (t.Name);
						});
						Program.Profiles.Add (newprofile);
						this.RefreshControls (RefreshLevel.Profile);
						this.saveOnClose = true;
						userinput_destination = string.Empty;
						userinput_source = string.Empty;
						if (renamed) {
							MessageBox.Show (Program.Language.MergeProfilesDoneRename);
						} else {
							MessageBox.Show (Program.Language.MergeProfilesDone);
						}
					}
				}
			} catch (ArgumentOutOfRangeException) {
				MessageBox.Show (Program.Language.MergeProfilesError);
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
			foreach (ToolStripMenuItem t in ((ToolStripMenuItem)this.mainFormCM.Items[1]).DropDownItems) {
				t.Checked = false;
			}

			((ToolStripMenuItem)sender).Checked = true;
			this.Localize ();
		}

		/*
		* Import a data file
		*/
		private void Import ()
		{
			this.NotIdle ();

			OpenFileDialog f = new OpenFileDialog ();
			f.Filter = string.Format ("{0}|{1}", Program.ProductName, Program.ImportFilter);
			if (f.ShowDialog () == DialogResult.OK) {
				try {
					new Storage ().DoLoad (f.FileName, this.heartbeatlock, true);
					this.saveOnClose = true;
					this.RefreshControls (RefreshLevel.Profile);
				} catch (InviumException ex) {
					MessageBox.Show (ex.Message);
				} catch (XmlException) {
					// Most likely an empty XML file, just ignore.
				}
			}
		}

		/*
		* Manually set a new master password
		*/
		private void SetMasterPassword ()
		{
			this.NotIdle ();

			string input_password = string.Empty;
			if (new InputBox ().ShowMe (InputBox.Mode.Doublepassword, ref input_password, Program.Language.MasterPasswordTitle, Program.Language.MasterPasswordPrompt, Program.Language.ConfirmPassword) == DialogResult.OK) {
				Program.MasterPassword.Password = input_password;
				this.saveOnClose = true;
				input_password = string.Empty;
			}
		}

		/*
		* Copy something to the clipboard and activate the timeout
		*/
		private void Copy2Clipboard (string message)
		{
			try {
				#if BuildForMono
				((Gtk.Clipboard)Gtk.Clipboard.Get (Gdk.Selection.Clipboard)).Text = message;
				((Gtk.Clipboard)Gtk.Clipboard.Get (Gdk.Selection.Clipboard)).Store ();
				#else
				Clipboard.SetText (message);
				#endif
				Program.ClipboardClearEnabled = true;
				this.clipboardTimeout = DateTime.Now.AddSeconds (CCTime);
			} catch (ArgumentOutOfRangeException) {
				this.RefreshControls (RefreshLevel.Profile);
			}
		}

		/*
		 * Closing the form
		 */
		private void MainForm_Closing (object sender, EventArgs e)
		{
			// Lock and stop the heartbeat, this is to prevent fringe situations where
			// corruption could theoretically occur.
			lock (this.heartbeatlock) {
				this.heartbeat.Abort ();
				while (this.heartbeat.IsAlive) {
				}
			}

			// Store all data if needed, we're done here.
			if (this.saveOnClose) {
				new Storage ().DoSave (this.encrypt.Checked, this.heartbeatlock);
			}
		}
	}
}