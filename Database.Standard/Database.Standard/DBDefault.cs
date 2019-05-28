using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Globalization;

namespace Interstates.Control.Database
{
    /// <summary>
    /// The DBDefault class allows you to use the default value of a stored procedure.
    /// </summary>
    [Serializable, ComVisible(false)]
    public sealed class DBDefault : ISerializable, IConvertible
    {
        // Fields
        public static readonly DBDefault Value = new DBDefault();

        // Methods
        private DBDefault()
        {
        }

        private DBDefault(SerializationInfo info, StreamingContext context)
        {
            throw new NotSupportedException("Not Supported DBDefault Serial");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.SetType(typeof(DBDefault));
            info.AddValue("Data", this, typeof(string));
        }
        
        public TypeCode GetTypeCode()
        {
            return TypeCode.DBNull;
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from DBDefault");
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from DBDefault");
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from DBDefault");
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from DBDefault");
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from DBDefault");
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from DBDefault");
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from DBDefault");
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from DBDefault");
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from DBDefault");
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from DBDefault");
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from DBDefault");
        }

        object IConvertible.ToType(Type type, IFormatProvider provider)
        {
            return DefaultToType(this, type, provider);
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from DBDefault");
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from DBDefault");
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new InvalidCastException("Invalid Cast from DBDefault");
        }

        public override string ToString()
        {
            return string.Empty;
        }

        public string ToString(IFormatProvider provider)
        {
            return string.Empty;
        }

        internal static readonly Type[] ConvertTypes = new Type[] { 
        typeof(object), typeof(DBNull), typeof(bool), typeof(char), typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal), 
        typeof(DateTime), typeof(object), typeof(string)};

        internal static object DefaultToType(IConvertible value, Type targetType, IFormatProvider provider)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }
            if (value.GetType() == targetType)
            {
                return value;
            }
            if (targetType == ConvertTypes[3])
            {
                return value.ToBoolean(provider);
            }
            if (targetType == ConvertTypes[3])
            {
                return value.ToChar(provider);
            }
            if (targetType == ConvertTypes[4])
            {
                return value.ToSByte(provider);
            }
            if (targetType == ConvertTypes[5])
            {
                return value.ToByte(provider);
            }
            if (targetType == ConvertTypes[6])
            {
                return value.ToInt16(provider);
            }
            if (targetType == ConvertTypes[7])
            {
                return value.ToUInt16(provider);
            }
            if (targetType == ConvertTypes[8])
            {
                return value.ToInt32(provider);
            }
            if (targetType == ConvertTypes[9])
            {
                return value.ToUInt32(provider);
            }
            if (targetType == ConvertTypes[10])
            {
                return value.ToInt64(provider);
            }
            if (targetType == ConvertTypes[11])
            {
                return value.ToUInt64(provider);
            }
            if (targetType == ConvertTypes[12])
            {
                return value.ToSingle(provider);
            }
            if (targetType == ConvertTypes[13])
            {
                return value.ToDouble(provider);
            }
            if (targetType == ConvertTypes[14])
            {
                return value.ToDecimal(provider);
            }
            if (targetType == ConvertTypes[0xF])
            {
                return value.ToDateTime(provider);
            }
            if (targetType == ConvertTypes[0x11])
            {
                return value.ToString(provider);
            }
            if (targetType == ConvertTypes[0])
            {
                return value;
            }
            if (targetType == ConvertTypes[1])
            {
                throw new InvalidCastException("Invalid Cast to DBDefault");
            }
            throw new InvalidCastException(string.Format(CultureInfo.CurrentCulture, string.Format("Invalid Cast From {0} To {1}", value.GetType().FullName, targetType.FullName)));
        }

    }
}