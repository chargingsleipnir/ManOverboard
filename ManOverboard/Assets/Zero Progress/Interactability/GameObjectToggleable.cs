using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Interactions
{
    public class GameObjectToggleable : ToggleableBase
    {
        [SerializeField]
        private List<GameObject> ObjectsToToggle = new List<GameObject>();
        
        public void AddObjectToToggleList(GameObject ObjectToAdd, bool SetValueImmediately = true)
        {
            ObjectsToToggle.Add(ObjectToAdd);

            if (SetValueImmediately)
                ObjectToAdd.SetActive(currentValue);
        }

        public void RemoveObjectFromToggleList(GameObject ObjectToRemove)
        {
            ObjectsToToggle.Remove(ObjectToRemove);
        }

        protected override void ApplyToggleChange(bool NewValue)
        {
            SetGameObjectsActiveValue(NewValue);
        }

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
