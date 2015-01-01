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
* Main program entry. Performs several functions before actually launching
* Invium's primary form.
*/
[assembly: System.Reflection.AssemblyTitle ("Invium")]
[assembly: System.Reflection.AssemblyDescription ("A password manager in C#")]
[assembly: System.Reflection.AssemblyProduct ("Invium")]
[assembly: System.Reflection.AssemblyCopyright ("Copyright © 2013, 2014 Armin Altorffer")]
[assembly: System.Reflection.AssemblyVersion ("0.41.*")]

namespace Invium
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Threading;
	using System.Windows.Forms;
	using System.Windows.Forms.VisualStyles;

	public static class Program
	{
		// Commandline parameters
		public static string ManualLanguage = string.Empty;

		// Store the active language
		public static LanguageClass Language;

		// and where else would we store our data...
		public static string ImportFilter = "n_data.xml";
		public static string DataFile = Application.StartupPath + Path.DirectorySeparatorChar + ImportFilter;

		// used to determine if we need to clear the clipboard; this sits here because of InviumExit()
		public static bool ClipboardClearEnabled = false;

		// forced read only state; useful in environments where access is shared
		public static bool ForcedReadOnly = false;

		// Main container
		public static List<Profile> Profiles = new List<Profile> ();

		// Used in auto saving upon changes
		public static MasterPassword_Class MasterPassword = new MasterPassword_Class ();

		// Title
		public static string ProductName = System.Windows.Forms.Application.ProductName;
		public static string ProductVersion = System.Windows.Forms.Application.ProductVersion;

		// used to prevent multiple instances
		private static string lockFile = Path.GetTempPath () + Program.ProductName;

		// Custom exit
		public static void InviumExit ()
		{
			// Clear the clipboard but only if we placed something on it...
			if (ClipboardClearEnabled) {
				#if BuildForMono
				((Gtk.Clipboard)Gtk.Clipboard.Get (Gdk.Selection.Clipboard)).Clear ();
				((Gtk.Clipboard)Gtk.Clipboard.Get (Gdk.Selection.Clipboard)).Store ();
				#else
				Clipboard.Clear ();
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
			Application.ThreadException += new ThreadExceptionEventHandler (Invium_ThreadException);
			Application.SetUnhandledExceptionMode (UnhandledExceptionMode.CatchException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler (Invium_UnhandledException);

			// Initialize the localization
			Language = new LanguageClass ();

			// Loop through all the specified commandline parameters
			foreach (string s in args) {
				switch (s.ToLower ()) {
				case "-language":
					// Manually specifying a language...
					try {
						ManualLanguage = args [Array.IndexOf (args, s) + 1];
						// Re-Initialize the localization
						Language = null;
						Language = new LanguageClass ();
					} catch (Exception) {
						// Probably no language code specified.
					}

					break;
				case "-forcedreadonly":
					// Forcing the read only state
					ForcedReadOnly = true;
					break;
				case "-load":
					// Manually specifying the data file to load...
					DataFile = args [Array.IndexOf (args, s) + 1];
					if (!File.Exists (DataFile)) {
						MessageBox.Show (string.Format (Language.CustomLoadFail, DataFile));
						InviumExit ();
					}

					break;
				default:
					// Whatever it is, it's not something we're interested in.
					break;
				}
			}

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

			Application.VisualStyleState = VisualStyleState.NoneEnabled;
			Application.SetCompatibleTextRenderingDefault (true);
			Application.Run (new MainForm ());
			InviumExit ();
		}

		private static void Invium_ThreadException (object sender, ThreadExceptionEventArgs e)
		{
			MessageBox.Show (e.Exception.ToString (), Language.ThreadException, MessageBoxButtons.OK);
			InviumExit ();
		}

		private static void Invium_UnhandledException (object sender, UnhandledExceptionEventArgs e)
		{
			MessageBox.Show (((Exception)e.ExceptionObject).InnerException.ToString (), Language.UnhandledException, MessageBoxButtons.OK);
			InviumExit ();
		}
	}
}