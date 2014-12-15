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
* This holds all the language strings, all the actual language entries
* and also performs any language changes.
*/
namespace PinCushion
{
	using System;
	using System.Globalization;

	public class LanguageClass
	{
		/* Stats */
		public string LoadStats;

		/* Data manipulation */
		public string ProfileExists;
		public string ServiceExists;
		public string AccountExists;
		public string AddAccountTitle;
		public string AddAccountPrompt;
		public string AddAccountPasswordTitle;
		public string AddAccountPasswordPrompt;
		public string AddAccountPasswordConfirmation;
		public string RenameAccountTitle;
		public string RenameAccountPrompt;
		public string AddProfileTitle;
		public string AddProfilePrompt;
		public string RenameProfileTitle;
		public string RenameProfilePrompt;
		public string AddServiceTitle;
		public string AddServicePrompt;
		public string RenameServiceTitle;
		public string RenameServicePrompt;
		public string SetExecuteTitle;
		public string SetExecutePrompt;
		public string CloneServiceTitle;
		public string CloneServicePrompt;
		public string CloneServiceRename;

		/* New password input */
		public string NewPasswordTitle;
		public string NewPasswordPrompt;
		public string NewPasswordConfirmation;

		/* Loading data/Authentication */
		public string Login;
		public string AuthPassword;
		public string AuthFailCancel;
		public string AuthFailIncorrect;
		public string ImportAuth;
		public string ImportPassword;
		public string ImportCancel;
		public string CustomLoadFail;
		public string UnsavedPasswordPrompt;
		public string UnsavedPasswordTitle;

		/* Saving PinCushion data */
		public string FirstRunTitle;
		public string FirstRunPrompt;
		public string FirstRunConfirmation;
		public string PinCushionSaveConfirmation;
		public string PinCushionReencryptTitle;
		public string PinCushionReencryptPrompt;
		public string PinCushionReencryptConfirmation;

		/* Fatal errors */
		public string FatalNoPasswordNode;
		public string FatalNoSaltNode;
		public string FatalSingleInstance;
		public string FatalNeedPasswordReencrypt;

		/* Interface */
		public string Profile;
		public string Service;
		public string Account;
		public string Password;
		public string Add;
		public string Remove;
		public string Rename;
		public string Set;
		public string Generate;
		public string ReadOnly;
		public string Encrypt;
		public string OK;
		public string Cancel;
		public string CopyToClipboard;
		public string ShowPassword;
		public string PasswordStrength;
		public string DisableIdleTimeout;
		public string Language;
		public string Import;
		public string PinCushionPassword;
		public string Execute;
		public string NoExecute;
		public string SetExecute;
		public string CloneService;

		/* etc... */
		public string ConfirmQuestion;
		public string ConfirmCaption;
		public string Stats;
		public string AddProfileError;
		public string RemoveProfileError;
		public string RenameProfileError;
		public string AddServiceError;
		public string RemoveServiceError;
		public string RenameServiceError;
		public string SetExecuteError;
		public string RunExecuteError;
		public string CloneServiceError;
		public string AddAccountError;
		public string RemoveAccountError;
		public string RenameAccountError;
		public string SetPasswordError;
		public string GeneratePasswordError;
		public string TrayGenerateReminder;
		public string ThreadException;
		public string UnhandledException;

		// Used in the rightclick menu
		public string[] Traylangs = { "English" };
		public string Trayfeedback;

		/*
        * For ease of use, just do the localization within the constructor of the language class.
        */
		public LanguageClass ()
		{
			Array.Sort (this.Traylangs);
			this.LoadLocalization (Program.ManualLanguage == string.Empty ? CultureInfo.CurrentCulture.Name : Program.ManualLanguage);
		}

