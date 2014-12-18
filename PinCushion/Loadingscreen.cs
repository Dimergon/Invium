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

namespace PinCushion
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public class Loadingscreen : Form
	{
		private System.Windows.Forms.Label prompt = new System.Windows.Forms.Label ();

		public Loadingscreen ()
		{
			this.SuspendLayout ();
			this.prompt.SuspendLayout ();
			this.prompt.SetBounds (10, 10, 200, 20);
			this.prompt.Text = Program.Language.Loading;
			this.prompt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.ClientSize = new System.Drawing.Size (this.prompt.Right + 10, this.prompt.Bottom + 10);
			this.ControlBox = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Controls.Add (this.prompt);
			this.prompt.ResumeLayout (false);
			this.ResumeLayout (false);
			this.PerformLayout ();
		}
	}
}