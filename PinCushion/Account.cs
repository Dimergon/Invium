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
* Container for the actual accounts.
*/
namespace PinCushion
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public class Account
	{
		private string accountname = string.Empty;
		private string accountpassword = string.Empty;
		private string internalpassword = Program.SafePC ? string.Empty : PinCushion.Password.GenSalt ();
		private string internalsalt = Program.SafePC ? string.Empty : PinCushion.Password.GenSalt ();

		public Account (string arg, string arg2)
		{
			this.Name = arg;
			this.Password = arg2;
		}

		public string Name {
			get { return Program.SafePC ? this.accountname : Crypto.Decrypt (this.accountname, this.internalpassword, this.internalsalt); }
			set { this.accountname = Program.SafePC ? value : Crypto.Encrypt (value, this.internalpassword, this.internalsalt); }
		}

		public string Password {
			get { return Program.SafePC ? this.accountpassword : Crypto.Decrypt (this.accountpassword, this.internalpassword, this.internalsalt); }
			set { this.accountpassword = Program.SafePC ? value : Crypto.Encrypt (value, this.internalpassword, this.internalsalt); }
		}
	}
}