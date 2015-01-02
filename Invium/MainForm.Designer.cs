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
	using System;
	using System.Drawing;
	using System.Windows.Forms;

	public partial class MainForm
	{
		private ComboBox profileSelection;
		private ComboBox serviceSelection;
		private Label profileLabel;
		private Label serviceLabel;
		private Button addProfile;
		private Button removeProfile;
		private Button renameProfile;
		private Button addService;
		private Button removeService;
		private Button renameService;
		private ComboBox accountSelection;
		private Label accountLabel;
		private Button addAccount;
		private Button removeAccount;
		private Button renameAccount;
		private TextBox accountPassword;
		private Button setPassword;
		private Button generatePassword;
		private Label passwordLabel;
		private TrackBar passwordStrength;
		private CheckBox readOnly;
		private CheckBox encrypt;
		private ContextMenuStrip copyTextCM;
		private ToolStripMenuItem copyTextCMMI;
		private ContextMenuStrip serviceCM;
		private ToolStripMenuItem serviceCMexecuteCommand;
		private ToolStripMenuItem serviceCMsetCommand;
		private ToolStripMenuItem serviceCMclone;
		private CheckBox showPassword;
		private Label passwordStrengthLabel;
		private NotifyIcon tray;
		private ContextMenuStrip mainFormCM;
		private ToolStripMenuItem mainFormCMdisableTimeout;
		private ToolStripMenuItem mainFormCMsetLanguage;
		private ToolStripMenuItem mainFormCMimport;
		private ToolStripMenuItem mainFormCMmergeProfiles;
		private Button setMasterPassword;
		private Label passwordStrengthDescription;

		private void InitializeComponent ()
		{
			this.profileSelection = new ComboBox ();
			this.serviceSelection = new ComboBox ();
			this.profileLabel = new Label ();
			this.serviceLabel = new Label ();
			this.addProfile = new Button ();
			this.removeProfile = new Button ();
			this.renameProfile = new Button ();
			this.addService = new Button ();
			this.removeService = new Button ();
			this.renameService = new Button ();
			this.accountSelection = new ComboBox ();
			this.accountLabel = new Label ();
			this.addAccount = new Button ();
			this.removeAccount = new Button ();
			this.renameAccount = new Button ();
			this.accountPassword = new TextBox ();
			this.copyTextCM = new ContextMenuStrip ();
			this.copyTextCMMI = new ToolStripMenuItem ();
			this.serviceCM = new ContextMenuStrip ();
			this.serviceCMexecuteCommand = new ToolStripMenuItem ();
			this.serviceCMsetCommand = new ToolStripMenuItem ();
			this.serviceCMclone = new ToolStripMenuItem ();
			this.setPassword = new Button ();
			this.generatePassword = new Button ();
			this.passwordLabel = new Label ();
			this.passwordStrength = new TrackBar ();
			this.readOnly = new CheckBox ();
			this.encrypt = new CheckBox ();
			this.showPassword = new CheckBox ();
			this.passwordStrengthLabel = new Label ();
			this.tray = new NotifyIcon ();
			this.mainFormCM = new ContextMenuStrip ();
			this.mainFormCMdisableTimeout = new ToolStripMenuItem ();
			this.mainFormCMsetLanguage = new ToolStripMenuItem ();
			this.mainFormCMimport = new ToolStripMenuItem ();
			this.mainFormCMmergeProfiles = new ToolStripMenuItem ();
			this.setMasterPassword = new Button ();
			this.passwordStrengthDescription = new Label ();
			this.copyTextCM.SuspendLayout ();
			this.serviceCM.SuspendLayout ();
			this.mainFormCM.SuspendLayout ();
			this.SuspendLayout ();
			this.profileSelection.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
			this.profileSelection.AutoCompleteSource = AutoCompleteSource.ListItems;
			this.profileSelection.ContextMenu = null;
			this.profileSelection.ContextMenuStrip = this.copyTextCM;
			this.profileSelection.FormattingEnabled = true;
			this.profileSelection.Location = new Point (143, 15);
			this.profileSelection.Size = new Size (508, 24);
			this.profileSelection.TabIndex = 0;
			this.profileSelection.SelectedIndexChanged += (object sender, EventArgs e) => {
				this.NotIdle ();
				this.RefreshControls (RefreshLevel.Service);
			};
			this.serviceSelection.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
			this.serviceSelection.AutoCompleteSource = AutoCompleteSource.ListItems;
			this.serviceSelection.ContextMenu = null;
			this.serviceSelection.ContextMenuStrip = this.serviceCM;
			this.serviceSelection.FormattingEnabled = true;
			this.serviceSelection.Location = new Point (143, 96);
			this.serviceSelection.Size = new Size (508, 24);
			this.serviceSelection.TabIndex = 1;
			this.serviceSelection.SelectedIndexChanged += (object sender, EventArgs e) => {
				this.NotIdle ();
				this.RefreshControls (RefreshLevel.Account);
			};
			this.profileLabel.Location = new Point (12, 18);
			this.profileLabel.Size = new Size (52, 17);
			this.serviceLabel.Location = new Point (12, 99);
			this.serviceLabel.Size = new Size (59, 17);
			this.addProfile.Location = new Point (179, 47);
			this.addProfile.Size = new Size (152, 41);
			this.addProfile.TabStop = false;
			this.addProfile.Click += (object sender, EventArgs e) => {
				this.AddProfile ();
			};
			this.removeProfile.Location = new Point (339, 47);
			this.removeProfile.Size = new Size (152, 41);
			this.removeProfile.TabStop = false;
			this.removeProfile.Click += (object sender, EventArgs e) => {
				this.RemoveProfile ();
			};
			this.renameProfile.Location = new Point (499, 47);
			this.renameProfile.Size = new Size (152, 41);
			this.renameProfile.TabStop = false;
			this.renameProfile.Click += (object sender, EventArgs e) => {
				this.RenameProfile ();
			};
			this.addService.Location = new Point (179, 128);
			this.addService.Size = new Size (152, 39);
			this.addService.TabStop = false;
			this.addService.Click += (object sender, EventArgs e) => {
				this.AddService ();
			};
			this.removeService.Location = new Point (339, 128);
			this.removeService.Size = new Size (152, 39);
			this.removeService.TabStop = false;
			this.removeService.Click += (object sender, EventArgs e) => {
				this.RemoveService ();
			};
			this.renameService.Location = new Point (499, 128);
			this.renameService.Size = new Size (152, 39);
			this.renameService.TabStop = false;
			this.renameService.Click += (object sender, EventArgs e) => {
				this.RenameService ();
			};
			this.accountSelection.FormattingEnabled = true;
			this.accountSelection.ContextMenu = null;
			this.accountSelection.ContextMenuStrip = this.copyTextCM;
			this.accountSelection.Location = new Point (143, 175);
			this.accountSelection.Size = new Size (508, 24);
			this.accountSelection.TabIndex = 2;
			this.accountSelection.SelectedIndexChanged += (object sender, EventArgs e) => {
				this.NotIdle ();
				this.accountPassword.Text = Program.Profiles [this.profileSelection.SelectedIndex].Services [this.serviceSelection.SelectedIndex].Accounts [this.accountSelection.SelectedIndex].Password;
			};
			this.accountLabel.Location = new Point (12, 178);
			this.accountLabel.Size = new Size (63, 17);
			this.addAccount.Location = new Point (180, 207);
			this.addAccount.Size = new Size (152, 39);
			this.addAccount.TabStop = false;
			this.addAccount.Click += (object sender, EventArgs e) => {
				this.AddAccount ();
			};
			this.removeAccount.Location = new Point (340, 207);
			this.removeAccount.Size = new Size (152, 39);
			this.removeAccount.TabStop = false;
			this.removeAccount.Click += (object sender, EventArgs e) => {
				this.RemoveAccount ();
			};
			this.renameAccount.Location = new Point (498, 207);
			this.renameAccount.Size = new Size (153, 39);
			this.renameAccount.TabStop = false;
			this.renameAccount.Click += (object sender, EventArgs e) => {
				this.RenameAccount ();
			};
			this.accountPassword.ContextMenu = null;
			this.accountPassword.ContextMenuStrip = this.copyTextCM;
			this.accountPassword.Location = new Point (143, 254);
			this.accountPassword.ReadOnly = true;
			this.accountPassword.Size = new Size (508, 22);
			this.accountPassword.TabIndex = 3;
			this.accountPassword.UseSystemPasswordChar = true;
			this.copyTextCM.Size = new Size (199, 28);
			this.copyTextCM.Items.AddRange (new ToolStripItem[] {
				this.copyTextCMMI
			});
			this.copyTextCMMI.Size = new Size (198, 24);
			this.copyTextCMMI.Click += (object sender, EventArgs e) => {
				this.NotIdle ();
				this.Copy2Clipboard (((Control)((ContextMenuStrip)((ToolStripMenuItem)sender).Owner).SourceControl).Text);
			};
			this.serviceCM.Size = new Size (199, 28);
			this.serviceCM.Items.AddRange (new ToolStripItem[] {
				this.serviceCMexecuteCommand,
				this.serviceCMsetCommand,
				this.serviceCMclone
			});
			this.serviceCMexecuteCommand.Size = new Size (198, 24);
			this.serviceCMexecuteCommand.Click += (object sender, EventArgs e) => {
				this.ExecuteCommand ();
			};
			this.serviceCMsetCommand.Size = new Size (198, 24);
			this.serviceCMsetCommand.Click += (object sender, EventArgs e) => {
				this.SetCommand ();
			};
			this.serviceCMclone.Size = new Size (198, 24);
			this.serviceCMclone.Click += (object sender, EventArgs e) => {
				this.CloneService ();
			};
			this.setPassword.Location = new Point (180, 284);
			this.setPassword.Size = new Size (152, 39);
			this.setPassword.TabStop = false;
			this.setPassword.Click += (object sender, EventArgs e) => {
				this.SetPassword ();
			};
			this.generatePassword.Location = new Point (340, 284);
			this.generatePassword.Size = new Size (152, 39);
			this.generatePassword.TabStop = false;
			this.generatePassword.Click += (object sender, EventArgs e) => {
				this.GeneratePassword ();
			};
			this.passwordLabel.Location = new Point (12, 257);
			this.passwordLabel.Size = new Size (73, 17);
			this.passwordStrength.Location = new Point (498, 284);
			this.passwordStrength.Maximum = new Password ().PasswordLength.GetLength (0) - 1;
			this.passwordStrength.Size = new Size (153, 56);
			this.passwordStrength.TabStop = false;
			this.passwordStrength.Value = 2;
			this.passwordStrength.ValueChanged += (object sender, EventArgs e) => {
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
			this.readOnly.CheckState = CheckState.Checked;
			this.readOnly.Location = new Point (15, 324);
			this.readOnly.Size = new Size (97, 21);
			this.readOnly.TabStop = false;
			this.readOnly.CheckedChanged += (object sender, EventArgs e) => {
				this.NotIdle ();
				this.RefreshControls (RefreshLevel.None);
			};
			this.encrypt.Checked = true;
			this.encrypt.CheckState = CheckState.Checked;
			this.encrypt.Location = new Point (15, 353);
			this.encrypt.Size = new Size (112, 21);
			this.encrypt.TabStop = false;
			this.encrypt.CheckedChanged += (object sender, EventArgs e) => {
				this.NotIdle ();
				this.saveOnClose = true;
			};
			this.showPassword.Location = new Point (15, 294);
			this.showPassword.Size = new Size (129, 21);
			this.showPassword.TabStop = false;
			this.showPassword.CheckedChanged += (object sender, EventArgs e) => {
				this.NotIdle ();
				this.accountPassword.UseSystemPasswordChar = !this.showPassword.Checked;
			};
			this.passwordStrengthLabel.Location = new Point (496, 330);
			this.passwordStrengthLabel.Size = new Size (127, 17);
			this.tray.ContextMenu = null;
			this.tray.ContextMenuStrip = null;
			this.tray.Visible = false;
			this.mainFormCM.Items.AddRange (new ToolStripItem[] {
				this.mainFormCMdisableTimeout,
				this.mainFormCMsetLanguage,
				this.mainFormCMimport,
				this.mainFormCMmergeProfiles
			});
			this.mainFormCM.Size = new Size (214, 76);
			this.mainFormCMdisableTimeout.Size = new Size (213, 24);
			this.mainFormCMdisableTimeout.Click += (object sender, EventArgs e) => {
				this.NotIdle ();
				this.notimeout = !this.notimeout;
				((ToolStripMenuItem)this.mainFormCM.Items [0]).Checked = this.notimeout;
			};
			this.mainFormCMsetLanguage.Size = new Size (213, 24);
			this.mainFormCMimport.Size = new Size (213, 24);
			this.mainFormCMimport.Click += (object sender, EventArgs e) => {
				this.Import ();
			};
			this.mainFormCMmergeProfiles.Size = new Size (213, 24);
			this.mainFormCMmergeProfiles.Click += (object sender, EventArgs e) => {
				this.MergeProfiles ();
			};
			this.setMasterPassword.Location = new Point (180, 330);
			this.setMasterPassword.Size = new Size (312, 39);
			this.setMasterPassword.TabStop = false;
			this.setMasterPassword.Click += (object sender, EventArgs e) => {
				this.SetMasterPassword ();
			};
			this.passwordStrengthDescription.Location = new Point (585, 354);
			this.passwordStrengthDescription.Size = new Size (66, 17);
			this.passwordStrengthDescription.Text = new Password ().PasswordLength [this.passwordStrength.Value].ToString () + " aA0!";
			this.AutoScaleMode = AutoScaleMode.None;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			this.ClientSize = new Size (this.passwordStrengthDescription.Right + 20, this.passwordStrengthDescription.Bottom + 10);
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
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Load += new EventHandler (this.MainForm_Load);
			this.FormClosing += new FormClosingEventHandler (this.MainForm_Closing);
			this.copyTextCM.ResumeLayout (false);
			this.serviceCM.ResumeLayout (false);
			this.mainFormCM.ResumeLayout (false);
			this.ResumeLayout (false);
			this.PerformLayout ();
		}
	}
}