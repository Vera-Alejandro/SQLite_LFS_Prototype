using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Interstates.Control.Database.Controls
{
    public partial class ProfileSetup : Form
    {
        protected DatabaseProfile _dbProfile;
        private string _strDefaultProfile;
        private DBProfileTemp _dbTemp;

        public ProfileSetup(DatabaseProfile profile)
        {
            InitializeComponent();

            //database settings
            _dbProfile = profile;
            _strDefaultProfile = DatabaseConfiguration.DatabaseSettings.DefaultProfile;

            try
            {
                _dbTemp = new DBProfileTemp(profile);
                propProfile.SelectedObject = _dbTemp;
                if (_dbTemp.Name == _strDefaultProfile)
                    _dbTemp.DefaultProfile = true;
                else
                    _dbTemp.DefaultProfile = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
        }

        private bool ApplyChanges()
        {
            bool blnSave = true;

            try
            {
                if (propProfile.SelectedObject != null)
                {
                    DBProfileTemp dbTemp = (DBProfileTemp)propProfile.SelectedObject;

                    if (dbTemp.Name == string.Empty)
                        throw new DatabaseException("Invalid profile name.");

                    _dbProfile.Name = dbTemp.Name;
                    _dbProfile.ConnectionString = dbTemp.ConnectionType.ConnectionStringBuilder.ConnectionString;
                    _dbProfile.Timeout = dbTemp.Timeout;
                    _dbProfile.EnablePing = dbTemp.EnablePing;
                    _dbProfile.PingTimeout = dbTemp.PingTimeout;
                    _dbProfile.PingTTL = dbTemp.PingTTL;
                    _dbProfile.ProviderType = dbTemp.ProviderType;
                    if (dbTemp.DefaultProfile == true)
                        _strDefaultProfile = dbTemp.Name;
                }
            }
            catch(Exception ex )
            {
                MessageBox.Show(ex.Message, "Apply Changes");
                blnSave = false;
            }

            return blnSave;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                ApplyChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }

        }

        private void propProfile_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            try
            {
                if (e.ChangedItem != null)
                { 
                }
                //mconnectionType.ConnectionString = mconnectionType.ConnectionType.ConnectionStringBuilder.ConnectionString;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Unable to update the provider type. " + ex.Message;
            }
        }
    }
    
    /// <summary>
    /// Used to edit only the properties of the profile that are important to the user
    /// </summary>
    class DBProfileTemp
    {
        private string _name;
        private int _timeout;
        private bool _defaultProfile;
        private string _providerType;
        private ConnectionType _connectionType;
        private bool _enablePing;
        private int _pingTimeout;
        private int _pingTTL;

        public DBProfileTemp(DatabaseProfile dbProfile)
        {
            this.Name = dbProfile.Name;
            this.Timeout = dbProfile.Timeout;
            this.ProviderType = dbProfile.ProviderType;
            this._connectionType = new ConnectionType(ProviderType, dbProfile.ConnectionString);
            this.EnablePing = dbProfile.EnablePing;
            this.PingTimeout = dbProfile.PingTimeout;
            this._pingTTL = dbProfile.PingTTL;
        }

        [Browsable(true)]
        [Description("The name of the database profile.")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [Browsable(true)]
        [Editor(typeof(ConnectionStringUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("The connection string defines how the database connection will be established.")]
        [DisplayName("Connection String")]
        public ConnectionType ConnectionType
        {
            get { return _connectionType; }
            set { _connectionType = value; }
        }

        [Browsable(true)]
        [Description("The number of seconds to wait for a connection to be established.")]
        [DisplayName("Connection Timeout")]
        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        [Browsable(true)]
        [Description("The data provider for the profile.")]
        [DisplayName("Database Provider")]
        [TypeConverter(typeof(ProviderConverter))]
        public string ProviderType
        {
            get { return _providerType; }
            set 
            {
                _providerType = value;
                try
                {
                    _connectionType = new ConnectionType(value, string.Empty);
                }
                catch { }
            }
        }

        [Browsable(true)]
        [Description("The default profile that the application will use if no specific profile is needed.")]
        public bool DefaultProfile
        {
            get { return _defaultProfile; }
            set { _defaultProfile = value; }
        }

        [Browsable(true)]
        [Description("Returns true if the datasource is a computer that can be pinged before opening the connection.")]
        [DisplayName("Enable Ping")]
        public bool EnablePing
        {
            get { return _enablePing; }
            set { _enablePing = value; }
        }

        [Browsable(true)]
        [Description("The number of seconds to wait for a ping reply.")]
        [DisplayName("Ping Timeout")]
        public int PingTimeout
        {
            get { return _pingTimeout; }
            set { _pingTimeout = value; }
        }

        [Browsable(true)]
        [Description("The Time-to-Live value is used to test the number of routers and gateways a packet must pass through. As the packet is passed though a network the value is decremented. When the value is 0 the packet is discarded.")]
        [DisplayName("Ping TTL")]
        public int PingTTL
        {
            get { return _pingTTL; }
            set { _pingTTL = value; }
        }
    }
}