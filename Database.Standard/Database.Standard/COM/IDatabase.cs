using System.Runtime.InteropServices;

namespace Interstates.Control.Database.COM
{
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIDispatch), ComVisible(true)]
    public interface IDatabase
    {
        DatabaseProfile DefaultProfile { get; }
    }
}
