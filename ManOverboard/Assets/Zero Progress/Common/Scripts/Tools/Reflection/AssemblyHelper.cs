using System;
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
    }
}