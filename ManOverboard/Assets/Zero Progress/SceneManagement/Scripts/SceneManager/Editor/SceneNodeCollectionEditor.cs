using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZeroProgress.NodeEditor;
using ZeroProgress.Common.Editors;
using System;
using ZeroProgress.Common;

namespace ZeroProgress.SceneManagementUtility.Editors
{
    /// <summary>
    /// Editor for the SceneNodeCollection class
    /// </summary>
    [CustomEditor(typeof(SceneNodeCollection))]
    public class SceneNodeCollectionEditor : Editor
    {
        private TransitionListEditor transitionListEditor;
        private ConditionsListEditor lockConditionsEditor;

        private bool showLockConditions = true;

        private DragReceiver scenePathDragReceiver;
        
        private SerializedSceneManager serializedManager;

        private SerializedSceneModel serializedScene;
        
        private void OnEnable()
        {
            Initialize();

            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed += OnUndoRedo;
        }
        
        private void OnUndoRedo()
        {
            if (serializedScene != null)
                serializedScene.SceneModelProp.serializedObject.Update();

            Repaint();
        }

        private void Initialize()
        {
            SceneManagerEditorWindow editor = RefreshManager();

            if (editor != null)
                NodeEditor_OnSelectionChanged(editor.NodeEditor.NodeEditor, null);

            scenePathDragReceiver = new DragReceiver(IsValidDrag, DragAndDropVisualMode.Link);
            scenePathDragReceiver.OnDragComplete += ScenePathDragReceiver_OnDragComplete;
        }

        private void ScenePathDragReceiver_OnDragComplete(object sender, EventArgs e)
        {
            DragReceiver senderReceiver = sender as DragReceiver;

            if (senderReceiver == null)
                return;

            if (serializedScene == null)
                return;
            
            serializedScene.AssetPathProp.stringValue = DragAndDrop.paths[0];
            serializedScene.SerializedObject.ApplyModifiedProperties();
        }

        private bool IsValidDrag(UnityEngine.Object[] selectedObjects, string[] selectedPaths)
        {
            string extension = System.IO.Path.GetExtension(selectedPaths[0]);

            // Ignore folders
            if (string.IsNullOrEmpty(extension))
                return false;

            return ScenePathProcessor.IsValidSceneItem(selectedPaths[0]);
        }

        private SceneManagerEditorWindow RefreshManager()
        {
            SceneManagerEditorWindow managerWindow = 
                SceneManagerEditorWindow.RefreshOpenSceneManagementWindow();

            if (managerWindow == null)
                return null;

            serializedManager = managerWindow.SerializedSceneManager;

            managerWindow.OnRefresh -= ManagerWindow_OnRefresh;
            managerWindow.OnRefresh += ManagerWindow_OnRefresh;

            return managerWindow;
        }

        private void ManagerWindow_OnRefresh(object sender, EventArgs e)
        {
            SceneManagerEditorWindow managerWindow = sender as SceneManagerEditorWindow;

            managerWindow.NodeEditor.NodeEditor.OnSelectionChanged -= NodeEditor_OnSelectionChanged;
            managerWindow.NodeEditor.NodeEditor.OnSelectionChanged += NodeEditor_OnSelectionChanged;
            
            serializedManager = managerWindow.SerializedSceneManager;
        }

        private void NodeEditor_OnSelectionChanged(object sender, System.EventArgs e)
        {
            lockConditionsEditor = null;
            transitionListEditor = null;
            serializedScene = null;

            NodeEditor.NodeEditor nodeEditor = sender as NodeEditor.NodeEditor;

            IEnumerable<Node> selectedNodes = nodeEditor.GetSelectedNodes();

            foreach (Node selected in selectedNodes)
            {
                SceneNode sceneNode = selected as SceneNode;

                if (sceneNode == null)
                    continue;

                SetSelectedNode(sceneNode);

                if (transitionListEditor == null)
                {
                    transitionListEditor = CreateInstance<TransitionListEditor>();
                    transitionListEditor.Initialize(serializedManager, serializedScene);
                }

                return;
            }

            IEnumerable<Connector> selectedConnectors = nodeEditor.GetSelectedConnectors();

            foreach (Connector selected in selectedConnectors)
            {
                SceneNode startNode = selected.GetStartNode() as SceneNode;
                SceneNode endNode = selected.GetEndNode() as SceneNode;

                SetSelectedNode(startNode);

                if (transitionListEditor == null)
                {
                    transitionListEditor = CreateInstance<TransitionListEditor>();
                    transitionListEditor.Initialize(serializedManager, serializedScene);
                }

                transitionListEditor.SetSelectedTransition(endNode.SceneId);
            }
        }

