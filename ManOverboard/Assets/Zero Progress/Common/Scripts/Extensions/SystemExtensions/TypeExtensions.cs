using System;
using System.Linq;

namespace ZeroProgress.Common
{
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
    }
}