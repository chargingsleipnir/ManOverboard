using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// A filter that determines a Game Objects' validity based on whether
    /// or not it has a component of a specific type
    /// </summary>
    [Serializable]
    public class GameObjectComponentFilter : IFilter<Type>, IFilter<GameObject>
    {
        // In order to filter the TypeReference collection display to include
        // only classes that extend from Component, we had to reimplement the
        // logic from the Filter<T> class specifically for this one :(
        // (needed the freedom to specify the ClassExtends attribute)


        [Tooltip("How to compare the types when filtering")]
        public TypeComparison ComparisonMode = TypeComparison.Equals;

        [Tooltip("True if the FilterItems collection should be treated as a listing of items" +
            "that result in an invalid match, false if they should result in a valid match")]
        public bool IsBlacklist = true;

        [SerializeField]
        [ClassExtends(typeof(Component), AllowAbstract = false, Grouping = ClassGrouping.ByNamespaceFlat)]
        [Tooltip("The collection of items that represent valid or invalid items, depending" +
            "on whether or not this collection is treated as a blacklist or not")]
        protected List<TypeReference> FilterItems = new List<TypeReference>();

        /// <summary>
        /// Determines if the passed item passes the filter criteria or not
        /// </summary>
        /// <param name="Item">The item to have validity determined for</param>
        /// <returns>True if the passed item is valid, false if not</returns>
        public virtual bool IsValidItem(TypeReference Item)
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
            return TypeFilter.IsValidItem(ComparisonMode, Item, FilterItems, IsBlacklist);
        }

        /// <summary>
        /// Determines if the passed item passes the filter criteria or not
        /// </summary>
        /// <param name="Item">The item to have validity determined for</param>
        /// <returns>True if the passed item is valid, false if not</returns>
        public bool IsValidItem(GameObject Item)
        {
            bool foundMatch = false;

            if (FilterItems.Count == 1)
                foundMatch = SingleTypeValidation(Item);
            else if (FilterItems.Count > 1)
                foundMatch = MultiTypeValidation(Item);

            if (IsBlacklist)
                return !foundMatch;
            else
                return foundMatch;
        }

        /// <summary>
        /// An optimization that can be used when there is a single type
        /// in the list of valid types to determine if the component exists
        /// </summary>
        /// <param name="Item">The item to check component existence on</param>
        /// <returns>True if the type is found on the object, false if not</returns>
        protected virtual bool SingleTypeValidation(GameObject Item)
        {
            Type typeToFind = FilterItems[0];

            Component found = Item.GetComponent(typeToFind);

            return found != null;
        }

        /// <summary>
        /// The main check that is used to determine if there is at least
        /// one component matching the specified types in this filter
        /// </summary>
        /// <param name="Item">The item to check component existence on</param>
        /// <returns>True if the component was found, false if not</returns>
        protected virtual bool MultiTypeValidation(GameObject Item)
        {
            Component[] objectComponents = Item.GetComponents<Component>();
            
            bool matchFound = false;

            foreach (Component component in objectComponents)
            {
                Type componentType = component.GetType();

                matchFound = IsValidItem(componentType);

                if (matchFound)
                    break;
            }

            return matchFound;
        }
    }
}