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
* Originally VS Studio generated, this is the container that shapes
* the UI of PinCushion.
*/
namespace PinCushion
{
	public partial class MainForm
	{
		private System.Windows.Forms.ComboBox profileSelection;
		private System.Windows.Forms.ComboBox serviceSelection;
		private System.Windows.Forms.Label profileLabel;
		private System.Windows.Forms.Label serviceLabel;
		private System.Windows.Forms.Button addProfile;
		private System.Windows.Forms.Button removeProfile;
		private System.Windows.Forms.Button renameProfile;
		private System.Windows.Forms.Button addService;
		private System.Windows.Forms.Button removeService;
		private System.Windows.Forms.Button renameService;
		private System.Windows.Forms.ComboBox accountSelection;
		private System.Windows.Forms.Label accountLabel;
		private System.Windows.Forms.Button addAccount;
		private System.Windows.Forms.Button removeAccount;
		private System.Windows.Forms.Button renameAccount;
		private System.Windows.Forms.TextBox accountPassword;
		private System.Windows.Forms.Button setPassword;
		private System.Windows.Forms.Button generatePassword;
		private System.Windows.Forms.Label passwordLabel;
		private System.Windows.Forms.TrackBar passwordStrength;
		private System.Windows.Forms.CheckBox readOnly;
		private System.Windows.Forms.CheckBox encrypt;
		private System.Windows.Forms.Timer idleTimer;
		private System.Windows.Forms.ContextMenuStrip copyTextToClipboardRightclick;
		private System.Windows.Forms.ToolStripMenuItem copyTextToClipboardToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip serviceRightclick;
		private System.Windows.Forms.ToolStripMenuItem executeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem setexecuteToolStripMenuItem;
		private System.Windows.Forms.CheckBox showPassword;
		private System.Windows.Forms.Label passwordStrengthLabel;
		private System.Windows.Forms.NotifyIcon tray;
		private System.Windows.Forms.ContextMenuStrip mainFormRightclick;
		private System.Windows.Forms.ToolStripMenuItem disableIdleTimeoutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
		private System.Windows.Forms.Button setPinCushionPassword;
		private System.Windows.Forms.Label passwordStrengthDescription;

		private System.ComponentModel.IContainer components = null;

		protected override void Dispose (bool disposing)
		{
			if (disposing && (this.components != null)) {
				this.components.Dispose ();
			}

			base.Dispose (disposing);
		}

