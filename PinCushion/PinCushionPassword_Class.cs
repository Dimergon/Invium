﻿/*
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
* Container for PinCushion's password.
*/
namespace PinCushion
{
	using System;
	using System.Text;

	public class PinCushionPassword_Class
	{
		private string pincushionpassword = string.Empty;
		private string internalpassword = PinCushion.Password.GenSalt ();

		public string Password {
			get { return Crypto.Decrypt (this.pincushionpassword, this.internalpassword, true); }
			set { this.pincushionpassword = Crypto.Encrypt (value, this.internalpassword, true); }
		}
	}
}