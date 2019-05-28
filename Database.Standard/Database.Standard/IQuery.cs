using System;
using System.Data;
using System.Data.Common;
using System.Net.NetworkInformation;
using Interstates.Control.Framework.Security;

namespace Interstates.Control.Database
{
    /// <summary>
    /// Defines how to interface with a query object.
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the in parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <remarks>
        /// <para>This version of the method is used when you can have the same parameter object multiple times with different values.</para>
        /// </remarks>        
        void AddInParameter(DbCommand command, string name, DbType dbType);

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The commmand to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="value"><para>The value of the parameter.</para></param>      
        void AddInParameter(DbCommand command, string name, DbType dbType, object value);

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>                
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the value.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        void AddInParameter(DbCommand command, string name, DbType dbType, string sourceColumn, DataRowVersion sourceVersion);

        /// <summary>
        /// Adds a new Out <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the out parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>        
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>        
        void AddOutParameter(DbCommand command, string name, DbType dbType, int size);

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
        void AddParameter(DbCommand command, string name, DbType dbType, ParameterDirection direction, string sourceColumn, DataRowVersion sourceVersion, object value);

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name"><para>The name of the parameter.</para></param>
        /// <param name="dbType"><para>One of the <see cref="DbType"/> values.</para></param>
        /// <param name="size"><para>The maximum size of the data within the column.</para></param>
        /// <param name="direction"><para>One of the <see cref="ParameterDirection"/> values.</para></param>
        /// <param name="nullable"><para>Avalue indicating whether the parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual Basic); values.</para></param>
        /// <param name="precision"><para>The maximum number of digits used to represent the <paramref name="value"/>.</para></param>
        /// <param name="scale"><para>The number of decimal places to which <paramref name="value"/> is resolved.</para></param>
        /// <param name="sourceColumn"><para>The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</para></param>
        /// <param name="sourceVersion"><para>One of the <see cref="DataRowVersion"/> values.</para></param>
        /// <param name="value"><para>The value of the parameter.</para></param>       
        void AddParameter(DbCommand command, string name, DbType dbType, int size, ParameterDirection direction, bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, object value);

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        /// <returns>The started transaction.</returns>
        Transaction BeginTransaction();

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level for the new transaction.</param>
        /// <returns>The started transaction.</returns>
        Transaction BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Begins a transaction that can be used for distributed transactions.
        /// </summary>
        /// <returns>The started transaction.</returns>
        Transaction BeginDistributedTransaction();

        /// <summary>
        /// Begins a new transaction using a new transaction using the profile settings assigned to the query.
        /// </summary>
        /// <returns>The starte transaction</returns>
        DbTransaction BeginDbTransaction();

        /// <summary>
        /// Begins a new transaction using a new transaction using the profile settings assigned to the query.
        /// </summary>
        /// <param name="isolationLevel">The isolation level for the new transaction.</param>
        /// <returns>The starte transaction</returns>
        DbTransaction BeginDbTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Builds a value parameter name for the current database.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>A correctly formated parameter name.</returns>
        string BuildParameterName(string name);

        /// <summary>
        /// Converts the command to a string.
        /// </summary>
        /// <param name="command">DbCommand object that we want to see as a string.</param>
        /// <returns>A string representation of the DbCommand object passed to the method.</returns>
        string CommandToString(DbCommand command);

        /// <summary>
        /// Creates a command builder for a provider.
        /// </summary>
        /// <returns>A <see cref="DbDataAdapter"/>.</returns>
        /// <seealso cref="DbCommandBuilder"/>
        DbCommandBuilder CreateCommandBuilder();

        /// <summary>
        /// Creates a parameter for a provider.
        /// </summary>
        /// <returns></returns>
        DbParameter CreateParameter();

        /// <summary>
        /// Creates a parameter for a provider.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="value">The value to assign the parameter.</param>
        /// <returns></returns>
        DbParameter CreateParameter(string name, object value);

        /// <summary>
        /// Create the connection that will be used to execute the query.
        /// </summary>
        /// <returns></returns>
        DbConnection CreateConnection();
        
