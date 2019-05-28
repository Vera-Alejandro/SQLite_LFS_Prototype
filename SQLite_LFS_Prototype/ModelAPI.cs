using System;
using System.Data;
using System.Data.SQLite;

namespace SQLite_LFS_Prototype.API
{

    //interfaces

    public interface IDbEnginerAdapter
    {
        bool Open();
        DataSet Execute(string command);
        IDataReader ExecuteQuery(string command);
        bool ExecureNonQuery(string command);
        bool Close();
    }

    ///Classes

    //abstract

    public abstract class DbAbstractFactory
    {
        public abstract IDbConnection CreateConnection(string constr);
        public abstract IDbCommand CreateCommand(IDbConnection con, string cmd);
        public abstract IDbDataAdapter CreateDbAdapter(IDbCommand cmd);
        public abstract IDataReader CreateDataReader(IDbCommand cmd);
    }

    //regualar

    public class SQLiteDbFactory : DbAbstractFactory
    {
        private string Drivertype { get; set; }

        public SQLiteDbFactory() { this.Drivertype = null; }

        public override IDbConnection CreateConnection(string constr)
        {
            if (constr == null || constr.Length == 0) { return null; }

            return new SQLiteConnection(constr);
        }

        public override IDbCommand CreateCommand(IDbConnection con, string cmd)
        {
            if(con == null || cmd == null || cmd.Length == 0) { return null; }

            if(con is SQLiteConnection) { return new SQLiteCommand(cmd, (SQLiteConnection)con); }

            return null;
        }

        public override IDbDataAdapter CreateDbAdapter(IDbCommand cmd)
        {
            if (cmd == null) { return null; }

            if (cmd is SQLiteCommand) { return new SQLiteDataAdapter((SQLiteCommand)cmd); }

            return null;
        }

        public override IDataReader CreateDataReader(IDbCommand cmd)
        {
            if (cmd == null) { return null; }

            if (cmd is SQLiteCommand) { return (SQLiteDataReader)cmd.ExecuteReader(); }

            return null;
        }

        public class DbEngineAdapter : IDbEnginerAdapter
        {
            //properties
            //static ObjectFactory of = new ObjectFactory("DbDrivers.xml");
            private IDbConnection _con = null;
            private IDbCommand _cmd = null;
            private DbAbstractFactory df = null;
            private string _constr;
            private string _driver;

            public bool Close()
            {
                throw new NotImplementedException();
            }
            
            public DbEngineAdapter(string constr, string driver)
            {
                _constr = constr;
                _driver = driver;
                //df = (DbAbstractFactory)of.Get(driver, "prototype");
            }



            public bool ExecureNonQuery(string command)
            {
                throw new NotImplementedException();
            }

            public DataSet Execute(string command)
            {
                throw new NotImplementedException();
            }

            public IDataReader ExecuteQuery(string command)
            {
                throw new NotImplementedException();
            }

            public bool Open()
            {
                throw new NotImplementedException();
            }
        }
    }
}