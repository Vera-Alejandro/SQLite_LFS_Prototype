using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace SQLite_LFS_Prototype
{
    class DataModel
    {
        public partial class ExtensionInfo
        {
            public Int64 Id { get; set; }
            public String Extension { get; set; }
        }

        public partial class FileData
        {
            public Int64 Id { get; set; }
            public String Name { get; set; }
            public Int32 ExtensionId { get; set; }
            public Byte[] Data { get; set; }
        }
    }
}
