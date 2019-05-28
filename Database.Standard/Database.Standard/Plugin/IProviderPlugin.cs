using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
using System.Data.Common;
using System.Data;

namespace Interstates.Control.Database.Plugin
{
    public interface IProviderPlugin
    {
        /// <summary>
        /// Create an object that can be used as a DBQuery object.
        /// </summary>        
        QueryBase CreateQuery(DatabaseProfile profile);

        /// <summary>
        /// Create a connection object for the plugin.
        /// </summary>
        /// <returns></returns>
        IDbConnection CreateConnection();

        /// <summary>
        /// Create a ConnectionStringBuilder that can be used to create a connection string.
        /// </summary>
        /// <returns></returns>
        DbConnectionStringBuilder CreateConnectionStringBuilder();

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        /// <returns></returns>
        string ProviderName { get; }

        /// <summary>
        /// 
        /// </summary>
        string ProviderDescription { get; }

        /// <summary>
        /// The name of the provider type.
        /// </summary>
        /// <returns></returns>
        string ProviderType { get; }

        /// <summary>
        /// Return the plugin assembly product name.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Return the plugin assembly version.
        /// </summary>
        string Version { get; }
        /// <summary>
        /// Return the plugin assembly description.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Return the plugin assembly company name.
        /// </summary>
        string Company { get; }
    }
}
