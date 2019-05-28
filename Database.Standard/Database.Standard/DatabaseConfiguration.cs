using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;
using Interstates.Control.Framework;
using Interstates.Control.Database.COM;
using Interstates.Control.Framework.Log;
using Interstates.Control.Database.Plugin;

namespace Interstates.Control.Database
{
    /// <summary>
    /// The database configuration that is defined for the application.
    /// </summary>
    public class DatabaseConfiguration
    {
        private static DatabaseProfile _defaultProfile;
        private static DatabaseSettings _databaseSettings;
        private static bool _initialized;

        private const string DATABASE_SETTINGS = "DatabaseSettings";
        internal const string DATABASE_NAMESPACE = "tag:interstates.com,2012:Control.Database/2.0";

        static DatabaseConfiguration()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            ApplicationConfiguration.ConfigurationChanged += new EventHandler<ConfigurationChangedEventArgs>(ApplicationConfiguration_ConfigurationChanged);
            ApplicationConfiguration.ConfigurationFileChanged += new EventHandler<ConfigurationFileChangedEventArgs>(ApplicationConfiguration_ConfigurationFileChanged);

            Init();
        }

        /// <summary>
        /// Gets the DatabaseSettings defined in the application configuration file.
        /// </summary>
        public static DatabaseSettings DatabaseSettings
        {
            get
            {
                return _databaseSettings;
            }
        }

        /// <summary>
        /// Gets the list of valid provider types based on the loaded database plugins.
        /// </summary>
        public static List<string> ProviderTypes
        {
            get
            {
                return Plugin.PluginManager.GetProviderTypes();
            }
        }

        /// <summary>
        /// Gets the ProviderPlugin associated with the provider type specified
        /// </summary>
        /// <param name="providerType">The provider type used to find the provider plugin.</param>
        /// <returns></returns>
        public static IProviderPlugin GetProviderPlugin(string providerType)
        {
            return Plugin.PluginManager.GetPluginByProviderType(providerType);
        }

        /// <summary>
        /// Gets the ProviderPlugin associated with the provider type specified
        /// </summary>
        /// <param name="profile">The DatabaseProfile to use to find the provider plugin.</param>
        /// <returns></returns>
        public static IProviderPlugin GetProviderPlugin(DatabaseProfile profile)
        {
            return Plugin.PluginManager.GetPluginByProviderType(profile.ProviderType);
        }

        /// <summary>
        /// Gets the default database profile.
        /// </summary>
        public static DatabaseProfile DefaultProfile
        {
            get
            {
                // If we already have the default profile return that, otherwise look through the collection to find  it.
                if (_defaultProfile == null || string.Compare(_defaultProfile.Name, DatabaseSettings.DefaultProfile, StringComparison.InvariantCultureIgnoreCase) != 0)
                {
                    _defaultProfile = Profiles[DatabaseSettings.DefaultProfile];
                    if (_defaultProfile == null)
                        throw new DatabaseException(String.Format("The default profile '{0}' was not found.  Check the spelling of the profile.", DatabaseSettings.DefaultProfile));
                }
                return _defaultProfile;
            }
        }

        /// <summary>
        /// Gets the database profiles that are defined for the application.
        /// </summary>
        public static DatabaseProfiles Profiles
        {
            get
            {
                return DatabaseSettings.Profiles;
            }
        }

        /// <summary>
        /// Force the database configuration to initialize. All settings are reloaded from the configuration file assigned the AppConfig property.
        /// </summary>
        public static void Initialize()
        {
            Init();
        }

        /// <summary>
        /// Initializes the log and database sections
        /// </summary>
        private static void Init()
        {
            if (_initialized) return;

            try
            {
                LoadConfigurationSettings(ApplicationConfiguration.AppConfig);
                _initialized = true;
            }
            catch (System.Exception ex)
            {
                throw new DatabaseException("Application initialization failed. " + ex.ToString());
            }
        }

