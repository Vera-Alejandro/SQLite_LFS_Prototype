using Dapper;
using Interstates.Control.Database;
using Interstates.Control.Database.SQLite3;
using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Data.Common;
using System.Data.SQLite;
using System.Xml.Serialization;
using System.Collections.Generic;
using SQLite_LFS_Prototype.Model;
using System.Text;

namespace SQLite_LFS_Prototype
{
    public class Database
    {
        private SQLiteConnection FileConnection { get; set; }
        private FormatType CommandAction { get; set; }
        private string constr { get; set; }

        private readonly string _sqlitePath;

        private readonly string _manual = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data\ManualTx\");

        private readonly string _pending = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data\PendingTx\");

        private readonly string _processed = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data\ProcessedTx\");

        private readonly string _extensions = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data\ExtensionInfo\");

        //constructor
        public Database(string FilePath)
        {
            _sqlitePath = FilePath;
            constr = $"Data Source={_sqlitePath};Version=3;";


            if (!(File.Exists(_sqlitePath)))
            {
                SQLiteConnection.CreateFile(_sqlitePath);
                Console.WriteLine("SQLite File Created.");
            }

            FileConnection = new SQLiteConnection(constr);
            CommandAction = FormatType.None;
        }

        //
        //Functions
        //

        public bool Connect()
        {
            if (File.Exists(_sqlitePath) && FileConnection.State != ConnectionState.Open)
            {
                FileConnection.Open();
                Console.WriteLine("File connection made.");
                return true;
            }
            else
            {
                Console.WriteLine("File connection was not made.");
                return false;
            }
        }

        public bool Disconnect()
        {
            if (File.Exists(_sqlitePath) && FileConnection.State != ConnectionState.Closed)
            {
                FileConnection.Close();
                Console.WriteLine("\t\t\tDatabase successfully closed.");
                return true;
            }
            else
            {
                Console.WriteLine("\t\t\t\tDatabase Was Not Able To Close.");
                return false;
            }
        }

