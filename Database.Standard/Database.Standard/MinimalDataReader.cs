using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;

namespace Interstates.Control.Database
{
    /// <summary>
    /// The MinimalDataReader is used to minimal information from a datareader and does not actually return rows.
    /// </summary>
    [ComVisible(false)]
    public class MinimalDataReader : IDataReader
    {
        private IDataReader _dataReader;

        public MinimalDataReader(IDataReader dataReader)
        {
            if (dataReader == null)
                throw new ArgumentNullException("dataReader");
            _dataReader = dataReader;
        }

        private Exception NullReferenceException(string p)
        {
            throw new NotImplementedException();
        }

        public int Depth
        {
            get
            {
                return _dataReader.Depth;
            }
        }

        public bool IsClosed
        {
            get
            {
                return _dataReader.IsClosed;
            }
        }

        public int RecordsAffected
        {
            get
            {
                return _dataReader.RecordsAffected;
            }
        }

        public int FieldCount
        {
            get
            {
                return _dataReader.FieldCount;
            }
        }

        public DataTable GetSchemaTable()
        {
            return _dataReader.GetSchemaTable();
        }

        public bool NextResult()
        {
            return false; // we do not want it to do this.
        }

        public bool ForceNextResult()
        {
            return _dataReader.NextResult();
        }

        public bool Read()
        {
            // Don't read any records
            return false;
        }

        public void Close()
        {
            _dataReader.Close();
        }

        public bool GetBoolean(int i)
        {
            return _dataReader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return _dataReader.GetByte(i);
        }

        public long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
        {
            return _dataReader.GetBytes(i, dataIndex, buffer, bufferIndex, length);
        }

        public char GetChar(int i)
        {
            return _dataReader.GetChar(i);
        }

        public long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
        {
            return _dataReader.GetChars(i, dataIndex, buffer, bufferIndex, length);
        }

        public IDataReader GetData(int i)
        {
            return _dataReader.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return _dataReader.GetDataTypeName(i);
        }

        public DateTime GetDateTime(int i)
        {
            return _dataReader.GetDateTime(i);
        }

        public decimal GetDecimal(int i)
        {
            return _dataReader.GetDecimal(i);
        }

        public double GetDouble(int i)
        {
            return _dataReader.GetDouble(i);
        }

        public Type GetFieldType(int i)
        {
            return _dataReader.GetFieldType(i);
        }

        public float GetFloat(int i)
        {
            return _dataReader.GetFloat(i);
        }

        public Guid GetGuid(int i)
        {
            return _dataReader.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return _dataReader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return _dataReader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return _dataReader.GetInt64(i);
        }

        public string GetName(int i)
        {
            return _dataReader.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return _dataReader.GetOrdinal(name);
        }

        public string GetString(int i)
        {
            return _dataReader.GetString(i);
        }

        public object GetValue(int i)
        {
            return _dataReader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return _dataReader.GetValues(values);
        }

        public bool IsDBNull(int i)
        {
            return _dataReader.IsDBNull(i);
        }

        public object this[string s]
        {
            get
            {
                return _dataReader[s];
            }
        }

        public object this[int i]
        {
            get
            {
                return _dataReader[i];
            }
        }

        public void Dispose()
        {
            _dataReader.Dispose();
        }
    }
}
