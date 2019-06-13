using System;
using System.Collections.Generic;
using SQLite_LFS_Prototype.Model;

namespace SQLite_LFS_Prototype.Comparer
{
    class ExtDataComparer : IEqualityComparer<ExtensionInfo>
    {
        public bool Equals(ExtensionInfo Data1, ExtensionInfo Data2)
        {
            if (Data1 == null && Data2 == null) { return true; }
            else if (Data1 == null || Data2 == null) { return false; }
            else
            if
            (
                Data1.Id == Data2.Id &&
                Data1.Extension == Data2.Extension
            ) { return true; }
            else { return false; }
        }


        public int GetHashCode(ExtensionInfo Data)
        {
            int _hashCode = Convert.ToInt32(Data.Id ^ Data.Extension.Length);
            return _hashCode.GetHashCode();
        }
    }
}


