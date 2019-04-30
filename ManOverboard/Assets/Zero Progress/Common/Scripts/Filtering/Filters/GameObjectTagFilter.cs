using System;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Filter using the game objects' tag to determine validity
    /// </summary>
    [Serializable]
    public class GameObjectTagFilter : StringFilter, IFilter<GameObject>
    {
        /// <summary>
        /// Determines if the passed GameObject passes filtering
        /// </summary>
        /// <param name="Item">The item to evaluate</param>
        /// <returns>True if valid, false if not</returns>
        public bool IsValidItem(GameObject Item)
        {
            return IsValidItem(Item.tag);
        }
    }
}