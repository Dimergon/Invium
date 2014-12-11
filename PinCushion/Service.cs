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
* Container for the services the selected profile (user) has accounts with.
*/
namespace PinCushion
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public class Service
	{
		public List<Account> ServiceAccounts = new List<Account> ();
		private string servicename = string.Empty;
		private string servicecommand = string.Empty;
		private string internalpassword = Program.SafePC ? string.Empty : Password.GenSalt ();
		private string internalsalt = Program.SafePC ? string.Empty : Password.GenSalt ();

		public Service (string arg0, string arg1)
		{
			this.Name = arg0;
			this.Command = arg1;
		}

		public string Name {
			get { return Program.SafePC ? this.servicename : Crypto.Decrypt (this.servicename, this.internalpassword, this.internalsalt); }
			set { this.servicename = Program.SafePC ? value : Crypto.Encrypt (value, this.internalpassword, this.internalsalt); }
		}

		public string Command {
			get { return Program.SafePC ? this.servicecommand : Crypto.Decrypt (this.servicecommand, this.internalpassword, this.internalsalt); }
			set { this.servicecommand = Program.SafePC ? value : Crypto.Encrypt (value, this.internalpassword, this.internalsalt); }
		}
	}
}