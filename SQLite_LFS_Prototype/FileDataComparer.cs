using System.Collections.Generic;
using System.Text;
using SQLite_LFS_Prototype.Model;

namespace SQLite_LFS_Prototype.Comparer
{
    public class FileDataComparer : IEqualityComparer<FileData>
    {
        public bool Equals(FileData Data1, FileData Data2)
        {
            if (Data1 == null && Data2 == null) { return true; }
            else if (Data1 == null || Data2 == null) { return false; }
            else 
            if
            (
                Data1.Id == Data2.Id &&
                Data1.Type == Data2.Type &&
                Data1.DateCreated.Year == Data2.DateCreated.Year &&
                Data1.DateCreated.Month == Data2.DateCreated.Month &&
                Data1.DateCreated.Day == Data2.DateCreated.Day &&
                Data1.DateCreated.Hour == Data2.DateCreated.Hour &&
                Data1.DateCreated.Minute == Data2.DateCreated.Minute &&
                Data1.DateCreated.Second == Data2.DateCreated.Second &&
                Data1.DateCreated.Millisecond == Data2.DateCreated.Millisecond &&
                Data1.DateUpdated.Year == Data2.DateUpdated.Year &&
                Data1.DateUpdated.Month == Data2.DateUpdated.Month &&
                Data1.DateUpdated.Day == Data2.DateUpdated.Day &&
                Data1.DateUpdated.Hour == Data2.DateUpdated.Hour &&
                Data1.DateUpdated.Minute == Data2.DateUpdated.Minute &&
                Data1.DateUpdated.Second == Data2.DateUpdated.Second &&
                Data1.DateUpdated.Millisecond == Data2.DateUpdated.Millisecond
            )
            {
                if (Data1.ExtensionId == 1)
                {
                    if (Encoding.ASCII.GetString(Data1.Data) == Encoding.ASCII.GetString(Data2.Data)) { return true; }
                    else { return false; }
                }
                else
                {
                    if (Data1.Data.Length == Data2.Data.Length) { return true; }
                    else { return false; }
                }
            }
            else { return false; }
        }


        public int GetHashCode(FileData Data)
        {
            int _hashCode = Data.DateUpdated.Second ^ Data.DateCreated.Second ^ Data.DateCreated.Minute;
            return _hashCode.GetHashCode();
        }
    }
}
