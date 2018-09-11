using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Helpers to extend the EditorGUILayout class. However, since
    /// EditorGUILayout is static, these aren't 'true' extension methods
    /// </summary>
    public static class EditorGUILayoutExtensions
    {
        /// <summary>
        /// Drag receiver for the Folder Textbox
        /// </summary>
        private static DragReceiver folderReceiver;
        
        /// <summary>
        /// Creates a text field that can have any item dragged onto it and the folder
        /// it is a part of populates the text field
        /// </summary>
        /// <param name="folderValue">The current value</param>
        /// <param name="label">Any label to display</param>
        /// <param name="rectOffset">Offset to apply to the rectangle. This is necessary because the position is returned
        /// relative to the EditorWindow. To get this value from an EditorWindow class, simply pass in position.position. 
        /// If you don't need an offset, pass in Vector2.zero</param>
        /// <param name="options">Any layout options to forward along</param>
        /// <returns>The new input if changes were made, otherwise the existing</returns>
        public static string FolderTextbox(string folderValue, GUIContent label, Vector2 rectOffset, params GUILayoutOption[] options)
        {
            if (folderReceiver == null)
                folderReceiver = new DragReceiver(DragReceiver.IsValidFunc, UnityEditor.DragAndDropVisualMode.Link);

            folderValue = EditorGUILayout.DelayedTextField(label, folderValue, options);

            folderReceiver.ReceiverBox = GUILayoutUtility.GetLastRect();

            folderReceiver.ReceiverBox = new Rect(folderReceiver.ReceiverBox.x + rectOffset.x,
                folderReceiver.ReceiverBox.y + rectOffset.y, folderReceiver.ReceiverBox.width, folderReceiver.ReceiverBox.height);

            System.EventHandler action = (sender, e) =>
            {
                folderValue = DragAndDrop.paths[0];

                if (!System.IO.Directory.Exists(folderValue))
                    folderValue = System.IO.Path.GetDirectoryName(DragAndDrop.paths[0]);

                if (!folderValue.EndsWith("/"))
                    folderValue += "/";
            };

            folderReceiver.OnDragComplete += action;

            folderReceiver.Update();

            folderReceiver.OnDragComplete -= action;

            return folderValue;
        }
        
    }
}