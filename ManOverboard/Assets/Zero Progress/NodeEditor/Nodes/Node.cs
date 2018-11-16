using System;
using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// The base class for nodes
    /// </summary>
    [Serializable]
    public class Node : ISelectable
    {
        /// <summary>
        /// True if the node is selectable, false if not
        /// </summary>
        public bool IsSelectable = true;
        
        /// <summary>
        /// Data associated with this node
        /// </summary>
        public System.Object NodeData = null;

        /// <summary>
        /// The rect that defines the nodes area
        /// </summary>
        public Rect NodeRect;

        /// <summary>
        /// Context menu for the node
        /// </summary>
        public GUIGenericMenu ContextMenu;

        /// <summary>
        /// True if this node has a context menu, false if not
        /// </summary>
        public bool HasContextMenu
        {
            get { return ContextMenu != null; }
        }

        /// <summary>
        /// True if selected, false if not
        /// </summary>
        [SerializeField]
        private bool isSelected = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public Node()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">The position of the node</param>
        /// <param name="nodeData">Custom data assigned to the node</param>
        /// <param name="menu">The context menu for right clicks</param>
        public Node(Vector2 position, System.Object nodeData = null, 
            GUIGenericMenu menu = null)
            : this(new Rect(position, Vector2.one), nodeData, menu)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeRect">The rectangle of the node</param>
        /// <param name="nodeData">Custom data assigned to the node</param>
        /// <param name="menu">The context menu for right clicks</param>
        public Node(Rect nodeRect, System.Object nodeData = null, 
            GUIGenericMenu menu = null)
        {
            NodeRect = nodeRect;
            NodeData = nodeData;
            ContextMenu = menu;
        }
        
        /// <summary>
        /// Gets the rectangle that defines the nodes area at the
        /// provided pan offset
        /// </summary>
        /// <param name="offset">Node editors' pan offset</param>
        /// <returns>The rectangle of the node at the provided offset</returns>
        public virtual Rect GetNodeRect(Vector2 offset)
        {
            return NodeRect.AddPosition(offset);
        }
        
        /// <summary>
        /// Handles rendering the node
        /// </summary>
        /// <param name="nodeOffset">Node Editors' Pan offset</param>
        public virtual void Draw(Vector2 nodeOffset)
        {     
        }

        /// <summary>
        /// Handles dragging the node
        /// </summary>
        /// <param name="delta">The amount to drag</param>
        /// <returns>True if dragged, false if not</returns>
        public virtual bool HandleDrag(Vector2 delta)
        {
            if (isSelected)
                NodeRect.position = NodeRect.position + delta;

            return isSelected;
        }

        /// <summary>
        /// Handles any non-repaint/layout events
        /// </summary>
        /// <param name="nodeEditor">The editor to work with</param>
        /// <param name="nodeOffset">The offset of the node</param>
        /// <returns>True if require repaint, false if not</returns>
        public virtual bool HandleEvents(NodeEditor nodeEditor, Vector2 nodeOffset)
        {
            return false;
        }

        /// <summary>
        /// Gets the style for the node
        /// </summary>
        /// <returns>The style from the skin, or a created skyle</returns>
        protected virtual GUIStyle GetNodeStyle()
        {
            GUIStyle nodeStyle = GUI.skin.FindStyle("SimpleNode");

            if (nodeStyle == null)
            {
                nodeStyle = new GUIStyle(GUI.skin.box);
                nodeStyle.normal.background = Resources.Load("ZeroProgress/NodeEditor/node-normal") as Texture2D;
                nodeStyle.border = new RectOffset(12, 12, 12, 12);
                nodeStyle.fontSize = 24;
                nodeStyle.padding = new RectOffset(5, 5, 5, 5);
                nodeStyle.name = "SimpleNode";
            }

            return nodeStyle;
        }

        #region Selectable Interface Implementation

        public virtual bool CanSelect()
        {
            return IsSelectable;
        }

        public virtual bool IsSelected()
        {
            return isSelected;
        }

        public virtual void Select()
        {
            if (!CanSelect())
                return;

            isSelected = true;
        }

        public virtual void ToggleSelected()
        {
            if (!CanSelect())
                return;

            isSelected = !isSelected;
        }

        public virtual void UnSelect()
        {
            if (!CanSelect())
                return;

            isSelected = false;
        }

        #endregion
    }
}