using UnityEditor;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Extensions meant for the AssetDatabase class... however, because AssetDatabase is a static
    /// class, these don't work exactly as normal extension methods and must be used explicitly
    /// </summary>
    public static class AssetDatabaseExtensions
    {
        /// <summary>
        /// Retrieves the name of the specified asset
        /// </summary>
        /// <param name="Asset">The object that represents the asset</param>
        /// <param name="WithExtension">True to get the filename with extension, false if not</param>
        /// <returns>The file name of the asset</returns>
        public static string GetAssetFileName(UnityEngine.Object Asset, bool WithExtension = false)
        {
            if (WithExtension)
                return System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Asset));
            else
                return System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(Asset));
        }

        /// <summary>
        /// Deletes all subassets of the specified type under the specified main asset
        /// </summary>
        /// <typeparam name="T">The type to search for in the subassets list</typeparam>
        /// <param name="MainAsset">The asset to get the subassets under</param>
        /// <param name="IgnoreSubAssets">Any subassets to be ignored</param>
        /// <returns>The number of subassets deleted</returns>
        public static int DeleteSubAssetsOfType<T>(UnityEngine.Object MainAsset,
                UnityEngine.Object[] IgnoreSubAssets = null)
                where T : UnityEngine.Object
        {
            string mainAssetPath = AssetDatabase.GetAssetPath(MainAsset);

            UnityEngine.Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(mainAssetPath);

            int deletedCount = 0;

            foreach (UnityEngine.Object subAsset in subAssets)
            {
                if (subAsset == MainAsset)
                    continue;

                if (!subAsset.GetType().IsSubclassOf(typeof(T)))
                    continue;

                if (IgnoreSubAssets != null)
                {
                    bool ignore = false;

                    foreach (UnityEngine.Object ignorable in IgnoreSubAssets)
                    {
                        if (subAsset == ignorable)
                        {
                            ignore = true;
                            break;
                        }
                    }

                    if (ignore)
                        break;
                }

                Editor.DestroyImmediate(subAsset, true);
                deletedCount++;
            }

            return deletedCount;
        }
    }
}