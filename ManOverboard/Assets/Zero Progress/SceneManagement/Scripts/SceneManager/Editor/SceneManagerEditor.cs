using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Editors;
using ZeroProgress.Common.Reflection;
using ZeroProgress.NodeEditor;

namespace ZeroProgress.SceneManagementUtility.Editors
{
    /// <summary>
    /// Editor Window for the Scene Manager object
    /// </summary>
    public partial class SceneManagerEditorWindow : EditorWindow
    {
        private enum ManagementWindowOption
        {
            Global_Variables,            
            Scene_Order,
            Scene_Import
        }

        /// <summary>
        /// The displayable content for each tab item
        /// </summary>
        private static GUIContent[] sceneManagerTabs = null;

        [SerializeField]
        private SceneManagerController sceneManager;

        private SerializedSceneManager serializedSceneManager;

        public SerializedSceneManager SerializedSceneManager
        {
            get
            {
                if (serializedSceneManager == null)
                    SerializedSceneManager = new SerializedSceneManager(sceneManager);

                return serializedSceneManager;
            }
            set { serializedSceneManager = value; }
        }

        private SceneManagerNodeEditor nodeEditor = new SceneManagerNodeEditor();

        public SceneManagerNodeEditor NodeEditor
        {
            get { return nodeEditor; }
            set { nodeEditor = value; }
        }

        public event System.EventHandler OnRefresh;

        private int selectedSceneManagerTab = 0;
        
        private SplitViewLayout splitView =
            new SplitViewLayout(SplitViewLayout.SplitDirection.Horizontal,
                initialNormalizedSplitPosition: 0.3f, minSizeConstraint: 230f);

        private bool isInitialized = false;

        private SceneHunterEditor sceneHunterEditor;

        private SceneVariableCollectionEditor variableCollectionEditor;

        private SceneOrderTab sceneOrderingEditor;
        
        private bool isTabVisible = true;

        private bool needsRefresh = false;

        private DragReceiver sceneFileDragReceiver;        

        /// <summary>
        /// Opens the scene manager editor window whenever a SceneManager
        /// asset is opened
        /// </summary>
        /// <param name="instanceId">The instance id of the object that has been opened</param>
        /// <param name="line">No idea</param>
        /// <returns>True if the asset opening was handled, false if not</returns>
        [OnOpenAsset(1)]
        public static bool OpenSceneManagerWindow(int instanceId, int line)
        {
            SceneManagerController manager = EditorUtility.InstanceIDToObject(instanceId) as SceneManagerController;

            if (manager == null)
                return false;

            SceneManagerEditorWindow window = GetWindow<SceneManagerEditorWindow>(title: "Scene Manager", focus: true);

            if (window.sceneManager == manager)
                return false;

            window.sceneManager = manager;
            window.FlagRefresh();

            return true;
        }
        
        public static SceneManagerEditorWindow RefreshOpenSceneManagementWindow()
        {
            SceneManagerEditorWindow managerEditor = TryGetExistingWindow();

            if (managerEditor == null)
                return null;

            managerEditor.FlagRefresh();
            return managerEditor;
        }

        public static SceneManagerEditorWindow TryGetExistingWindow()
        {
            SceneManagerEditorWindow[] managerEditors =
                Resources.FindObjectsOfTypeAll<SceneManagerEditorWindow>();

            if (managerEditors.Length == 0)
                return null;

            return managerEditors[0];
        }

        /// <summary>
        /// Editor callback for whenever the currently selected asset is changed.
        /// 
        /// Used to allow swapping the active scene manager while this window is open
        /// </summary>
        public void OnSelectionChange()
        {
            OpenSceneManagerWindow(Selection.activeInstanceID, -1);
        }

        private void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
        {
            ReflectionUtilities.SetFieldByName(serializedSceneManager.TargetManager, 
                "activeScene", BindingFlags.Instance | BindingFlags.NonPublic, null);

            FlagRefresh();
        }

        private void OnDestroy()
        {
            NodeEditor.NodeEditor.UnselectAll();
        }

        private void OnEnable()
        {
            FlagRefresh();
        }

        public void FlagRefresh()
        {
            needsRefresh = true;
            Repaint();
        }

