using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace Interstates.Control.Database
{
    /// <summary>
    /// Represents the method that will handle the initialization of the asynchronous operation.
    /// </summary>
    /// <param name="data">The data that was used to run the FillSync command.</param>
    public delegate void FillInitializedHandler(FillDataParam data);
    /// <summary>
    /// Represents the method that will handle the fill completed asynchronous operation.
    /// </summary>
    /// <param name="data">The data that was used to run the FillSync command.</param>
    public delegate void FillCompletedHandler(FillDataParam data);
    /// <summary>
    /// Represents the method that will handle the fill data chunk asynchronous operation.
    /// </summary>
    /// <param name="data">The data that was used to run the FillSync command.</param>
    /// <param name="firstRecordRead">The record within the entire result that was read.</param>
    /// <param name="recordsRead">The number of records read in this operation.</param>
    public delegate void FillDataChunkHandler(FillDataParam data, int firstRecordRead, int recordsRead);

    /// <summary>
    /// The AsyncDataAdapter class is used to retrieve data from DBDataAdapters that support Asynchronous operations.
    /// </summary>
    [ComVisible(false)]
    public class AsyncDataAdapter : DbDataAdapter
    {
        private BackgroundWorker _workerThread;

        /// <summary>
        /// The FillInitialized event that will be used to indicate that the DataAdapter is ready to fill the DataSet.
        /// </summary>
        public event FillInitializedHandler FillInitialized;
        /// <summary>
        /// The FillCompleted event is raised when the FillAsync operation has completed.
        /// </summary>
        public event FillCompletedHandler FillCompleted;
        /// <summary>
        /// The FillDataChunk event is raised when another page of data is read.
        /// </summary>
        public event FillDataChunkHandler FillDataChunk;

        /// <summary>
        /// Default constructor for the AsyncDataAdapter
        /// </summary>
        public AsyncDataAdapter()
        {
        }

        /// <summary>
        /// The IsExecuting property is used to determine if the Adapter is involved in an asynchronous operation.
        /// </summary>
        public bool IsExecuting { get; private set; } = false;

        /// <summary>
        /// The PageSize property determines how many records will be retrieved by the DataAdapter in a Fill operation.
        /// </summary>
        public int PageSize { get; set; } = 1000;

        /// <summary>
        /// The CancelFill property will cancel the asynchronous fill operation.
        /// </summary>
        public void CancelFill()
        {
            if (_workerThread != null)
            {
                _workerThread.CancelAsync();
            }
        }

        private void BeginExecuteReader(FillDataParam state)
        {
            Log("Running FileAsync...");
            OpenSelectConnection();
            Log("Beginning the asynchronous execution of the reader...");
            ((SqlCommand)SelectCommand).BeginExecuteReader(Execute, state);
        }

        /// <summary>
        /// The FillAsync method will open the connection of the <see cref="DbDataAdapter.SelectCommand"/>. 
        /// The <see cref="DbDataAdapter.SelectCommand"/> is executed asynchronously and the results are returned through the <see cref="FillCompleted"/> event.
        /// <seealso cref="DbDataAdapter.SelectCommand"/>
        /// </summary>
        /// <param name="data">The DataSet to fill with the results of the <see cref="DbDataAdapter.SelectCommand"/></param>
        public void FillAsync(DataSet data) => BeginExecuteReader(new FillDataParam(data));

        /// <summary>
        /// The FillAsync method will open the connection of the <see cref="DbDataAdapter.SelectCommand"/>. 
        /// The <see cref="DbDataAdapter.SelectCommand"/> is executed asynchronously and the results are returned through the <see cref="FillCompleted"/> event.
        /// <seealso cref="DbDataAdapter.SelectCommand"/>
        /// </summary>
        /// <param name="dataTable">The DataTable to fill with the results of the <see cref="DbDataAdapter.SelectCommand"/>.</param>
        public void FillAsync(DataTable dataTable) => BeginExecuteReader(new FillDataParam(dataTable));

        /// <summary>
        /// The FillAsync method will open the connection of the <see cref="DbDataAdapter.SelectCommand"/>. 
        /// The <see cref="DbDataAdapter.SelectCommand"/> is executed asynchronously and the results are returned through the <see cref="FillCompleted"/> event.
        /// <seealso cref="DbDataAdapter.SelectCommand"/>
        /// </summary>
        /// <param name="data">The DataSet to fill with the results of the <see cref="DbDataAdapter.SelectCommand"/>.</param>
        /// <param name="sourceTableName">The table name within the dataset to fill.</param>
        public void FillAsync(DataSet data, string sourceTableName) => BeginExecuteReader(new FillDataParam(data, sourceTableName));

        /// <summary>
        /// The FillAsync method will open the connection of the <see cref="DbDataAdapter.SelectCommand"/>. 
        /// The <see cref="DbDataAdapter.SelectCommand"/> is executed asynchronously and the results are returned through the <see cref="FillCompleted"/> event.
        /// <seealso cref="DbDataAdapter.SelectCommand"/>
        /// </summary>
        /// <param name="startRecord">The first record of the <see cref="DbDataAdapter.SelectCommand"/> to begin at.</param>
        /// <param name="maxRecords">The maximum number of records to select from the <see cref="DbDataAdapter.SelectCommand"/>.</param>
        /// <param name="dataTables">The DataTables to fill with the <see cref="DbDataAdapter.SelectCommand"/>.</param>
        public void FillAsync(int startRecord, int maxRecords, params DataTable[] dataTables) => BeginExecuteReader(new FillDataParam(startRecord, maxRecords, dataTables));

        /// <summary>
        /// The FillAsync method will open the connection of the <see cref="DbDataAdapter.SelectCommand"/>. The <see cref="DbDataAdapter.SelectCommand"/> is executed asynchronously and the results are returned through the <see cref="FillCompleted"/> event.
        /// <seealso cref="DbDataAdapter.SelectCommand"/>
        /// </summary>
        /// <param name="data">The DataSet to fill with the results of the <see cref="DbDataAdapter.SelectCommand"/>.</param>
        /// <param name="startRecord">The first record of the <see cref="DbDataAdapter.SelectCommand"/> to begin at.</param>
        /// <param name="maxRecords">The maximum number of records to select from the <see cref="DbDataAdapter.SelectCommand"/>.</param>
        /// <param name="dataTable">The name of the DataTable to fill with the <see cref="DbDataAdapter.SelectCommand"/>.</param>
        public void FillAsync(DataSet data, int startRecord, int maxRecords, string dataTable) => BeginExecuteReader(new FillDataParam(data, startRecord, maxRecords, dataTable));

        private void Execute(IAsyncResult result)
        {
            Log("Starting FillAsync invoke...");
            FillDataParam fillData = (FillDataParam)result.AsyncState;
            try
            {
                IsExecuting = true;
                fillData.DataReader = ((SqlCommand)SelectCommand).EndExecuteReader(result);
                MinimalDataReader minimalReader = new MinimalDataReader(fillData.DataReader);

                Log("Starting initial table fills...");
                if (fillData.DataSet != null)
                {
                    Fill(fillData.DataSet, fillData.SourceTableName, minimalReader, 0, PageSize);
                }
                else
                {
                    Fill(fillData.DataTables, minimalReader, 0, PageSize);
                }

                if (FillInitialized != null)
                {
                    FillInitialized(fillData);
                    if (fillData.Cancel)
                    {
                        return; // Don't start the data retrieval
                    }
                }

                Log("Creating new background worker...");
                if (_workerThread != null)
                {
                    Log("Disposing former background worker...");
                    _workerThread.DoWork -= new DoWorkEventHandler(FillDataSet);
                    _workerThread.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(CompleteFillDataSet);
                    _workerThread.Dispose();
                }
                _workerThread = new BackgroundWorker();
                _workerThread.DoWork += new DoWorkEventHandler(FillDataSet);
                _workerThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompleteFillDataSet);
                _workerThread.WorkerSupportsCancellation = true;

                Log("Starting new background worker...");
                _workerThread.RunWorkerAsync(fillData);
            }
            catch (Exception ex)
            {
                // Save the exception
                fillData.Error = ex;
                CompleteFillDataSet(this, new RunWorkerCompletedEventArgs(fillData, null, false));
            }
        }

        private void OpenSelectConnection()
        {
            if (!(SelectCommand is SqlCommand))
            {
                throw new DatabaseException("The SelectCommand of the AsyncDataAdapter must be a SqlCommand object");
            }

            if (SelectCommand.Connection == null)
            {
                throw new InvalidOperationException("The DataAdapter SelectCommand Connection property cannot be null.");
            }

            Log("Checking if the database connection is open...");

            if (SelectCommand.Connection.State == ConnectionState.Closed)
            {
                Log("Database connection is not open, now openning...");
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(SelectCommand.Connection.ConnectionString)
                {
                    AsynchronousProcessing = true // Add the Asynchronous processing attribute to the connection string
                };

                //const string asyncProcessingColumnName = "Asynchronous Processing";
                //if (builder[asyncProcessingColumnName] == null || !(bool)builder[asyncProcessingColumnName])
                //{
                //    builder[asyncProcessingColumnName] = true;
                //}

                SelectCommand.Connection.ConnectionString = builder.ConnectionString;
                SelectCommand.Connection.Open();
            }
        }

        private void CompleteFillDataSet(object sender, RunWorkerCompletedEventArgs e)
        {
            Log("Fill data background worker complete.");

            var result = (FillDataParam)e.Result;
            if (result.Error == null && e.Error != null)
            {
                result.Error = e.Error;
            }
            FillCompleted?.Invoke(result);
        }

        private void FillDataSet(object sender, DoWorkEventArgs e)
        {
            Log("Starting FillDataSet invoke...");
            FillDataParam fillData = (FillDataParam)e.Argument;
            try
            {
                int startRecordIndex = fillData.StartRecord;
                int recordCount = 0;

                Log("Starting internal fill while loop...");
                while (!fillData.DataReader.IsClosed && !_workerThread.CancellationPending)
                {
                    bool fillCompleted = false;

                    try
                    {
                        Log("Filling tables...");
                        if (fillData.DataSet != null)
                        {
                            recordCount = Fill(fillData.DataSet, fillData.SourceTableName, fillData.DataReader, 0, PageSize);
                        }
                        else
                        {
                            recordCount = Fill(fillData.DataTables, fillData.DataReader, 0, PageSize);
                        }

                        Log($"Tables filled. Record count={recordCount}");

                        fillCompleted = recordCount == 0 || recordCount != PageSize;

                        Log($"Fill completed boolean={fillCompleted}");

                        if (FillDataChunk != null)
                        {
                            FillDataChunk(fillData, startRecordIndex, recordCount);
                            if (fillData.Cancel)
                            {
                                _workerThread.CancelAsync();
                                SelectCommand.Cancel();
                                fillCompleted = true;
                            }
                        }

                        startRecordIndex += recordCount;

                        Log($"Next start instance: {startRecordIndex}");
                    }
                    catch (Exception ex)
                    {
                        fillCompleted = true;
                        throw new InvalidOperationException("Failed to fill data chunk.", ex);
                    }
                    finally
                    {
                        Log($"Closing resources.");

                        if (fillData.DataReader != null && fillCompleted)
                        {
                            fillData.DataReader.Close();
                        }
                        if (SelectCommand.Connection != null)
                        {
                            SelectCommand.Connection.Close();
                        }
                    }
                }
                IsExecuting = false;
            }
            catch (Exception ex)
            {
                fillData.Error = ex;
            }
            finally
            {
                e.Result = fillData;
            }
        }

        private static void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[{DateTimeOffset.Now.ToString("o")}][tid:{System.Threading.Thread.CurrentThread.ManagedThreadId.ToString("0000")}] {message}");
            Framework.Log.ApplicationLog.WriteVerbose(message);
        }
    }

    /// <summary>
    /// The FillDataParam class is used with the AsyncDataAdapter<seealso cref="AsyncDataAdapter"/> class to pass parameters to the caller that initiated the asynchronous operation.
    /// </summary>
    public sealed class FillDataParam
    {
        /// <summary>
        /// The exception that was caused by the operation.
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// The DataSet that will be filled by the fill operation.
        /// </summary>
        public DataSet DataSet { get; set; }

        /// <summary>
        /// The DataTables specified by the FillAsync operation.
        /// </summary>
        public DataTable[] DataTables { get; set; }

        /// <summary>
        /// The first record of the result to place in the DataSet.
        /// </summary>
        public int StartRecord { get; set; } = 0;

        /// <summary>
        /// The maximum number of records to place in the DataSet.
        /// </summary>
        public int MaxRecords { get; set; } = 0;

        /// <summary>
        /// If specified, the name of the table within the DataSet to store data from the fill operation.
        /// </summary>
        public string SourceTableName { get; set; }

        /// <summary>
        /// The underlying DbDataReader used for the fill operation.
        /// </summary>
        public DbDataReader DataReader { get; set; } = null;

        /// <summary>
        /// The Cancel property is used to notify the DataAdapter that it must stop executing the query.
        /// </summary>
        public bool Cancel { get; set; } = false;

        /// <summary>
        /// FillDataParam Constructor 
        /// </summary>
        /// <param name="data">The DataSet to fill.</param>
        public FillDataParam(DataSet data)
        {
            DataSet = data;
        }

        /// <summary>
        /// FillDataParam Constructor 
        /// </summary>
        /// <param name="data">The DataSet to fill.</param>
        /// <param name="sourceTableName">The name of the table within the DataSet to fill.</param>
        public FillDataParam(DataSet data, string sourceTableName)
        {
            DataSet = data;
            SourceTableName = sourceTableName;
        }

        /// <summary>
        /// FillDataParam Constructor 
        /// </summary>
        /// <param name="datatable">The DataTable to fill.</param>
        public FillDataParam(DataTable datatable)
        {
            DataTables = new DataTable[] { datatable };
        }

        /// <summary>
        /// FillDataParam Constructor 
        /// </summary>
        /// <param name="startRecord">The first record to use from the query result.</param>
        /// <param name="dataTables">The DataTables to fill with the query result.</param>
        /// <param name="maxRecords">The maximum number of records to place in the DataTables.</param>
        public FillDataParam(int startRecord, int maxRecords, params DataTable[] dataTables)
        {
            StartRecord = startRecord;
            MaxRecords = maxRecords;
            DataTables = dataTables;
        }

        /// <summary>
        /// FillDataParam Constructor 
        /// </summary>
        /// <param name="data">The DataSet to fill.</param>
        /// <param name="startRecord">The first record to use from the query result.</param>
        /// <param name="maxRecords">The maximum number of records to place in the DataTables.</param>
        /// <param name="datatable">The name of the table within the DataSet to fill.</param>
        public FillDataParam(DataSet data, int startRecord, int maxRecords, string datatable)
        {
            DataSet = data;
            StartRecord = startRecord;
            MaxRecords = maxRecords;
            SourceTableName = datatable;
        }

        public FillDataParam(Exception error)
        {
            Error = error;
        }
    }
}