        private void SetSelectedNode(SceneNode selectedNode)
        {
            if (selectedNode == null)
                return;

            if (selectedNode.SceneId == SceneManagerController.ANY_SCENE_ID)
            {
                serializedScene = serializedManager.GetAnySceneSerializedModel();
                Repaint();
                return;
            }

            SerializedProperty scenesProp = serializedManager.ScenesProp;

            // Get the serialized property of the scene node from the scene manager
            for (int i = 0; i < scenesProp.arraySize; i++)
            {
                SerializedSceneModel serializedModel = serializedManager.GetSerializedSceneModel(i);
                
                if (serializedModel.SceneIdProp.intValue == selectedNode.SceneId)
                {
                    serializedScene = serializedModel;
                    Repaint();
                    break;
                }
            }
        }

        protected override void OnHeaderGUI()
        {
            if (serializedScene == null)
                return;
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (serializedScene.SceneIdProp.intValue >= 0)
                EditorGUILayout.LabelField("Scene " + serializedScene.SceneIdProp.intValue.ToString(), EditorStyles.boldLabel);

            EditorGUILayout.LabelField(serializedScene.SceneNameProp.stringValue, EditorStyles.boldLabel);

            EditorGUILayout.EndVertical();
        }

        public override void OnInspectorGUI()
        {
            if (serializedScene == null)
            {
                base.OnInspectorGUI();
                return;
            }
            
            bool isAnyScene = serializedScene.SceneIdProp.intValue == 
                SceneManagerController.ANY_SCENE_ID;

            DrawIdentificationInfo(isAnyScene);
            DrawLockConditons(isAnyScene);

            transitionListEditor.OnInspectorGUI();
        }

        private void DrawIdentificationInfo(bool isAnyScene)
        {
            EditorGUI.BeginDisabledGroup(isAnyScene);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(serializedScene.SceneNameProp);
            
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.TextField("Scene Path: ", serializedScene.AssetPathProp.stringValue);

            Rect scenePathRect = GUILayoutUtility.GetLastRect();

            scenePathDragReceiver.ReceiverBox = scenePathRect.WithPosition(
                GUIUtility.GUIToScreenPoint(scenePathRect.position));

            if (GUILayout.Button("Ping", GUILayout.MaxWidth(40f)))
            {
                UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(
                    serializedScene.AssetPathProp.stringValue);
                EditorGUIUtility.PingObject(asset);
            }

            EditorGUILayout.EndHorizontal();
            
            scenePathDragReceiver.Update();
            EditorGUILayout.PropertyField(serializedScene.UseAnySceneTransProp);

            if (EditorGUI.EndChangeCheck())
            {
                serializedScene.SerializedObject.ApplyModifiedProperties();

                SceneManagerEditorWindow nodeEditor = SceneManagerEditorWindow.TryGetExistingWindow();
                
                if (nodeEditor != null)
                    nodeEditor.Repaint();
            }

            EditorGUI.EndDisabledGroup();
        }

        private void DrawLockConditons(bool isAnyScene)
        {
            if (lockConditionsEditor == null && !isAnyScene)
            {
                lockConditionsEditor = CreateInstance<ConditionsListEditor>();
                lockConditionsEditor.ListHeader = "Lock Conditions";
                lockConditionsEditor.Initialize(serializedManager, serializedScene.LockConditionsProp);
            }

            if (lockConditionsEditor != null)
            {
                Rect foldoutRect = EditorGUILayout.BeginHorizontal();
                lockConditionsEditor.ShowElements = showLockConditions;

                lockConditionsEditor.OnInspectorGUI();

                EditorGUILayout.EndHorizontal();

                foldoutRect.width = 10;
                showLockConditions = EditorGUI.Foldout(foldoutRect, showLockConditions, GUIContent.none);
            }
        }
    }
}