using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    public class AnimatorParamExport
    {
        /// <summary>
        /// The menu item that activates to generate the parameters
        /// </summary>
        [MenuItem("Assets/Export Animation Parameters")]
        private static void ExportAnimationParamsMenuAction()
        {
            AnimatorController selectedController = Selection.activeObject as AnimatorController;

            if (selectedController == null)
                return;

            // Create a folder drop dialog window
            UnityFolderEntryDialog window = EditorWindow.CreateInstance(typeof(UnityFolderEntryDialog)) as UnityFolderEntryDialog;

            window.position = new Rect(window.position.x, window.position.y, 600f, 300f);

            string currentlySelected = Selection.assetGUIDs[0];
            
            string assetPath = AssetDatabase.GUIDToAssetPath(currentlySelected);

            string windowTitle = System.IO.Path.GetFileNameWithoutExtension(assetPath);

            window.titleContent = new GUIContent(windowTitle + " Animation Parameter Export");
            
            // Wait for acknowledgement that a folder has been selected
            window.OnFolderSelected += (sender, selectedFolder) =>
            {
                ExportAnimationParams(selectedController, selectedFolder.Value);
            };

            window.Show();
        }

        /// <summary>
        /// Validates the selected item to ensure that we only export parameters from
        /// AnimationControllers and no other type of asset
        /// </summary>
        /// <returns>True if the selected item is an AnimatorController, false if not</returns>
        [MenuItem("Assets/Export Animation Parameters", true)]
        private static bool ExportAnimationParamsValidation()
        {
            // This returns true when the selected object is a Texture2D (the menu item will be disabled otherwise).
            return Selection.activeObject.GetType() == typeof(AnimatorController);
        }

        /// <summary>
        /// Performs the actual exporting of the parameters
        /// </summary>
        /// <param name="controller">The controller to extract the parameters from</param>
        /// <param name="selectedFolder">The folder to write the parameters to</param>
        private static void ExportAnimationParams(AnimatorController controller, string selectedFolder)
        {
            foreach (AnimatorControllerParameter parameter in controller.parameters)
            {
                ScriptableAnimParam animParam = ScriptableObject.CreateInstance<ScriptableAnimParam>();

                animParam.AnimParamName = parameter.name;
                animParam.DefaultValue = animParam.CurrentValue = parameter.nameHash;

                AssetDatabase.CreateAsset(animParam, selectedFolder + parameter.name + ".asset");
            }
        }
    }
}