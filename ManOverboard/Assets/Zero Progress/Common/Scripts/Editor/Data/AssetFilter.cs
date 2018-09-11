using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    [CreateAssetMenu(fileName = "New Asset Filter", 
        menuName = ScriptableObjectPaths.ZERO_PROGRESS_EDITOR_ONLY_ASSETS_PATH + "Asset Filter")]
    public class AssetFilter : ScriptableObject
    {
        [Tooltip("The folders that should be excluded. Tied for third priority with " +
            "inclusion folders")]
        public List<string> FoldersToExclude = new List<string>();

        [Tooltip("The files that should be excluded. Takes second priority")]
        public List<string> FilesToExclude = new List<string>();

        [Tooltip("The folders that should be included. Tied for third priority with " +
            "exclusion folders")]
        public List<string> FoldersToInclude = new List<string>();

        [Tooltip("The files that should be included. Takes top priority")]
        public List<string> FilesToInclude = new List<string>();

        [Tooltip("True to include the file if there are no rules applied that affect it, false " +
            "to exclude the file if there are no rules applied that affect it")]
        public bool IncludeIfNoMatch = false;

        /// <summary>
        /// Finds all of the assets based on the provided filtering
        /// </summary>
        /// <param name="queryString">The filter to be used in the same style
        /// as https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html </param>
        /// <returns>The collection of valid assets</returns>
        public string[] FindAssetPaths(string queryString)
        {
            IEnumerable<string> searchResults = AssetDatabase.FindAssets(queryString);

            List<string> filterResults = new List<string>();

            foreach (string assetGuid in searchResults)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);

                if (ShouldBeIncluded(assetPath))
                    filterResults.Add(assetPath);
            }

            return filterResults.ToArray();
        }

        /// <summary>
        /// Determines if the provided asset path should be considered
        /// a desired or undesired asset
        /// </summary>
        /// <param name="assetPath">The path to the asset</param>
        /// <returns>True if it should be included, false if not</returns>
        private bool ShouldBeIncluded(string assetPath)
        {
            if (FilesToInclude.Contains(assetPath))
                return true;

            if (FilesToExclude.Contains(assetPath))
                return false;

            int highestExclusionIndex = FoldersToExclude.FindBestContainsMatch(assetPath);
            int highestInclusionIndex = FoldersToInclude.FindBestContainsMatch(assetPath);

            if (highestExclusionIndex < 0 && highestInclusionIndex < 0)
                return IncludeIfNoMatch;

            if (highestExclusionIndex > highestInclusionIndex)
                return false;
            else
                return true;
        }
    }
}