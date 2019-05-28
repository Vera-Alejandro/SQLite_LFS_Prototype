using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using Interstates.Control.Database.Plugin;
using Interstates.Control.Database.Forms;
using System.Text;
using System.Net.NetworkInformation;
using System.Threading;

namespace Interstates.Control.Database.Controls
{
    public partial class DatabaseSettingsControl : UserControl
    {
        private DatabaseSettings _dbSettings;
        private DataTable _dtProfile;
        
        public DatabaseSettingsControl()
        {
            DataSet dsProfiles = new DataSet();
            _dtProfile = dsProfiles.Tables.Add("Profile");
            _dtProfile.Columns.Add("Name");
            _dtProfile.Columns.Add("Provider");
            _dtProfile.Columns.Add("ConnectionString");
            _dtProfile.Columns.Add("Timeout");

            InitializeComponent();
        }

        public DatabaseSettingsControl(DatabaseSettings settings) : this()
        {
            _dbSettings = settings;
            ShowSettings();
        }

        public bool EncryptSettings
        {
            get
            {
                return chkEncrypt.Checked;
            }
            set
            {
                chkEncrypt.Checked = value;
            }
        }

        public DatabaseSettings DatabaseSetting
        {
            get
            {
                return _dbSettings;
            }
            set
            {
                _dbSettings = value;
                ShowSettings();
            }
        }

        private void ShowSettings()
        {
            if (_dbSettings == null)
                return;

            ShowProfiles();
        }

        private void ShowDefaultProfile()
        {
            int intLastProfile;

            intLastProfile = cboProfiles.SelectedIndex;
            cboProfiles.Items.Clear();
            _dtProfile.Clear();

            foreach (DatabaseProfile dbProfile in _dbSettings.Profiles)
            {
                _dtProfile.Rows.Add(new object[] { dbProfile.Name, dbProfile.ProviderType.ToString(), dbProfile.ConnectionString, dbProfile.Timeout });
                cboProfiles.Items.Add(dbProfile.Name);
                if (intLastProfile < 0)
                    if (dbProfile.Name == _dbSettings.DefaultProfile)
                        intLastProfile = cboProfiles.Items.Count - 1;
            }
            if (cboProfiles.Items.Count > 0)
            {
                if (intLastProfile >= 0 && intLastProfile < cboProfiles.Items.Count)
                    cboProfiles.SelectedIndex = intLastProfile;
                else
                    cboProfiles.SelectedIndex = 0;
            }
        }

        protected void ShowProfiles()
        {
            int intPos;

            dgdSettings.Columns.Clear();
            intPos = dgdSettings.Columns.Add( "Name", "Name" );
            dgdSettings.Columns[intPos].DataPropertyName = "Name";
            DataGridViewComboBoxCell cellProvider = new DataGridViewComboBoxCell();
            DataGridViewColumn colProvider = new DataGridViewColumn( cellProvider );
            cellProvider.MaxDropDownItems = 4;
            Array arrItems = DatabaseConfiguration.ProviderTypes.ToArray();
            foreach (string item in arrItems)
            {
                cellProvider.Items.Add(item);
            }
            colProvider.HeaderText = "Provider";
            colProvider.DataPropertyName = "Provider";
            colProvider.Name = "Provider";
            dgdSettings.Columns.Add(colProvider );
            intPos = dgdSettings.Columns.Add("ConnectionString", "ConnectionString");
            dgdSettings.Columns[intPos].DataPropertyName = "ConnectionString";
            intPos = dgdSettings.Columns["ConnectionString"].Width = dgdSettings.Width - 250;
            intPos = dgdSettings.Columns.Add("Timeout", "Timeout");
            dgdSettings.Columns[intPos].DataPropertyName = "Timeout";
            dgdSettings.AutoGenerateColumns = false;
            dgdSettings.DataSource = _dtProfile;

            ShowDefaultProfile();
        }