        /// <summary>
        /// Creates a data adapter for a provider.
        /// </summary>
        /// <returns>A <see cref="DbDataAdapter"/>.</returns>
        /// <seealso cref="DbDataAdapter"/>
        DbDataAdapter CreateDataAdapter();
        
        /// <summary>
        /// <para>Creates a <see cref="DbCommand"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>       
        DbCommand CreateStoredProcedureCommand(string storedProcedureName);
        
        /// <summary>
        /// <para>Creates a <see cref="DbCommand"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <param name="parameterValues"><para>The list of parameters for the procedure.</para></param>
        /// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>
        /// <remarks>
        /// <para>The parameters for the stored procedure will be discovered and the values are assigned in positional order.</para>
        /// </remarks>        
        DbCommand CreateStoredProcedureCommand(string storedProcedureName, params object[] parameterValues);
        
        /// <summary>
        /// Wraps around a derived class's implementation of the CreateStoredProcedureCommandWrapper method and adds functionality for
        /// using this method with UpdateDataSet.  The GetStoredProcCommandWrapper method (above); that takes a params array 
        /// expects the array to be filled with VALUES for the parameters. This method differs from the GetStoredProcCommandWrapper 
        /// method in that it allows a user to pass in a string array. It will also dynamically discover the parameters for the 
        /// stored procedure and set the parameter's SourceColumns to the strings that are passed in. It does this by mapping 
        /// the parameters to the strings IN ORDER. Thus, order is very important.
        /// </summary>
        /// <param name="storedProcedureName"><para>The name of the stored procedure.</para></param>
        /// <param name="sourceColumns"><para>The list of DataFields for the procedure.</para></param>
        /// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>
        DbCommand CreateStoredProcedureCommandWithSourceColumns(string storedProcedureName, params string[] sourceColumns);

        /// <summary>
        /// <para>Creates a <see cref="DbCommand"/> for a SQL query.</para>
        /// </summary>
        /// <param name="query"><para>The text of the query.</para></param>        
        /// <returns><para>The <see cref="DbCommand"/> for the SQL query.</para></returns>        
        DbCommand CreateTextCommand(string query);
        
        /// <summary>
        /// <para>Creates a <see cref="DbCommand"/> for a stored procedure.</para>
        /// </summary>
        /// <param name="query"><para>The name of the Sql command.</para></param>
        /// <param name="parameterValues"><para>The list of parameters for the procedure.</para></param>
        /// <returns><para>The <see cref="DbCommand"/> for the stored procedure.</para></returns>
        /// <remarks>
        /// <para>The parameters for the stored procedure will be discovered and the values are assigned in positional order.</para>
        /// </remarks>        
        DbCommand CreateTextCommand(string query, params object[] parameterValues);
        
        /// <summary>
        /// Gets the <see cref="DatabaseProfile">database profile</see> used to connect to the database.
        /// </summary>
        DatabaseProfile DatabaseProfile{ get; }
        
