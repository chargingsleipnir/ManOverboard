using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Node editor input for moving the pan offset to focus
    /// on selection or the origin of the editor
    /// </summary>
    public class FocusInput : InputMode
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeEditor">Node editor to work on</param>
        /// <param name="priority">When this mode should be processed relative
        /// to other input modes. Lower number == processed first</param>
        public FocusInput(NodeEditor nodeEditor, int priority = 2) 
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
        /// Check if focus key is pressed
        /// </summary>
        public override void BeforeEditorEvents()
        {
            CheckActivation();

            if (!IsActivated)
                return;

            if (Event.current.type != EventType.KeyDown)
                return;

            if (Event.current.keyCode == KeyCode.F)
            {
                PerformFocus();
                Event.current.Use();
            }
        }

        /// <summary>
        /// Perform the focusing logic
        /// </summary>
        private void PerformFocus()
        {
            IEnumerable<Node> selectedNodes = NodeEditor.GetSelectedNodes();

            int count = 0;
            Vector2 averagePosition = Vector2.zero;

            foreach (Node node in selectedNodes)
            {
                averagePosition += node.NodeRect.position;
                count++;
            }

            IEnumerable<Connector> selectedConnectors = NodeEditor.GetSelectedConnectors();

            foreach (Connector connector in selectedConnectors)
            {
                Node startNode = connector.GetStartNode();
                Node endNode = connector.GetEndNode();

                averagePosition += ((startNode.NodeRect.position + endNode.NodeRect.position) * 0.5f);
                count++;
            }

            if (count == 0)
            {
                foreach (Node node in NodeEditor.Nodes)
                {
                    averagePosition += node.NodeRect.position;
                    count++;
                }
            }

            if (count == 0)
            {
                NodeEditor.SetPanOffset(Vector2.zero);
                return;
            }

            Vector2 offset = NodeEditor.NodeEditorRect.size * 0.5f;

            NodeEditor.SetPanOffset((-averagePosition / count) + offset);
        }
    }
}