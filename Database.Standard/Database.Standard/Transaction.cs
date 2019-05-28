using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Transactions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Interstates.Control.Database
{
    /// <summary>
    /// The Transaction class is a wrapper for a transaction object. The Transaction class makes it possible to 
    /// use the referenced transaction object accross stateless interfaces like web service methods.
    /// </summary>
    [XmlSchemaProvider("GetServiceSchema")]
    public sealed class Transaction : ISerializable, IDisposable, IXmlSerializable
    {
        private bool _isStarted = false;
        System.Transactions.IsolationLevel _isolationLevel;
        Guid _transactionId;
        string _name = String.Empty;
        TransactionState _state = TransactionState.Unknown;

        /// <summary>
        /// The date the transaction was created.
        /// </summary>
        DateTime CreatedOn;

        /// <summary>
        /// Indicates that the transaction state has changed.
        /// </summary>
        public event EventHandler<TransactionStateChangedEventArgs> StateChanged;

        /// <summary>
        /// The default constructor the Transaction.
        /// </summary>
        public Transaction()
        {
            TransactionId = Guid.NewGuid();
            IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            CreatedOn = DateTime.Now;
        }

        /// <summary>
        /// Creates a new Transaction with a name.
        /// </summary>
        /// <param name="transactionName"></param>
        public Transaction(string transactionName)
            : this()
        {
            _name = transactionName;
        }

        #region Serialization
        private Transaction(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            TransactionId = (Guid)info.GetValue("transactionId", typeof(Guid));
            _name = info.GetString("name");
            IsolationLevel = (IsolationLevel)Enum.Parse(typeof(IsolationLevel), info.GetString("isolationLevel"));
            CreatedOn = info.GetDateTime("createdOn");
            TransactionState = (TransactionState)Enum.Parse(typeof(TransactionState), info.GetString("transactionState"));
            IDisposable tx = TransactionManager.Instance.GetTransaction(this) as IDisposable;
            if (tx == null)
                IsStarted = false;
            else
                IsStarted = true;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new System.ArgumentNullException("info");

            info.AddValue("transactionId", TransactionId);
            info.AddValue("name", _name);
            info.AddValue("isolationLevel", IsolationLevel.ToString());
            info.AddValue("createdOn", CreatedOn);
            info.AddValue("transactionState", TransactionState);
        }

        #region IXmlSerializer
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return TransactionSchema();
        }

        private static readonly XmlSerializer _schemaSerializer = new XmlSerializer(typeof(XmlSchema));
        private static XmlSchema TransactionSchema()
        {
            Assembly assem = typeof(Transaction).Assembly;
            using (Stream stream = assem.GetManifestResourceStream("Interstates.Control.Database.Transaction.xsd"))
            {

                return (XmlSchema)_schemaSerializer.Deserialize(new XmlTextReader(stream), null);
            }
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToFirstAttribute())
            {
                TransactionId = new Guid(reader.GetAttribute("transactionId"));
                _name = reader.GetAttribute("name");
                IsolationLevel = (IsolationLevel)Enum.Parse(typeof(IsolationLevel), reader.GetAttribute("isolationLevel"));
                CreatedOn = DateTime.Parse(reader.GetAttribute("createdOn"));
                TransactionState = (TransactionState)Enum.Parse(typeof(TransactionState), reader.GetAttribute("transactionState"));

                // Setting the IsStarted property is important so that our checks to see if the
                // Transaction is started are valid.
                IDisposable tx = TransactionManager.Instance.GetTransaction(this);
                if (tx == null)
                    IsStarted = false;
                else
                    IsStarted = true;
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("transactionId", TransactionId.ToString());
            writer.WriteAttributeString("name", _name.ToString());
            writer.WriteAttributeString("isolationLevel", IsolationLevel.ToString());
            writer.WriteAttributeString("createdOn", CreatedOn.ToString());
            writer.WriteAttributeString("transactionState", TransactionState.ToString());
        }
        #endregion XmlSerializer

        // This is the method named by the XmlSchemaProviderAttribute applied to the type.
        public static XmlQualifiedName GetServiceSchema(XmlSchemaSet xs)
        {
            // This method is called by the framework to get the schema for this type.
            // We return an existing schema from disk.

            xs.XmlResolver = new XmlUrlResolver();
            xs.Add(TransactionSchema());

            return new XmlQualifiedName("Transaction", DatabaseConfiguration.DATABASE_NAMESPACE);
        }

        #endregion Serialization

        /// <summary>
        /// Gets the isolation level for the transaction.
        /// </summary>
        [XmlIgnore]
        public System.Transactions.IsolationLevel IsolationLevel
        {
            get { return _isolationLevel; }
            private set { _isolationLevel = value; }
        }

        /// <summary>
        /// Gets the unique identifier of the transaction.
        /// </summary>
        [XmlIgnore]
        public Guid TransactionId
        {
            get { return _transactionId; }
            private set { _transactionId = value; }
        }

        /// <summary>
        /// Gets the name of the transaction.
        /// </summary>
        [XmlIgnore]
        public string TransactionName
        {
            get { return _name; }
            private set { _name = value; }
        }

        /// <summary>
        /// Returns true if the transaction is started.
        /// </summary>
        [XmlIgnore]
        public bool IsStarted
        {
            get { return _isStarted; }
            private set { _isStarted = value; }
        }

        /// <summary>
        /// Returns the state of the transaction.
        /// </summary>
        [XmlIgnore]
        public TransactionState TransactionState
        {
            get { return _state; }
            set
            {
                if (value != _state)
                {
                    _state = value;
                    OnStateChanged(_state);
                }
            }
        }
        #region Transaction Commands

        private bool _disposedValue = false; // To detect redundant calls
        /// <summary>
        /// Disposes the transaction and if the transaction is not committed, all changes will be rolled back.
        /// </summary>
        public void Dispose()
        {
            if (!_disposedValue)
            {
                try
                {
                    // Delete the transaction from the transaction manager and end the transaction
                    TransactionManager.Instance.StopTransaction(this);

                    // Unhook all subscribers
                    if (this.StateChanged != null)
                    {
                        foreach (EventHandler<TransactionStateChangedEventArgs> h in this.StateChanged.GetInvocationList())
                        {
                            this.StateChanged -= h;
                        }
                    }
                }
                catch
                {
                    // dispose should not throw exception
                }
                finally
                {
                    _disposedValue = true;
                }
            }
        }

        void OnStateChanged(TransactionState state)
        {
            if (StateChanged != null)
                StateChanged(this, new TransactionStateChangedEventArgs(state));
        }

        /// <summary>
        /// Dispose the transaction and close the connection associated with a DbTransaction.
        /// </summary>
        internal void End()
        {
            IDisposable tx = TransactionManager.Instance.GetTransaction(this);

            IsStarted = false;

            if (tx != null)
            {
                using (tx)
                {
                    System.Data.Common.DbTransaction dbTx = tx as System.Data.Common.DbTransaction;

                    if (dbTx != null && dbTx.Connection != null)
                    {
                        // Rollback and close the associated connection.
                        using (System.Data.Common.DbConnection connection = dbTx.Connection)
                        {
                            dbTx.Rollback();
                            TransactionState = TransactionState.Aborted;
                        }
                    }
                }
            }

            // The transaction was ended and a Commit or Rollback was not called
            if (TransactionState == TransactionState.Active)
                TransactionState = TransactionState.Unknown;
        }

        /// <summary>
        /// Begin a new transaction using the ambient transaction, Transaction.Current, of the current TransactionScope.
        /// </summary>
        public void Begin()
        {
            Begin(System.Transactions.Transaction.Current);
        }

        /// <summary>
        /// Begin a new transaction for the query using the ReadCommitted isolation level.
        /// </summary>
        /// <param name="query">The query used to create the connection that will begin the transaction.</param>
        public void Begin(IQuery query)
        {
            Begin(query, System.Data.IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// Begin a new transaction for the query using the specified isolation level.
        /// </summary>
        /// <param name="query">The query used to create the connection that will begin the transaction.</param>
        /// <param name="isolationLevel">The isolation level for the new transaction</param>
        public void Begin(IQuery query, System.Data.IsolationLevel isolationLevel)
        {
            if (IsStarted)
                throw new DatabaseException("The current transaction is already started.");

            SetIsolationLevel(isolationLevel);
            TransactionManager.Instance.StartTransaction(this, query.BeginDbTransaction(isolationLevel));
            IsStarted = true;
            TransactionState = TransactionState.Active;
        }

        /// <summary>
        /// Associates the System.Transactions.Transaction with the specified transaction. 
        /// </summary>
        /// <param name="currentTransaction">The transaction to enlist in.</param>
        public void Begin(System.Transactions.Transaction currentTransaction)
        {
            if (IsStarted)
                throw new DatabaseException("The current transaction is already started.");
            if (System.Transactions.Transaction.Current == null)
                throw new DatabaseException("The current transaction is not set. This may be caused by not using the TransactionScope. Create a TransactionScope before calling Transaction.Begin.");

            IsolationLevel = currentTransaction.IsolationLevel;
            TransactionManager.Instance.StartTransaction(this, currentTransaction);
            IsStarted = true;
            TransactionState = TransactionState.Active;
        }

        /// <summary>
        /// Commit a started transaction.
        /// </summary>
        public void Commit()
        {
            if (!IsStarted)
                throw new TransactionException("The transaction is not started. Call the Begin method to start the transaction.");

            IDisposable tx = TransactionManager.Instance.GetTransaction(this);

            if (tx is System.Data.Common.DbTransaction)
            {
                System.Data.Common.DbTransaction dbTx = tx as System.Data.Common.DbTransaction;

                // Close the connection associated with the transaction after the commit
                using (System.Data.Common.DbConnection connection = dbTx.Connection)
                {
                    dbTx.Commit();
                }
            }
            else
            {
                if (tx is CommittableTransaction)
                {
                    CommittableTransaction commitableTx = tx as CommittableTransaction;

                    commitableTx.Commit();
                }
            }

            TransactionState = TransactionState.Committed;
            TransactionManager.Instance.StopTransaction(this);
        }

        /// <summary>
        /// Rollback a started transaction.
        /// </summary>
        public void Rollback()
        {
            if (!IsStarted)
                throw new TransactionException("The transaction is not started. Call the Begin method to start the transaction.");

            IDisposable tx = TransactionManager.Instance.GetTransaction(this) as IDisposable;

            if (tx is System.Data.Common.DbTransaction)
            {
                System.Data.Common.DbTransaction dbTx = tx as System.Data.Common.DbTransaction;

                // Close the connection associated with the transaction after the rollback
                using (System.Data.Common.DbConnection connection = dbTx.Connection)
                {
                    dbTx.Rollback();
                }
            }
            else
            {
                if (tx is CommittableTransaction)
                {
                    CommittableTransaction commitableTx = tx as CommittableTransaction;

                    commitableTx.Rollback();
                }
            }

            TransactionState = TransactionState.Aborted;
            TransactionManager.Instance.StopTransaction(this);
        }
        #endregion Transaction Commands

        private void SetIsolationLevel(System.Data.IsolationLevel level)
        {
            switch (level)
            {
                case System.Data.IsolationLevel.Chaos:
                    IsolationLevel = System.Transactions.IsolationLevel.Chaos;
                    break;

                case System.Data.IsolationLevel.ReadCommitted:
                    IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                    break;

                case System.Data.IsolationLevel.ReadUncommitted:
                    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                    break;

                case System.Data.IsolationLevel.RepeatableRead:
                    IsolationLevel = System.Transactions.IsolationLevel.RepeatableRead;
                    break;

                case System.Data.IsolationLevel.Serializable:
                    IsolationLevel = System.Transactions.IsolationLevel.Serializable;
                    break;

                case System.Data.IsolationLevel.Snapshot:
                    IsolationLevel = System.Transactions.IsolationLevel.Snapshot;
                    break;

                case System.Data.IsolationLevel.Unspecified:
                    IsolationLevel = System.Transactions.IsolationLevel.Unspecified;
                    break;
            }
        }
    }

    /// <summary>
    /// Provides data about the state of the transaction changing.
    /// </summary>
    public class TransactionStateChangedEventArgs : EventArgs
    {
        public TransactionStateChangedEventArgs(TransactionState state)
        {
            TransactionState = state;
        }

        public readonly TransactionState TransactionState;
    }


}
