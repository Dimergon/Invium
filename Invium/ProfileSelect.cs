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

// Profile selection popup; used in cloning/merging
namespace Invium
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public class ProfileSelect : Form
	{
		private ComboBox profiles = new ComboBox ();

		public ProfileSelect ()
		{
			this.SuspendLayout ();
			this.profiles.SuspendLayout ();
			this.profiles.Location = new Point (10, 10);
			this.profiles.Size = new Size (200, 20);
			this.StartPosition = FormStartPosition.CenterScreen;
			this.ClientSize = new Size (this.profiles.Right + 10, this.profiles.Bottom + 10);
			this.Controls.Add (this.profiles);
			this.profiles.ResumeLayout (false);
			this.ResumeLayout (false);
			this.PerformLayout ();
		}
	}
}
