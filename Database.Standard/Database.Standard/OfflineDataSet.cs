using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Interstates.Control.Framework.Log;
using Interstates.Control.Framework.Security;


namespace Interstates.Control.Database
{
	/// <summary>
	/// Summary description for OfflineDataSet.
	/// </summary>
    [ComVisible(false)]
    public class OfflineDataSet
	{
		private String _offlineFolder;
		private String _fileName;
		private String _offlineFileExt;
		private String _schemaName;
        private String _tableName;

		/// <summary>
		/// Determines the format to save the file in.
		/// </summary>
		private OfflineFormat _streamFormat;

		/// <summary>
		/// All data for the class is stored here
		/// </summary>
		private DataSet _dataSet;

		#region Constructors
		/// <summary>
		/// Take the table offline so that data can be read from a local file instead of the database
		/// </summary>
		protected OfflineDataSet( String tableName, String schemaName, OfflineFormat streamFormat )
		{
			ApplicationAssert.Check( tableName != null || tableName.Length == 0, "Invalid Table name.", ApplicationAssert.LineNumber );
			ApplicationAssert.Check( schemaName != null || schemaName.Length == 0, "Invalid Schema name.", ApplicationAssert.LineNumber );

            _tableName = tableName;
			_fileName = "Offline_" + tableName;
			_schemaName = schemaName;
			_streamFormat = streamFormat;
			Constructor();
		}

		/// <summary>
		/// Take the table offline so that data can be read from a local file instead of the database
		/// </summary>
		protected OfflineDataSet( String tableName, String schemaName )
		{
			ApplicationAssert.Check( tableName != null || tableName.Length == 0, "Invalid Table name.", ApplicationAssert.LineNumber );
			ApplicationAssert.Check( schemaName != null || schemaName.Length == 0, "Invalid Schema name.", ApplicationAssert.LineNumber );

            _fileName = tableName;
            _fileName = "Offline_" + tableName;
			_schemaName = schemaName;
			// By default, use binary format so that any changes in row values are preserved
			_streamFormat = OfflineFormat.Binary;
			Constructor();
		}

		private void Constructor()
		{			
			try
			{
				if( OfflineFormat.Xml == _streamFormat )
					_offlineFileExt = ".xml";
				else
					_offlineFileExt = ".bin";

				// Create an empty dataset
				_dataSet = ClearOfflineData();
			}
			catch( System.Exception ex )
			{
				ApplicationLog.WriteError( "Initialize OfflineDataSet error", ex );
                throw;
            }		
		}
		#endregion // Constructors
		
