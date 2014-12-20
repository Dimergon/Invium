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
	using System.Net;
	using System.Text;

	public class Account
	{
		private NetworkCredential data = new NetworkCredential ();

		public Account (string arg, string arg2)
		{
			this.data.UserName = arg;
			this.data.Password = arg2;
		}

		public string Name {
			get { return this.data.UserName; }
			set { this.data.UserName = value; }
		}

		public string Password {
			get { return this.data.Password; }
			set { this.data.Password = value; }
		}
	}
}