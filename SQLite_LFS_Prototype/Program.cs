using System;
using System.Collections.Generic;
using CommandLine;
using SQLite_LFS_Prototype.Model;

namespace SQLite_LFS_Prototype
{
    class Program
    {
        static void Main(string[] args)
        {
            //variables
            bool _menuContinue = true;
            List<ExtensionInfo> extData = new List<ExtensionInfo>();
            List<FileData> selectedData = new List<FileData>();
            List<string> tables = new List<string>();

            Options results = Parser.Default.ParseArguments<Options>(args)
                .MapResult
                (
                    opt => opt,
                    error => throw new Exception("Failed to Parse Arguments."
                ));

            //create database
            Database sqlDatabase = new Database(results.FileLocation);

            //if connection failed exit program
            if (!(sqlDatabase.Connect()))
            {
                Console.ReadKey();
                return;
            }

            sqlDatabase.CreateTable();

            //grab the table info
            tables = sqlDatabase.GetTables();

            sqlDatabase.Sync();

            Menu();

            sqlDatabase.Disconnect();
            Console.ReadKey();

            //
            //Submethods
            //

            void Menu()
            {
                while (_menuContinue)
                {
                    int _mainChoice = -1;
                    Console.Clear();

                    #region Main Menu Screen
                    Console.Write("\n\n\n\n\n" +
                        "\t\t\t\t_________SQlite LFS Proof of Concept__________\n" +
                        "\t\t\t\t|--------------------------------------------|\n" +
                        "\t\t\t\t|--------------------------------------------|\n" +
                        "\t\t\t\t|------------1 - Insert----------------------|\n" +
                        "\t\t\t\t|------------2 - Delete----------------------|\n" +
                        "\t\t\t\t|------------3 - Show Table Data-------------|\n" +
                        "\t\t\t\t|------------4 - Process Manually------------|\n" +
                        "\t\t\t\t|--------------------------------------------|\n" +
                        "\t\t\t\t|------------0 - Exit------------------------|\n" +
                        "\t\t\t\t|--------------------------------------------|\n" +
                        "\t\t\t\t|____________________________________________|" +
                        "\n\n\t\t\t\tWhat would you like to do? ");
                    #endregion

                    if (!(int.TryParse(Console.ReadLine().ToString(), out _mainChoice)))
                    {
                        _mainChoice = -1;
                    }

                    switch (_mainChoice)
                    {
                        //Insert
                        case 1:
                            InsertMenu();
                            break;
                        //Delete
                        case 2:
                            DeleteMenu();
                            break;
                        //Show Table Data
                        case 3:
                            TableDataMenu();
                            break;
                        //Move Data
                        case 4:
                            ManuallyProcessMenu();
                            break;
                        //Exit
                        case 0:
                            #region Exit Case
                            Console.Clear();
                            Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\t\t\t\tGoodbye.\n\n\n\n\n\n\n\n\n\n\t\t\t\t");
                            _menuContinue = false;
                            #endregion
                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine("Please Enter a Valid Option");
                            Wait();
                            break;
                    }

                    void InsertMenu()
                    {
                        int _insertMenuChoice;
                        bool _tableDataContinue;

                        do
                        {
                            _tableDataContinue = false;

                            #region Insert Data Menu

                            Console.Clear();
                            Console.WriteLine("\n\n\n\n\n" +
                                        "\t\t\t\t_______________Insert Data Menu_______________");
                            PrintMenu();
                            Console.Write("What Table would you like to Add a row to: ");

                            #endregion

                            if (!int.TryParse(Console.ReadLine(), out _insertMenuChoice)) { _insertMenuChoice = -1; }

                            try
                            {
                                if (_insertMenuChoice == 0)
                                {
                                    return;
                                }
                                sqlDatabase.InsertInto(tables[_insertMenuChoice - 1]);
                            }
                            catch (Exception)
                            {
                                _tableDataContinue = true;
                                Console.Clear();
                                Console.WriteLine("Please Enter a Valid Option");
                                throw;
                            }

                        } while (_tableDataContinue);
                    }

                    void DeleteMenu()
                    {
                        int _deleteMenuChoice;
                        bool _tableDataContinue;

                        do
                        {
                            _tableDataContinue = false;

                            #region Delete Data Menu

                            Console.Clear();
                            Console.WriteLine("\n\n\n\n\n" +
                            "\t\t\t\t_______________Delete Data Menu_______________");
                            PrintMenu();

                            #endregion

                            if (!int.TryParse(Console.ReadLine(), out _deleteMenuChoice)) { _deleteMenuChoice = -1; }

                            if (_deleteMenuChoice > tables.Count || _deleteMenuChoice < 0)
                            {
                                _tableDataContinue = true;
                                Console.Clear();
                                Console.WriteLine("Please Enter a Valid Option");
                                Console.ReadKey();
                            }

                            try
                            {
                                if (_deleteMenuChoice == 0) { return; }
                                sqlDatabase.DeleteRow(tables[_deleteMenuChoice - 1]);
                            }
                            catch (Exception ex)
                            {
                                _tableDataContinue = true;
                                Console.Clear();
                                Console.WriteLine(ex.Message);
                                throw;
                            }

                        } while (_tableDataContinue);
                    }

                    void TableDataMenu()
                    {
                        int _tableDataChoice;
                        bool _tableDataContinue;

                        do
                        {
                            _tableDataContinue = false;

                            #region Table Data Menu

                            Console.Clear();
                            Console.WriteLine("\n\n\n\n\n" +
                            "\t\t\t\t_______________Table Data Menu________________");
                            PrintMenu();
                            Console.Write("\n\t\t\t\tSelect the table you want to view: ");

                            #endregion

                        if (!int.TryParse(Console.ReadLine(), out _tableDataChoice)) { _tableDataChoice = -1; }

                        if (_tableDataChoice > tables.Count)
                        {
                                _tableDataContinue = true;
                        }

                            try
                            {
                                if (_tableDataChoice == 0) { return; }
                                sqlDatabase.SelectRow(tables[_tableDataChoice - 1]);
                            }
                            catch (Exception ex)
                            {
                                _tableDataContinue = true;
                                Console.Clear();
                                Console.WriteLine(ex.Message);
                                Wait();
                            }

                        } while (_tableDataContinue);
                    }

                    void ManuallyProcessMenu()
                    {
                        bool _moveDataContinue;

                        do
                        {
                            _moveDataContinue = false;
                            
                            try
                            {
                                sqlDatabase.ManuallyProcess_Testing("ManualTx");
                            }
                            catch (Exception ex)
                            {
                                _moveDataContinue = true;
                                Console.Clear();
                                Console.WriteLine(ex.Message);
                                Console.WriteLine("Please Enter a Valid Option");
                                Wait();
                            }

                        } while (_moveDataContinue);
                    }

                    void Wait()
                    {
                        Console.WriteLine("Press Any Key To Continue...");
                        Console.ReadKey();
                    }

                    void PrintMenu()
                    {
                        #region local Variables
                        int _dashCount;
                        int _option = 0;

                        string _nameMenu;
                        #endregion
                        Console.WriteLine(
                                    "\t\t\t\t|--------------------------------------------|\n" +
                                    "\t\t\t\t|--------------------------------------------|");
                        #region Dash Loop

                        foreach (string table in tables)
                        {
                            _nameMenu = $"\t\t\t\t|------------{_option + 1} - {table}";
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
                                    "\t\t\t\t");
                    }
                }
            }
        }
        

        public class Options
        {
            [Option('f', "filelocation", Required = true, HelpText = "enter the file location of the SQLite Db File.")]
            public string FileLocation { get; set; }
        }
    }
}