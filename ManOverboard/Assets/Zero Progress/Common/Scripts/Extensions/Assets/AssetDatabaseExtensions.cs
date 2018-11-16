using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

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
        /// Checks if the provided asset name is a unique sub-asset name
        /// </summary>
        /// <param name="mainAsset">The main asset hosting the sub-assets</param>
        /// <param name="name">The name to check</param>
        /// <param name="comparisonMode">How to compare the names</param>
        /// <returns>True if it's unique, false if not</returns>
        public static bool IsUniqueSubAssetName(UnityEngine.Object mainAsset, 
            string name, StringComparison comparisonMode)
        {
            IEnumerable<UnityEngine.Object> subAssets = 
                GetSubAssetsOfType<UnityEngine.Object>(mainAsset);

            return subAssets.Where((x) =>
                x.name.Equals(name, comparisonMode)).FirstOrDefault() == null;
        }

        /// <summary>
        /// Adds a number to the end of the name until it's unique
        /// </summary>
        /// <param name="mainAsset">The main asset</param>
        /// <param name="name">The name to make unique</param>
        /// <param name="comparisonMode">How to compare against existing names</param>
        /// <returns>A unique asset name. Either the name provided, or the name_# setup</returns>
        public static string GetUniqueSubAssetName(UnityEngine.Object mainAsset,
            string name, StringComparison comparisonMode)
        {
            string generatedName = name;

            int index = 1;

            while (!IsUniqueSubAssetName(mainAsset, generatedName, comparisonMode))
            {
                generatedName = name + "_" + index.ToString();
                index++;
            }

            return generatedName;
        }

        /// <summary>
        /// Deletes all subassets whose name match the provided string
        /// </summary>
        /// <param name="mainAsset">The main asset object</param>
        /// <param name="subAssetName">The name of the subasset to be matched</param>
        /// <param name="compareMode">How to compare the names</param>
        public static void DeleteSubAssetByName(UnityEngine.Object mainAsset,
            string subAssetName, StringComparison compareMode = StringComparison.OrdinalIgnoreCase)
        {
            string mainAssetPath = AssetDatabase.GetAssetPath(mainAsset);

            IEnumerable<UnityEngine.Object> subAssets = GetSubAssetsOfType<UnityEngine.Object>(mainAssetPath);

            subAssets = subAssets.Where((x) => x.name.Equals(subAssetName, compareMode));

            foreach (UnityEngine.Object subAsset in subAssets)
            {
                UnityEngine.Object.DestroyImmediate(subAsset, true);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(mainAssetPath);
        }

        /// <summary>
        /// Deletes the given subasset from the asset database
        /// </summary>
        /// <param name="target">The sub asset to be deleted</param>
        public static void DeleteSubAsset(UnityEngine.Object target)
        {
            string mainAssetPath = AssetDatabase.GetAssetPath(target);

            UnityEngine.Object.DestroyImmediate(target, true);

            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(mainAssetPath);
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
            
        /// <summary>
        /// Gets all assets of the provided type
        /// </summary>
        /// <typeparam name="T">The type to get</typeparam>
        /// <returns>Collection of the assets that match the desired type</returns>
        public static IEnumerable<T> GetAssetsOfType<T>() where T: UnityEngine.Object
        {
            return GetAssetsOfType(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Retrieves all assets of the provided type
        /// </summary>
        /// <param name="assetType">The type of asset to retrieve. Must inherit from UnityEngine.Object</param>
        /// <returns>Collection of the assets that match the desired type</returns>
        public static IEnumerable<UnityEngine.Object> GetAssetsOfType(Type assetType)
        {
            string[] guids = AssetDatabase.FindAssets("t:" + assetType.Name);

            UnityEngine.Object[] assets = new UnityEngine.Object[guids.Length];

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            }

            return assets;
        }

        /// <summary>
        /// Retrieves all of the subassets of the provided object that match the provided type
        /// </summary>
        /// <typeparam name="T">The type of subasset to retrieve</typeparam>
        /// <param name="mainAsset">The main asset</param>
        /// <returns>The sub assets that match the provided type</returns>
        public static IEnumerable<T> GetSubAssetsOfType<T>(UnityEngine.Object mainAsset) where T : UnityEngine.Object
        {
            return GetSubAssetsOfType(mainAsset, typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Retrieves all of the subassets at the provided path that match the provided type
        /// </summary>
        /// <param name="mainAsset">The main asset</param>
        /// <param name="assetType">The type of the asset to retrieve</param>
        /// <returns>The sub assets that match the provided type</returns>
        public static IEnumerable<UnityEngine.Object> GetSubAssetsOfType(
            UnityEngine.Object mainAsset, System.Type assetType)
        {
            string mainAssetPath = AssetDatabase.GetAssetPath(mainAsset);

            return GetSubAssetsOfType(mainAssetPath, assetType);
        }

        /// <summary>
        /// Retrieves all of the subassets at the provided path that match the provided type
        /// </summary>
        /// <typeparam name="T">The type of subasset to retrieve</typeparam>
        /// <param name="mainAssetPath">The path to the main asset</param>
        /// <returns>The sub assets that match the provided type</returns>
        public static IEnumerable<T> GetSubAssetsOfType<T>(string mainAssetPath) where T:UnityEngine.Object
        {
            return GetSubAssetsOfType(mainAssetPath, typeof(T)).Cast<T>();
        }
        
        /// <summary>
        /// Retrieves all of the subassets at the provided path that match the provided type
        /// </summary>
        /// <param name="mainAssetPath">The path to the main asset</param>
        /// <param name="assetType">The type of the asset to retrieve</param>
        /// <returns>The sub assets that match the provided type</returns>
        public static IEnumerable<UnityEngine.Object> GetSubAssetsOfType(
            string mainAssetPath, System.Type assetType)
        {
            List<UnityEngine.Object> loadedSubassets =
                LoadAllAssetsAtPath(mainAssetPath, assetType).ToList();
            
            loadedSubassets.RemoveAll((x) =>
            {
                return AssetDatabase.IsMainAsset(x);
            });

            return loadedSubassets;
        }
        
        /// <summary>
        /// Retrieves all of the assets of the specified type from the provided path
        /// </summary>
        /// <typeparam name="T">The type of subasset to retrieve</typeparam>
        /// <param name="mainAssetPath">The path of the main asset</param>
        /// <returns>Collection of assets that match the provided type</returns>
        public static IEnumerable<T> LoadAllAssetsAtPath<T>(string mainAssetPath) where T:UnityEngine.Object
        {
            return LoadAllAssetsAtPath(mainAssetPath, typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Retrieves all of the assets of the specified type from the provided path
        /// </summary>
        /// <param name="mainAssetPath">The path of the main asset</param>
        /// <param name="assetType">The type of subasset to retrieve</param>
        /// <returns>Collection of assets that match the provided type</returns>
        public static IEnumerable<UnityEngine.Object> LoadAllAssetsAtPath(
            string mainAssetPath, System.Type assetType)
        {
            if (!assetType.IsOrIsSubclassOf(typeof(UnityEngine.Object)))
                return new UnityEngine.Object[0];

            List<UnityEngine.Object> subAssets = AssetDatabase.LoadAllAssetsAtPath(mainAssetPath).ToList();

            subAssets.RemoveAll((x) => !(x.GetType().IsOrIsSubclassOf(assetType)));

            return subAssets;
        }

        /// <summary>
        /// Retrieves the first subasset that matches the provided type
        /// </summary>
        /// <typeparam name="T">The type of subasset to retrieve</typeparam>
        /// <param name="mainAsset">The main asset</param>
        /// <returns>The first asset found of the provided type, or null if not found</returns>
        public static T GetFirstSubAssetOf<T>(UnityEngine.Object mainAsset)
            where T : UnityEngine.Object
        {
            string mainAssetPath = AssetDatabase.GetAssetPath(mainAsset);
            return GetFirstSubAssetOf<T>(mainAssetPath);
        }

        /// <summary>
        /// Retrieves the first subasset that matches the provided type
        /// </summary>
        /// <typeparam name="T">The type of subasset to retrieve</typeparam>
        /// <param name="mainAssetPath">The path of the main asset</param>
        /// <returns>The first asset found of the provided type, or null if not found</returns>
        public static T GetFirstSubAssetOf<T>(string mainAssetPath)
            where T : UnityEngine.Object
        {
            IEnumerable<T> subAssets = GetSubAssetsOfType<T>(mainAssetPath);
            return subAssets.FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first subasset that matches the provided type
        /// </summary>
        /// <param name="mainAsset">The main asset</param>
        /// <param name="subAssetType">The type of the subasset to retrieve</param>
        /// <returns>The first asset found of the provided type, or null if not found</returns>
        public static UnityEngine.Object GetFirstSubAssetOf(
            UnityEngine.Object mainAsset, System.Type subAssetType)
        {
            IEnumerable<UnityEngine.Object> subAssets = 
                GetSubAssetsOfType(mainAsset, subAssetType);

            return subAssets.FirstOrDefault();
        }

        /// <summary>
        /// Retrieves the first subasset that matches the provided type
        /// </summary>
        /// <param name="mainAssetPath">The path to the main asset</param>
        /// <param name="subAssetType">The type of the subasset to retrieve</param>
        /// <returns>The first asset found of the provided type, or null if not found</returns>
        public static UnityEngine.Object GetFirstSubAssetOf(
            string mainAssetPath, System.Type subAssetType)
        {
            IEnumerable<UnityEngine.Object> subAssets =
                GetSubAssetsOfType(mainAssetPath, subAssetType);

            return subAssets.FirstOrDefault();
        }

        /// <summary>
        /// Finds or creates a sub asset of the provided scriptable object type
        /// </summary>
        /// <typeparam name="T">The type to find or create</typeparam>
        /// <param name="mainAsset">The main asset</param>
        /// <param name="newObjectName">The name to assign to the object if it's created</param>
        /// <returns>The found or created subasset instance</returns>
        public static T FindOrCreateSubScriptableObject<T>(UnityEngine.Object mainAsset, string newObjectName)
            where T:UnityEngine.ScriptableObject
        {
            string mainAssetPath = AssetDatabase.GetAssetPath(mainAsset);

            return FindOrCreateSubScriptableObject<T>(mainAssetPath, newObjectName);
        }

        /// <summary>
        /// Finds or creates a sub asset of the provided scriptable object type
        /// </summary>
        /// <typeparam name="T">The type to find or create</typeparam>
        /// <param name="mainAssetPath">The path of the main asset</param>
        /// <param name="newObjectName">The name to assign to the object if it's created</param>
        /// <returns>The found or created subasset instance</returns>
        public static T FindOrCreateSubScriptableObject<T>(string mainAssetPath, 
            string newObjectName, out bool wasCreated)
            where T : UnityEngine.ScriptableObject
        {
            return FindOrCreateSubScriptableObject(mainAssetPath, typeof(T), 
                newObjectName, out wasCreated) as T;
        }

        /// <summary>
        /// Finds or creates a sub asset of the provided scriptable object type
        /// </summary>
        /// <typeparam name="T">The type to find or create</typeparam>
        /// <param name="mainAssetPath">The path of the main asset</param>
        /// <param name="newObjectName">The name to assign to the object if it's created</param>
        /// <returns>The found or created subasset instance</returns>
        public static T FindOrCreateSubScriptableObject<T>(string mainAssetPath, string newObjectName)
            where T : UnityEngine.ScriptableObject
        {
            bool wasCreated = false;
            return FindOrCreateSubScriptableObject(mainAssetPath, typeof(T), 
                newObjectName, out wasCreated) as T;
        }

        /// <summary>
        /// Finds or creates a sub asset of the provided scriptable object type
        /// </summary>
        /// <param name="mainAsset">The main asset</param>
        /// <param name="newObjectName">The name to assign to the object if it's created</param>
        /// <param name="assetType">The type to find or create</param>
        /// <returns>The found or created subasset instance</returns>
        public static UnityEngine.Object FindOrCreateSubScriptableObject(
            UnityEngine.Object mainAsset, System.Type assetType, string newObjectName, out bool wasCreated)
        {
            string mainAssetPath = AssetDatabase.GetAssetPath(mainAsset);

            return FindOrCreateSubScriptableObject(mainAssetPath, assetType, newObjectName, out wasCreated);
        }

        /// <summary>
        /// Finds or creates a sub asset of the provided scriptable object type
        /// </summary>
        /// <typeparam name="T">The type of object to be returned</typeparam>
        /// <param name="mainAsset">The main asset</param>
        /// <param name="newObjectName">The name to assign to the object if it's created</param>
        /// <param name="assetType">The type to find or create</param>
        /// <returns>The found or created subasset instance</returns>
        public static T FindOrCreateSubScriptableObject<T>(
            UnityEngine.Object mainAsset, System.Type assetType, string newObjectName)
            where T: UnityEngine.ScriptableObject
        {
            bool wasCreated = false;

            return (T)FindOrCreateSubScriptableObject(mainAsset, assetType, newObjectName, out wasCreated);
        }
        
        /// <summary>
        /// Finds or creates a sub asset of the provided scriptable object type
        /// </summary>
        /// <typeparam name="T">The type of object to be returned</typeparam>
        /// <param name="mainAsset">The main asset</param>
        /// <param name="newObjectName">The name to assign to the object if it's created</param>
        /// <param name="assetType">The type to find or create</param>
        /// <returns>The found or created subasset instance</returns>
        public static T FindOrCreateSubScriptableObject<T>(
            string mainAssetPath, System.Type assetType, string newObjectName)
            where T : UnityEngine.ScriptableObject
        {
            bool wasCreated = false;

            return (T)FindOrCreateSubScriptableObject(mainAssetPath,
                assetType, newObjectName, out wasCreated);
        }

        /// <summary>
        /// Finds or creates a sub asset of the provided scriptable object type
        /// </summary>
        /// <typeparam name="T">The type of object to be returned</typeparam>
        /// <param name="mainAsset">The main asset</param>
        /// <param name="newObjectName">The name to assign to the object if it's created</param>
        /// <param name="assetType">The type to find or create</param>
        /// <returns>The found or created subasset instance</returns>
        public static T FindOrCreateSubScriptableObject<T>(
            string mainAssetPath, System.Type assetType, string newObjectName, out bool wasCreated)
            where T : UnityEngine.ScriptableObject
        {
            return (T)FindOrCreateSubScriptableObject(mainAssetPath, 
                assetType, newObjectName, out wasCreated);
        }

        /// <summary>
        /// Finds or creates a sub asset of the provided scriptable object type
        /// </summary>
        /// <typeparam name="T">The type to find or create</typeparam>
        /// <param name="mainAssetPath">The path of the main asset</param>
        /// <param name="newObjectName">The name to assign to the object if it's created</param>
        /// <returns>The found or created subasset instance</returns>
        public static ScriptableObject FindOrCreateSubScriptableObject(string mainAssetPath, 
            System.Type assetType, string newObjectName, out bool wasCreated)
        {
            wasCreated = false;

            if (!assetType.IsOrIsSubclassOf(typeof(ScriptableObject)))
                return null;

            UnityEngine.Object foundSubAsset = GetFirstSubAssetOf(mainAssetPath, assetType);

            if (foundSubAsset == null)
            {
                foundSubAsset = ScriptableObject.CreateInstance(assetType);
                foundSubAsset.name = newObjectName;

                AssetDatabase.AddObjectToAsset(foundSubAsset, mainAssetPath);
                AssetDatabase.ImportAsset(mainAssetPath);

                wasCreated = true;
            }

            return (ScriptableObject)foundSubAsset;
        }

        /// <summary>
        /// Creates an instance of a scriptable object as a subasset of the
        /// provided main asset
        /// </summary>
        /// <typeparam name="T">Type of the subasset to create</typeparam>
        /// <param name="mainAsset">The asset to add the scriptable object to</param>
        /// <param name="newObjectName">The name of the new subasset</param>
        /// <returns>The created scriptable object</returns>
        public static T CreateSubScriptableObject<T>(UnityEngine.Object mainAsset, string newObjectName)
            where T : ScriptableObject
        {
            return CreateSubScriptableObject(mainAsset, typeof(T), newObjectName) as T;
        }

        /// <summary>
        /// Creates an instance of a scriptable object as a subasset of the
        /// provided main asset
        /// </summary>
        /// <typeparam name="T">Type of the subasset to create</typeparam>
        /// <param name="mainAssetPath">The path of the asset to add the 
        /// scriptable object to</param>
        /// <param name="newObjectName">The name of the new subasset</param>
        /// <returns>The created scriptable object</returns>
        public static T CreateSubScriptableObject<T>(string mainAssetPath, string newObjectName)
            where T : ScriptableObject
        {
            return CreateSubScriptableObject(mainAssetPath, typeof(T), newObjectName) as T;
        }

        /// <summary>
        /// Creates an instance of a scriptable object as a subasset of the
        /// provided main asset
        /// </summary>
        /// <param name="mainAsset">The asset to add the scriptable object to</param>
        /// <param name="assetType">The type of subasset to create</param>
        /// <param name="newObjectName">The name of the new subasset</param>
        /// <returns>The created scriptable object</returns>
        public static ScriptableObject CreateSubScriptableObject(UnityEngine.Object mainAsset,
            System.Type assetType, string newObjectName)
        {
            string mainAssetPath = AssetDatabase.GetAssetPath(mainAsset);

            return CreateSubScriptableObject(mainAssetPath, assetType, newObjectName);
        }

        /// <summary>
        /// Creates an instance of a scriptable object as a subasset of the
        /// provided main asset
        /// </summary>
        /// <param name="mainAsset">The path of the main asset to add 
        /// the scriptable object to</param>
        /// <param name="assetType">The type of subasset to create</param>
        /// <param name="newObjectName">The name of the new subasset</param>
        /// <returns>The created scriptable object</returns>
        public static ScriptableObject CreateSubScriptableObject(string mainAssetPath,
            System.Type assetType, string newObjectName)
        {
            ScriptableObject created = ScriptableObject.CreateInstance(assetType);
            created.name = newObjectName;

            AssetDatabase.AddObjectToAsset(created, mainAssetPath);
            AssetDatabase.ImportAsset(mainAssetPath);

            return created;
        }

        /// <summary>
        /// Find all of the assets that have the provided extension
        /// </summary>
        /// <typeparam name="T">The type to retrieve</typeparam>
        /// <param name="fileExtension">The extension of the file to retrieve</param>
        /// <returns>The assets loaded from the files that had the specified extension</returns>
        public static IEnumerable<T> FindAssetsWithExtension<T>(string fileExtension) where T : UnityEngine.Object
        {
            return FindAssetsWithAnyExtension<T>(new string[] { fileExtension });
        }

        /// <summary>
        /// Finds all asset paths that have the provided file extension
        /// </summary>
        /// <param name="fileExtension">The file extension to search for</param>
        /// <returns>The paths of all the assets that have the provided extension
        /// (relative to the Application.dataPath and without extension)</returns>
        public static IEnumerable<string> FindAssetPathsWithExtension(string fileExtension)
        {
            return FindAssetPathsWithAnyExtension(new string[] { fileExtension });
        }

        /// <summary>
        /// Find all of the assets that have any of the provided extensions
        /// </summary>
        /// <typeparam name="T">The type to retrieve</typeparam>
        /// <param name="fileExtension">The extension of the file to retrieve</param>
        /// <returns>The assets loaded from the files that had the specified extension</returns>
        public static IEnumerable<T> FindAssetsWithAnyExtension<T>(IEnumerable<string> fileExtensions) where T : UnityEngine.Object
        {
            IEnumerable<string> paths = FindAssetPathsWithAnyExtension(fileExtensions);
            
            List<T> assetsOfType = new List<T>();

            foreach (string path in paths)
            {
                T asset = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;

                if (asset == null)
                    continue;

                assetsOfType.Add(asset);
            }

            return assetsOfType;
        }

        /// <summary>
        /// Finds all asset paths that have the provided file extension
        /// </summary>
        /// <param name="fileExtension">The file extension to search for</param>
        /// <returns>The paths of all the assets that have the provided extension
        /// (relative to the Application.dataPath and without extension)</returns>
        public static IEnumerable<string> FindAssetPathsWithAnyExtension(IEnumerable<string> fileExtensions)
        {
            // Sanitize the provided collection
            fileExtensions = fileExtensions.Select((fExt) =>
               {
                   if (fExt[0] != '.')
                       return "." + fExt;

                   return fExt;
               });

            DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);

            IEnumerable<FileInfo> fileInfos =
                directoryInfo.GetFiles("*.*", SearchOption.AllDirectories)
                .Where((f) => fileExtensions.Contains(f.Extension));
            
            // Make relative to the Assets/ folder and replace backslashes
            return fileInfos.Select((file) =>
               {
                   return file.FullName.Replace(@"\", "/")
                        .Replace(Application.dataPath, "Assets");
               });
        }

        /// <summary>
        /// Helper to modify the provided path to be safe for asset creation
        /// </summary>
        /// <param name="path">The provided path, local to the project
        /// 
        /// i.e. Assets/Scripts/this.cs
        /// </param>
        /// <param name="desiredExtension">The extension to be set on the item 
        /// (even if an extension is provided)</param>
        /// <returns>The valid path, or null if an error was encountered</returns>
        public static string GetValidAssetPath(string path, string desiredExtension)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("No path provided, cannot generate code");
                return null;
            }
            
            if (path.StartsWith("Assets/"))
                path = path.Remove(0, "Assets".Length);

            path = Application.dataPath + path;

            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);

            if (string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("No file name provided, cannot create file");
                return null;
            }

            path = System.IO.Path.ChangeExtension(path, desiredExtension);

            return path;
        }

        /// <summary>
        /// Iterates over assets by loading them and then returning them
        /// 
        /// Automatically fills the FindAssets filter with t: AssetType
        /// </summary>
        /// <typeparam name="AssetType">The type of asset to search for</typeparam>
        /// <param name="folders">Folders to restrict the search to</param>
        /// <returns>Enumerator to iterate over each ite</returns>
        public static IEnumerable<AssetType> GetAssetIterator<AssetType>(string[] folders = null)
            where AssetType: UnityEngine.Object
        {
            string[] assetGuids = AssetDatabase.FindAssets("t: " + typeof(AssetType).Name, folders);

            foreach (string guid in assetGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                AssetType assetType = AssetDatabase.LoadAssetAtPath<AssetType>(path);

                yield return assetType;
            }
        }
    }
}