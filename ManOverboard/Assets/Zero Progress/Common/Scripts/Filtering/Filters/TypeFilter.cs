using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Comparison modes for comparing two types
    /// </summary>
    public enum TypeComparison
    {
        /// <summary>
        /// Check equality between the two types
        /// </summary>
        Equals,
        /// <summary>
        /// Check if a type inherits the other type
        /// </summary>
        Inherits,
        /// <summary>
        /// Check if a type implements the type of an interface
        /// </summary>
        Implements
    }

    /// <summary>
    /// Extensions for the TypeComparison enum
    /// </summary>
    public static class TypeComparisonExtensions
    {
        /// <summary>
        /// Retrieves the function that should be used to compare two types
        /// based on the selected enum mode
        /// </summary>
        /// <param name="ComparisonMode">The mode to get the function for</param>
        /// <returns>The delegate for evaluating the two types</returns>
        public static Func<Type, Type, bool> GetTypeComparisonDelegate(this TypeComparison ComparisonMode)
        {
            switch (ComparisonMode)
            {
                case TypeComparison.Equals:
                    return CheckTypeEquality;                   
                case TypeComparison.Inherits:
                    return CheckTypeInherits;
                case TypeComparison.Implements:
                    return CheckTypeImplements;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Determines if the two provided types are equal
        /// </summary>
        /// <param name="T1">The first type</param>
        /// <param name="T2">The second type</param>
        /// <returns>True if they are equal, false if not</returns>
        public static bool CheckTypeEquality(Type T1, Type T2)
        {
            if (T1 == null)
                return false;

            return T1.Equals(T2);
        }

        /// <summary>
        /// Determines if a type inherits from another type
        /// </summary>
        /// <param name="EvalType">The type to check for inheritance</param>
        /// <param name="ParentType">The parent type used to determine inheritance</param>
        /// <returns>True if it inherits, false if not</returns>
        public static bool CheckTypeInherits(Type EvalType, Type ParentType)
        {
            if (EvalType == null)
                return false;

            return EvalType.IsSubclassOf(ParentType);
        }

        /// <summary>
        /// Determines if a type implements an interface type
        /// </summary>
        /// <param name="EvalType">The type to check implementation</param>
        /// <param name="InterfaceType">The type representing the interface</param>
        /// <returns>True if the interface is implemented, false if not</returns>
        public static bool CheckTypeImplements(Type EvalType, Type InterfaceType)
        {
            if (EvalType == null || InterfaceType == null)
                return false;

            return EvalType.IsAssignableFrom(InterfaceType) && InterfaceType.IsInterface;
        }
    }

    /// <summary>
    /// A filter that works upon the types of objects
    /// </summary>
    [Serializable]
    public class TypeFilter : Filter<TypeReference>, IFilter<Type>
    {
        [Tooltip("How to compare the types when filtering")]
        public TypeComparison ComparisonMode = TypeComparison.Equals;

        /// <summary>
        /// Determines if the passed item passes the filter criteria or not
        /// </summary>
        /// <param name="Item">The item to have validity determined for</param>
        /// <returns>True if the passed item is valid, false if not</returns>
        public override bool IsValidItem(TypeReference Item)
        {
            return IsValidItem(Item.Type);
        }

        /// <summary>
        /// Determines if the passed item passes the filter criteria or not
        /// </summary>
        /// <param name="Item">The item to have validity determined for</param>
        /// <returns>True if the passed item is valid, false if not</returns>
        public bool IsValidItem(Type Item)
        {
            return IsValidItem(ComparisonMode, Item, FilterItems, IsBlacklist);
        }

        /// <summary>
        /// Determines if the passed type passes the filter criteria or not
        /// </summary>
        /// <param name="ComparisonMode">The type comparison mode to implement</param>
        /// <param name="Item">The item to be evaluated</param>
        /// <param name="FilterItems">The collection of items that the compare item will be compared against</param>
        /// <param name="IsBlacklist">True if it should behave like a blacklist, false if not</param>
        /// <returns>True if the type is considered valid, false if not</returns>
        public static bool IsValidItem(TypeComparison ComparisonMode, Type Item, 
            IEnumerable<TypeReference> FilterItems, bool IsBlacklist = true)
        {
            Func<Type, Type, bool> evaluationFunc = ComparisonMode.GetTypeComparisonDelegate();

            bool foundMatch = false;

            foreach (TypeReference filterItem in FilterItems)
            {
                foundMatch = evaluationFunc(Item, filterItem);

                if (foundMatch)
                    break;
            }

            if (IsBlacklist)
                return !foundMatch;
            else
                return foundMatch;
        }
    }
}
