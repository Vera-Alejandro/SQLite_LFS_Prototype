using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;
using Interstates.Control.Framework.Log;
using Interstates.Control.Database.Plugin;
using Interstates.Control.Database.COM;

namespace Interstates.Control.Database
{
    /// <summary>
    ///     This class handles database profile configuration settings 
    /// </summary>
    [ClassInterface(ClassInterfaceType.None),
     ComVisible(true)]
    public sealed class DatabaseProfile : ConfigurationElement, IDatabaseProfile
    {
        private const string CONNECTIONSTRING_DEFAULT = "provider=MSDASQL.1;server=.; Integrated Security=SSPI; database=";
        private const int COMMANDTIMEOUT_DEFAULT = 30;
        private const string PROVIDERTYPE_DEFAULT = "SQL";
        private const int PINGTIMEOUT_DEFAULT = 3;
        private const int PINGTTL_DEFAULT = 128;
        private DbConnectionStringBuilder _connString = new EventRaisingDbConnectionStringBuilder();

        public DatabaseProfile()
        {
            // See comments in mconnString_ConnectionStringChanged
            ((EventRaisingDbConnectionStringBuilder)_connString).ConnectionStringChanged += new EventHandler(mconnString_ConnectionStringChanged);
        }

        /// <summary>
        /// Create a new database profile using the name specified.
        /// </summary>
        /// <param name="name"></param>
        public DatabaseProfile(string name)
            : this()
        {
            Name = name;
        }
        
        /// <summary>
        /// Create a new database profile that can connect to the connection string.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connectionString"></param>
        /// <param name="providerType"></param>
        public DatabaseProfile(string name, string connectionString, string providerType)
            : this()
        {
            Name = name;
            ConnectionString = connectionString;
            ProviderType = providerType;
        }

        private void mconnString_ConnectionStringChanged(object sender, EventArgs e)
        {
            // The 'ConfigurationElement' base of this class will only recognize changes when they are written to
            // it via the 'this[propertyName]' construct. So, we need to write the ConnectionString property to
            // the base this way. The problem is when the connection string is modified via the ConnectionParams
            // property, so to handle that, I've overridden the DbConnectionStringBuilder class to raise an event
            // when the connection string changes, and this method catches that change and stores the connection
            // string to the ConfigurationElement base.
            this["ConnectionString"] = _connString.ConnectionString;
        }

