using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace SQLite_LFS_Prototype
{
    [Serializable]
    public partial class ExtensionInfo
    {
        public Int64 Id { get; set; }
        public String Extension { get; set; }
    }

    [Serializable]
    public partial class FileData
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public byte[] Data { get; set; }
        public int ExtensionId { get; set; }
        public string DateCreated { get; set; }
        public string DateUpdated { get; set; }
    }
}