using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// A class that helps manage our dynamically created types
    /// </summary>
    internal static class DynamicAssembly
    {
        /// <summary>
        /// The name to be assigned to our dynamic assembly
        /// </summary>
        private static readonly AssemblyName assemblyName = 
            new AssemblyName("ZeroProgressDynamicAssembly");

        /// <summary>
        /// The builder responsible for creating our assembly
        /// </summary>
        private static readonly AssemblyBuilder assemblyBuilder =
            AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);

        /// <summary>
        /// The module that will store the types
        /// </summary>
        private static readonly ModuleBuilder moduleBuilder =
            assemblyBuilder.DefineDynamicModule("MainModule");
        
        /// <summary>
        /// Retrieves a TypeBuilder for this module
        /// </summary>
        /// <param name="newTypeName">The name to assign to the type</param>
        /// <param name="typeAttributes">The attributes to be assigned to the type</param>
        /// <param name="parent">The parent of the type</param>
        /// <param name="interfaces">Any interfaces that the type implements</param>
        /// <returns>The type builder to be used to generate the new type</returns>
        internal static TypeBuilder GetTypeBuilder(string newTypeName, 
            TypeAttributes typeAttributes = 
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout, 
            Type parent = null, Type[] interfaces = null)
        {
            Type existingType = moduleBuilder.GetType(newTypeName, ignoreCase: true);

            if (existingType != null)
                return null;
            
            return moduleBuilder.DefineType(newTypeName, typeAttributes, parent, interfaces);
        }
        
        /// <summary>
        /// Retrieves a type previously created
        /// </summary>
        /// <param name="typeName">The name of the type to retrieve</param>
        /// <returns>The found type, or null if not found</returns>
        internal static Type GetCachedType(String typeName)
        {
            Type existingType = moduleBuilder.GetType(typeName, 
                throwOnError: false, ignoreCase: true);

            if (existingType == null)
                return null;
                       
            return existingType;
        }
    }
}