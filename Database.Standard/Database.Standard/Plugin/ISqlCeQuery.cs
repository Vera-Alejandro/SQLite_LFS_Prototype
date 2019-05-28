using System;
using System.Data;
namespace Interstates.Control.Database.Plugin
{
    public interface ISqlCeQuery
    {
        void AddInParameter(System.Data.Common.DbCommand command, string name, System.Data.SqlDbType dbType);
        void AddInParameter(System.Data.Common.DbCommand command, string name, System.Data.SqlDbType dbType, object value);
        void AddInParameter(System.Data.Common.DbCommand command, string name, System.Data.SqlDbType dbType, string sourceColumn, System.Data.DataRowVersion sourceVersion);
        void AddOutParameter(System.Data.Common.DbCommand command, string name, System.Data.SqlDbType dbType, int size);
        void AddParameter(System.Data.Common.DbCommand command, string name, System.Data.SqlDbType dbType, System.Data.ParameterDirection direction, string sourceColumn, System.Data.DataRowVersion sourceVersion, object value);
        void AddParameter(System.Data.Common.DbCommand command, string name, System.Data.SqlDbType dbType, int size, System.Data.ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, System.Data.DataRowVersion sourceVersion, object value);
        string BuildParameterName(string name);
        void Compact(string connectionString);
        System.Data.Common.DbCommandBuilder CreateCommandBuilder();
        System.Data.Common.DbConnection CreateConnection();
        System.Data.Common.DbDataAdapter CreateDataAdapter();
        void CreateDatabase();
        System.Data.Common.DbParameter CreateParameter();
        System.Data.Common.DbParameter CreateParameter(string name, object value);
        void Repair(string connectionString, bool recoverCorruptedRows);
        void Shrink();
        int UpdateDataSet(System.Data.Common.DbTransaction transaction, System.Data.DataSet data, string tableName);
        int UpdateDataSet(System.Data.DataSet data, string tableName);
        void UpgradeDB();
        void UpgradeDB(string destinationConnectionString);
        bool Verify();
    }
}
