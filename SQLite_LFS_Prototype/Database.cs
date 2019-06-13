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
using SQLite_LFS_Prototype.Comparer;
using System.Text;

namespace SQLite_LFS_Prototype
{
    public class Database
    {
        private SQLiteConnection FileConnection { get; set; }
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
        }

        //
        //Functions
        //

        /// <summary>
        /// Creates a connection to the SQLite DB
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Safely closes connection to SQLite DB
        /// </summary>
        /// <returns></returns>
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

            //string insertCommand = "INSERT INTO ExtensionInfo (Extension) VALUES ('txt'), ('exe'), ('pdf'), ('jpg'), ('cs'), ('c'), ('cpp');";

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
        /// imports folder of XML files
        /// </summary>
        /// <param name="FileLocation"></param>
        public void Import(string FileLocation)
        {
            List<string> _files = new List<string>();
            List<ExtensionInfo> _ext = new List<ExtensionInfo>();
            List<FileData> _fData = new List<FileData>();

            string tmp;

            int rowsAffected = 0;

            _files = Directory.EnumerateFiles(FileLocation).ToList();

            foreach (string file in _files)
            {
                tmp = file.Remove(0, FileLocation.Length);
                tmp = file.Remove(0, 12);

                if (tmp == "ManualTx_ID_")
                {
                    InsertRow("ManualTx", DeserializeFile(file));
                }
                if (tmp == "PendingTx_ID") 
                {
                    InsertRow("PendingTx", DeserializeFile(file));
                }
                if (tmp == "ProcessedTx_") 
                {
                    InsertRow("ProcessedTx", DeserializeFile(file));
                }
                if (tmp == "ExtensionInf")
                {
                    InsertRow("ExtensionInfo", DeserializeExtension(file));
                }

                rowsAffected++;
            }

            Console.WriteLine($"Import Successfull. {rowsAffected} row(s) affected.");
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

        /// <summary>
        /// Prints formated table given list of row data
        /// </summary>
        /// <param name="data"></param>
        public string PrintTable(List<FileData> data, string CommandAfter)
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
                    if (row.Data.Length > dataTabs) { dataTabs = row.Data.Length; }
                }

                if (row.Type.Length > nameTabs) { nameTabs = row.Type.Length; }
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

            for (int n = 0; n < nameTabs; n++) { Console.Write("\t"); }

            Console.Write("Data");

            for (int i = 0; i < dataTabs; i++) { Console.Write("\t"); }

            Console.WriteLine("Extension ID\tDate Created\t\t\tDate Updated");

