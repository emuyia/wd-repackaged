namespace WDLaunch
{
    partial class WDLaunch_Form
    {

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WDLaunch_Form));
			this.D3D8WrapperCheckBox = new System.Windows.Forms.CheckBox();
			this.VersionLabel = new System.Windows.Forms.Label();
			this.AdminModeLabel = new System.Windows.Forms.Label();
			this.hints = new System.Windows.Forms.ToolTip(this.components);
			this.ExitButton = new System.Windows.Forms.PictureBox();
			this.LaunchButton = new System.Windows.Forms.PictureBox();
			this.OpenMainDirButton = new System.Windows.Forms.Label();
			this.OpenSavesDirButton = new System.Windows.Forms.Label();
			this.OpenCaptureDirButton = new System.Windows.Forms.Label();
			this.AutoLaunchCheckBox = new System.Windows.Forms.CheckBox();
			this.LangRadioButton_KR = new System.Windows.Forms.RadioButton();
			this.LangRadioButton_EN = new System.Windows.Forms.RadioButton();
			this.AAComboBox = new System.Windows.Forms.ComboBox();
			this.OhJaemiLaunchButton = new System.Windows.Forms.PictureBox();
			this.TexFiltComboBox = new System.Windows.Forms.ComboBox();
			this.AAComboBoxLabel = new System.Windows.Forms.Label();
			this.TexFiltComboBoxLabel = new System.Windows.Forms.Label();
			this.MoreSettingsButton = new System.Windows.Forms.Button();
			this.FixLocaleCheckBox = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.ExitButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.LaunchButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.OhJaemiLaunchButton)).BeginInit();
			this.SuspendLayout();
			// 
			// D3D8WrapperCheckBox
			// 
			this.D3D8WrapperCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.D3D8WrapperCheckBox.AutoCheck = false;
			this.D3D8WrapperCheckBox.AutoSize = true;
			this.D3D8WrapperCheckBox.BackColor = System.Drawing.Color.Transparent;
			this.D3D8WrapperCheckBox.Checked = true;
			this.D3D8WrapperCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.D3D8WrapperCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
			this.D3D8WrapperCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.D3D8WrapperCheckBox.ForeColor = System.Drawing.Color.White;
			this.D3D8WrapperCheckBox.Location = new System.Drawing.Point(83, 116);
			this.D3D8WrapperCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.D3D8WrapperCheckBox.Name = "D3D8WrapperCheckBox";
			this.D3D8WrapperCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.D3D8WrapperCheckBox.Size = new System.Drawing.Size(114, 20);
			this.D3D8WrapperCheckBox.TabIndex = 3;
			this.D3D8WrapperCheckBox.Text = "Wrap Direct3D";
			this.D3D8WrapperCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.D3D8WrapperCheckBox.UseVisualStyleBackColor = false;
			this.D3D8WrapperCheckBox.Click += new System.EventHandler(this.D3D8WrapperCheckBox_Clicked);
			// 
			// VersionLabel
			// 
			this.VersionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.VersionLabel.BackColor = System.Drawing.Color.Transparent;
			this.VersionLabel.Cursor = System.Windows.Forms.Cursors.Hand;
			this.VersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.VersionLabel.ForeColor = System.Drawing.Color.White;
			this.VersionLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.VersionLabel.Location = new System.Drawing.Point(236, 173);
			this.VersionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.VersionLabel.Name = "VersionLabel";
			this.VersionLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.VersionLabel.Size = new System.Drawing.Size(76, 20);
			this.VersionLabel.TabIndex = 7;
			this.VersionLabel.Text = "1.00";
			this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.VersionLabel.Click += new System.EventHandler(this.VersionLabel_Click);
			// 
			// AdminModeLabel
			// 
			this.AdminModeLabel.AutoSize = true;
			this.AdminModeLabel.BackColor = System.Drawing.Color.Transparent;
			this.AdminModeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AdminModeLabel.ForeColor = System.Drawing.Color.Red;
			this.AdminModeLabel.Location = new System.Drawing.Point(260, 12);
			this.AdminModeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.AdminModeLabel.Name = "AdminModeLabel";
			this.AdminModeLabel.Size = new System.Drawing.Size(15, 16);
			this.AdminModeLabel.TabIndex = 9;
			this.AdminModeLabel.Text = "✱";
			this.AdminModeLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.AdminModeLabel.Visible = false;
			this.AdminModeLabel.Click += new System.EventHandler(this.AdminModeLabel_Click);
			// 
			// hints
			// 
			this.hints.AutoPopDelay = 20000;
			this.hints.InitialDelay = 500;
			this.hints.ReshowDelay = 100;
			this.hints.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
			// 
			// ExitButton
			// 
			this.ExitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ExitButton.BackColor = System.Drawing.Color.Transparent;
			this.ExitButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.ExitButton.Image = ((System.Drawing.Image)(resources.GetObject("ExitButton.Image")));
			this.ExitButton.InitialImage = null;
			this.ExitButton.Location = new System.Drawing.Point(283, 12);
			this.ExitButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.ExitButton.Name = "ExitButton";
			this.ExitButton.Size = new System.Drawing.Size(29, 28);
			this.ExitButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.ExitButton.TabIndex = 20;
			this.ExitButton.TabStop = false;
			this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
			// 
			// LaunchButton
			// 
			this.LaunchButton.BackColor = System.Drawing.Color.Transparent;
			this.LaunchButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.LaunchButton.Image = ((System.Drawing.Image)(resources.GetObject("LaunchButton.Image")));
			this.LaunchButton.Location = new System.Drawing.Point(-31, 11);
			this.LaunchButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.LaunchButton.Name = "LaunchButton";
			this.LaunchButton.Size = new System.Drawing.Size(292, 39);
			this.LaunchButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.LaunchButton.TabIndex = 21;
			this.LaunchButton.TabStop = false;
			this.LaunchButton.Click += new System.EventHandler(this.LaunchButton_Click);
			// 
			// OpenMainDirButton
			// 
			this.OpenMainDirButton.AutoSize = true;
			this.OpenMainDirButton.BackColor = System.Drawing.Color.Transparent;
			this.OpenMainDirButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.OpenMainDirButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.OpenMainDirButton.ForeColor = System.Drawing.Color.White;
			this.OpenMainDirButton.Image = ((System.Drawing.Image)(resources.GetObject("OpenMainDirButton.Image")));
			this.OpenMainDirButton.Location = new System.Drawing.Point(9, 84);
			this.OpenMainDirButton.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.OpenMainDirButton.Name = "OpenMainDirButton";
			this.OpenMainDirButton.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.OpenMainDirButton.Size = new System.Drawing.Size(75, 26);
			this.OpenMainDirButton.TabIndex = 22;
			this.OpenMainDirButton.Text = "\\whiteday";
			this.OpenMainDirButton.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.OpenMainDirButton.Click += new System.EventHandler(this.OpenMainDirButton_Click);
			// 
			// OpenSavesDirButton
			// 
			this.OpenSavesDirButton.AutoSize = true;
			this.OpenSavesDirButton.BackColor = System.Drawing.Color.Transparent;
			this.OpenSavesDirButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.OpenSavesDirButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.OpenSavesDirButton.ForeColor = System.Drawing.Color.White;
			this.OpenSavesDirButton.Image = ((System.Drawing.Image)(resources.GetObject("OpenSavesDirButton.Image")));
			this.OpenSavesDirButton.Location = new System.Drawing.Point(9, 114);
			this.OpenSavesDirButton.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.OpenSavesDirButton.Name = "OpenSavesDirButton";
			this.OpenSavesDirButton.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.OpenSavesDirButton.Size = new System.Drawing.Size(57, 26);
			this.OpenSavesDirButton.TabIndex = 23;
			this.OpenSavesDirButton.Text = "\\saves";
			this.OpenSavesDirButton.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.OpenSavesDirButton.Click += new System.EventHandler(this.OpenSavesDirButton_Click);
			// 
			// OpenCaptureDirButton
			// 
			this.OpenCaptureDirButton.AutoSize = true;
			this.OpenCaptureDirButton.BackColor = System.Drawing.Color.Transparent;
			this.OpenCaptureDirButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.OpenCaptureDirButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.OpenCaptureDirButton.ForeColor = System.Drawing.Color.White;
			this.OpenCaptureDirButton.Image = ((System.Drawing.Image)(resources.GetObject("OpenCaptureDirButton.Image")));
			this.OpenCaptureDirButton.Location = new System.Drawing.Point(9, 144);
			this.OpenCaptureDirButton.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.OpenCaptureDirButton.Name = "OpenCaptureDirButton";
			this.OpenCaptureDirButton.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.OpenCaptureDirButton.Size = new System.Drawing.Size(67, 26);
			this.OpenCaptureDirButton.TabIndex = 24;
			this.OpenCaptureDirButton.Text = "\\capture";
			this.OpenCaptureDirButton.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.OpenCaptureDirButton.Click += new System.EventHandler(this.OpenCaptureDirButton_Click);
			// 
			// AutoLaunchCheckBox
			// 
			this.AutoLaunchCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.AutoLaunchCheckBox.AutoCheck = false;
			this.AutoLaunchCheckBox.AutoSize = true;
			this.AutoLaunchCheckBox.BackColor = System.Drawing.Color.Transparent;
			this.AutoLaunchCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
			this.AutoLaunchCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AutoLaunchCheckBox.ForeColor = System.Drawing.Color.White;
			this.AutoLaunchCheckBox.Location = new System.Drawing.Point(90, 89);
			this.AutoLaunchCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.AutoLaunchCheckBox.Name = "AutoLaunchCheckBox";
			this.AutoLaunchCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.AutoLaunchCheckBox.Size = new System.Drawing.Size(107, 20);
			this.AutoLaunchCheckBox.TabIndex = 13;
			this.AutoLaunchCheckBox.Text = "Direct Launch";
			this.AutoLaunchCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.AutoLaunchCheckBox.UseVisualStyleBackColor = false;
			this.AutoLaunchCheckBox.Click += new System.EventHandler(this.AutoLaunchCheckBox_Clicked);
			// 
			// LangRadioButton_KR
			// 
			this.LangRadioButton_KR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.LangRadioButton_KR.AutoCheck = false;
			this.LangRadioButton_KR.AutoSize = true;
			this.LangRadioButton_KR.BackColor = System.Drawing.Color.Transparent;
			this.LangRadioButton_KR.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.LangRadioButton_KR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LangRadioButton_KR.Location = new System.Drawing.Point(254, 58);
			this.LangRadioButton_KR.Name = "LangRadioButton_KR";
			this.LangRadioButton_KR.Size = new System.Drawing.Size(58, 17);
			this.LangRadioButton_KR.TabIndex = 26;
			this.LangRadioButton_KR.TabStop = true;
			this.LangRadioButton_KR.Text = "한국어";
			this.LangRadioButton_KR.UseVisualStyleBackColor = false;
			this.LangRadioButton_KR.Click += new System.EventHandler(this.LangRadioButton_KR_Click);
			// 
			// LangRadioButton_EN
			// 
			this.LangRadioButton_EN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.LangRadioButton_EN.AutoCheck = false;
			this.LangRadioButton_EN.AutoSize = true;
			this.LangRadioButton_EN.BackColor = System.Drawing.Color.Transparent;
			this.LangRadioButton_EN.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.LangRadioButton_EN.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.LangRadioButton_EN.Location = new System.Drawing.Point(191, 58);
			this.LangRadioButton_EN.Name = "LangRadioButton_EN";
			this.LangRadioButton_EN.Size = new System.Drawing.Size(59, 17);
			this.LangRadioButton_EN.TabIndex = 27;
			this.LangRadioButton_EN.TabStop = true;
			this.LangRadioButton_EN.Text = "English";
			this.LangRadioButton_EN.UseVisualStyleBackColor = false;
			this.LangRadioButton_EN.Click += new System.EventHandler(this.LangRadioButton_EN_Click);
			// 
			// AAComboBox
			// 
			this.AAComboBox.BackColor = System.Drawing.Color.LemonChiffon;
			this.AAComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.AAComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AAComboBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.AAComboBox.FormattingEnabled = true;
			this.AAComboBox.Items.AddRange(new object[] {
            "Native",
            "2x MSAA",
            "4x MSAA",
            "8x MSAA"});
			this.AAComboBox.Location = new System.Drawing.Point(208, 103);
			this.AAComboBox.Name = "AAComboBox";
			this.AAComboBox.Size = new System.Drawing.Size(104, 21);
			this.AAComboBox.TabIndex = 4;
			this.AAComboBox.SelectionChangeCommitted += new System.EventHandler(this.AAComboBox_SelectionChangeCommitted);
			// 
			// OhJaemiLaunchButton
			// 
			this.OhJaemiLaunchButton.BackColor = System.Drawing.Color.Transparent;
			this.OhJaemiLaunchButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.OhJaemiLaunchButton.Image = ((System.Drawing.Image)(resources.GetObject("OhJaemiLaunchButton.Image")));
			this.OhJaemiLaunchButton.Location = new System.Drawing.Point(-13, 45);
			this.OhJaemiLaunchButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.OhJaemiLaunchButton.Name = "OhJaemiLaunchButton";
			this.OhJaemiLaunchButton.Size = new System.Drawing.Size(156, 24);
			this.OhJaemiLaunchButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.OhJaemiLaunchButton.TabIndex = 28;
			this.OhJaemiLaunchButton.TabStop = false;
			this.OhJaemiLaunchButton.Click += new System.EventHandler(this.OhJaemiLaunchButton_Click);
			this.OhJaemiLaunchButton.MouseEnter += new System.EventHandler(this.Button_MouseEnter);
			this.OhJaemiLaunchButton.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
			// 
			// TexFiltComboBox
			// 
			this.TexFiltComboBox.BackColor = System.Drawing.Color.LemonChiffon;
			this.TexFiltComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.TexFiltComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TexFiltComboBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.TexFiltComboBox.FormattingEnabled = true;
			this.TexFiltComboBox.Items.AddRange(new object[] {
            "Native",
            "Bilinear",
            "Trilinear",
            "Anisotropic 2x",
            "Anisotropic 4x",
            "Anisotropic 8x",
            "Anisotropic 16x"});
			this.TexFiltComboBox.Location = new System.Drawing.Point(208, 147);
			this.TexFiltComboBox.Name = "TexFiltComboBox";
			this.TexFiltComboBox.Size = new System.Drawing.Size(104, 21);
			this.TexFiltComboBox.TabIndex = 29;
			this.TexFiltComboBox.SelectionChangeCommitted += new System.EventHandler(this.TexFiltComboBox_SelectionChangeCommitted);
			// 
			// AAComboBoxLabel
			// 
			this.AAComboBoxLabel.AutoSize = true;
			this.AAComboBoxLabel.BackColor = System.Drawing.Color.Transparent;
			this.AAComboBoxLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.AAComboBoxLabel.Location = new System.Drawing.Point(206, 87);
			this.AAComboBoxLabel.Name = "AAComboBoxLabel";
			this.AAComboBoxLabel.Size = new System.Drawing.Size(58, 12);
			this.AAComboBoxLabel.TabIndex = 30;
			this.AAComboBoxLabel.Text = "Anti-Aliasing";
			// 
			// TexFiltComboBoxLabel
			// 
			this.TexFiltComboBoxLabel.AutoSize = true;
			this.TexFiltComboBoxLabel.BackColor = System.Drawing.Color.Transparent;
			this.TexFiltComboBoxLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TexFiltComboBoxLabel.Location = new System.Drawing.Point(206, 132);
			this.TexFiltComboBoxLabel.Name = "TexFiltComboBoxLabel";
			this.TexFiltComboBoxLabel.Size = new System.Drawing.Size(71, 12);
			this.TexFiltComboBoxLabel.TabIndex = 31;
			this.TexFiltComboBoxLabel.Text = "Texture Filtering";
			// 
			// MoreSettingsButton
			// 
			this.MoreSettingsButton.BackColor = System.Drawing.Color.Transparent;
			this.MoreSettingsButton.BackgroundImage = global::WDLaunch.Properties.Resources.transpbg;
			this.MoreSettingsButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.MoreSettingsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MoreSettingsButton.Location = new System.Drawing.Point(140, 178);
			this.MoreSettingsButton.Margin = new System.Windows.Forms.Padding(0);
			this.MoreSettingsButton.Name = "MoreSettingsButton";
			this.MoreSettingsButton.Size = new System.Drawing.Size(39, 15);
			this.MoreSettingsButton.TabIndex = 32;
			this.MoreSettingsButton.Text = "▼";
			this.MoreSettingsButton.UseVisualStyleBackColor = false;
			// 
			// FixLocaleCheckBox
			// 
			this.FixLocaleCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.FixLocaleCheckBox.AutoCheck = false;
			this.FixLocaleCheckBox.AutoSize = true;
			this.FixLocaleCheckBox.BackColor = System.Drawing.Color.Transparent;
			this.FixLocaleCheckBox.Checked = true;
			this.FixLocaleCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.FixLocaleCheckBox.Cursor = System.Windows.Forms.Cursors.Hand;
			this.FixLocaleCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FixLocaleCheckBox.ForeColor = System.Drawing.Color.White;
			this.FixLocaleCheckBox.Location = new System.Drawing.Point(110, 143);
			this.FixLocaleCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.FixLocaleCheckBox.Name = "FixLocaleCheckBox";
			this.FixLocaleCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.FixLocaleCheckBox.Size = new System.Drawing.Size(87, 20);
			this.FixLocaleCheckBox.TabIndex = 33;
			this.FixLocaleCheckBox.Text = "Fix Locale";
			this.FixLocaleCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.FixLocaleCheckBox.UseVisualStyleBackColor = false;
			this.FixLocaleCheckBox.Click += new System.EventHandler(this.FixLocaleCheckBox_Click);
			// 
			// WDLaunch_Form
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ClientSize = new System.Drawing.Size(321, 197);
			this.ControlBox = false;
			this.Controls.Add(this.FixLocaleCheckBox);
			this.Controls.Add(this.MoreSettingsButton);
			this.Controls.Add(this.TexFiltComboBoxLabel);
			this.Controls.Add(this.AAComboBoxLabel);
			this.Controls.Add(this.TexFiltComboBox);
			this.Controls.Add(this.LangRadioButton_EN);
			this.Controls.Add(this.OhJaemiLaunchButton);
			this.Controls.Add(this.OpenCaptureDirButton);
			this.Controls.Add(this.D3D8WrapperCheckBox);
			this.Controls.Add(this.OpenMainDirButton);
			this.Controls.Add(this.OpenSavesDirButton);
			this.Controls.Add(this.AutoLaunchCheckBox);
			this.Controls.Add(this.AAComboBox);
			this.Controls.Add(this.VersionLabel);
			this.Controls.Add(this.LangRadioButton_KR);
			this.Controls.Add(this.AdminModeLabel);
			this.Controls.Add(this.ExitButton);
			this.Controls.Add(this.LaunchButton);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "WDLaunch_Form";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "wdlaunch";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.WDLaunch_Load);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WDLaunch_Form_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.WDLaunch_Form_MouseMove);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.WDLaunch_Form_MouseUp);
			((System.ComponentModel.ISupportInitialize)(this.ExitButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.LaunchButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.OhJaemiLaunchButton)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox D3D8WrapperCheckBox;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Label AdminModeLabel;
        private System.Windows.Forms.ToolTip hints;
        private System.Windows.Forms.PictureBox ExitButton;
        private System.Windows.Forms.PictureBox LaunchButton;
        private System.Windows.Forms.Label OpenMainDirButton;
        private System.Windows.Forms.Label OpenSavesDirButton;
        private System.Windows.Forms.Label OpenCaptureDirButton;
        private System.Windows.Forms.CheckBox AutoLaunchCheckBox;
        private System.Windows.Forms.RadioButton LangRadioButton_KR;
        private System.Windows.Forms.RadioButton LangRadioButton_EN;
        private System.Windows.Forms.ComboBox AAComboBox;
		private System.Windows.Forms.PictureBox OhJaemiLaunchButton;
		private System.Windows.Forms.ComboBox TexFiltComboBox;
		private System.Windows.Forms.Label AAComboBoxLabel;
		private System.Windows.Forms.Label TexFiltComboBoxLabel;
		private System.Windows.Forms.Button MoreSettingsButton;
		private System.Windows.Forms.CheckBox FixLocaleCheckBox;
	}
}

