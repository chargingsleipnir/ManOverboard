using System;
using UnityEditor;
using ZeroProgress.Common.Editors;
using ZeroProgress.SceneManagementUtility.Editors;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Asset modification processor that watches certain assets for
    /// renames/moves
    /// </summary>
    public class ScenePathProcessor : UnityEditor.AssetModificationProcessor
    {
        /// <summary>
        /// The collection of extensions that potentially represent scenes
        /// </summary>
        private static readonly string[] WhitelistExtensions = new string[]
        {
            ".unity",
            ".txt",
            ".csv",
            ".xml",
            ".json",
            ".bin"
        };

        /// <summary>
        /// Determines if the moved/renamed asset is possibly a scene type
        /// (i.e. ignore assets and scripts)
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        public static bool IsValidSceneItem(string sourcePath)
        {
            string extension = System.IO.Path.GetExtension(sourcePath);

            if (string.IsNullOrEmpty(extension))
                return false;

            extension = extension.Trim();

            foreach (string whiteListed in WhitelistExtensions)
            {
                if (extension.Equals(whiteListed, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Unity built-in callback for when an asset is moved
        /// </summary>
        /// <param name="sourcePath">Where the file originated</param>
        /// <param name="destinationPath">Where the file is going</param>
        /// <returns>AssetMoveResult.DidNotMove to indicate Unity should perform the
        /// moving for us</returns>
        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            UpdateSceneAssetPaths(sourcePath, destinationPath);

            return AssetMoveResult.DidNotMove;
        }

        private static AssetDeleteResult OnWillDeleteAsset(string sourcePath, RemoveAssetOptions options)
        {
            RemoveSceneAsset(sourcePath);
            return AssetDeleteResult.DidNotDelete;
        }

        /// <summary>
        /// Checks every scene model in every scene manager to determine if the moved
        /// scene was a dependency and updates the path if so
        /// </summary>
        /// <param name="sourcePath">Where the file originated</param>
        /// <param name="destinationPath">Where the file is going</param>
        private static void UpdateSceneAssetPaths(string sourcePath, string destinationPath)
        {
            if (!IsValidSceneItem(sourcePath))
                return;

            foreach (SceneManagerController controller in
                AssetDatabaseExtensions.GetAssetIterator<SceneManagerController>())
            {
                SerializedObject serializedController = new SerializedObject(controller);

                SerializedProperty scenesProp = serializedController.FindProperty("scenes");

                for (int i = 0; i < scenesProp.arraySize; i++)
                {
                    SerializedProperty currentProp = scenesProp.GetArrayElementAtIndex(i);
                    SerializedProperty pathProp = currentProp.FindPropertyRelative("sceneAssetPath");

                    // Check if this scene model is pointing to the active scene
                    if (!pathProp.stringValue.Equals(sourcePath))
                        continue;

                    pathProp.stringValue = destinationPath;

                    SerializedProperty nameProp = currentProp.FindPropertyRelative("sceneName");

                    string sourcePathSceneName = System.IO.Path.GetFileNameWithoutExtension(nameProp.stringValue);
                    string destPathSceneName = System.IO.Path.GetFileNameWithoutExtension(destinationPath);

                    // If the scene name matched the file path, update the scene 
                    // name with the new path name
                    if (nameProp.stringValue.Equals(sourcePathSceneName, StringComparison.OrdinalIgnoreCase))
                        nameProp.stringValue = destPathSceneName;

                    break;
                }
                // Renames can't be undone, so updating scene path shouldn't either
                serializedController.ApplyModifiedPropertiesWithoutUndo();
                SceneManagerEditorWindow.RefreshOpenSceneManagementWindow();
            }
        }

        private static void RemoveSceneAsset(string sourcePath)
        {
            if (!IsValidSceneItem(sourcePath))
                return;

            foreach (SceneManagerController controller in
                AssetDatabaseExtensions.GetAssetIterator<SceneManagerController>())
            {
                controller.RemoveScene(sourcePath);

                SceneNodeCollection nodes =
                    AssetDatabaseExtensions.GetFirstSubAssetOf<SceneNodeCollection>(controller);

                if (nodes != null)
                    nodes.RemoveSceneNodesByPath(sourcePath);
            }

            SceneManagerEditorWindow.RefreshOpenSceneManagementWindow();
        }
    }
}