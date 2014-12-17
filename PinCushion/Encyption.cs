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
* and hashes).
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
		public static string Encrypt (string plainText, string passPhrase, bool fast_encrypt = false)
		{
			string result;
			using (Rijndael algR = Rijndael.Create ()) {
				RNGCryptoServiceProvider rngC = new RNGCryptoServiceProvider ();
				byte[] iv = new byte[16];
				rngC.GetBytes (iv);
				Rfc2898DeriveBytes derived = new Rfc2898DeriveBytes (passPhrase, iv, fast_encrypt ? 10 : 100);
				byte[] key = derived.GetBytes (32);
				algR.Key = key;
				algR.IV = iv;
				using (MemoryStream memoryStream = new MemoryStream ()) {
					memoryStream.Write (iv, 0, 16);
					using (CryptoStream cryptoStreamEncrypt = new CryptoStream (memoryStream, algR.CreateEncryptor (algR.Key, algR.IV), CryptoStreamMode.Write)) {
						using (StreamWriter streamWriterEncrypt = new StreamWriter (cryptoStreamEncrypt)) {
							streamWriterEncrypt.Write (plainText);
						}
					}

					result = Convert.ToBase64String (memoryStream.ToArray ());
				}
			}

			return result;
		}

		public static string Decrypt (string cipherText, string passPhrase, bool fast_decrypt = false)
		{
			string result;
			using (Rijndael algR = Rijndael.Create ()) {
				byte[] cipherBytes = Convert.FromBase64String (cipherText);
				using (MemoryStream memoryStream = new MemoryStream (cipherBytes)) {
					byte[] iv = new byte[16];
					memoryStream.Read (iv, 0, 16);
					Rfc2898DeriveBytes derived = new Rfc2898DeriveBytes (passPhrase, iv, fast_decrypt ? 10 : 100);
					byte[] key = derived.GetBytes (32);
					algR.Key = key;
					algR.IV = iv;
					using (CryptoStream cryptoStreamDecrypt = new CryptoStream (memoryStream, algR.CreateDecryptor (algR.Key, algR.IV), CryptoStreamMode.Read)) {
						using (StreamReader streamReaderDecrypt = new StreamReader (cryptoStreamDecrypt)) {
							result = streamReaderDecrypt.ReadToEnd ();
						}
					}
				}
			}

			return result;
		}

		/*
		 * A SHA512 mash hash generator
		 *
		 * This will mash together the input and the salt into a single array by even and odd positions.
		 *
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