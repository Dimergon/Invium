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
* Originally VS Studio generated, this is the container that shapes
* the UI of Invium.
*/
namespace Invium
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
		private System.Windows.Forms.ContextMenuStrip copyTextCM;
		private System.Windows.Forms.ToolStripMenuItem copyTextCMMI;
		private System.Windows.Forms.ContextMenuStrip serviceCM;
		private System.Windows.Forms.ToolStripMenuItem serviceCMexecuteCommand;
		private System.Windows.Forms.ToolStripMenuItem serviceCMsetCommand;
		private System.Windows.Forms.ToolStripMenuItem serviceCMclone;
		private System.Windows.Forms.CheckBox showPassword;
		private System.Windows.Forms.Label passwordStrengthLabel;
		private System.Windows.Forms.NotifyIcon tray;
		private System.Windows.Forms.ContextMenuStrip mainFormCM;
		private System.Windows.Forms.ToolStripMenuItem mainFormCMdisableTimeout;
		private System.Windows.Forms.ToolStripMenuItem mainFormCMsetLanguage;
		private System.Windows.Forms.ToolStripMenuItem mainFormCMimport;
		private System.Windows.Forms.ToolStripMenuItem mainFormCMmergeProfiles;
		private System.Windows.Forms.Button setMasterPassword;
		private System.Windows.Forms.Label passwordStrengthDescription;

		private void InitializeComponent ()
		{
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
			this.copyTextCM = new System.Windows.Forms.ContextMenuStrip ();
			this.copyTextCMMI = new System.Windows.Forms.ToolStripMenuItem ();
			this.serviceCM = new System.Windows.Forms.ContextMenuStrip ();
			this.serviceCMexecuteCommand = new System.Windows.Forms.ToolStripMenuItem ();
			this.serviceCMsetCommand = new System.Windows.Forms.ToolStripMenuItem ();
			this.serviceCMclone = new System.Windows.Forms.ToolStripMenuItem ();
			this.setPassword = new System.Windows.Forms.Button ();
			this.generatePassword = new System.Windows.Forms.Button ();
			this.passwordLabel = new System.Windows.Forms.Label ();
			this.passwordStrength = new System.Windows.Forms.TrackBar ();
			this.readOnly = new System.Windows.Forms.CheckBox ();
			this.encrypt = new System.Windows.Forms.CheckBox ();
			this.idleTimer = new System.Windows.Forms.Timer ();
			this.showPassword = new System.Windows.Forms.CheckBox ();
			this.passwordStrengthLabel = new System.Windows.Forms.Label ();
			this.tray = new System.Windows.Forms.NotifyIcon ();
			this.mainFormCM = new System.Windows.Forms.ContextMenuStrip ();
			this.mainFormCMdisableTimeout = new System.Windows.Forms.ToolStripMenuItem ();
			this.mainFormCMsetLanguage = new System.Windows.Forms.ToolStripMenuItem ();
			this.mainFormCMimport = new System.Windows.Forms.ToolStripMenuItem ();
			this.mainFormCMmergeProfiles = new System.Windows.Forms.ToolStripMenuItem ();
			this.setMasterPassword = new System.Windows.Forms.Button ();
			this.passwordStrengthDescription = new System.Windows.Forms.Label ();
			this.copyTextCM.SuspendLayout ();
			this.serviceCM.SuspendLayout ();
			this.mainFormCM.SuspendLayout ();
			this.SuspendLayout ();
			this.profileSelection.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.profileSelection.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.profileSelection.ContextMenu = null;
			this.profileSelection.ContextMenuStrip = this.copyTextCM;
			this.profileSelection.FormattingEnabled = true;
			this.profileSelection.Location = new System.Drawing.Point (143, 15);
			this.profileSelection.Size = new System.Drawing.Size (508, 24);
			this.profileSelection.TabIndex = 0;
			this.profileSelection.SelectedIndexChanged += (object sender, System.EventArgs e) => {
				this.NotIdle ();
				this.RefreshControls (RefreshLevel.Service);
			};
			this.serviceSelection.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.serviceSelection.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.serviceSelection.ContextMenu = null;
			this.serviceSelection.ContextMenuStrip = this.serviceCM;
			this.serviceSelection.FormattingEnabled = true;
			this.serviceSelection.Location = new System.Drawing.Point (143, 96);
			this.serviceSelection.Size = new System.Drawing.Size (508, 24);
			this.serviceSelection.TabIndex = 1;
			this.serviceSelection.SelectedIndexChanged += (object sender, System.EventArgs e) => {
				this.NotIdle ();
				this.RefreshControls (RefreshLevel.Account);
			};
			this.profileLabel.Location = new System.Drawing.Point (12, 18);
			this.profileLabel.Size = new System.Drawing.Size (52, 17);
			this.serviceLabel.Location = new System.Drawing.Point (12, 99);
			this.serviceLabel.Size = new System.Drawing.Size (59, 17);
			this.addProfile.Location = new System.Drawing.Point (179, 47);
			this.addProfile.Size = new System.Drawing.Size (152, 41);
			this.addProfile.TabStop = false;
			this.addProfile.Click += (object sender, System.EventArgs e) => {
				this.AddProfile ();
			};
			this.removeProfile.Location = new System.Drawing.Point (339, 47);
			this.removeProfile.Size = new System.Drawing.Size (152, 41);
			this.removeProfile.TabStop = false;
			this.removeProfile.Click += (object sender, System.EventArgs e) => {
				this.RemoveProfile ();
			};
			this.renameProfile.Location = new System.Drawing.Point (499, 47);
			this.renameProfile.Size = new System.Drawing.Size (152, 41);
			this.renameProfile.TabStop = false;
			this.renameProfile.Click += (object sender, System.EventArgs e) => {
				this.RenameProfile ();
			};
			this.addService.Location = new System.Drawing.Point (179, 128);
			this.addService.Size = new System.Drawing.Size (152, 39);
			this.addService.TabStop = false;
			this.addService.Click += (object sender, System.EventArgs e) => {
				this.AddService ();
			};
			this.removeService.Location = new System.Drawing.Point (339, 128);
			this.removeService.Size = new System.Drawing.Size (152, 39);
			this.removeService.TabStop = false;
			this.removeService.Click += (object sender, System.EventArgs e) => {
				this.RemoveService ();
			};
			this.renameService.Location = new System.Drawing.Point (499, 128);
			this.renameService.Size = new System.Drawing.Size (152, 39);
			this.renameService.TabStop = false;
			this.renameService.Click += (object sender, System.EventArgs e) => {
				this.RenameService ();
			};
			this.accountSelection.FormattingEnabled = true;
			this.accountSelection.ContextMenu = null;
			this.accountSelection.ContextMenuStrip = this.copyTextCM;
			this.accountSelection.Location = new System.Drawing.Point (143, 175);
			this.accountSelection.Size = new System.Drawing.Size (508, 24);
			this.accountSelection.TabIndex = 2;
			this.accountSelection.SelectedIndexChanged += (object sender, System.EventArgs e) => {
				this.NotIdle ();
				this.accountPassword.Text = Program.Profiles [this.profileSelection.SelectedIndex].Profileservices [this.serviceSelection.SelectedIndex].ServiceAccounts [this.accountSelection.SelectedIndex].Password;
			};
			this.accountLabel.Location = new System.Drawing.Point (12, 178);
			this.accountLabel.Size = new System.Drawing.Size (63, 17);
			this.addAccount.Location = new System.Drawing.Point (180, 207);
			this.addAccount.Size = new System.Drawing.Size (152, 39);
			this.addAccount.TabStop = false;
			this.addAccount.Click += (object sender, System.EventArgs e) => {
				this.AddAccount ();
			};
			this.removeAccount.Location = new System.Drawing.Point (340, 207);
			this.removeAccount.Size = new System.Drawing.Size (152, 39);
			this.removeAccount.TabStop = false;
			this.removeAccount.Click += (object sender, System.EventArgs e) => {
				this.RemoveAccount ();
			};
			this.renameAccount.Location = new System.Drawing.Point (498, 207);
			this.renameAccount.Size = new System.Drawing.Size (153, 39);
			this.renameAccount.TabStop = false;
			this.renameAccount.Click += (object sender, System.EventArgs e) => {
				this.RenameAccount ();
			};
			this.accountPassword.ContextMenu = null;
			this.accountPassword.ContextMenuStrip = this.copyTextCM;
			this.accountPassword.Location = new System.Drawing.Point (143, 254);
			this.accountPassword.ReadOnly = true;
			this.accountPassword.Size = new System.Drawing.Size (508, 22);
			this.accountPassword.TabIndex = 3;
			this.accountPassword.UseSystemPasswordChar = true;
			this.copyTextCM.Size = new System.Drawing.Size (199, 28);
			this.copyTextCM.Items.AddRange (new System.Windows.Forms.ToolStripItem[] {
				this.copyTextCMMI
			});
			this.copyTextCMMI.Size = new System.Drawing.Size (198, 24);
			this.copyTextCMMI.Click += (object sender, System.EventArgs e) => {
				this.NotIdle ();
				this.Copy2Clipboard (((System.Windows.Forms.Control)((System.Windows.Forms.ContextMenuStrip)((System.Windows.Forms.ToolStripMenuItem)sender).Owner).SourceControl).Text);
			};
			this.serviceCM.Size = new System.Drawing.Size (199, 28);
			this.serviceCM.Items.AddRange (new System.Windows.Forms.ToolStripItem[] {
				this.serviceCMexecuteCommand,
				this.serviceCMsetCommand,
				this.serviceCMclone
			});
			this.serviceCMexecuteCommand.Size = new System.Drawing.Size (198, 24);
			this.serviceCMexecuteCommand.Click += (object sender, System.EventArgs e) => {
				this.ExecuteCommand ();
			};
			this.serviceCMsetCommand.Size = new System.Drawing.Size (198, 24);
			this.serviceCMsetCommand.Click += (object sender, System.EventArgs e) => {
				this.SetCommand ();
			};
			this.serviceCMclone.Size = new System.Drawing.Size (198, 24);
			this.serviceCMclone.Click += (object sender, System.EventArgs e) => {
				this.CloneService ();
			};
			this.setPassword.Location = new System.Drawing.Point (180, 284);
			this.setPassword.Size = new System.Drawing.Size (152, 39);
			this.setPassword.TabStop = false;
			this.setPassword.Click += (object sender, System.EventArgs e) => {
				this.SetPassword ();
			};
			this.generatePassword.Location = new System.Drawing.Point (340, 284);
			this.generatePassword.Size = new System.Drawing.Size (152, 39);
			this.generatePassword.TabStop = false;
			this.generatePassword.Click += (object sender, System.EventArgs e) => {
				this.GeneratePassword ();
			};
			this.passwordLabel.Location = new System.Drawing.Point (12, 257);
			this.passwordLabel.Size = new System.Drawing.Size (73, 17);
			this.passwordStrength.Location = new System.Drawing.Point (498, 284);
			this.passwordStrength.Maximum = new Password ().PasswordLength.GetLength (0) - 1;
			this.passwordStrength.Size = new System.Drawing.Size (153, 56);
			this.passwordStrength.TabStop = false;
			this.passwordStrength.Value = 2;
			this.passwordStrength.ValueChanged += (object sender, System.EventArgs e) => {
				this.NotIdle ();
				this.passwordStrengthDescription.Text = new Password ().PasswordLength [this.passwordStrength.Value].ToString ();
				this.passwordStrengthDescription.Text += " aA0";
				if (this.passwordStrength.Value > 1) {
					this.passwordStrengthDescription.Text += "!";
				}

				if (this.passwordStrength.Value > 5) {
					this.passwordStrengthDescription.Text += "#";
				}
			};
			this.readOnly.Checked = true;
			this.readOnly.CheckState = System.Windows.Forms.CheckState.Checked;
			this.readOnly.Location = new System.Drawing.Point (15, 324);
			this.readOnly.Size = new System.Drawing.Size (97, 21);
			this.readOnly.TabStop = false;
			this.readOnly.CheckedChanged += (object sender, System.EventArgs e) => {
				this.NotIdle ();
				this.RefreshControls (RefreshLevel.None);
			};
			this.encrypt.Checked = true;
			this.encrypt.CheckState = System.Windows.Forms.CheckState.Checked;
			this.encrypt.Location = new System.Drawing.Point (15, 353);
			this.encrypt.Size = new System.Drawing.Size (112, 21);
			this.encrypt.TabStop = false;
			this.encrypt.CheckedChanged += (object sender, System.EventArgs e) => {
				this.NotIdle ();
				this.saveOnClose = true;
			};
			this.idleTimer.Enabled = true;
			this.idleTimer.Interval = 1000;
			this.idleTimer.Tick += (object sender, System.EventArgs e) => {
				this.ProcessTick ();
			};
			this.showPassword.Location = new System.Drawing.Point (15, 294);
			this.showPassword.Size = new System.Drawing.Size (129, 21);
			this.showPassword.TabStop = false;
			this.showPassword.CheckedChanged += (object sender, System.EventArgs e) => {
				this.NotIdle ();
				this.accountPassword.UseSystemPasswordChar = !this.showPassword.Checked;
			};
			this.passwordStrengthLabel.Location = new System.Drawing.Point (496, 330);
			this.passwordStrengthLabel.Size = new System.Drawing.Size (127, 17);
			this.tray.ContextMenu = null;
			this.tray.ContextMenuStrip = null;
			this.tray.Visible = false;
			this.mainFormCM.Items.AddRange (new System.Windows.Forms.ToolStripItem[] {
				this.mainFormCMdisableTimeout,
				this.mainFormCMsetLanguage,
				this.mainFormCMimport,
				this.mainFormCMmergeProfiles
			});
			this.mainFormCM.Size = new System.Drawing.Size (214, 76);
			this.mainFormCMdisableTimeout.Size = new System.Drawing.Size (213, 24);
			this.mainFormCMdisableTimeout.Click += (object sender, System.EventArgs e) => {
				this.NotIdle ();
				this.notimeout = !this.notimeout;
				((System.Windows.Forms.ToolStripMenuItem)this.mainFormCM.Items [0]).Checked = this.notimeout;
			};
			this.mainFormCMsetLanguage.Size = new System.Drawing.Size (213, 24);
			this.mainFormCMimport.Size = new System.Drawing.Size (213, 24);
			this.mainFormCMimport.Click += (object sender, System.EventArgs e) => {
				this.Import ();
			};
			this.mainFormCMmergeProfiles.Size = new System.Drawing.Size (213, 24);
			this.mainFormCMmergeProfiles.Click += (object sender, System.EventArgs e) => {
				this.MergeProfiles ();
			};
			this.setMasterPassword.Location = new System.Drawing.Point (180, 330);
			this.setMasterPassword.Size = new System.Drawing.Size (312, 39);
			this.setMasterPassword.TabStop = false;
			this.setMasterPassword.Click += (object sender, System.EventArgs e) => {
				this.SetMasterPassword ();
			};
			this.passwordStrengthDescription.Location = new System.Drawing.Point (585, 354);
			this.passwordStrengthDescription.Size = new System.Drawing.Size (66, 17);
			this.passwordStrengthDescription.Text = new Password ().PasswordLength [this.passwordStrength.Value].ToString () + " aA0!";
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.ClientSize = new System.Drawing.Size (this.passwordStrengthDescription.Right + 20, this.passwordStrengthDescription.Bottom + 10);
			this.ContextMenu = null;
			this.ContextMenuStrip = this.mainFormCM;
			this.Controls.Add (this.passwordStrengthDescription);
			this.Controls.Add (this.setMasterPassword);
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
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Load += new System.EventHandler (this.MainForm_Load);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler (this.MainForm_Closing);
			this.copyTextCM.ResumeLayout (false);
			this.serviceCM.ResumeLayout (false);
			this.mainFormCM.ResumeLayout (false);
			this.ResumeLayout (false);
			this.PerformLayout ();
		}
	}
}