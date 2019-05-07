using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Editors;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Scene hunter for Unity Text Assets
    /// </summary>
    public class TextAssetSceneHunter : SceneHunter
    {
        /// <summary>
        /// Supported text asset extensions
        /// 
        /// As defined https://docs.unity3d.com/Manual/class-TextAsset.html
        /// </summary>
        [Flags]
        public enum SupportedTextExtension
        {
            NONE = 0,
            TXT = 1,
            HTML = 2,
            HTM = 4,
            XML = 8,
            BYTES = 16,
            JSON = 32,
            CSV = 64,
            YAML = 128,
            ALL = ~0
        }
        
        /// <summary>
        /// Collection of extensions to retrieve
        /// </summary>
        [EnumMask]
        public SupportedTextExtension ExtensionsToFind = SupportedTextExtension.JSON;

        [Tooltip("How to sort the imported scenes")]
        public SortMode Sorting = SortMode.ASCENDING;

        /// <summary>
        /// Retrieves the scene paths for all of the selected extensions
        /// </summary>
        /// <returns>Retrieves the scene paths for all of the selected extensions</returns>
        public override IEnumerable<string> GetScenePaths()
        {
            IEnumerable<System.Enum> selectedExtensions = ExtensionsToFind.GetEnabledFlagsInt();

            IEnumerable<string> assetPaths = AssetDatabaseExtensions.FindAssetPathsWithAnyExtension(
                selectedExtensions.Select((ext) => ext.ToString().ToLowerInvariant()));

            switch (Sorting)
            {
                case SortMode.NONE:
                default:
                    return assetPaths;
                case SortMode.ASCENDING:
                    return assetPaths.OrderBy((x) => x);
                case SortMode.DESCENDING:
                    return assetPaths.OrderByDescending((x) => x);
            }

        }
    }
}