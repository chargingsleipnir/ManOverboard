using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Editors;

namespace ZeroProgress.SceneManagementUtility.Editors
{
    public class SceneHunterEditor : Editor
    {
        private static GUIContent clearScenesContent = new GUIContent("Clear Build Scenes");

        public event EventHandler<EventArgs<IEnumerable<String>>> OnSceneImport;

        private string selectedSceneHunterType = "";

        private ClassExtendsAttribute sceneHunterTypeFilter;

        private GUIContent typeWindowContent = new GUIContent("Scene Hunter Type");

        private Editor populatorEditor;
        private AssetFilterEditor assetFilterEditor;

        private SerializedSceneManager serializedSceneManager;

        public void Initialize(SerializedSceneManager serializedManager)
        {
            serializedSceneManager = serializedManager;

            sceneHunterTypeFilter = new ClassExtendsAttribute(typeof(SceneHunter))
            {
                AllowAbstract = false,
                AllowStructs = false,
                RemoveNamespaceFromSelected = true,
                Grouping = ClassGrouping.ByNamespaceFlat,
                ShowNoneOption = false
            };

            selectedSceneHunterType = serializedManager.
                ScenePopulator.targetObject.GetType().AssemblyQualifiedName;

            RefreshPopulatorEditor();

            if (assetFilterEditor != null)
                DestroyImmediate(assetFilterEditor);

            assetFilterEditor = CreateEditor(serializedManager.SerializedSceneFilter.
                targetObject, typeof(AssetFilterEditor)) as AssetFilterEditor;
        }
        
        private void RefreshPopulatorEditor()
        {
            if (populatorEditor != null)
                DestroyImmediate(populatorEditor);

            populatorEditor = CreateEditor(serializedSceneManager.ScenePopulator.targetObject);
        }

        public override void OnInspectorGUI()
        {
            RenderImportSection();

            RenderFilterSection();
        }

        private void RenderImportSection()
        {
            Rect sceneImportSection = EditorGUILayout.BeginVertical();

            EditorGUILayout.Space();

            if (Event.current.type == EventType.Repaint)
                GUIExtensions.ColouredBox(sceneImportSection, Color.gray, GUIContent.none);

            EditorGUI.indentLevel++;

            RenderTypeSelection();

            EditorGUI.BeginChangeCheck();

            populatorEditor.DrawExcludingScript(serializedSceneManager.ScenePopulator);

            if (EditorGUI.EndChangeCheck())
                serializedSceneManager.ScenePopulator.ApplyModifiedProperties();

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            if (GUILayout.Button("Import Scenes"))
                ImportScenes();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(clearScenesContent))
                ClearExistingBuildScenes();

            if (GUILayout.Button("Send To Build", 
                GUILayout.MinWidth(GUI.skin.button.CalcSize(clearScenesContent).x)))
                SendToBuildSettings();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();
        }

        private void RenderFilterSection()
        {
            EditorGUILayout.LabelField("Drag and drop files and folders into the below sections " +
                "to apply additional filtering to the values that the selected Scene Populator finds.",
                SceneManagerResources.SceneHunterInstrucStyle);

            assetFilterEditor.OnInspectorGUI();
        }

        private void RenderTypeSelection()
        {
            if (sceneHunterTypeFilter == null)
                return;

            string previousSelection = selectedSceneHunterType;

            Rect controlRect = EditorGUILayout.GetControlRect();
            selectedSceneHunterType = TypeReferencePropertyDrawer.DrawTypeSelectionControl(
                controlRect, typeWindowContent, selectedSceneHunterType, sceneHunterTypeFilter);

            if (previousSelection != selectedSceneHunterType)
                ChangeScenePopulator();
        }

        private void ChangeScenePopulator()
        {
            Type selectedType = Type.GetType(selectedSceneHunterType);
            
            serializedSceneManager.ReplaceScenePopulator(selectedType);
            
            RefreshPopulatorEditor();

            // Abort current rendering, otherwise exceptions about mismatched layout
            // will be thrown due to changing the current editor
            GUIUtility.ExitGUI();
        }

        private void ImportScenes()
        {
            SceneHunter scenePopulator = serializedSceneManager.ScenePopulator.
               targetObject as SceneHunter;

            AssetFilter filter = serializedSceneManager.SerializedSceneFilter.
                targetObject as AssetFilter;

            IEnumerable<string> retrievedScenes = scenePopulator.GetScenePaths();

            IEnumerable<string> includedFiles = filter.FilesToInclude;

            List<string> allFiles = new List<string>();
            allFiles.AddUniqueRange(retrievedScenes);
            allFiles.AddUniqueRange(includedFiles);

            allFiles.RemoveAll((x) => !filter.ShouldBeIncluded(x));
            
            OnSceneImport.SafeInvoke(this, 
                new EventArgs<IEnumerable<string>>() { Value = allFiles });
        }

        private void SendToBuildSettings()
        {
            List<EditorBuildSettingsScene> newScenes = 
                new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

            foreach (SceneModel sceneModel in serializedSceneManager.TargetManager.scenes)
            {
                EditorBuildSettingsScene existing = newScenes.FirstOrDefault((x) =>
                        x.path.Equals(sceneModel.SceneAssetPath));

                if (existing != null)
                    continue;

                EditorBuildSettingsScene newBuildScene =
                    new EditorBuildSettingsScene(sceneModel.SceneAssetPath, true);

                if (sceneModel == serializedSceneManager.TargetManager.EntryScene)
                    newScenes.Insert(0, newBuildScene);
                else
                    newScenes.Add(newBuildScene);
            }

            MakeEntrySceneFirstBuilt(newScenes);

            EditorBuildSettings.scenes = newScenes.ToArray();
        }

        private void MakeEntrySceneFirstBuilt(List<EditorBuildSettingsScene> scenes)
        {
            SceneModel entryScene = serializedSceneManager.TargetManager.EntryScene;

            if (entryScene == null || scenes.Count == 0)
                return;

            if (scenes[0].path.Equals(entryScene.SceneAssetPath))
                return;

            int indexOfEntry = scenes.FindIndex((x) => 
                x.path.Equals(entryScene.SceneAssetPath));

            if (indexOfEntry < 0)
            {
                Debug.LogError("Failed to find Entry Scene, cannot set as first build scene");
                return;
            }

            scenes.SwapValues(indexOfEntry, 0);
        }

        private void ClearExistingBuildScenes()
        {
            if (EditorBuildSettings.scenes.Length == 0)
                return;

            if (!EditorUtility.DisplayDialog("Are you sure?", "Clear all scenes currently in build?", "Yes", "No"))
                return;

            EditorBuildSettings.scenes = new EditorBuildSettingsScene[0];
        }
    }
}