using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extension methods for the AppDomain class
    /// </summary>
    public static class AppDomainExtensions
    {
        /// <summary>
        /// Retrieves all types from all loaded assemblies that implement the
        /// specified interface
        /// </summary>
        /// <typeparam name="InterfaceType">The type of the interface to check implementation of</typeparam>
        /// <param name="thisAppDomain">The app domain to search through</param>
        /// <returns>Collection of all types implementing the interface</returns>
        public static IEnumerable<Type> GetAllTypesThatImplement<InterfaceType>(this AppDomain thisAppDomain)
        {
            return thisAppDomain.GetAllTypesThatImplement(typeof(InterfaceType));
        }

        /// <summary>
        /// Retrieves all types from all loaded assemblies that implement the
        /// specified interface
        /// </summary>
        /// <param name="thisAppDomain">The app domain to search through</param>
        /// <param name="interfaceType">The type of the interface to check implementation of</param>
        /// <returns>Collection of all types implementing the interface</returns>
        public static IEnumerable<Type> GetAllTypesThatImplement(
            this AppDomain thisAppDomain, Type interfaceType)
        {
            Assembly[] assemblies = thisAppDomain.GetAssemblies();

            HashSet<Type> typesThatImplement = new HashSet<Type>();

            foreach (Assembly assembly in assemblies)
            {
                typesThatImplement.AddRange(assembly.GetTypesThatImplement(interfaceType));
            }

            return typesThatImplement;
        }

        /// <summary>
        /// Retrieves all the types from all loaded assemblies that implement the
        /// provided base class
        /// </summary>
        /// <typeparam name="BaseClassType">The type of the base class to check inheritance of</typeparam>
        /// <param name="assembly">The assembly to search through</param>
        /// <param name="includeBaseType">True to include the base class type if it's found
        /// in the assembly, false to exclude it</param>
        /// <returns>Collection of types that extend the specified base class</returns>
        public static IEnumerable<Type> GetAllTypesThatExtend<BaseClassType>(
            this AppDomain thisAppDomain, bool includeBaseType = true)
        {
            return thisAppDomain.GetAllTypesThatExtend(typeof(BaseClassType), includeBaseType);
        }

        /// <summary>
        /// Retrieves all the types from all loaded assemblies that implement the
        /// provided base class
        /// </summary>
        /// <param name="assembly">The assembly to search through</param>
        /// <param name="baseClassType">The type of the base class to check inheritance of</param>
        /// <param name="includeBaseType">True to include the base class type if it's found
        /// in the assembly, false to exclude it</param>
        /// <returns>Collection of types that extend the specified base class</returns>
        public static IEnumerable<Type> GetAllTypesThatExtend(
            this AppDomain thisAppDomain, Type baseClassType, bool includeBaseType = true)
        {
            Assembly[] assemblies = thisAppDomain.GetAssemblies();

            HashSet<Type> typesThatImplement = new HashSet<Type>();

            foreach (Assembly assembly in assemblies)
            {
                typesThatImplement.AddRange(assembly.
                    GetTypesThatExtend(baseClassType, includeBaseType));
            }

            return typesThatImplement;
        }
    }
}