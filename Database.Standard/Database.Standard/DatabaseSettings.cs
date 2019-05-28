using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace Interstates.Control.Database
{

    /// <summary>
    /// This class handles database specific configuration settings in App.Config
    /// <remarks>
    /// Special considerations:
    ///         The configuaration settings are kept in 
    ///         the DatabaseSettings section of the App.Config file.
    /// </remarks>
    /// </summary>
    [ComVisible(false)]
    public sealed class DatabaseSettings : ConfigurationSection
	{
        string _fileDefaultProfile;

		public DatabaseSettings()
		{
            // Create a new instance of the profiles to add it to the configuration
        }

        [ConfigurationProperty("DefaultProfile", 
            DefaultValue = "Default",
            IsRequired = false, 
            IsKey = false)]
        public string DefaultProfile
        {
            get
            {
                return (string)this["DefaultProfile"];
            }
            set
            {
                this["DefaultProfile"] = value;
            }
        }

        /// <summary>
        /// Addes a collection of profiles to the configuration file
        /// Note: the "IsDefaultCollection = false" 
        /// instructs the .NET Framework to build a nested 
        /// section like <Profiles> ...</Profiles>.
        /// </summary>
        [ConfigurationProperty("Profiles",
            IsDefaultCollection = false)]
        public DatabaseProfiles Profiles
        {
            get
            {
                return (DatabaseProfiles)base["Profiles"];;
            }
        }

        /// <summary>
        /// The PluginLocation is the folder to the Plugins\DataProviders folder.
        /// </summary>
        [ConfigurationProperty("PluginLocation", IsRequired = false)]
        public string PluginLocation
        {
            get
            {
                return (string)this["PluginLocation"];
            }
            set
            {
                this["PluginLocation"] = value;
            }
        }

        /// <summary>
        /// The location of the database settings files. This file does not have to be
        /// in the path of the application.
        /// </summary>
        [ConfigurationProperty("file", DefaultValue = "")]
        public string File
        {
            get
            {
                string str = (string)base["file"];
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set
            {
                base["file"] = value;
            }
        }

        public void CopyTo(DatabaseSettings setting)
        {
            foreach (ConfigurationProperty property in this.Properties)
            {
                if (property.Type == typeof(DatabaseProfiles))
                {
                    setting[property.Name] = this.Profiles.Copy();
                }
                else
                {
                    setting[property.Name] = this[property.Name];
                }
            }
        }

        public DatabaseSettings Copy()
        {
            DatabaseSettings copySetting = new DatabaseSettings();

            this.CopyTo(copySetting);
            return copySetting;
        }

        protected override string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
        {
            StringWriter sWriter = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
            XmlWriterSettings xSettings = new XmlWriterSettings();

            xSettings.Indent = true;
            xSettings.IndentChars = "\t";
            xSettings.OmitXmlDeclaration = true;

            XmlWriter xWriter = XmlWriter.Create(sWriter, xSettings);

            this.SerializeToXmlElement(xWriter, name);
            xWriter.Flush();
            return sWriter.ToString();
        }
        
        protected override bool SerializeToXmlElement(XmlWriter writer, string elementName)
        {
            if (writer == null)
                return false;

            bool success = true;

            writer.WriteStartElement(elementName);
            if (string.IsNullOrEmpty(File))
            {
                success = SerializeElement(writer, false);
            }
            else
            {
                if (!string.IsNullOrEmpty(DefaultProfile))
                {
                    writer.WriteAttributeString("DefaultProfile", DefaultProfile);
                }

                writer.WriteAttributeString("file", File);

                using (FileStream file = new FileStream(File, FileMode.Create, FileAccess.Write))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();

                    settings.Indent = true;
                    settings.IndentChars = ("\t");
                    settings.OmitXmlDeclaration = false;
                    
                    XmlWriter wtr = XmlWriter.Create(file, settings);
                    wtr.WriteStartDocument();
                    wtr.WriteStartElement(elementName);
                    success = SerializeElement(wtr, false);
                    wtr.Flush();
                    wtr.Close();
                }
            }
            writer.WriteEndElement();

            return success;
        }

        protected override bool SerializeElement(XmlWriter writer, bool serializeCollectionKey)
        {
            string restoreDefault = DefaultProfile;
            string restoreFile = File;
            try
            {
                if (!string.IsNullOrEmpty(File))
                {
                    DefaultProfile = _fileDefaultProfile;
                    File = null; // We don't want to write the File attribute out to the file
                }

                return base.SerializeElement(writer, serializeCollectionKey);
            }
            finally
            {
                DefaultProfile = restoreDefault;
                File = restoreFile;
            }
        }

        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            string name = reader.Name;
            string defaultProfile;

            base.DeserializeElement(reader, serializeCollectionKey);

            // After the deserializing we will have the DefaultProfile 
            // defined in the Application configuration file, not the DefaultProfile
            // defined by the file specified in the File attribute.
            defaultProfile = DefaultProfile;

            if ((this.File != null) && (this.File.Length > 0))
            {
                string file;
                string source = base.ElementInformation.Source;
                if (string.IsNullOrEmpty(source))
                {
                    file = this.File;
                }
                else
                {
                    file = Path.Combine(Path.GetDirectoryName(source), this.File);
                }
                if (System.IO.File.Exists(file))
                {
                    using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (XmlTextReader reader2 = new XmlTextReader(stream))
                        {
                            reader2.WhitespaceHandling = WhitespaceHandling.None;
                            bool flag = false;
                            while (!flag && reader2.Read())
                            {
                                switch (reader2.NodeType)
                                {
                                    case XmlNodeType.Comment:
                                    case XmlNodeType.DocumentType:
                                    case XmlNodeType.XmlDeclaration:
                                        {
                                            continue;
                                        }
                                    case XmlNodeType.Element:
                                        {
                                            flag = true;
                                            continue;
                                        }
                                }
                            }
                            // We should be on the DatabaseSettings now
                            reader2.MoveToElement();
                            base.DeserializeElement(reader2, serializeCollectionKey);
                        }
                    }
                }

                // Remember what the file has for a default profile.
                // We don't want to overwrite it when someone saves the configuration.
                _fileDefaultProfile = DefaultProfile;
                DefaultProfile = defaultProfile;
            }
        }
	}
}
