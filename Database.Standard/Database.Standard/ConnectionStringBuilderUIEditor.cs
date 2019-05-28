using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using System.Data.SqlClient;

namespace Interstates.Control.Database
{
    public partial class ConnectionStringBuilderUIEditor : Form
    {
        private ConnectionType _connectionType;
        private delegate void UpdateStatusUI(string message);

        public ConnectionStringBuilderUIEditor()
        {
            InitializeComponent();
        }

        private void ConnectionStringBuilderDialog_Load(object sender, EventArgs e)
        {
            
        }

        public ConnectionType ConnectionType
        {
            get 
            {
                return _connectionType;
            }
            set 
            {
                _connectionType = value;
                propertyGrid1.SelectedObject = _connectionType.ConnectionStringBuilder;
            }
        }

        private void cmdTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateStatus("Connecting...");

                backgroundConnect.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                lblStatus.Text = ex.Message;
            }
        }

        private void backgroundConnect_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                UpdateStatus(e.Error.Message);
            else
                UpdateStatus("Successfully connected to " + ((DbConnectionStringBuilder)propertyGrid1.SelectedObject).ConnectionString);
        }

        private void backgroundConnect_DoWork(object sender, DoWorkEventArgs e)
        {
            using (IDbConnection connection = _connectionType.GetConnection())
            {
                connection.ConnectionString = _connectionType.ConnectionStringBuilder.ConnectionString;
                connection.Open();
            }
        }

        private void UpdateStatus(string message)
        {
            if (lblStatus.InvokeRequired == true)
            {
                Invoke(new UpdateStatusUI(UpdateStatus), message);
            }
            else
            {
                lblStatus.Text = message;
            }
        }
    }
}