using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Editors;

namespace ZeroProgress.SceneManagementUtility.Editors
{
    public class SerializedSceneManager
    {
        private SerializedObject serializedManager;

        public SerializedObject SerializedManager
        {
            get { return serializedManager; }
            set { serializedManager = value; }
        }

        public SceneManagerController TargetManager
        {
            get { return SerializedManager.targetObject as SceneManagerController; }
        }

        public SerializedObject SerializedSceneFilter { get; private set; }

        public SerializedObject ScenePopulator { get; private set; }

        public SerializedSceneVariableCollection SerializedVariables{ get; private set; }
        
        public SerializedObject SceneNodesObject { get; private set; }

        public SceneNodeCollection SceneNodes { get; set; }

        #region Serialized Properties

        /// <summary>
        /// Serialized property for the SceneManager.scenes collection
        /// </summary>
        public SerializedProperty ScenesProp { get; private set; }

        /// <summary>
        /// SceneManager.anyScene
        /// </summary>
        public SerializedProperty AnyScenesProp { get; set; }

        /// <summary>
        /// SceneManager.entrySceneId
        /// </summary>
        public SerializedProperty EntrySceneIdProp { get; private set; }

        /// <summary>
        /// SceneNodeCollection.sceneNodes
        /// </summary>
        public SerializedProperty SceneNodesProp { get; private set; }

        /// <summary>
        /// SceneManager.useOnPlay
        /// </summary>
        public SerializedProperty UseOnPlayProp { get; private set; }

        /// <summary>
        /// SceneManager.goToOnPlay
        /// </summary>
        public SerializedProperty GoToOnPlay { get; private set; }

        #endregion

        public string SceneManagerAssetPath { get; set; }
        
        public SerializedSceneManager(SceneManagerController managerToSerialize)
        {
            SerializedManager = new SerializedObject(managerToSerialize);
            InitializeDefaults();
        }
        
        private void InitializeDefaults()
        {
            SceneManagerAssetPath = AssetDatabase.GetAssetPath(SerializedManager.targetObject);

            bool wasCreated = false;

            AssetFilter assetFilter = AssetDatabaseExtensions.
                FindOrCreateSubScriptableObject<AssetFilter>(SceneManagerAssetPath, "SceneFilter", out wasCreated);

            if (wasCreated)
                assetFilter.IncludeIfNoMatch = true;

            SerializedSceneFilter = new SerializedObject(assetFilter);

            SceneHunter sceneHunter = AssetDatabaseExtensions.
                GetFirstSubAssetOf<SceneHunter>(SceneManagerAssetPath);

            if (sceneHunter == null)
                sceneHunter = AssetDatabaseExtensions.
                    CreateSubScriptableObject<UnitySceneHunter>(SceneManagerAssetPath, "SceneHunter");

            ScenePopulator = new SerializedObject(sceneHunter);
            
            SceneNodes = AssetDatabaseExtensions.
                FindOrCreateSubScriptableObject<SceneNodeCollection>(SceneManagerAssetPath, "SceneNodes", out wasCreated);

            if (wasCreated)
                SceneNodes.SceneManager = TargetManager;

            SceneNodes.PopulateSceneInfos();

            SceneNodesObject = new SerializedObject(SceneNodes);
            SceneNodesProp = SceneNodesObject.FindProperty("sceneNodes");

            if (AssetDatabaseExtensions.GetSubAssetsOfType<SceneVariableContainer>(TargetManager).Count() == 0)
            {
                TargetManager.SceneVariables.name = "ManagerVariables";
                AssetDatabase.AddObjectToAsset(TargetManager.SceneVariables, TargetManager);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(TargetManager));
            }
            SerializedManager.ApplyModifiedProperties();
            
            ScenesProp = SerializedManager.FindProperty("scenes");
            EntrySceneIdProp = SerializedManager.FindProperty("entrySceneId");
            AnyScenesProp = SerializedManager.FindProperty("anyScene");

            UseOnPlayProp = SerializedManager.FindProperty("useOnPlay");
            GoToOnPlay = SerializedManager.FindProperty("goToOnPlay");

            SerializedVariables = new SerializedSceneVariableCollection(TargetManager.SceneVariables);
        }

        public void ReplaceScenePopulator<T>() where T:SceneHunter
        {
            ReplaceScenePopulator(typeof(T));
        }

        public void ReplaceScenePopulator(System.Type scenePopulatorType)
        {
            if (!scenePopulatorType.IsSubclassOf(typeof(SceneHunter)))
                return;

            Undo.SetCurrentGroupName("Scene Populator Change");
            int undoGroup = Undo.GetCurrentGroup();

            Undo.DestroyObjectImmediate(ScenePopulator.targetObject);

            ScenePopulator = new SerializedObject(AssetDatabaseExtensions.
                    FindOrCreateSubScriptableObject<SceneHunter>(TargetManager, 
                    scenePopulatorType, "SceneHunter"));

            Undo.RegisterCreatedObjectUndo(ScenePopulator.targetObject, "Scene Populator Creation");

            Undo.CollapseUndoOperations(undoGroup);
            AssetDatabase.ImportAsset(SceneManagerAssetPath);
            SerializedManager.ApplyModifiedProperties();
            SerializedManager.Update();
        }
        
        internal bool IsEntryScene(int id)
        {
            return id == EntrySceneIdProp.intValue;
        }
        