        /// <summary>
        /// Discovers the parameters for a <see cref="DbCommand"/>.
        /// </summary>
        /// <param name="command">The <see cref="DbCommand"/> to discover the parameters.</param>
        void DiscoverParameters(DbCommand command);
        
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
        DataSet Execute(CommandType commandType, string commandText);
        
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
        DataSet Execute(Transaction transaction, CommandType commandType, string commandText);
        
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
        DataSet Execute(DbTransaction transaction, CommandType commandType, string commandText);
        
        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        DataSet Execute(DbCommand command);
        
        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        DataSet Execute(Transaction transaction, DbCommand command);
        
        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        DataSet Execute(DbTransaction transaction, DbCommand command);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        DataSet Execute(DbCommand command, params object[] parameterValues);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <param name="parameterValues">
		/// <para>An array of paramters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
		/// </param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        DataSet Execute(Transaction transaction, DbCommand command, params object[] parameterValues);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <returns>A <see cref="DataSet"/> with the results of the <paramref name="command"/>.</returns>        
        DataSet Execute(DbTransaction transaction, DbCommand command, params object[] parameterValues);

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> with <paramref name="parameterValues" /> and returns the results in a new <see cref="DataSet"/>.</para>
        /// </summary>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="storedProcedureName"/>.</para>
        /// </returns>
        DataSet Execute(string storedProcedureName, params object[] parameterValues);
        
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
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="storedProcedureName"/>.</para>
        /// </returns>
        DataSet Execute(Transaction transaction, string storedProcedureName, params object[] parameterValues);
        
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
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataSet"/> with the results of the <paramref name="storedProcedureName"/>.</para>
        /// </returns>
        DataSet Execute(DbTransaction transaction, string storedProcedureName, params object[] parameterValues);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DbDataReader"/>.</para>
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="behavior">The behavior to execute the command with.</param>
        /// <returns>A <see cref="DbDataReader"/> with the results of the <paramref name="command"/>.</returns>        
        DbDataReader ExecuteReader(DbCommand command, CommandBehavior behavior = CommandBehavior.Default);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DbDataReader"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <param name="behavior">The behavior to execute the command with.</param>
        /// <returns>A <see cref="DbDataReader"/> with the results of the <paramref name="command"/>.</returns>        
        DbDataReader ExecuteReader(DbTransaction transaction, DbCommand command, CommandBehavior behavior = CommandBehavior.Default);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DbDataReader"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <param name="behavior">The behavior to execute the command with.</param>
        /// <returns>A <see cref="DbDataReader"/> with the results of the <paramref name="command"/>.</returns>        
        DbDataReader ExecuteReader(Transaction transaction, DbCommand command, CommandBehavior behavior = CommandBehavior.Default);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the results in a new <see cref="DbDataReader"/>.</para>
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="behavior">The behavior to execute the command with.</param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <returns>A <see cref="DbDataReader"/> with the results of the <paramref name="command"/>.</returns>        
        DbDataReader ExecuteReader(DbCommand command, CommandBehavior behavior = CommandBehavior.Default, params object[] parameterValues);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DbDataReader"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <param name="behavior">The behavior to execute the command with.</param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <returns>A <see cref="DbDataReader"/> with the results of the <paramref name="command"/>.</returns>        
        DbDataReader ExecuteReader(Transaction transaction, DbCommand command, CommandBehavior behavior = CommandBehavior.Default, params object[] parameterValues);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction" /> and returns the results in a new <see cref="DbDataReader"/>.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <param name="behavior">The behavior to execute the command with.</param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <returns>A <see cref="DbDataReader"/> with the results of the <paramref name="command"/>.</returns>        
        DbDataReader ExecuteReader(DbTransaction transaction, DbCommand command, CommandBehavior behavior = CommandBehavior.Default, params object[] parameterValues);

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
        DbDataReader ExecuteReader(CommandType commandType, string commandText, CommandBehavior behavior = CommandBehavior.Default);

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
        DbDataReader ExecuteReader(DbTransaction transaction, CommandType commandType, string commandText, CommandBehavior behavior = CommandBehavior.Default);

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
        DbDataReader ExecuteReader(Transaction transaction, CommandType commandType, string commandText, CommandBehavior behavior = CommandBehavior.Default);

        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> with <paramref name="parameterValues" /> and returns the results in a new <see cref="DataTable"/>.</para>
        /// </summary>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataTable"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        DataTable ExecuteText(string commandText, params object[] parameterValues);
        
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
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataTable"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        DataTable ExecuteText(Transaction transaction, string commandText, params object[] parameterValues);
        
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
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>A <see cref="DataTable"/> with the results of the <paramref name="commandText"/>.</para>
        /// </returns>
        DataTable ExecuteText(DbTransaction transaction, string commandText, params object[] parameterValues);
        
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
        int ExecuteNonQuery(Transaction transaction, CommandType commandType, string commandText);
        
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
        int ExecuteNonQuery(Transaction transaction, string storedProcedureName, params object[] parameterValues);
        
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
        int ExecuteNonQuery(CommandType commandType, string commandText);
        
        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>       
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        int ExecuteNonQuery(DbCommand command);
        
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
        int ExecuteNonQuery(Transaction transaction, DbCommand command);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        int ExecuteNonQuery(DbCommand command, params object[] parameterValues);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction"/> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="Transaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        int ExecuteNonQuery(Transaction transaction, DbCommand command, params object[] parameterValues);

