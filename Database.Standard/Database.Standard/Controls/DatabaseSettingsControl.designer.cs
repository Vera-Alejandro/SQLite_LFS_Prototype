namespace Interstates.Control.Database.Controls
{
    partial class DatabaseSettingsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelDatabaseEnabled = new System.Windows.Forms.Panel();
            this.chkEncrypt = new System.Windows.Forms.CheckBox();
            this.cboProfiles = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.panelDatabase = new System.Windows.Forms.Panel();
            this.cmdTestConnection = new System.Windows.Forms.Button();
            this.cmdCopy = new System.Windows.Forms.Button();
            this.cmdDelete = new System.Windows.Forms.Button();
            this.cmdNew = new System.Windows.Forms.Button();
            this.cmdEdit = new System.Windows.Forms.Button();
            this.dgdSettings = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panelDatabaseEnabled.SuspendLayout();
            this.panelDatabase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgdSettings)).BeginInit();
            this.SuspendLayout();
            // 
            // panelDatabaseEnabled
            // 
            this.panelDatabaseEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDatabaseEnabled.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelDatabaseEnabled.Controls.Add(this.chkEncrypt);
            this.panelDatabaseEnabled.Controls.Add(this.cboProfiles);
            this.panelDatabaseEnabled.Controls.Add(this.label5);
            this.panelDatabaseEnabled.Controls.Add(this.panelDatabase);
            this.panelDatabaseEnabled.Controls.Add(this.label3);
            this.panelDatabaseEnabled.Controls.Add(this.label2);
            this.panelDatabaseEnabled.Location = new System.Drawing.Point(0, 0);
            this.panelDatabaseEnabled.Name = "panelDatabaseEnabled";
            this.panelDatabaseEnabled.Size = new System.Drawing.Size(612, 294);
            this.panelDatabaseEnabled.TabIndex = 81;
            // 
            // chkEncrypt
            // 
            this.chkEncrypt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkEncrypt.AutoSize = true;
            this.chkEncrypt.Location = new System.Drawing.Point(451, 48);
            this.chkEncrypt.Name = "chkEncrypt";
            this.chkEncrypt.Size = new System.Drawing.Size(152, 17);
            this.chkEncrypt.TabIndex = 83;
            this.chkEncrypt.Text = "Encrypt Database Settings";
            this.chkEncrypt.UseVisualStyleBackColor = true;
            // 
            // cboProfiles
            // 
            this.cboProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProfiles.FormattingEnabled = true;
            this.cboProfiles.Location = new System.Drawing.Point(17, 44);
            this.cboProfiles.Name = "cboProfiles";
            this.cboProfiles.Size = new System.Drawing.Size(216, 21);
            this.cboProfiles.TabIndex = 58;
            this.cboProfiles.SelectedIndexChanged += new System.EventHandler(this.cboProfiles_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 13);
            this.label5.TabIndex = 57;
            this.label5.Text = "Default Profile";
            // 
            // panelDatabase
            // 
            this.panelDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDatabase.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelDatabase.Controls.Add(this.cmdTestConnection);
            this.panelDatabase.Controls.Add(this.cmdCopy);
            this.panelDatabase.Controls.Add(this.cmdDelete);
            this.panelDatabase.Controls.Add(this.cmdNew);
            this.panelDatabase.Controls.Add(this.cmdEdit);
            this.panelDatabase.Controls.Add(this.dgdSettings);
            this.panelDatabase.Location = new System.Drawing.Point(3, 70);
            this.panelDatabase.Name = "panelDatabase";
            this.panelDatabase.Size = new System.Drawing.Size(600, 221);
            this.panelDatabase.TabIndex = 54;
            // 
            // cmdTestConnection
            // 
            this.cmdTestConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdTestConnection.Location = new System.Drawing.Point(339, 195);
            this.cmdTestConnection.Name = "cmdTestConnection";
            this.cmdTestConnection.Size = new System.Drawing.Size(125, 23);
            this.cmdTestConnection.TabIndex = 32;
            this.cmdTestConnection.Text = "&Test Connection";
            this.cmdTestConnection.UseVisualStyleBackColor = true;
            this.cmdTestConnection.Click += new System.EventHandler(this.cmdTestConnection_Click);
            // 
            // cmdCopy
            // 
            this.cmdCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdCopy.Location = new System.Drawing.Point(15, 195);
            this.cmdCopy.Name = "cmdCopy";
            this.cmdCopy.Size = new System.Drawing.Size(75, 23);
            this.cmdCopy.TabIndex = 31;
            this.cmdCopy.Text = "&Copy";
            this.cmdCopy.UseVisualStyleBackColor = true;
            this.cmdCopy.Click += new System.EventHandler(this.cmdCopy_Click);
            // 
            // cmdDelete
            // 
            this.cmdDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdDelete.Location = new System.Drawing.Point(96, 195);
            this.cmdDelete.Name = "cmdDelete";
            this.cmdDelete.Size = new System.Drawing.Size(75, 23);
            this.cmdDelete.TabIndex = 30;
            this.cmdDelete.Text = "&Delete";
            this.cmdDelete.UseVisualStyleBackColor = true;
            this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click);
            // 
            // cmdNew
            // 
            this.cmdNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdNew.Location = new System.Drawing.Point(177, 195);
            this.cmdNew.Name = "cmdNew";
            this.cmdNew.Size = new System.Drawing.Size(75, 23);
            this.cmdNew.TabIndex = 28;
            this.cmdNew.Text = "&New";
            this.cmdNew.UseVisualStyleBackColor = true;
            this.cmdNew.Click += new System.EventHandler(this.cmdNew_Click);
            // 
            // cmdEdit
            // 
            this.cmdEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdEdit.Location = new System.Drawing.Point(258, 195);
            this.cmdEdit.Name = "cmdEdit";
            this.cmdEdit.Size = new System.Drawing.Size(75, 23);
            this.cmdEdit.TabIndex = 29;
            this.cmdEdit.Text = "&Edit";
            this.cmdEdit.UseVisualStyleBackColor = true;
            this.cmdEdit.Click += new System.EventHandler(this.cmdEdit_Click);
            // 
            // dgdSettings
            // 
            this.dgdSettings.AllowUserToAddRows = false;
            this.dgdSettings.AllowUserToDeleteRows = false;
            this.dgdSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgdSettings.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgdSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgdSettings.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgdSettings.Location = new System.Drawing.Point(15, 3);
            this.dgdSettings.Name = "dgdSettings";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgdSettings.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgdSettings.Size = new System.Drawing.Size(582, 186);
            this.dgdSettings.TabIndex = 27;
            this.dgdSettings.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgdSettings_RowValidating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 52;
            this.label3.Text = "Database Settings";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(109, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(427, 2);
            this.label2.TabIndex = 3;
            // 
            // DatabaseSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.Controls.Add(this.panelDatabaseEnabled);
            this.Name = "DatabaseSettingsControl";
            this.Size = new System.Drawing.Size(616, 299);
            this.Load += new System.EventHandler(this.DatabaseSettingsControl_Load);
            this.Validating += new System.ComponentModel.CancelEventHandler(this.DatabaseSettingsControl_Validating);
            this.panelDatabaseEnabled.ResumeLayout(false);
            this.panelDatabaseEnabled.PerformLayout();
            this.panelDatabase.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgdSettings)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelDatabaseEnabled;
        private System.Windows.Forms.Panel panelDatabase;
        private System.Windows.Forms.DataGridView dgdSettings;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cmdDelete;
        private System.Windows.Forms.Button cmdNew;
        private System.Windows.Forms.Button cmdEdit;
        private System.Windows.Forms.ComboBox cboProfiles;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button cmdCopy;
        private System.Windows.Forms.CheckBox chkEncrypt;
        private System.Windows.Forms.Button cmdTestConnection;
    }
}
