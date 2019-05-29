using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Interstates.Control.Database;
using Interstates.Control.Database.SQLite3;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using SQLite_LFS_Prototype.Model;

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
            
            //FileConnection = new SQLiteConnection($@"Data Source={_filePath}; Version=3;");
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
                SQLiteCommand newTable;
                using (newTable = new SQLiteCommand(command, FileConnection))
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
                SQLiteCommand newCommand;
                using (newCommand = new SQLiteCommand(command, FileConnection))
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
                SQLiteCommand insertCommand;
                using (insertCommand = new SQLiteCommand(command, FileConnection))
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

        public int InsertInto(int Id, RowData data)
        {
            return -1;
        }

        //this is using the reader
        public void SelectAll(string table, List<string> columns)
        {
            #region Variables
            List<int> _tabsNeeded = new List<int>();
            List<string> _tableData = new List<string>();
            List<string> _tempColumns = new List<string>();

            string command = $"SELECT * FROM {table};";

            SQLiteCommand selectAll = new SQLiteCommand(command, FileConnection);
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

            dataTabs = (dataTabs / 8) + 1;
            nameTabs = (nameTabs / 8) + 1;

            //printing stuff
            Console.Clear();

            Console.Write("ID\tName");

            for (int n = 0; n < nameTabs; n++)
            {
                Console.Write("\t");
            }

            Console.Write("Data");

            for (int i = 0; i < (dataTabs % 8); i++)
            {
                Console.Write("\t");
            }

            Console.WriteLine("ExtensionId");

            foreach (RowData value in data)
            {
                valuesDataTab = dataTabs - (value.Data.Length / 8);
                valuesNameTab = nameTabs - (value.Name.Length / 8);

                Console.Write($"{value.Id.ToString()}\t{value.Name}");

                for (int n = 0; n < valuesNameTab; n++)
                {
                    Console.Write("\t");
                }

                if(value.Data.Length > 60)
                { 
                    Console.Write($"{value.Data.Remove(60)}");
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

        void PrintTable(List<ExtensionInfo> data)
        {
            //printing stuff
            Console.WriteLine("ID\tExtension");

            foreach (ExtensionInfo value in data)
            {
                Console.WriteLine($"{value.Id.ToString()}\t{value.Extension}");
            }
        }

        public string DropTable (string table)
        {
            string command = $"DROP TABLE IF EXISTS {table};";

            try
            {
                SQLiteCommand dropTable;
                using (dropTable = new SQLiteCommand(command, FileConnection))
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

        

        public DataSet SelectAll(string table)
        {
            int _totalRows;
            string _command = $"SELECT * FROM {table};";

            List<ExtensionInfo> extData = new List<ExtensionInfo>();
            List<RowData> _rowData = new List<RowData>();

            DataSet data = new DataSet();
            DbCommand command = FileConnection.CreateCommand();
            DatabaseProfile profile = new DatabaseProfile("sqlite", constr, "SQLite 3");
            SQLiteQuery query = new SQLiteQuery(profile);

            command.CommandText = _command;
            command.CommandType = CommandType.Text;
            data = query.Execute(command);
            _totalRows = data.Tables[0].Rows.Count;

            Console.Clear();
            Console.WriteLine($"\n\n\n\nData from {table}\n");

            if (table == "ExtensionInfo")
            {

                foreach (DataRow row in data.Tables[0].Rows)
                {
                    extData.Add(new ExtensionInfo()
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
            
            return data;
        }

        public void DeleteRow(string table)
        {
            #region variables 

            int _rowChoice;
            int _totalRows;
            int _rowsAffected;

            bool _rowContinue = true;


            DataSet data = SelectAll(table);

            List<RowData> _rowData = new List<RowData>();
            List<ExtensionInfo> _extInfo = new List<ExtensionInfo>();

            _totalRows = data.Tables[0].Rows.Count;
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
                string _deleteCmd = $"DELETE FROM {table} WHERE Id = ";

                PrintTable(_rowData);

                Console.WriteLine("\n\nWhat Row would you like to remove? Press 0 to exit.");
                if (!int.TryParse(Console.ReadLine(), out _rowChoice)) { _rowChoice = -1; }

                if (_rowChoice == 0)
                {
                    _rowContinue = false;
                }

                try
                {
                    if (_rowChoice > _totalRows)
                    {
                        _rowContinue = true;

                        Console.Clear();
                        Console.WriteLine("Please Enter a Valid Option");
                        Console.WriteLine("Press Any Key To Continue...");
                        Console.ReadKey();
                    }

                    _deleteCmd += $"{_rowChoice};";
                    SQLiteCommand _deleteRow;
                    using (_deleteRow = new SQLiteCommand(_deleteCmd, FileConnection))
                    {
                        _rowsAffected = _deleteRow.ExecuteNonQuery();
                        _rowData.RemoveAt(_rowChoice - 1);
                        _totalRows--;
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

            DataSet data = SelectAll(table);

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
                _totalRows = data.Tables[0].Rows.Count;

                Console.WriteLine("\n\nTo view the data of a row select the Id. Press 0 to exit");
                if (!int.TryParse(Console.ReadLine(), out _rowChoice)) { _rowChoice = -1; }

                if (_rowChoice == 0)
                {
                    return;
                }

                try
                {
                    if (_rowChoice > _totalRows)
                    {
                        _rowContinue = true;

                        Console.Clear();
                        Console.WriteLine("Please Enter a Valid Option");
                        Console.WriteLine("Press Any Key To Continue...");
                        Console.ReadKey();
                    }

                    foreach (RowData item in _rowData)
                    {
                        if (item.Id == _rowChoice)
                        {
                            Console.Clear();
                            Console.WriteLine($"\n\n\n\n\n\t\t\t\t\tSelected {item.Name}\n\n");
                            Console.WriteLine($"\t\t\tID: {item.Id}\n");
                            Console.WriteLine($"\t\t\tName: {item.Name}\n");
                            Console.WriteLine($"\t\t\tData: \n\t\t\t{item.Data}\n");
                            Console.WriteLine($"\t\t\tExtensionId: {item.ExtensionId}\n");
                            Console.WriteLine("\nPress Any Key To Continue...");
                            Console.ReadKey();
                            return;
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

        #endregion
    }
}
