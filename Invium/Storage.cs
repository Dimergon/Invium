﻿/*
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

// Holds storage (loading/saving) related functions
namespace Invium
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using System.Threading;
	using System.Windows.Forms;
	using System.Xml;

	public class Storage
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

		/*
		* Load the specified data file
		*
		* Steps:
		* - Load all the data into an XML document, but only if it exists.
		* - Request the password if the data is encrypted.
		* - Loop through all child profiles.
		*      - Loop through all child services of each profile.
		*          - Loop through all child accounts of each service.
		* - Refresh controls.
		*/
		public void DoLoad (string file, object heartbeat, bool importing)
		{
			/* Actually load the data
			*
			* I experimented with various other approaches but XmlDocument + Xpath proved the least cumbersome.
			*/
			InviumCryptography crypto = new InviumCryptography ();
			Stopwatch load_timer = new Stopwatch ();
			XmlDocument document = new XmlDocument ();
			document.Load (file);

			// Create the loadingscreen
			StorageSplash loadingscreen = new StorageSplash (Program.Language.Loading);
			Thread loadingscreen_thread = new Thread (new ThreadStart (delegate() {
				try {
					loadingscreen.ShowDialog ();
				} catch (ThreadAbortException) {
					loadingscreen.Close ();
				}
			}));
			loadingscreen_thread.IsBackground = true;

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
					throw new InviumException (Program.Language.FatalNoSaltNode);
				}

				if (password_node == null) {
					throw new InviumException (Program.Language.FatalNoPasswordNode);
				}

				salt = salt_node.InnerText;
				string password_hash = password_node.InnerText;

				// This InputBox will ask for the password, of either log on or of the import
				if (new InputBox ().ShowMe (InputBox.Mode.Singlepassword, ref input_password, importing ? Program.Language.ImportAuth : Program.Language.Login, importing ? Program.Language.ImportPassword : Program.Language.AuthPassword) == DialogResult.Cancel) {
					throw new InviumException (importing ? Program.Language.ImportCancel : Program.Language.AuthFailCancel);
				}

				// Start the loading screen
				loadingscreen_thread.Start ();
				while (!loadingscreen_thread.IsAlive) {
				}
				// this next call is a nasty hack to avoid a possible Fatal IO error in *nix's window manager(s)
				Thread.Sleep (1000);

				load_timer.Start ();

				if (crypto.Hash (input_password, salt) != password_hash) {
					loadingscreen.Close ();
					throw new InviumException (Program.Language.AuthFailIncorrect);
				}
			}

			/*
			* - Securely store the password in case of initial log on (for (auto)saving purposes)
			* - Ask for a new password in case of an unencrypted main data file (to re-enable encryption)
			*
			* Skip if importing
			*/
			bool do_recrypt_save = false;
			if (!importing) {
				if (!do_decrypt) {
					if (new InputBox ().ShowMe (InputBox.Mode.Doublepassword, ref input_password, Program.Language.MasterPasswordTitle, Program.Language.MasterPasswordPrompt, Program.Language.ConfirmPassword) == DialogResult.Cancel) {
						throw new InviumException (Program.Language.FatalNeedPasswordReencrypt);
					} else {
						do_recrypt_save = true;
					}
				}

				Program.MasterPassword.Password = input_password;
			}

			short p_count, s_count, a_count;
			p_count = s_count = a_count = 0;
			if (!load_timer.IsRunning) {
				load_timer.Start ();
			}

			// Main Loop, will also decrypt if data is encrypted
			lock (heartbeat) {
				foreach (XmlNode profile_node in document.SelectNodes(string.Format("{0}::{1}", XMLDescendant, XMLProfile))) {
					p_count++;
					XmlNode profile_name_node = profile_node.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLName));
					string profile_name = do_decrypt ? crypto.Decrypt (profile_name_node.InnerText, input_password) : profile_name_node.InnerText;

					// Check for duplicates, importing requires this step.
					if (Program.Profiles.Find (delegate(Profile x) {
						return x.Name == profile_name;
					}) != null) {
						profile_name += new Password ().GenSalt ();
					}

					Profile p = new Profile (profile_name);
					foreach (XmlNode service_node in profile_node.SelectNodes(string.Format("{0}::{1}", XMLDescendant, XMLService))) {
						s_count++;
						XmlNode service_name_node = service_node.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLName));
						XmlNode service_command_node = service_node.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLCommand));
						string service_name = do_decrypt ? crypto.Decrypt (service_name_node.InnerText, input_password) : service_name_node.InnerText;
						string service_command = string.Empty;
						if (service_command_node != null) {
							service_command = do_decrypt ? crypto.Decrypt (service_command_node.InnerText, input_password) : service_command_node.InnerText;
						}

						Service s = new Service (service_name, service_command);
						foreach (XmlNode account_node in service_node.SelectNodes(string.Format("{0}::{1}", XMLDescendant, XMLAccount))) {
							a_count++;
							XmlNode account_name_node = account_node.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLName));
							XmlNode account_password_node = account_node.SelectSingleNode (string.Format ("{0}::{1}", XMLDescendant, XMLPassword));
							string account_name = do_decrypt ? crypto.Decrypt (account_name_node.InnerText, input_password) : account_name_node.InnerText;
							string account_password = do_decrypt ? crypto.Decrypt (account_password_node.InnerText, input_password) : account_password_node.InnerText;
							Account a = new Account (account_name, account_password);
							s.Accounts.Add (a);
						}

						p.Services.Add (s);
					}

					Program.Profiles.Add (p);
				}
			}

			// and close the loading screen
			loadingscreen_thread.Abort ();

			// Done, inform user of the time it took to load all data
			load_timer.Stop ();
			NotifyIcon tray = new NotifyIcon ();
			tray.BalloonTipTitle = string.Format ("{0} v{1}", Program.ProductName, Program.ProductVersion);
			tray.BalloonTipIcon = ToolTipIcon.Info;
			tray.BalloonTipText = string.Format (Program.Language.LoadStats, p_count, s_count, a_count, load_timer.ElapsedMilliseconds);
			tray.ShowBalloonTip (int.MaxValue);
			input_password = string.Empty;

			// Save all data in case data was unencrypted, to encrypt it again
			if (do_recrypt_save) {
				this.DoSave (true, heartbeat);
			}
		}

		/*
		 * Save all data
		 */
		public void DoSave (bool do_encrypt, object heartbeat)
		{
			// Refuse to save any data in case of a forced read only state.
			if (Program.ForcedReadOnly) {
				return;
			}

			// Lock (interrupt the heartbeat) and start storing the data.
			lock (heartbeat) {
				InviumCryptography crypto = new InviumCryptography ();
				// Create and load the savingscreen
				StorageSplash savingscreen = new StorageSplash (Program.Language.Saving);
				Thread savingscreen_thread = new Thread (new ThreadStart (delegate() {
					try {
						savingscreen.ShowDialog ();
					} catch (ThreadAbortException) {
						savingscreen.Close ();
					}
				}));
				savingscreen_thread.IsBackground = true;
				savingscreen_thread.Start ();
				while (!savingscreen_thread.IsAlive) {
				}
				// this next call is a nasty hack to avoid a possible Fatal IO error in *nix's window manager(s)
				Thread.Sleep (1000);

				// Create the required streams and write the start
				using (StreamWriter document = File.CreateText (Program.DataFile)) {
					XmlWriterSettings settings = new XmlWriterSettings ();
					settings.Encoding = Encoding.UTF8;
					XmlWriter document_writer = XmlWriter.Create (document, settings);
					document_writer.WriteStartElement (XMLBody);

					// encryption releated data
					string salt = new Password ().GenSalt ();
					string password_hash = crypto.Hash (Program.MasterPassword.Password, salt);
					if (do_encrypt) {
						document_writer.WriteElementString (XMLencrypt, new Password ().GenSalt ());
						document_writer.WriteElementString (XMLSalt, salt);
						document_writer.WriteElementString (XMLPassword, password_hash);
					}

					// Main Loop, will also encrypt if specified
					foreach (Profile p in Program.Profiles) {
						document_writer.WriteStartElement (XMLProfile);
						string profile_name = do_encrypt ? crypto.Encrypt (p.Name, Program.MasterPassword.Password) : p.Name;
						document_writer.WriteElementString (XMLName, profile_name);
						foreach (Service s in p.Services) {
							document_writer.WriteStartElement (XMLService);
							string service_name = do_encrypt ? crypto.Encrypt (s.Name, Program.MasterPassword.Password) : s.Name;
							document_writer.WriteElementString (XMLName, service_name);
							if (s.Command != string.Empty) {
								string service_command = do_encrypt ? crypto.Encrypt (s.Command, Program.MasterPassword.Password) : s.Command;
								document_writer.WriteElementString (XMLCommand, service_command);
							}

							foreach (Account a in s.Accounts) {
								document_writer.WriteStartElement (XMLAccount);
								string account_name = do_encrypt ? crypto.Encrypt (a.Name, Program.MasterPassword.Password) : a.Name;
								string account_password = do_encrypt ? crypto.Encrypt (a.Password, Program.MasterPassword.Password) : a.Password;
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
				savingscreen_thread.Abort ();
			}
		}
	}
}
