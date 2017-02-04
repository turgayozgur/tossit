using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;


namespace Tossit.Core
{
    /// <summary>
    /// Proxy of DependencyContext
    /// </summary>
    public class DependencyContextProxy : IDependencyContextProxy
    {
        /// <summary>
        /// Default property of DependencyContext.
        /// </summary>
        private DependencyContext _context => DependencyContext.Default;

        /// <summary>
        /// Get default assembly name instances from default dependency context.
        /// </summary>
        /// <returns>Returns list of assembly name instance.</returns>
        public IEnumerable<AssemblyName> GetDefaultAssemblyNames()
        {
            return _context.GetDefaultAssemblyNames();
        }
    }
}