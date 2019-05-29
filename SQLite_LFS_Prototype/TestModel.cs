using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite_LFS_Prototype.Model
{
    public class ExtensionInfo 
    {
        public Int64 Id { get; set; }
        public string Extension { get; set; }
    }

    public class RowData
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public Int64 ExtensionId { get; set; }
        public string Data { get; set; }
    }
}