        private void Refresh()
        {
            EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;

            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed += OnUndoRedo;

            if (sceneManager == null)
                return;

            if (sceneManager.Transitioner != null)
            {
                sceneManager.Transitioner.OnTransitionCompleted.RemoveListener(OnTransitioned);
                sceneManager.Transitioner.OnTransitionCompleted.AddListener(OnTransitioned);

                sceneManager.Transitioner.OnTransitionStarted.RemoveListener(OnTransitionStarted);
                sceneManager.Transitioner.OnTransitionStarted.AddListener(OnTransitionStarted);
            }

            SerializedSceneManager = new SerializedSceneManager(sceneManager);
            serializedSceneManager.SetActiveScene();

            isInitialized = false;

            if (!EditorApplication.isPlaying)
                sceneManager.SceneVariables.ResetVariables();

            needsRefresh = false;

            OnRefresh.SafeInvoke(this, System.EventArgs.Empty);

            Repaint();
        }

        private void OnTransitioned(SceneTransitionEventArgs e)
        {
            serializedSceneManager.SetActiveScene();

            e.DestinationScene = null;
            UpdateActiveTransition(e);
        }

        private void OnTransitionStarted(SceneTransitionEventArgs e)
        {
            UpdateActiveTransition(e);
        }

        public void RefreshStatuses()
        {
            if (sceneManager.scenes.Count == 0)
                selectedSceneManagerTab = (int)ManagementWindowOption.Scene_Import;

            sceneManager.ReevaluateLockStatuses();
            Repaint();
        }

        private void OnUndoRedo()
        {
            isInitialized = false;
            
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(sceneManager));

            SerializedSceneManager = new SerializedSceneManager(sceneManager);

            if (Selection.activeObject != serializedSceneManager.SceneNodesObject.targetObject)
                Selection.activeObject = SerializedSceneManager.SceneNodesObject.targetObject;

            FlagRefresh();

            Repaint();
        }

        private void Initialize()
        {
            GUI.skin.AddCustomStyle(SceneManagerResources.SceneNodeOpenStyle, overrideIfExist: false);
            GUI.skin.AddCustomStyle(SceneManagerResources.SceneNodeCloseStyle, overrideIfExist: false);

            if (isInitialized)
                return;

            sceneManagerTabs = new GUIContent[]
            {
                new GUIContent(SceneManagerResources.VariablesListIcon, "Variables Listing"),
                new GUIContent(SceneManagerResources.SceneOrderIcon, "Organize Scene Iteration"),
                new GUIContent(SceneManagerResources.SceneImportIcon, "Import Scenes")
            };

            sceneFileDragReceiver = new DragReceiver(SceneFileDragValidator, DragAndDropVisualMode.Link);
            sceneFileDragReceiver.OnDragComplete += SceneFileDragReceiver_OnDragComplete;
            RefreshStatuses();
            
            InitializeSceneImport();
            InitializeVariablesTab();
            InitializeSceneOrderTab();

            NodeEditor.Initialize(serializedSceneManager, this);

            isInitialized = true;
        }

        private void SceneFileDragReceiver_OnDragComplete(object sender, System.EventArgs e)
        {
            bool wasSceneAdded = false;

            foreach (string path in DragAndDrop.paths)
            {
                if (!ScenePathProcessor.IsValidSceneItem(path))
                    continue;

                if (sceneManager.GetSceneByPath(path) != null)
                    continue;
                
                AddNewScene(path, serializedSceneManager.SceneNodes, NodeEditor.GetNewNodePosition());
                    
                wasSceneAdded = true;
            }

            if (wasSceneAdded)
            {
                RefreshStatuses();
                serializedSceneManager.SerializedManager.Update();
                serializedSceneManager.SceneNodesObject.Update();
                Repaint();
            }
        }

        private static bool SceneFileDragValidator(Object[] items, string[] paths)
        {
            foreach (string path in paths)
            {
                if (ScenePathProcessor.IsValidSceneItem(path))
                    return true;
            }

            return false;
        }

        private void InitializeSceneImport()
        {
            if (sceneHunterEditor != null)
                DestroyImmediate(sceneHunterEditor);

            sceneHunterEditor = CreateInstance<SceneHunterEditor>();
            sceneHunterEditor.Initialize(SerializedSceneManager);
            sceneHunterEditor.OnSceneImport += SceneHunterEditor_OnSceneImport;
        }

