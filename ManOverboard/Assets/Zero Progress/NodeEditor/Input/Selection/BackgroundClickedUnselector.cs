using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Checks for when the background of the node editor is clicked
    /// </summary>
    public class BackgroundClickedUnselector : SelectionInputMode
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeEditor">Node editor to work on</param>
        /// <param name="priority">When this mode should be processed relative
        /// to other input modes. Lower number == processed first</param>
        public BackgroundClickedUnselector(NodeEditor nodeEditor, int priority = 10) 
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
        /// Fired after the node editor events
        /// </summary>
        public override void AfterEditorEvents()
        {
            CheckActivation();

            if (!IsActivated)
                return;

            if (!Event.current.IsLeftClickEvent())
                return;

            if (Event.current.modifiers != EventModifiers.None)
                return;

            Node clickedNode = NodeEditor.GetNodeUnderMouse();
            Connector clickedConnector = NodeEditor.GetNodeConnectorUnderMouse();

            if (clickedNode == null && clickedConnector == null)
            {
                NodeEditor.UnselectAll();
                NodeEditor.RaiseSelectionChanged();
                NodeEditor.FlagRepaint();
            }
        }
    }
}
