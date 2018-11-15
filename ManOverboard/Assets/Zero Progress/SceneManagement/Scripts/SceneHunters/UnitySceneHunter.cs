using System.Collections.Generic;
using ZeroProgress.Common.Editors;
using System.Linq;
using ZeroProgress.Common;
using UnityEngine;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Scene hunter for unity scenes
    /// </summary>
    public class UnitySceneHunter : SceneHunter
    {
        [Tooltip("How to sort the imported scenes")]
        public SortMode Sorting = SortMode.ASCENDING;

        /// <summary>
        /// Retrieves all assets with the .unity extension
        /// </summary>
        /// <returns>Retrieves all assets with the .unity extension</returns>
        public override IEnumerable<string> GetScenePaths()
        {
            IEnumerable<string> assetPaths = AssetDatabaseExtensions.FindAssetPathsWithExtension(".unity");

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