        /// <summary>
        /// The configuration file changed.
        /// </summary>
        static void ApplicationConfiguration_ConfigurationChanged(object sender, ConfigurationChangedEventArgs e)
        {
            try
            {
                LoadConfigurationSettings(ApplicationConfiguration.AppConfig);
                System.Diagnostics.Debug.WriteLine("The database configuration was reloaded from " + e.Configuration.FilePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Unable to reload the configuration settings. " + ex.ToString());
            }
        }

        /// <summary>
        /// One of the configuration files or sources changed, check to see if it was ours.
        /// </summary>
        static void ApplicationConfiguration_ConfigurationFileChanged(object sender, ConfigurationFileChangedEventArgs e)
        {
            try
            {
                if ((string.IsNullOrEmpty(DatabaseSettings.SectionInformation.ConfigSource) && (string.Compare(ApplicationConfiguration.AppConfig.FilePath, e.FullPath, true) == 0))
                    ||
                    string.Compare(DatabaseSettings.SectionInformation.ConfigSource, e.FileName, true) == 0)
                {
                    LoadConfigurationSettings(ApplicationConfiguration.AppConfig);
                    ApplicationLog.WriteInfo("Database settings were reloaded from " + e.FullPath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Unable to reload the settings from {0}. ", e.FullPath) + ex.ToString());
            }
        }


        /// <summary>
        /// Gets the DatabaseSettings for a configuration.
        /// </summary>
        /// <param name="configurationFile">The configuration file to read settings from</param>
        /// <returns>The DatabaseSettings from the file</returns>
        public static DatabaseSettings GetDatabaseSettings(Configuration configurationFile)
        {
            DatabaseSettings databaseSettings = configurationFile.Sections[DATABASE_SETTINGS] as DatabaseSettings;
            if (databaseSettings == null)
            {
                // Allows the configuration section to be in the Interstates sectionGroup
                databaseSettings = (DatabaseSettings)configurationFile.GetSection("Interstates/" + DATABASE_SETTINGS);
                if (databaseSettings == null)
                {
                    databaseSettings = ApplicationConfiguration.FindConfigurationSection(configurationFile, typeof(DatabaseSettings)) as DatabaseSettings;
                }
            }
            return databaseSettings;
        }

        /// <summary>
        /// Gets the DatabaseSettings from the file.
        /// </summary>
        /// <param name="fileName">The configuration file to read settings from</param>
        /// <returns>The DatabaseSettings from the file</returns>
        public static DatabaseSettings GetDatabaseSettings(string fileName)
        {
            return GetDatabaseSettings(ApplicationConfiguration.LoadConfiguration(fileName));
        }
        /// <summary>
        /// Loads database configuration settings from a configuration file.
        /// </summary>
        /// <param name="configurationFile"></param>
        /// <returns></returns>
        public static void LoadConfigurationSettings(Configuration configurationFile)
        {
            _databaseSettings = GetDatabaseSettings(configurationFile);
            if (_databaseSettings == null)
            {
                _databaseSettings = new DatabaseSettings();
                configurationFile.Sections.Remove(DATABASE_SETTINGS);
                if (configurationFile.Sections[DATABASE_SETTINGS] == null)
                    configurationFile.Sections.Add(DATABASE_SETTINGS, _databaseSettings);
            }
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // This handler is called only when the common language runtime tries to bind to the assembly and fails.
            // This occurs when a executable other than .NET loads an COM component.

            Assembly missingAssembly = null, execAssembly;
            string strMissingAssembly;

            if (args.Name.Contains(",") == true)
            {
                strMissingAssembly = args.Name.Substring(0, args.Name.IndexOf(","));
            }
            else
            {
                strMissingAssembly = args.Name;
            }

            execAssembly = Assembly.GetExecutingAssembly();
            if (strMissingAssembly == execAssembly.GetName().Name)
                //Load the assembly from the specified path. 					
                missingAssembly = execAssembly; // Assembly.LoadFrom(execAssembly.Location);

            //Return the loaded assembly.
            return missingAssembly;
        }
    }
}