        /// <summary>
        /// <para>Executes the <paramref name="command"/> as part of the <paramref name="transaction"/> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="transaction">
        /// <para>The <see cref="DbTransaction"/> to execute the command within.</para>
        /// </param>
        /// <param name="command">The command to execute.</param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the command. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        int ExecuteNonQuery(DbTransaction transaction, DbCommand command, params object[] parameterValues);

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
        int ExecuteNonQuery(DbTransaction transaction, DbCommand command);
        
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
        int ExecuteNonQuery(DbTransaction transaction, CommandType commandType, string commandText);
        
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
        int ExecuteNonQuery(DbTransaction transaction, string storedProcedureName, params object[] parameterValues);
        
        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues" /> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="storedProcedureName">
        /// <para>The name of the stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        int ExecuteNonQuery(string storedProcedureName, params object[] parameterValues);
        
        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> using the given <paramref name="parameterValues" /> and returns the number of rows affected.</para>
        /// </summary>
        /// <param name="commandText">
        /// <para>The name of the command text to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the command text. The parameter values must be in call order as they appear in the command text.</para>
        /// </param>
        /// <returns>
        /// <para>The number of rows affected</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        int ExecuteTextNonQuery(string commandText, params object[] parameterValues);
        
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
        int ExecuteTextNonQuery(Transaction transaction, string commandText, params object[] parameterValues);
        
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
        int ExecuteTextNonQuery(DbTransaction transaction, string commandText, params object[] parameterValues);
        
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
        object ExecuteScalar(CommandType commandType, string commandText);
        
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
        object ExecuteScalar(Transaction transaction, CommandType commandType, string commandText);
        
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
        object ExecuteScalar(DbTransaction transaction, CommandType commandType, string commandText);
        
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
        object ExecuteScalar(DbCommand command);
        
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
        object ExecuteScalar(Transaction transaction, DbCommand command);
        
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
        object ExecuteScalar(DbTransaction transaction, DbCommand command);
        
        /// <summary>
        /// <para>Executes the <paramref name="command"/> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="command">
        /// <para>The command that contains the query to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>        
        object ExecuteScalar(DbCommand command, params object[] parameterValues);

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
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        object ExecuteScalar(Transaction transaction, DbCommand command, params object[] parameterValues);

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
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the command.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        object ExecuteScalar(DbTransaction transaction, DbCommand command, params object[] parameterValues);

        /// <summary>
        /// <para>Executes the <paramref name="storedProcedureName"/> with the given <paramref name="parameterValues" /> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="storedProcedureName">
        /// <para>The stored procedure to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        object ExecuteScalar(string storedProcedureName, params object[] parameterValues);
        
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
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        object ExecuteScalar(Transaction transaction, string storedProcedureName, params object[] parameterValues);
        
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
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        /// <seealso cref="IDbCommand.ExecuteScalar"/>
        object ExecuteScalar(DbTransaction transaction, string storedProcedureName, params object[] parameterValues);
        
        /// <summary>
        /// <para>Executes the <paramref name="commandText"/> with the given <paramref name="parameterValues" /> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.</para>
        /// </summary>
        /// <param name="commandText">
        /// <para>The command text to execute.</para>
        /// </param>
        /// <param name="parameterValues">
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        object ExecuteTextScalar(string commandText, params object[] parameterValues);
        
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
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        object ExecuteTextScalar(Transaction transaction, string commandText, params object[] parameterValues);
        
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
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        /// <returns>
        /// <para>The first column of the first row in the result set.</para>
        /// </returns>
        object ExecuteTextScalar(DbTransaction transaction, string commandText, params object[] parameterValues);
        
