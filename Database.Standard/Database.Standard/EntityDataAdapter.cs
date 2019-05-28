using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Interstates.Control.Database
{
    /// <summary>
    /// Provides an inteface that will select, insert, update and delete data for an entity object.
    /// </summary>
    public abstract class EntityDataAdapter<T> : IDisposable where T : IEntity
    {
        Query _query;
        System.Data.Common.DbCommand _selectCommand, _insertCommand, _updateCommand, _deleteCommand;
        protected System.Data.Common.DbDataAdapter _dataAdapter;
        System.Data.Common.DbCommandBuilder _commandBuilder;
        int _batchSize = 1; // Disable batch mode

        /// <summary>
        /// Raised when the save method completes the save in batch mode.
        /// </summary>
        public EventHandler<BatchCompleteEventArgs<T>> BatchComplete;

        /// <summary>
        /// Creates a new instance of the EntityAdapter that will use the specified query.
        /// </summary>
        /// <param name="query"></param>
        public EntityDataAdapter(Query query)
        {
            _query = query;
        }

        /// <summary>
        /// Disposes any resources that have been created.
        /// </summary>
        public void Dispose()
        {
            if (_dataAdapter != null)
                _dataAdapter.Dispose();
            if (_commandBuilder != null)
                _commandBuilder.Dispose();
            if (_selectCommand != null)
                _selectCommand.Dispose();
            if (_insertCommand != null)
                _insertCommand.Dispose();
            if (_updateCommand != null)
                _updateCommand.Dispose();
            if (_deleteCommand != null)
                _deleteCommand.Dispose();
        }

        void CreateDataAdapter()
        {
            _dataAdapter = _query.CreateDataAdapter();
            _commandBuilder = _query.CreateCommandBuilder();
            _commandBuilder.DataAdapter = _dataAdapter;

            // Select * from the entity table
            if (SelectCommand == null)
                _dataAdapter.SelectCommand = _query.CreateTextCommand("SELECT * FROM " + this.EntityTableName);
            else
                _dataAdapter.SelectCommand = SelectCommand;
            // Set the connection string on the select so that CommandBuilder GetxxCommand() calls work
            if (_dataAdapter.SelectCommand.Connection == null)
                _dataAdapter.SelectCommand.Connection = _query.CreateConnection();
        }

        /// <summary>
        /// Gets or sets the name of the table associated with entity type T.
        /// </summary>
        public abstract string EntityTableName
        {
            get;
        }

        System.Data.Common.DbCommand CreateSelectCommand()
        {
            if (_commandBuilder == null)
                CreateDataAdapter();
            return _dataAdapter.SelectCommand;
        }

        System.Data.Common.DbCommand CreateInsertCommand()
        {
            if (_commandBuilder == null)
                CreateDataAdapter();
            return _commandBuilder.GetInsertCommand(true);
        }

        System.Data.Common.DbCommand CreateUpdateCommand()
        {
            if (_commandBuilder == null)
                CreateDataAdapter();
            return _commandBuilder.GetUpdateCommand(true);
        }

        System.Data.Common.DbCommand CreateDeleteCommand()
        {
            if (_commandBuilder == null)
                CreateDataAdapter();
            return _commandBuilder.GetDeleteCommand(true);
        }

        /// <summary>
        /// Gets the query used to execute a command.
        /// </summary>
        public Query Query { get { return _query; } }

        /// <summary>
        /// Gets or sets the command to select the entity.
        /// </summary>
        public virtual System.Data.Common.DbCommand SelectCommand { get { return _selectCommand; } set { _selectCommand = value; } }

        /// <summary>
        /// Gets or sets the command to insert the entity.
        /// </summary>
        public virtual System.Data.Common.DbCommand InsertCommand { get { return _insertCommand; } set { _insertCommand = value; } }

        /// <summary>
        /// Gets or sets the command to update the entity.
        /// </summary>
        public virtual System.Data.Common.DbCommand UpdateCommand { get { return _updateCommand; } set { _updateCommand = value; } }

        /// <summary>
        /// Gets or sets the command to delete the entity.
        /// </summary>
        public virtual System.Data.Common.DbCommand DeleteCommand { get { return _deleteCommand; } set { _deleteCommand = value; } }

        /// <summary>
        /// Retrieves a list of objects.
        /// </summary>
        /// <returns></returns>
        public virtual void Get(out List<T> result)
        {
            Get(null, out result);
        }

        /// <summary>
        /// Retrieves a list of objects.
        /// </summary>
        /// <param name="transaction">The transaction to execute the command in.</param>
        /// <param name="result">When this method returns, a list of entity objects created
        /// by the SelectCommand. This parameter is passed uninitialized..</param>
        /// <returns></returns>
        public virtual void Get(Transaction transaction, out List<T> result)
        {
            result = new List<T>();
            if (SelectCommand == null)
            {
                DataTable table = CreateDataTable();
                InitializeDataAdapter(transaction);
                _dataAdapter.Fill(table);
                foreach (DataRow row in table.Rows)
                    result.Add(CreateEntityFromDataRow(row));
            }
            else
            {
                DataSet set;
                if (transaction == null)
                    set = _query.Execute(SelectCommand);
                else
                    set = _query.Execute(transaction, SelectCommand);
                foreach (DataRow row in set.Tables[0].Rows)
                    result.Add(CreateEntityFromDataRow(row));
            }
        }

        /// <summary>
        /// Creates a new entity from a data row.
        /// </summary>
        /// <param name="row">A row of data that represents that can be mapped to the entity.</param>
        /// <returns></returns>
        public abstract T CreateEntityFromDataRow(DataRow row); // return new T(row);

        /// <summary>
        /// Create a new DataTable based on a collection of entities.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public abstract DataTable CreateDataTable(ICollection<T> entities);

        /// <summary>
        /// Create a new DataTable based on the entity.
        /// </summary>
        /// <returns></returns>
        public abstract DataTable CreateDataTable();

        /// <summary>
        /// Calls the respective insert, update or delete command
        /// on the object specified to save the entity to the database.
        /// </summary>
        /// <param name="entity">The entity to save.</param>
        public virtual void Save(T entity)
        {
            Save(null, entity);
        }

        /// <summary>
        /// Calls the respective insert, update or delete command
        /// on the object specified to save the entity to the database.
        /// </summary>
        /// <param name="transaction">The transaction to save within.</param>
        /// <param name="entity">The entity to save.</param>
        public virtual void Save(Transaction transaction, T entity)
        {
            _batchSize = 1;
            switch (entity.EntityState)
            {
                case EntityState.New:
                    OnInsert(transaction, entity);
                    break;
                case EntityState.Modified:
                    OnUpdate(transaction, entity);
                    break;
                case EntityState.Deleted:
                    OnDelete(transaction, entity);
                    break;
            }
        }

        /// <summary>
        /// Calls the respective insert, update or delete command
        /// for each object in the specified collection to save the entity to the database.
        /// </summary>
        /// <param name="entities">The entities to save.</param>
        /// <param name="batchSize">A value that enables or disables batch processing support, and specifies the number of commands that can be executed in a batch.
        /// <para>When the value is 0, the default, the adapter will use the largest batch size the server can handle.</para>
        /// <para>When the value is 1 batching is disabled.</para>
        /// <para>A value > 1 will send changes to the database using the specified batch size.</para></param>
        public virtual void Save(ICollection<T> entities, int batchSize = 0)
        {
            Save(null, entities, batchSize);
        }

        /// <summary>
        /// Calls the respective insert, update or delete command
        /// for each object in the specified collection to save the entity to the database.
        /// </summary>
        /// <param name="transaction">The transaction to save within.</param>
        /// <param name="entities">The entities to save.</param>
        /// <param name="batchSize">A value that enables or disables batch processing support, and specifies the number of commands that can be executed in a batch.
        /// <para>When the value is 0, the default, the adapter will use the largest batch size the server can handle.</para>
        /// <para>When the value is 1 batching is disabled.</para>
        /// <para>A value > 1 will send changes to the database using the specified batch size.</para></param>
        public virtual void Save(Transaction transaction, ICollection<T> entities, int batchSize = 0)
        {
            _batchSize = batchSize;
            if (_batchSize != 1)
            {
                InitializeDataAdapter(transaction);
                DataTable changes = CreateDataTable(entities);
                _dataAdapter.Update(changes);
                if (BatchComplete != null)
                    BatchComplete(this, new BatchCompleteEventArgs<T>(entities, changes.Rows));
            }
            else
            {
                foreach (T entity in entities)
                    Save(transaction, entity);
            }
        }

        void InitializeDataAdapter(Transaction transaction)
        {
            // Use the DbDataAdapter to execute the commands
            if (_dataAdapter == null)
                CreateDataAdapter();
            
            _dataAdapter.UpdateBatchSize = _batchSize;

            // Use the commands that are provided, 
            // otherwise create new commands for each operation
            _dataAdapter.InsertCommand = InsertCommand;
            _dataAdapter.UpdateCommand = UpdateCommand;
            _dataAdapter.DeleteCommand = DeleteCommand;
            if (_dataAdapter.InsertCommand == null)
                _dataAdapter.InsertCommand = CreateInsertCommand();
            if (_dataAdapter.UpdateCommand == null)
                _dataAdapter.UpdateCommand = CreateUpdateCommand();
            if (_dataAdapter.DeleteCommand == null)
                _dataAdapter.DeleteCommand = CreateDeleteCommand();

            // Include the command in the transaction, if there is one
            if (transaction is Transaction)
            {
                DbTransaction tx = TransactionManager.Instance.GetTransaction(transaction) as DbTransaction;
                if (tx is DbTransaction)
                {
                    // For some reason we only need to set the connection on the select command
                    _dataAdapter.SelectCommand.Connection = tx.Connection;
                    _dataAdapter.SelectCommand.Transaction = tx;
                }
            }

            // UpdatedRowSource can't be Both or FirstReturnedRow in batch mode
            if (_dataAdapter.InsertCommand.UpdatedRowSource == UpdateRowSource.Both
                ||
                _dataAdapter.InsertCommand.UpdatedRowSource == UpdateRowSource.FirstReturnedRecord)
                _dataAdapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;
            if (_dataAdapter.UpdateCommand.UpdatedRowSource == UpdateRowSource.Both
                ||
                _dataAdapter.UpdateCommand.UpdatedRowSource == UpdateRowSource.FirstReturnedRecord)
                _dataAdapter.UpdateCommand.UpdatedRowSource = UpdateRowSource.None;
            if (_dataAdapter.DeleteCommand.UpdatedRowSource == UpdateRowSource.Both
                ||
                _dataAdapter.DeleteCommand.UpdatedRowSource == UpdateRowSource.FirstReturnedRecord)
                _dataAdapter.DeleteCommand.UpdatedRowSource = UpdateRowSource.None;
        }

        /// <summary>
        /// Insert a new entity.
        /// </summary>
        /// <param name="transaction">The transaction to save within.</param>
        /// <param name="entity">The entity to save.</param>
        protected virtual void OnInsert(Transaction transaction, T entity)
        {
            if (InsertCommand == null)
            {
                InitializeDataAdapter(transaction);
                DataTable table = CreateDataTable(new List<T>() { entity });
                _dataAdapter.Update(table);
            }
            else
            {
                if (transaction == null)
                    _query.ExecuteNonQuery(InsertCommand);
                else
                    _query.ExecuteNonQuery(transaction, InsertCommand);
            }
        }

        /// <summary>
        /// Update an entity.
        /// </summary>
        /// <param name="transaction">The transaction to save within.</param>
        /// <param name="entity">The entity to save.</param>
        protected virtual void OnUpdate(Transaction transaction, T entity)
        {
            if (UpdateCommand == null)
            {
                InitializeDataAdapter(transaction);
                _dataAdapter.Update(CreateDataTable(new List<T>() { entity }));
            }
            else
            {
                if (transaction == null)
                    _query.ExecuteNonQuery(UpdateCommand);
                else
                    _query.ExecuteNonQuery(transaction, UpdateCommand);
            }
        }

        /// <summary>
        /// Delete an entity.
        /// </summary>
        /// <param name="transaction">The transaction to save within.</param>
        /// <param name="entity">The entity to save.</param>
        protected virtual void OnDelete(Transaction transaction, T entity)
        {
            if (DeleteCommand == null)
            {
                InitializeDataAdapter(transaction);
                _dataAdapter.Update(CreateDataTable(new List<T>() { entity }));
            }
            else
            {
                if (transaction == null)
                    _query.ExecuteNonQuery(DeleteCommand);
                else
                    _query.ExecuteNonQuery(transaction, DeleteCommand);
            }
        }
    }

    /// <summary>
    /// The events that are passed to the BatchComplete event.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BatchCompleteEventArgs<T> : EventArgs
    {
        public readonly ICollection<T> Entities;
        public readonly DataRowCollection Rows;

        public BatchCompleteEventArgs(ICollection<T> entities, DataRowCollection rows)
        {
            Entities = entities;
            Rows = rows;
        }
    }
}