		private void InitializeComponent ()
		{
			this.components = new System.ComponentModel.Container ();
			this.profileSelection = new System.Windows.Forms.ComboBox ();
			this.serviceSelection = new System.Windows.Forms.ComboBox ();
			this.profileLabel = new System.Windows.Forms.Label ();
			this.serviceLabel = new System.Windows.Forms.Label ();
			this.addProfile = new System.Windows.Forms.Button ();
			this.removeProfile = new System.Windows.Forms.Button ();
			this.renameProfile = new System.Windows.Forms.Button ();
			this.addService = new System.Windows.Forms.Button ();
			this.removeService = new System.Windows.Forms.Button ();
			this.renameService = new System.Windows.Forms.Button ();
			this.accountSelection = new System.Windows.Forms.ComboBox ();
			this.accountLabel = new System.Windows.Forms.Label ();
			this.addAccount = new System.Windows.Forms.Button ();
			this.removeAccount = new System.Windows.Forms.Button ();
			this.renameAccount = new System.Windows.Forms.Button ();
			this.accountPassword = new System.Windows.Forms.TextBox ();
			this.copyTextToClipboardRightclick = new System.Windows.Forms.ContextMenuStrip (this.components);
			this.copyTextToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
			this.serviceRightclick = new System.Windows.Forms.ContextMenuStrip (this.components);
			this.executeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
			this.setexecuteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
			this.setPassword = new System.Windows.Forms.Button ();
			this.generatePassword = new System.Windows.Forms.Button ();
			this.passwordLabel = new System.Windows.Forms.Label ();
			this.passwordStrength = new System.Windows.Forms.TrackBar ();
			this.readOnly = new System.Windows.Forms.CheckBox ();
			this.encrypt = new System.Windows.Forms.CheckBox ();
			this.idleTimer = new System.Windows.Forms.Timer (this.components);
			this.showPassword = new System.Windows.Forms.CheckBox ();
			this.passwordStrengthLabel = new System.Windows.Forms.Label ();
			this.tray = new System.Windows.Forms.NotifyIcon (this.components);
			this.mainFormRightclick = new System.Windows.Forms.ContextMenuStrip (this.components);
			this.disableIdleTimeoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
			this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
			this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem ();
			this.setPinCushionPassword = new System.Windows.Forms.Button ();
			this.passwordStrengthDescription = new System.Windows.Forms.Label ();
			this.copyTextToClipboardRightclick.SuspendLayout ();
			this.serviceRightclick.SuspendLayout ();
			((System.ComponentModel.ISupportInitialize)this.passwordStrength).BeginInit ();
			this.mainFormRightclick.SuspendLayout ();
			this.SuspendLayout ();
			this.profileSelection.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.profileSelection.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.profileSelection.ContextMenu = null;
			this.profileSelection.ContextMenuStrip = this.copyTextToClipboardRightclick;
			this.profileSelection.FormattingEnabled = true;
			this.profileSelection.Location = new System.Drawing.Point (143, 15);
			this.profileSelection.Margin = new System.Windows.Forms.Padding (4);
			this.profileSelection.Size = new System.Drawing.Size (508, 24);
			this.profileSelection.TabIndex = 0;
			this.profileSelection.SelectedIndexChanged += new System.EventHandler (this.ProfileSelection_SelectedIndexChanged);
			this.profileSelection.TextUpdate += new System.EventHandler (this.NotIdle);
			this.serviceSelection.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.serviceSelection.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.serviceSelection.ContextMenu = null;
			this.serviceSelection.ContextMenuStrip = this.serviceRightclick;
			this.serviceSelection.FormattingEnabled = true;
			this.serviceSelection.Location = new System.Drawing.Point (143, 96);
			this.serviceSelection.Margin = new System.Windows.Forms.Padding (4);
			this.serviceSelection.Size = new System.Drawing.Size (508, 24);
			this.serviceSelection.TabIndex = 1;
			this.serviceSelection.SelectedIndexChanged += new System.EventHandler (this.ServiceSelection_SelectedIndexChanged);
			this.serviceSelection.TextUpdate += new System.EventHandler (this.NotIdle);
			this.profileLabel.Location = new System.Drawing.Point (12, 18);
			this.profileLabel.Margin = new System.Windows.Forms.Padding (4, 0, 4, 0);
			this.profileLabel.Size = new System.Drawing.Size (52, 17);
			this.serviceLabel.Location = new System.Drawing.Point (12, 99);
			this.serviceLabel.Margin = new System.Windows.Forms.Padding (4, 0, 4, 0);
			this.serviceLabel.Size = new System.Drawing.Size (59, 17);
			this.addProfile.Location = new System.Drawing.Point (179, 47);
			this.addProfile.Margin = new System.Windows.Forms.Padding (4);
			this.addProfile.Size = new System.Drawing.Size (152, 41);
			this.addProfile.TabStop = false;
			this.addProfile.Click += new System.EventHandler (this.AddProfile_Click);
			this.removeProfile.Location = new System.Drawing.Point (339, 47);
			this.removeProfile.Margin = new System.Windows.Forms.Padding (4);
			this.removeProfile.Size = new System.Drawing.Size (152, 41);
			this.removeProfile.TabStop = false;
			this.removeProfile.Click += new System.EventHandler (this.RemoveProfile_Click);
			this.renameProfile.Location = new System.Drawing.Point (499, 47);
			this.renameProfile.Margin = new System.Windows.Forms.Padding (4);
			this.renameProfile.Size = new System.Drawing.Size (152, 41);
			this.renameProfile.TabStop = false;
			this.renameProfile.Click += new System.EventHandler (this.RenameProfile_Click);
			this.addService.Location = new System.Drawing.Point (179, 128);
			this.addService.Margin = new System.Windows.Forms.Padding (4);
			this.addService.Size = new System.Drawing.Size (152, 39);
			this.addService.TabStop = false;
			this.addService.Click += new System.EventHandler (this.AddService_Click);
			this.removeService.Location = new System.Drawing.Point (339, 128);
			this.removeService.Margin = new System.Windows.Forms.Padding (4);
			this.removeService.Size = new System.Drawing.Size (152, 39);
			this.removeService.TabStop = false;
			this.removeService.Click += new System.EventHandler (this.RemoveService_Click);
			this.renameService.Location = new System.Drawing.Point (499, 128);
			this.renameService.Margin = new System.Windows.Forms.Padding (4);
			this.renameService.Size = new System.Drawing.Size (152, 39);
			this.renameService.TabStop = false;
			this.renameService.Click += new System.EventHandler (this.RenameService_Click);
			this.accountSelection.FormattingEnabled = true;
			this.accountSelection.ContextMenu = null;
			this.accountSelection.ContextMenuStrip = this.copyTextToClipboardRightclick;
			this.accountSelection.Location = new System.Drawing.Point (143, 175);
			this.accountSelection.Margin = new System.Windows.Forms.Padding (4);
			this.accountSelection.Size = new System.Drawing.Size (508, 24);
			this.accountSelection.TabIndex = 2;
			this.accountSelection.SelectedIndexChanged += new System.EventHandler (this.AccountSelection_SelectedIndexChanged);
			this.accountSelection.TextUpdate += new System.EventHandler (this.NotIdle);
			this.accountLabel.Location = new System.Drawing.Point (12, 178);
			this.accountLabel.Margin = new System.Windows.Forms.Padding (4, 0, 4, 0);
			this.accountLabel.Size = new System.Drawing.Size (63, 17);
			this.addAccount.Location = new System.Drawing.Point (180, 207);
			this.addAccount.Margin = new System.Windows.Forms.Padding (4);
			this.addAccount.Size = new System.Drawing.Size (152, 39);
			this.addAccount.TabStop = false;
			this.addAccount.Click += new System.EventHandler (this.AddAccount_Click);
			this.removeAccount.Location = new System.Drawing.Point (340, 207);
			this.removeAccount.Margin = new System.Windows.Forms.Padding (4);
			this.removeAccount.Size = new System.Drawing.Size (152, 39);
			this.removeAccount.TabStop = false;
			this.removeAccount.Click += new System.EventHandler (this.RemoveAccount_Click);
			this.renameAccount.Location = new System.Drawing.Point (498, 207);
			this.renameAccount.Margin = new System.Windows.Forms.Padding (4);
			this.renameAccount.Size = new System.Drawing.Size (153, 39);
			this.renameAccount.TabStop = false;
			this.renameAccount.Click += new System.EventHandler (this.RenameAccount_Click);
			this.accountPassword.ContextMenu = null;
			this.accountPassword.ContextMenuStrip = this.copyTextToClipboardRightclick;
			this.accountPassword.Location = new System.Drawing.Point (143, 254);
			this.accountPassword.Margin = new System.Windows.Forms.Padding (4);
			this.accountPassword.ReadOnly = true;
			this.accountPassword.Size = new System.Drawing.Size (508, 22);
			this.accountPassword.TabIndex = 3;
			this.accountPassword.UseSystemPasswordChar = true;
			this.copyTextToClipboardRightclick.Size = new System.Drawing.Size (199, 28);
			this.copyTextToClipboardRightclick.Items.AddRange (new System.Windows.Forms.ToolStripItem[] {
				this.copyTextToClipboardToolStripMenuItem
			});
			this.copyTextToClipboardToolStripMenuItem.Size = new System.Drawing.Size (198, 24);
			this.copyTextToClipboardToolStripMenuItem.Click += new System.EventHandler (this.CopyTextToClipboardToolStripMenuItem_Click);
			this.serviceRightclick.Size = new System.Drawing.Size (199, 28);
			this.serviceRightclick.Items.AddRange (new System.Windows.Forms.ToolStripItem[] {
				this.executeToolStripMenuItem,
				this.setexecuteToolStripMenuItem
			});
			this.executeToolStripMenuItem.Size = new System.Drawing.Size (198, 24);
			this.executeToolStripMenuItem.Click += new System.EventHandler (this.ExecuteToolStripMenuItem_Click);
			this.setexecuteToolStripMenuItem.Size = new System.Drawing.Size (198, 24);
			this.setexecuteToolStripMenuItem.Click += new System.EventHandler (this.SetexecuteToolStripMenuItem_Click);
			this.setPassword.Location = new System.Drawing.Point (180, 284);
			this.setPassword.Margin = new System.Windows.Forms.Padding (4);
			this.setPassword.Size = new System.Drawing.Size (152, 39);
			this.setPassword.TabStop = false;
			this.setPassword.Click += new System.EventHandler (this.SetPassword_Click);
			this.generatePassword.Location = new System.Drawing.Point (340, 284);
			this.generatePassword.Margin = new System.Windows.Forms.Padding (4);
			this.generatePassword.Size = new System.Drawing.Size (152, 39);
			this.generatePassword.TabStop = false;
			this.generatePassword.Click += new System.EventHandler (this.GeneratePassword_Click);
			this.passwordLabel.Location = new System.Drawing.Point (12, 257);
			this.passwordLabel.Margin = new System.Windows.Forms.Padding (4, 0, 4, 0);
			this.passwordLabel.Size = new System.Drawing.Size (73, 17);
			this.passwordStrength.Location = new System.Drawing.Point (498, 284);
			this.passwordStrength.Margin = new System.Windows.Forms.Padding (4);
			this.passwordStrength.Maximum = Password.PasswordLength.GetLength (0) - 1;
			this.passwordStrength.Size = new System.Drawing.Size (153, 56);
			this.passwordStrength.TabStop = false;
			this.passwordStrength.Value = 2;
			this.passwordStrength.ValueChanged += new System.EventHandler (this.PasswordStrength_ValueChanged);
			this.readOnly.Checked = true;
			this.readOnly.CheckState = System.Windows.Forms.CheckState.Checked;
			this.readOnly.Location = new System.Drawing.Point (15, 324);
			this.readOnly.Margin = new System.Windows.Forms.Padding (4);
			this.readOnly.Size = new System.Drawing.Size (97, 21);
			this.readOnly.TabStop = false;
			this.readOnly.CheckedChanged += new System.EventHandler (this.ReadOnly_CheckedChanged);
			this.encrypt.Checked = true;
			this.encrypt.CheckState = System.Windows.Forms.CheckState.Checked;
			this.encrypt.Location = new System.Drawing.Point (15, 353);
			this.encrypt.Margin = new System.Windows.Forms.Padding (4);
			this.encrypt.Size = new System.Drawing.Size (112, 21);
			this.encrypt.TabStop = false;
			this.encrypt.CheckedChanged += new System.EventHandler (this.NotIdle);
			this.idleTimer.Enabled = true;
			this.idleTimer.Interval = 1000;
			this.idleTimer.Tick += new System.EventHandler (this.IdleTimer_Tick);
			this.showPassword.Location = new System.Drawing.Point (15, 294);
			this.showPassword.Size = new System.Drawing.Size (129, 21);
			this.showPassword.TabStop = false;
			this.showPassword.CheckedChanged += new System.EventHandler (this.ShowPassword_CheckedChanged);
			this.passwordStrengthLabel.Location = new System.Drawing.Point (496, 330);
			this.passwordStrengthLabel.Size = new System.Drawing.Size (127, 17);
			this.tray.ContextMenu = null;
			this.tray.ContextMenuStrip = null;
			this.tray.Visible = false;
			this.mainFormRightclick.Items.AddRange (new System.Windows.Forms.ToolStripItem[] {
				this.disableIdleTimeoutToolStripMenuItem,
				this.languageToolStripMenuItem,
				this.importToolStripMenuItem
			});
			this.mainFormRightclick.Size = new System.Drawing.Size (214, 76);
			this.disableIdleTimeoutToolStripMenuItem.Size = new System.Drawing.Size (213, 24);
			this.disableIdleTimeoutToolStripMenuItem.Click += new System.EventHandler (this.DisableIdleTimeoutToolStripMenuItem_Click);
			this.languageToolStripMenuItem.Size = new System.Drawing.Size (213, 24);
			this.importToolStripMenuItem.Size = new System.Drawing.Size (213, 24);
			this.importToolStripMenuItem.Click += new System.EventHandler (this.ImportToolStripMenuItem_Click);
			this.setPinCushionPassword.Location = new System.Drawing.Point (180, 330);
			this.setPinCushionPassword.Size = new System.Drawing.Size (312, 39);
			this.setPinCushionPassword.TabStop = false;
			this.setPinCushionPassword.Click += new System.EventHandler (this.SetPinCushionPassword_Click);
			this.passwordStrengthDescription.Location = new System.Drawing.Point (585, 354);
			this.passwordStrengthDescription.Size = new System.Drawing.Size (66, 17);
			this.AutoScaleDimensions = new System.Drawing.SizeF (8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.ClientSize = new System.Drawing.Size (this.passwordStrengthDescription.Right + 20, this.passwordStrengthDescription.Bottom + 10);
			this.ContextMenu = null;
			this.ContextMenuStrip = this.mainFormRightclick;
			this.Controls.Add (this.passwordStrengthDescription);
			this.Controls.Add (this.setPinCushionPassword);
			this.Controls.Add (this.passwordStrengthLabel);
			this.Controls.Add (this.showPassword);
			this.Controls.Add (this.encrypt);
			this.Controls.Add (this.readOnly);
			this.Controls.Add (this.passwordStrength);
			this.Controls.Add (this.passwordLabel);
			this.Controls.Add (this.generatePassword);
			this.Controls.Add (this.setPassword);
			this.Controls.Add (this.accountPassword);
			this.Controls.Add (this.renameAccount);
			this.Controls.Add (this.removeAccount);
			this.Controls.Add (this.addAccount);
			this.Controls.Add (this.accountLabel);
			this.Controls.Add (this.accountSelection);
			this.Controls.Add (this.renameService);
			this.Controls.Add (this.removeService);
			this.Controls.Add (this.addService);
			this.Controls.Add (this.renameProfile);
			this.Controls.Add (this.removeProfile);
			this.Controls.Add (this.addProfile);
			this.Controls.Add (this.serviceLabel);
			this.Controls.Add (this.profileLabel);
			this.Controls.Add (this.serviceSelection);
			this.Controls.Add (this.profileSelection);
			this.Margin = new System.Windows.Forms.Padding (4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Load += new System.EventHandler (this.MainForm_Load);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler (this.MainForm_Closing);
			this.copyTextToClipboardRightclick.ResumeLayout (false);
			this.serviceRightclick.ResumeLayout (false);
			((System.ComponentModel.ISupportInitialize)this.passwordStrength).EndInit ();
			this.mainFormRightclick.ResumeLayout (false);
			this.ResumeLayout (false);
			this.PerformLayout ();
		}
	}
}