        internal SerializedSceneModel GetSerializedSceneModel(int elementIndex)
        {
            return new SerializedSceneModel(ScenesProp.GetArrayElementAtIndex(elementIndex));
        }

        internal SerializedSceneModel GetAnySceneSerializedModel()
        {
            return new SerializedSceneModel(AnyScenesProp);
        }

        internal void SetEntryScene(int id)
        {
            if (id < 0)
                return;

            if (TargetManager.GetSceneById(id) == null)
                return;

            EntrySceneIdProp.intValue = id;
            EntrySceneIdProp.serializedObject.ApplyModifiedProperties();

            SceneNodeCollection nodes = SceneNodesObject.targetObject as SceneNodeCollection;
            nodes.SetEntrySceneNode(id);
        }

        internal void SetActiveScene()
        {
            SceneNodeCollection nodes = SceneNodesObject.targetObject as SceneNodeCollection;
            nodes.UpdateActiveSceneNode();
        }

        internal void DeleteSceneNode(int sceneId, bool addToExclusionFilter)
        {
            SceneModel model = TargetManager.scenes.Find((x) => x.SceneId == sceneId);

            if (sceneId == TargetManager.AnyScene.SceneId)
                return;

            if (model == null)
                return;

            Undo.SetCurrentGroupName("Delete Scene Node");
            int undoGroup = Undo.GetCurrentGroup();

            Undo.RegisterCompleteObjectUndo(SceneNodesObject.targetObject, "Scene Nodes");

            SerializedProperty scenesProperty = ScenesProp;

            for (int i = scenesProperty.arraySize - 1; i >= 0; i--)
            {
                SerializedSceneModel serializedScene = GetSerializedSceneModel(i);

                serializedScene.DeleteTransitionsInvolving(sceneId);

                if (serializedScene.SceneIdProp.intValue == sceneId)
                    scenesProperty.DeleteArrayElementAtIndex(i);
            }

            SerializedManager.ApplyModifiedProperties();
            
            for (int i = 0; i < SceneNodesProp.arraySize; i++)
            {
                if (GetSceneNodeIdProperty(i).intValue == sceneId)
                {
                    SceneNodesProp.DeleteArrayElementAtIndex(i);
                    break;
                }
            }

            SceneNodesObject.ApplyModifiedProperties();

            if (addToExclusionFilter)
            {
                SerializedProperty exclusion = SerializedSceneFilter.FindProperty("FilesToExclude");
                SerializedProperty newEntry = exclusion.AddArrayElement();
                newEntry.stringValue = model.SceneAssetPath;
                SerializedSceneFilter.ApplyModifiedProperties();
            }

            Undo.CollapseUndoOperations(undoGroup);
        }

        internal SerializedProperty GetSceneNodeIdProperty(int nodeIndex)
        {
            return SceneNodesProp.GetArrayElementAtIndex(nodeIndex).FindPropertyRelative("SceneId");
        }

        internal void CreateNewVariable(string name, System.Type typeToCreate)
        {
            SerializedVariables.CreateNewVariable(name, typeToCreate);
        }

        /// <summary>
        /// Callback for when the delete button in the scene variable tab is clicked
        /// </summary>
        /// <param name="variableListProp">The property that the deletion took place on</param>
        /// <param name="index">The index of the element to be deleted</param>
        internal void DeleteVariable(SerializedProperty variableListProp, int index)
        {
            serializedManager.Update();

            SerializedVariables.DeleteVariable(index);

            serializedManager.ApplyModifiedProperties();

            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(TargetManager));
        }

        internal SceneCondition CreateNewCondition(string name, System.Type conditionType)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogError("Name not provided");
                return null;
            }

            if (conditionType == null)
                return null;

            if (!conditionType.IsOrIsSubclassOf(typeof(SceneCondition)))
            {
                Debug.LogError("Provided type does not derive from Scene Condition, cannot create");
                return null;
            }

            SceneCondition newCondition =
                AssetDatabaseExtensions.CreateSubScriptableObject(TargetManager,
                    conditionType, name) as SceneCondition;

            Undo.RegisterCreatedObjectUndo(newCondition, "Create Condition");

            return newCondition;
        }

        internal void DeleteCondition(SerializedProperty conditionListProp, int index)
        {
            if (conditionListProp == null)
            {
                Debug.LogError("Condition prop not provided");
                return;
            }
            
            serializedManager.Update();

            SerializedProperty conditionProp = 
                conditionListProp.GetArrayElementAtIndex(index);

            SceneCondition condition = conditionProp.objectReferenceValue as SceneCondition;

            if (condition == null)
            {
                Debug.LogError("Failed to find condition at index " + index);
                return;
            }

            Undo.SetCurrentGroupName("Delete Condition");
            int undoGroup = Undo.GetCurrentGroup();

            Undo.RegisterCompleteObjectUndo(TargetManager, "Condition Deleted");
            Undo.DestroyObjectImmediate(condition);

            // Need to call delete twice to actually remove it from the array
            conditionListProp.DeleteArrayElementAtIndex(index);
            conditionListProp.DeleteArrayElementAtIndex(index);

            conditionListProp.serializedObject.ApplyModifiedProperties();

            Undo.CollapseUndoOperations(undoGroup);

            serializedManager.ApplyModifiedProperties();

            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(TargetManager));
        }
    }
}
