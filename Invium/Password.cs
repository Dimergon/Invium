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
* This performs password related functions such as generating character
* sets and generating actual passwords.
*/
namespace Invium
{
	using System;
	using System.Collections.Generic;
	using System.Security.Cryptography;
	using System.Text.RegularExpressions;

	public class Password
	{
		/*
		* Change these in case password requirements need to be altered.
		*/
		public short[] PasswordLength = { 6, 10, 16, 20, 24, 32, 64, 128 };
		private string csetlower = "abcdefghijklmnopqrstuvwxyz";
		private string csetupper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private string csetdigit = "0123456789";
		private string csetsymbol = "!";
		private string csetsymbolextreme = "@#$%^&*().";
		private string csetlowerregex = "[a-z]";
		private string csetupperregex = "[A-Z]";
		private string csetdigitregex = "[0-9]";
		private string csetsymbolregex = "[\\!]";
		private string csetsymbolextremeregex = "[\\@\\#\\$\\%\\^\\&\\*\\(\\)\\.]";

		/*
		* Generate a password.
		*/
		public string Generate (ref List<Profile> profiles, int password_strength)
		{
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider ();
			byte[] buffer = new byte[this.PasswordLength [password_strength]];
			string[] character_set = this.CharacterSet (password_strength);
			string password = null;
			do {
				password = null;
				rng.GetBytes (buffer);
				for (short i = 0; i < this.PasswordLength [password_strength]; i++) {
					password += character_set [buffer [i] % character_set.Length];
				}
			} while (!this.EvaluatePassword (ref profiles, password, string.Join (string.Empty, character_set)));

			return password;
		}

		/*
		* Generate a salt
		*/
		public string GenSalt ()
		{
			byte[] buffer = new byte[16];
			string[] character_set = this.CharacterSet (this.PasswordLength.GetUpperBound (0));
			new RNGCryptoServiceProvider ().GetBytes (buffer);
			string output = null;
			for (short i = 0; i < 16; i++) {
				output += character_set [buffer [i] % character_set.Length];
			}

			return output;
		}

		/*
		* Evaluate a password
		*
		* Steps:
		* - Check for requirements
		* - Check for doubles across all accounts, services, profiles
		*/
		private bool EvaluatePassword (ref List<Profile> profiles, string password, string character_set)
		{
			// Check for characters
			if (!Regex.IsMatch (password, this.csetlowerregex, RegexOptions.None)) {
				return false;
			}

			if (!Regex.IsMatch (password, this.csetupperregex, RegexOptions.None)) {
				return false;
			}

			if (!Regex.IsMatch (password, this.csetdigitregex, RegexOptions.None)) {
				return false;
			}

			if (!Regex.IsMatch (password, this.csetsymbolregex, RegexOptions.None) && Regex.IsMatch (character_set, "[" + this.csetsymbol + "]", RegexOptions.None)) {
				return false;
			}

			if (!Regex.IsMatch (password, this.csetsymbolextremeregex, RegexOptions.None) && Regex.IsMatch (character_set, "[" + this.csetsymbolextreme + "]", RegexOptions.None)) {
				return false;
			}

			// Check for doubles across all profiles, services and accounts
			foreach (Profile p in profiles) {
				foreach (Service s in p.Services) {
					/*
					 * The following alternative seems more eloquant but after careful
					 * measurement, it was found to be at best on-par in terms of performance
					 * and in practice more erratic performance wise.
					 * if (s.Accounts.Find (x => (x.Name == password) || (x.Password == password)) != null) {
					 * return false;
					 * }
					 */
					foreach (Account a in s.Accounts) {
						if (password == a.Password) {
							return false;
						}

						if (password == a.Name) {
							return false;
						}
					}
				}
			}

			// If we reach this point, the password should have passed
			return true;
		}

		/*
		* Build the character set to use when generating a password.
		*
		* This is based on the passwordStrength control.
		*/
		private string[] CharacterSet (int password_strength)
		{
			string character_set = string.Empty;

			character_set += this.csetlower;
			character_set += this.csetupper;
			character_set += this.csetdigit;
			if (password_strength > 1) {
				character_set += this.csetsymbol;
			}

			if (password_strength > 5) {
				character_set += this.csetsymbolextreme;
			}

			List<string> l = new List<string> ();
			l.AddRange (Regex.Split (character_set, string.Empty));
			l.RemoveAll (x => x == string.Empty);

			return l.ToArray ();
		}
	}
}