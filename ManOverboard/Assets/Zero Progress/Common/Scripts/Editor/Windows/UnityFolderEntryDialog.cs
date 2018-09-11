using System;
using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// A simple dialog to allow dragging and dropping of a folder
    /// as a path
    /// </summary>
    public class UnityFolderEntryDialog : EditorWindow
    {
        private static GUIStyle descriptionStyle;

        private static GUIStyle folderTextboxStyle;

        private static GUIStyle dialogButtonStyle;

        private string selectedFolder;

        private DragReceiver folderDrag;

        private bool okClicked = false;

        /// <summary>
        /// The text to display as a description of what the folder is used for
        /// </summary>
        public string DescriptionText = "Drag and drop a folder onto the textbox to auto-populate it";

        /// <summary>
        /// True to return the full path of the item (with the disk drive and such)
        /// False to return the path relative to the Assets folder
        /// </summary>
        public bool ReturnFullDiskPath = false;
        
        /// <summary>
        /// Event for when a folder has been selected
        /// </summary>
        public event EventHandler<EventArgs<string>> OnFolderSelected;

        /// <summary>
        /// Event for when the selection is cancelled
        /// </summary>
        public event EventHandler OnCancel;

        /// <summary>
        /// Sets up the styles used to render the elements of the window
        /// </summary>
        private void InitializeStyles()
        {
            descriptionStyle = new GUIStyle(GUI.skin.label);
            descriptionStyle.wordWrap = true;
            descriptionStyle.margin = new RectOffset(5, 5, 5, 5);
            descriptionStyle.alignment = TextAnchor.MiddleCenter;

            folderTextboxStyle = new GUIStyle(GUI.skin.textField);
            folderTextboxStyle.margin = new RectOffset(5, 5, 5, 5);

            dialogButtonStyle = new GUIStyle(GUI.skin.button);
            dialogButtonStyle.fixedWidth = 80f;
            dialogButtonStyle.alignment = TextAnchor.MiddleCenter;
        }

        private void OnEnable()
        {
            if(folderDrag == null)
                folderDrag = new DragReceiver(DragReceiver.IsValidFunc, DragAndDropVisualMode.Link);

            folderDrag.OnDragComplete += FolderDrag_OnDragComplete;
            okClicked = false;
        }

        private void OnDisable()
        {
            folderDrag.OnDragComplete -= FolderDrag_OnDragComplete;
        }

        private void OnDestroy()
        {
            if (!okClicked)
                Cancel(close: false);
        }

        private void FolderDrag_OnDragComplete(object sender, System.EventArgs e)
        {
            if (DragAndDrop.paths.Length == 0)
                return;

            selectedFolder = DragAndDrop.paths[0];

            if(!System.IO.Directory.Exists(selectedFolder))
                selectedFolder = System.IO.Path.GetDirectoryName(DragAndDrop.paths[0]);

            selectedFolder += "/";
        }
        
        private void OnGUI()
        {
            if (descriptionStyle == null)
                InitializeStyles();
            
            folderDrag.ReceiverBox = position;
            Debug.Log(position);

            Debug.Log(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));

            EditorGUILayout.LabelField(DescriptionText, descriptionStyle);

            EditorGUILayout.Space();

            selectedFolder = EditorGUILayout.TextField("Folder Path:", selectedFolder, folderTextboxStyle);
            
            Rect folderRect = GUILayoutUtility.GetLastRect();
            folderDrag.Update();

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("OK", dialogButtonStyle))
                Ok();

            if (GUILayout.Button("Cancel", dialogButtonStyle))
                Cancel(close: true);

            EditorGUILayout.EndHorizontal();

        }

        /// <summary>
        /// Action to be taken when the cancel button is pressed
        /// </summary>
        private void Cancel(bool close = false)
        {
            OnCancel.SafeInvoke(this);

            if(close)
                Close();
        }

        /// <summary>
        /// Action to be taken when the OK button is pressed
        /// </summary>
        private void Ok()
        {
            if (string.IsNullOrEmpty(selectedFolder))
            {
                EditorUtility.DisplayDialog("No folder selected!", "The selected folder cannot be blank", "OK");
                return;
            }

            OnFolderSelected.SafeInvoke(this, 
                new EventArgs<string>() { Value = selectedFolder });

            okClicked = true;
            Close();
        }

    }
}