        /// <summary>
        /// Gets a parameter value.
        /// </summary>
        /// <param name="command">The command that contains the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        object GetParameterValue(DbCommand command, string name);
        
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
        void LoadDataSet(Transaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames);
        
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
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        void LoadDataSet(Transaction transaction, string storedProcedureName, DataSet dataSet, string[] tableNames, params object[] parameterValues);
        
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
        void LoadDataSet(CommandType commandType, string commandText, DataSet dataSet, string[] tableNames);
        
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
        /// <exception cref="System.ArgumentNullException">Any input parameter was <see langword="null"/> (<b>Nothing</b> in Visual Basic);.</exception>
        /// <exception cref="System.ArgumentException">tableName was an empty string.</exception>
        void LoadDataSet(Transaction transaction, DbCommand command, DataSet dataSet, string tableName);
        
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
        void LoadDataSet(Transaction transaction, DbCommand command, DataSet dataSet, string[] tableNames);

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
        /// <exception cref="System.ArgumentNullException">Any input parameter was <see langword="null"/> (<b>Nothing</b> in Visual Basic);.</exception>
        /// <exception cref="System.ArgumentException">tableName was an empty string.</exception>
        void LoadDataSet(DbTransaction transaction, DbCommand command, DataSet dataSet, string tableName);
        
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
        void LoadDataSet(DbTransaction transaction, DbCommand command, DataSet dataSet, string[] tableNames);
        
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
        /// <exception cref="System.ArgumentNullException">Any input parameter was <see langword="null"/> (<b>Nothing</b> in Visual Basic);</exception>
        /// <exception cref="System.ArgumentException">tableName was an empty string</exception>
        void LoadDataSet(DbCommand command, DataSet dataSet, string tableName);
        
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
        void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames);
        
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
        void LoadDataSet(DbTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames);
        
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
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        void LoadDataSet(DbTransaction transaction, string storedProcedureName, DataSet dataSet, string[] tableNames, params object[] parameterValues);
        
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
        /// <para>An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</para>
        /// </param>
        void LoadDataSet(string storedProcedureName, DataSet dataSet, string[] tableNames, params object[] parameterValues);
        
        /// <summary>
        /// Pings the datasource defined by the DatabaseProfile.
        /// </summary>
        /// <returns></returns>
        System.Net.NetworkInformation.PingReply Ping();

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
        Framework.Security.SecurityContext SecurityContext { get; set; }
        
        /// <summary>
        /// Sets a parameter value.
        /// </summary>
        /// <param name="command">The command with the parameter.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        void SetParameterValue(DbCommand command, string parameterName, object value);
        
        /// <summary>
        /// Update the dataset using the default insert, update and delete commands built from the command builder.
        /// </summary>
        /// <param name="dataSet">The dataset to update.</param>
        /// <param name="tableName">The database table to update.  NOTE: The table name must also match the name of the table in the dataset.</param>
        /// <returns></returns>
        int UpdateDataSet(DataSet dataSet, string tableName);

        /// <summary>
        /// Update the dataset using the default insert, update and delete commands built from the command builder.
        /// </summary>
        /// <param name="transaction"><para>The <see cref="Transaction"/> to use.</para></param>
        /// <param name="dataSet">The dataset to update.</param>
        /// <param name="tableName">The database table to update.  NOTE: The table name must also match the name of the table in the dataset.</param>
        /// <returns></returns>
        int UpdateDataSet(Transaction transaction, DataSet dataSet, string tableName);
        
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
        int UpdateDataSet(Transaction transaction, DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand);
        
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
        int UpdateDataSet(DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand, UpdateBehavior updateBehavior);
        
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
        int UpdateDataSet(DbTransaction transaction, DataSet dataSet, string tableName, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand);
        
        /// <summary>
        /// Update the dataset using the default insert, update and delete commands built from the command builder.
        /// </summary>
        /// <param name="dataSet">The dataset to update.</param>
        /// <param name="tableName">The database table to update.  NOTE: The table name must also match the name of the table in the dataset.</param>
        /// <param name="updateAdapter"><para>The <see cref="DbDataAdapter"/> to use.</para></param>
        /// <returns></returns>
        int UpdateDataSet(DataSet dataSet, string tableName, DbDataAdapter updateAdapter);
        
        /// <summary>
        /// Update the dataset using the default insert, update and delete commands built from the command builder.
        /// </summary>
        /// <param name="transaction"><para>The <see cref="DbTransaction"/> to use.</para></param>
        /// <param name="dataSet">The dataset to update.</param>
        /// <param name="tableName">The database table to update.  NOTE: The table name must also match the name of the table in the dataset.</param>
        /// <returns></returns>
        int UpdateDataSet(DbTransaction transaction, DataSet dataSet, string tableName);
    }
}
