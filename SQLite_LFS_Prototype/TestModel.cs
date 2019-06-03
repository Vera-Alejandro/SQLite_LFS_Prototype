using System;

namespace SQLite_LFS_Prototype.Model
{
    [Serializable]
    public class ExtensionInfo 
    {
        public Int64 Id { get; set; }
        public string Extension { get; set; }
    }

    [Serializable]
    public class RowData
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public Int64 ExtensionId { get; set; }
        public string Data { get; set; }
    }
}