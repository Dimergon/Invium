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

// Inputbox, used in various places.
namespace Invium
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public class InputBox : Form
	{
		private Label label;
		private Label label2;
		private TextBox textBox;
		private TextBox textBox2;
		private Button buttonOk;
		private Button buttonCancel;

		public enum Mode
		{
			Normal,
			Singlepassword,
			Doublepassword
		}

		public DialogResult ShowMe (Mode mode, ref string value, string title, string prompt, string prompt2 = null)
		{
			this.label = new Label ();
			this.label2 = new Label ();
			this.textBox = new TextBox ();
			this.textBox2 = new TextBox ();
			this.buttonOk = new Button ();
			this.buttonCancel = new Button ();
			this.label.SuspendLayout ();
			this.label2.SuspendLayout ();
			this.textBox.SuspendLayout ();
			this.textBox2.SuspendLayout ();
			this.buttonOk.SuspendLayout ();
			this.buttonCancel.SuspendLayout ();
			this.label.Text = prompt;
			this.label2.Text = prompt2;
			this.Text = title;
			this.buttonOk.Text = Program.Language.OK;
			this.buttonCancel.Text = Program.Language.Cancel;
			this.buttonOk.DialogResult = DialogResult.OK;
			this.buttonCancel.DialogResult = DialogResult.Cancel;
			switch (mode) {
			case Mode.Normal:
			case Mode.Singlepassword:
				this.label.SetBounds (9, 20, 372, 13);
				this.textBox.SetBounds (12, 54, 372, 20);
				this.buttonOk.SetBounds (228, 88, 75, 23);
				this.buttonCancel.SetBounds (309, 88, 75, 23);
				break;
			case Mode.Doublepassword:
				this.label.SetBounds (9, 20, 372, 13);
				this.textBox.SetBounds (12, 54, 372, 20);
				this.buttonOk.SetBounds (228, 156, 75, 23);
				this.buttonCancel.SetBounds (309, 156, 75, 23);
				this.label2.SetBounds (9, 88, 372, 13);
				this.textBox2.SetBounds (12, 122, 372, 20);
				break;
			default:
				break;
			}

			this.textBox.UseSystemPasswordChar = this.textBox2.UseSystemPasswordChar = (mode == Mode.Normal) ? false : true;

			if (mode == Mode.Doublepassword) {
				this.textBox.TextChanged += (object sender, EventArgs e) => {
					this.buttonOk.Enabled = (this.textBox.Text == string.Empty) ? false : ((this.textBox.Text == this.textBox2.Text) ? true : false);
				};
				this.textBox2.TextChanged += (object sender, EventArgs e) => {
					this.buttonOk.Enabled = (this.textBox.Text == string.Empty) ? false : ((this.textBox.Text == this.textBox2.Text) ? true : false);
				};
				this.buttonOk.Enabled = false;
			} else {
				this.textBox.TextChanged += (object sender, EventArgs e) => {
					this.buttonOk.Enabled = (this.textBox.Text == string.Empty) ? false : true;
				};
				this.buttonOk.Enabled = false;
			}

			this.ClientSize = new Size (this.buttonCancel.Right + 10, this.buttonCancel.Bottom + 10);
			if (mode == Mode.Doublepassword) {
				this.Controls.AddRange (new Control[] {
					this.label,
					this.label2,
					this.textBox,
					this.textBox2,
					this.buttonOk,
					this.buttonCancel
				});
			} else {
				this.Controls.AddRange (new Control[] { this.label, this.textBox, this.buttonOk, this.buttonCancel });
			}

			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.StartPosition = FormStartPosition.CenterScreen;
			this.AutoScaleMode = AutoScaleMode.None;
			this.ControlBox = this.MinimizeBox = this.MaximizeBox = false;
			this.AcceptButton = this.buttonOk;
			this.CancelButton = this.buttonCancel;

			this.label.ResumeLayout (false);
			this.label2.ResumeLayout (false);
			this.textBox.ResumeLayout (false);
			this.textBox2.ResumeLayout (false);
			this.buttonOk.ResumeLayout (false);
			this.buttonCancel.ResumeLayout (false);
			this.ResumeLayout (false);
			this.PerformLayout ();

			DialogResult dialogResult = this.ShowDialog ();
			value = this.textBox.Text;
			return dialogResult;
		}
	}
}