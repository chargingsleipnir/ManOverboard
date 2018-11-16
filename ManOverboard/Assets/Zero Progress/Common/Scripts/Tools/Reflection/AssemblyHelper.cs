using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Utility methods for accessing information contained within an assembly
    /// </summary>
    public static class AssemblyHelper
    {
        /// <summary>
        /// Safely retrieves all of the types from the specified Assembly
        /// </summary>
        /// <param name="assembly">The assembly to retrieve the types from</param>
        /// <returns>Array of types in the assembly, or an empty array if failed</returns>
        public static Type[] GetTypesFromAssembly(this Assembly assembly)
        {
            if (assembly == null)
            {
                return new Type[0];
            }
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException)
            {
                return new Type[0];
            }
        }

        /// <summary>
        /// Retrieves all the types from the assembly that implement the
        /// provided interface
        /// </summary>
        /// <param name="assembly">The assembly to search through</param>
        /// <typeparam name="InterfaceType">The type of the interface to check whether
        /// or not it is implemented</typeparam>
        /// <returns>Collection of types that implement the specified interface</returns>
        public static IEnumerable<Type> GetTypesThatImplement<InterfaceType>(this Assembly assembly)
        {
            return assembly.GetTypesThatImplement(typeof(InterfaceType));
        }

        /// <summary>
        /// Retrieves all the types from the assembly that implement the
        /// provided interface
        /// </summary>
        /// <param name="assembly">The assembly to search through</param>
        /// <param name="interfaceType">The type of the interface to check whether
        /// or not it is implemented</param>
        /// <returns>Collection of types that implement the specified interface</returns>
        public static IEnumerable<Type> GetTypesThatImplement(this Assembly assembly, 
            Type interfaceType)
        {
            Type[] assemblyTypes = assembly.GetTypesFromAssembly();

            return assemblyTypes.Where((type) => type.IsAssignableFrom(interfaceType));
        }

        /// <summary>
        /// Retrieves all the types from the assembly that implement the
        /// provided base class
        /// </summary>
        /// <typeparam name="BaseClassType">The type of the base class to check inheritance of</typeparam>
        /// <param name="assembly">The assembly to search through</param>
        /// <param name="includeBaseType">True to include the base class type if it's found
        /// in the assembly, false to exclude it</param>
        /// <returns>Collection of types that extend the specified base class</returns>
        public static IEnumerable<Type> GetTypesThatExtend<BaseClassType>(this Assembly assembly, 
            bool includeBaseType = true) where BaseClassType:class
        {
            return assembly.GetTypesThatExtend(typeof(BaseClassType), includeBaseType);
        }

        /// <summary>
        /// Retrieves all the types from the assembly that implement the
        /// provided base class
        /// </summary>
        /// <param name="assembly">The assembly to search through</param>
        /// <param name="baseClassType">The type of the base class to check inheritance of</param>
        /// <param name="includeBaseType">True to include the base class type if it's found
        /// in the assembly, false to exclude it</param>
        /// <returns>Collection of types that extend the specified base class</returns>
        public static IEnumerable<Type> GetTypesThatExtend(this Assembly assembly, 
            Type baseClassType, bool includeBaseType = true)
        {
            Type[] assemblyTypes = assembly.GetTypesFromAssembly();

            return assemblyTypes.Where((type) => type.IsSubclassOf(baseClassType) ||
                (includeBaseType && type == baseClassType));
        }
    }
}