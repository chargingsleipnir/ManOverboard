using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Handles the context menu for connectors
    /// </summary>
    public class ConnectorRightClickMode : ContextMenuInput
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeEditor">Node editor to work on</param>
        /// <param name="priority">When this mode should be processed relative
        /// to other input modes. Lower number == processed first</param>
        public ConnectorRightClickMode(NodeEditor nodeEditor, int priority = 0)
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

            Connector connector = NodeEditor.GetNodeConnectorUnderMouse();

            return connector != null && connector.HasContextMenu;
        }

        /// <summary>
        /// Shows the menu
        /// </summary>
        protected override void OnActivated()
        {
            Connector connectorUnderMouse = NodeEditor.GetNodeConnectorUnderMouse();

            if (connectorUnderMouse.IsSelectable && !connectorUnderMouse.IsSelected())
            {
                NodeEditor.UnselectAll();
                connectorUnderMouse.Select();
            }

            UpdateActiveMenu(connectorUnderMouse.ContextMenu, connectorUnderMouse);

            Event.current.Use();
        }
    }
}