        /// <summary>
        /// This functions allowd for execution of any SQLite command.
        /// returns true if completed successfuly and false if failed
        /// </summary>
        /// <param name="SQLiteCommand"></param>
        /// <returns></returns>
        public bool ExecuteCommand(string SQLiteCommand)
        {
            try
            {
                using (SQLiteCommand newTable = new SQLiteCommand(SQLiteCommand, FileConnection))
                {
                    newTable.ExecuteNonQuery();
                    Console.WriteLine($"Command Executed Successfully");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        ///Default constructor of all the nessesary tables
        /// </summary>
        /// <returns></returns>
        public string CreateTable()
        {
            #region Sqlite Create Table Command
            string command = "CREATE TABLE IF NOT EXISTS PendingTx (" +
                                    "Id INTEGER PRIMARY KEY NOT NULL," +
                                    "Type INT," +
                                    "Data BLOB NOT NULL," +
                                    "DateCreated TEXT," +
                                    "DateUpdated TEXT" +
                            ");" +
                            "CREATE TABLE IF NOT EXISTS ProcessedTx (" +
                            "Id INTEGER PRIMARY KEY NOT NULL," +
                                    "Type INT," +
                                    "Data BLOB NOT NULL," +
                                    "DateCreated TEXT," +
                                    "DateUpdated TEXT" +
                            ");" +
                            "CREATE TABLE IF NOT EXISTS ManualTx (" +
                            "Id INTEGER PRIMARY KEY NOT NULL," +
                                "Type INT," +
                                "Data BLOB NOT NULL," +
                                "DateCreated TEXT," +
                                "DateUpdated TEXT" +
                            ");" +
                            "CREATE TABLE IF NOT EXISTS ExtensionInfo (" +
                                "Id INTEGER PRIMARY KEY NOT NULL," +
                                "Extension TEXT" +
                            ");";
            #endregion

            #region SQLite Insert Command

            string insertCommand = "INSERT INTO ExtensionInfo (Extension) VALUES ('txt'), ('exe'), ('pdf'), ('jpg'), ('cs'), ('c'), ('cpp');";

            #endregion

            try
            {
                using (SQLiteCommand newCommand = new SQLiteCommand(command, FileConnection))
                {
                    newCommand.ExecuteNonQuery();
                    return "Command Completed Successfully!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Command Failed!";
            }
        }

        /// <summary>
        /// Insert Command asking user for paramaters
        /// </summary>
        /// <param name="table"></param>
        public void InsertInto(string table)
        {
            FileData data = new FileData();

            DateTime _currentDateTime = DateTime.Now;

            Console.Write("\n\t\t\t\tEnter the Type of the new entry.\n\t\t\t\t");
            data.Type = Console.ReadLine();
            Console.WriteLine();

            Console.Write("\t\t\t\tEnter the Data for this new entry\n\t\t\t\t");
            data.Data = Encoding.ASCII.GetBytes(Console.ReadLine());
            Console.WriteLine();

            Console.Write("\t\t\t\tEnter the Extension ID for this new entry\n\t\t\t\t");
            data.ExtensionId = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

            data.DateCreated = new DateTime
                (
                    _currentDateTime.Year,
                    _currentDateTime.Month,
                    _currentDateTime.Day,
                    _currentDateTime.Hour,
                    _currentDateTime.Minute,
                    _currentDateTime.Second,
                    Convert.ToInt32(_currentDateTime.Millisecond) % 1000
                ); ;

            data.DateUpdated = new DateTime
                (
                _currentDateTime.Year,
                    _currentDateTime.Month,
                    _currentDateTime.Day,
                    _currentDateTime.Hour,
                    _currentDateTime.Minute,
                    _currentDateTime.Second,
                    Convert.ToInt32(_currentDateTime.Millisecond) % 1000
                );

            InsertRow(table, data);
        }

        /// <summary>
        /// Displays all table data from SQLite db using stream reader
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columns"></param>
        public void SelectAll(string table, List<string> columns)
        {
            #region Variables
            List<int> _tabsNeeded = new List<int>();
            List<string> _tableData = new List<string>();
            List<string> _tempColumns = new List<string>();

            string command = $"SELECT * FROM {table};";

            #endregion

            using (SQLiteCommand selectAll = new SQLiteCommand(command, FileConnection))
            {
                SQLiteDataReader read = selectAll.ExecuteReader();
                _tempColumns = GetFeilds(columns).ToList();

                foreach (var item in read)
                {
                    for (int i = 0; i < read.FieldCount; i++)
                    {
                        _tableData.Add(read.GetString(i));
                    }
                }
            }

            foreach (string entry in _tableData) { _tabsNeeded.Add((entry.Length / 8) + 1); }

            int n = 0;

            //table names
            Console.WriteLine($"\nSELECT ALL FROM {table}\n");

            foreach (string titles in _tempColumns)
            {
                Console.Write(titles);
                for (int i = 0; i < _tabsNeeded[n]; i++)
                {
                    Console.Write("\t");
                }
                n++;
            }
            Console.WriteLine();

            n = 0;

            //data corresponding to table columns
            foreach (string entry in _tableData)
            {
                if (n % 3 == 0 && n > 1)
                {
                    Console.Write("\n");
                }
                Console.Write($"{entry}\t");
                n++;
            }

            Console.WriteLine("\n");
        }

        #region Dapper Testing

        /// <summary>
        /// Insert using Dapper
        /// </summary>
        /// <param name="rowData"></param>
        /// <param name="table"></param>
        public void Insert(FileData rowData, string table)
        {
            string _command = $@"INSERT INTO {table} (Type, Data, ExtensionId, DateCreated, DateUpdated) VALUES (@Type, @Data, @ExtensionID, @DateCreated, @DateUpdated); SELECT last_insert_rowid();";

            DatabaseProfile profile = new DatabaseProfile("sqlite", constr, "SQLite3");

            rowData.Id = FileConnection.Query<int>(_command, rowData).First();
        }

        /// <summary>
        /// Select Row Data using Dapper
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public FileData SelectData(int Id, string table)
        {
            string _command = $@"SELECT Id, Name, Data, ExtensionId FROM {table} WHERE Id = @id";
            FileData _retValue = FileConnection.Query<FileData>(_command, new { Id }).FirstOrDefault();

            return _retValue;
        }

        #endregion

        /// <summary>
        /// Prints formated table given list of row data
        /// </summary>
        /// <param name="data"></param>
        string PrintTable(List<FileData> data, string CommandAfter)
        {
            int dataTabs = 8;
            int nameTabs = 1;
            int valuesDataTab;
            int valuesNameTab;
            int _retHeight;
            int _retWidth;

            string _retString;
            string readableData;

            foreach (FileData row in data)
            {
                //check to see if data is printable 
                if (row.ExtensionId == 1) //1 == txt
                {
                    //gets the length of the longest entry in data
                    if (row.Data.Length > dataTabs)
                    {
                        dataTabs = row.Data.Length;
                    }
                }

                if (row.Type.Length > nameTabs)
                {
                    nameTabs = row.Type.Length;
                }
            }

            dataTabs = (dataTabs > 8) ? 8 : (dataTabs / 8) + 1;
            nameTabs = (nameTabs / 8) + 1;

            //Printing Titles
            _retHeight = Console.WindowHeight;
            _retWidth = Console.WindowWidth;

            Console.WindowHeight = 35;
            Console.WindowWidth = 165;

            Console.Clear();

            Console.Write("ID\tName");

            for (int n = 0; n < nameTabs; n++)
            {
                Console.Write("\t");
            }

            Console.Write("Data");

            for (int i = 0; i < dataTabs; i++)
            {
                Console.Write("\t");
            }

            Console.WriteLine("Extension ID\tDate Created\t\t\tDate Updated");

            //Printing Table Data
            foreach (FileData value in data)
            {
                if (value.ExtensionId == 1) // 1 == .txt
                {
                    readableData = Encoding.ASCII.GetString(value.Data);
                }
                else
                {
                    readableData = "This data is not in a readable format";
                }

                valuesDataTab = (value.ExtensionId == 1) ? (value.Data.Length > 60) ? 1 : dataTabs - (value.Data.Length / 8) : dataTabs - (readableData.Length / 8);
                valuesNameTab = nameTabs - (value.Type.Length / 8);

                Console.Write($"{value.Id.ToString()}\t{value.Type}");

                for (int n = 0; n < valuesNameTab; n++)
                {
                    Console.Write("\t");
                }

                if (readableData.Length > 60)
                {
                    Console.Write($"{readableData.Remove(60)}...");
                }
                else
                {
                    Console.Write(readableData);
                }

                for (int k = 0; k < valuesDataTab; k++)
                {
                    Console.Write("\t");
                }

                Console.WriteLine($"{value.ExtensionId}\t\t{value.DateCreated}\t\t{value.DateUpdated}");
            }

            Console.Write($"\n\n{CommandAfter}");
            _retString = Console.ReadLine();

            Console.Clear();

            Console.WindowHeight = _retHeight;
            Console.WindowWidth = _retWidth;

            return _retString;
        }

        /// <summary>
        /// Drops selected table 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public string DropTable(string table)
        {
            string command = $"DROP TABLE IF EXISTS {table};";

            try
            {
                using (SQLiteCommand dropTable = new SQLiteCommand(command, FileConnection))
                {
                    dropTable.ExecuteNonQuery();
                    Console.WriteLine("Comand Executed Successfully");
                    return "Command Successfully Executed";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Operation Failed";
            }
        }

        /// <summary>
        /// Converts data from XML file into a RowData Object given the file path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public FileData Deserialize(string path)
        {
            FileData _returnValue;

            XmlSerializer reader = new XmlSerializer(typeof(FileData));

            using (Stream _readTx = new FileStream(path, FileMode.Open))
            {
                _returnValue = (FileData)reader.Deserialize(_readTx);
            }

            return _returnValue;
        }

        /// <summary>
        /// Converts objects into an XML file for later storage
        /// </summary>
        /// <param name="data"></param>
        /// <param name="table"></param>
        public bool Serialize(FileData data, string table)
        {
            string _path = "";

            XmlSerializer writer = new XmlSerializer(typeof(FileData));

            if (table == "ManualTx") { _path = _manual + $"{table}_ID_{data.Id}_Tx.tranx"; }
        
            if(table == "PendingTx") { _path = _pending + $"{table}_ID_{data.Id}_Tx.tranx"; }

            if(table == "ProcessedTx") { _path = _processed + $"{table}_ID_{data.Id}_Tx.tranx"; }

            if(table == "Extensioninfo") { _path = _extensions + $"{table}_ID_{data.Id}_Tx.tranx"; }

            if(_path == "") { return false; }

            try
            {
                if (File.Exists(_path))
                {
                    File.Delete(_path);
                }

                FileStream _newTransaction = _newTransaction = File.Create(_path);

                writer.Serialize(_newTransaction, data);

                _newTransaction.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns all data from a table and stores it in a DataSet using Control.Database.SQLite3
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public DataSet GrabData(string table)
        {
            string _command = $"SELECT * FROM {table};";

            DataSet data = new DataSet();

            using (DbCommand command = FileConnection.CreateCommand())
            {
                DatabaseProfile profile = new DatabaseProfile("sqlite", constr, "SQLite 3");
                SQLiteQuery query = new SQLiteQuery(profile);

                command.CommandText = _command;
                command.CommandType = CommandType.Text;
                data = query.Execute(command);
            }

            Console.Clear();

            return data;
        }

        /// <summary>
        /// Delete Row Menu.
        /// Waits for User input
        /// </summary>
        /// <param name="table"></param>
        public void DeleteRow(string table)
        {
            #region variables 

            int _rowChoice;
            int _rowsAffected;

            bool _rowContinue = true;

            DataSet data = GrabData(table);

            List<FileData> _rowData = new List<FileData>();
            List<ExtensionInfo> _extInfo = new List<ExtensionInfo>();
            List<Int64> _IDs = new List<Int64>();

            #endregion

            //sets the data to the correst List
            if (table == "ExtensionInfo")
            {
                foreach (DataRow row in data.Tables[0].Rows)
                {
                    _extInfo.Add(new ExtensionInfo()
                    {
                        Id = (Int64)row[0],
                        Extension = (string)row[1]
                    });

                    _IDs.Add((long)row[0]);
                }
            }

            foreach (DataRow row in data.Tables[0].Rows)
            {
                _rowData.Add(new FileData()
                {
                    Id = (Int64)row[0],
                    Type = (string)row[1],
                    Data = (Byte[])row[2],
                    DateCreated = Convert.ToDateTime(row[3]),
                    DateUpdated = Convert.ToDateTime(row[4]),
                    ExtensionId = (Int64)row[5]
                });

                _IDs.Add((Int64)row[0]);
            }

            do
            {
                string _parseStr = PrintTable(_rowData, "What Row would you like to remove? Press 0 to exit: ");
                if (!int.TryParse(_parseStr, out _rowChoice)) { _rowChoice = -1; }

                if (_rowChoice == 0) { _rowContinue = false; break; }

                if (_rowChoice > _IDs.Max() || _rowChoice < 0)
                {
                    Console.WriteLine("Please enter a value in range.");
                    Console.WriteLine("Press Any Key To Continue...");
                    Console.ReadKey();
                    break;
                }

                try
                {
                    if (!_IDs.Contains(_rowChoice))
                    {
                        _rowContinue = true;
                        Console.Clear();
                        Console.WriteLine("Please Enter a Valid Option");
                        Console.WriteLine("Press Any Key To Continue...");
                        Console.ReadKey();
                        break;
                    }

                    _rowData = DeleteRow(table, _rowChoice, _rowData);
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press Any Key To Continue...");
                    Console.ReadKey();
                }

            } while (_rowContinue);
        }

        /// <summary>
        /// Displays data from indivisual row. 
        /// Waits for user input
        /// </summary>
        /// <param name="table"></param>
        public void SelectRow(string table)
        {
            #region variables 

            int _rowChoice;
            int _totalRows;

            bool _rowContinue;
            bool found = false;

            string readableData;

            DataSet data = GrabData(table);

            List<FileData> _rowData = new List<FileData>();
            List<ExtensionInfo> _extInfo = new List<ExtensionInfo>();

            #endregion

            if (table == "ExtensionInfo")
            {

                foreach (DataRow row in data.Tables[0].Rows)
                {
                    _extInfo.Add(new ExtensionInfo()
                    {
                        Id = (Int64)row[0],
                        Extension = (string)row[1]
                    });
                }
            }
            else
            {
                foreach (DataRow row in data.Tables[0].Rows)
                {
                    _rowData.Add(new FileData()
                    {
                        Id = (Int64)row[0],
                        Type = (string)row[1],
                        Data = (Byte[])row[2],
                        DateCreated = Convert.ToDateTime(row[3]),
                        DateUpdated = Convert.ToDateTime(row[4]),
                        ExtensionId = (Int64)row[5]
                    });
                }
            }

            do
            {
                _rowContinue = false;
                _totalRows = _rowData.Count;

                string _parseStr = PrintTable(_rowData, "To view the data of a row select the Id. Press 0 to exit: ");
                if (!int.TryParse(_parseStr, out _rowChoice)) { _rowChoice = -1; }

                int consoleWidth = Console.WindowWidth;
                int consoleHeight = Console.WindowHeight;

                if (_rowChoice == 0)
                {
                    return;
                }

                try
                {
                    foreach (FileData item in _rowData)
                    {
                        if (item.Id == _rowChoice)
                        {
                            readableData = (item.ExtensionId == 1) ? Encoding.ASCII.GetString(item.Data) : "This data is not in a readable format";

                            found = true;
                            Console.Clear();
                            Console.WriteLine($"\n\n\n\n\nSelected {item.Type}\n\n");
                            Console.WriteLine($"ID: {item.Id}\n");
                            Console.WriteLine($"Type: {item.Type}\n");
                            Console.WriteLine($"Data: \n{readableData}\n");
                            Console.WriteLine($"ExtensionId: {item.ExtensionId}\n");
                            Console.WriteLine($"Date Created: {item.DateCreated}\n");
                            Console.WriteLine($"Date Updated: {item.DateUpdated}\n");
                            Console.WriteLine("\nPress Any Key To Continue...");
                            Console.ReadKey();
                            return;
                        }
                        Serialize(item, table);
                    }

                    if (!found)
                    {
                        if (_rowChoice > _totalRows)
                        {
                            _rowContinue = true;

                            Console.Clear();
                            Console.WriteLine("Please Enter a Valid Option");
                            Console.WriteLine("Press Any Key To Continue...");
                            Console.ReadKey();
                        }
                    }

                }
                catch (Exception ex)
                {
                    _rowContinue = true;
                    Console.Clear();
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press Any Key To Continue...");
                    Console.ReadKey();
                }

            } while (_rowContinue);
        }

        /// <summary>
        /// Manually process rows and move them over to Processed folder
        /// </summary>
        /// <param name="PrimaryTable"></param>
        public void ManuallyProcess_Testing(string PrimaryTable)
        {
            DataSet data = new DataSet();
            FileData _fileData = new FileData();
            List<FileData> rows = new List<FileData>();
            List<int> rowIDs = new List<int>();

            data = GrabData(PrimaryTable);

            if (PrimaryTable == "ExtensionInfo") { return; }

            foreach (DataRow row in data.Tables[0].Rows)
            {
                rows.Add(new FileData
                {
                    Id = (long)row[0],
                    Type = (string)row[1],
                    Data = (byte[])row[2],
                    DateCreated = Convert.ToDateTime(row[3]),
                    DateUpdated = Convert.ToDateTime(row[4]),
                    ExtensionId = (long)row[5]
                });

                rowIDs.Add(Convert.ToInt32(row[0]));
            }

            do
            {
                string _parseStr = PrintTable(rows, "Select the Row to move. Press 0 to exit: ");
                int _rowSelected = int.Parse(_parseStr);

                if (_rowSelected == 0) { return; }

                if (!rowIDs.Contains(_rowSelected)) { _rowSelected = -1; }

                if (_rowSelected != -1)
                {
                    //transfer SQLite row data
                    int _rowFound = rowIDs.BinarySearch(_rowSelected);
                    _fileData = rows[_rowFound];
                    InsertRow("ProcessedTx", _fileData);
                    rows = DeleteRow(PrimaryTable, _rowSelected, rows);
                    rowIDs.RemoveAt(_rowFound);

                    //transfer file data 
                    TransferData(PrimaryTable, _rowSelected);
                }


            } while (true);
        }

        /// <summary>
        /// This function moves a selected file from the manual folder into the processed folder
        /// </summary>
        /// <param name="PrimaryTable"></param>
        /// <param name="RowId"></param>
        public void TransferData(string PrimaryTable, int RowId)
        {
            List<string> tables = new List<string>();
            List<FileData> rows = new List<FileData>();

            string source = _manual + $"{PrimaryTable}_ID_{RowId}_Tx.tranx";
            string destination = _processed + $"{PrimaryTable}_ID_{RowId}_Tx.tranx";

            try
            {
                File.Move(source, destination);
                File.Delete(source);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            Console.WriteLine("\n\n\t\t\t\tFile Transfer Successful.");
            Console.ReadKey();
        }

        /// <summary>
        /// Uses Deserialization to capture the desired file data
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public FileData GetFileData(string table)
        {
            int _IdChoice;

            string _path;

            FileData _fileData = new FileData();
            List<string> _files = Directory.EnumerateFiles(_manual).ToList();

            string _parseStr = Console.ReadLine();
            if (!int.TryParse(_parseStr, out _IdChoice)) { _IdChoice = -1; }

            foreach (string file in _files)
            {
                _path = _manual + $"{table}_ID_{_IdChoice}_Tx.tranx";

                if (file == _path)
                {
                    _fileData = Deserialize(file);
                }
            }

            return _fileData;
        }

        public void Sync()
        {
            int _index = 0;

            DataSet dataSet = new DataSet();

            List<FileData> _dataFromDB = new List<FileData>();
            List<FileData> _dataFromFile = new List<FileData>();

            List<string> _dirFiles = new List<string>();

            Dictionary<string, string> _tables = new Dictionary<string, string>
            {
                { "ManualTx", _manual },
                { "PendingTx", _pending },
                { "ProcessedTx", _processed }
            };

            foreach (KeyValuePair<string, string> table in _tables)
            {
                dataSet = GrabData(table.Key);

                //populate data from SQLite
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    _dataFromDB.Add(new FileData
                    {
                        Id = (long)row[0],
                        Type = (string)row[1],
                        Data = (byte[])row[2],
                        DateCreated = Convert.ToDateTime(row[3]),
                        DateUpdated = Convert.ToDateTime(row[4]),
                        ExtensionId = (long)row[5]
                    });
                }

                FilePrep();
                _dirFiles = Directory.EnumerateFiles(table.Value).ToList();

                //Deserialize data from Folder
                foreach (string _file in _dirFiles)
                {
                    _dataFromFile.Add(Deserialize(_file));
                }

                foreach (FileData item in _dataFromFile)
                {
                    if(!_dataFromDB.Contains(item, new FileDataComparer()))
                    {
                        File.Delete(_dirFiles[_index]);
                        _dataFromFile.RemoveAt(_index);
                    }

                    _index++;
                }

                foreach(FileData item in _dataFromDB)
                {
                    if(!_dataFromFile.Contains(item, new FileDataComparer()))
                    {
                        Serialize(item, table.Key);
                    }
                }

                
                Console.WriteLine(table.Key);

            }
        }

        #region Support Functions

        //
        //support functions
        //

        private string DateTimeSQlite(DateTime date)
        {
            string _dateFormat = "{0}-{1}-{2} {3}:{4}:{5}.{6}";

            string _newDate = string.Format(_dateFormat, date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond);

            return _newDate;
        }

        private string FormatList(List<string> fields, FormatType CommandType)
        {
            List<string> temp = new List<string>();
            temp = fields.ToList();
            string formatedCommand = "";

            switch (CommandType)
            {
                case FormatType.All:
                    formatedCommand += temp.First();
                    temp.RemoveAt(0);
                    foreach (string entries in temp) { formatedCommand += ", " + entries; }
                    return formatedCommand;
                case FormatType.InsertFormat:
                    formatedCommand += temp.First().Split(' ').First();
                    temp.RemoveAt(0);
                    foreach (string item in temp) { formatedCommand += ", " + item.Split(' ').First(); }
                    return formatedCommand;
                case FormatType.None:
                    return formatedCommand;
                default:
                    Console.WriteLine("Could not format list");
                    return "Command Failed";
            }
        }

        private List<string> GetFeilds(List<string> columns)
        {
            List<string> returnList = new List<string>();
            foreach (string column in columns) { returnList.Add(column.Substring(0, column.IndexOf(" ", 0))); }
            return returnList;
        }

        public List<string> GetTables()
        {
            string _path;

            List<string> _retValue = new List<string>();
            DataTable tableData = FileConnection.GetSchema("Tables");

            foreach (DataRow row in tableData.Rows)
            {
                _path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data\");
                _path += $@"{row[2].ToString()}\";

                _retValue.Add(row[2].ToString());

                if (!Directory.Exists(_path)) { Directory.CreateDirectory(_path); }
            }

            return _retValue;
        }

        public Dictionary<long, string> GetExtensionInfo()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// insert command that allows for one insert at a time.
        /// </summary>
        /// <param name="Table"></param>
        /// <param name="RowId"></param>
        /// <param name="Collection"></param>
        private void InsertRow(string Table, FileData data)
        {
            string _command = $"INSERT INTO {Table} (Type, Data, ExtensionId, DateCreated, DateUpdated) VALUES (@Type, @Data, @ExtensionId, @DateCreated, @DateUpdated);";
            int rowsAffected;

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter(@"Type", data.Type),
                new SQLiteParameter(@"Data", data.Data),
                new SQLiteParameter(@"ExtensionId", data.ExtensionId),
                new SQLiteParameter(@"DateCreated", data.DateCreated),
                new SQLiteParameter(@"DateUpdated", data.DateUpdated)
            };


            using (SQLiteCommand insertCommand = new SQLiteCommand(_command, FileConnection))
            {
                insertCommand.Parameters.AddRange(parameters);

                rowsAffected = insertCommand.ExecuteNonQuery();

                Console.WriteLine($"\n\n\t\t\t\tCommand Executed Successfully. {rowsAffected} row(s) affected.");
                Console.ReadKey();
            }
        }

        private List<FileData> DeleteRow(string Table, int RowID, List<FileData> _rowData)
        {
            int _rowsAffected;
            int removeIndex = 0;

            foreach (FileData item in _rowData)
            {
                if (item.Id == RowID) { break; }

                removeIndex++;
            }

            string _deleteCmd = $"DELETE FROM {Table} WHERE Id = ";

            _deleteCmd += $"{RowID};";
            using (SQLiteCommand _deleteRow = new SQLiteCommand(_deleteCmd, FileConnection))
            {
                _rowsAffected = _deleteRow.ExecuteNonQuery();
                _rowData.RemoveAt(removeIndex);
                Console.WriteLine($"Executing Command: {_rowsAffected} row(s) effected.");
                Console.WriteLine("Press Any Key To Continue...");
                Console.ReadKey();

                return _rowData;
            }
        }

        private void FilePrep()
        {
            if (!Directory.Exists(_manual)) { Directory.CreateDirectory(_manual); }

            if (!Directory.Exists(_pending)) { Directory.CreateDirectory(_pending); }

            if (!Directory.Exists(_processed)) { Directory.CreateDirectory(_processed); }

            if (!Directory.Exists(_extensions)) { Directory.CreateDirectory(_extensions); }
        }
        #endregion

        #region Enums

        //
        //enums
        //

        public enum FormatType
        {
            All,
            InsertFormat,
            None
        }

        #endregion
    }



    class FileDataComparer : IEqualityComparer<FileData>
    {
        public bool Equals(FileData Data1, FileData Data2)
        {
            if (Data1 == null && Data2 == null) { return true; }
            else if (Data1 == null || Data2 == null) { return false; }
            else if
                (
                    Data1.Id == Data2.Id &&
                    Data1.Type == Data2.Type &&
                    Data1.DateCreated.Year == Data2.DateCreated.Year &&
                    Data1.DateCreated.Month == Data2.DateCreated.Month &&
                    Data1.DateCreated.Day == Data2.DateCreated.Day &&
                    Data1.DateCreated.Hour == Data2.DateCreated.Hour &&
                    Data1.DateCreated.Minute == Data2.DateCreated.Minute &&
                    Data1.DateCreated.Second == Data2.DateCreated.Second &&
                    Data1.DateCreated.Millisecond == Data2.DateCreated.Millisecond &&
                    Data1.DateUpdated.Year == Data2.DateUpdated.Year &&
                    Data1.DateUpdated.Month == Data2.DateUpdated.Month &&
                    Data1.DateUpdated.Day == Data2.DateUpdated.Day &&
                    Data1.DateUpdated.Hour == Data2.DateUpdated.Hour &&
                    Data1.DateUpdated.Minute == Data2.DateUpdated.Minute &&
                    Data1.DateUpdated.Second == Data2.DateUpdated.Second &&
                    Data1.DateUpdated.Millisecond == Data2.DateUpdated.Millisecond
                )
                {
                    if (Data1.ExtensionId == 1)
                    {
                        if (Encoding.ASCII.GetString(Data1.Data) == Encoding.ASCII.GetString(Data2.Data)) { return true; }
                        else { return false; }
                    }
                    else
                    {
                        if (Data1.Data.Length == Data2.Data.Length) { return true; }
                        else { return false; }
                    }
                }    
            else { return false; }
        }


        public int GetHashCode(FileData Data)
        {
            int _hashCode = Data.DateUpdated.Second ^ Data.DateCreated.Second ^ Data.DateCreated.Minute;
            return _hashCode.GetHashCode();
        }
    }
}