        private void cmdNew_Click(object sender, EventArgs e)
        {
            try
            {
                DatabaseProfile dbProfile = new DatabaseProfile(string.Format("New"));

            RenameProfile:
                NewProfile frmAddProfile = new NewProfile(dbProfile.Name);

                if (DialogResult.OK == frmAddProfile.ShowDialog())
                {
                    dbProfile.Name = frmAddProfile.ProfileName;
                    if (_dbSettings.Profiles[dbProfile.Name] != null)
                    {
                        MessageBox.Show("The profile you entered already exists. Choose a different name for the profile.");
                        goto RenameProfile;
                    }
                    ProfileSetup newProfile = new ProfileSetup(dbProfile);
                    if (newProfile.ShowDialog() == DialogResult.OK)
                    {
                        _dbSettings.Profiles.Add(dbProfile);
                        ShowProfiles();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Add profile");
            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            if (dgdSettings.CurrentRow != null)
            {
                DialogResult removeDefault = DialogResult.Yes;

                //confirm if they are trying to remove the default profile
                if ((string)dgdSettings.CurrentRow.Cells["Name"].Value == _dbSettings.DefaultProfile)
                    removeDefault = MessageBox.Show("Are you sure you want to remove the Default profile?", "Remove Default", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (removeDefault == DialogResult.Yes)
                {
                    try
                    {
                        if (dgdSettings.CurrentRow.Index >= 0)
                        {
                            _dbSettings.Profiles.Remove((string)dgdSettings.CurrentRow.Cells["Name"].Value);
                            dgdSettings.Rows.Remove(dgdSettings.CurrentRow);
                            ShowDefaultProfile();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Remove profile");
                    }
                }
            }
        }

        private void cmdEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgdSettings.CurrentRow != null)
                {
                    DatabaseProfile dbProfile = _dbSettings.Profiles[dgdSettings.CurrentRow.Index];
                    ProfileSetup frmEdit = new ProfileSetup(dbProfile);

                    if (DialogResult.OK == frmEdit.ShowDialog())
                    {
                        ShowProfiles();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Edit profile");
            }

        }

        private void dgdSettings_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Update the database profile
            try
            {
                if (dgdSettings.CurrentRow != null && dgdSettings.IsCurrentRowDirty == true)
                {
                    DatabaseProfile dbProfile;

                    if (dgdSettings.CurrentRow.Index >= _dbSettings.Profiles.Count)
                    {
                        dbProfile = new DatabaseProfile();
                    }
                    else
                    {
                        dbProfile = _dbSettings.Profiles[dgdSettings.CurrentRow.Index];
                    }

                    // Find any duplicates in the list
                    int intDup = 0;
                    foreach (DatabaseProfile profile in _dbSettings.Profiles)
                    {
                        if (profile.Name == ((string)dgdSettings.CurrentRow.Cells["Name"].Value).Trim())
                            intDup++;
                    }
                    if (intDup > 1)
                    {
                        e.Cancel = true;
                        MessageBox.Show("The profile name must be unique. ", "Validating profile");
                        return;
                    }

                    dbProfile.Name = ((string)dgdSettings.CurrentRow.Cells["Name"].Value).Trim();
                    dbProfile.ProviderType = (string)dgdSettings.CurrentRow.Cells["Provider"].Value;
                    try
                    {
                        dbProfile.ConnectionString = (string)dgdSettings.CurrentRow.Cells["ConnectionString"].Value;
                    }
                    catch (Exception ex)
                    {
                        e.Cancel = true;
                        MessageBox.Show("The ConnectionString is not valid. " + ex.Message, "Validating profile");
                        return;
                    }

                    dbProfile.Timeout = Convert.ToInt16(dgdSettings.CurrentRow.Cells["Timeout"].Value);

                    // Update the combobox 
                    string strSelected = cboProfiles.Text;
                    if( cboProfiles.SelectedIndex == dgdSettings.CurrentRow.Index )
                    {
                        strSelected = dbProfile.Name;
                        cboProfiles.Items.RemoveAt(dgdSettings.CurrentRow.Index);
                        cboProfiles.Items.Insert(dgdSettings.CurrentRow.Index, dbProfile.Name);
                    }
                    cboProfiles.Text = strSelected;

                    // Add the profile
                    if (dgdSettings.CurrentRow.Index >= _dbSettings.Profiles.Count)
                    {
                        _dbSettings.Profiles.Add(dbProfile);
                        ShowDefaultProfile();
                    }
                }
            }
            catch (Exception ex)
            {
                e.Cancel = true;
                MessageBox.Show(ex.Message, "Validating profile");
            }

        }

        private void cmdCopy_Click(object sender, EventArgs e)
        {
            if (dgdSettings.CurrentRow == null)
                return;

            try
            {
                DatabaseProfile dbProfile = _dbSettings.Profiles[dgdSettings.CurrentRow.Index];
                DatabaseProfile dbProfileCopy = new DatabaseProfile("Copy of " + dbProfile.Name);

                dbProfileCopy.ProviderType = dbProfile.ProviderType;
                dbProfileCopy.Timeout = dbProfile.Timeout;
                dbProfileCopy.ConnectionString = dbProfile.ConnectionString;

                ProfileSetup frmEdit = new ProfileSetup(dbProfileCopy);
                if (DialogResult.OK == frmEdit.ShowDialog())
                {
                    _dbSettings.Profiles.Add(dbProfileCopy);
                    ShowProfiles();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Copy profile");
            }
        }

        private void cboProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if( cboProfiles.SelectedIndex >= 0)
            {
                _dbSettings.DefaultProfile = cboProfiles.Text;
            }
        }

        private void DatabaseSettingsControl_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                e.Cancel = !ValidateSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Returns true if the settings are valid.
        /// </summary>
        /// <returns></returns>
        private bool ValidateSettings()
        {
            if (_dbSettings.DefaultProfile.Length == 0)
                throw new DatabaseException("The default profile cannot be blank");
            return true; // Validated
        }

        private void DatabaseSettingsControl_Load(object sender, EventArgs e)
        {
            dgdSettings.DataError += new DataGridViewDataErrorEventHandler(dgdSettings_DataError);
        }

        void dgdSettings_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Handled
        }
        
        private void cmdTestConnection_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(a =>
            {
                StringBuilder messages = new StringBuilder();
                try
                {
                    DatabaseProfile dbProfile = _dbSettings.Profiles[dgdSettings.CurrentRow.Index];
                    Query dbQuery = new Query(dbProfile);
                    if (dbProfile.EnablePing)
                    {
                        messages.AppendLine("Ping results");
                        PingReply reply = dbQuery.Ping();
                        messages.AppendLine(reply.Status.ToString());
                        messages.AppendLine();
                    }

                    messages.AppendLine("Connection results");
                    using (var con = dbQuery.CreateConnection())
                    {
                        con.Open();
                        messages.AppendLine(String.Format("Connected to datasource {0}", dbProfile.DataSource));
                    }

                    MessageBox.Show(messages.ToString(), "Test connection succeeded");

                }
                catch (Exception ex)
                {
                    messages.AppendLine(ex.Message);
                    MessageBox.Show(messages.ToString(), "Test connection failed");
                }
            });
        }
    }
}
