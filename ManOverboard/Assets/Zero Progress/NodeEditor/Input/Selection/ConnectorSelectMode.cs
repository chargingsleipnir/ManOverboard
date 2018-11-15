using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Handles selection for single connections
    /// </summary>
    public class ConnectorSelectMode : SelectionInputMode
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeEditor">Node editor to work on</param>
        /// <param name="priority">When this mode should be processed relative
        /// to other input modes. Lower number == processed first</param>
        public ConnectorSelectMode(NodeEditor nodeEditor, int priority = 0) 
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

        public override void BeforeEditorEvents()
        {
            CheckActivation();

            if (!IsActivated)
                return;

            if (!Event.current.IsLeftClickEvent())
                return;

            bool multiSelectActive = Event.current.modifiers == EventModifiers.Control ||
                Event.current.modifiers == EventModifiers.Shift;

            Connector clickedConnector = NodeEditor.GetNodeConnectorUnderMouse();

            if (clickedConnector == null)
                return;            

            if (!clickedConnector.IsSelectable)
                return;

            bool isSelected = clickedConnector.IsSelected();

            if (!multiSelectActive)
            {
                if (!isSelected)
                {
                    NodeEditor.UnselectAll();
                    clickedConnector.Select();
                }
            }
            else
                clickedConnector.ToggleSelected();

            NodeEditor.RaiseSelectionChanged();
            NodeEditor.FlagRepaint();
            Event.current.Use();
        }
    }
}