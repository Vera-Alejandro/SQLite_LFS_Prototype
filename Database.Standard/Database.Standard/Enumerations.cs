using System.Runtime.InteropServices;

namespace Interstates.Control.Database
{
    /// <summary>
    /// The state of a database object. The state can be used to determine the type of DML to perform on an object.
    /// </summary>
    [ComVisible(false)]
    public enum EntityState
    {
        /// <summary>
        /// The object is unchanged.
        /// </summary>
        Unchanged = 0,
        /// <summary>
        /// The object will be a new instance in the database.
        /// </summary>
        New = 1,
        /// <summary>
        /// The object already exists in the database and is modified.
        /// </summary>
        Modified = 2,
        /// <summary>
        /// The object will be deleted from the database.
        /// </summary>
        Deleted = 3
    }

    /// <summary>
    /// Used with the Database.UpdateDataSet method. Provides control over behavior when the Data
    /// Adapter's update command encounters an error.
    /// </summary>
    [ComVisible(false)]
    public enum UpdateBehavior
    {
        /// <summary>
        /// No interference with the DataAdapter's Update command. If Update encounters
        /// an error, the update stops.  Additional rows in the Datatable are uneffected.
        /// </summary>
        Standard,
        /// <summary>
        /// If the DataAdapter's Update command encounters an error, the update will
        /// continue. The Update command will try to update the remaining rows. 
        /// </summary>
        Continue,
        /// <summary>
        /// If the DataAdapter encounters an error, all updated rows will be rolled back.
        /// </summary>
        Transactional
    }

    /// <summary>
    /// How this slot is defined
    /// </summary>
    public enum SlotType
    {
        /// <summary>
        /// This slot is positional, for example in 'example_sp 23, @x, @p4 = 37, @p3 = @y', the first two parameters are positional.
        /// </summary>
        Positional,
        /// <summary>
        /// This slot is named, for example in 'example_23, @x, @p4 = 37, @p3 = @y', the last two parameters are named
        /// </summary>
        Named
    }

    /// <summary>
    /// The enumerated stream format types for the offline file.
    /// </summary>
    public enum OfflineFormat : int
    {
        Xml = 0,	// XML format
        Binary = 1,	// Binary format
        Encrypt = 2	// Encrypted format
    }

    /// <summary>
    /// Describes the current state of the transaction.
    /// </summary>
    public enum TransactionState
    {
        /// <summary>
        /// The transaction state is not known.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// The transaction has been started.
        /// </summary>
        Active = 1,
        /// <summary>
        /// The transaction has been committed.
        /// </summary>
        Committed = 2,
        /// <summary>
        /// The transaction has been rolled back.
        /// </summary>
        Aborted = 3
    }
}
