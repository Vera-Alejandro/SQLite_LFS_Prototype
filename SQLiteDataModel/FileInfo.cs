using System;

namespace SQLiteDataModel
{
    public class FileInfo
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual ExtensionInfo ExtensionId { get; set; }
        public virtual byte[] Data { get; set; }
    }
}
