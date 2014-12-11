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
* Container for the profiles (users) that make use of the running
* copy of PinCushion.
*/
namespace PinCushion
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public class Profile
	{
		public List<Service> Profileservices = new List<Service> ();
		private string profilename = string.Empty;
		private string internalpassword = Program.SafePC ? string.Empty : Password.GenSalt ();
		private string internalsalt = Program.SafePC ? string.Empty : Password.GenSalt ();

		public Profile (string arg)
		{
			this.Name = arg;
		}

		public string Name {
			get { return Program.SafePC ? this.profilename : Crypto.Decrypt (this.profilename, this.internalpassword, this.internalsalt); }
			set { this.profilename = Program.SafePC ? value : Crypto.Encrypt (value, this.internalpassword, this.internalsalt); }
		}
	}
}