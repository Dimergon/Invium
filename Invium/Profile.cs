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
* Container for the profiles (users) that make use of the running
* copy of Invium.
*/
namespace Invium
{
	using System.Collections.Generic;

	public class Profile
	{
		public List<Service> Services = new List<Service> ();
		private string name;
		private string internalpassword = new Password ().GenSalt ();

		public Profile (string arg)
		{
			this.name = new InviumCryptography ().Encrypt (arg, this.internalpassword, true);
		}

		public string Name {
			get { return new InviumCryptography ().Decrypt (this.name, this.internalpassword, true); }
			set { this.name = new InviumCryptography ().Encrypt (value, this.internalpassword, true); }
		}
	}
}