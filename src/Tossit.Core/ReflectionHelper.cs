using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace Tossit.Core
{
    /// <summary>
    /// Reflection helper
    /// Find type, create instance, call method etc.
    /// </summary>
    public class ReflectionHelper : IReflectionHelper
    {
        /// <summary>
        /// Application assembly list to working on it.
        /// </summary>
        private readonly IList<Assembly> _assemblies = new List<Assembly>();

        /// <summary>
        /// Ctor
        /// </summary>
        public ReflectionHelper()
        {
            this.LoadAssemblies();
        }

        /// <summary>
        /// Find all given interface type's implementations and create instances.
        /// </summary>
        /// <param name="interfaceType">Type of interface to finding dependencies.</param>
        /// <returns>Returns new instances of dependencies.</returns>
        public IList<object> GetImplementationsByInterfaceType(Type interfaceType)
        {
            // Find all implementation types implemented from given interfaceType.
            var typeInfoList = _assemblies.SelectMany(a => a.DefinedTypes.Where(t => !t.IsGenericType))
                .Where(t => t.GetInterfaces().Any(i => i.GetTypeInfo().IsAssignableFrom(interfaceType)))
                .ToList();

            // Create and return instances from them.
            return typeInfoList.Select(typeInfo => Activator.CreateInstance(typeInfo.AsType())).ToList();
        }

        /// <summary>
        /// Invokes generic method like: void MethodName{T}(IInterface{T} parameter) {}
        /// </summary>
        /// <param name="name">Name of method.</param>
        /// <param name="obj">The object on which to invoke the method.</param>
        /// <param name="parameter">An argument for the invoked method.</param>
        /// <param name="genericInterfaceType">Type of generic method argument. e.g: IInterface T</param>
        /// <exception cref="InvalidOperationException">Throws when genericInterfaceType not found.</exception>>
        /// <exception cref="ArgumentNullException">Throws when obj, parameter or genericInterfaceType is null.</exception>>
        public void InvokeGenericMethod(string name, object obj, object parameter, Type genericInterfaceType)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (genericInterfaceType == null) throw new ArgumentNullException(nameof(genericInterfaceType));

            var method = GetMethod(name, obj);

            // Get method argument's interfaces.
            var parameterType = parameter.GetType();
            var interfaces = parameterType.GetInterfaces();

            // Find requested interface from them.
            var wantedInterface = interfaces.FirstOrDefault(i => i.GetTypeInfo().GetGenericTypeDefinition() == genericInterfaceType);
            if (wantedInterface == null)
            {
                throw new InvalidOperationException($"{parameterType.FullName}:{genericInterfaceType.FullName}");
            }

            // Find generic arguments from generic interface.
            var genericArguments = wantedInterface.GetTypeInfo().GetGenericArguments();

            // Make generic method with generic arguments.
            var invocable = method.MakeGenericMethod(genericArguments);

            // invokek
            invocable.Invoke(obj, new[] { parameter });
        }

        /// <summary>
        /// Get method info.
        /// </summary>
        /// <param name="name">Name of method.</param>
        /// <param name="obj">The object on which to invoke the method.</param>
        /// <returns>Returns MethodInfo instance.</returns>
        /// <exception cref="ArgumentNullException">Throws when name is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Throws when method not found.</exception>
        private MethodInfo GetMethod(string name, object obj)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            var type = obj.GetType();
            var method = type.GetMethod(name);

            if (method == null) throw new InvalidOperationException($"{type.FullName}.{name}");

            return method;
        }

        /// <summary>
        /// Load all assemblies to _asemblies field from application context.
        /// </summary>
        private void LoadAssemblies()
        {
            var context = DependencyContext.Default;

            // Find assembly names without ignored ones.
            var assemblyNames = context.GetDefaultAssemblyNames()
                .Where(an => !_ignoredNames.Any(n => an.Name.StartsWith(n)));

            // Load all found assemblies.
            foreach (var assemblyName in assemblyNames)
            {
                try
                {
                    _assemblies.Add(LoadAssembly(assemblyName));
                }
                catch
                {
                    // ignored.
                }
            }
        }

        /// <summary>
        /// LoadAssembly
        /// </summary>
        /// <param name="assemblyName">AssemblyName instance of assembly.</param>
        /// <returns>Returns loaded assembly.</returns>
        private Assembly LoadAssembly(AssemblyName assemblyName)
        {
            return LoadAssembly(assemblyName.Name);
        }

        /// <summary>
        /// LoadAssembly
        /// </summary>
        /// <param name="name">Name of assembly.</param>
        /// <returns>Returns loaded assembly.</returns>
        private Assembly LoadAssembly(string name)
        {
            return Assembly.Load(new AssemblyName(name));
        }

        /// <summary>
        /// Ignored assembly name prefixes.
        /// </summary>
        private readonly IList<string> _ignoredNames = new List<string>
        {
            "System.",
            "Microsoft.",
            "NuGet.",
            "xunit.",
            "dotnet-test-xunit",
            "mscorlib",
            "SOS.",
            "Newtonsoft.Json"
        };
    }
}