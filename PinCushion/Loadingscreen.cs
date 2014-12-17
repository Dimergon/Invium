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

// Shows a loadingscreen
namespace PinCushion
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public class Loadingscreen : Form
	{
		private System.Windows.Forms.Label loadingScreenLabel;

		public Loadingscreen ()
		{
			this.InitializeComponent ();
		}

		private void InitializeComponent ()
		{
			this.SuspendLayout ();
			this.loadingScreenLabel = new Label ();
			this.loadingScreenLabel.SuspendLayout ();
			this.loadingScreenLabel.SetBounds (10, 10, 200, 20);
			this.loadingScreenLabel.TextAlign = ContentAlignment.MiddleCenter;
			this.loadingScreenLabel.Text = Program.Language.Loading;
			this.FormBorderStyle = FormBorderStyle.None;
			this.ControlBox = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.ClientSize = new Size (this.loadingScreenLabel.Right + 10, this.loadingScreenLabel.Bottom + 10);
			this.StartPosition = FormStartPosition.CenterScreen;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler (this.Release);
			this.Controls.Add (this.loadingScreenLabel);
			this.loadingScreenLabel.ResumeLayout (false);
			this.ResumeLayout (false);
			this.PerformLayout ();
		}

		private void Release (object sender, EventArgs e)
		{
			this.Dispose ();
		}
	}
}