        private void InitializeSceneOrderTab()
        {
            sceneOrderingEditor = CreateInstance<SceneOrderTab>();

            sceneOrderingEditor.Initialize(serializedSceneManager);
        }

        private void InitializeVariablesTab()
        {
            variableCollectionEditor = CreateInstance<SceneVariableCollectionEditor>();

            variableCollectionEditor.HeaderText = "Global Variables";
            variableCollectionEditor.SetVariableCollectionProperty(serializedSceneManager.SerializedVariables);

            variableCollectionEditor.AddAction = SerializedSceneManager.CreateNewVariable;
            variableCollectionEditor.RemoveAction = SerializedSceneManager.DeleteVariable;
            variableCollectionEditor.OnSomethingChanged += VariableCollectionEditor_OnSomethingChanged;
            variableCollectionEditor.OnVariableNameChanged += VariableCollectionEditor_OnVariableNameChanged;
        }

        private void VariableCollectionEditor_OnSomethingChanged(object sender, System.EventArgs e)
        {
            serializedSceneManager.TargetManager.Evaluate();
            Repaint();
        }
        
        private void LoadScene(GUIMenuItemParamEventArgs args)
        {
            SceneNode node = args.MenuContext as SceneNode;

            if (node == null)
                return;

            if (node.SceneId == SceneManagerController.ANY_SCENE_ID)
                return;

            sceneManager.TransitionToScene(node.SceneInfo);
        }

        /// <summary>
        /// Filter to determine deletable nodes
        /// </summary>
        /// <param name="node">The node to evaluate</param>
        /// <returns>True if deletable, false if not</returns>
        private bool CanDeleteNode(Node node)
        {
            SceneNode sceneNode = node as SceneNode;

            if (sceneNode == null)
                return true;

            if (sceneNode.SceneInfo == SerializedSceneManager.TargetManager.AnyScene)
                return false;

            return true;
        }
        
        private void SceneHunterEditor_OnSceneImport(object sender, EventArgs<IEnumerable<string>> e)
        {
            Undo.SetCurrentGroupName("Scene Import");
            int importGroup = Undo.GetCurrentGroup();

            Undo.RecordObject(sceneManager, "Before Scene Import");
            
            NodeEditor.NodeEditor.ClearAll();

            SceneNodeCollection nodeCollection = SerializedSceneManager.
                SceneNodesObject.targetObject as SceneNodeCollection;
            
            // Remove scene models that aren't part of the listing
            sceneManager.scenes.RemoveAll((x) => !e.Value.Contains(x.SceneAssetPath));

            nodeCollection.SceneNodes.RemoveAll((x) =>
                x.SceneInfo == null ||
                (!e.Value.Contains(x.SceneInfo.SceneAssetPath) && x.SceneId >= 0));

            Vector2 position = new Vector2(20f, 100f);

            AddAnySceneNode(nodeCollection, position);

            position = new Vector2(300f, 0f);
            
            foreach (string scenePath in e.Value)
            {
                AddNewScene(scenePath, nodeCollection, position);
                position.y += SceneManagerResources.SceneNodeOpenStyle.fixedHeight;
            }

            SerializedSceneManager.SceneNodesObject.Update();
            SerializedSceneManager.SerializedManager.Update();

            Undo.CollapseUndoOperations(importGroup);

            isInitialized = false;
        }

        private void AddAnySceneNode(SceneNodeCollection nodeCollection, Vector2 pos)
        {
            if (nodeCollection.SceneNodes.Find((x) => x.SceneId < 0) != null)
                return;

            SceneNode node = nodeCollection.AddSceneNode(
                SceneManagerController.ANY_SCENE_ID, pos);

            NodeEditor.NodeEditor.AddNode(node);
        }

        private SceneNode AddNewScene(string scenePath, 
            SceneNodeCollection collection, Vector2 pos)
        {
            SceneModel existing = sceneManager.GetSceneByPath(scenePath);
                        
            if (existing == null)
            {
                int id = sceneManager.AddScene(scenePath);
                existing = sceneManager.GetSceneById(id);
            }
            
            SceneNode existingNode = collection.GetNodeBySceneId(existing.SceneId);

            if (existingNode == null)
                existingNode = collection.AddSceneNode(existing.SceneId, pos);

            NodeEditor.NodeEditor.AddNode(existingNode);

            return existingNode;
        }