        // read the connection string that was stored in the app.config and init. the commandstringbuilder
        protected override void PostDeserialize()
        {
            if ((string)this["ConnectionString"] != null)
            {
                _connString.ConnectionString = (string)this["ConnectionString"];
            }
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Gets the connection string builder used to manage the contents of the connection string.
        /// </summary>
        [Browsable(false)]
        [ComVisible(false)]
        public DbConnectionStringBuilder ConnectionStringBuilder
        {
            get
            {
                return _connString;
            }
        }

        /// <summary>
        /// Gets or sets the option to ping a datasource before opening the connection.
        /// </summary>
        [Browsable(false)]
        [ConfigurationProperty("EnablePing",
                    DefaultValue = false,
                    IsRequired = false)]
        public bool EnablePing
        {
            get
            {
                bool blnPing = false;

                if (this["EnablePing"] != null)
                    blnPing = Convert.ToBoolean(this["EnablePing"]);
                return blnPing;
            }
            set
            {
                this["EnablePing"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the time to wait for a ping reply.
        /// </summary>
        [ConfigurationProperty("PingTimeout",
           DefaultValue = (int)PINGTIMEOUT_DEFAULT,
            IsRequired = false)]
        [IntegerValidator(MinValue = 0,
            MaxValue = Int32.MaxValue, ExcludeRange = false)]
        public int PingTimeout
        {
            get
            {
                return (int)this["PingTimeout"];
            }
            set
            {
                this["PingTimeout"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Time-To-Live value for the ping command.
        /// The Time-to-Live value is used to test the number of routers and gateways a packet must pass through.
        /// As the packet is passed though a network the value is decremented. When the value is 0 the packet is discarded.
        /// </summary>
        [ConfigurationProperty("PingTTL",
           DefaultValue = (int)PINGTTL_DEFAULT,
            IsRequired = false)]
        [IntegerValidator(MinValue = 0,
            MaxValue = Int32.MaxValue, ExcludeRange = false)]
        public int PingTTL
        {
            get
            {
                return (int)this["PingTTL"];
            }
            set
            {
                this["PingTTL"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the database profile.
        /// </summary>
        [Browsable(true)]
        [ConfigurationProperty("Name",
                    DefaultValue = "",
                    IsRequired = true,
                    IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this["Name"];
            }
            set
            {
                this["Name"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the connection string used to connect to the database.
        /// </summary>
        [ConfigurationProperty("ConnectionString",
           DefaultValue = CONNECTIONSTRING_DEFAULT,
            IsRequired = true)]
        public string ConnectionString
        {
            get
            {
                return _connString.ConnectionString;
            }
            set
            {
                _connString.ConnectionString = value;
                this["ConnectionString"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the provider type for the database profile. The provider type defines which plugin provider will be used for the database profile.
        /// </summary>
        [ConfigurationProperty("ProviderType",
           DefaultValue = PROVIDERTYPE_DEFAULT,
            IsRequired = true)]
        public string ProviderType
        {
            get
            {
                return (string)this["ProviderType"];
            }
            set
            {
                this["ProviderType"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the command timeout for a database connection.
        /// </summary>
        [ConfigurationProperty("Timeout",
           DefaultValue = (int)COMMANDTIMEOUT_DEFAULT,
            IsRequired = false)]
        [IntegerValidator(MinValue = 0,
            MaxValue = Int32.MaxValue, ExcludeRange = false)]
        public int Timeout
        {
            get
            {
                return (int)this["Timeout"];
            }
            set
            {
                this["Timeout"] = value;
            }
        }

        /// <summary>
        /// Gets the name of the Provider specified in the "Provider=" clause of the connection string.
        /// </summary>
        public string Provider
        {
            get
            {
                object objValue = null;
                _connString.TryGetValue("Provider", out objValue);
                return (string)objValue;
            }
        }

        /// <summary>
        /// Gets the name of the data source specified in the "Data Source=", "Server=", or "Address=" clause of the connection string.
        /// </summary>
        public string DataSource
        {
            get
            {
                object objValue = null;
                if (_connString.TryGetValue("Data Source", out objValue) == false)
                {                    
                    if (_connString.TryGetValue("Server", out objValue) == false)
                    {
                        if (_connString.TryGetValue("Address", out objValue) == false)
                        {                            
                        }
                    }
                }
                return (string)objValue;
            }
        }

        /// <summary>
        /// Gets the name of the database specified in the "Initial Catalog=", "Database=" clause of the connection string.
        /// </summary>
        public string Database
        {
            get
            {
                object objValue = null;
                if (_connString.TryGetValue("Initial Catalog", out objValue) == false)
                {
                    if (_connString.TryGetValue("Database", out objValue) == false)
                    {                            
                    }
                }
                return (string)objValue;
            }
        }

        /// <summary>
        /// Gets the name of the user specified in the "User Id=", "Uid=" clause of the connection string.
        /// </summary>
        public string UserId
        {
            get
            {
                object objValue = null;
                if (_connString.TryGetValue("User Id", out objValue) == false)
                {
                    if (_connString.TryGetValue("Uid", out objValue) == false)
                    {                            
                    }
                }
                return (string)objValue;
            }
        }

        /// <summary>
        /// Gets the name of the password specified in the "Password=", "Pwd=" clause of the connection string.
        /// </summary>
        public string Password
        {
            get
            {
                object objValue = null;
                if (_connString.TryGetValue("Password", out objValue) == false)
                {
                    if (_connString.TryGetValue("Pwd", out objValue) == false)
                    {                            
                    }
                }
                return (string)objValue;
            }
        }

        /// <summary>
        /// Creates a copy of the database profile into a new database profile.
        /// </summary>
        /// <returns></returns>
        public DatabaseProfile Copy()
        {
            DatabaseProfile copySetting = new DatabaseProfile();

            foreach (ConfigurationProperty property in this.Properties)
            {
                copySetting[property.Name] = this[property.Name];
            }
            copySetting.ConnectionString = this.ConnectionString;
            return copySetting;
        }
    }

    /// <summary>
    /// A 'DbConnectionStringBuilder' class that raises an event whenever anything
    /// changes that could possibly affect the connection string. For an explanation of
    /// why we need this, see the comment in DBProfile's 'mconnString_ConnectionStringChanged'
    /// method.
    /// </summary>
    public class EventRaisingDbConnectionStringBuilder : DbConnectionStringBuilder
    {
        public event EventHandler ConnectionStringChanged;
        protected void RaiseConnectionStringChangedEvent()
        {
            if (ConnectionStringChanged != null)
                ConnectionStringChanged(this, new EventArgs());
        }

        public EventRaisingDbConnectionStringBuilder()
            : base()
        { }

        public EventRaisingDbConnectionStringBuilder(bool useOdbcRules)
            : base()
        { }

        #region Overrides

        public override void Clear()
        {
            base.Clear();
            RaiseConnectionStringChangedEvent();
        }

        public override bool Remove(string keyword)
        {
            bool bResult = base.Remove(keyword);
            RaiseConnectionStringChangedEvent();
            return bResult;
        }

        public override object this[string keyword]
        {
            get
            {
                return base[keyword];
            }
            set
            {
                base[keyword] = value;
                RaiseConnectionStringChangedEvent();
            }
        }

        #endregion

    }

    public class ConnectionType
    {
        private IProviderPlugin _plugin;
        private DbConnectionStringBuilder _connectionStringBuilder;

        public ConnectionType(string providerType, string connectionString)
        {
            SetProviderType(providerType);
            try
            {
                _connectionStringBuilder.ConnectionString = connectionString;
            }
            catch(Exception ex)
            {
                // This could be an invalid connection string for the provider
                ApplicationLog.WriteError("Unable to set the connection string.", ex);
            }
        }

        private void SetProviderType(string providerType)
        {
            _plugin = PluginManager.GetPluginByProviderType(providerType);
            _connectionStringBuilder = _plugin.CreateConnectionStringBuilder();
        }

        public string ProviderType
        {
            get { return _plugin.ProviderType; }
            set { SetProviderType(value); }
        }

        public string Description
        {
            get { return _plugin.ProviderDescription; }
        }

        public DbConnectionStringBuilder ConnectionStringBuilder
        {
            get
            {
                return _connectionStringBuilder;
            }
        }

        public override string ToString()
        {
            return _connectionStringBuilder.ConnectionString;
        }

        public IDbConnection GetConnection()
        {
            return _plugin.CreateConnection();
        }
    }

}
