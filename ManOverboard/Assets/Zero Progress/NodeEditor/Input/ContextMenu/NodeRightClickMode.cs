using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Handles the context menu for nodes
    /// </summary>
    public class NodeRightClickMode : ContextMenuInput
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeEditor">Node editor to work on</param>
        /// <param name="priority">When this mode should be processed relative
        /// to other input modes. Lower number == processed first</param>
        public NodeRightClickMode(NodeEditor nodeEditor, int priority = 0) 
            : base(nodeEditor, priority)
        {
        }

        /// <summary>
        /// Checks if the node is a valid context menu receiving node
        /// and displays its context menu
        /// </summary>
        /// <returns>True if context menu should be displayed, false if not</returns>
        protected override bool ShouldActivate()
        {
            if (!Event.current.IsRightClickEvent())
                return false;

            Node nodeUnderMouse = NodeEditor.GetNodeUnderMouse();

            return nodeUnderMouse != null && nodeUnderMouse.HasContextMenu;
        }

        /// <summary>
        /// Shows the menu
        /// </summary>
        protected override void OnActivated()
        {
            Node nodeUnderMouse = NodeEditor.GetNodeUnderMouse();

            if (nodeUnderMouse.IsSelectable && !nodeUnderMouse.IsSelected())
            {
                NodeEditor.UnselectAll();
                nodeUnderMouse.Select();
            }

            UpdateActiveMenu(nodeUnderMouse.ContextMenu, nodeUnderMouse);
            Event.current.Use();
        }
    }
}