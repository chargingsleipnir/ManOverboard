using UnityEngine;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Handles selection for single nodes
    /// </summary>
    public class NodeSelectMode : SelectionInputMode
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeEditor">Node editor to work on</param>
        /// <param name="priority">When this mode should be processed relative
        /// to other input modes. Lower number == processed first</param>
        public NodeSelectMode(NodeEditor nodeEditor, int priority = 0) 
            : base(nodeEditor, priority)
        {
        }
        
        /// <summary>
        /// Checks whether this mode meets the criteria for activation
        /// </summary>
        /// <returns>Activatable status</returns>
        protected override bool ShouldActivate()
        {
            return true;
        }
        
        /// <summary>
        /// Checks for node selection before the editor processes the
        /// left-clicks
        /// </summary>
        public override void BeforeEditorEvents()
        {
            CheckActivation();

            if (!IsActivated)
                return;

            if (Event.current.type != EventType.MouseDown || 
                Event.current.button != 0)
                return;

            bool multiSelectActive = Event.current.modifiers == EventModifiers.Control ||
                Event.current.modifiers == EventModifiers.Shift;

            Node clickedNode = NodeEditor.GetNodeUnderMouse();

            if (clickedNode == null)
                return;

            if (!clickedNode.IsSelectable)
                return;
            
            bool isNodeSelected = clickedNode.IsSelected();

            if (!multiSelectActive)
            {
                if (!isNodeSelected)
                {
                    NodeEditor.UnselectAll();
                    clickedNode.Select();
                }
            }
            else
            {
                clickedNode.ToggleSelected();
            }

            NodeEditor.RaiseSelectionChanged();
            NodeEditor.FlagRepaint();
            Event.current.Use();
        }
    }
}