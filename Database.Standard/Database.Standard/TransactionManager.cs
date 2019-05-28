using System;
using System.Collections;
using System.Data.Common;
using SystemTransaction = System.Transactions.Transaction;
using TransactionCompletedEventHandler = System.Transactions.TransactionCompletedEventHandler;

namespace Interstates.Control.Database
{
    /// <summary>
    /// Manages any Transaction objects that are started.
    /// </summary>
    internal sealed class TransactionManager
    {
        private static readonly TransactionManager _instance = new TransactionManager();
        public static TransactionManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private readonly Hashtable _wrappedTransactions;
        private readonly Hashtable _dbTransactions;
        private readonly object _padlock;

        private TransactionManager()
        {
            _wrappedTransactions = new Hashtable();
            _dbTransactions = new Hashtable();
            _padlock = new object();
        }

        public void StartTransaction(Transaction transaction, SystemTransaction wrappedTransaction)
        {
            lock (_padlock)
            {
                SystemTransaction tx = wrappedTransaction.Clone();

                _wrappedTransactions[transaction.TransactionId] = tx;
                _dbTransactions[tx] = transaction;
                TransactionCompletedEventHandler handler = null;
                handler = (_, args) =>
                {
                    lock (_padlock)
                    {
                        tx.TransactionCompleted -= handler;
                        Transaction foundTx = _dbTransactions[args.Transaction] as Transaction;

                        try
                        {
                            if (foundTx != null)
                                _dbTransactions.Remove(args.Transaction);
                            args.Transaction.Dispose();
                        }
                        finally
                        {
                            StopTransaction(foundTx);
                        }
                    }
                };
                tx.TransactionCompleted += handler;
            }
        }

        public void StartTransaction(Transaction transaction, DbTransaction wrappedTransaction)
        {
            lock (_padlock)
            {
                _wrappedTransactions[transaction.TransactionId] = wrappedTransaction;
            }
        }

        /// <summary>
        /// Removes the transaction and ends it.
        /// </summary>
        /// <param name="transaction"></param>
        public void StopTransaction(Transaction transaction)
        {
            lock (_padlock)
            {
                // End and remove the transaction
                try
                {
                    transaction.End();
                }
                finally
                {
                    _wrappedTransactions.Remove(transaction.TransactionId);
                }
            }
        }

        /// <summary>
        /// Returns the transaction object that was stored for the DBTransaction.
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public IDisposable GetTransaction(Transaction transaction)
        {
            lock (_padlock)
            {
                return _wrappedTransactions[transaction.TransactionId] as IDisposable;
            }
        }
    }
}
