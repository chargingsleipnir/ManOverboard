using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Editors;

namespace ZeroProgress.SceneManagementUtility.Editors
{
    public class TransitionListEditor : Editor
    {
        private const string HeaderText = "Transitions";
        private const string MuteText = "Mute";
        
        public SerializedSceneManager SerializedSceneManager;

        SerializedSceneModel serializedScene;

        public event EventHandler OnRequestRepaint;

        private SerializedProperty selectedTransitionProp;

        private ReorderableList transitionListDisplay;
        private ConditionsListEditor conditionsEditor;

        internal void Initialize(SerializedSceneManager serializedManager,
            SerializedSceneModel serializedScene)
        {
            SerializedSceneManager = serializedManager;
            this.serializedScene = serializedScene;
            selectedTransitionProp = null;

            // Need this to reset transition prop because for some reason Redo isn't
            // resulting in this being called properly when the SceneManagerEditor reinitializes
            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed += OnUndoRedo;

            transitionListDisplay = new ReorderableList(serializedScene.SerializedObject,
                serializedScene.TransitionsProp, true, true, false, true);

            transitionListDisplay.drawHeaderCallback = OnDrawHeader;
            transitionListDisplay.drawElementCallback = DrawElement;
            transitionListDisplay.onSelectCallback = OnSelected;
            transitionListDisplay.onRemoveCallback = OnRemoved;

            transitionListDisplay.index = -1;

            if (serializedScene.TransitionsProp.arraySize > 0)
            {
                transitionListDisplay.index = 0;
                OnSelected(transitionListDisplay);
            }
        }

        private void OnUndoRedo()
        {
            selectedTransitionProp = null;
        }

        public override void OnInspectorGUI()
        {
            transitionListDisplay.DoLayoutList();
            
            if (selectedTransitionProp != null)
                RenderSelectedTransition();
        }

        public void SetSelectedTransition(int destId)
        {
            for (int i = 0; i < serializedScene.TransitionsProp.arraySize; i++)
            {
                SerializedTransition serializedTrans = serializedScene.GetTransitionAtIndex(i);

                if (serializedTrans.DestSceneIdProp.intValue == destId)
                {
                    transitionListDisplay.index = i;
                    selectedTransitionProp = serializedTrans.TransitionProperty;
                    break;
                }
            }
        }

        private void RenderSelectedTransition()
        {
            SerializedTransition transitionProps = new SerializedTransition(selectedTransitionProp);
            selectedTransitionProp.serializedObject.Update();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            
            EditorGUILayout.LabelField("Selected Transition: ");
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(GetTransitionDisplayName(transitionProps));
            EditorGUI.indentLevel--;
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();

            transitionProps.TransitionLabelProp.stringValue =
                EditorGUILayout.TextField(transitionProps.TransitionLabelProp.stringValue, 
                    SceneManagerResources.TransitionNameStyle);

            GUI.SetNextControlName("ClearButton");

            if (GUILayout.Button("Clear", GUILayout.Width(50f)))
            {
                transitionProps.TransitionLabelProp.stringValue = string.Empty;
                // Steal focus from the text field to force it to update
                GUI.FocusControl("ClearButton");
            }

            if (EditorGUI.EndChangeCheck())
                transitionProps.TransitionLabelProp.serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            if (conditionsEditor != null)
                conditionsEditor.OnInspectorGUI();
            
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Get the display name for the transition
        /// </summary>
        /// <param name="transition">The extracted serialized properties</param>
        /// <returns>The text to display as the transition identifier</returns>
        private string GetTransitionDisplayName(SerializedTransition transition, bool forceArrowFormat = false)
        {
            SceneModel destinationScene = SerializedSceneManager.TargetManager.
                GetSceneById(transition.DestSceneIdProp.intValue);

            string displayName = transition.TransitionLabelProp.stringValue;

            if (string.IsNullOrEmpty(displayName) || forceArrowFormat)
                displayName = serializedScene.SceneNameProp.stringValue + 
                    " -> " + destinationScene.SceneName;

            return displayName;
        }

        #region Reorderable List Callbacks

        /// <summary>
        /// Used by the ReorderableList to determine what to display as the header
        /// </summary>
        /// <param name="rect">The rect that the header is restricted to</param>
        private void OnDrawHeader(Rect rect)
        {
            // Remove offset caused by the remove button at the bottom
            rect.xMax -= 10f;
            Rect muteBtnHeader = new Rect(rect);

            muteBtnHeader.xMin = rect.width - 40f;

            Rect labelRect = new Rect(rect);

            labelRect.xMax = muteBtnHeader.xMin;
            
            EditorGUI.LabelField(labelRect, HeaderText);
            
            if(GUI.Button(muteBtnHeader, MuteText))
            {
                serializedScene.ToggleMuteAllTransitions();
                serializedScene.SerializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// Draws a scene variable element of the lsit
        /// </summary>
        /// <param name="rect">The rectangle to render the variable in</param>
        /// <param name="index">The index of the property</param>
        /// <param name="isActive">True if the element is active, false if not</param>
        /// <param name="isSelected">True if the element is selected, false if not</param>
        private void DrawElement(Rect rect, int index, bool isActive, bool isSelected)
        {
            SerializedTransition transitionProps = serializedScene.GetTransitionAtIndex(index);

            Rect muteToggleRect = new Rect(rect);
            muteToggleRect.xMin = rect.width - 10f;

            Rect nameRect = new Rect(rect);
            nameRect.xMax = muteToggleRect.xMin;

            GUI.Label(nameRect, GetTransitionDisplayName(transitionProps));

            EditorGUI.BeginChangeCheck();

            transitionProps.IsMutedProp.boolValue = GUI.Toggle(muteToggleRect, 
                transitionProps.IsMutedProp.boolValue, GUIContent.none);

            if (EditorGUI.EndChangeCheck())
                serializedScene.SerializedObject.ApplyModifiedProperties();
        }
        
        private void OnSelected(ReorderableList reorderableList)
        {
            int selected = reorderableList.index;

            if (selected < 0 || reorderableList.serializedProperty.arraySize == 0)
            {
                if (conditionsEditor != null)
                {
                    DestroyImmediate(conditionsEditor);
                    conditionsEditor = null;
                }

                selectedTransitionProp = null;
                return;
            }

            selectedTransitionProp = serializedScene.GetTransitionAtIndex(selected).TransitionProperty;

            if (conditionsEditor == null)
                conditionsEditor = CreateInstance<ConditionsListEditor>();
            
            conditionsEditor.Initialize(SerializedSceneManager, 
                serializedScene.GetTransitionAtIndex(selected).ConditionsProp);

            conditionsEditor.ListHeader = "Transition Conditions";
        }

        private void OnRemoved(ReorderableList reorderableList)
        {
            if (reorderableList.count <= 0)
                return;

            if (reorderableList.index < 0)
                return;

            serializedScene.DeleteTransitionAtIndex(reorderableList.index);

            reorderableList.index--;
            OnSelected(reorderableList);

            OnRequestRepaint.SafeInvoke(this, EventArgs.Empty);

            GUIUtility.ExitGUI();
        }

        #endregion
    }
}