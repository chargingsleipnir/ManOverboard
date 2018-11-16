using System;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Event args for when a node is added
    /// </summary>
    public class NodeAddedEventArgs : EventArgs
    {
        /// <summary>
        /// The node that was added
        /// </summary>
        public Node AddedNode;

        /// <summary>
        /// Constructor
        /// </summary>
        public NodeAddedEventArgs()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="addedNode">The node that was added</param>
        public NodeAddedEventArgs(Node addedNode)
        {
            AddedNode = addedNode;
        }
    }

    /// <summary>
    /// Event args for when a node is removed
    /// </summary>
    public class NodeRemovedEventArgs: EventArgs
    {
        /// <summary>
        /// The node that was removed
        /// </summary>
        public Node RemovedNode;

        /// <summary>
        /// Constructor
        /// </summary>
        public NodeRemovedEventArgs()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="addedNode">The node that was removed</param>
        public NodeRemovedEventArgs(Node removedNode)
        {
            RemovedNode = removedNode;
        }
    }
}