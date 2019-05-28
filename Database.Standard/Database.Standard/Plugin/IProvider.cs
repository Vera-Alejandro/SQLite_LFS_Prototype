using System;
using System.Collections.Generic;
using System.Text;

namespace Interstates.Control.Database.Plugin
{
    /// <summary>
    /// A provider to create queries for.
    /// </summary>
    public interface IProvider
    {
        IProviderPlugin Plugin { get; }
    }    
}
