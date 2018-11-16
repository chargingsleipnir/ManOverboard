using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Interactions
{
    /// <summary>
    /// Toggles a collection of game objects between active states
    /// </summary>
    public class GameObjectToggleable : ToggleableBase
    {
        [SerializeField]
        [Tooltip("The collection of objects to toggle active state between")]
        private List<GameObject> ObjectsToToggle = new List<GameObject>();
        
        /// <summary>
        /// Adds an object to the list of items to toggle with an option of applying the
        /// current toggle state immediately
        /// </summary>
        /// <param name="ObjectToAdd">The object to be added</param>
        /// <param name="SetValueImmediately">True to apply the current value now, false to wait
        /// for the next toggle call</param>
        public void AddObjectToToggleList(GameObject ObjectToAdd, bool SetValueImmediately = true)
        {
            ObjectsToToggle.Add(ObjectToAdd);

            if (SetValueImmediately)
                ObjectToAdd.SetActive(toggleState);
        }
        
        /// <summary>
        /// Removes an object from the toggle list
        /// </summary>
        /// <param name="ObjectToRemove">The object to be removed</param>
        public void RemoveObjectFromToggleList(GameObject ObjectToRemove)
        {
            ObjectsToToggle.Remove(ObjectToRemove);
        }

        protected override void OnActivate()
        {
            SetGameObjectsActiveValue(true);
        }

        protected override void OnDeactivate()
        {
            SetGameObjectsActiveValue(false);
        }

        /// <summary>
        /// Iterates all registered objects and sets their current enabled state
        /// </summary>
        /// <param name="ValueToSet">The value to set the objects active state to</param>
        private void SetGameObjectsActiveValue(bool ValueToSet)
        {
            for (int i = ObjectsToToggle.Count - 1; i >= 0; i--)
            {
                if (ObjectsToToggle[i] == null)
                {
                    ObjectsToToggle.RemoveAt(i);
                    continue;
                }

                ObjectsToToggle[i].SetActive(ValueToSet);
            }
        }

    }
}
