using System.Data.Odbc;
using System.Reflection;
using Interstates.Control.Database.Plugin; 

namespace Interstates.Control.Database
{
    class OdbcProviderPlugin : IProviderPlugin
    {
        #region IProviderPlugin

        public System.Data.IDbConnection CreateConnection()
        {
            return new OdbcConnection();
        }

        public System.Data.Common.DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return new OdbcConnectionStringBuilder();
        }

        public QueryBase CreateQuery(DatabaseProfile profile)
        {
            return new OdbcQuery(profile);
        }

        public string ProviderDescription
        {
            get { return "ODBC Connection"; }
        }

        public string ProviderName
        {
            get { return "MSDASQL"; }
        }

        public string ProviderType
        {
            get { return "ODBC"; }
        }

        #region Assembly properties

        public string Name
        {
            get
            {
                // Get all Product attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // If there aren't any Product attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Product attribute, return its value
                return ((AssemblyTitleAttribute)attributes[0]).Title;
            }
        }

        public string Version
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string Description
        {
            get
            {
                // Get all Description attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                // If there aren't any Description attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Description attribute, return its value
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string Company
        {
            get
            {
                // Get all Company attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                // If there aren't any Company attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Company attribute, return its value
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
        #endregion IProviderPlugin
    }
}