		/*
         * Manual localizaion
         */
		public void LoadLocalization (string lang)
		{
			switch (lang) {
			default:
				this.LoadStats = "Loaded {0} profiles, {1} services and {2} accounts in {3:0.##} ms.";
				this.ProfileExists = "There already is a profile with that name.";
				this.ServiceExists = "There already is a service with that name for profile {0}.";
				this.AccountExists = "There already is an account with that name for service {0} of profile {1}.";
				this.AddAccountTitle = "New account";
				this.AddAccountPrompt = "Please provide an account name.";
				this.AddAccountPasswordTitle = "Password for new account";
				this.AddAccountPasswordPrompt = "Please provide a password for this new account.";
				this.AddAccountPasswordConfirmation = "Please confirm this password.";
				this.RenameAccountTitle = "Rename account";
				this.RenameAccountPrompt = "Please provide a new account name.";
				this.AddProfileTitle = "New profile";
				this.AddProfilePrompt = "Please provide a profile name.";
				this.RenameProfileTitle = "Rename profile";
				this.RenameProfilePrompt = "Please provide a new profile name.";
				this.AddServiceTitle = "New service";
				this.AddServicePrompt = "Please provide a service name.";
				this.RenameServiceTitle = "Rename service";
				this.RenameServicePrompt = "Please provide a new service name.";
				this.SetExecuteTitle = "Set execute command";
				this.SetExecutePrompt = "Please set a command to execute for this service.";
				this.CloneServiceTitle = "Specify the destination profile";
				this.CloneServicePrompt = "Please specify the profile to clone {0} to.";
				this.CloneServiceRename = "A service named {0} already exists in profile {1}.\r\nCloned it to a different name: {2}";
				this.NewPasswordTitle = "Password";
				this.NewPasswordPrompt = "Please provide a new password.";
				this.NewPasswordConfirmation = "Please confirm the password.";
				this.Login = "Log in";
				this.AuthPassword = "Please enter the correct password.";
				this.AuthFailCancel = "To log on, please enter the correct password.";
				this.AuthFailIncorrect = "That is not the correct password.";
				this.ImportAuth = "Importing encrypted data";
				this.ImportPassword = "To import this data, enter its password.";
				this.ImportCancel = "Importing this data requires a password.";
				this.CustomLoadFail = "The specified file {0} could not be found.";
				this.UnsavedPasswordPrompt = "A generated password for {0} was not stored, click 'Yes' to store it now.\r\nClicking 'No' will cause it to be lost.";
				this.UnsavedPasswordTitle = "Unsaved password";
				this.FirstRunTitle = "Initial {0} setup";
				this.FirstRunPrompt = "Please provide a password for {0} access.";
				this.FirstRunConfirmation = "Please confirm this password.";
				this.PinCushionSaveConfirmation = "Please confirm this password.";
				this.PinCushionReencryptTitle = "Enabling {0} encryption";
				this.PinCushionReencryptPrompt = "Please provide a new password for {0} access.";
				this.PinCushionReencryptConfirmation = "Please confirm this password.";
				this.FatalNoPasswordNode = "Data is encrypted but the password information is absent.\r\nData is unrecoverable.";
				this.FatalNoSaltNode = "Data is encrypted but the salt information is absent.\r\nData is unrecoverable.";
				this.FatalSingleInstance = "Only one instance of this application may be run at any single time.";
				this.FatalNeedPasswordReencrypt = "A new password is required to encrypt.";
				this.Profile = "Profile";
				this.Service = "Service";
				this.Account = "Account";
				this.Password = "Password";
				this.Add = "Add";
				this.Remove = "Remove";
				this.Rename = "Rename";
				this.Set = "Set";
				this.Generate = "Generate";
				this.ReadOnly = "Read Only";
				this.Encrypt = "Encrypt";
				this.OK = "OK";
				this.Cancel = "Cancel";
				this.CopyToClipboard = "Copy to clipboard.";
				this.ShowPassword = "Show password";
				this.PasswordStrength = "Password Strength";
				this.DisableIdleTimeout = "Disable idle timeout.";
				this.Language = "Language";
				this.Import = "Import";
				this.PinCushionPassword = "Set {0} password";
				this.Execute = "Open {0}";
				this.NoExecute = "No command specified to execute.";
				this.CloneService = "Clone service";
				this.SetExecute = "Set command to execute.";
				this.ConfirmQuestion = "Are you sure?";
				this.ConfirmCaption = "Please confirm";
				this.Stats = "Profile {0} contains {1} services.";
				this.AddProfileError = "An error occurred while trying to add a profile, please try again.";
				this.RemoveProfileError = "An error occurred while trying to remove a profile, please try again.";
				this.RenameProfileError = "An error occurred while trying to rename a profile, please try again.";
				this.AddServiceError = "An error occurred while trying to add a service, please try again.";
				this.RemoveServiceError = "An error occurred while trying to remove a service, please try again.";
				this.RenameServiceError = "An error occurred while trying to rename a service, please try again.";
				this.SetExecuteError = "An error occurred while trying to set a command for the specified service, please try again.";
				this.RunExecuteError = "An error occurred while trying to launch the command for this service, please try again.";
				this.CloneServiceError = "An error occurred while trying to clone {0}, please try again.";
				this.AddAccountError = "An error occurred while trying to add an account, please try again.";
				this.RemoveAccountError = "An error occurred while trying to remove an account, please try again.";
				this.RenameAccountError = "An error occurred while trying to rename an account, please try again.";
				this.SetPasswordError = "An error occurred while setting a password, please try again.";
				this.GeneratePasswordError = "An error occured while generating a password, please try again.";
				this.TrayGenerateReminder = "The generated password has been copied to the clipboard.\r\nDo not forget to use Set after confrming this new password is accepted by {0}.";
				this.ThreadException = "Thread exception!";
				this.UnhandledException = "Unhandled exception!";
				this.Trayfeedback = "English";
				break;
			}
		}
	}
}