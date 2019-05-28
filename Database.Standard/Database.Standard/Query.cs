using System;
using System.Data;
using System.Data.Common;
using Interstates.Control.Database.Plugin;

namespace Interstates.Control.Database
{
    /// <summary>
    /// A Query is used to run your SQL commands against a database.
    /// </summary>
    public class Query : IQuery
    {
        QueryBase _query;

        /// <summary>
        /// Creates the appropriate Query based on the settings of the connection string.
        /// If the connection string contains a provider that is not supported, the Query will be based on the OleDbQuery class.
        /// If the provider parameter is missing, an instance of the SqlQuery class is returned.
        /// </summary>
        /// <returns></returns>
        public Query()
            : this(DatabaseConfiguration.DefaultProfile)
        {
        }

        /// <summary>
        /// Creates the appropriate Query based on the settings of the connection string.
        /// If the connection string contains a provider that is not supported, the Query will be based on the OleDbQuery class.
        /// If the provider parameter is missing, an instance of the SqlQuery class is returned.
        /// </summary>
        /// <returns></returns>
        public Query(string profileName)
            : this(DatabaseConfiguration.Profiles[profileName])
        {
        }

        /// <summary>
        /// Creates the appropriate Query based on the provider type.
        /// </summary>
        /// <returns></returns>
        public Query(DatabaseProfile profile)
        {
            IProviderPlugin plugin = Plugin.PluginManager.GetPluginByProviderType(profile.ProviderType);

            _query = plugin.CreateQuery(profile);
        }

