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
        /// Find all implementations types that implemented by given interface type.
        /// </summary>
        /// <param name="interfaceType">Type of interface to finding dependencies types.</param>
        /// <returns>Returns type of dependencies.</returns>
        IEnumerable<Type> GetTypesThatImplementedByInterface(Type interfaceType);

        /// <summary>
        /// Filter objects by given interface type.
        /// </summary>
        /// <param name="objects">List of object to filtering.</param>
        /// <param name="interfaceType">Type of interface to finding dependencies.</param>
        /// <returns>Returns list of object that implemented by given interface type.</returns>
        /// <exception cref="System.ArgumentNullException">Throws when objects is null.</exception>
        IEnumerable<T> FilterObjectsByInterface<T>(IEnumerable<T> objects, Type interfaceType);

        /// <summary>
        /// Invokes generic method like: void MethodName{T}(IInterface{T} parameter) {}
        /// </summary>
        /// <param name="name">Name of method.</param>
        /// <param name="obj">The object on which to invoke the method.</param>
        /// <param name="parameter">An argument for the invoked method.</param>
        /// <param name="genericInterfaceType">Type of generic method argument. e.g: IInterface{T}</param>
        /// <exception cref="System.InvalidOperationException">Throws when genericInterfaceType not found.</exception>
        /// <exception cref="System.ArgumentNullException">Throws when obj, parameter or genericInterfaceType is null.</exception>
        void InvokeGenericMethod(string name, object obj, object parameter, Type genericInterfaceType);
    }
}