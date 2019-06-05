using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace SQLite_LFS_Prototype.Model
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
        public long ExtensionId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}