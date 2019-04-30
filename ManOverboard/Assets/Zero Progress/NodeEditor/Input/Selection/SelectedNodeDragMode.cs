using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Handles the behaviour for dragging nodes
    /// </summary>
    public class SelectedNodeDragMode : InputMode
    {
        /// <summary>
        /// Indicates if the mouse down event was performed
        /// inside a selected node
        /// </summary>
        private bool wasMouseDownInSelectedNode = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeEditor">Node editor to work on</param>
        /// <param name="priority">When this mode should be processed relative
        /// to other input modes. Lower number == processed first</param>
        public SelectedNodeDragMode(NodeEditor nodeEditor, int priority = 0) 
            : base(nodeEditor, priority)
        {
        }

        /// <summary>
        /// Activates if a selected node was clicked and if
        /// there are currently nodes selected
        /// </summary>
        /// <returns></returns>
        protected override bool ShouldActivate()
        {
            return NodeEditor.GetSelectedNodesCount() > 0 && wasMouseDownInSelectedNode;
        }

        /// <summary>
        /// Performs the drags before the other editor events
        /// 
        /// This should be called after any node select modes
        /// </summary>
        public override void BeforeEditorEvents()
        {
            if (Event.current.button != 0)
                return;

            if(NodeEditor.GetPreUsedEventType() == EventType.MouseDown)
            {
                Node node = NodeEditor.GetNodeUnderMouse();

                wasMouseDownInSelectedNode = node != null && node.IsSelected();

                CheckActivation();
            }

            if (!IsActivated)
                return;

            if (Event.current.type == EventType.MouseDrag)
                HandleMouseMove();
            else if (Event.current.type == EventType.MouseUp ||
                Event.current.type == EventType.MouseLeaveWindow)
                Deactivate();
        }
        
        /// <summary>
        /// Handles sending the motion data to the selected nodes
        /// </summary>
        private void HandleMouseMove()
        {
            IEnumerable<Node> selectedNodes = NodeEditor.GetSelectedNodes();

            int used = 0;
            int count = 0;

            Vector2 zoomedDrag = NodeEditor.ApplyZoomToVector(Event.current.delta);

            foreach (Node node in selectedNodes)
            {
                if (node.HandleDrag(zoomedDrag))
                    used++;
                count++;
            }

            if (used > 0)
            {
                NodeEditor.FlagRepaint();
                Event.current.Use();
            }
            else if (count == 0)
                Deactivate();
        }
    }
}