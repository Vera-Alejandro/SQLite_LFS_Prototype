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

            //create database
            Database sqlDatabase = new Database(args[0]);
            //if connection failed exit program
            if(!(sqlDatabase.Connect()))
            {
                Console.ReadKey();
                return;
            }


            sqlDatabase.CreateTable();

            //grab the table info
            tables = sqlDatabase.GetTables();

            Menu();

            #region Graveyard
            //graveyard

            /*
            string testTable = "FileTest";

            List<string> columns = new List<string>
            {
                "Name TEXT",
                "Data TEXT",
                "Extension TEXT"
            };
            sqlDatabase.CreateTable(testTable, columns);

            List<string> data1 = new List<string>
            {
                "'FirstFile.txt'",
                "'this is the data'",
                "'.txt'"
            };

            List<string> data2 = new List<string>
            {
                "'PhoneLogs.txt'",
                "'someone called me at this number. today'",
                "'.txt'"
            };

            sqlDatabase.InsertInto(testTable, columns, data1);

            sqlDatabase.InsertInto(testTable, columns, data2);

            sqlDatabase.SelectAll(testTable, columns);
            */

            #endregion

            sqlDatabase.Disconnect();
            Console.ReadKey();
            
            //Submethods

            void Menu()
            {
                while (_menuContinue)
                {
                    int _mainChoice = -1;
                    Console.Clear();

                    #region Menu Screen
                    Console.Write("\n\n\n\n\n" +
                        "\t\t\t\t_________SQlite LFS Proof of Concept__________\n" +
                        "\t\t\t\t|--------------------------------------------|\n" +
                        "\t\t\t\t|--------------------------------------------|\n" +
                        "\t\t\t\t|------------1 - Insert----------------------|\n" +
                        "\t\t\t\t|------------2 - Delete----------------------|\n" +
                        "\t\t\t\t|------------3 - Show Table Data-------------|\n" +
                        "\t\t\t\t|------------4 - Move Data-------------------|\n" +
                        "\t\t\t\t|------------5 - Show Data-------------------|\n" +
                        "\t\t\t\t|--------------------------------------------|\n" +
                        "\t\t\t\t|------------0 - Exit------------------------|\n" +
                        "\t\t\t\t|--------------------------------------------|\n" +
                        "\t\t\t\t|____________________________________________|" +
                        "\n\n\t\t\t\tWhat would you like to do? ");
                    #endregion


                    if(!(int.TryParse(Console.ReadLine().ToString(), out _mainChoice)))
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
                            Console.WriteLine("Move Data Selected");
                            Wait();
                            break;
                        //Show Data
                        case 5:
                            sqlDatabase.SelectAll();
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

                    }

                    void DeleteMenu()
                    {

                    }

                    void TableDataMenu()
                    {
                        #region local Variables
                        int _tableDataChoice = -1;
                        int _dashCount;
                        int _option;

                        bool _tableDataContinue;

                        string _nameMenu;
                        #endregion

                        do
                        {
                            #region declare variables

                            _option = 0;
                            _tableDataContinue = false;
                            
                            #endregion

                            #region Table Data Menu

                            Console.Clear();
                            Console.WriteLine("\n\n\n\n\n" +
                            "\t\t\t\t_______________Table Data Menu________________\n" +
                            "\t\t\t\t|--------------------------------------------|\n" +
                            "\t\t\t\t|--------------------------------------------|");
                            #region Dash Loop

                            foreach(string table in tables)
                            {
                                _nameMenu = $"\t\t\t\t|------------{_option + 1} - {table}";
                                Console.Write(_nameMenu);
                                _dashCount = (49 - _nameMenu.Length);
                                for (int i = 0; i < _dashCount; i++)
                                {
                                    Console.Write("-");
                                }
                                Console.WriteLine("|");
                                _option++;
                            }

                            #endregion
                            Console.Write("\t\t\t\t|--------------------------------------------|\n" +
                            "\t\t\t\t|------------0 - Back------------------------|\n" +
                            "\t\t\t\t|--------------------------------------------|\n" +
                            "\t\t\t\t|____________________________________________|" +
                            "\n\n\t\t\t\tWhat would you like to do? ");
                            
                            #endregion

                            if (!int.TryParse(Console.ReadLine(), out _tableDataChoice)) { _tableDataChoice = -1; }

                            try
                            {
                                sqlDatabase.SelectAll(tables[_tableDataChoice - 1]);
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
                }
            }
        }

        

      
    }
}           

