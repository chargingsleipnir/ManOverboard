using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.SceneManagementUtility.Editors
{
    /// <summary>
    /// Tab for the scene management utility to set
    /// the order of the scenes used for iteration
    /// (such as for a level select menu)
    /// </summary>
    public class SceneOrderTab : Editor
    {
        private static GUIContent includeInIterContent = new GUIContent(string.Empty, 
            "True to include when iterating, false to exclude");

        private SerializedSceneManager serializedManager;

        private ReorderableList sceneOrderList;
        
        public void Initialize(SerializedSceneManager serializedManager)
        {
            this.serializedManager = serializedManager;

            sceneOrderList = new ReorderableList(serializedManager.SerializedManager, 
                serializedManager.ScenesProp, true, true, false, false);

            sceneOrderList.drawHeaderCallback = OnDrawHeader;
            sceneOrderList.elementHeight = EditorGUIUtility.singleLineHeight;
            sceneOrderList.drawElementBackgroundCallback = OnDrawBackground;
            sceneOrderList.drawElementCallback = OnDrawElement;
        }

        public override void OnInspectorGUI()
        {
            sceneOrderList.DoLayoutList();
        }

        #region Reorderable List Callbacks

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Scene Order");
        }

        private void OnDrawBackground(Rect rect, int index, bool isActive, bool isSelected)
        {
            if (isSelected)
            {
                GUIExtensions.ColouredBox(rect,
                    GUI.skin.settings.selectionColor, ZPCommonResources.SimpleSolidBackground);
                return;
            }

            GUIExtensions.ColouredBox(rect, 
                ZPCommonResources.GetElementBgColor(index, EditorGUIUtility.isProSkin), 
                ZPCommonResources.SimpleSolidBackground);
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isSelected)
        {
            SerializedSceneModel serializedScene = serializedManager.GetSerializedSceneModel(index);
            
            Rect toggleRect = new Rect(rect);
            toggleRect.width = 20f;

            Rect labelRect = new Rect(rect);
            labelRect.xMin = 40f;

            EditorGUI.BeginChangeCheck();

            serializedScene.IncludeInIterProp.boolValue = EditorGUI.Toggle(toggleRect, 
                includeInIterContent, serializedScene.IncludeInIterProp.boolValue);

            EditorGUI.LabelField(labelRect, serializedScene.SceneNameProp.stringValue);

            if(EditorGUI.EndChangeCheck())
                serializedScene.SerializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}