﻿/*
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
* Container for the actual accounts.
*/
namespace Invium
{
	public class Account
	{
		private string name;
		private string password;
		private string internalpassword = new Password ().GenSalt ();

		public Account (string arg, string arg2)
		{
			this.name = new InviumCryptography ().Encrypt (arg, this.internalpassword, true);
			this.password = new InviumCryptography ().Encrypt (arg2, this.internalpassword, true);
		}

		public string Name {
			get { return new InviumCryptography ().Decrypt (this.name, this.internalpassword, true); }
			set { this.name = new InviumCryptography ().Encrypt (value, this.internalpassword, true); }
		}

		public string Password {
			get { return new InviumCryptography ().Decrypt (this.password, this.internalpassword, true); }
			set { this.password = new InviumCryptography ().Encrypt (value, this.internalpassword, true); }
		}
	}
}