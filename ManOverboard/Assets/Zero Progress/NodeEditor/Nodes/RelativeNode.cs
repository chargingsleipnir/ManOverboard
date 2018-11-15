using UnityEngine;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// A node that receives its positioning from another node. Meant
    /// to be used for intermediary connection points
    /// </summary>
    public class RelativeNode : Node
    {
        /// <summary>
        /// The node to be positioned in relation to
        /// </summary>
        [HideInInspector]
        public Node RelativeTo;

        /// <summary>
        /// Offset from the relative node
        /// </summary>
        [SerializeField]
        public Vector2 Offset;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="relativeTo">The node to be positioned in relation to</param>
        /// <param name="offset">Offset from the relative node</param>
        public RelativeNode(Node relativeTo, Vector2 offset)
        {
            RelativeTo = relativeTo;
            Offset = offset;
            IsSelectable = false;
        }

        public override Rect GetNodeRect(Vector2 panOffset)
        {
            Rect nodeRect = RelativeTo.GetNodeRect(panOffset);

            nodeRect.position += Offset;

            return nodeRect;
        }

    }
}