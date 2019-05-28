using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Xml;
using System.Text.RegularExpressions;

namespace Interstates.Control.Database
{
    [ComVisible(false)]
    public sealed class SqlQuery : QueryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlQuery"/> class with a database profile <see cref="DatabaseProfile"/>.
        /// </summary>
        /// <param name="profile">The database profile that contains the connection to the database.</param>
        public SqlQuery(DatabaseProfile profile)
            : base(profile)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBase"/> class with a connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqlQuery(string connectionString)
            : base(connectionString)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBase"/> class with a connection.
        /// </summary>
        /// <param name="connection">An existing connection to base the query on.</param>
        public SqlQuery(DbConnection connection)
            : base(connection)
        {
        }

        /// <summary>
        /// <para>Gets the parameter token used to delimit parameters for the SQL Server database.</para>
        /// </summary>
        /// <value>
        /// <para>The '@' symbol.</para>
        /// </value>
        private char ParameterToken
        {
            get { return '@'; }
        }

        /// <summary>
        /// Create a new SQL Connection
        /// </summary>
        /// <returns></returns>
        public override System.Data.Common.DbConnection CreateConnection()
        {
            SqlConnection dbConnection = new SqlConnection(DatabaseProfile.ConnectionString);
            return dbConnection;
        }

        /// <summary>
        /// Create a new SQL DataAdapter
        /// </summary>
        /// <returns>DbDataAdapter</returns>
        public override System.Data.Common.DbDataAdapter CreateDataAdapter()
        {
            return new SqlDataAdapter();
        }

        /// <summary>
        /// Create a new SQL CommandBuilder
        /// </summary>
        /// <returns>DbCommandBuilder</returns>
        public override System.Data.Common.DbCommandBuilder CreateCommandBuilder()
        {
            return new SqlCommandBuilder();
        }

        /// <summary>
        /// Creates a parameter for a provider.
        /// </summary>
        /// <returns></returns>
        public override DbParameter CreateParameter()
        {
            return new SqlParameter();
        }

