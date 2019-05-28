using System;
using System.Collections.Generic;
using System.Text;

namespace Interstates.Control.Database
{
    [Serializable]
    public class DatabaseException : ApplicationException
    {
        public DatabaseException() { }
        public DatabaseException(string message) : base(message) { }
        public DatabaseException(string message, Exception inner) : base(message, inner) { }
        protected DatabaseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class PrimaryKeyException : ApplicationException
    {
        public PrimaryKeyException(string message) : base(message) { }
        public PrimaryKeyException(string message, Exception inner) : base(message, inner) { }
        protected PrimaryKeyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }    
    }

    [Serializable]
    public class UniqueIndexException : ApplicationException
    {
        public UniqueIndexException(string message) : base(message) { }
        public UniqueIndexException(string message, Exception inner) : base(message, inner) { }
        protected UniqueIndexException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
