using System;
using System.Collections.Generic;
using Interstates.Control.Database.SQLite3;
using Interstates.Control.Database;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
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
            List<RowData> selectedData = new List<RowData>();
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
                        "\t\t\t\t|------------4 - Move Data-------------------|\n" +
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
                            MoveDataMenu();
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

                            try
                            {
                                if (_deleteMenuChoice == 0)
                                {
                                    return;
                                }
                                sqlDatabase.DeleteRow(tables[_deleteMenuChoice - 1]);
                            }
                            catch (Exception ex)
                            {
                                _tableDataContinue = true;
                                Console.Clear();
                                Console.WriteLine("Please Enter a Valid Option");
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

                            #endregion

                            if (!int.TryParse(Console.ReadLine(), out _tableDataChoice)) { _tableDataChoice = -1; }

                            try
                            {
                                if (_tableDataChoice == 0)
                                {
                                    return;
                                }
                                sqlDatabase.SelectRow(tables[_tableDataChoice - 1]);
                            }
                            catch (Exception)
                            {
                                _tableDataContinue = true;
                                Console.Clear();
                                Console.WriteLine("Please Enter a Valid Option");
                                Wait();
                            }

                        } while (_tableDataContinue);
                    }

                    void MoveDataMenu()
                    {
                        int _tableDataChoice;
                        bool _tableDataContinue;

                        do
                        {
                            _tableDataContinue = false;

                            #region Move Data Menu

                            Console.Clear();
                            Console.WriteLine("\n\n\n\n\n" +
                            "\t\t\t\t________________Move Data Menu________________");
                            PrintMenu();
                            Console.Write("\n\t\t\t\tSelect the Primary Table: ");
                            #endregion

                            if (!int.TryParse(Console.ReadLine(), out _tableDataChoice)) { _tableDataChoice = -1; }

                            try
                            {
                                if (_tableDataChoice == 0)
                                {
                                    return;
                                }
                                sqlDatabase.MoveData(tables[_tableDataChoice - 1]);
                            }
                            catch (Exception)
                            {
                                _tableDataContinue = true;
                                Console.Clear();
                                Console.WriteLine("Please Enter a Valid Option");
                                Wait();
                            }

                        } while (_tableDataContinue);
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
                        Console.WriteLine("\t\t\t\t|--------------------------------------------|\n" +
                                    "\t\t\t\t|------------0 - Back------------------------|\n" +
                                    "\t\t\t\t|--------------------------------------------|\n" +
                                    "\t\t\t\t|____________________________________________|");
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