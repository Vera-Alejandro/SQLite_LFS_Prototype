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

        private readonly string _main = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data\");

        private readonly string _manual = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data\Manual\");

        private readonly string _pending = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data\Pending\");

        private readonly string _processed = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data\Processed\");

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
            if(File.Exists(_sqlitePath) && FileConnection.State != ConnectionState.Open)
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
            if(File.Exists(_sqlitePath) && FileConnection.State != ConnectionState.Closed)
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
                            ");" +
                            "CREATE TABLE IF NOT EXISTS FileData (" +
                                "Id INTEGER PRIMARY KEY NOT NULL," +
                                "Name TEXT," +
                                "Data TEXT," +
                                "ExtensionId INTEGER," +
                                "FOREIGN KEY (ExtensionId) REFERENCES ExtensionInfo(Id)" +
                            ");";
            #endregion

            #region SQLite Insert Command
            string insertCommand = "INSERT INTO ExtensionInfo (Extension) VALUES ('txt'), ('exe'), ('pdf'), ('jpg'), ('cs'), ('c'), ('cpp');" +
                            "INSERT INTO FileData (Name, Data, ExtensionId) VALUES ('Battleship', 'attack(); Play(); funcion();', 5), ('GreatGraph', 'tree stuff if(not tree == do nothing', 7), ('GameFinal', 'this is my csc250 Final', 6), ('Security Report', 'you cant do anything about it youre going to get hacked lol', 1), ('memegato', '*here lies funny cat meme*', 4), ('Scholarship Reward', 'you just got a full ride for being really really cool', 3);";
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

            Console.Write("\n\t\t\t\tEnter the Name of the new entry.\n\t\t\t\t");
            data.Type = Console.ReadLine();
            Console.WriteLine();

            Console.Write("\t\t\t\tEnter the Data for this new entry\n\t\t\t\t");
            data.Data = Encoding.ASCII.GetBytes(Console.ReadLine());
            Console.WriteLine();

            Console.Write("\t\t\t\tEnter the Extensino ID for this new entry\n\t\t\t\t");
            data.DateCreated = Console.ReadLine();
            Console.WriteLine();

            Console.Write("\t\t\t\tEnter the Extensino ID for this new entry\n\t\t\t\t");
            data.DateUpdated = Console.ReadLine();
            Console.WriteLine();

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
                if(n % 3 == 0 && n > 1)
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
        public void Insert(RowData rowData, string table)
        {
            string _command = $@"INSERT INTO {table} (Name, Data, Extension) VALUES (@Name, @Data, @Extension); SELECT last_insert_rowid();";

            DatabaseProfile profile = new DatabaseProfile("sqlite", constr, "SQLite3");

            rowData.Id = FileConnection.Query<int>(_command, rowData).First();
        }

        /// <summary>
        /// Select Row Data using Dapper
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public RowData SelectData(int Id, string table)
        {
            string _command = $@"SELECT Id, Name, Data, ExtensionId FROM {table} WHERE Id = @id";
            RowData _retValue = FileConnection.Query<RowData>(_command, new { Id }).FirstOrDefault();

            return _retValue;
        }

        #endregion

        /// <summary>
        /// Prints formated table given list of row data
        /// </summary>
        /// <param name="data"></param>
        void PrintTable(List<FileData> data)
        {
            int dataTabs = 8;
            int nameTabs = 1;
            int valuesDataTab;
            int valuesNameTab;

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

            Console.WriteLine("ExtensionId");

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

                Console.WriteLine(value.ExtensionId);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Drops selected table 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public string DropTable (string table)
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
        public RowData Deserialize(string path)
        {
            XmlSerializer reader = new XmlSerializer(typeof(RowData));

            StreamReader _readTx = new StreamReader(path);

            RowData _returnValue = (RowData)reader.Deserialize(_readTx);

            _readTx.Close();

            return _returnValue;
        }

        /// <summary>
        /// Converts objects into an XML file for later storage
        /// </summary>
        /// <param name="data"></param>
        /// <param name="table"></param>
        public void Serialize(RowData data, string table)
        {
            string _path;

            #region File Exist & Create Check

            if(!Directory.Exists(_main)) { Directory.CreateDirectory(_main); }

            if (!Directory.Exists(_manual)) { Directory.CreateDirectory(_manual); }

            if (!Directory.Exists(_pending)) { Directory.CreateDirectory(_pending); }

            if(!Directory.Exists(_processed)) { Directory.CreateDirectory(_processed); }

            #endregion

            XmlSerializer writer = new XmlSerializer(typeof(RowData));

            _path = _manual + $"{table}_ID_{data.Id}_Tx.tranx";

            if (File.Exists(_path))
            {
                File.Delete(_path);
            }

            FileStream _newTransaction = _newTransaction = File.Create(_path);
            
            writer.Serialize(_newTransaction, data);

            _newTransaction.Close();
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

            List<RowData> _rowData = new List<RowData>();
            List<ExtensionInfo> _extInfo = new List<ExtensionInfo>();
            List<int> _IDs = new List<int>();

            #endregion

            //sets the data to the correst List
            if (table == "ExtensionInfo")
            {
                foreach(DataRow row in data.Tables[0].Rows)
                {
                    _extInfo.Add(new ExtensionInfo()
                    {
                        Id = (Int64)row[0],
                        Extension = (string)row[1]
                    });

                    _IDs.Add((int)row[0]);
                }
            }

            foreach (DataRow row in data.Tables[0].Rows)
            {
                _rowData.Add(new RowData()
                {
                    Id = (Int64)row[0],
                    Name = (string)row[1],
                    Data = (string)row[2],
                    ExtensionId = (Int64)row[3]
                });

                _IDs.Add((int)row[0]);
            }

            do
            {
                string _deleteCmd = $"DELETE FROM {table} WHERE Id = ";

                //PrintTable(_rowData);

                Console.WriteLine("\n\nWhat Row would you like to remove? Press 0 to exit.");
                if (!int.TryParse(Console.ReadLine(), out _rowChoice)) { _rowChoice = -1; }

                if (_rowChoice == 0)
                {
                    _rowContinue = false;
                    break;
                }

                try
                {
                    if (_IDs.Contains(_rowChoice))
                    {
                        _rowContinue = true;
                        Console.Clear();
                        Console.WriteLine("Please Enter a Valid Option");
                        Console.WriteLine("Press Any Key To Continue...");
                        Console.ReadKey();
                        break;
                    }

                    _deleteCmd += $"{_rowChoice};";
                    using (SQLiteCommand _deleteRow = new SQLiteCommand(_deleteCmd, FileConnection))
                    {
                        _rowsAffected = _deleteRow.ExecuteNonQuery();
                        _rowData.RemoveAt(_rowChoice - 1);
                        Console.WriteLine($"Executing Command: {_rowsAffected} row(s) effected.");
                        Console.WriteLine("Press Any Key To Continue...");
                        Console.ReadKey();
                    }
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

            DataSet data = GrabData(table);

            List<RowData> _rowData = new List<RowData>();
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
                    _rowData.Add(new RowData()
                    {
                        Id = (Int64)row[0],
                        Name = (string)row[1],
                        Data = (string)row[2],
                        ExtensionId = (Int64)row[3]
                    });
                }
            }

            do
            {
                _rowContinue = false;
                _totalRows = _rowData.Count;
                //PrintTable(_rowData);

                Console.WriteLine("\n\nTo view the data of a row select the Id. Press 0 to exit");
                if (!int.TryParse(Console.ReadLine(), out _rowChoice)) { _rowChoice = -1; }

                if (_rowChoice == 0)
                {
                    return;
                }

                try
                {
                    foreach (RowData item in _rowData)
                    {
                        if (item.Id == _rowChoice)
                        {
                            found = true;
                            Console.Clear();
                            Console.WriteLine($"\n\n\n\n\nSelected {item.Name}\n\n");
                            Console.WriteLine($"ID: {item.Id}\n");
                            Console.WriteLine($"Name: {item.Name}\n");
                            Console.WriteLine($"Data: \n{item.Data}\n");
                            Console.WriteLine($"ExtensionId: {item.ExtensionId}\n");
                            Console.WriteLine("\nPress Any Key To Continue...");
                            Console.ReadKey();
                            return;
                        }
                        Serialize(item, table);
                    }
                    
                    if(!found)
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
            string SecondaryTable;

            DataSet data = new DataSet();
            FileData _fileData = new FileData();
            List<FileData> rows = new List<FileData>();
            List<string> tables = new List<string>();
            List<int> rowIds = new List<int>();

            data = GrabData(PrimaryTable);
            tables = GetTables();

            /*

            foreach (RowData item in data.Tables[0].Rows)
            {
                rows.Add(new RowData()
                {
                    Id = (Int64)item.ItemArray[0],
                    Name = (string)item.ItemArray[1],
                    Data = (string)item.ItemArray[2],
                    ExtensionId = (Int64)item.ItemArray[3]
                });

                rowIds.Add(Convert.ToInt32(item.ItemArray[0]));
            }


    */

            do
            {
                #region Secondary Menu

                #region Menu Variables
                int _dashCount;
                int _option = 0;

                string _nameMenu;

                #endregion

                PrintTable(rows);

                Console.WriteLine("\nSelect the Row to move: ");
                if (!int.TryParse(Console.ReadLine(), out int _rowSelected)) { _rowSelected = -1; }

                if(!rowIds.Contains(_rowSelected)) { _rowSelected = -1; }


                //transfer file data 
                TransferData(PrimaryTable, _rowSelected);

                //transfer SQLite row data
                int _rowFound = rowIds.BinarySearch(_rowSelected);
                _fileData = rows[_rowFound];






                //do we need another menu? 
                // if we are just processing everything in the manual to processed then 
                //nothing else should be needed no direction just a set process

                #region Potential Removal

                Console.Clear();
                Console.WriteLine("\n\n\n\n\n" +
                            "\t\t\t\t____________Manually Process Menu_____________\n" +
                            "\t\t\t\t|--------------------------------------------|\n" +
                            "\t\t\t\t|--------------------------------------------|");
                #region Dash Loop

                foreach (string table in tables)
                {
                    if (table == PrimaryTable)
                    {
                        _nameMenu = $"\t\t\t\t|---Primary--{_option + 1} - {table}";
                    }
                    else
                    {
                        _nameMenu = $"\t\t\t\t|------------{_option + 1} - {table}";
                    }

                    Console.Write(_nameMenu);
                    _dashCount = (49 - _nameMenu.Length);
                    for (int i = 0; i < _dashCount; i++) { Console.Write("-"); }
                    Console.WriteLine("|");
                    _option++;
                }

                #endregion
                Console.Write("\t\t\t\t|--------------------------------------------|\n" +
                            "\t\t\t\t|------------0 - Back------------------------|\n" +
                            "\t\t\t\t|--------------------------------------------|\n" +
                            "\t\t\t\t|____________________________________________|\n" +
                            "\t\t\t\tSelect the Secondary Table: ");

                #endregion

                if (!int.TryParse(Console.ReadLine(), out int _secondaryChoice)) { _secondaryChoice = -1; }

                SecondaryTable = tables[_secondaryChoice - 1];

                #endregion

                try
                {
                    if (tables[_secondaryChoice - 1] != PrimaryTable)
                    {
                        DataSet _Unfilte = GrabData(PrimaryTable);

                        TransferData(PrimaryTable, _rowSelected);

                        SQLiteCommand InsertCommand;
                        //InsertCommand.Parameters.Add();

                        Console.ReadKey();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Press Any Key to Continue...");
                    Console.ReadKey();
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
            List<RowData> rows = new List<RowData>();

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
        }

        /// <summary>
        /// Uses Deserialization to capture the desired file data
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public RowData GetFileData(string table)
        {
            int _IdChoice;

            string _path;

            RowData _fileData = new RowData();
            List<string>  _files = Directory.EnumerateFiles(_manual).ToList();

            if(!int.TryParse(Console.ReadLine(), out _IdChoice)) { _IdChoice = -1; }

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
            string _path = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data\");

            List<string> _retValue = new List<string>();
            DataTable tableData = FileConnection.GetSchema("Tables");

            foreach (DataRow row in tableData.Rows)
            {
                _path += $@"{row[2].ToString()}\";

                _retValue.Add(row[2].ToString());

                if (!Directory.Exists(_path)) { Directory.CreateDirectory(_path); }
            }

            return _retValue;
        }

        /// <summary>
        /// insert command that allows for one insert at a time.
        /// </summary>
        /// <param name="Table"></param>
        /// <param name="RowId"></param>
        /// <param name="Collection"></param>
        private void InsertRow(string Table, FileData data)
        {
            string _command = $"INSERT INTO {Table} (Type, Data, DateCreated, DateUpdated) VALUES (@Type, @Data, @DateCreated, @DateUpdated);";
            int rowsAffected;

            SQLiteParameter[] parameters = 
            {
                new SQLiteParameter(@"Type", data.Type),
                new SQLiteParameter(@"Data", data.Data),
                new SQLiteParameter(@"DateCreated", data.DateCreated),
                new SQLiteParameter(@"DateUpdated", data.DateUpdated)
            };


            using (SQLiteCommand insertCommand = new SQLiteCommand(_command, FileConnection))
            {
                insertCommand.Parameters.AddRange(parameters);

                rowsAffected = insertCommand.ExecuteNonQuery();

                Console.WriteLine($"Command Executed Successfully. {rowsAffected} row(s) affected.");
                Console.ReadKey();
            }
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
}