		#region Public Properties
		/// <summary>
		/// The offline folder property contains the location of the offline files.
		/// </summary>
		public String OfflineFolder
		{
			get
			{
				return _offlineFolder;
			}
			set
			{
				_offlineFolder = value;
				// Remove the \ at the end of the path
				if( _offlineFolder.LastIndexOf( @"\" ) == _offlineFolder.Length-1 )
					_offlineFolder = _offlineFolder.Substring( 0, _offlineFolder.Length-1 );
			}
		}

		/// <summary>
		/// The offline file property contains the full path name to the offline file.
		/// </summary>
		public String OfflineFile
		{
			get
			{
				if( _offlineFolder.Length == 0 )
					return _fileName + _offlineFileExt;
				else
					return _offlineFolder + @"\" + _fileName + _offlineFileExt;
			}
		}

		/// <summary>
		/// The name of the file extension for the offline file
		/// </summary>
		public String OfflineFileExt
		{
			get
			{
				return _offlineFileExt;
			}
			set
			{
				_offlineFileExt = value;
			}
		}

		/// <summary>
		/// The format to use for the offline file.
		/// </summary>
		public OfflineFormat OfflineFileFormat
		{
			get
			{
				return _streamFormat;
			}
			set
			{
				_streamFormat = value;
			}
		}
		
		/// <summary>
		/// Returns the offline Data
		/// </summary>
		public DataSet OfflineData
		{
			get
			{
				return _dataSet;
			}
		}
		#endregion // Public Properties

		/// <summary>
		/// Initializes the data
		/// </summary>	
		/// <returns></returns>
		public DataSet ClearOfflineData()
		{
			if( null == _dataSet )
			{
				// Create an offline copy to store data
				_dataSet = new DataSet();
                Assembly clsAssembly = Assembly.GetCallingAssembly();
				//			String[] resources = clsAssembly.GetManifestResourceNames();
				//			String list = "";
				//			foreach( String  resource in resources )
				//			{
				//				list += resource + Environment.NewLine;
				//			}

				// Read the embedded resource to get the dataset format
				_dataSet.ReadXmlSchema( clsAssembly.GetManifestResourceStream( this.GetType(), _schemaName ) );
			}
			foreach( DataTable tblOffline in _dataSet.Tables )
			{
				tblOffline.Clear();

				// Add the OfflineTranId column if it doesn't exist
				if( tblOffline.Columns.Contains( "OfflineTranId" ) == false )
				{
					tblOffline.Columns.Add( "OfflineTranId", System.Type.GetType("System.Int64") );
				}			
			}

			return _dataSet;
		}

		/// <summary>
		/// Reads the dataset from the offline file
		/// </summary>
		/// <returns>A DataSet</returns>
		public DataSet LoadOffline()
		{
			switch( _streamFormat )
			{
				case OfflineFormat.Xml:
					LoadXML();
					break;

				case OfflineFormat.Binary:
					LoadBinary();
					break;

				case OfflineFormat.Encrypt:
					LoadEncrypt();
					break;
			}
			return _dataSet;
		}	
		
		/// <summary>
		/// Reads the dataset from the offline file using the XML format option
		/// </summary>
		private void LoadXML()
		{
			String strFile = OfflineFile;

			try
			{
				// Read the offline file
				if( System.IO.File.Exists( strFile ) )
				{
					_dataSet.ReadXml( strFile, XmlReadMode.Auto );
				}
			}
			catch( Exception ex )
			{
				ApplicationLog.WriteError( "Failed to Load XML file.  ", ex );
                throw;
            }
		}

		/// <summary>
		/// Reads the dataset from the offline file using the binary format option
		/// </summary>
		/// <returns>A DataSet</returns>
		private void LoadBinary()
		{
			FileStream clsStream = null;
			IFormatter clsFormat = new BinaryFormatter();
			String strOfflineFile = OfflineFile;

			try
			{
				if( File.Exists( strOfflineFile ) )
				{
					clsStream = new FileStream( strOfflineFile, FileMode.Open, FileAccess.Read, FileShare.Read);
					_dataSet = (DataSet) clsFormat.Deserialize( clsStream );
				}
				else
				{
					// Nothing to do
				}
			}
			catch( SerializationException se )
			{
				ApplicationLog.WriteError( "Failed to Read Offline file.  ", se );
                throw;
            }
			finally
			{
				if( clsStream != null )
					clsStream.Close();
			}
		}	

		/// <summary>
		/// Reads the dataset from the offline file using the encrypted format option
		/// </summary>
		/// <returns>A DataSet</returns>
		private DataSet LoadEncrypt() 
		{
			string strFile = this.OfflineFile;
			object objData = new object();

			try 
			{
				// 1. Open the file
				StreamReader sr = new StreamReader(strFile);
		    
				try 
				{
					MemoryStream streamMemory;
					BinaryFormatter formatter = new BinaryFormatter();
				
					// 2. Read the binary objData, and convert it to a string
					string cipherData = sr.ReadToEnd();
				
					// 3. Decrypt the binary objData
					byte[] binaryData = Convert.FromBase64String(DataProtection.Decrypt(cipherData, DataProtection.Store.User));
				
					// 4. Rehydrate the dataset
					streamMemory = new MemoryStream(binaryData);
					objData = formatter.Deserialize(streamMemory);
				} 
				catch 
				{ 
					// objData could not be deserialized
					objData = null;
				} 
				finally 
				{
					// 5. Close the reader
					sr.Close();
				}
			} 
			catch 
			{
				// file doesn't exist
				objData = null;
                throw;
            }

			return (DataSet)objData;
		}

		/// <summary>
		/// Writes the data to the offline file 
		/// New, modified and updated records will be associated with this transaction
		/// </summary>
		public void SaveOffline( Int64 offlineTranId )
		{
			DataRow[] clsRows;

			// Update each set of affected records
			foreach( DataTable tblOffline in _dataSet.Tables )
			{
				// Add the OfflineTranId column if it doesn't exist
				if( this._dataSet.Tables[_tableName].Columns.Contains( "OfflineTranId" ) == false )
				{
                    this._dataSet.Tables[_tableName].Columns.Add("OfflineTranId", System.Type.GetType("System.Int64"));
				}

                clsRows = _dataSet.Tables[_tableName].Select("", "", DataViewRowState.Deleted);
				foreach( DataRow clsRow in clsRows )
				{
					clsRow["OfflineTranId"] = offlineTranId;
				}
                clsRows = _dataSet.Tables[_tableName].Select("", "", DataViewRowState.ModifiedCurrent);
				foreach( DataRow clsRow in clsRows )
				{
					clsRow["OfflineTranId"] = offlineTranId;
				}
                clsRows = _dataSet.Tables[_tableName].Select("", "", DataViewRowState.Added);
				foreach( DataRow clsRow in clsRows )
				{
					clsRow["OfflineTranId"] = offlineTranId;
				}
			}

			// Save the dataset
			//this.Save();
		}

		/// <summary>
		/// Writes the data to the offline file
		/// </summary>
		public void SaveOffline()
		{
			switch( _streamFormat )
			{
				case OfflineFormat.Xml:
					SaveXML();
					break;

				case OfflineFormat.Binary:
					SaveBinary();
					break;
			
				case OfflineFormat.Encrypt:
					SaveEncrypt();
					break;
			}		
			ApplicationLog.WriteInfo( "Offline data saved to file '" + OfflineFile + "'" );
		}

		/// <summary>
		/// Save the data to an offline file in XML format.
		/// </summary>
		private void SaveXML()
		{
			// Save the dataset as xml
			_dataSet.WriteXml( OfflineFile, XmlWriteMode.WriteSchema );
		}

		/// <summary>
		/// Writes the data to the offline file in binary format.
		/// </summary>
		private void SaveBinary()
		{
			FileStream clsStream = null;
			BinaryFormatter clsFormat = new BinaryFormatter();

			try
			{
				// Serialize object to a file
				clsStream = File.Create( OfflineFile );
				clsFormat.Serialize( clsStream, _dataSet );
			}
			catch( SerializationException se )
			{
				ApplicationLog.WriteError( "Failed to Save Offline binary file. ", se );
                throw;
			}
			finally
			{
				if( clsStream != null )
					clsStream.Close();
			}
		}

		/// <summary>
		/// Saves the dataset to the offline file using the encrypted format option
		/// </summary>
		private void SaveEncrypt() 
		{
			object objData;
			string strFile;

			try 
			{
				strFile = this.OfflineFile;
				objData = _dataSet;

				// 1. Open the file
				StreamWriter fs = new StreamWriter(strFile, false);
	        
				try 
				{
					MemoryStream streamMemory = new MemoryStream();
					BinaryFormatter formatter = new BinaryFormatter();
				
					// 2. Serialize the dataset object using the binary formatter
					formatter.Serialize(streamMemory, objData);
				
					// 3. Encrypt the binary objData
					string binaryData = Convert.ToBase64String(streamMemory.GetBuffer());
					string cipherData = DataProtection.Encrypt(binaryData, DataProtection.Store.User);
				
					// 4. Write the objData to a file
					fs.Write(cipherData);
				} 
				finally 
				{
					// 5. Close the file
					fs.Flush();
					fs.Close();
				}
			} 
			catch
			{
                throw;
			}
		}
	}
}
