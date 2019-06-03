using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLite_LFS_Prototype;
using System.Collections.Generic;
using System.Data.SQLite;
using System;

namespace SQliteTesting
{
    [TestClass]
    public class UnitTest1
    {
        Database dbTest = new Database("LogFile.sqlite3");


        [TestMethod]
        public void Connect()
        {

            bool expected = true;
            bool actual = dbTest.Connect();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Disconnect()
        {
            dbTest.Connect();

            bool expected = true;
            bool actual = dbTest.Disconnect();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CreateTable()
        {
            dbTest.Connect();

            //test the int return of the command.execute SQLite Function
            var expected = "Command Completed Successfully!";
            var actual = dbTest.CreateTable();

            Assert.AreEqual(expected, actual);
            //notes

            dbTest.Disconnect();
        }

        [TestMethod]
        public void PrepInsert()
        {
            string table = "InsertTest";
            dbTest.Connect();

            List<string> columns = new List<string>
            {
                "Name TEXT",
                "Data TEXT",
                "Extension TEXT"
            };
            
            dbTest.CreateTable(table, columns);

            List<string> fields = new List<string>
            {
                "Name",
                "Data",
                "Extension"
            };

            List<string> data = new List<string>
            {
                "'FirstFile.txt'",
                "'this is the data'",
                "'.txt'"
            };

            var expected = "INSERT INTO InsertTest(Name, Data, Extension) VALUES ('FirstFile.txt', 'this is the data', '.txt');";
            var actual = dbTest.InsertInto(table, fields, data);
            
            Assert.AreEqual(expected, actual);

            dbTest.Disconnect();
        }

        [TestMethod]
        public void DropTable()
        {
            string table = "TheBeat";

            dbTest.Connect();

            TestCase(table);

            var expected = "Command Successfully Executed";
            var actual = dbTest.DropTable(table);

            Assert.AreEqual(expected, actual);

            dbTest.Disconnect();
        }

        [TestMethod]
        public void SelectAll()
        {
            string table = "SelectTest";
            dbTest.Connect();
            TestCase(table);

            TestCase(table);

            //var expected = "";
            //var actual = " ";

            Assert.Fail();

            dbTest.Disconnect();
        }

        [TestMethod]
        public void TableNames()
        {
            dbTest.Connect();

            List<string> expected = new List<string>
            {
                "ExtensionInfo",
                "FileData",
                "ManualTx",
                "PendingTx",
                "ProcessedTx"
                
            };

            List<string> actual = dbTest.GetTables();

            Assert.AreEqual(expected, actual);

            dbTest.Disconnect();
        }
        
        [TestMethod]
        public void TestDate()
        {
            DateTime date = DateTime.Now;

            string _formatedDate = "{0}-{1}-{2} {3}:{4}:{5}.{6}";
            string _newDate = string.Format(_formatedDate, date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond);

            DateTime _return = Convert.ToDateTime(_newDate);
            string _secondDate = string.Format(_formatedDate, _return.Year, _return.Month, _return.Day, _return.Hour, _return.Minute, _return.Second, _return.Millisecond);

            DateTime final = Convert.ToDateTime(_secondDate);

            var actual = _newDate;
            var expected = _secondDate;

            Assert.AreEqual(expected, actual);
        }


        //convert data in tables to objects 
        public void Objectify()
        {

            Assert.Fail();
        }

        //for testing purposes only
        public void TestCase(string TableName)
        {
            List<string> columns = new List<string>
            {
                "Name TEXT",
                "Data TEXT",
                "Extension TEXT"
            };
            dbTest.CreateTable(TableName, columns);

            List<string> data = new List<string>
            {
                "'FirstFile.txt'",
                "'this is the data'",
                "'.txt'"
            };
            dbTest.InsertInto(TableName, columns, data);
        }
    }
}