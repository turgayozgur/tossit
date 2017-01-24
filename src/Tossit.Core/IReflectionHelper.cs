using System;
using System.Collections.Generic;

namespace Tossit.Core
{
    /// <summary>
    /// Interface to helper methods for reflections.
    /// Find type, create instance, call method etc.
    /// </summary>
    public interface IReflectionHelper
    {
        /// <summary>
        /// Find all given interface type's implementations and create instances.
        /// </summary>
        /// <param name="interfaceType">Type of interface to finding dependencies.</param>
        /// <returns>Returns new instances of dependencies.</returns>
        IList<object> GetImplementationsByInterfaceType(Type interfaceType);
        /// <summary>
        /// Invokes generic method like: void MethodName{T}(IInterface{T} parameter) {}
        /// </summary>
        /// <param name="name">Name of method.</param>
        /// <param name="obj">The object on which to invoke the method.</param>
        /// <param name="parameter">An argument for the invoked method.</param>
        /// <param name="genericInterfaceType">Type of generic method argument. e.g: IInterface{T}</param>
        /// <exception cref="System.InvalidOperationException">Throws when genericInterfaceType not found.</exception>>
        /// <exception cref="System.ArgumentNullException">Throws when obj, parameter or genericInterfaceType is null.</exception>>
        void InvokeGenericMethod(string name, object obj, object parameter, Type genericInterfaceType);
    }
}