using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    [CustomEditor(typeof(AssetFilter))]
    public class AssetFilterEditor : Editor
    {
        private SerializedAssetFilter filterProperties;

        private readonly Color inclusionColor = new Color(0.3f, 0.9f, 0.4f);
        private readonly Color exclusionColor = new Color(0.8f, 0.3f, 0.3f);

        private GUIStyle midCenterLabelledBoxStyle;
        private GUIStyle pathEntryValueStyle;

        private DragReceiver inclusionDragReceiver;
        private DragReceiver exclusionDragReceiver;

        private bool includedFoldersFoldoutValue = true, includedFilesFoldoutValue = true;
        private bool excludedFoldersFoldoutValue = true, excludedFilesFoldoutValue = true;

        private string queryString = "";

        public bool DisplayTestingSection = true;

        public override void OnInspectorGUI()
        {
            if (inclusionDragReceiver == null)
                Initialize();

            Render();
        }

        private void Initialize()
        {
            filterProperties = new SerializedAssetFilter();
            filterProperties.ExtractSerializedProperties(serializedObject);

            midCenterLabelledBoxStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                fontStyle = FontStyle.Bold
            };

            pathEntryValueStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                clipping = TextClipping.Clip
            };

            inclusionDragReceiver = new DragReceiver(DragReceiver.IsValidFunc, DragAndDropVisualMode.Link);

            inclusionDragReceiver.OnDragComplete += InclusionDragReceiver_OnDragComplete;

            exclusionDragReceiver = new DragReceiver(DragReceiver.IsValidFunc, DragAndDropVisualMode.Link);

            exclusionDragReceiver.OnDragComplete += ExclusionDragReceiver_OnDragComplete;
        }

        #region DragHandlers

        private void ExclusionDragReceiver_OnDragComplete(object sender, System.EventArgs e)
        {
            DragCompletionHandler(
                (path) => filterProperties.AddExcludedFolder(path),
                (path) => filterProperties.AddExcludedFile(path));
        }

        private void InclusionDragReceiver_OnDragComplete(object sender, System.EventArgs e)
        {
            DragCompletionHandler(
                (path) => filterProperties.AddIncludedFolder(path),
                (path) => filterProperties.AddIncludedFile(path));
        }

        private void DragCompletionHandler(System.Action<string> FolderAction, System.Action<string> FileAction)
        {
            for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
            {
                Object objRef = DragAndDrop.objectReferences[i];
                string objPath = DragAndDrop.paths[i];

                // Folders are considered 'Default Assets'. So we use this and the Valid Folder call
                // to confirm it's most likely a folder
                bool isDefaultAsset = objRef.GetType() == typeof(DefaultAsset);

                if (isDefaultAsset)
                {
                    bool isFolder = AssetDatabase.IsValidFolder(objPath);

                    if (isFolder)
                    {
                        FolderAction(objPath);
                        continue;
                    }
                }

                FileAction(objPath);
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        private void Render()
        {
            serializedObject.Update();
            filterProperties.RefreshCacheListing();

            if (DisplayTestingSection)
                RenderTestSection();
            else
                RenderDefaultInclusionRuleProperty();
            
            RenderDragAndDropReceivers();
            RenderListings();
        }

        private void RenderDragAndDropReceivers()
        {
            Rect dropContainer = EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(80f));

            GUILayout.Space(80f);
            dropContainer.width = (dropContainer.width * 0.5f) - 5f;

            Color originalColor = GUI.backgroundColor;

            GUI.backgroundColor = inclusionColor;
            GUI.Box(dropContainer, "Drag Included Items Here", midCenterLabelledBoxStyle);
            
            inclusionDragReceiver.ReceiverBox = dropContainer.WithPosition(
                GUIUtility.GUIToScreenPoint(dropContainer.position));

            inclusionDragReceiver.Update();

            dropContainer.x += dropContainer.width + 15f;
            dropContainer.width -= 5f;

            GUI.backgroundColor = exclusionColor;
            GUI.Box(dropContainer, "Drag Excluded Items Here", midCenterLabelledBoxStyle);

            GUI.backgroundColor = originalColor;

            exclusionDragReceiver.ReceiverBox = dropContainer.WithPosition(
                GUIUtility.GUIToScreenPoint(dropContainer.position));

            exclusionDragReceiver.Update();

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Renders all of the listings of the Drag-and-Drop interface
        /// </summary>
        private void RenderListings()
        {
            RenderSection(ref includedFoldersFoldoutValue, "Included Folders",
                filterProperties.IncludedFoldersCache, inclusionColor,
                (path) => filterProperties.RemoveIncludedFolder(path));

            EditorGUILayout.Space();

            RenderSection(ref includedFilesFoldoutValue, "Included Files",
                filterProperties.IncludedFilesCache, inclusionColor,
                (path) => filterProperties.RemoveIncludedFile(path));

            EditorGUILayout.Space();

            RenderSection(ref excludedFoldersFoldoutValue, "Excluded Folders",
                filterProperties.ExcludedFoldersCache, exclusionColor,
                (path) => filterProperties.RemoveExcludedFolder(path));

            EditorGUILayout.Space();

            RenderSection(ref excludedFilesFoldoutValue, "Excluded Files",
                filterProperties.ExcludedFilesCache, exclusionColor,
                (path) => filterProperties.RemoveExcludedFile(path));
        }

        /// <summary>
        /// Renders the listing of a singular section (i.e. all of the Included Folders) in
        /// a foldout
        /// </summary>
        /// <param name="DisplaySection">The boolean that represents the foldout state</param>
        /// <param name="FoldoutLabel">The label used to describe the foldout contents</param>
        /// <param name="Entries">Collection of entries to be displayed</param>
        /// <param name="EntryColour">The colour of the entries</param>
        /// <param name="RemoveButtonClickAction">The action taken when the entry remove button is selected</param>
        private void RenderSection(ref bool DisplaySection,
            string FoldoutLabel, IEnumerable<string> Entries,
            Color EntryColour, System.Action<string> RemoveButtonClickAction)
        {
            EditorGUILayout.Space();
            Rect listing = EditorGUILayout.BeginVertical();
            listing.height += 10;

            GUI.Box(listing, "");

            EditorGUI.indentLevel++;

            EditorGUILayout.Space();

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout)
            {
                alignment = TextAnchor.UpperCenter
            };

            // Get the foldout text to be around the midpoint of the box it represents
            foldoutStyle.contentOffset = new Vector2((listing.width * 0.3f), 0);

            DisplaySection = EditorGUILayout.Foldout(DisplaySection, FoldoutLabel, true, foldoutStyle);

            EditorGUILayout.Space();

            if (DisplaySection)
            {
                if (Entries == null)
                    Debug.LogError(FoldoutLabel);

                foreach (string entry in Entries)
                {
                    RenderPathElement(entry, EntryColour, RemoveButtonClickAction);
                }
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Displays an individual path listing in the drag-and-drop interface
        /// </summary>
        /// <param name="Value">The path to be displayed</param>
        /// <param name="BackgroundColor">The color to be used for the element (to color inclusion differently
        /// from exclusion)</param>
        /// <param name="RemoveButtonClickAction">The action to be taken when the remove button for the element is clicked</param>
        private void RenderPathElement(string Value, Color BackgroundColor, System.Action<string> RemoveButtonClickAction)
        {
            Color backgroundColor = GUI.backgroundColor;

            GUI.backgroundColor = BackgroundColor;
            GUIStyle horizontalStyle = new GUIStyle
            {
                margin = new RectOffset(15, 15, 0, 0)
            };

            Rect entryBox = EditorGUILayout.BeginHorizontal(horizontalStyle, GUILayout.Height(20));

            GUI.Box(entryBox, "");

            // Create the content utilizing the same value for the display and tooltip
            // (so if the inspector is skinny you can hover over the entry to see the full display)
            GUIContent labelContent = new GUIContent(Value, Value);

            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField(labelContent, pathEntryValueStyle, GUILayout.Height(20));
            EditorGUI.indentLevel++;

            // Form a margin here manually since the style wasn't working for us
            Rect buttonRect = entryBox;
            buttonRect.x = buttonRect.xMax - 22;
            buttonRect.width = 20;
            buttonRect.height = 19;
            buttonRect.y += 2;

            if (GUI.Button(buttonRect, "X"))
                RemoveButtonClickAction(Value);

            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = backgroundColor;
        }

        private void RenderTestSection()
        {
            queryString = EditorGUILayout.TextField("Query String: ", queryString);

            RenderDefaultInclusionRuleProperty();

            if (GUILayout.Button("Test Filter"))
            {
                AssetFilter filter = target as AssetFilter;

                IEnumerable<string> results = filter.FindAssetPaths(queryString);

                foreach (string result in results)
                {
                    Debug.Log(result);
                }
            }
        }

        private void RenderDefaultInclusionRuleProperty()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(filterProperties.DefaultInclusionRule);

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}