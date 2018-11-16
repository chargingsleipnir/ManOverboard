using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Handles deleting subassets through the Project Window
    /// </summary>
    public static class SubAssetDeleter
    {
        /// <summary>
        /// Menu item action for deleting a sub asset
        /// </summary>
        [MenuItem("Assets/Delete Sub Asset")]
        private static void DeleteSubAssetMenuAction()
        {
            AssetDatabaseExtensions.DeleteSubAsset(Selection.activeObject);
        }

        /// <summary>
        /// Validates the selected item to ensure that we can only apply the delete action
        /// on sub assets
        /// </summary>
        /// <returns>True if the selected item represents a subasset, false if not</returns>
        [MenuItem("Assets/Delete Sub Asset", true)]
        private static bool ExportAnimationParamsValidation()
        {
            if (Selection.activeObject == null)
                return false;
            
            return AssetDatabase.IsSubAsset(Selection.activeInstanceID);
        }
    }
}