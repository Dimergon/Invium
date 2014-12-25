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

/*
* Main program entry. Performs several functions before actually launching
* PinCushion's primary form.
*/
namespace PinCushion
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Threading;
	using System.Windows.Forms;

	public static class Program
	{
		// Commandline parameters
		public static string ManualLanguage = string.Empty;

		// Store the active language
		public static LanguageClass Language;

		// and where else would we store our data...
		public static string ImportFilter = "n_data.xml";
		public static string DataFile = System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar + ImportFilter;

		// used to determine if we need to clear the clipboard; this sits here because of PinCushionExit()
		public static bool ClipboardClearEnabled = false;

		// Main container
		public static List<Profile> Profiles = new List<Profile> ();

		// used to prevent multiple instances
		private static string lockFile = Path.GetTempPath () + System.Windows.Forms.Application.ProductName;

		// Custom exit
		public static void PinCushionExit ()
		{
			// Clear the clipboard but only if we placed something on it...
			if (ClipboardClearEnabled) {
				#if BuildForMono
				((Gtk.Clipboard)Gtk.Clipboard.Get (Gdk.Selection.Clipboard)).Clear ();
				((Gtk.Clipboard)Gtk.Clipboard.Get (Gdk.Selection.Clipboard)).Store ();
				#else
				System.Windows.Forms.Clipboard.Clear ();
				#endif
			}

			// Release the lock, so we may again start someday!
			if (File.Exists (lockFile)) {
				File.Delete (lockFile);
			}

			Environment.Exit (0);
		}

		// Main entry point
		private static void Main (string[] args)
		{
			// Unhandled exception...
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler (Pincushion_ThreadException);
			Application.SetUnhandledExceptionMode (UnhandledExceptionMode.CatchException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler (PinCushion_UnhandledException);

			/* Initialize the localization */
			Language = new LanguageClass ();

			// multiple instances lock...
			if (File.Exists (lockFile)) {
				MessageBox.Show (Language.FatalSingleInstance, string.Empty, MessageBoxButtons.OK);
				Environment.Exit (0);
			}

			// first instance, so let's create the lock...
			try {
				using (StreamWriter f = File.CreateText (lockFile)) {
					f.WriteLine ((string)DateTime.UtcNow.ToLongTimeString ());
					f.Close ();
				}
			} catch (UnauthorizedAccessException) {
				/*
				* The most likely scenario for ending up here is when running on a system
				* that is completely write-protected, without even a temp folder/swap partition.
				* Unlikely as that may seem, we still have to catch this exception... otherwise
				* the unhandled exception handlers would catch it and prevent the application
				* from running... at all.
				*/
			}

			/* Loop through all the specified commandline parameters */
			foreach (string s in args) {
				switch (s.ToLower ()) {
				case "-language":
					// Manually specifying a language...
					try {
						ManualLanguage = args [Array.IndexOf (args, s) + 1];
					} catch (Exception) {
						// Probably no language code specified.
					}

					break;
				case "-load":
					// Manually specifying the data file to load...
					DataFile = args [Array.IndexOf (args, s) + 1];
					if (!File.Exists (DataFile)) {
						MessageBox.Show (string.Format (Language.CustomLoadFail, DataFile));
						PinCushionExit ();
					}

					break;
				default:
					// Whatever it is, it's not something we're interested in.
					break;
				}
			}

			Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NoneEnabled;
			Application.SetCompatibleTextRenderingDefault (true);
			Application.Run (new MainForm ());
			PinCushionExit ();
		}

		private static void Pincushion_ThreadException (object sender, ThreadExceptionEventArgs e)
		{
			MessageBox.Show (e.Exception.ToString (), Language.ThreadException, MessageBoxButtons.OK);
			PinCushionExit ();
		}

		private static void PinCushion_UnhandledException (object sender, UnhandledExceptionEventArgs e)
		{
			MessageBox.Show (((Exception)e.ExceptionObject).InnerException.ToString (), Language.UnhandledException, MessageBoxButtons.OK);
			PinCushionExit ();
		}
	}
}