            //Printing Table Data
            foreach (FileData value in data)
            {
                if (value.ExtensionId == 1) { readableData = Encoding.ASCII.GetString(value.Data); }
                else { readableData = "This data is not in a readable format"; }

                valuesDataTab = (value.ExtensionId == 1) ? (value.Data.Length > 60) ? 1 : dataTabs - (value.Data.Length / 8) : dataTabs - (readableData.Length / 8);
                valuesNameTab = nameTabs - (value.Type.Length / 8);

                Console.Write($"{value.Id.ToString()}\t{value.Type}");

                for (int n = 0; n < valuesNameTab; n++) { Console.Write("\t"); }

                if (readableData.Length > 60) { Console.Write($"{readableData.Remove(60)}..."); }
                else { Console.Write(readableData); }

                for (int k = 0; k < valuesDataTab; k++) { Console.Write("\t"); }

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
        /// Prints formated table of ExtensionInfo
        /// </summary>
        /// <param name="data"></param>
        /// <param name="CommandAfter"></param>
        /// <returns></returns>
        public string PrintTable(List<ExtensionInfo> data, string CommandAfter)
        {
            string _retString;

            //Printing Titles
            Console.Clear();

            Console.WriteLine("ID\tExtension");

            //Printing Table Data
            foreach (ExtensionInfo value in data) { Console.WriteLine($"{value.Id.ToString()}\t{value.Extension}"); }

            Console.Write($"\n\n{CommandAfter}");
            _retString = Console.ReadLine();

            Console.Clear();

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
        /// Converts data from XML file into a FileData Object given the file path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public FileData DeserializeFile(string path)
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
        /// Converts data from XML file into a ExtensionInfo Object given the file path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ExtensionInfo DeserializeExtension(string path)
        {
            ExtensionInfo _returnValue;

            XmlSerializer reader = new XmlSerializer(typeof(ExtensionInfo));

            using (Stream _readTx = new FileStream(path, FileMode.Open))
            {
                _returnValue = (ExtensionInfo)reader.Deserialize(_readTx);
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
                if (File.Exists(_path)) { File.Delete(_path); }

                using (FileStream _newTransaction = File.Create(_path))
                {
                    writer.Serialize(_newTransaction, data);

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Converts objects into an XML file for later storage
        /// </summary>
        /// <param name="data"></param>
        /// <param name="table"></param>
        public bool Serialize(ExtensionInfo data, string table)
        {
            string _path = "";

            XmlSerializer writer = new XmlSerializer(typeof(ExtensionInfo));

            if (table == "ManualTx") { _path = _manual + $"{table}_ID_{data.Id}_Tx.tranx"; }

            if (table == "PendingTx") { _path = _pending + $"{table}_ID_{data.Id}_Tx.tranx"; }

            if (table == "ProcessedTx") { _path = _processed + $"{table}_ID_{data.Id}_Tx.tranx"; }

            if (table == "ExtensionInfo") { _path = _extensions + $"{table}_ID_{data.Id}_Tx.tranx"; }

            if (_path == "") { return false; }

            try
            {
                if (File.Exists(_path)) { File.Delete(_path); }

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

            bool _rowContinue = true;

            List<int> _IDs = new List<int>();

            #endregion

            //sets the data to the correst List
            if (table == "ExtensionInfo")
            {
                List<ExtensionInfo> _extInfo = new List<ExtensionInfo>();

                _extInfo = GetExtensionInfo();

                do
                {
                    string _parseStr = PrintTable(_extInfo, "What Row would you like to remove? Press 0 to exit: ");
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

                        _extInfo = DeleteRow(table, _rowChoice, _extInfo);
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
            else
            {
                List<FileData> _rowData = new List<FileData>();

                _rowData = GetFileData(table);

                _IDs = GetIDs(_rowData);

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

            bool found = false;
            bool _rowContinue;

            string readableData;

            List<int> IDs = new List<int>();

            #endregion

            if (table == "ExtensionInfo")
            {
                List<ExtensionInfo> _extInfo = new List<ExtensionInfo>();
                
                _extInfo = GetExtensionInfo();
                IDs = GetIDs(_extInfo);
                do
                {
                    _rowContinue = false;

                    string _parseStr = PrintTable(_extInfo, "To view the data of a row select the Id. Press 0 to exit: ");
                    if (!int.TryParse(_parseStr, out _rowChoice)) { _rowChoice = -1; }

                    if (_rowChoice == 0) { return; }

                    try
                    {
                        if (!IDs.Contains(_rowChoice))
                        {
                            _rowContinue = true;

                            Console.Clear();
                            Console.WriteLine("Please Enter a Valid Option");
                            Console.WriteLine("Press Any Key To Continue...");
                            Console.ReadKey();

                            break;
                        }

                        foreach (ExtensionInfo item in _extInfo)
                        {
                            if (item.Id == _rowChoice)
                            {
                                found = true;

                                Console.Clear();
                                Console.WriteLine($"\n\n\n\n\nSelected {item.Extension}\n\n");
                                Console.WriteLine($"ID: {item.Id}\n");
                                Console.WriteLine($"ExtensionId: {item.Extension}\n");
                                Console.WriteLine("\nPress Any Key To Continue...");
                                Console.ReadKey();

                                return;
                            }

                            Serialize(item, table);
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
            else
            {
                List<FileData> _rowData = new List<FileData>();

                _rowData = GetFileData(table);
                IDs = GetIDs(_rowData);

                do
                {
                    _rowContinue = false;

                    string _parseStr = PrintTable(_rowData, "To view the data of a row select the Id. Press 0 to exit: ");
                    if (!int.TryParse(_parseStr, out _rowChoice)) { _rowChoice = -1; }

                    int consoleWidth = Console.WindowWidth;
                    int consoleHeight = Console.WindowHeight;

                    if (_rowChoice == 0) { return; }

                    try
                    {
                        if(!IDs.Contains(_rowChoice))
                        {
                            _rowContinue = true;

                            Console.Clear();
                            Console.WriteLine("Please Enter a Valid Option");
                            Console.WriteLine("Press Any Key To Continue...");
                            Console.ReadKey();

                            break;
                        }

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
        }

        /// <summary>
        /// Manually process rows and move them over to Processed folder
        /// </summary>
        /// <param name="PrimaryTable"></param>
        public void ManuallyProcess()
        {
            string _table = "ManualTx";
            DataSet data = new DataSet();

            FileData _fileData = new FileData();

            List<FileData> rows = new List<FileData>();

            List<int> rowIDs = new List<int>();

            rows = GetFileData(_table);
            rowIDs = GetIDs(rows);

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
                    rows = DeleteRow(_table, _rowSelected, rows);
                    rowIDs.RemoveAt(_rowFound);

                    //transfer file data 
                    TransferData(_table, _rowSelected);
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
        public FileData DeserializeFileData(string table)
        {
            int _IdChoice;

            string _dirPath = "";
            string _path;

            FileData _fileData = new FileData();

            #region _path Declaration

            if (table == "ManualTx") { _dirPath = _manual; }

            if (table == "PendingTx") { _dirPath = _pending; }

            if (table == "ProcessedTx") { _dirPath = _processed; }

            if (table == "Extensioninfo") { _dirPath = _extensions; }

            if (_dirPath == "") { return null; }

            #endregion

            List<string> _files = Directory.EnumerateFiles(_dirPath).ToList();

            PrintFileName();
            string _parseStr = Console.ReadLine();
            if (!int.TryParse(_parseStr, out _IdChoice)) { _IdChoice = -1; }

            foreach (string file in _files)
            {
                _path = _dirPath + $"{table}_ID_{_IdChoice}_Tx.tranx";

                if (file == _path)
                {
                    _fileData = DeserializeFile(file);
                }
            }

            return _fileData;

            //
            //Sub-Method
            //

            void PrintFileName()
            {
                int index = 1;

                Console.WriteLine("ID\tFileName");

                foreach (string file in _files)
                {
                    Console.WriteLine($"{index}\t{file.Remove(0, _dirPath.Length)}");
                    index++;
                }

                Console.Write("What file would you like to Deserialize: ");
            }
        }

        public ExtensionInfo SelectDeserialization (string table)
        {
            int _IdChoice;

            string _path = "";

            #region _path Declaration

            if (table == "ManualTx") { _path = _manual; }

            if (table == "PendingTx") { _path = _pending; }

            if (table == "ProcessedTx") { _path = _processed; }

            if (table == "Extensioninfo") { _path = _extensions; }

            if (_path == "") { return null; }

            #endregion

            ExtensionInfo _fileData = new ExtensionInfo();
            List<string> _files = Directory.EnumerateFiles(_manual).ToList();

            string _parseStr = Console.ReadLine();
            if (!int.TryParse(_parseStr, out _IdChoice)) { _IdChoice = -1; }

            foreach (string file in _files)
            {
                _path = _manual + $"{table}_ID_{_IdChoice}_Tx.tranx";

                if (file == _path)
                {
                    _fileData = DeserializeExtension(file);
                }
            }

            return _fileData;
        }

        public void Sync()
        {
            int _index;

            DataSet dataSet = new DataSet();

            List<FileData> _dataFromDB = new List<FileData>();
            List<ExtensionInfo> _extDataFromDB = new List<ExtensionInfo>();

            List<FileData> _dataFromFile = new List<FileData>();
            List<ExtensionInfo> _extDataFromFile = new List<ExtensionInfo>();

            List<string> _dirFiles = new List<string>();

            Dictionary<string, string> _tables = new Dictionary<string, string>
            {
                { "ManualTx", _manual },
                { "PendingTx", _pending },
                { "ProcessedTx", _processed },
                { "ExtensionInfo", _extensions }
            };

            foreach (KeyValuePair<string, string> table in _tables)
            {
                _index = 0;
                _dataFromDB.Clear();
                _dataFromFile.Clear();

                if (table.Key != "ExtensionInfo")
                {
                    //populate data from SQLite
                    _dataFromDB = GetFileData(table.Key);

                    FilePrep();
                    _dirFiles = Directory.EnumerateFiles(table.Value).ToList();

                    //Deserialize data from Folder
                    foreach (string _file in _dirFiles)
                    {
                        _dataFromFile.Add(DeserializeFile(_file));
                    }

                    foreach (FileData item in _dataFromFile)
                    {
                        if (!_dataFromDB.Contains(item, new FileDataComparer()))
                        {
                            File.Delete(_dirFiles[_index]);
                            _dataFromFile.RemoveAt(_index);
                        }

                        _index++;
                    }

                    foreach (FileData item in _dataFromDB)
                    {
                        if (!_dataFromFile.Contains(item, new FileDataComparer()))
                        {
                            Serialize(item, table.Key);
                        }
                    }
                }
                else
                {
                    _extDataFromDB = GetExtensionInfo();

                    FilePrep();
                    _dirFiles = Directory.EnumerateFiles(table.Value).ToList();

                    //Deserialize data from Folder
                    foreach (string _file in _dirFiles)
                    {
                        _extDataFromFile.Add(DeserializeExtension(_file));
                    }

                    foreach (ExtensionInfo item in _extDataFromFile)
                    {
                        if (!_extDataFromDB.Contains(item, new ExtDataComparer()))
                        {
                            File.Delete(_dirFiles[_index]);
                            _extDataFromFile.RemoveAt(_index);
                        }

                        _index++;
                    }

                    foreach (ExtensionInfo item in _extDataFromDB)
                    {
                        if (!_extDataFromFile.Contains(item, new ExtDataComparer()))
                        {
                            Serialize(item, table.Key);
                        }
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

        public List<FileData> GetFileData (string Table)
        {
            DataSet rawdata = new DataSet();

            List<FileData> _filedata = new List<FileData>();

            rawdata = GrabData(Table);


            foreach (DataRow row in rawdata.Tables[0].Rows)
            {
                _filedata.Add(new FileData
                {
                    Id = (long)row[0],
                    Type = (string)row[1],
                    Data = (byte[])row[2],
                    DateCreated = Convert.ToDateTime(row[3]),
                    DateUpdated = Convert.ToDateTime(row[4]),
                    ExtensionId = (long)row[5]
                });
            }

            return _filedata;
        }

        public List<ExtensionInfo> GetExtensionInfo()
        {
            DataSet _rawdata = new DataSet();

            List<ExtensionInfo> extensionData = new List<ExtensionInfo>();

            _rawdata = GrabData("ExtensionInfo");

            foreach(DataRow row in _rawdata.Tables[0].Rows)
            {
                extensionData.Add(new ExtensionInfo
                {
                    Id = (long)row[0],
                    Extension = (string)row[1]
                });
            }

            return extensionData;
        }

        public List<int> GetIDs(List<FileData> Rows)
        {
            List<int> IDs = new List<int>();

            foreach (FileData item in Rows)
            {
                IDs.Add(Convert.ToInt32(item.Id));
            }

            return IDs;
        }

        public List<int> GetIDs(List<ExtensionInfo> Rows)
        {
            List<int> IDs = new List<int>();

            foreach (ExtensionInfo item in Rows)
            {
                IDs.Add(Convert.ToInt32(item.Id));
            }

            return IDs;
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

        private void InsertRow(string Table, ExtensionInfo data)
        {
            string _command = $"INSERT INTO {Table} (Extension) VALUES (@ExtensionId);";
            int rowsAffected;

            SQLiteParameter[] parameters =
            {
                new SQLiteParameter(@"Extension", data.Extension),
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

        private List<ExtensionInfo> DeleteRow(string Table, int RowID, List<ExtensionInfo> _rowData)
        {
            int _rowsAffected;
            int removeIndex = 0;

            foreach (ExtensionInfo item in _rowData)
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
    }
}
