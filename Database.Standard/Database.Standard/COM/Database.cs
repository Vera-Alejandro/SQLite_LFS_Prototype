using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Interstates.Control.Database.COM
{
    [ClassInterface(ClassInterfaceType.None), ComVisible(true)]
    public class Database : IDatabase
    {
        /// <summary>
        /// The default database profile.
        /// </summary>
        public DatabaseProfile DefaultProfile
        {
            get
            {
                return DatabaseConfiguration.DefaultProfile;
            }
        }
    }
}
