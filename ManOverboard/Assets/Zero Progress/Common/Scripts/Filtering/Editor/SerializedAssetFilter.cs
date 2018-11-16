using System.Collections.Generic;
using UnityEditor;

namespace ZeroProgress.Common.Editors
{
    public class SerializedAssetFilter
    {
        public SerializedProperty IncludedFoldersProperty { get; private set; }
        public SerializedProperty IncludedFilesProperty { get; private set; }
        public SerializedProperty ExcludedFoldersProperty { get; private set; }
        public SerializedProperty ExcludedFilesProperty { get; private set; }

        public SerializedProperty DefaultInclusionRule { get; private set; }

        #region Display Lists Cache

        private IEnumerable<string> includedFoldersCache;

        public IEnumerable<string> IncludedFoldersCache
        {
            get
            {
                if (includedFoldersCache == null)
                    includedFoldersCache = IncludedFoldersProperty.GetSafeArrayValues<string>();

                return includedFoldersCache;
            }
            set { includedFoldersCache = value; }
        }

        private IEnumerable<string> includedFilesCache;

        public IEnumerable<string> IncludedFilesCache
        {
            get
            {
                if (includedFilesCache == null)
                    includedFilesCache = IncludedFilesProperty.GetSafeArrayValues<string>();

                return includedFilesCache;
            }
            set { includedFilesCache = value; }
        }

        private IEnumerable<string> excludedFoldersCache;

        public IEnumerable<string> ExcludedFoldersCache
        {
            get
            {
                if (excludedFoldersCache == null)
                    excludedFoldersCache = ExcludedFoldersProperty.GetSafeArrayValues<string>();

                return excludedFoldersCache;
            }
            set { excludedFoldersCache = value; }
        }

        private IEnumerable<string> excludedFilesCache;

        public IEnumerable<string> ExcludedFilesCache
        {
            get
            {
                if (excludedFilesCache == null)
                    excludedFilesCache = ExcludedFilesProperty.GetSafeArrayValues<string>();

                return excludedFilesCache;
            }
            set { excludedFilesCache = value; }
        }

        #endregion

        public void ExtractSerializedProperties(SerializedObject ObjectToExtract)
        {
            IncludedFoldersProperty = ObjectToExtract.FindProperty("FoldersToInclude");
            IncludedFilesProperty = ObjectToExtract.FindProperty("FilesToInclude");
            ExcludedFoldersProperty = ObjectToExtract.FindProperty("FoldersToExclude");
            ExcludedFilesProperty = ObjectToExtract.FindProperty("FilesToExclude");

            DefaultInclusionRule = ObjectToExtract.FindProperty("IncludeIfNoMatch");
        }

        public void RefreshCacheListing()
        {
            includedFilesCache = includedFoldersCache = 
                excludedFilesCache = excludedFoldersCache = null;
        }

        public void AddIncludedFolder(string Path)
        {
            AddToArray(IncludedFoldersProperty, Path, ref includedFoldersCache);
        }

        public void AddIncludedFile(string Path)
        {
            AddToArray(IncludedFilesProperty, Path, ref includedFilesCache);
        }

        public void AddExcludedFolder(string Path)
        {
            AddToArray(ExcludedFoldersProperty, Path, ref excludedFoldersCache);
        }

        public void AddExcludedFile(string Path)
        {
            AddToArray(ExcludedFilesProperty, Path, ref excludedFilesCache);
        }

        private void AddToArray(SerializedProperty Property, string ValueToAdd, ref IEnumerable<string> Cache)
        {
            Property.AddUniqueArrayValue(ValueToAdd);
            Cache = Property.GetSafeArrayValues<string>();
        }

        public void RemoveIncludedFolder(string Path)
        {
            RemoveFromArray(IncludedFoldersProperty, Path, ref includedFoldersCache);
        }

        public void RemoveIncludedFile(string Path)
        {
            RemoveFromArray(IncludedFilesProperty, Path, ref includedFilesCache);
        }

        public void RemoveExcludedFolder(string Path)
        {
            RemoveFromArray(ExcludedFoldersProperty, Path, ref excludedFoldersCache);
        }

        public void RemoveExcludedFile(string Path)
        {
            RemoveFromArray(ExcludedFilesProperty, Path, ref excludedFilesCache);
        }

        private void RemoveFromArray(SerializedProperty Property, string ValueToRemove, ref IEnumerable<string> Cache)
        {
            Property.RemoveFromArray(ValueToRemove);
            Cache = Property.GetSafeArrayValues<string>();
        }
    }
}