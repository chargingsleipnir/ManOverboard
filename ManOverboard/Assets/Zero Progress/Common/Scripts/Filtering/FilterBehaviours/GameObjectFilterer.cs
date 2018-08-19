using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// A monobehaviour wrapper of the GameObject IFilter implementations
    /// </summary>
    public class GameObjectFilterer : MonoBehaviour, IFilter<GameObject> {

        [Tooltip("Filter a game object by components")]
        public GameObjectComponentFilterRef ComponentFilter;

        [Tooltip("Filter a game object by tag")]
        public GameObjectTagFilterRef TagFilter;

        [Tooltip("True to combine the filters with AND logic, false to combine with OR logic")]
        public bool AndCombineFilters = true;

        /// <summary>
        /// Evaluates the provided item to determine if it passes the filtering conditions
        /// </summary>
        /// <param name="Item">The item to evaluate</param>
        /// <returns>True if valid, false if not</returns>
        public bool IsValidItem(GameObject Item)
        {
            bool hasComponentFilter = ComponentFilter != null && ComponentFilter.Value != null;

            bool validComponent = !hasComponentFilter;

            if (hasComponentFilter)
                validComponent = ComponentFilter.Value.IsValidItem(Item);

            bool hasTagFilter = TagFilter != null && TagFilter.Value != null;

            bool validTag = !hasTagFilter;

            if (hasTagFilter)
                validTag = TagFilter.Value.IsValidItem(Item);

            if (AndCombineFilters)
                return validComponent && validTag;
            else
                return validComponent || validTag;
        }
    }
}
