using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Interstates.Control.Framework;
using Interstates.Control.Framework.Log;

namespace Interstates.Control.Database.Plugin
{
    /// <summary>
    /// A class to load all known plugins.
    /// </summary>
    static class PluginManager
    {
        static Dictionary<string, LoadedPlugin> _plugins = new Dictionary<string, LoadedPlugin>(StringComparer.CurrentCultureIgnoreCase);
        
        public static List<LoadedPlugin> Plugins
        {
            get { return new List<LoadedPlugin>(_plugins.Values); }
        }

        static PluginManager()
        {
            try
            {
                LoadPlugins();
            }
            catch (Exception exception)
            {
                ApplicationLog.WriteError("Failed to load Control.Database plugins.", exception, component: "Control.Database");
                // There is a chance that the loaded assembly doesn't have all of the references that it needs
                // In this case we want to add the default types..
            }
        }

        public static List<string> GetProviderTypes()
        {
            return new List<string>(_plugins.Keys);
        }

        public static IProviderPlugin GetPluginByProviderType(string providerType)
        {
            if (false == _plugins.ContainsKey(providerType))
                throw new DatabaseException(string.Format("The provider {0} was not found.", providerType));
            return _plugins[providerType].ProviderPlugin;
        }

        private static void LoadPlugins()
        {
            string strPluginPath = DatabaseConfiguration.DatabaseSettings.PluginLocation;
            string strConfigPath = Path.GetDirectoryName(ApplicationConfiguration.AppConfig.FilePath);
            string[] plugins = new string[] { };
            Assembly thisAssembly = Assembly.GetCallingAssembly();

            // Add default plugins from our Framework assembly
            LoadedPlugin sql = new LoadedPlugin(thisAssembly.Location, typeof(SqlProviderPlugin));
            LoadedPlugin odbc = new LoadedPlugin(thisAssembly.Location, typeof(OdbcProviderPlugin));
            LoadedPlugin oledb = new LoadedPlugin(thisAssembly.Location, typeof(OleDbProviderPlugin));
            _plugins.Add(sql.ProviderPlugin.ProviderType, sql);
            _plugins.Add(odbc.ProviderPlugin.ProviderType, odbc);
            _plugins.Add(oledb.ProviderPlugin.ProviderType, oledb);

            // If the configuration file did not specify a plugin location use the default from the registry
            if (string.IsNullOrEmpty(strPluginPath))
                strPluginPath = DefaultPluginLocation;

            // If the plugins do not exist where the config file was loaded, check the location of the executable
            if (Directory.Exists(Path.Combine(strConfigPath, strPluginPath)) == true)
            {
                strPluginPath = Path.Combine(strConfigPath, strPluginPath);
            }
            else
            {
                strPluginPath = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), strPluginPath);
            }

            try
            {
                plugins = Directory.GetFiles(strPluginPath, "*.dll");
            }
            catch { }
            foreach (string plugin in plugins)
            {
                try
                {
                    foreach (LoadedPlugin p in LoadPlugins(Assembly.LoadFile(plugin)))
                        _plugins.Add(p.ProviderPlugin.ProviderType, p);
                }
                catch (Exception exception)
                {
                    Exception meaningfulException = exception;

                    while (null != meaningfulException.InnerException)
                        meaningfulException = meaningfulException.InnerException;
                    ApplicationLog.WriteError(string.Format("Unable to load plugin {0}", plugin), meaningfulException ?? exception);
                }
            }
        }

        private static string _defaultPluginLocation = null;
        const string DEFAULT_PLUGIN_LOCATION = @"plugins\DataProviders";
        /// <summary>
        /// The default location to load the plugins from if the SecuritySetting does not specify the location.
        /// </summary>
        public static string DefaultPluginLocation
        {
            get
            {
                if (string.IsNullOrEmpty(_defaultPluginLocation))
                {
                    _defaultPluginLocation = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), DEFAULT_PLUGIN_LOCATION);
                    if (false == Directory.Exists(_defaultPluginLocation))
                    {
                        _defaultPluginLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
                            @"Interstates Control Systems\Control.Framework 2.0\" + DEFAULT_PLUGIN_LOCATION);
                    }
                }
                return _defaultPluginLocation;
            }
            set
            {
                _defaultPluginLocation = value;
            }
        }

        private static List<LoadedPlugin> LoadPlugins(Assembly loadedAssembly)
        {
            List<LoadedPlugin> loadedPlugins = new List<LoadedPlugin>();
            AssemblyName[] referencedAssemblies = loadedAssembly.GetReferencedAssemblies();
            bool loadedAssemblyIsValid = false;
            AssemblyName pluginInterfaceAssembly = Assembly.GetAssembly(typeof(IProviderPlugin)).GetName();

            // Check the assembly references to make sure that we can load the plugin
            // by checking the referenced version of this assembly
            foreach (AssemblyName referencedAssembly in referencedAssemblies)
            {
                if (referencedAssembly.Name == pluginInterfaceAssembly.Name)
                {
                    if (referencedAssembly.Version.Major == pluginInterfaceAssembly.Version.Major
                        &&
                        referencedAssembly.Version.Minor == pluginInterfaceAssembly.Version.Minor
                        &&
                        referencedAssembly.Version.Build == pluginInterfaceAssembly.Version.Build)
                    {
                        loadedAssemblyIsValid = true;
                    }
                    break;
                }
                else
                if (loadedAssembly.FullName == pluginInterfaceAssembly.FullName)
                {
                    loadedAssemblyIsValid = true;
                    break;
                }
            }
            if (loadedAssemblyIsValid)
            {
                Type[] loadedAssemblyTypes = loadedAssembly.GetTypes();
                Type pluginInterfaceType = typeof(IProviderPlugin);

                foreach (Type possiblePluginType in loadedAssemblyTypes)
                {
                    Type[] typeInterfaces = possiblePluginType.GetInterfaces();

                    foreach (Type typeInterface in typeInterfaces)
                    {
                        if (possiblePluginType.IsAbstract == false && pluginInterfaceType.FullName == typeInterface.FullName)
                        {
                            LoadedPlugin plugin = new LoadedPlugin(loadedAssembly.Location, possiblePluginType);
                            loadedPlugins.Add(plugin);
                        }
                    }
                }
            }

            return loadedPlugins;
        }
    }

    class LoadedPlugin : MarshalByRefObject
    {
        string _pluginFile;
        IProviderPlugin _providerPlugin;
        Assembly _controlLib;

        public LoadedPlugin(string pluginFile, Type providerPluginType)
        {
            _pluginFile = pluginFile;
            _controlLib = Assembly.LoadFrom(_pluginFile);
            _providerPlugin = Activator.CreateInstance(providerPluginType) as IProviderPlugin;
        }

        public IProviderPlugin ProviderPlugin
        {
            get { return _providerPlugin; }
        }

        public Assembly PlugInAssembly
        {
            get
            {
                return _controlLib;
            }
        }

        public override string ToString()
        {
            return _pluginFile.ToString();
        }
    }
}
