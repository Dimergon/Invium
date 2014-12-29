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
* This holds all the language strings, all the actual language entries
* and also performs any language changes.
*/
namespace Invium
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
		public string CloneServiceDone;
		public string CloneServiceNoSuchProfile;
		public string MergeProfilesSourceTitle;
		public string MergeProfilesSourcePrompt;
		public string MergeProfilesDestinationTitle;
		public string MergeProfilesDestinationPrompt;
		public string MergeProfilesDestinationExists;
		public string MergeProfilesSourceDoesNotExist;
		public string MergeProfilesDone;
		public string MergeProfilesDoneRename;

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

		/* Saving data */
		public string FirstRunTitle;
		public string FirstRunPrompt;
		public string MasterPasswordConfirmation;
		public string MasterPasswordTitle;
		public string MasterPasswordPrompt;

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
		public string ProfileMerge;
		public string MasterPassword;
		public string Execute;
		public string NoExecute;
		public string SetExecute;
		public string CloneService;
		public string Loading;
		public string Saving;

		/* etc... */
		public string ConfirmPassword;
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
		public string MergeProfilesError;
		public string AddAccountError;
		public string RemoveAccountError;
		public string RenameAccountError;
		public string SetPasswordError;
		public string GeneratePasswordError;
		public string TrayGenerateReminder;
		public string ThreadException;
		public string UnhandledException;

		/*
		 * Explanation on adding translations:
		 * add a full translation in LoadLocalization(string lang) and
		 * then add a name for that into this array (Traylangs)
		 * public string[] Traylangs = { "English", "Nederlands", "Example" };
		 */
		public string[] Traylangs = { "English", "Nederlands" };
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
			/*
			case "Example":
			case "xx-YY":
			case "xx-ZZ":
				this.ExampleLanguageEntry = "Example translation";
				// this.Trayfeedback needs to be set to the exact entry that was added
				// to string[] Traylangs.
				this.Trayfeedback = "Example";
				break;
			*/
			case "Nederlands":
			case "nl-NL":
			case "nl-BE":
				this.LoadStats = "Er zijn {0} profielen, {1} diensten en {2} accounts geladen in {3:0.##} ms.";
				this.ProfileExists = "Er bestaat al een profiel met die naam.";
				this.ServiceExists = "Voor profiel {0} bestaat er al een dienst met die naam.";
				this.AccountExists = "Voor dienst {0} van profiel {1} bestaat er al een account met die naam.";
				this.AddAccountTitle = "Nieuw account";
				this.AddAccountPrompt = "Geef een naam op voor dit nieuwe account.";
				this.AddAccountPasswordTitle = "Wacht voor nieuw account";
				this.AddAccountPasswordPrompt = "Geef het wachtwoord op voor dit nieuwe account.";
				this.RenameAccountTitle = "Account hernoemen";
				this.RenameAccountPrompt = "Geef een nieuwe naam op voor het account.";
				this.AddProfileTitle = "Nieuw profiel";
				this.AddProfilePrompt = "Geef een naam op voor dit nieuwe profiel.";
				this.RenameProfileTitle = "Profiel hernoemen";
				this.RenameProfilePrompt = "Geef een nieuwe naam op voor dit profiel.";
				this.AddServiceTitle = "Nieuwe dienst";
				this.AddServicePrompt = "Geef een naam op voor deze nieuwe dienst.";
				this.RenameServiceTitle = "Dienst hernoemen";
				this.RenameServicePrompt = "Geef een nieuwe naam op voor deze dienst.";
				this.SetExecuteTitle = "Stel commando in";
				this.SetExecutePrompt = "Geef een commando in om op te starten voor deze dienst.";
				this.CloneServiceTitle = "Geef doelbestemming op";
				this.CloneServicePrompt = "Geef de naam van het profiel op om {0} naar te klonen.";
				this.CloneServiceRename = "Een dienst genaamd {0} bestaat al voor profiel {1}.\r\nDienst gekloond onder de naam: {2}";
				this.CloneServiceDone = "De dienst {0} is geklooned naar {1}";
				this.CloneServiceNoSuchProfile = "Er bestaat geen profiel met de opgegeven naam, probeer het nogmaals.";
				this.MergeProfilesSourceTitle = "Profielen samenvoegen - Bron";
				this.MergeProfilesSourcePrompt = "Geef de namen op van de profielen om samen te voegen, scheidt deze met een komma.";
				this.MergeProfilesDestinationTitle = "Profielen samenvoegen - Doel";
				this.MergeProfilesDestinationPrompt = "Geef de naam op voor het profiel om de genoemde profielen naar samen te voegen.";
				this.MergeProfilesDestinationExists = "Een profiel met de opgegeven doelbestemmingsnaam bestaat al, geannuleerd.";
				this.MergeProfilesSourceDoesNotExist = "Niet alle opgegeven bron profielen bestaan, geannuleerd.";
				this.MergeProfilesDone = "Het samenvoegen van de opgegeven profielen is successvol verlopen.";
				this.MergeProfilesDoneRename = "Het samenvoegen van de opgegeven profielen is successvol verlopen.\r\nEchter, er bestonden duplicaten binnen de opgegeven profielen, deze zijn hernoemd.";
				this.NewPasswordTitle = "Wachtwoord";
				this.NewPasswordPrompt = "Geef een nieuw wachtwoord op.";
				this.NewPasswordConfirmation = "Bevestig het wachtwoord.";
				this.Login = "Inloggen";
				this.AuthPassword = "Geef het juiste wachtwoord op.";
				this.AuthFailCancel = "Er is een wachtwoord vereist om in te loggen.";
				this.AuthFailIncorrect = "Dat is niet het juiste wachtwoord.";
				this.ImportAuth = "Importeren van versleutelde gegevens";
				this.ImportPassword = "Geef het wachtwoord van de versleutelde gegevens op.";
				this.ImportCancel = "Er is een wachtwoord vereist voor het importeren van deze gegevens.";
				this.CustomLoadFail = "Het opgegeven bestand {0} kon niet worden geopend.";
				this.UnsavedPasswordPrompt = "Een aangemaakt wachtwoord voor {0} is nog niet opgeslagen, klik op 'Ja' om dit nu te doen.\r\nKlik op 'Nee' om het aangemaakte wachtwoord te vergeten.";
				this.UnsavedPasswordTitle = "Niet opgeslagen wachtwoord";
				this.FirstRunTitle = "Instellen";
				this.FirstRunPrompt = "Geef een hoofdwachtwoord op.";
				this.MasterPasswordConfirmation = "Bevestig het wachtwoord.";
				this.MasterPasswordTitle = "Hoofdwachtwoord";
				this.MasterPasswordPrompt = "Geef een nieuw hoofdwachtwoord op.";
				this.FatalNoPasswordNode = "De gegevens zijn versleuteld maar de wachtwoord informatie ontbreekt.\r\nDe gegevens zijn niet te herstellen.";
				this.FatalNoSaltNode = "De gegevens zijn versleuteld maar de salt informatie ontbreekt.\r\nDe gegevens zijn niet te herstellen.";
				this.FatalSingleInstance = "Dit programma mag maar een keer tegelijk actief zijn.";
				this.FatalNeedPasswordReencrypt = "Er is een nieuw wachtwoord vereist voor het versleutelen van gegevens.";
				this.Profile = "Profiel";
				this.Service = "Dienst";
				this.Account = "Account";
				this.Password = "Wachtwoord";
				this.Add = "Toevoegen";
				this.Remove = "Verwijderen";
				this.Rename = "Hernoemen";
				this.Set = "Instellen";
				this.Generate = "Aanmaken";
				this.ReadOnly = "Alleen-lezen";
				this.Encrypt = "Versleutelen";
				this.OK = "OK";
				this.Cancel = "Annuleren";
				this.CopyToClipboard = "Naar klemboard kopieren.";
				this.ShowPassword = "Toon wachtwoord";
				this.PasswordStrength = "Wachtwoord sterkte";
				this.DisableIdleTimeout = "Automatisch uitschakelen deactiveren.";
				this.Language = "Taal";
				this.Import = "Importeren";
				this.ProfileMerge = "Profielen samenvoegen";
				this.MasterPassword = "Hoofdwachtwoord instellen";
				this.Execute = "{0} uitvoeren";
				this.NoExecute = "Geen commando ingesteld om uit te voeren.";
				this.CloneService = "Dienst klonen";
				this.SetExecute = "Stel commando in om uit te voeren.";
				this.Loading = "Even geduld tijdens het laden.";
				this.Saving = "Even geduld tijdens het opslaan.";
				this.ConfirmPassword = "Bevestig het wachtwoord.";
				this.ConfirmQuestion = "Weet u het zeker?";
				this.ConfirmCaption = "Graag bevestigen";
				this.Stats = "Profiel {0} bevat {1} diensten.";
				this.AddProfileError = "Er is een fout opgetreden tijdens het toevoegen van een profiel, probeer het nogmaals.";
				this.RemoveProfileError = "Er is een fout opgetreden tijdens het verwijderen van dit profiel, probeer het nogmaals.";
				this.RenameProfileError = "Er is een fout opgetreden tijdens het hernoemen van dit profiel, probeer het nogmaals.";
				this.AddServiceError = "Er is een fout opgetreden tijdens het toevoegen van een dienst, probeer het nogmaals.";
				this.RemoveServiceError = "Er is een fout opgetreden tijdens het verwijderen van deze dienst, probeer het nogmaals.";
				this.RenameServiceError = "Er is een fout opgetreden tijdens het hernoemen van deze dienst, probeer het nogmaals.";
				this.SetExecuteError = "Er is een fout opgetreden tijdens het instellen van een commando voor deze dienst, probeer het nogmaals.";
				this.RunExecuteError = "Er is een fout opgetreden tijdens het uitvoeren van het commando voor deze dienst, probeer het nogmaals.";
				this.CloneServiceError = "Er is een fout opgetreden tijdens het klonen van deze dienst, probeer het nogmaals.";
				this.MergeProfilesError = "Er is een fout opgetreden tijdens het samenvoegen van de opgegeven profielen, probeer het nogmaals.";
				this.AddAccountError = "Er is een fout opgetreden tijdens het toevoegen van een account, probeer het nogmaals.";
				this.RemoveAccountError = "Er is een fout opgetreden tijdens het verwijderen van dit account, probeer het nogmaals.";
				this.RenameAccountError = "Er is een fout opgetreden tijdens het hernoemen van dit account, probeer het nogmaals.";
				this.SetPasswordError = "Er is een fout opgetreden tijdens het instellen van een wachtwoord, probeer het nogmaals.";
				this.GeneratePasswordError = "Er is een fout opgetreden tijdens het aanmaken van een wachtwoord, probeer het nogmaals.";
				this.TrayGenerateReminder = "Het aangemaakte wachtwoord is naar het klemboard gekopieerd.\r\nVergeet niet op 'Instellen' te klikken wanneer is bevestigd dat het wachtwoord is geaccepteerd door {0}.";
				this.ThreadException = "Thread uitzondering!";
				this.UnhandledException = "Onbekende uitzondering!";
				this.Trayfeedback = "Nederlands";
				break;
			case "English":
			case "en-GB":
			case "en-US":
			default:
				this.LoadStats = "Loaded {0} profiles, {1} services and {2} accounts in {3:0.##} ms.";
				this.ProfileExists = "There already is a profile with that name.";
				this.ServiceExists = "There already is a service with that name for profile {0}.";
				this.AccountExists = "There already is an account with that name for service {0} of profile {1}.";
				this.AddAccountTitle = "New account";
				this.AddAccountPrompt = "Please provide an account name.";
				this.AddAccountPasswordTitle = "Password for new account";
				this.AddAccountPasswordPrompt = "Please provide a password for this new account.";
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
				this.CloneServiceDone = "The service {0} has been cloned to {1}";
				this.CloneServiceNoSuchProfile = "There is no profile with that name, please try again.";
				this.MergeProfilesSourceTitle = "Merge profiles - Source";
				this.MergeProfilesSourcePrompt = "Enter the names of the profiles to merge, separated by comma.";
				this.MergeProfilesDestinationTitle = "Merge profiles - Destination";
				this.MergeProfilesDestinationPrompt = "Enter the profile name to merge the specified profiles to.";
				this.MergeProfilesDestinationExists = "A profile with the specified destination name already exists, aborted.";
				this.MergeProfilesSourceDoesNotExist = "Not all the specified source profiles exist, aborted.";
				this.MergeProfilesDone = "The merging process was completed successfully.";
				this.MergeProfilesDoneRename = "The merging process was completed sucessfully.\r\nHowever, since duplicate services exist, these have been renamed.";
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
				this.FirstRunTitle = "Initial setup";
				this.FirstRunPrompt = "Please provide a master password.";
				this.MasterPasswordConfirmation = "Please confirm this password.";
				this.MasterPasswordTitle = "Master password";
				this.MasterPasswordPrompt = "Please provide a new master password.";
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
				this.ProfileMerge = "Merge profiles";
				this.MasterPassword = "Set master password";
				this.Execute = "Open {0}";
				this.NoExecute = "No command specified to execute.";
				this.CloneService = "Clone service";
				this.SetExecute = "Set command to execute.";
				this.Loading = "Loading, please wait.";
				this.Saving = "Saving, please wait.";
				this.ConfirmPassword = "Please confirm this password.";
				this.ConfirmQuestion = "Are you sure?";
				this.ConfirmCaption = "Please confirm";
				this.Stats = "Profile {0} contains {1} services.";
				this.AddProfileError = "An error occurred while trying to add a profile, please try again.";
				this.RemoveProfileError = "An error occurred while trying to remove this profile, please try again.";
				this.RenameProfileError = "An error occurred while trying to rename this profile, please try again.";
				this.AddServiceError = "An error occurred while trying to add a service, please try again.";
				this.RemoveServiceError = "An error occurred while trying to remove this service, please try again.";
				this.RenameServiceError = "An error occurred while trying to rename this service, please try again.";
				this.SetExecuteError = "An error occurred while trying to set a command for this service, please try again.";
				this.RunExecuteError = "An error occurred while trying to launch the command for this service, please try again.";
				this.CloneServiceError = "An error occurred while trying to clone this service, please try again.";
				this.MergeProfilesError = "An error ocurred while trying to merge the specified profiles, please try again.";
				this.AddAccountError = "An error occurred while trying to add an account, please try again.";
				this.RemoveAccountError = "An error occurred while trying to remove this account, please try again.";
				this.RenameAccountError = "An error occurred while trying to rename this account, please try again.";
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