        private void VariableCollectionEditor_OnVariableNameChanged(object sender,
            ValueChangedEventArgs<string> e)
        {
            IEnumerable<ScriptableObject> subAssets = AssetDatabaseExtensions.
                GetSubAssetsOfType<ScriptableObject>(sceneManager);

            ScriptableObject foundSubAsset = subAssets.Where(
                (x) => x.name == e.OldValue).FirstOrDefault();

            if (foundSubAsset == null)
            {
                Debug.Log("Failed to rename variable");
                return;
            }

            foundSubAsset.name = e.NewValue;
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(sceneManager));
        }

        private void OnGUI()
        {
            if (needsRefresh)
                Refresh();

            SerializedSceneManager.SerializedManager.Update();
            SerializedSceneManager.SceneNodesObject.Update();

            Initialize();

            sceneFileDragReceiver.ReceiverBox = position;

            sceneFileDragReceiver.Update();

            RenderToolbar();

            if (isTabVisible)
            {
                splitView.BeginSplitView();

                RenderManagementWindow();

                splitView.Split();
            }

            NodeEditor.OnGUI();

            if (isTabVisible)
            {
                if (splitView.EndSplitView())
                    Repaint();
            }

            serializedSceneManager.SceneNodes.PanOffset = NodeEditor.NodeEditor.GetPanOffset();
        }
        
        /// <summary>
        /// Renders the toolbar that handles displaying the left-side inspector
        /// </summary>
        private void RenderToolbar()
        {
            Rect horizontalRect = EditorGUILayout.BeginHorizontal();

            GUI.Box(horizontalRect, "", EditorStyles.toolbar);

            Rect eyeIconPos = new Rect(horizontalRect);
            eyeIconPos.width = 40f;
            eyeIconPos.height = EditorGUIUtility.singleLineHeight;
            
            if (isTabVisible)
            {
                selectedSceneManagerTab = GUILayout.Toolbar(selectedSceneManagerTab,
                    sceneManagerTabs, EditorStyles.toolbarButton, GUILayout.Width(120f));
                eyeIconPos.x = splitView.GetConstrainedValue() - eyeIconPos.width;
            }
            else
            {
                eyeIconPos = GUILayoutUtility.GetRect(eyeIconPos.width, eyeIconPos.height, GUILayout.Width(eyeIconPos.width));
            }

            GUIContent eyeIcon = isTabVisible ?
                SceneManagerEditorResources.EyeOnContent :
                SceneManagerEditorResources.EyeOffContent;

            if (GUI.Button(eyeIconPos, eyeIcon,
                    SceneManagerEditorResources.GetTabsVisibilityIconStyle(!isTabVisible)))
                isTabVisible = !isTabVisible;
            
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Renders the window where the Global Variables and Scene Import
        /// tools are defined
        /// </summary>
        private void RenderManagementWindow()
        {
            switch ((ManagementWindowOption)selectedSceneManagerTab)
            {
                case ManagementWindowOption.Global_Variables:

                    RenderVariablesTab();

                    break;

                case ManagementWindowOption.Scene_Import:

                    RenderSceneImport();

                    break;

                case ManagementWindowOption.Scene_Order:

                    RenderSceneOrderTab();

                    break;

                default:
                    break;
            }
        }

        private void UpdateActiveTransition(SceneTransitionEventArgs e)
        {
            NodeEditor.UpdateActiveTransition(e.InitialScene, e.DestinationScene);
            Repaint();
        }

        private void RenderVariablesTab()
        {
            EditorGUILayout.BeginVertical(SceneManagerResources.SectionOutlineStyle, GUILayout.ExpandHeight(true));

            if (variableCollectionEditor != null)
                variableCollectionEditor.OnInspectorGUI();

            EditorGUILayout.EndVertical();
        }

        private void RenderSceneImport()
        {
            EditorGUILayout.BeginVertical(SceneManagerResources.SectionOutlineStyle, GUILayout.ExpandHeight(true));

            sceneHunterEditor.OnInspectorGUI();

            EditorGUILayout.EndVertical();
        }

        private void RenderSceneOrderTab()
        {
            EditorGUILayout.BeginVertical(SceneManagerResources.SectionOutlineStyle, GUILayout.ExpandHeight(true));

            sceneOrderingEditor.OnInspectorGUI();

            EditorGUILayout.EndVertical();
        }
    }
}
