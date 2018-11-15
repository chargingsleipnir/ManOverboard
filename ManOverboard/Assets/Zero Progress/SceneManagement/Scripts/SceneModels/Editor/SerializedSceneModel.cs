using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroProgress.SceneManagementUtility.Editors
{
    /// <summary>
    /// Helper to get the serialized properties of the scene model
    /// </summary>
    internal class SerializedSceneModel
    {
        public SerializedObject SerializedObject { get; private set; }

        public SerializedProperty SceneModelProp { get; private set; }
        
        /// <summary>
        /// Property representing id of the scene model
        /// </summary>
        public SerializedProperty SceneIdProp { get; private set; }

        /// <summary>
        /// Property representing the scene name
        /// </summary>
        public SerializedProperty SceneNameProp { get; private set; }

        /// <summary>
        /// Property representing the bool that determines if transitions from
        /// the ANY scene can be used
        /// </summary>
        public SerializedProperty UseAnySceneTransProp { get; private set; }

        /// <summary>
        /// Property of whether or not to include the model in iterations
        /// </summary>
        public SerializedProperty IncludeInIterProp { get; set; }

        /// <summary>
        /// Property for the lock conditions
        /// </summary>
        public SerializedProperty LockConditionsProp { get; private set; }

        /// <summary>
        /// Property of the collection of transitions of the scene
        /// </summary>
        public SerializedProperty TransitionsProp { get; private set; }

        /// <summary>
        /// Property representing the path to the scene data
        /// </summary>
        public SerializedProperty AssetPathProp { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sceneModelProp">The property of the scene model</param>
        public SerializedSceneModel(SerializedProperty sceneModelProp)
        {
            SceneModelProp = sceneModelProp;
            SerializedObject = sceneModelProp.serializedObject;
            SceneIdProp = sceneModelProp.FindPropertyRelative("sceneId");
            SceneNameProp = sceneModelProp.FindPropertyRelative("sceneName");
            UseAnySceneTransProp = sceneModelProp.FindPropertyRelative("useAnySceneTransitions");
            AssetPathProp = sceneModelProp.FindPropertyRelative("sceneAssetPath");
            IncludeInIterProp = sceneModelProp.FindPropertyRelative("includeInIteration");
            LockConditionsProp = sceneModelProp.FindPropertyRelative("lockConditions");
            TransitionsProp = sceneModelProp.FindPropertyRelative("transitions");
        }

        /// <summary>
        /// Gets the serialized transition at the provided index
        /// </summary>
        /// <param name="transitionIndex">The index of the transition</param>
        /// <returns>Serialized representation of the transition</returns>
        public SerializedTransition GetTransitionAtIndex(int transitionIndex)
        {
            return new SerializedTransition(TransitionsProp.GetArrayElementAtIndex(transitionIndex));
        }

        public void SetAllTransitionsMute(bool value)
        {
            for (int i = 0; i < TransitionsProp.arraySize; i++)
            {
                GetTransitionAtIndex(i).IsMutedProp.boolValue = value;
            }
        }

        public void ToggleMuteAllTransitions()
        {
            bool allMuted = true;

            for (int i = 0; i < TransitionsProp.arraySize; i++)
            {
                if(!GetTransitionAtIndex(i).IsMutedProp.boolValue)
                {
                    allMuted = false;
                    break;
                }
            }

            if (allMuted)
                SetAllTransitionsMute(false);
            else
                SetAllTransitionsMute(true);
        }

        public void DeleteTransitionsInvolving(int sceneId)
        {
            for (int t = TransitionsProp.arraySize - 1; t >= 0; t--)
            {
                SerializedTransition currentTrans = GetTransitionAtIndex(t);

                if (currentTrans.DestSceneIdProp.intValue == sceneId)
                    TransitionsProp.DeleteArrayElementAtIndex(t);
            }
        }
    }
}