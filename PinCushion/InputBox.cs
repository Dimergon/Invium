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
* This will draw a form with either 1 or 2 text fields to fill out.
* 1 -- Used for singular password entry, (re)naming profiles, services
* or accounts and other such uses.
* 2 -- Used to enter a new password for either an account or PinCushion itself.
*/
namespace PinCushion
{
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public partial class MainForm : Form
	{
		public enum InputBoxMode
		{
			Normal,
			Singlepassword,
			Doublepassword
		}

		public DialogResult Inputbox (InputBoxMode mode, ref string value, string title, string prompt, string prompt2 = null)
		{
			Form form = new Form ();
			Label label = new Label ();
			Label label2 = new Label ();
			TextBox textBox = new TextBox ();
			TextBox textBox2 = new TextBox ();
			Button buttonOk = new Button ();
			Button buttonCancel = new Button ();

			form.Text = title;
			label.Text = prompt;
			label2.Text = prompt2;

			buttonOk.Text = Program.Language.OK;
			buttonCancel.Text = Program.Language.Cancel;
			buttonOk.DialogResult = DialogResult.OK;
			buttonCancel.DialogResult = DialogResult.Cancel;

			switch (mode) {
			case InputBoxMode.Normal:
			case InputBoxMode.Singlepassword:
				label.SetBounds (9, 20, 372, 13);
				textBox.SetBounds (12, 54, 372, 20);
				buttonOk.SetBounds (228, 88, 75, 23);
				buttonCancel.SetBounds (309, 88, 75, 23);
				break;
			case InputBoxMode.Doublepassword:
				label.SetBounds (9, 20, 372, 13);
				textBox.SetBounds (12, 54, 372, 20);
				buttonOk.SetBounds (228, 156, 75, 23);
				buttonCancel.SetBounds (309, 156, 75, 23);
				label2.SetBounds (9, 88, 372, 13);
				textBox2.SetBounds (12, 122, 372, 20);
				break;
			default:
				break;
			}

			textBox.UseSystemPasswordChar = textBox2.UseSystemPasswordChar = (mode == InputBoxMode.Normal) ? false : true;

			if (mode == InputBoxMode.Doublepassword) {
				textBox.TextChanged += new System.EventHandler (delegate {
					buttonOk.Enabled = (textBox.Text == string.Empty) ? false : ((textBox.Text == textBox2.Text) ? true : false);
				});
				textBox2.TextChanged += new System.EventHandler (delegate {
					buttonOk.Enabled = (textBox.Text == string.Empty) ? false : ((textBox.Text == textBox2.Text) ? true : false);
				});
				buttonOk.Enabled = false;
			} else {
				textBox.TextChanged += new System.EventHandler (delegate {
					buttonOk.Enabled = (textBox.Text == string.Empty) ? false : true;
				});
				buttonOk.Enabled = false;
			}

			form.ClientSize = new Size (buttonCancel.Right + 10, buttonCancel.Bottom + 10);
			if (mode == InputBoxMode.Doublepassword) {
				form.Controls.AddRange (new Control[] { label, label2, textBox, textBox2, buttonOk, buttonCancel });
			} else {
				form.Controls.AddRange (new Control[] { label, textBox, buttonOk, buttonCancel });
			}

			form.FormBorderStyle = FormBorderStyle.FixedDialog;
			form.StartPosition = FormStartPosition.CenterScreen;
			form.MinimizeBox = form.MaximizeBox = false;
			form.AcceptButton = buttonOk;
			form.CancelButton = buttonCancel;

			DialogResult dialogResult = form.ShowDialog ();
			value = textBox.Text;
			return dialogResult;
		}
	}
}