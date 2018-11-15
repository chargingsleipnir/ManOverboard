using System;
using System.Linq;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extensions for the System.Type class
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Helper to check if this type is assignable from the provided interface type
        /// </summary>
        /// <typeparam name="I">The interface type to check</typeparam>
        /// <param name="type">This type to compare</param>
        /// <param name="interfaceType">The interface type to check</param>
        /// <returns>True if this type is assignable from the provided generic type, false if not</returns>
        public static bool IsAssignableFromGenericInterface(this Type type, Type genericInterface, params Type[] generics)
        {
            return type.GetInterfaces().Any(x =>
                  x.IsGenericType &&
                  x.GetGenericTypeDefinition() == genericInterface
                  && x.GetGenericArguments().Intersect(generics).Count() == generics.Length);
        }

        /// <summary>
        /// Checks if the provided type is a subclass of a generic class
        /// without requiring the generic parameter
        /// 
        /// i.e. checking if it's a subclass of List<> instead of List<string></string>
        /// </summary>
        /// <param name="toCheck">The type to check</param>
        /// <param name="generic">The generic type to check</param>
        /// <returns>True if toCheck inherits from generic, false if not</returns>
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        /// <summary>
        /// Determines if this type is equal or is a descendent of the provided
        /// base class type
        /// </summary>
        /// <param name="thisType">The type to be evaluated</param>
        /// <param name="baseClassType">The base class type to evaluate</param>
        /// <returns>True if this type is equal or a descendant to the base class type,
        /// false if not</returns>
        public static bool IsOrIsSubclassOf(this Type thisType, Type baseClassType)
        {
            if (thisType == null)
                return false;

            return thisType.IsSubclassOf(baseClassType) ||
                thisType == baseClassType;
        }
    }
}