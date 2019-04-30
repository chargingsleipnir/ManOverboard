using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Handles input for selecting multiple nodes/connections
    /// within a rectangle
    /// </summary>
    public class BoxSelectMode : SelectionInputMode
    {
        /// <summary>
        /// Color to make the selection rectangle
        /// </summary>
        public Color SelectionColor = NodeEditor.DefaultSelectionColor;

        /// <summary>
        /// True to allow selecting of nodes, false to not
        /// </summary>
        public bool CanSelectNodes { get; set; }

        /// <summary>
        /// True to allow selecting of connectors, false to not
        /// </summary>
        public bool CanSelectConnectors { get; set; }

        /// <summary>
        /// True to allow overlapping with the selection rect, false to
        /// require full containment before selecting the objects
        /// </summary>
        public bool AllowOverlaps { get; set; }

        /// <summary>
        /// Start point of the box rectangle
        /// </summary>
        private Vector2 dragStartPos;

        /// <summary>
        /// Rectangle for selecting
        /// </summary>
        private Rect selectionRect;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeEditor">Node editor to work on</param>
        /// <param name="priority">When this mode should be processed relative
        /// to other input modes. Lower number == processed first</param>
        public BoxSelectMode(NodeEditor nodeEditor, int priority = 0) : base(nodeEditor, priority)
        {
            CanSelectConnectors = CanSelectNodes = true;
        }

        /// <summary>
        /// Determines if a box select mode should be activated
        /// </summary>
        /// <returns>True to activate, false to not</returns>
        protected override bool ShouldActivate()
        {
            return Event.current.type == EventType.MouseDown &&
                Event.current.modifiers != EventModifiers.Alt;
        }

        /// <summary>
        /// Handles configuring the node editor for box select mode activation
        /// </summary>
        protected override void OnActivated()
        {
            dragStartPos = Event.current.mousePosition;
            NodeEditor.DisableZoom();
        }

        /// <summary>
        /// Handles cancelling box select mode
        /// </summary>
        protected override void OnCancelled()
        {
            NodeEditor.UnselectAll();
            NodeEditor.RaiseSelectionChanged();
        }

        /// <summary>
        /// Handles re-configuring the Node Editor to its original state
        /// </summary>
        protected override void OnDeactivated()
        {
            NodeEditor.EnableZoom();
            selectionRect = Rect.zero;
        }

        /// <summary>
        /// Configures the rectangle used for selection
        /// </summary>
        public override void AfterEditorEvents()
        {
            CheckActivation();

            if (!IsActivated)
                return;
            
            if (Event.current.button != 0)
            {
                Cancel();
                Event.current.Use();
                return;
            }

            if (Event.current.type == EventType.MouseDrag)
            {
                Vector2 startPoint = NodeEditor.ApplyZoomToVector(dragStartPos);
                Vector2 endPoint = NodeEditor.GetZoomedMousePosition();

                selectionRect = RectExtensions.FromPoints(startPoint, endPoint);

                NodeEditor.FlagRepaint();
                Event.current.Use();
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                SelectNodes();
                SelectConnectors();

                NodeEditor.RaiseSelectionChanged();

                selectionRect = Rect.zero;
                Deactivate();

                Event.current.Use();
            }
            else if(Event.current.type == EventType.MouseLeaveWindow)
            {
                Cancel();
            }
        }

        /// <summary>
        /// Handles drawing the selection rect
        /// </summary>
        public override void AfterNodeRender()
        {
            if (!IsActivated)
                return;

            GUIExtensions.ColouredBox(selectionRect, SelectionColor, null);
        }

        /// <summary>
        /// Select all nodes within the selection box
        /// </summary>
        private void SelectNodes()
        {
            if (!CanSelectNodes)
                return;

            IEnumerable<Node> nodesInRect = 
                NodeEditor.GetNodesInRect(selectionRect, AllowOverlaps);

            foreach (Node node in nodesInRect)
            {
                node.Select();
            }
        }

        /// <summary>
        /// Select all connectors within the selection box
        /// </summary>
        private void SelectConnectors()
        {
            if (!CanSelectConnectors)
                return;
            
            IEnumerable<Connector> connectorsInRect =
                NodeEditor.GetConnectorsInRect(selectionRect, AllowOverlaps);

            foreach (Connector connector in connectorsInRect)
            {
                connector.Select();
            }
        }
    }
}