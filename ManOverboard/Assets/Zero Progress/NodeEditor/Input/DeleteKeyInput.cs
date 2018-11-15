using UnityEngine;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Input mode for responding to the delete key press
    /// </summary>
    public class DeleteKeyInput : InputMode
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeEditor">Node editor to work on</param>
        /// <param name="priority">When this mode should be processed relative
        /// to other input modes. Lower number == processed first</param>
        public DeleteKeyInput(NodeEditor nodeEditor, int priority = 0) 
            : base(nodeEditor, priority)
        {
        }

        /// <summary>
        /// Should activate only if the required key combination is pressed
        /// </summary>
        /// <returns>True if should activate, false if not</returns>
        protected override bool ShouldActivate()
        {
            return true;
        }

        /// <summary>
        /// Check if delete key is pressed
        /// </summary>
        public override void BeforeEditorEvents()
        {
            CheckActivation();

            if (!IsActivated)
                return;

            if (Event.current.type != EventType.KeyDown)
                return;

            if (Event.current.keyCode == KeyCode.Delete)
            {
                NodeEditor.RemoveSelectedNodes();
                NodeEditor.RemoveSelectedConnectors();
                Event.current.Use();
            }
        }
    }
}