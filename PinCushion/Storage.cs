/*
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

// Holds storage (loading/saving) related functions
namespace PinCushion
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using System.Windows.Forms;
	using System.Xml;

	public partial class MainForm
	{
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

		// Used in auto saving upon changes
		private PinCushionPassword_Class pinCushionPassword = new PinCushionPassword_Class ();

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
			XmlDocument document = new XmlDocument ();
			document.Load (file);

			// Check for encryption
			bool do_decrypt = true;
			if (document.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLencrypt)) == null) {
				do_decrypt = false;
			}

			// User validation
			string salt = string.Empty, input_password = string.Empty;

			if (do_decrypt) {
				XmlNode salt_node = document.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLSalt));
				XmlNode password_node = document.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLPassword));
				if (salt_node == null) {
					throw new PinCushionException (Program.Language.FatalNoSaltNode);
				}

				if (password_node == null) {
					throw new PinCushionException (Program.Language.FatalNoPasswordNode);
				}

				salt = salt_node.InnerText;
				string password_hash = password_node.InnerText;

				// This InputBox will ask for the password, of either log on or of the import
				if (InputBox.Show (InputBox.Mode.Singlepassword, ref input_password, importing ? Program.Language.ImportAuth : Program.Language.Login, importing ? Program.Language.ImportPassword : Program.Language.AuthPassword) == DialogResult.Cancel) {
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
					if (InputBox.Show (InputBox.Mode.Doublepassword, ref input_password, string.Format (Program.Language.PinCushionReencryptTitle, System.Windows.Forms.Application.ProductName), string.Format (Program.Language.PinCushionReencryptPrompt, System.Windows.Forms.Application.ProductName), Program.Language.PinCushionReencryptConfirmation) == DialogResult.Cancel) {
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

			// Create and load the loadingscreen
			SplashScreen loadingscreen = new SplashScreen (Program.Language.Loading);
			System.Threading.Thread loadingscreen_thread = new System.Threading.Thread (new System.Threading.ThreadStart (delegate() {
				loadingscreen.ShowDialog ();
			}));
			loadingscreen_thread.IsBackground = true;
			loadingscreen_thread.Start ();
			while (!loadingscreen_thread.IsAlive) {
			}
			// this next call is a nasty hack to avoid a possible Fatal IO error in *nix's window manager(s)
			System.Threading.Thread.Sleep (1000);

			/*
			 * Main Loop, will also decrypt if data is encrypted
			 */
			foreach (XmlNode profile_node in document.SelectNodes(string.Format("{0}::{1}", XMLDescendant, XMLProfile))) {
				p_count++;
				XmlNode profile_name_node = profile_node.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLName));
				string profile_name = do_decrypt ? Crypto.Decrypt (profile_name_node.InnerText, input_password) : profile_name_node.InnerText;

				// Check for duplicates, importing requires this step.
				if (Program.Profiles.Find (delegate(Profile x) {
					return x.Name == profile_name;
				}) != null) {
					profile_name += DateTime.Now.ToString ();
				}

				Profile p = new Profile (profile_name);
				foreach (XmlNode service_node in profile_node.SelectNodes(string.Format("{0}::{1}", XMLDescendant, XMLService))) {
					s_count++;
					XmlNode service_name_node = service_node.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLName));
					XmlNode service_command_node = service_node.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLCommand));
					string service_name = do_decrypt ? Crypto.Decrypt (service_name_node.InnerText, input_password) : service_name_node.InnerText;
					string service_command = string.Empty;
					if (service_command_node != null) {
						service_command = do_decrypt ? Crypto.Decrypt (service_command_node.InnerText, input_password) : service_command_node.InnerText;
					}

					Service s = new Service (service_name, service_command);
					foreach (XmlNode account_node in service_node.SelectNodes(string.Format("{0}::{1}", XMLDescendant, XMLAccount))) {
						a_count++;
						XmlNode account_name_node = account_node.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLName));
						XmlNode account_password_node = account_node.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLPassword));
						string account_name = do_decrypt ? Crypto.Decrypt (account_name_node.InnerText, input_password) : account_name_node.InnerText;
						string account_password = do_decrypt ? Crypto.Decrypt (account_password_node.InnerText, input_password) : account_password_node.InnerText;
						Account a = new Account (account_name, account_password);
						s.ServiceAccounts.Add (a);
					}

					p.Profileservices.Add (s);
				}

				Program.Profiles.Add (p);
			}

			// Save all data in case data was unencrypted, to encrypt it again
			if (do_recrypt_save) {
				this.saveOnClose = true;
			}

			// and close the loading screen, which should close the thread as well.
			loadingscreen.Close ();

			// Done, inform user of the time it took to load all data
			load_timer.Stop ();
			this.tray.BalloonTipText = string.Format (Program.Language.LoadStats, p_count, s_count, a_count, load_timer.ElapsedMilliseconds);
			this.tray.ShowBalloonTip (int.MaxValue);
		}

		/*
		 * Save all data
		 */
		public void DoSave ()
		{
			// Create and load the savingscreen
			SplashScreen savingscreen = new SplashScreen (Program.Language.Saving);
			System.Threading.Thread savingscreen_thread = new System.Threading.Thread (new System.Threading.ThreadStart (delegate() {
				savingscreen.ShowDialog ();
			}));
			savingscreen_thread.IsBackground = true;
			savingscreen_thread.Start ();
			while (!savingscreen_thread.IsAlive) {
			}
			// this next call is a nasty hack to avoid a possible Fatal IO error in *nix's window manager(s)
			System.Threading.Thread.Sleep (1000);

			// Create the required streams and write the start
			using (StreamWriter document = File.CreateText (Program.DataFile)) {
				XmlWriterSettings settings = new XmlWriterSettings ();
				settings.Encoding = Encoding.UTF8;
				XmlWriter document_writer = XmlWriter.Create (document, settings);
				document_writer.WriteStartElement (XMLBody);

				// encryption releated data
				string salt = Password.GenSalt ();
				string password_hash = Crypto.Hash (this.pinCushionPassword.Password, salt);
				if (this.encrypt.Checked) {
					document_writer.WriteElementString (XMLencrypt, Password.GenSalt ());
					document_writer.WriteElementString (XMLSalt, salt);
					document_writer.WriteElementString (XMLPassword, password_hash);
				}

				// Main Loop, will also encrypt if specified
				foreach (Profile p in Program.Profiles) {
					document_writer.WriteStartElement (XMLProfile);
					string profile_name = this.encrypt.Checked ? Crypto.Encrypt (p.Name, this.pinCushionPassword.Password) : p.Name;
					document_writer.WriteElementString (XMLName, profile_name);
					foreach (Service s in p.Profileservices) {
						document_writer.WriteStartElement (XMLService);
						string service_name = this.encrypt.Checked ? Crypto.Encrypt (s.Name, this.pinCushionPassword.Password) : s.Name;
						document_writer.WriteElementString (XMLName, service_name);
						if (s.Command != string.Empty) {
							string service_command = this.encrypt.Checked ? Crypto.Encrypt (s.Command, this.pinCushionPassword.Password) : s.Command;
							document_writer.WriteElementString (XMLCommand, service_command);
						}

						foreach (Account a in s.ServiceAccounts) {
							document_writer.WriteStartElement (XMLAccount);
							string account_name = this.encrypt.Checked ? Crypto.Encrypt (a.Name, this.pinCushionPassword.Password) : a.Name;
							string account_password = this.encrypt.Checked ? Crypto.Encrypt (a.Password, this.pinCushionPassword.Password) : a.Password;
							document_writer.WriteElementString (XMLName, account_name);
							document_writer.WriteElementString (XMLPassword, account_password);
							document_writer.WriteEndElement ();
						}

						document_writer.WriteEndElement ();
					}

					document_writer.WriteEndElement ();
				}

				// Done
				document_writer.WriteEndElement ();
				document_writer.Close ();
				document.Close ();
			}

			// really done!
			savingscreen.Close ();
		}
	}
}
