using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Base class used as a condition for the Scene Management Utility
    /// </summary>
    public abstract class SceneCondition : ScriptableObject, ICondition<SceneManagerController>
    {
        /// <summary>
        /// Determines if this condition has been met
        /// </summary>
        /// <param name="Param">The scene manager calling this conditional</param>
        /// <returns>True if successful, false if not</returns>
        public abstract bool IsMet(SceneManagerController Param);

        /// <summary>
        /// ICondition interface implementation which fails if
        /// the provided object is not a Scene Manager
        /// </summary>
        /// <param name="Param">The scene manager</param>
        /// <returns>True if successful, false if not</returns>
        public bool IsMet(object Param)
        {
            SceneManagerController manager = Param as SceneManagerController;

            if (manager == null)
                return false;

            return IsMet(manager);
        }
    }
}