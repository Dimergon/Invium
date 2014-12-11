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
* This performs password related functions such as generating character
* sets and generating actual passwords.
*/
namespace PinCushion
{
	using System;
	using System.Collections.Generic;
	using System.Security.Cryptography;
	using System.Text.RegularExpressions;

	public static class Password
	{
		/*
		* Change these in case password requirements need to be altered.
		*/
		public static short[] PasswordLength = { 6, 10, 16, 20, 24, 32, 64, 128 };
		private static string csetlower = "abcdefghijklmnopqrstuvwxyz";
		private static string csetupper = csetlower.ToUpper ();
		private static string csetdigit = "0123456789";
		private static string csetsymbol = "!";
		private static string csetsymbolextreme = "@#$%^&*().";

		/*
		* Generate a seed (based on cryptography)
		*/
		public static int CryptoSeed ()
		{
			RNGCryptoServiceProvider rng_crypto = new RNGCryptoServiceProvider ();
			byte[] rng_crypto_array = new byte[4];
			rng_crypto.GetBytes (rng_crypto_array);
			if (BitConverter.IsLittleEndian) {
				Array.Reverse (rng_crypto_array);
			}

			return BitConverter.ToInt32 (rng_crypto_array, 0);
		}

		/*
		* Generate a password.
		*/
		public static string Generate (ref List<Profile> profiles, int password_strength)
		{
			Random rng = new Random (CryptoSeed ());
			string[] character_set = CharacterSet (password_strength);
			string output = null;
			do {
				string password = null;
				for (short i = 0; i < PasswordLength [password_strength]; i++) {
					password += character_set [rng.Next (character_set.GetLowerBound (0), character_set.GetUpperBound (0))];
				}

				output = password;
			} while (!EvaluatePassword (ref profiles, output, password_strength));

			return output;
		}

		/*
		* Generate a salt
		*/
		public static string GenSalt ()
		{
			Random rng = new Random (CryptoSeed ());
			string[] character_set = CharacterSet (2);
			string output = null;
			do {
				string password = null;
				for (short i = 0; i < 16; i++) {
					password += character_set [rng.Next (character_set.GetLowerBound (0), character_set.GetUpperBound (0))];
				}

				output = password;
			} while (output.Length != 16);

			return output;
		}

		/*
		* Evaluate a password
		*
		* Steps:
		* - Check for requirements
		* - Check for doubles across all accounts, services, profiles
		*/
		public static bool EvaluatePassword (ref List<Profile> profiles, string password, int password_strength)
		{
			/*
			 * Nasty hack
			 *
			 * Sometimes, completely unexpectedly the loop to add characters to
			 * a password ends early, giving us a password shorter than we want.
			 *
			 * As stated, this next check is a nasty hack to avoid accepting such
			 * a malformed password.
			 */
			if (password.Length < PasswordLength [password_strength]) {
				return false;
			}

			// Check for characters
			if (!Regex.IsMatch (password, "[" + csetlower + "]", RegexOptions.None)) {
				return false;
			}

			if (!Regex.IsMatch (password, "[" + csetupper + "]", RegexOptions.None)) {
				return false;
			}

			if (!Regex.IsMatch (password, "[" + csetdigit + "]", RegexOptions.None)) {
				return false;
			}

			if (!Regex.IsMatch (password, "[" + csetsymbol + "]", RegexOptions.None) && password_strength > 1) {
				return false;
			}

			// Check for doubles across all profiles, services and accounts
			foreach (Profile p in profiles) {
				foreach (Service s in p.Profileservices) {
					foreach (Account a in s.ServiceAccounts) {
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
		public static string[] CharacterSet (int password_strength)
		{
			string character_set = string.Empty;

			character_set += csetlower;
			character_set += csetupper;
			character_set += csetdigit;
			if (password_strength > 1) {
				character_set += csetsymbol;
			}

			if (password_strength > 5) {
				character_set += csetsymbolextreme;
			}

			return Regex.Split (character_set, string.Empty);
		}
	}
}