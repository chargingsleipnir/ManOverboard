using UnityEngine;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// A node that doesn't apply the offset to the rect
    /// </summary>
    public class NoPanNode : Node
    {
        public override Rect GetNodeRect(Vector2 offset)
        {
            return base.GetNodeRect(Vector2.zero);
        }
    }
}