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
using System.Reflection;

namespace SQLite_LFS_Prototype
{
    public class Database
    {
        private SQLiteConnection FileConnection { get; set; }
        private FormatType CommandAction { get; set; }
        private string constr { get; set; }

        private readonly string _filePath;

        //constructor
        public Database(string FilePath)
        {
            _filePath = FilePath;
            constr = $"Data Source={_filePath};Version=3;";


            if (!(File.Exists(_filePath)))
            {
                SQLiteConnection.CreateFile(_filePath);
                Console.WriteLine("SQLite File Created.");
            }
            
            FileConnection = new SQLiteConnection(constr);
            CommandAction = FormatType.None;
        }

        //Functions
        public bool Connect()
        {
            if(File.Exists(_filePath) && FileConnection.State != ConnectionState.Open)
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
            if(File.Exists(_filePath) && FileConnection.State != ConnectionState.Closed)
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

        //might not need this
        //manual create table command
        //this might just be used to create the Pending, Proccessed, and Manual tables if they don't exist
        public string CreateTable(string tableName, List<string> columns)
        {
            string command = $"CREATE TABLE IF NOT EXISTS {tableName} ({FormatList(columns, FormatType.All)})";

            try
            {
                using (SQLiteCommand newTable = new SQLiteCommand(command, FileConnection))
                {
                    newTable.ExecuteNonQuery();
                    Console.WriteLine($"Command Executed Successfully");
                    return command;
                }
            }
            catch (Exception)
            {
                return "Command Execution Failed";
            }

        }

        //default constructor of all the nessesary tables
        public string CreateTable()
        {
            #region SQLite Create Tables Command (Old)
            string command0 =
                "CREATE TABLE IF NOT EXISTS PendingTx (" +
                    "Id INTEGER PRIMARY KEY NOT NULL," +
                    "Type INT," + //Type (Enum) so should this be a foregin key 
                    "Data BLOB NOT NULL," +
                    "DateCreated TEXT," + //there is no date data type so text will be used instead (YYYY-MM-DD HH:MM:SS,SSS)
                    "DateUpdated TEXT" +
                ");" +
                "CREATE TABLE IF NOT EXISTS ProcessedTx (" +
                "Id INTEGER PRIMARY KEY NOT NULL," +
                    "Type INT," + //Type (Enum) so should this be a foregin key 
                    "Data BLOB NOT NULL," +
                    "DateCreated TEXT," + //there is no date data type so text will be used instead (YYYY-MM-DD HH:MM:SS,SSS)
                    "DateUpdated TEXT" +
                ");" +
                "CREATE TABLE IF NOT EXISTS ManualTx (" +
                "Id INTEGER PRIMARY KEY NOT NULL," +
                    "Type INT," + //Type (Enum) so should this be a foregin key 
                    "Data BLOB NOT NULL," +
                    "DateCreated TEXT," + //there is no date data type so text will be used instead (YYYY-MM-DD HH:MM:SS,SSS)
                    "DateUpdated TEXT" +
                ");";
            #endregion

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
            catch (Exception)
            {
                return "Command Failed!";
            }
        }

        public int InsertInto(string table, List<string> columns, List<string> values)
        {
            int retValue = 0;

            string command = $"INSERT INTO {table}({FormatList(columns, FormatType.InsertFormat)}) VALUES ({FormatList(values, FormatType.All)});";

            try
            {
                using (SQLiteCommand insertCommand = new SQLiteCommand(command, FileConnection))
                {
                    retValue = insertCommand.ExecuteNonQuery();
                    Console.WriteLine($"Executing Comman: {retValue} row(s) effected.");
                    return retValue;
                }
            }
            catch (Exception)
            {

                Console.WriteLine("Command Failed");
                return -2;
            }
        }

        public void InsertInto(string table)
        {
            string _insertCmd = $@"INSERT INTO {table} (Name, Data, ExtensionId) VALUES (@Name, @Data, @ExtensionId); SELECT last_insert_rowid();";
            string _name;
            string _data;
            Int64 _extId;
            int _rowsaffected;

            Console.Write("\n\t\t\t\tEnter the Name of the new entry.\n\t\t\t\t");
            _name = Console.ReadLine();
            Console.WriteLine();

            Console.Write("\t\t\t\tEnter the Data for this new entry\n\t\t\t\t");
            _data = Console.ReadLine();
            Console.WriteLine();

            Console.Write("\t\t\t\tEnter the Extensino ID for this new entry\n\t\t\t\t");
            _extId = Convert.ToInt64(Console.ReadLine());
            Console.WriteLine();
            
            
            try
            {
                using (SQLiteCommand Insert = new SQLiteCommand(_insertCmd, FileConnection))
                {
                    Insert.Parameters.Add(new SQLiteParameter("@Name", _name));
                    Insert.Parameters.Add(new SQLiteParameter("@Data", _data));
                    Insert.Parameters.Add(new SQLiteParameter("@ExtensionId", _extId));

                    _rowsaffected = Insert.ExecuteNonQuery();
                    Console.WriteLine($"\t\t\t\tCommand Executed Successfully: {_rowsaffected} row(s) affected.");
                    Console.ReadKey();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                throw;
            }
        }

        //this is using the reader
        public void SelectAll(string table, List<string> columns)
        {
            #region Variables
            List<int> _tabsNeeded = new List<int>();
            List<string> _tableData = new List<string>();
            List<string> _tempColumns = new List<string>();

            string command = $"SELECT * FROM {table};";

            using (SQLiteCommand selectAll = new SQLiteCommand(command, FileConnection))
            {
                SQLiteDataReader read = selectAll.ExecuteReader();
                _tempColumns = GetFeilds(columns).ToList();
                #endregion

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

        //to use this i think you need to populate the objects first and then have the query function apply that to the db
        public void TestInsert(RowData rowData, string table)
        {
            string _command = $@"INSERT INTO {table} (Name, Data, Extension) VALUES (@Name, @Data, @Extension); SELECT last_insert_rowid();";

            DatabaseProfile profile = new DatabaseProfile("sqlite", constr, "SQLite3");
            
            rowData.Id = FileConnection.Query<int>(_command, rowData).First();
        }

        /// <summary>
        /// This one uses Dapper
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

        void PrintTable(List<RowData> data)
        {
            int dataTabs = 0;
            int nameTabs = 0;
            int valuesDataTab = 0;
            int valuesNameTab = 0;

            foreach (var row in data)
            {
                //gets the length of the longest entry in data
                if (row.Data.Length > dataTabs)
                {
                    dataTabs = row.Data.Length;
                }

                if (row.Name.Length > nameTabs)
                {
                    nameTabs = row.Name.Length;
                }
            }

            dataTabs = (dataTabs > 8) ? 8 : (dataTabs / 8) + 1;
            nameTabs = (nameTabs / 8) + 1;

            //printing stuff
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

            foreach (RowData value in data)
            {
                valuesDataTab = (value.Data.Length > 60) ? 1 :dataTabs - (value.Data.Length / 8);
                valuesNameTab = nameTabs - (value.Name.Length / 8);

                Console.Write($"{value.Id.ToString()}\t{value.Name}");

                for (int n = 0; n < valuesNameTab; n++)
                {
                    Console.Write("\t");
                }

                if(value.Data.Length > 60)
                { 
                    Console.Write($"{value.Data.Remove(60)}...");
                }
                else
                {
                    Console.Write($"{value.Data}");
                }

                for (int k = 0; k < valuesDataTab; k++)
                {
                    Console.Write("\t");
                }

                Console.WriteLine(value.ExtensionId);
            }
        }

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
            catch (Exception)
            {
                return "Operation Failed";
            }
        }

        public RowData Deserialize(RowData data, int Id, string table, DeserializationPath path)
        {
            string _path;
            string _main = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data");
            string _manual = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data\Manual\");
            string _pending = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data\Pending\");
            string _processed = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data\Processed\");

            switch (path)
            {
                case DeserializationPath.Manual:
                    _path = _manual + $"{table}_ID_{data.Id}_Tx.tranx";
                    break;
                case DeserializationPath.Pending:
                    _path = _pending + $"{table}_ID_{data.Id}_Tx.tranx";
                    break;
                case DeserializationPath.Processed:
                    _path = _processed + $"{table}_ID_{data.Id}_Tx.tranx";
                    break;
                default:
                    return null;
            }

            XmlSerializer reader = new XmlSerializer(typeof(RowData));

            StreamReader _readTx = new StreamReader(_path);

            RowData _returnValue = (RowData)reader.Deserialize(_readTx);

            _readTx.Close();

            return _returnValue;
        }

        public void Serialize(RowData data, string table)
        {
            string _path;
            string _main= Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data\");
            string _manual = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data\Manual\");
            string _pending = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data\Pending\");
            string _processed = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName, @"Data\Processed\");

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

        public DataSet GrabData(string table)
        {
            int _totalRows;
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
            Console.WriteLine($"\n\n\n\nData from {table}\n");

            return data;
        }

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

                PrintTable(_rowData);

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
                PrintTable(_rowData);

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

        public void MoveData(string PrimaryTable)
        {
            int _secondaryChoice;
            int _rowSelected;

            DataSet data = new DataSet();
            List<RowData> rows = new List<RowData>();
            List<string> tables = new List<string>();

            data = GrabData(PrimaryTable);
            tables = GetTables();

            foreach (DataRow item in data.Tables[0].Rows)
            {
                rows.Add(new RowData()
                {
                    Id = (Int64)item.ItemArray[0],
                    Name = (string)item.ItemArray[1],
                    Data = (string)item.ItemArray[2],
                    ExtensionId = (Int64)item.ItemArray[3]
                });
            }

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
                if(!int.TryParse(Console.ReadLine(), out _rowSelected)) { _rowSelected = -1; }



                Console.Clear();
                Console.WriteLine("\n\n\n\n\n" +
                            "\t\t\t\t________________Move Data Menu________________\n"+
                            "\t\t\t\t|--------------------------------------------|\n" +
                            "\t\t\t\t|--------------------------------------------|");
                #region Dash Loop

                foreach (string table in tables)
                {
                    if(table == PrimaryTable)
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

                if(!int.TryParse(Console.ReadLine(), out _secondaryChoice)) { _secondaryChoice = -1; }

                try
                {
                    if (_secondaryChoice == 0)
                    {
                        return;
                    }

                    if (tables[_secondaryChoice - 1]  !=  PrimaryTable)
                    {
                        MoveToSecondary(tables[_secondaryChoice - 1], (_rowSelected - 1));
                        Console.WriteLine("\t\t\t\tMove Succsessful.");
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

            //
            //Submethods
            //

            void MoveToSecondary(string SecondaryTable, int RowId)
            {
                string _cmd = $"INSERT INTO {SecondaryTable} (Name, Data, ExtensionId) VALUES (@Name, @Data, @ExtensionId);";
                int _rowsAffected = 0;

                using (SQLiteCommand insertCommand = new SQLiteCommand(_cmd, FileConnection))
                {
                    insertCommand.Parameters.Add(new SQLiteParameter("@Name", rows[RowId].Name));
                    insertCommand.Parameters.Add(new SQLiteParameter("@Data", rows[RowId].Data));
                    insertCommand.Parameters.Add(new SQLiteParameter("@ExtensionId", rows[RowId].ExtensionId));

                    _rowsAffected += insertCommand.ExecuteNonQuery();
                }
                

                Console.WriteLine($"\n\n\t\t\t\tCommand Executed Successfully: {_rowsAffected} row(s) affected.");
            }
        }

        #region Support Functions

        //
        //support functions
        //
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
            List<string> _retValue = new List<string>();
            DataTable tableData = FileConnection.GetSchema("Tables");

            foreach (DataRow row in tableData.Rows) { _retValue.Add(row[2].ToString()); }

            return _retValue;
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

        public enum DeserializationPath
        {
            Manual = 1,
            Pending = 2, 
            Processed = 3
        }

        #endregion
    }
}
