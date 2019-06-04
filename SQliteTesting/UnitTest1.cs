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
            DateTime Mi1 = new DateTime(2019, 08, 13, 10, 30, 15, 952);
            DateTime Mi2 = new DateTime(2019, 08, 13, 10, 30, 15, 95);
            DateTime Mi3 = new DateTime(2019, 08, 13, 10, 30, 15, 9);

            DateTime S1 = new DateTime(2019, 08, 13, 10, 30, 15, 952);
            DateTime S2 = new DateTime(2019, 08, 13, 10, 30, 1, 952);

            DateTime Mn1 = new DateTime(2019, 08, 13, 10, 30, 15, 952);
            DateTime Mn2 = new DateTime(2019, 08, 13, 10, 3, 15, 952);

            DateTime H1 = new DateTime(2019, 08, 13, 10, 30, 15, 952);
            DateTime H2 = new DateTime(2019, 08, 13, 1, 30, 15, 952);

            DateTime D1 = new DateTime(2019, 08, 13, 10, 30, 15, 952);
            DateTime D2 = new DateTime(2019, 08, 1, 10, 30, 15, 952);

            DateTime Mo1 = new DateTime(2019, 08, 13, 10, 30, 15, 952);
            DateTime Mo2 = new DateTime(2019, 10, 13, 10, 30, 15, 952);

            string Mi1Test = DateTimeSQlite(Mi1);
            string Mi2Test = DateTimeSQlite(Mi2);
            string Mi3Test = DateTimeSQlite(Mi3);

            string S1Test = DateTimeSQlite(S1);
            string S2Test = DateTimeSQlite(S2);

            string Mn1Test = DateTimeSQlite(Mn1);
            string Mn2Test = DateTimeSQlite(Mn2);

            string H1Test = DateTimeSQlite(H1);
            string H2Test = DateTimeSQlite(H2);

            string D1Test = DateTimeSQlite(D1);
            string D2Test = DateTimeSQlite(D2);

            string Mo1Test = DateTimeSQlite(Mo1);
            string Mo2Test = DateTimeSQlite(Mo2);

            dbTest.Connect();

            string cmd = $"INSERT INTO ManualTx (Data, DateCreated) VALUES ('sdfadsfadfa', '{Mi1Test}');" +
                $"SELECT * FROM ManualTx;";

            Assert.IsTrue(dbTest.ExecuteCommand(cmd));

            cmd = $"INSERT INTO ManualTx (Data, DateCreated) VALUES ('afsdfadsf', '{Mi2Test}');" +
                $"SELECT * FROM ManualTx;";

            Assert.IsTrue(dbTest.ExecuteCommand(cmd));

            cmd = $"INSERT INTO ManualTx (Data, DateCreated) VALUES ('afsdfadsf', '{Mi3Test}');" +
                $"SELECT * FROM ManualTx;";

            Assert.IsTrue(dbTest.ExecuteCommand(cmd));

            cmd = $"INSERT INTO ManualTx (Data, DateCreated) VALUES ('afsdfadsf', '{S1Test}');" +
                $"SELECT * FROM ManualTx;";

            Assert.IsTrue(dbTest.ExecuteCommand(cmd));

            cmd = $"INSERT INTO ManualTx (Data, DateCreated) VALUES ('afsdfadsf', '{S2Test}');" +
                $"SELECT * FROM ManualTx;";

            Assert.IsTrue(dbTest.ExecuteCommand(cmd));

            cmd = $"INSERT INTO ManualTx (Data, DateCreated) VALUES ('afsdfadsf', '{Mn1Test}');" +
                $"SELECT * FROM ManualTx;";

            Assert.IsTrue(dbTest.ExecuteCommand(cmd));

            cmd = $"INSERT INTO ManualTx (Data, DateCreated) VALUES ('afsdfadsf', '{Mi2Test}');" +
                $"SELECT * FROM ManualTx;";

            Assert.IsTrue(dbTest.ExecuteCommand(cmd));

            cmd = $"INSERT INTO ManualTx (Data, DateCreated) VALUES ('afsdfadsf', '{H1Test}');" +
                $"SELECT * FROM ManualTx;";

            Assert.IsTrue(dbTest.ExecuteCommand(cmd));

            cmd = $"INSERT INTO ManualTx (Data, DateCreated) VALUES ('afsdfadsf', '{H2Test}');" +
                $"SELECT * FROM ManualTx;";

            Assert.IsTrue(dbTest.ExecuteCommand(cmd));

            cmd = $"INSERT INTO ManualTx (Data, DateCreated) VALUES ('afsdfadsf', '{D1Test}');" +
                $"SELECT * FROM ManualTx;";

            Assert.IsTrue(dbTest.ExecuteCommand(cmd));

            cmd = $"INSERT INTO ManualTx (Data, DateCreated) VALUES ('afsdfadsf', '{D2Test}');" +
                $"SELECT * FROM ManualTx;";

            Assert.IsTrue(dbTest.ExecuteCommand(cmd));

            cmd = $"INSERT INTO ManualTx (Data, DateCreated) VALUES ('afsdfadsf', '{Mo1Test}');" +
                $"SELECT * FROM ManualTx;";

            Assert.IsTrue(dbTest.ExecuteCommand(cmd));

            cmd = $"INSERT INTO ManualTx (Data, DateCreated) VALUES ('afsdfadsf', '{Mo2Test}');" +
                $"SELECT * FROM ManualTx;";

            Assert.IsTrue(dbTest.ExecuteCommand(cmd));



            dbTest.Disconnect();

        
            string DateTimeSQlite(DateTime date)
            {
                string testDate;
                string _formatedDate = "{0}-{1}-{2} {3}:{4}:{5}.{6}";

                if (date.Month > 10 && date.Day > 10)
                {
                    testDate = string.Format(_formatedDate, date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond);
                }
                else
                {
                    testDate = $"{date.Year}-";

                    testDate += (date.Month < 10) ? $"0{date.Month}-" : $"{date.Month}-";

                    testDate += (date.Day < 10) ? $"0{date.Day}" : $"{date.Day}";

                    testDate += $" {date.Hour}:{date.Minute}:{date.Second}.";

                    testDate += (date.Millisecond / 10 != 0) ? date.Millisecond.ToString() : testDate += (date.Millisecond / 100 == 0) ? $"{date.Millisecond}00" : $"{date.Millisecond}0";
                }

                return testDate;
            }
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