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

// Splash screen...
namespace Invium
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public class StorageSplash : Form
	{
		private Label promptLabel;

		public StorageSplash (string prompt)
		{
			this.promptLabel = new Label ();
			this.promptLabel.SuspendLayout ();
			this.SuspendLayout ();
			this.promptLabel.SetBounds (10, 10, 200, 20);
			this.promptLabel.TextAlign = ContentAlignment.MiddleCenter;
			this.promptLabel.Text = prompt;
			this.ControlBox = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.FormBorderStyle = FormBorderStyle.None;
			this.ClientSize = new Size (this.promptLabel.Right + 10, this.promptLabel.Bottom + 10);
			this.StartPosition = FormStartPosition.CenterScreen;
			this.AutoScaleMode = AutoScaleMode.None;
			this.Controls.Add (this.promptLabel);
			this.promptLabel.ResumeLayout (false);
			this.ResumeLayout (false);
			this.PerformLayout ();
		}
	}
}