        /// <summary>
        /// Creates a parameter for a provider.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value to assign the parameter.</param>
        /// <returns></returns>
        public override DbParameter CreateParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }

        /// <summary>
        /// <para>Executes the <see cref="SqlCommand"/> and returns a new <see cref="XmlReader"/>.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The <see cref="SqlCommand"/> to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="XmlReader"/> object.</para>
        /// </returns>
        public XmlReader ExecuteXmlReader(DbCommand command)
        {
            SqlCommand sqlCommand = CheckIfSqlCommand(command);
            // If the command connection is already open don't create a new connection.
            if (command.Connection == null)
                command.Connection = OpenNewConnection();
            else if (command.Connection.State != ConnectionState.Open)
                OpenConnection(command.Connection);
            return DoExecuteXmlReader(sqlCommand);
        }

        /// <summary>
        /// <para>Executes the <see cref="SqlCommand"/> in a transaction and returns a new <see cref="XmlReader"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">
        /// <para>The <see cref="SqlCommand"/> to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="XmlReader"/> object.</para>
        /// </returns>
        public XmlReader ExecuteXmlReader(Transaction transaction, DbCommand command)
        {
            SqlCommand sqlCommand = CheckIfSqlCommand(command);
            if (transaction is Transaction) // don't do this if the transaction is null
                PrepareTransaction(transaction, sqlCommand);
            return DoExecuteXmlReader(sqlCommand);
        }

        /// <summary>
        /// <para>Executes the <see cref="SqlCommand"/> in a transaction and returns a new <see cref="XmlReader"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="IDbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">
        /// <para>The <see cref="SqlCommand"/> to execute.</para>
        /// </param>
        /// <returns>
        /// <para>An <see cref="XmlReader"/> object.</para>
        /// </returns>
        public XmlReader ExecuteXmlReader(DbTransaction transaction, DbCommand command)
        {
            SqlCommand sqlCommand = CheckIfSqlCommand(command);

            PrepareTransaction(transaction, sqlCommand);
            return DoExecuteXmlReader(sqlCommand);
        }

        /// <devdoc>
        /// Execute the actual XML Reader call.
        /// </devdoc>        
        private XmlReader DoExecuteXmlReader(SqlCommand sqlCommand)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                XmlReader reader = sqlCommand.ExecuteXmlReader();
                return reader;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static SqlCommand CheckIfSqlCommand(DbCommand command)
        {
            SqlCommand sqlCommand = command as SqlCommand;
            if (sqlCommand == null) throw new ArgumentException("The command is not a SQL Command object.");
            return sqlCommand;
        }

        /// <summary>
        /// Retrieves parameter information from the stored procedure specified in the <see cref="DbCommand"/> and populates the Parameters collection of the specified <see cref="DbCommand"/> object. 
        /// </summary>
        /// <param name="discoveryCommand">The <see cref="DbCommand"/> to do the discovery.</param>
        /// <remarks>The <see cref="DbCommand"/> must be a <see cref="SqlCommand"/> instance.</remarks>
        protected override void DeriveParameters(DbCommand discoveryCommand)
        {
            if (discoveryCommand.CommandType == CommandType.StoredProcedure)
                SqlCommandBuilder.DeriveParameters((SqlCommand)discoveryCommand);
            else
            {
                // Add the return parameter normally expected by stored procs
                discoveryCommand.Parameters.Add(CreateParameter(discoveryCommand, "@RETURN_VALUE"));
                discoveryCommand.Parameters[0].Direction = ParameterDirection.ReturnValue;

                // Find the parameters in the string based on the parameter token
                // param=@Param1 param=@ITems, and ignore things like @@Trancount
                // ?<! RegEx is a negative lookbehind to eliminate @@ paramaters
                string strSearch = @"(?<!" + ParameterToken + @")" + ParameterToken + @"[\w]+";
                MatchCollection matchParameters = Regex.Matches(discoveryCommand.CommandText, strSearch);
                foreach (Match paramMatch in matchParameters)
                {
                    // Only add the parameter if it doesn't exist yet
                    if (discoveryCommand.Parameters.IndexOf(paramMatch.Value) < 0)
                        discoveryCommand.Parameters.Add(CreateParameter(discoveryCommand, paramMatch.Value));
                }
            }
        }

        /// <summary>
        /// Returns the starting index for parameters in a command.
        /// </summary>
        /// <returns>The starting index for parameters in a command.</returns>
        protected override int UserParametersStartIndex()
        {
            return 1;
        }

        /// <summary>
        /// Builds a value parameter name for the current database.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>A correctly formated parameter name.</returns>
        public override string BuildParameterName(string name)
        {
            if (name[0] != this.ParameterToken)
            {
                return name.Insert(0, new string(this.ParameterToken, 1));
            }
            return name;
        }

        /// <summary>
        /// Determines if the number of parameters in the command matches the array of parameter values.
        /// </summary>
        /// <param name="command">The <see cref="DbCommand"/> containing the parameters.</param>
        /// <param name="values">The array of parameter values.</param>
        /// <returns><see langword="true"/> if the number of parameters and values match; otherwise, <see langword="false"/>.</returns>
        protected override bool SameNumberOfParametersAndValues(DbCommand command, object[] values)
        {
            int returnParameterCount = 1;
            int numberOfParametersToStoredProcedure = command.Parameters.Count - returnParameterCount;
            int numberOfValuesProvidedForStoredProcedure = values.Length;
            return numberOfParametersToStoredProcedure == numberOfValuesProvidedForStoredProcedure;
        }

        /// <summary>
        /// <para>Adds a new instance of a <see cref="DbParameter"/> object to the command.</para>
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>
        /// <param name="nullable"><para>A value indicating whether the parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual Basic) values.</para></param>
        /// <param name="precision"><para>The maximum number of digits used to represent the <paramref name="value"/>.</para></param>
        /// <param name="scale"><para>The number of decimal places to which <paramref name="value"/> is resolved.</para></param>
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>       
        public void AddParameter(DbCommand command, string name, SqlDbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            DbParameter parameter = CreateParameter(command, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// <para>Adds a new instance of a <see cref="DbParameter"/> object to the command.</para>
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="SqlDbType"/> values.</para></param>        
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>    
        public void AddParameter(DbCommand command, string name, SqlDbType dbType, ParameterDirection direction, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            AddParameter(command, name, dbType, 0, direction, false, 0, 0, sourceColumn, sourceVersion, value);
        }

        /// <summary>
        /// Adds a new Out <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the out parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="SqlDbType"/> values.</para></param>        
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>        
        public void AddOutParameter(DbCommand command, string name, SqlDbType dbType, int size)
        {
            AddParameter(command, name, dbType, size, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Default, DBNull.Value);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the in parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="SqlDbType"/> values.</para></param>                
        /// <remarks>
        /// <para>This version of the method is used when you can have the same parameter object multiple times with different values.</para>
        /// </remarks>        
        public void AddInParameter(DbCommand command, string name, SqlDbType dbType)
        {
            AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, null);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The commmand to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="SqlDbType"/> values.</para></param>                
        /// <param name="value"><para>The value of the parameter.</para></param>      
        public void AddInParameter(DbCommand command, string name, SqlDbType dbType, object value)
        {
            AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, value);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="SqlDbType"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the value.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        public void AddInParameter(DbCommand command, string name, SqlDbType dbType, string sourceColumn, DataRowVersion sourceVersion)
        {
            AddParameter(command, name, dbType, 0, ParameterDirection.Input, true, 0, 0, sourceColumn, sourceVersion, null);
        }

        /// <summary>
        /// <para>Adds a new instance of a <see cref="DbParameter"/> object.</para>
        /// </summary>
        /// <param name="command">The command to create the parameter for.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>
        /// <param name="nullable"><para>A value indicating whether the parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual Basic) values.</para></param>
        /// <param name="aPrecision"><para>The maximum number of digits used to represent the <paramref name="value"/>.</para></param>
        /// <param name="scale"><para>The number of decimal places to which <paramref name="value"/> is resolved.</para></param>
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>  
        internal DbParameter CreateParameter(DbCommand command, string name, SqlDbType dbType, int size, ParameterDirection direction, bool nullable, byte aPrecision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            SqlParameter param = CreateParameter(command, name) as SqlParameter;
            param.SqlDbType = dbType;
            param.Size = size;
            param.Value = (value == null) ? DBNull.Value : value;
            param.Direction = direction;
            param.IsNullable = nullable;
            param.SourceColumn = sourceColumn;
            param.SourceVersion = sourceVersion;
            return param;
        }
    }
}
