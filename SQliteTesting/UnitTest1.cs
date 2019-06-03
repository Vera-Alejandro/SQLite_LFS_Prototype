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
        Database dbTest = new Database(@"C:\Users\alejandro.vera\source\repos\SQLite_LFS_Prototype\SQLite_LFS_Prototype\LogFile.sqlite3");

        #region Successful Tests
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

        #endregion

        [TestMethod]
        public void MoveData()
        {

            dbTest.Connect();

            


            Assert.IsTrue(true);

            dbTest.Disconnect();
        }

    }
}