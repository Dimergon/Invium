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
* This performs all cryptography related functions (encryption, decryption
* and hashesh).
*/
namespace PinCushion
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Security.Cryptography;
	using System.Text;

	public static class Crypto
	{
		/*
		* Encryption/Decryption is in AES-256 bit.
		*/
		public static string Encrypt (string plainText, string passPhrase, string salt)
		{
			using (MemoryStream memoryStream = new MemoryStream ()) {
				byte[] saltBytes = Encoding.UTF8.GetBytes (salt);
				byte[] plainTextBytes = Encoding.UTF8.GetBytes (plainText);
				PasswordDeriveBytes password = new PasswordDeriveBytes (passPhrase, null);
				byte[] keyBytes = password.GetBytes (32);
				RijndaelManaged symmetricKey = new RijndaelManaged ();
				symmetricKey.Mode = CipherMode.CBC;
				ICryptoTransform encryptor = symmetricKey.CreateEncryptor (keyBytes, saltBytes);
				CryptoStream cryptoStream = new CryptoStream (memoryStream, encryptor, CryptoStreamMode.Write);
				cryptoStream.Write (plainTextBytes, 0, plainTextBytes.Length);
				cryptoStream.FlushFinalBlock ();
				byte[] cipherTextBytes = memoryStream.ToArray ();
				return Convert.ToBase64String (cipherTextBytes);
			}
		}

		public static string Decrypt (string cipherText, string passPhrase, string salt)
		{
			using (MemoryStream memoryStream = new MemoryStream (Convert.FromBase64String (cipherText))) {
				byte[] saltBytes = Encoding.UTF8.GetBytes (salt);
				byte[] cipherTextBytes = Convert.FromBase64String (cipherText);
				PasswordDeriveBytes password = new PasswordDeriveBytes (passPhrase, null);
				byte[] keyBytes = password.GetBytes (32);
				RijndaelManaged symmetricKey = new RijndaelManaged ();
				symmetricKey.Mode = CipherMode.CBC;
				ICryptoTransform decryptor = symmetricKey.CreateDecryptor (keyBytes, saltBytes);
				CryptoStream cryptoStream = new CryptoStream (memoryStream, decryptor, CryptoStreamMode.Read);
				byte[] plainTextBytes = new byte[cipherTextBytes.Length];
				int decryptedByteCount = cryptoStream.Read (plainTextBytes, 0, plainTextBytes.Length);
				return Encoding.UTF8.GetString (plainTextBytes, 0, decryptedByteCount);
			}
		}

		/*
        * A SHA512 mash hash generator
        *
        * This will mash together the input and the salt into a single array by even and odd positions.
        * So:
        *
        * input_bytes will be placed at 0, 2, 4, 6, ..
        * salt_bytes will be placed at 1, 3, 5, 7, ..
        *
        * The hash is based on the mash.
        */
		public static string Hash (string input, string salt)
		{
			byte[] salt_bytes = Encoding.UTF8.GetBytes (salt);
			byte[] input_bytes = Encoding.UTF8.GetBytes (input);
			byte[] combined_bytes = new byte[2 * (input_bytes.Length + salt_bytes.Length)];

			short i = 0;

			Action<byte> copyinput = new Action<byte> (delegate {
				Array.Copy (input_bytes, i, combined_bytes, i * 2, 1);
				i++;
			});

			Action<byte> copysalt = new Action<byte> (delegate {
				Array.Copy (salt_bytes, i, combined_bytes, (i * 2) + 1, 1);
				i++;
			});

			Array.ForEach (input_bytes, copyinput);
			i = 0;
			Array.ForEach (salt_bytes, copysalt);

			List<byte> combined_list = new List<byte> ();
			combined_list.AddRange (combined_bytes);
			combined_list.RemoveAll (x => x == 0);                               // dump excess 0's (non-assigned bytes), clean-up
			return Convert.ToBase64String (new SHA512Managed ().ComputeHash (combined_list.ToArray ()));
		}
	}
}