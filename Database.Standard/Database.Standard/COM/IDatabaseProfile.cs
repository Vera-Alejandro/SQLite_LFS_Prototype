using System.Runtime.InteropServices;

namespace Interstates.Control.Database.COM
{
    /// <summary>
    /// The DatabaseProfile Interface that must be implemented for COM Visibility
    /// </summary>
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIDispatch), ComVisible(true)]
    public interface IDatabaseProfile
    {
        string ConnectionString { get; }

        string ProviderType { get; }

        int Timeout { get; }

        string Provider { get; }

        string DataSource { get; }

        string Database { get; }

        string UserId { get; }

        string Password { get; }

        bool EnablePing { get; }

        string Name { get; }
    }
}
