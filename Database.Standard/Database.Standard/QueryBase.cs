using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Transactions;
using Interstates.Control.Database.Properties;
using Interstates.Control.Framework.Security;

namespace Interstates.Control.Database
{
    /// <summary>
	/// Represents an abstract database that commands can be run against. 
	/// </summary>
	/// <remarks>
	/// The <see cref="QueryBase"/> class leverages the provider factory model from ADO.NET. A database instance holds 
	/// a reference to a concrete <see cref="DbProviderFactory"/> object to which it forwards the creation of ADO.NET objects.
	/// </remarks>
    [ComVisible(false)]
    public abstract class QueryBase : Interstates.Control.Database.IQuery
    {
        private const string PingData = "Ping Test. Are you there or not?"; // Send 32 bytes of data
        private const string NullValue = "null";

        private static ParameterCache _parameterCache = new ParameterCache();
        private DatabaseProfile _profile;
        private SecurityContext _contextSecurity = NullSecurityContext.Instance;
        private static byte[] _bytPingBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBase"/> class with a database profile <see cref="DatabaseProfile"/>.
        /// </summary>
        /// <param name="profile">The database profile that contains the connection to the database.</param>
        protected QueryBase(DatabaseProfile profile)
        {
            if (profile == null) throw new ArgumentNullException("The profile may not be NULL.");

            _profile = profile;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBase"/> class with a connection string and a <see cref="DbConnection"/>.
        /// </summary>
        /// <param name="connection">The connection to the database.</param>
        protected QueryBase(DbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("The Connection may not be NULL.");

            _profile = new DatabaseProfile();
            _profile.ConnectionString = connection.ConnectionString;
        }
        
        /// <summary>
		/// Initializes a new instance of the <see cref="QueryBase"/> class with a connection string./>.
		/// </summary>
		/// <param name="connectionString">The connection string for the database.</param>
		protected QueryBase(string connectionString)
		{
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("ConnectionString must be a valid string.");

            _profile = new DatabaseProfile();
            _profile.ConnectionString = connectionString;
		}

        /// <summary>
        /// Gets the <see cref="DatabaseProfile">database profile</see> used to connect to the database.
        /// </summary>
        public DatabaseProfile DatabaseProfile
        {
            get
            {
                return _profile;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="SecurityContext"/> used to Connect to the database.
        /// </summary>
        /// <value>
        /// The <see cref="SecurityContext"/> used to Connect to the database.
        /// </value>
        /// <remarks>
        /// <para>
        /// Unless a <see cref="SecurityContext"/> is specified here for the 
        /// default behavior is to use the security context of the current thread.
        /// </para>
        /// </remarks>
        public SecurityContext SecurityContext
        {
            get
            {
                return _contextSecurity;
            }
            set
            {
                _contextSecurity = value;
            }
        }
        
        /// <summary>
		/// <para>Gets the string used to open a database.</para>
		/// </summary>
		/// <value>
		/// <para>The string used to open a database.</para>
		/// </value>
		/// <seealso cref="DbConnection.ConnectionString"/>
		protected internal string ConnectionString
		{
			get
			{
				return _profile.ConnectionString;
			}
		}

        /// <summary>
        /// Create the connection that will be used to execute the query.
        /// </summary>
        /// <returns></returns>
        public virtual DbConnection CreateConnection()
        {
            DbConnection dbConnection = new OleDbConnection(DatabaseProfile.ConnectionString);
            return dbConnection;
        }

        /// <summary>
        /// Creates a data adapter for a provider.
        /// </summary>
        /// <returns>A <see cref="DbDataAdapter"/>.</returns>
        /// <seealso cref="DbDataAdapter"/>
        public virtual DbDataAdapter CreateDataAdapter()
        {
            return new OleDbDataAdapter();
        }

        /// <summary>
        /// Creates a command builder for a provider.
        /// </summary>
        /// <returns>A <see cref="DbDataAdapter"/>.</returns>
        /// <seealso cref="DbCommandBuilder"/>
        public virtual DbCommandBuilder CreateCommandBuilder()
        {
            return new OleDbCommandBuilder();
        }

        /// <summary>
        /// Creates a parameter for a provider.
        /// </summary>
        /// <returns></returns>
        public virtual DbParameter CreateParameter()
        {
            return new OleDbParameter();
        }

        /// <summary>
        /// Creates a parameter for a provider.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value to assign the parameter.</param>
        /// <returns></returns>
        public virtual DbParameter CreateParameter(string name, object value)
        {
            return new OleDbParameter(name, value);
        }

        /// <summary>
		/// <para>Creates a <see cref="DbCommand"/> for a stored procedure.</para>
		/// </summary>
		/// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
		/// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>       
		public virtual DbCommand CreateStoredProcedureCommand(string storedProcedureName)
		{
			if (string.IsNullOrEmpty(storedProcedureName)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "storedProcedureName");

			DbCommand command = CreateCommandByCommandType(CommandType.StoredProcedure, storedProcedureName);
            _parameterCache.SetParameters(command, this);
            return command;
		}

		/// <summary>
		/// <para>Creates a <see cref="DbCommand"/> for a stored procedure.</para>
		/// </summary>
		/// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
		/// <param name="parameterValues"><para>The list of parameters for the procedure.</para></param>
		/// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>
		/// <remarks>
		/// <para>The parameters for the stored procedure will be discovered and the values are assigned in positional order.</para>
		/// </remarks>        
		public virtual DbCommand CreateStoredProcedureCommand(string storedProcedureName, params object[] parameterValues)
		{
			if (string.IsNullOrEmpty(storedProcedureName)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "storedProcedureName");

            DbCommand command = CreateCommandByCommandType(CommandType.StoredProcedure, storedProcedureName);
            _parameterCache.SetParameters(command, this);
            AssignParameterValues(command, parameterValues);
            return command;
		}

		/// <summary>
		/// Wraps around a derived class's implementation of the CreateStoredProcedureCommandWrapper method and adds functionality for
		/// using this method with UpdateDataSet.  The GetStoredProcCommandWrapper method (above) that takes a params array 
		/// expects the array to be filled with VALUES for the parameters. This method differs from the GetStoredProcCommandWrapper 
		/// method in that it allows a user to pass in a string array. It will also dynamically discover the parameters for the 
		/// stored procedure and set the parameter's SourceColumns to the strings that are passed in. It does this by mapping 
		/// the parameters to the strings IN ORDER. Thus, order is very important.
		/// </summary>
		/// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
		/// <param name="sourceColumns"><para>The list of DataFields for the procedure.</para></param>
		/// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>
		public DbCommand CreateStoredProcedureCommandWithSourceColumns(string storedProcedureName, params string[] sourceColumns)
		{
			if (string.IsNullOrEmpty(storedProcedureName)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "storedProcedureName");
			if (sourceColumns == null) throw new ArgumentNullException("sourceColumns");

            DbCommand command = CreateStoredProcedureCommand(storedProcedureName);
            
            DiscoverParameters(command);

            int iSourceIndex = 0;
            foreach (IDataParameter dbParam in command.Parameters)
            {
                if ((dbParam.Direction == ParameterDirection.Input) | (dbParam.Direction == ParameterDirection.InputOutput))
                {
                    dbParam.SourceColumn = sourceColumns[iSourceIndex];
                    iSourceIndex++;
                }
            }

            return command;
		}

		/// <summary>
		/// <para>Creates a <see cref="DbCommand"/> for a SQL query.</para>
		/// </summary>
		/// <param name="query"><para>The text of the query.</para></param>        
		/// <returns><para>The <see cref="DbCommand"/> for the SQL query.</para></returns>        
		public DbCommand CreateTextCommand(string query)
		{
			if (string.IsNullOrEmpty(query)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "query");

			return CreateCommandByCommandType(CommandType.Text, query);
		}

        /// <summary>
        /// <para>Creates a <see cref="DbCommand"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="query"><para>The name of the Sql command.</para></param>
        /// <param name="parameterValues"><para>The list of parameters for the procedure.</para></param>
        /// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>
        /// <remarks>
        /// <para>The parameters for the stored procedure will be discovered and the values are assigned in positional order.</para>
        /// </remarks>        
        public DbCommand CreateTextCommand(string query, params object[] parameterValues)
        {
            if (string.IsNullOrEmpty(query)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "query");

            DbCommand command = CreateCommandByCommandType(CommandType.Text, query);

            if (parameterValues.Length > 0)
            {
                _parameterCache.SetParameters(command, this);
                AssignParameterValues(command, parameterValues);
            }
            return command;
        }

		/// <summary>
		/// Gets the DbDataAdapter with the given update behavior and connection from the proper derived class.
		/// </summary>
		/// <param name="updateBehavior">
		/// <para>One of the <see cref="UpdateBehavior"/> values.</para>
		/// </param>        
		/// <returns>A <see cref="DbDataAdapter"/>.</returns>
		/// <seealso cref="DbDataAdapter"/>
		protected DbDataAdapter CreateDataAdapter(UpdateBehavior updateBehavior)
		{
			DbDataAdapter dbAdapter = CreateDataAdapter();

			if (updateBehavior == UpdateBehavior.Continue)
			{
                this.SetUpRowUpdatedEvent(dbAdapter);
			}
            return dbAdapter;
		}

		/// <summary>
		/// Sets the RowUpdated event for the data adapter.
		/// </summary>
		/// <param name="adapter">The <see cref="DbDataAdapter"/> to set the event.</param>
		protected virtual void SetUpRowUpdatedEvent(DbDataAdapter adapter)
		{
		}

        /// <summary>
		/// <para>Executes the <paramref name="command"/> and adds a new <see cref="DataTable"></see> to the existing <see cref="DataSet"></see>.</para>
		/// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="dataSet">
		/// <para>The <see cref="DataSet"/> to load.</para>
		/// </param>
		/// <param name="tableName">
		/// <para>The name for the new <see cref="DataTable"/> to add to the <see cref="DataSet"/>.</para>
		/// </param>        
        /// <exception cref="System.ArgumentNullException">Any input parameter was <see langword="null"/> (<b>Nothing</b> in Visual Basic)</exception>
		/// <exception cref="System.ArgumentException">tableName was an empty string</exception>
		public virtual void LoadDataSet(DbCommand command, DataSet dataSet, string tableName)
		{
            LoadDataSet(command, dataSet, new string[] { tableName });
		}

		/// <summary>
		/// <para>Executes the <paramref name="command"/> within the given <paramref name="transaction" /> and adds a new <see cref="DataTable"></see> to the existing <see cref="DataSet"></see>.</para>
		/// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>        
        /// <param name="dataSet">
		/// <para>The <see cref="DataSet"/> to load.</para>
		/// </param>
		/// <param name="tableName">
		/// <para>The name for the new <see cref="DataTable"/> to add to the <see cref="DataSet"/>.</para>
		/// </param>
		/// <exception cref="System.ArgumentNullException">Any input parameter was <see langword="null"/> (<b>Nothing</b> in Visual Basic).</exception>
		/// <exception cref="System.ArgumentException">tableName was an empty string.</exception>
        public virtual void LoadDataSet(DbTransaction transaction, DbCommand command, DataSet dataSet, string tableName)
		{
            PrepareTransaction(transaction, command);
            LoadDataSet(command, dataSet, new string[] { tableName });
		}

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within the given <paramref name="transaction" /> and adds a new <see cref="DataTable"></see> to the existing <see cref="DataSet"></see>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>        
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to load.</para>
        /// </param>
        /// <param name="tableName">
        /// <para>The name for the new <see cref="DataTable"/> to add to the <see cref="DataSet"/>.</para>
        /// </param>
        /// <exception cref="System.ArgumentNullException">Any input parameter was <see langword="null"/> (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.ArgumentException">tableName was an empty string.</exception>
        public virtual void LoadDataSet(Transaction transaction, DbCommand command, DataSet dataSet, string tableName)
        {
            if (transaction != null) // don't do this if the transaction is null
                PrepareTransaction(transaction, command);
            LoadDataSet(command, dataSet, new string[] { tableName });
        }

		/// <summary>
		/// <para>Loads a <see cref="DataSet"/> from a <see cref="DbCommand"/>.</para>
		/// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="dataSet">
		/// <para>The <see cref="DataSet"/> to fill.</para>
		/// </param>
		/// <param name="tableNames">
		/// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
		/// </param>
		public virtual void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames)
		{
            // If the command connection is already open don't create a new connection.
            if (command.Connection == null)
                command.Connection = OpenNewConnection();
            else if (command.Connection.State != ConnectionState.Open)
                OpenConnection(command.Connection);
            DoLoadDataSet(command, dataSet, tableNames);
		}

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> from a <see cref="DbCommand"/> in  a transaction.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command in.</para>
        /// </param>
        /// <param name="command">
        /// <para>The <see cref="DbCommand"> to execute.</see>/></para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        public virtual void LoadDataSet(DbTransaction transaction, DbCommand command, DataSet dataSet, string[] tableNames)
        {
            PrepareTransaction(transaction, command);
            LoadDataSet(command, dataSet, tableNames);
        }

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> from a <see cref="DbCommand"/> in  a transaction.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command in.</para>
        /// </param>
        /// <param name="command">
        /// <para>The <see cref="DbCommand"> to execute.</see>/></para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        public virtual void LoadDataSet(Transaction transaction, DbCommand command, DataSet dataSet, string[] tableNames)
        {
            if (transaction != null) // don't do this if the transaction is null
                PrepareTransaction(transaction, command);
            LoadDataSet(command, dataSet, tableNames);
        }

		/// <summary>
		/// <para>Loads a <see cref="DataSet"/> with the results returned from a stored procedure.</para>
		/// </summary>
		/// <param name="storedProcedureName">
		/// <para>The stored procedure name to execute.</para>
		/// </param>
		/// <param name="dataSet">
		/// <para>The <see cref="DataSet"/> to fill.</para>
		/// </param>
		/// <param name="tableNames">
		/// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
		/// </param>
		/// <param name="parameterValues">
		/// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
		/// </param>
		public virtual void LoadDataSet(string storedProcedureName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
		{
            using (DbCommand command = CreateStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                try
                {
                    LoadDataSet(command, dataSet, tableNames);
                }
                finally
                {
                    command.Connection.Dispose();
                }
            }
		}

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> with the results returned from a stored procedure executed in a transaction.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the stored procedure in.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure name to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        public virtual void LoadDataSet(DbTransaction transaction, string storedProcedureName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            using (DbCommand command = CreateStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                PrepareTransaction(transaction, command);
                LoadDataSet(command, dataSet, tableNames);
            }
        }

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> with the results returned from a stored procedure executed in a transaction.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the stored procedure in.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure name to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        public virtual void LoadDataSet(Transaction transaction, string storedProcedureName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            using (DbCommand command = CreateStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                if (transaction != null) // don't do this if the transaction is null
                    PrepareTransaction(transaction, command);
                LoadDataSet(command, dataSet, tableNames);
            }
        }

        /// <summary>
		/// <para>Loads a <see cref="DataSet"/> from command text.</para>
		/// </summary>
		/// <param name="commandType">
		/// <para>One of the <see cref="CommandType"/> values.</para>
		/// </param>
		/// <param name="commandText">
		/// <para>The command text to execute.</para>
		/// </param>
		/// <param name="dataSet">
		/// <para>The <see cref="DataSet"/> to fill.</para>
		/// </param>
		/// <param name="tableNames">
		/// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
		/// </param>
		public virtual void LoadDataSet(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
		{
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                try
                {
                    LoadDataSet(command, dataSet, tableNames);
                }
                finally
                {
                    command.Connection.Dispose();
                }
            }
		}

		/// <summary>
		/// <para>Loads a <see cref="DataSet"/> from command text in a transaction.</para>
		/// </summary>
		/// <param name="transaction">
		/// <para>The <see cref="DbTransaction"/> to execute the command in.</para>
		/// </param>
		/// <param name="commandType">
		/// <para>One of the <see cref="CommandType"/> values.</para>
		/// </param>
		/// <param name="commandText">
		/// <para>The command text to execute.</para>
		/// </param>
		/// <param name="dataSet">
		/// <para>The <see cref="DataSet"/> to fill.</para>
		/// </param>
		/// <param name="tableNames">
		/// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
		/// </param>
		public virtual void LoadDataSet(DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
		{
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                PrepareTransaction(transaction, command);
                DoLoadDataSet(command, dataSet, tableNames);
            }
		}

        /// <summary>
        /// <para>Loads a <see cref="DataSet"/> from command text in a transaction.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command in.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="dataSet">
        /// <para>The <see cref="DataSet"/> to fill.</para>
        /// </param>
        /// <param name="tableNames">
        /// <para>An array of table name mappings for the <see cref="DataSet"/>.</para>
        /// </param>
        public virtual void LoadDataSet(Transaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                if (transaction != null) // don't do this if the transaction is null
                    PrepareTransaction(transaction, command);
                DoLoadDataSet(command, dataSet, tableNames);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        public virtual DataSet Execute(DbCommand command)
        {
            DataSet dataSet = new DataSet();

            dataSet.Locale = CultureInfo.InvariantCulture;
            LoadDataSet(command, dataSet, "Table");

            return dataSet;
        }

        /// <summary>
		/// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/>.</para>
		/// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
		/// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        public virtual DataSet Execute(DbTransaction transaction, DbCommand command)
		{
            PrepareTransaction(transaction, command);
            return Execute(command);
		}

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        public virtual DataSet Execute(Transaction transaction, DbCommand command)
        {
            if (transaction != null) // don't do this if the transaction is null
                PrepareTransaction(transaction, command);
            return Execute(command);
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        public virtual DataSet Execute(DbCommand command, params object[] parameterValues)
        {
            AssignParameterValues(command, parameterValues);
            return Execute(command);
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <param name="parameterValues">
		/// <para>An array of parameters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
		/// </param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        public virtual DataSet Execute(Transaction transaction, DbCommand command, params object[] parameterValues)
        {
            if (transaction != null) // don't do this if the transaction is null
                PrepareTransaction(transaction, command);
            return Execute(command, parameterValues);        
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        public virtual DataSet Execute(DbTransaction transaction, DbCommand command, params object[] parameterValues)
        {
            PrepareTransaction(transaction, command);
            return Execute(command, parameterValues);
        }

		/// <summary>
		/// <para>Executes the <paramref name="storedProcedureName"/> with <paramref name="parameterValues" /> and returns the results in a new <see cref="DataSet"/>.</para>
		/// </summary>
		/// <param name="storedProcedureName">
		/// <para>The stored procedure to execute.</para>
		/// </param>
		/// <param name="parameterValues">
		/// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
		/// </param>
		/// <returns>
		/// <para>A <see cref="DataSet"/> with the results of the <paramref name="storedProcedureName"/>.</para>
		/// </returns>
		public virtual DataSet Execute(string storedProcedureName, params object[] parameterValues)
		{
            using (DbCommand command = CreateStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                try
                {
                    return Execute(command);
                }
                finally
                {
                    command.Connection.Dispose();
                }
            }
		}

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> with <paramref name="parameterValues" /> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/> within a transaction.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="storedProcedureName"/>.</para>
        /// </returns>
        public virtual DataSet Execute(DbTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            using (DbCommand command = CreateStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                return Execute(transaction, command);
            }
        }

		/// <summary>
		/// <para>Executes the <paramref name="storedProcedureName"/> with <paramref name="parameterValues" /> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/> within a transaction.</para>
		/// </summary>
		/// <param name="transaction">
		/// <para>The <see cref="Transaction"/> to execute the command within.</para>
		/// </param>
		/// <param name="storedProcedureName">
		/// <para>The stored procedure to execute.</para>
		/// </param>
		/// <param name="parameterValues">
		/// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
		/// </param>
		/// <returns>
		/// <para>A <see cref="DataSet"/> with the results of the <paramref name="storedProcedureName"/>.</para>
		/// </returns>
		public virtual DataSet Execute(Transaction transaction, string storedProcedureName, params object[] parameterValues)
		{
            using (DbCommand command = CreateStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                return Execute(transaction, command);
            }
		}

		/// <summary>
		/// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> and returns the results in a new <see cref="DataSet"/>.</para>
		/// </summary>
		/// <param name="commandType">
		/// <para>One of the <see cref="CommandType"/> values.</para>
		/// </param>
		/// <param name="commandText">
		/// <para>The command text to execute.</para>
		/// </param>
		/// <returns>
		/// <para>A <see cref="DataSet"/> with the results of the <paramref name="commandText"/>.</para>
		/// </returns>
		public virtual DataSet Execute(CommandType commandType, string commandText)
		{
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                try
                {
                    return Execute(command);
                }
                finally
                {
                    command.Connection.Dispose();
                }
            }
		}

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> as part of the given <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        public virtual DataSet Execute(DbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return Execute(transaction, command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> as part of the given <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        public virtual DataSet Execute(Transaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return Execute(transaction, command);
            }
        }


        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DbDataReader"/>.</para>
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="behavior">The behavior to execute the command with.</param>
        /// <returns>A <see cref="DbDataReader"/> with the results of the <paramref name="command"/>.</returns>        
        public virtual DbDataReader ExecuteReader(DbCommand command, CommandBehavior behavior = CommandBehavior.Default)
        {
            // If the command connection is already open don't create a new connection.
            if (command.Connection == null)
                command.Connection = OpenNewConnection();
            else if (command.Connection.State != ConnectionState.Open)
                OpenConnection(command.Connection);
            return command.ExecuteReader(behavior);
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DbDataReader"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <param name="behavior">The behavior to execute the command with.</param>
        /// <returns>A <see cref="DbDataReader"/> with the results of the <paramref name="command"/>.</returns>        
        public virtual DbDataReader ExecuteReader(DbTransaction transaction, DbCommand command, CommandBehavior behavior = CommandBehavior.Default)
        {
            PrepareTransaction(transaction, command);
            return ExecuteReader(command, behavior);
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DbDataReader"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <param name="behavior">The behavior to execute the command with.</param>
        /// <returns>A <see cref="DbDataReader"/> with the results of the <paramref name="command"/>.</returns>        
        public virtual DbDataReader ExecuteReader(Transaction transaction, DbCommand command, CommandBehavior behavior = CommandBehavior.Default)
        {
            if (transaction != null) // don't do this if the transaction is null
                PrepareTransaction(transaction, command);
            return ExecuteReader(command, behavior);
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DbDataReader"/>.</para>
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="behavior">The behavior to execute the command with.</param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <returns>A <see cref="DbDataReader"/> with the results of the <paramref name="command"/>.</returns>        
        public virtual DbDataReader ExecuteReader(DbCommand command, CommandBehavior behavior = CommandBehavior.Default, params object[] parameterValues)
        {
            AssignParameterValues(command, parameterValues);
            return ExecuteReader(command, behavior);
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DbDataReader"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <param name="behavior">The behavior to execute the command with.</param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <returns>A <see cref="DbDataReader"/> with the results of the <paramref name="command"/>.</returns>        
        public virtual DbDataReader ExecuteReader(Transaction transaction, DbCommand command, CommandBehavior behavior = CommandBehavior.Default, params object[] parameterValues)
        {
            if (transaction != null) // don't do this if the transaction is null
                PrepareTransaction(transaction, command);
            return ExecuteReader(command, behavior, parameterValues);
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DbDataReader"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <param name="behavior">The behavior to execute the command with.</param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <returns>A <see cref="DbDataReader"/> with the results of the <paramref name="command"/>.</returns>        
        public virtual DbDataReader ExecuteReader(DbTransaction transaction, DbCommand command, CommandBehavior behavior = CommandBehavior.Default, params object[] parameterValues)
        {
            PrepareTransaction(transaction, command);
            return ExecuteReader(command, behavior, parameterValues);
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> and returns the results in a new <see cref="DbDataReader"/>.</para>
        /// </summary>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <param name="behavior">The behavior to execute the command with.</param>
        /// <para>A <see cref="DbDataReader"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        public virtual DbDataReader ExecuteReader(CommandType commandType, string commandText, CommandBehavior behavior = CommandBehavior.Default)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteReader(command, behavior);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> as part of the given <paramref name="transaction" /> and returns the results in a new <see cref="DbDataReader"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <param name="behavior">The behavior to execute the command with.</param>
        /// <para>A <see cref="DbDataReader"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        public virtual DbDataReader ExecuteReader(DbTransaction transaction, CommandType commandType, string commandText, CommandBehavior behavior = CommandBehavior.Default)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteReader(transaction, command, behavior);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> as part of the given <paramref name="transaction" /> and returns the results in a new <see cref="DbDataReader"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <param name="behavior">The behavior to execute the command with.</param>
        /// <para>A <see cref="DbDataReader"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        public virtual DbDataReader ExecuteReader(Transaction transaction, CommandType commandType, string commandText, CommandBehavior behavior = CommandBehavior.Default)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteReader(transaction, command, behavior);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> with <paramref name="parameterValues" /> and returns the results in a new <see cref="DataTable"/>.</para>
        /// </summary>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataTable"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        public virtual DataTable ExecuteText(string commandText, params object[] parameterValues)
        {
            using (DbCommand command = CreateTextCommand(commandText, parameterValues))
            {
                try
                {
                    return Execute(command).Tables[0];
                }
                finally
                {
                    command.Connection.Dispose();
                }
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> with <paramref name="parameterValues" /> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataTable"/> within a transaction.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataTable"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        public virtual DataTable ExecuteText(DbTransaction transaction, string commandText, params object[] parameterValues)
        {
            using (DbCommand command = CreateTextCommand(commandText, parameterValues))
            {
                return Execute(transaction, command).Tables[0];
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> with <paramref name="parameterValues" /> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataTable"/> within a transaction.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataTable"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        public virtual DataTable ExecuteText(Transaction transaction, string commandText, params object[] parameterValues)
        {
            using (DbCommand command = CreateTextCommand(commandText, parameterValues))
            {
                return Execute(transaction, command).Tables[0];
            }
        }

		/// <summary>
		/// <para>Executes the <paramref name="command"/> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
		/// </summary>
		/// <param name="command">
		/// <para>The command that contains the query to execute.</para>
		/// </param>
		/// <returns>
		/// <para>The first column of the first row in the result set.</para>
		/// </returns>
		/// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(DbCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");

            return DoExecuteScalar(command);
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within a <paramref name="transaction" />, and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(DbTransaction transaction, DbCommand command)
        {
            PrepareTransaction(transaction, command);
            return ExecuteScalar(command);
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within a <paramref name="transaction" />, and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(Transaction transaction, DbCommand command)
        {
            if (transaction != null) // don't do this if the transaction is null
                PrepareTransaction(transaction, command);
            return ExecuteScalar(command);
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>        
        public virtual object ExecuteScalar(DbCommand command, params object[] parameterValues)
        {
            AssignParameterValues(command, parameterValues);
            return ExecuteScalar(command);
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within a <paramref name="transaction" />, and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(Transaction transaction, DbCommand command, params object[] parameterValues)
        {
            if (transaction != null) // don't do this if the transaction is null
                PrepareTransaction(transaction, command);
            return ExecuteScalar(command, parameterValues);        
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within a <paramref name="transaction" />, and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(DbTransaction transaction, DbCommand command, params object[] parameterValues)
        {
            PrepareTransaction(transaction, command);
            return ExecuteScalar(command, parameterValues);
        }

		/// <summary>
		/// <para>Executes the <paramref name="storedProcedureName"/> with the given <paramref name="parameterValues" /> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
		/// </summary>
		/// <param name="storedProcedureName">
		/// <para>The stored procedure to execute.</para>
		/// </param>
		/// <param name="parameterValues">
		/// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
		/// </param>
		/// <returns>
		/// <para>The first column of the first row in the result set.</para>
		/// </returns>
		/// <seealso cref="IDbCommand.ExecuteScalar"/>
		public virtual object ExecuteScalar(string storedProcedureName, params object[] parameterValues)
		{
            using (DbCommand command = CreateStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                try
                {
                    return ExecuteScalar(command);
                }
                finally
                {
                    command.Connection.Dispose();
                }
            }
		}

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> with the given <paramref name="parameterValues" /> within a 
        /// <paramref name="transaction" /> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(DbTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            using (DbCommand command = CreateStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                return ExecuteScalar(transaction, command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> with the given <paramref name="parameterValues" /> within a 
        /// <paramref name="transaction" /> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(Transaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            using (DbCommand command = CreateStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                return ExecuteScalar(transaction, command);
            }
        }

		/// <summary>
		/// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" />  and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
		/// </summary>
		/// <param name="commandType">
		/// <para>One of the <see cref="CommandType"/> values.</para>
		/// </param>
		/// <param name="commandText">
		/// <para>The command text to execute.</para>
		/// </param>
		/// <returns>
		/// <para>The first column of the first row in the result set.</para>
		/// </returns>
		/// <seealso cref="IDbCommand.ExecuteScalar"/>
		public virtual object ExecuteScalar(CommandType commandType, string commandText)
		{
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                try
                {
                    return ExecuteScalar(command);
                }
                finally
                {
                    command.Connection.Dispose();
                }
            }
		}

		/// <summary>
		/// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> 
		/// within the given <paramref name="transaction" /> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
		/// </summary>
		/// <param name="transaction">
		/// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
		/// </param>
		/// <param name="commandType">
		/// <para>One of the <see cref="CommandType"/> values.</para>
		/// </param>
		/// <param name="commandText">
		/// <para>The command text to execute.</para>
		/// </param>
		/// <returns>
		/// <para>The first column of the first row in the result set.</para>
		/// </returns>
		/// <seealso cref="IDbCommand.ExecuteScalar"/>
		public virtual object ExecuteScalar(DbTransaction transaction, CommandType commandType, string commandText)
		{
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteScalar(transaction, command);
            }
		}

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> 
        /// within the given <paramref name="transaction" /> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual object ExecuteScalar(Transaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                return ExecuteScalar(transaction, command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> with the given <paramref name="parameterValues" /> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        public virtual object ExecuteTextScalar(string commandText, params object[] parameterValues)
        {
            using (DbCommand command = CreateTextCommand(commandText, parameterValues))
            {
                try
                {
                    return ExecuteScalar(command);
                }
                finally
                {
                    command.Connection.Dispose();
                }
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> with the given <paramref name="parameterValues" /> within a 
        /// <paramref name="transaction" /> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        public virtual object ExecuteTextScalar(DbTransaction transaction, string commandText, params object[] parameterValues)
        {
            using (DbCommand command = CreateTextCommand(commandText, parameterValues))
            {
                return ExecuteScalar(transaction, command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> with the given <paramref name="parameterValues" /> within a 
        /// <paramref name="transaction" /> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        public virtual object ExecuteTextScalar(Transaction transaction, string commandText, params object[] parameterValues)
        {
            using (DbCommand command = CreateTextCommand(commandText, parameterValues))
            {
                return ExecuteScalar(transaction, command);
            }
        }

		/// <summary>
		/// <para>Executes the <paramref name="command"/> and returns the number of rows affected.</para>
		/// </summary>
		/// <param name="command">
		/// <para>The command that contains the query to execute.</para>
		/// </param>       
		/// <seealso cref="IDbCommand.ExecuteScalar"/>
		public virtual int ExecuteNonQuery(DbCommand command)
		{
			return DoExecuteNonQuery(command);
		}

		/// <summary>
		/// <para>Executes the <paramref name="command"/> within the given <paramref name="transaction" />, and returns the number of rows affected.</para>
		/// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
		/// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(DbTransaction transaction, DbCommand command)
		{
            PrepareTransaction(transaction, command);
			return DoExecuteNonQuery(command);
		}

        /// <summary>
        /// <para>Executes the <paramref name="command"/> within the given <paramref name="transaction" />, and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(Transaction transaction, DbCommand command)
        {
            if (transaction != null) // don't do this if the transaction is null
                PrepareTransaction(transaction, command);
            return DoExecuteNonQuery(command);
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(DbCommand command, params object[] parameterValues)
        {
            AssignParameterValues(command, parameterValues);
            return DoExecuteNonQuery(command);
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction"/> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(Transaction transaction, DbCommand command, params object[] parameterValues)
        {
            if (transaction != null) // don't do this if the transaction is null
                PrepareTransaction(transaction, command);
            return ExecuteNonQuery(command, parameterValues);
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction"/> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(DbTransaction transaction, DbCommand command, params object[] parameterValues)
        {
            PrepareTransaction(transaction, command);
            return ExecuteNonQuery(command, parameterValues);
        }

		/// <summary>
		/// <para>Executes the <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues" /> and returns the number of rows affected.</para>
		/// </summary>
		/// <param name="storedProcedureName">
		/// <para>The name of the stored procedure to execute.</para>
		/// </param>
		/// <param name="parameterValues">
		/// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
		/// </param>
		/// <returns>
		/// <para>The number of rows affected</para>
		/// </returns>
		/// <seealso cref="IDbCommand.ExecuteScalar"/>
		public virtual int ExecuteNonQuery(string storedProcedureName, params object[] parameterValues)
		{
            using (DbCommand command = CreateStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                try
                {
                    return DoExecuteNonQuery(command);
                }
                finally
                {
                    command.Connection.Dispose();
                }
            }
		}

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues" /> within a transaction and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The name of the stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected.</para>
        /// </returns>
        /// <seealso cref="DbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(DbTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            using (DbCommand command = CreateStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                PrepareTransaction(transaction, command);
                return DoExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues" /> within a transaction and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="storedProcedureName">
        /// <para>The name of the stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected.</para>
        /// </returns>
        /// <seealso cref="DbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(Transaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            using (DbCommand command = CreateStoredProcedureCommand(storedProcedureName, parameterValues))
            {
                if (transaction != null) // don't do this if the transaction is null
                    PrepareTransaction(transaction, command);
                return DoExecuteNonQuery(command);
            }
        }

		/// <summary>
		/// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> and returns the number of rows affected.</para>
		/// </summary>
		/// <param name="commandType">
		/// <para>One of the <see cref="CommandType"/> values.</para>
		/// </param>
		/// <param name="commandText">
		/// <para>The command text to execute.</para>
		/// </param>
		/// <returns>
		/// <para>The number of rows affected.</para>
		/// </returns>
		/// <seealso cref="IDbCommand.ExecuteScalar"/>
		public virtual int ExecuteNonQuery(CommandType commandType, string commandText)
		{
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                try
                {
                    return DoExecuteNonQuery(command);
                }
                finally
                {
                    command.Connection.Dispose();
                }
            }
		}

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> as part of the given <paramref name="transaction" /> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(DbTransaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                PrepareTransaction(transaction, command);
                return DoExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> interpreted as specified by the <paramref name="commandType" /> as part of the given <paramref name="transaction" /> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandType">
        /// <para>One of the <see cref="CommandType"/> values.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected</para>
        /// </returns>
        /// <seealso cref="DbCommand.ExecuteScalar"/>
        public virtual int ExecuteNonQuery(Transaction transaction, CommandType commandType, string commandText)
        {
            using (DbCommand command = CreateCommandByCommandType(commandType, commandText))
            {
                if (transaction != null) // don't do this if the transaction is null
                    PrepareTransaction(transaction, command);
                return DoExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> using the given <paramref name="parameterValues" /> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="commandText">
        /// <para>The name of the command text to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the command text. The parameter values must be in call order as they appear in the command text.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        public virtual int ExecuteTextNonQuery(string commandText, params object[] parameterValues)
        {
            using (DbCommand command = CreateTextCommand(commandText, parameterValues))
            {
                try
                {
                    return DoExecuteNonQuery(command);
                }
                finally
                {
                    command.Connection.Dispose();
                }
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> using the given <paramref name="parameterValues" /> within a transaction and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The name of the command text to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the command text. The parameter values must be in call order as they appear in the command text.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected.</para>
        /// </returns>
        /// <seealso cref="DbCommand.ExecuteScalar"/>
        public virtual int ExecuteTextNonQuery(DbTransaction transaction, string commandText, params object[] parameterValues)
        {
            using (DbCommand command = CreateTextCommand(commandText, parameterValues))
            {
                PrepareTransaction(transaction, command);
                return DoExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> using the given <paramref name="parameterValues" /> within a transaction and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The name of the command text to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of parameters to pass to the command text. The parameter values must be in call order as they appear in the command text.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected.</para>
        /// </returns>
        /// <seealso cref="DbCommand.ExecuteScalar"/>
        public virtual int ExecuteTextNonQuery(Transaction transaction, string commandText, params object[] parameterValues)
        {
            using (DbCommand command = CreateTextCommand(commandText, parameterValues))
            {
                if (transaction != null) // don't do this if the transaction is null
                    PrepareTransaction(transaction, command);
                return DoExecuteNonQuery(command);
            }
        }

		/// <summary>
		/// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/>.</para>
		/// </summary>        
		/// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
		/// <param name="tableName"><para>The name of the source table to use for table mapping.</para></param>
		/// <param name="insertCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Added"/></para></param>
		/// <param name="updateCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Modified"/></para></param>        
		/// <param name="deleteCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Deleted"/></para></param>        
		/// <param name="updateBehavior"><para>One of the <see cref="UpdateBehavior"/> values.</para></param>
		/// <returns>number of records affected</returns>        
		public int UpdateDataSet(DataSet dataSet, string tableName,
								    DbCommand insertCommand, DbCommand updateCommand,
								    DbCommand deleteCommand, UpdateBehavior updateBehavior)
		{
			using (DbConnection connection = OpenNewConnection())
			{
				if (updateBehavior == UpdateBehavior.Transactional)
				{
					DbTransaction trans = BeginDbTransaction(connection);
					try
					{
                        using (DbDataAdapter adapter = CreateDataAdapter(updateBehavior))
                        {
                            PrepareDataAdapter(trans, adapter, insertCommand, updateCommand, deleteCommand);
                            int rowsAffected = DoUpdateDataSet(dataSet, tableName, adapter);
                            trans.Commit();
                            return rowsAffected;
                        }
					}
					catch
					{
						trans.Rollback();
						throw;
					}
				}
				else
				{
                    using (DbDataAdapter adapter = CreateDataAdapter(updateBehavior))
                    {
                        PrepareDataAdapter(adapter, connection, insertCommand, updateCommand, deleteCommand);
					    return DoUpdateDataSet(dataSet, tableName, adapter );
                    }
                }
			}
		}

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/> within a transaction.</para>
        /// </summary>        
        /// <param name="transaction"><para>The <see cref="DbTransaction"/> to use.</para></param>
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="tableName"><para>The name of the source table to use for table mapping.</para></param>
        /// <param name="insertCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Added"/>.</para></param>        
        /// <param name="updateCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Modified"/>.</para></param>        
        /// <param name="deleteCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Deleted"/>.</para></param>
        /// <returns>Number of records affected.</returns>        
        public int UpdateDataSet(DbTransaction transaction, DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand)
        {
            using (DbDataAdapter adapter = CreateDataAdapter(UpdateBehavior.Transactional))
            {
                PrepareDataAdapter(transaction, adapter, insertCommand, updateCommand, deleteCommand);
                return DoUpdateDataSet(dataSet, tableName, adapter);
            }
        }

        /// <summary>
        /// <para>Calls the respective INSERT, UPDATE, or DELETE statements for each inserted, updated, or deleted row in the <see cref="DataSet"/> within a transaction.</para>
        /// </summary>        
        /// <param name="transaction"><para>The <see cref="Transaction"/> to use.</para></param>
        /// <param name="dataSet"><para>The <see cref="DataSet"/> used to update the data source.</para></param>
        /// <param name="tableName"><para>The name of the source table to use for table mapping.</para></param>
        /// <param name="insertCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Added"/>.</para></param>        
        /// <param name="updateCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Modified"/>.</para></param>        
        /// <param name="deleteCommand"><para>The <see cref="DbCommand"/> executed when <see cref="DataRowState"/> is <seealso cref="DataRowState.Deleted"/>.</para></param>
        /// <returns>Number of records affected.</returns>        
        public int UpdateDataSet(Transaction transaction, DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand)
        {
            using (DbDataAdapter adapter = CreateDataAdapter(UpdateBehavior.Transactional))
            {
                DbTransaction tx = TransactionManager.Instance.GetTransaction(transaction) as DbTransaction;

                if (tx != null)
                    PrepareDataAdapter(tx, adapter, insertCommand, updateCommand, deleteCommand);
                return DoUpdateDataSet(dataSet, tableName, adapter);
            }
        }

        /// <summary>
        /// Update the dataset using the default insert, update and delete commands built from the command builder.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <param name="tableName">The database table to update.  NOTE: The table name must also match the name of the table in the dataset.</param>
        /// <returns></returns>
        public virtual int UpdateDataSet(DataSet dataSet, string tableName)
        {
            DbCommandBuilder dbCommands = CreateCommandBuilder();

            using (DbDataAdapter dbAdapter = CreateDataAdapter())
            {
                dbCommands.DataAdapter = dbAdapter;
                dbAdapter.SelectCommand = this.CreateTextCommand(string.Format("SELECT * FROM [{0}]", tableName));
                PrepareDataAdapter(dbAdapter, dbAdapter.SelectCommand.Connection,
                    dbCommands.GetInsertCommand(true),
                    dbCommands.GetUpdateCommand(true),
                    dbCommands.GetDeleteCommand(true));
                return DoUpdateDataSet(dataSet, tableName, dbAdapter);
            }
        }

        /// <summary>
        /// Update the dataset using the default insert, update and delete commands built from the command builder.
        /// </summary>
        /// <param name="transaction"><para>The <see cref="DbTransaction"/> to use.</para></param>
        /// <param name="dataSet">The dataset to update.</param>
        /// <param name="tableName">The database table to update.  NOTE: The table name must also match the name of the table in the dataset.</param>
        /// <returns></returns>
        public virtual int UpdateDataSet(DbTransaction transaction, DataSet dataSet, string tableName)
        {
            DbCommandBuilder dbCommands = CreateCommandBuilder();

            using (DbDataAdapter dbAdapter = CreateDataAdapter())
            {
                dbCommands.DataAdapter = dbAdapter;
                dbAdapter.SelectCommand = this.CreateTextCommand(string.Format("SELECT * FROM [{0}]", tableName));
                PrepareTransaction(transaction, dbAdapter.SelectCommand);
                PrepareDataAdapter(transaction, dbAdapter, dbCommands.GetInsertCommand(true), dbCommands.GetUpdateCommand(true), dbCommands.GetDeleteCommand(true)); 

                return DoUpdateDataSet(dataSet, tableName, dbAdapter);
            }
        }

        /// <summary>
        /// Update the dataset using the default insert, update and delete commands built from the command builder.
        /// </summary>
        /// <param name="transaction"><para>The <see cref="Transaction"/> to use.</para></param>
        /// <param name="dataSet">The dataset to update.</param>
        /// <param name="tableName">The database table to update.  NOTE: The table name must also match the name of the table in the dataset.</param>
        /// <returns></returns>
        public virtual int UpdateDataSet(Transaction transaction, DataSet dataSet, string tableName)
        {
            DbCommandBuilder dbCommands = CreateCommandBuilder();

            using (DbDataAdapter dbAdapter = CreateDataAdapter())
            {
                dbCommands.DataAdapter = dbAdapter;
                dbAdapter.SelectCommand = this.CreateTextCommand(string.Format("SELECT * FROM [{0}]", tableName));
                DbTransaction tx = TransactionManager.Instance.GetTransaction(transaction) as DbTransaction;

                if (tx != null)
                {
                    PrepareTransaction(tx, dbAdapter.SelectCommand);
                    PrepareDataAdapter(tx, dbAdapter, dbCommands.GetInsertCommand(true), dbCommands.GetUpdateCommand(true), dbCommands.GetDeleteCommand(true));
                }
                return DoUpdateDataSet(dataSet, tableName, dbAdapter);
            }
        }

        /// <summary>
        /// Update the dataset using the default insert, update and delete commands built from the command builder.
        /// </summary>
        /// <param name="dataSet">The dataset to update.</param>
        /// <param name="tableName">The database table to update.  NOTE: The table name must also match the name of the table in the dataset.</param>
        /// <param name="updateAdapter"><para>The <see cref="DbDataAdapter"/> to use.</para></param>
        /// <returns></returns>
        public int UpdateDataSet(DataSet dataSet, string tableName, DbDataAdapter updateAdapter)
        {
            return DoUpdateDataSet(dataSet, tableName, updateAdapter);
        }

        protected void PrepareDataAdapter(DbDataAdapter adapter, DbConnection connection, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand)
        {
            if (insertCommand != null)
            {
                insertCommand.Connection = connection;
                adapter.InsertCommand = insertCommand;
            }
            if (updateCommand != null)
            {
                updateCommand.Connection = connection;
                adapter.UpdateCommand = updateCommand;
            }
            if (deleteCommand != null)
            {
                deleteCommand.Connection = connection;
                adapter.DeleteCommand = deleteCommand;
            }
        }

        protected void PrepareDataAdapter(DbTransaction transaction, DbDataAdapter adapter, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand)
        {
            if (insertCommand != null)
            {
                PrepareTransaction(transaction, insertCommand);
                adapter.InsertCommand = insertCommand;
            }
            if (updateCommand != null)
            {
                PrepareTransaction(transaction, updateCommand);
                adapter.UpdateCommand = updateCommand;
            }
            if (deleteCommand != null)
            {
                PrepareTransaction(transaction, deleteCommand);
                adapter.DeleteCommand = deleteCommand;
            }
        }

        /// <summary>
		/// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
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
		public virtual void AddParameter(DbCommand command, string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
		{
			DbParameter parameter = CreateParameter(command, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
			command.Parameters.Add(parameter);
		}

		/// <summary>
		/// <para>Adds a new instance of a <see cref="DbParameter"/> object to the command.</para>
		/// </summary>
		/// <param name="command">The command to add the parameter.</param>
		/// <param name="name"><para>The name of the parameter.</para></param>
		/// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
		/// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>                
		/// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
		/// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
		/// <param name="value"><para>The value of the parameter.</para></param> 
        public void AddParameter(DbCommand command, string name, DbType dbType, ParameterDirection direction, string sourceColumn, DataRowVersion sourceVersion, object value)
		{
			AddParameter(command, name, dbType, 0, direction, false, 0, 0, sourceColumn, sourceVersion, value);
		}

		/// <summary>
		/// Adds a new Out <see cref="DbParameter"/> object to the given <paramref name="command"/>.
		/// </summary>
		/// <param name="command">The command to add the out parameter.</param>
		/// <param name="name"><para>The name of the parameter.</para></param>
		/// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
		/// <param name="size"><para>The maximum size of the data within the column.</para></param>        
		public void AddOutParameter(DbCommand command, string name, DbType dbType, int size)
		{
			AddParameter(command, name, dbType, size, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Default, DBNull.Value);
		}

		/// <summary>
		/// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
		/// </summary>
		/// <param name="command">The command to add the in parameter.</param>
		/// <param name="name"><para>The name of the parameter.</para></param>
		/// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
		/// <remarks>
		/// <para>This version of the method is used when you can have the same parameter object multiple times with different values.</para>
		/// </remarks>        
		public void AddInParameter(DbCommand command, string name, DbType dbType)
		{
			AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, null);
		}

		/// <summary>
		/// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
		/// </summary>
		/// <param name="command">The command to add the parameter.</param>
		/// <param name="name"><para>The name of the parameter.</para></param>
		/// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
		/// <param name="value"><para>The value of the parameter.</para></param>      
		public void AddInParameter(DbCommand command, string name, DbType dbType, object value)
		{
			AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, value);
		}

		/// <summary>
		/// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
		/// </summary>
		/// <param name="command">The command to add the parameter.</param>
		/// <param name="name"><para>The name of the parameter.</para></param>
		/// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
		/// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the value.</para></param>
		/// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
		public void AddInParameter(DbCommand command, string name, DbType dbType, string sourceColumn, DataRowVersion sourceVersion)
		{
			AddParameter(command, name, dbType, 0, ParameterDirection.Input, true, 0, 0, sourceColumn, sourceVersion, null);
		}

		/// <summary>
		/// Clears the parameter cache. Since there is only one parameter cache that is shared by all instances
		/// of this class, this clears all parameters cached for all databases.
		/// </summary>
		public static void ClearParameterCache()
		{
			QueryBase._parameterCache.Clear();
		}

		/// <summary>
		/// Sets a parameter value.
		/// </summary>
		/// <param name="command">The command with the parameter.</param>
		/// <param name="parameterName">The parameter name.</param>
		/// <param name="value">The parameter value.</param>
		public virtual void SetParameterValue(DbCommand command, string parameterName, object value)
		{
            if (value == null)
                value = DBNull.Value; 
            else if (value is DBDefault)
                value = null;

			command.Parameters[BuildParameterName(parameterName)].Value = value;
		}

		/// <summary>
		/// Gets a parameter value.
		/// </summary>
		/// <param name="command">The command that contains the parameter.</param>
		/// <param name="name">The name of the parameter.</param>
		/// <returns>The value of the parameter.</returns>
		public virtual object GetParameterValue(DbCommand command, string name)
		{
			return command.Parameters[BuildParameterName(name)].Value;
		}

		/// <summary>
		/// Determines if the number of parameters in the command matches the array of parameter values.
		/// </summary>
		/// <param name="command">The <see cref="DbCommand"/> containing the parameters.</param>
		/// <param name="values">The array of parameter values.</param>
		/// <returns><see langword="true"/> if the number of parameters and values match; otherwise, <see langword="false"/>.</returns>
		protected virtual bool SameNumberOfParametersAndValues(DbCommand command, object[] values)
		{
			int numberOfParametersToStoredProcedure = command.Parameters.Count;
			int numberOfValuesProvidedForStoredProcedure = values.Length;
			return numberOfParametersToStoredProcedure == numberOfValuesProvidedForStoredProcedure;
		}

		/// <summary>
		/// Builds a value parameter name for the current database.
		/// </summary>
		/// <param name="name">The name of the parameter.</param>
		/// <returns>A correctly formated parameter name.</returns>
		public virtual string BuildParameterName(string name)
		{
			return name;
		}

		/// <summary>
		/// Discovers the parameters for a <see cref="DbCommand"/>.
		/// </summary>
		/// <param name="command">The <see cref="DbCommand"/> to discover the parameters.</param>
		public void DiscoverParameters(DbCommand command)
		{
            // Uses the transaction scope to suppress this connection from being part of a transaction
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                using (DbConnection discoveryConnection = OpenNewConnection())
                {
                    using (DbCommand discoveryCommand = CreateCommandByCommandType(command.CommandType, command.CommandText))
                    {
                        discoveryCommand.Connection = discoveryConnection;
                        DeriveParameters(discoveryCommand);

                        foreach (IDataParameter parameter in discoveryCommand.Parameters)
                        {
                            IDataParameter cloneParameter = (IDataParameter)((ICloneable)parameter).Clone();
                            command.Parameters.Add(cloneParameter);
                        }
                    }
                }
            }
		}

		/// <summary>
		/// Retrieves parameter information from the stored procedure specified in the <see cref="DbCommand"/> and populates the Parameters collection of the specified <see cref="DbCommand"/> object. 
		/// </summary>
		/// <param name="discoveryCommand">The <see cref="DbCommand"/> to do the discovery.</param>
		protected abstract void DeriveParameters(DbCommand discoveryCommand);

        /// <summary>
        /// Returns the database connection for a specific provider
        /// </summary>
        /// <returns></returns>
        protected DbConnection OpenNewConnection()
        {
            DbConnection dbConnection = CreateConnection();
            OpenConnection(dbConnection);
            return dbConnection;
        }

        /// <summary>
        /// Opens the database connection for a specific provider using the security context defined for the query.
        /// </summary>
        /// <returns></returns>
        protected void OpenConnection(DbConnection connection)
        {
            if (DatabaseProfile.EnablePing == true)
            {
                PingReply reply = Ping(DatabaseProfile);
                if (reply.Status != IPStatus.Success)
                    throw new DatabaseException(string.Format("Unable to ping the datasource {0}. {1}", DatabaseProfile.DataSource, reply.ToString()));
            }
            using (SecurityContext.Impersonate(DatabaseProfile))
            {
                connection.Open();
                Console.WriteLine("Connection Opened");
            }
        }

        /// <summary>
        /// Begins a new transaction using a new transaction using the profile settings assigned to the query.
        /// </summary>
        /// <returns></returns>
        public DbTransaction BeginDbTransaction()
        {
            DbConnection connection = OpenNewConnection();
            return BeginDbTransaction(connection);
        }

        /// <summary>
        /// Begins a new transaction using a new transaction using the profile settings assigned to the query.
        /// </summary>
        /// <param name="isolation">The isolation level for the new transaction.</param>
        /// <returns></returns>
        public DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolation)
        {
            DbConnection connection = OpenNewConnection();
            return BeginDbTransaction(connection, isolation);
        }

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        /// <returns>The started transaction.</returns>
        public Transaction BeginTransaction()
        {
            Transaction tx = new Transaction();
            tx.Begin(this);
            return tx;
        }

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        /// <param name="isolation">The isolation level for the new transaction.</param>
        /// <returns>The started transaction.</returns>
        public Transaction BeginTransaction(System.Data.IsolationLevel isolation)
        {
            Transaction tx = new Transaction();
            tx.Begin(this, isolation);
            return tx;
        }

        /// <summary>
        /// Begins a transaction that can be used for distributed transactions.
        /// </summary>
        /// <returns>The started transaction.</returns>
        public Transaction BeginDistributedTransaction()
        {
            Transaction tx = new Transaction();
            tx.Begin();
            return tx;
        }

        private DbTransaction BeginDbTransaction(DbConnection connection, System.Data.IsolationLevel isolation)
        {
            DbTransaction tran = connection.BeginTransaction(isolation);
            return tran;
        }

        private DbTransaction BeginDbTransaction(DbConnection connection)
        {
            DbTransaction tran = connection.BeginTransaction();
            return tran;
        }
        
        /// <summary>
        /// Pings the datasource defined by the DatabaseProfile.
        /// </summary>
        /// <returns></returns>
        public PingReply Ping()
        {
            return this.Ping(_profile);
        }

        private PingReply Ping(DatabaseProfile profile)
        {
            string strDataSource = null;
            PingReply reply = null;

            try
            {
                PingOptions options = new PingOptions(profile.PingTTL, true);
                Ping pingServer = new Ping();
                
                // Send 32 bytes of data to the ping
                _bytPingBuffer = System.Text.Encoding.ASCII.GetBytes(PingData);
                strDataSource = GetServerNameToPing(profile);
                reply = pingServer.Send(strDataSource, profile.PingTimeout * 1000, _bytPingBuffer, options);
            }
            catch (Exception ex)
            {
                throw new DatabaseException(string.Format("Error trying to ping the datasource {0}. {1}", strDataSource, ex.Message), ex);
            }

            return reply;
        }

        private string GetServerNameToPing(DatabaseProfile profile)
        {
            string name = profile.DataSource;

            // Try to get the name of the computer out of the datasource name
            if (name.StartsWith(".") == true)
            {
                return Environment.MachineName;
            }

            // Look for a file extension
            if(!String.IsNullOrEmpty(Path.GetExtension(name)))
            {
                // Strip off a UNC path indicator if it exists
                if (name.StartsWith(@"\\"))
                {
                    // look for a single \
                    name = name.Substring(2);
                }
                else
                {
                    return Environment.MachineName;
                }
            }

            // Split on \ or / to strip off the instance name or UNC folder path
            string[] names = name.Split(new char[] { '\\', '/' });

            if (names.Length > 0)
            {
                return names[0];
            }

            throw new InvalidOperationException(String.Format("Unable to find server name for datasource {0}", profile.DataSource));
        }

        protected virtual DbCommand CreateCommandByCommandType(CommandType commandType, string commandText)
		{
            DbConnection dbConnection = CreateConnection();
            DbCommand command = dbConnection.CreateCommand();
			command.CommandType = commandType;
			command.CommandText = commandText;
            command.CommandTimeout = _profile.Timeout;

			return command;
		}

		protected int DoUpdateDataSet(DataSet dataSet, string tableName, DbDataAdapter adapter)
		{
			if (string.IsNullOrEmpty(tableName)) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, "tableName");
			if (dataSet == null) throw new ArgumentNullException("dataSet");
            if (dataSet.Tables[tableName] == null) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, string.Format("Table {0} was not found in the dataset.", tableName));

			if (adapter.InsertCommand == null && adapter.UpdateCommand == null && adapter.DeleteCommand == null)
			{
				throw new ArgumentException("Update DataSet argument failure.");
			}

			try
            {
				return adapter.Update(dataSet.Tables[tableName]);
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		private void DoLoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames)
		{
			if (tableNames == null) throw new ArgumentNullException("tableNames");
			if (tableNames.Length == 0)
			{
				throw new ArgumentException("tableNames length is not valid.");
			}
			for (int i = 0; i < tableNames.Length; i++)
			{
				if (string.IsNullOrEmpty(tableNames[i])) throw new ArgumentException(Resources.ExceptionNullOrEmptyString, string.Concat("tableNames[", i, "]"));
			}

			using (DbDataAdapter adapter = CreateDataAdapter(UpdateBehavior.Standard))
			{
				((IDbDataAdapter)adapter).SelectCommand = command;

				try
				{
					string systemCreatedTableNameRoot = "Table";
					for (int i = 0; i < tableNames.Length; i++)
					{
						string systemCreatedTableName = (i == 0)
							 ? systemCreatedTableNameRoot
							 : systemCreatedTableNameRoot + i;

						adapter.TableMappings.Add(systemCreatedTableName, tableNames[i]);
					}

					adapter.Fill(dataSet);
				}
				catch (Exception e)
				{
					throw e;
				}
			}
		}

		private object DoExecuteScalar(DbCommand command)
		{
			try
			{
                // If the command connection is already open don't create a new connection.
                if (command.Connection == null)
                    command.Connection = OpenNewConnection();
                else if (command.Connection.State != ConnectionState.Open)
                    OpenConnection(command.Connection);
                return command.ExecuteScalar();
            }
			catch (Exception e)
			{
				throw e;
			}
		}

		private int DoExecuteNonQuery(DbCommand command)
		{
			try
			{
                // If the command connection is already open don't create a new connection.
                if (command.Connection == null)
                    command.Connection = OpenNewConnection();
                else if (command.Connection.State != ConnectionState.Open)
                    OpenConnection(command.Connection);
                return command.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				throw e;
			}
		}

        protected virtual void PrepareTransaction(DbTransaction transaction, DbCommand command)
        {
            if (command == null) throw new ArgumentNullException("Command");
            if (transaction == null) throw new ArgumentNullException("Transaction");

            command.Connection = transaction.Connection;
            command.Transaction = transaction;
        }

        protected virtual void PrepareTransaction(Transaction transaction, DbCommand command)
        {
            if (command == null) throw new ArgumentNullException("Command");
            if (transaction == null) throw new ArgumentNullException("Transaction");

            DbTransaction tx = TransactionManager.Instance.GetTransaction(transaction) as DbTransaction;

            if (tx != null)
            {
                command.Connection = tx.Connection;
                command.Transaction = tx;
            }
        }

		private void AssignParameterValues(DbCommand command, object[] values)
		{
			int parameterIndexShift = UserParametersStartIndex();	// DONE magic number, depends on the database
            if (values != null)
            {
                for (int i = 0; i < values.Length && i < command.Parameters.Count + parameterIndexShift; i++)
                {
                    IDataParameter parameter = command.Parameters[i + parameterIndexShift];

                    // There used to be code here that checked to see if the parameter was input or input/output
                    // before assigning the value to it. We took it out because of an operational bug with
                    // deriving parameters for a stored procedure. It turns out that output parameters are set
                    // to input/output after discovery, so any direction checking was unneeded. Should it ever
                    // be needed, it should go here, and check that a parameter is input or input/output before
                    // assigning a value to it.
                    SetParameterValue(command, parameter.ParameterName, values[i]);
                }
            }
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
		/// <param name="precision"><para>The maximum number of digits used to represent the <paramref name="value"/>.</para></param>
		/// <param name="scale"><para>The number of decimal places to which <paramref name="value"/> is resolved.</para></param>
		/// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
		/// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
		/// <param name="value"><para>The value of the parameter.</para></param>  
		/// <returns>A newly created <see cref="DbParameter"/> fully initialized with given parameters.</returns>
		protected DbParameter CreateParameter(DbCommand command, string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
		{
			DbParameter param = CreateParameter(command, name);
			ConfigureParameter(param, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
			return param;
		}

		/// <summary>
		/// <para>Adds a new instance of a <see cref="DbParameter"/> object.</para>
		/// </summary>
        /// <param name="command">The command to create the parameter for.</param>
		/// <param name="name"><para>The name of the parameter.</para></param>
		/// <returns><para>An unconfigured parameter.</para></returns>
		protected DbParameter CreateParameter(DbCommand command, string name)
		{
			DbParameter param = command.CreateParameter();
			param.ParameterName = BuildParameterName(name);

			return param;
		}

		/// <summary>
		/// Returns the starting index for parameters in a command.
		/// </summary>
		/// <returns>The starting index for parameters in a command.</returns>
		protected virtual int UserParametersStartIndex()
		{
			return 0;
		}

		/// <summary>
		/// Configures a given <see cref="DbParameter"/>.
		/// </summary>
		/// <param name="param">The <see cref="DbParameter"/> to configure.</param>
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
		protected virtual void ConfigureParameter(DbParameter param, string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
		{
			param.DbType = dbType;
			param.Size = size;
			param.Value = (value == null) ? DBNull.Value : value;
			param.Direction = direction;
			param.IsNullable = nullable;
			param.SourceColumn = sourceColumn;
			param.SourceVersion = sourceVersion;
		}

        /// <summary>
        /// Converts the command to a string.
        /// </summary>
        /// <param name="command">DbCommand object that we want to see as a string.</param>
        /// <returns>A string representation of the DbCommand object passed to the method.</returns>
        public string CommandToString(DbCommand command)
        {
            StringBuilder builder = new StringBuilder(command.CommandText);
            string paramValue;

            if (command.CommandType == CommandType.StoredProcedure)
            {
                builder.Append("(");
                for (int i = 0; i < command.Parameters.Count; i++)
                {
                    if (command.Parameters[i].Direction != ParameterDirection.ReturnValue)
                    {
                        object objValue = command.Parameters[i].Value;

                        if (objValue == DBNull.Value)
                            paramValue = NullValue;
                        else
                            paramValue = objValue?.ToString() ?? NullValue;
                        if (i == 0)
                            builder.Append(paramValue);
                        else
                            builder.Append(", " + paramValue);
                    }
                }
                builder.Append(")");
            }
            else
            {
                string textCommand = command.CommandText;

                if (command.CommandType == CommandType.Text)
                {
                    for (int i = 0; i < command.Parameters.Count; i++)
                    {
                        if (command.Parameters[i].Direction != ParameterDirection.ReturnValue)
                        {
                            object objValue = command.Parameters[i].Value;

                            if (objValue == DBNull.Value)
                                paramValue = NullValue;
                            else
                                paramValue = objValue?.ToString() ?? NullValue;
                            System.Text.RegularExpressions.MatchCollection colMatches = System.Text.RegularExpressions.Regex.Matches(textCommand, command.Parameters[i].ParameterName + @"\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            foreach (System.Text.RegularExpressions.Match match in colMatches)
                            {
                                if (match.Value.Length > 0)
                                {
                                    switch (command.Parameters[i].DbType)
                                    {
                                        case DbType.StringFixedLength:
                                        case DbType.String:
                                        case DbType.AnsiString:
                                        case DbType.AnsiStringFixedLength:
                                        case DbType.DateTime:
                                            if (paramValue != NullValue)
                                                textCommand = System.Text.RegularExpressions.Regex.Replace(textCommand, command.Parameters[i].ParameterName + @"\b", string.Concat("'", paramValue, "'"), System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                                            else
                                                textCommand = System.Text.RegularExpressions.Regex.Replace(textCommand, command.Parameters[i].ParameterName + @"\b", paramValue, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                                            break;
                                        default:
                                            textCommand = System.Text.RegularExpressions.Regex.Replace(textCommand, command.Parameters[i].ParameterName + @"\b", paramValue, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    builder = new StringBuilder(textCommand);
                }
            }
            return builder.ToString();
        }
        public string CommandToString(DbCommand command, params object[] parameterValues)
        {
            StringBuilder builder = new StringBuilder(command.CommandText);
            
            if (command.CommandType == CommandType.StoredProcedure)
            {
                // Show the named parameters
                builder.Append("(");
                for (int i = 0; i < command.Parameters.Count; i++)
                {
                    if (command.Parameters[i].Direction != ParameterDirection.ReturnValue)
                    {
                        if (i == 0)
                            builder.Append(command.Parameters[i].ParameterName);
                        else
                            builder.Append(", " + command.Parameters[i].ParameterName);
                    }
                }
                builder.Append(")");
            }
            if (parameterValues != null)
            {
                int length = builder.Length;

                builder.Append("[");
                foreach (var value in parameterValues)
                {
                    string stringValue;

                    if (value == DBNull.Value)
                        stringValue = NullValue;
                    else
                        stringValue = value?.ToString() ?? NullValue;
                    if (builder.Length == length)
                    {
                        builder.Append(stringValue);
                    }
                    else
                    {
                        builder.Append(", ");
                        builder.Append(stringValue);
                    }
                }
                builder.Append("]");
            }
            return builder.ToString();
        }
    }    
}
