using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Represents the model that defines whether a scene can
    /// move on to another
    /// </summary>
    [System.Serializable]
    public class SceneTransition
    {
        [SerializeField]
        internal int destinationSceneId;

        public int DestinationSceneId
        {
            get { return destinationSceneId; }
            private set { destinationSceneId = value; }
        }

        [SerializeField]
        internal string transitionLabel;

        public string TransitionLabel
        {
            get { return transitionLabel; }
        }

        [SerializeField]
        private List<SceneCondition> conditions;

        public List<SceneCondition> Conditions
        {
            get { return conditions; }
            private set { conditions = value; }
        }

        [SerializeField]
        private bool isMuted;

        public bool IsMuted
        {
            get { return isMuted; }
            set { isMuted = value; }
        }

        /// <summary>
        /// Determines if the transition to the destination scene is valid
        /// </summary>
        /// <param name="manager">The manager containing all of the variables to be evaluated</param>
        /// <returns>True if a transition can be performed, false if not
        /// Also returns false if DestinationScene is null</returns>
        public bool CanTransition(SceneManagerController manager)
        {
            SceneModel destinationScene = manager.GetSceneById(destinationSceneId);

            if (destinationScene == null)
                return false;

            bool evaluation = true;

            foreach (SceneCondition condition in Conditions)
            {
                evaluation = condition.IsMet(manager);

                if (!evaluation)
                    break;
            }

            return evaluation;
        }

    }
}