        /// <summary>
        /// Creates the appropriate Query based on an existing query.
        /// </summary>
        /// <returns></returns>
        public Query(QueryBase query)
        {
            _query = query;
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
            _query.AddInParameter(command, name, dbType);
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
            _query.AddInParameter(command, name, dbType, value);
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
            _query.AddInParameter(command, name, dbType, sourceColumn, sourceVersion);
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
            _query.AddOutParameter(command, name, dbType, size);
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
            _query.AddParameter(command, name, dbType, direction, sourceColumn, sourceVersion, value);
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
        public void AddParameter(DbCommand command, string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value)
        {
            _query.AddParameter(command, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
        }

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        /// <returns>The started transaction.</returns>
        public Transaction BeginTransaction()
        {
            return _query.BeginTransaction();
        }

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level for the new transaction.</param>
        /// <returns>The started transaction.</returns>
        public Transaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return _query.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Begins a transaction that can be used for distributed transactions.
        /// </summary>
        /// <returns>The started transaction.</returns>
        public Transaction BeginDistributedTransaction()
        {
            return _query.BeginDistributedTransaction();
        }

        /// <summary>
        /// Begins a new transaction using a new transaction using the profile settings assigned to the query.
        /// </summary>
        /// <returns>The started transaction</returns>
        public DbTransaction BeginDbTransaction()
        {
            return _query.BeginDbTransaction();
        }

        /// <summary>
        /// Begins a new transaction using a new transaction using the profile settings assigned to the query.
        /// </summary>
        /// <param name="isolationLevel">The isolation level for the new transaction.</param>
        /// <returns>The started transaction</returns>
        public DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return _query.BeginDbTransaction(isolationLevel);
        }

        /// <summary>
        /// Builds a value parameter name for the current database.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>A correctly formated parameter name.</returns>
        public string BuildParameterName(string name)
        {
            return _query.BuildParameterName(name);
        }

        /// <summary>
        /// Converts the command to a string.
        /// </summary>
        /// <param name="command">DbCommand object that we want to see as a string.</param>
        /// <returns>A string representation of the DbCommand object passed to the method.</returns>
        public string CommandToString(DbCommand command)
        {
            return _query.CommandToString(command);
        }

        /// <summary>
        /// Creates a command builder for a provider.
        /// </summary>
        /// <returns>A <see cref="DbDataAdapter"/>.</returns>
        /// <seealso cref="DbCommandBuilder"/>
        public DbCommandBuilder CreateCommandBuilder()
        {
            return _query.CreateCommandBuilder();
        }

        /// <summary>
        /// Create the connection that will be used to execute the query.
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateConnection()
        {
            return _query.CreateConnection();
        }

        /// <summary>
        /// Creates a data adapter for a provider.
        /// </summary>
        /// <returns>A <see cref="DbDataAdapter"/>.</returns>
        /// <seealso cref="DbDataAdapter"/>
        public DbDataAdapter CreateDataAdapter()
        {
            return _query.CreateDataAdapter();
        }

        /// <summary>
        /// Creates a parameter for a provider.
        /// </summary>
        /// <returns></returns>
        public DbParameter CreateParameter()
        {
            return _query.CreateParameter();
        }

        /// <summary>
        /// Creates a parameter for a provider.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value to assign the parameter.</param>
        /// <returns></returns>
        public DbParameter CreateParameter(string name, object value)
        {
            return _query.CreateParameter(name, value);
        }

        /// <summary>
        /// <para>Creates a <see cref="DbCommand"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>       
        public DbCommand CreateStoredProcedureCommand(string storedProcedureName)
        {
            return _query.CreateStoredProcedureCommand(storedProcedureName);
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
        public DbCommand CreateStoredProcedureCommand(string storedProcedureName, params object[] parameterValues)
        {
            return _query.CreateStoredProcedureCommand(storedProcedureName, parameterValues);
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
            return _query.CreateStoredProcedureCommandWithSourceColumns(storedProcedureName, sourceColumns);
        }

        /// <summary>
        /// <para>Creates a <see cref="DbCommand"/> for a SQL query.</para>
        /// </summary>
        /// <param name="query"><para>The text of the query.</para></param>        
        /// <returns><para>The <see cref="DbCommand"/> for the SQL query.</para></returns>        
        public DbCommand CreateTextCommand(string query)
        {
            return _query.CreateTextCommand(query);
        }

        /// <summary>
        /// <para>Creates a <see cref="DbCommand"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="query"><para>The name of the SQL command.</para></param>
        /// <param name="parameterValues"><para>The list of parameters for the procedure.</para></param>
        /// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>
        /// <remarks>
        /// <para>The parameters for the stored procedure will be discovered and the values are assigned in positional order.</para>
        /// </remarks>        
        public DbCommand CreateTextCommand(string query, params object[] parameterValues)
        {
            return _query.CreateTextCommand(query, parameterValues);
        }

        /// <summary>
        /// Gets the <see cref="DatabaseProfile">database profile</see> used to connect to the database.
        /// </summary>
        public DatabaseProfile DatabaseProfile
        {
            get { return _query.DatabaseProfile; }
        }

        /// <summary>
        /// Discovers the parameters for a <see cref="DbCommand"/>.
        /// </summary>
        /// <param name="command">The <see cref="DbCommand"/> to discover the parameters.</param>
        public void DiscoverParameters(DbCommand command)
        {
            _query.DiscoverParameters(command);
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
            return _query.Execute(commandType, commandText);
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
            return _query.Execute(transaction, commandType, commandText);
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
            return _query.Execute(transaction, commandType, commandText);
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        public virtual DataSet Execute(DbCommand command)
        {
            return _query.Execute(command);
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
            return _query.Execute(transaction, command);
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
            return _query.Execute(transaction, command);
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
            return _query.Execute(command, parameterValues);
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
            return _query.Execute(transaction, command, parameterValues);
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
            return _query.Execute(transaction, command, parameterValues);
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
            return _query.Execute(storedProcedureName, parameterValues);
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
            return _query.Execute(transaction, storedProcedureName, parameterValues);
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
            return _query.Execute(transaction, storedProcedureName, parameterValues);
        }

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DbDataReader"/>.</para>
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="behavior">The behavior to execute the command with.</param>
        /// <returns>A <see cref="DbDataReader"/> with the results of the <paramref name="command"/>.</returns>        
        public virtual DbDataReader ExecuteReader(DbCommand command, CommandBehavior behavior = CommandBehavior.Default)
        {
            return _query.ExecuteReader(command, behavior);
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
            return _query.ExecuteReader(transaction, command, behavior);
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
            return _query.ExecuteReader(transaction, command, behavior);
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
            return _query.ExecuteReader(command, behavior);
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
            return _query.ExecuteReader(transaction, command, behavior);
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
            return _query.ExecuteReader(transaction, command, behavior);
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
            return _query.ExecuteReader(commandType, commandText, behavior);
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
            return _query.ExecuteReader(transaction, commandType, commandText, behavior);
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
            return _query.ExecuteReader(transaction, commandType, commandText, behavior);
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> and returns the results in a new <see cref="DataTable"/>.</para>
        /// </summary>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataTable"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        public virtual DataTable ExecuteText(string commandText)
        {
            return _query.ExecuteText(commandText);
        }

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataTable"/> within a transaction.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataTable"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        public virtual DataTable ExecuteText(Transaction transaction, string commandText)
        {
            return _query.ExecuteText(transaction, commandText);
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
            return _query.ExecuteText(commandText, parameterValues);
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
            return _query.ExecuteText(transaction, commandText, parameterValues);
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
            return _query.ExecuteText(transaction, commandText, parameterValues);
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
            return _query.ExecuteNonQuery(transaction, commandType, commandText);
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
            return _query.ExecuteNonQuery(transaction, storedProcedureName, parameterValues);
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
            return _query.ExecuteNonQuery(commandType, commandText);
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
            return _query.ExecuteNonQuery(command);
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
            return _query.ExecuteNonQuery(transaction, command);
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
            return _query.ExecuteNonQuery(transaction, command);
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
            return _query.ExecuteNonQuery(command, parameterValues);
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
            return _query.ExecuteNonQuery(transaction, command, parameterValues);
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
            return _query.ExecuteNonQuery(transaction, command, parameterValues);
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
            return _query.ExecuteNonQuery(transaction, commandType, commandText);
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
            return _query.ExecuteNonQuery(transaction, storedProcedureName, parameterValues);
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
            return _query.ExecuteNonQuery(storedProcedureName, parameterValues);
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
            return _query.ExecuteTextNonQuery(commandText, parameterValues);
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
            return _query.ExecuteTextNonQuery(transaction, commandText, parameterValues);
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
            return _query.ExecuteTextNonQuery(transaction, commandText, parameterValues);
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
            return _query.ExecuteScalar(commandType, commandText);
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
            return _query.ExecuteScalar(transaction, commandType, commandText);
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
            return _query.ExecuteScalar(transaction, commandType, commandText);
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
            return _query.ExecuteScalar(command);
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
            return _query.ExecuteScalar(transaction, command);
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
            return _query.ExecuteScalar(transaction, command);
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
            return _query.ExecuteScalar(command, parameterValues);
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
            return _query.ExecuteScalar(transaction, command, parameterValues);
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
            return _query.ExecuteScalar(transaction, command, parameterValues);
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
            return _query.ExecuteScalar(storedProcedureName, parameterValues);
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
            return _query.ExecuteScalar(transaction, storedProcedureName, parameterValues);
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
            return _query.ExecuteScalar(transaction, storedProcedureName, parameterValues);
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
            return _query.ExecuteTextScalar(commandText, parameterValues);
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
            return _query.ExecuteTextScalar(transaction, commandText, parameterValues);
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
            return _query.ExecuteTextScalar(transaction, commandText, parameterValues);
        }

        /// <summary>
        /// Gets a parameter value.
        /// </summary>
        /// <param name="command">The command that contains the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        public object GetParameterValue(DbCommand command, string name)
        {
            return _query.GetParameterValue(command, name);
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
        public void LoadDataSet(Transaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            _query.LoadDataSet(transaction, commandType, commandText, dataSet, tableNames);
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
        public void LoadDataSet(Transaction transaction, string storedProcedureName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            _query.LoadDataSet(transaction, storedProcedureName, dataSet, tableNames, parameterValues);
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
        public void LoadDataSet(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            _query.LoadDataSet(commandType, commandText, dataSet, tableNames);
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
        public void LoadDataSet(Transaction transaction, DbCommand command, DataSet dataSet, string tableName)
        {
            _query.LoadDataSet(transaction, command, dataSet, tableName);
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
        public void LoadDataSet(Transaction transaction, DbCommand command, DataSet dataSet, string[] tableNames)
        {
            _query.LoadDataSet(transaction, command, dataSet, tableNames);
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
        public void LoadDataSet(DbTransaction transaction, DbCommand command, DataSet dataSet, string tableName)
        {
            _query.LoadDataSet(transaction, command, dataSet, tableName);
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
        public void LoadDataSet(DbTransaction transaction, DbCommand command, DataSet dataSet, string[] tableNames)
        {
            _query.LoadDataSet(transaction, command, dataSet, tableNames);
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
        public void LoadDataSet(DbCommand command, DataSet dataSet, string tableName)
        {
            _query.LoadDataSet(command, dataSet, tableName);
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
        public void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames)
        {
            _query.LoadDataSet(command, dataSet, tableNames);
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
        public void LoadDataSet(DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            _query.LoadDataSet(transaction, commandType, commandText, dataSet, tableNames);
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
        public void LoadDataSet(DbTransaction transaction, string storedProcedureName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            _query.LoadDataSet(transaction, storedProcedureName, dataSet, tableNames, parameterValues);
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
        public void LoadDataSet(string storedProcedureName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            _query.LoadDataSet(storedProcedureName, dataSet, tableNames, parameterValues);
        }

        /// <summary>
        /// Pings the datasource defined by the DatabaseProfile.
        /// </summary>
        /// <returns></returns>
        public System.Net.NetworkInformation.PingReply Ping()
        {
            return _query.Ping();
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
        public Framework.Security.SecurityContext SecurityContext
        {
            get
            {
                return _query.SecurityContext;
            }
            set
            {
                _query.SecurityContext = value;
            }
        }

        /// <summary>
        /// Sets a parameter value.
        /// </summary>
        /// <param name="command">The command with the parameter.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        public void SetParameterValue(DbCommand command, string parameterName, object value)
        {
            _query.SetParameterValue(command, parameterName, value);
        }

        /// <summary>
        /// Update the dataset using the default insert, update and delete commands built from the command builder.
        /// </summary>
        /// <param name="dataSet">The dataset to update.</param>
        /// <param name="tableName">The database table to update.  NOTE: The table name must also match the name of the table in the dataset.</param>
        /// <returns></returns>
        public int UpdateDataSet(DataSet dataSet, string tableName)
        {
            return _query.UpdateDataSet(dataSet, tableName);
        }

        /// <summary>
        /// Update the dataset using the default insert, update and delete commands built from the command builder.
        /// </summary>
        /// <param name="transaction"><para>The <see cref="Transaction"/> to use.</para></param>
        /// <param name="dataSet">The dataset to update.</param>
        /// <param name="tableName">The database table to update.  NOTE: The table name must also match the name of the table in the dataset.</param>
        /// <returns></returns>
        public int UpdateDataSet(Transaction transaction, DataSet dataSet, string tableName)
        {
            return _query.UpdateDataSet(transaction, dataSet, tableName);
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
            return _query.UpdateDataSet(transaction, dataSet, tableName, insertCommand, updateCommand, deletzeCommand);
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
        public int UpdateDataSet(DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand, UpdateBehavior updateBehavior)
        {
            return _query.UpdateDataSet(dataSet, tableName, insertCommand, updateCommand, deleteCommand, updateBehavior);
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
            return _query.UpdateDataSet(transaction, dataSet, tableName, insertCommand, updateCommand, deleteCommand);
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
            return _query.UpdateDataSet(dataSet, tableName, updateAdapter);
        }

        /// <summary>
        /// Update the dataset using the default insert, update and delete commands built from the command builder.
        /// </summary>
        /// <param name="transaction"><para>The <see cref="DbTransaction"/> to use.</para></param>
        /// <param name="dataSet">The dataset to update.</param>
        /// <param name="tableName">The database table to update.  NOTE: The table name must also match the name of the table in the dataset.</param>
        /// <returns></returns>
        public int UpdateDataSet(DbTransaction transaction, DataSet dataSet, string tableName)
        {
            return _query.UpdateDataSet(transaction, dataSet, tableName);
        }
    }
}
