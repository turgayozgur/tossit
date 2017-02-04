using System.Collections.Generic;
using System.Reflection;

namespace Tossit.Core
{
    /// <summary>
    /// Proxy interface of DependencyContext
    /// </summary>
    public interface IDependencyContextProxy
    {
        /// <summary>
        /// Get default assembly name instances from default dependency context.
        /// </summary>
        /// <returns>Returns list of assembly name instance.</returns>
        IEnumerable<AssemblyName> GetDefaultAssemblyNames();
    }
}