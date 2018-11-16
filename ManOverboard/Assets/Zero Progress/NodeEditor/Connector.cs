using System;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// A line that connects two or more nodes together
    /// </summary>
    public class Connector : ISelectable
    {
        /// <summary>
        /// True to allow a connection to the same node
        /// </summary>
        public bool AllowDuplicateNodes = true;

        /// <summary>
        /// True if the connection can be selected, false if not
        /// </summary>
        public bool IsSelectable = true;

        /// <summary>
        /// Any context menu to be associated with this connector
        /// </summary>
        public GUIGenericMenu ContextMenu;

        /// <summary>
        /// Checks if there is a context menu associated with this connector
        /// </summary>
        public bool HasContextMenu
        {
            get { return ContextMenu != null; }
        }

        /// <summary>
        /// The nodes that this connector connects
        /// </summary>
        protected List<Node> nodes = new List<Node>();

        /// <summary>
        /// True if selected false if not
        /// </summary>
        private bool isSelected = false;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Connector()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectedNodes">List of nodes to assign to this connection</param>
        public Connector(params Node[] connectedNodes)
            : this(null, connectedNodes)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="menu">Context menu for right clicking</param>
        /// <param name="connectedNodes">List of nodes to assign to this connection</param>
        public Connector(GUIGenericMenu menu, params Node[] connectedNodes)
        {
            ContextMenu = menu;

            foreach (Node node in connectedNodes)
            {
                AddNode(node);
            }
        }

        /// <summary>
        /// Adds a node to the end of this connector
        /// </summary>
        /// <param name="nodeToAdd">The node to be added</param>
        public virtual void AddNode(Node nodeToAdd)
        {
            if (nodeToAdd == null)
                throw new ArgumentNullException("Node to add cannot be null");

            if (AllowDuplicateNodes)
                nodes.Add(nodeToAdd);
            else
                nodes.AddUnique(nodeToAdd);
        }

        /// <summary>
        /// Removes a node
        /// </summary>
        /// <param name="nodeToRemove">The node to be removed</param>
        public virtual void RemoveNode(Node nodeToRemove)
        {
            nodes.Remove(nodeToRemove);
        }

        /// <summary>
        /// Remove all instances of the provided node
        /// (only necessary if duplicate nodes are allowed)
        /// </summary>
        /// <param name="nodeToRemove">The node to remove all instances of</param>
        public virtual void RemoveAllNodeInstances(Node nodeToRemove)
        {
            nodes.RemoveAll((x) => x == nodeToRemove);
        }

        /// <summary>
        /// Determines if the provided point is within the connector
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <param name="margin">The margin to apply around the point</param>
        /// <param name="offset">The node editors offset</param>
        /// <returns>True if the point is found within the connector, false if not</returns>
        public virtual bool ContainsPoint(Vector2 point, Vector2 margin, Vector2 offset)
        {
            bool contains = false;
            
            Rect pointRect = new Rect(point - margin, margin);

            ForEachConnection(offset, (node1, node1Rect, node2, node2Rect) =>
            {
                if (pointRect.LineRectIntersection(node1Rect.center, node2Rect.center))
                {
                    contains = true;
                    return false;
                }

                return true;
            });

            return contains;
        }

        /// <summary>
        /// Determines if the connector is contained within the provided rect
        /// </summary>
        /// <param name="queryRect">The rectangle used to check connector containment</param>
        /// <param name="offset">Node editors' pan offset</param>
        /// <param name="allowOverlap">True to pass if only overlapping, false to require
        /// full containment</param>
        /// <returns>True if contained, false if not</returns>
        public virtual bool ContainedByRect(Rect queryRect, Vector2 offset, bool allowOverlap = false)
        {
            bool contains = true;
            
            ForEachConnection(offset, (node1, node1Rect, node2, node2Rect) =>
            {
                if (allowOverlap)
                {
                    // if overlap is allowed and currently overlapping, skip the contains
                    // check, because we've passed
                    if (queryRect.LineRectIntersection(node1Rect.center, node2Rect.center))
                        return true;
                }

                // If the start or end point are not within the query rectangle
                // then containment check is failed
                if (!queryRect.Contains(node1Rect.center) || 
                    !queryRect.Contains(node2Rect.center))
                {
                    contains = false;
                    return false;
                }
                
                return true;
            });

            return contains;
        }

        /// <summary>
        /// Renders the connector
        /// </summary>
        /// <param name="offset">Node editor pan offset</param>
        public virtual void Draw(Vector2 offset)
        {
            ForEachConnection(offset, (node1, node1Rect, node2, Node2Rect) =>
            {
                Color color = Color.white;

                if (IsSelected())
                    color = NodeEditor.DefaultSelectionColor;

                GLExtensions.DrawGUILineZDepthOff(node1Rect.center, Node2Rect.center, color);

                return true;
            });
        }

        /// <summary>
        /// Gets all the nodes connected by this connector as a readonly collection
        /// </summary>
        /// <returns>Readonly collection of nodes</returns>
        public IEnumerable<Node> GetNodes()
        {
            return nodes.AsReadOnly();
        }

        /// <summary>
        /// The first node in the list
        /// </summary>
        /// <returns>The first node, or null</returns>
        public Node GetStartNode()
        {
            if (nodes.Count == 0)
                return null;

            return nodes[0];
        }

        /// <summary>
        /// The last node in the list
        /// </summary>
        /// <returns>The last node, or null</returns>
        public Node GetEndNode()
        {
            if (nodes.Count == 0)
                return null;

            return nodes[nodes.Count - 1];
        }

        /// <summary>
        /// Helper for children to iterate between the connections in each node
        /// </summary>
        /// <param name="offset">The panning offset of the Node Editor</param>
        /// <param name="callback">The callback that provides the start node, end node, 
        /// and each nodes rectangle for easier manipulation</param>
        protected void ForEachConnection(Vector2 offset, Func<Node, Rect, Node, Rect, bool> callback)
        {
            if (nodes.Count < 2)
                return;

            for (int i = 1; i < nodes.Count; i++)
            {
                Node current = nodes[i - 1];
                Node next = nodes[i];

                Rect currentRect = current.GetNodeRect(offset);
                Rect nextRect = next.GetNodeRect(offset);

                if (!callback(current, currentRect, next, nextRect))
                    return;
            }
        }

        #region ISelectable Implementation

        /// <summary>
        /// Determines if this is a selectable connector
        /// </summary>
        /// <returns>True if selectable, false if not</returns>
        public virtual bool CanSelect()
        {
            return IsSelectable;
        }

        /// <summary>
        /// Is this connector currently selected
        /// </summary>
        /// <returns>True if selected, false if not</returns>
        public virtual bool IsSelected()
        {
            return isSelected;
        }

        /// <summary>
        /// Select this connector if it's selectable
        /// </summary>
        public virtual void Select()
        {
            if (CanSelect())
                isSelected = true;
        }

        /// <summary>
        /// Unselect this connector if it's selectable
        /// </summary>
        public virtual void UnSelect()
        {
            if (CanSelect())
                isSelected = false;
        }

        /// <summary>
        /// Toggle current selection state if it's selectable
        /// </summary>
        public virtual void ToggleSelected()
        {
            if (CanSelect())
                isSelected = !isSelected;
        }

        #endregion
    }
}