using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroProgress.SceneManagementUtility.Editors
{
    internal class SerializedTransition
    {
        internal SerializedProperty DestSceneIdProp;
        internal SerializedProperty TransitionLabelProp;
        internal SerializedProperty ConditionsProp;
        internal SerializedProperty IsMutedProp;

        internal SerializedTransition(SerializedProperty transitionProperty)
        {
            DestSceneIdProp = transitionProperty.FindPropertyRelative("destinationSceneId");
            TransitionLabelProp = transitionProperty.FindPropertyRelative("transitionLabel");
            IsMutedProp = transitionProperty.FindPropertyRelative("isMuted");
            ConditionsProp = transitionProperty.FindPropertyRelative("conditions");
        }

    }
}