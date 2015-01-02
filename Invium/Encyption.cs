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
* This performs all cryptography related functions (encryption, decryption
* and hashes).
*/
namespace Invium
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Security.Cryptography;
	using System.Text;

	public class InviumCryptography
	{
		/*
		 * Encryption/Decryption, based on AES256 and PBKDF2
		 */
		public string Encrypt (string plainText, string passPhrase, bool fast_encrypt = false)
		{
			string result;
			using (Rijndael algR = Rijndael.Create ()) {
				RNGCryptoServiceProvider rngC = new RNGCryptoServiceProvider ();
				byte[] iv = new byte[16];
				rngC.GetBytes (iv);
				Rfc2898DeriveBytes derived = new Rfc2898DeriveBytes (passPhrase, iv, fast_encrypt ? 10 : 125);
				algR.KeySize = 256;
				algR.BlockSize = 128;
				algR.Key = derived.GetBytes (32);
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

		public string Decrypt (string cipherText, string passPhrase, bool fast_decrypt = false)
		{
			string result;
			using (Rijndael algR = Rijndael.Create ()) {
				using (MemoryStream memoryStream = new MemoryStream (Convert.FromBase64String (cipherText))) {
					byte[] iv = new byte[16];
					memoryStream.Read (iv, 0, 16);
					Rfc2898DeriveBytes derived = new Rfc2898DeriveBytes (passPhrase, iv, fast_decrypt ? 10 : 125);
					algR.KeySize = 256;
					algR.BlockSize = 128;
					algR.Key = derived.GetBytes (32);
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
		 * Generates the hash, based on SHA512 and PBKDF2
		 */
		public string Hash (string input, string salt)
		{
			string result;
			using (SHA512Managed sha512 = new SHA512Managed ()) {
				byte[] salt_bytes = Encoding.UTF8.GetBytes (salt);
				Rfc2898DeriveBytes derived = new Rfc2898DeriveBytes (input, salt_bytes, 1000);
				result = Convert.ToBase64String (sha512.ComputeHash (derived.GetBytes (64)));
			}

			return result;
		}
	}
}