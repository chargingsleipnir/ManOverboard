using System;
using System.Collections.Generic;
using ZeroProgress.Common;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Base class for input modes that are associated with
    /// making connections between nodes
    /// </summary>
    public abstract class ConnectorInputMode : InputMode
    {
        public delegate bool NodeValidator(Node node);

        public delegate bool ConnectionValidator(Node startNode, Node endNode);

        public delegate Connector ConnectorFactoryDelegate(NodeEditor editor, params Node[] nodes);

        /// <summary>
        /// Callback used to validate that the node can be used
        /// for a connection
        /// </summary>
        public NodeValidator IsNodeValidCallback = null;

        /// <summary>
        /// Callback used to validate that a connection between the two nodes can be made
        /// </summary>
        public ConnectionValidator IsConnectionValidCallback = null;

        /// <summary>
        /// Event for when new connections are made
        /// </summary>
        public event EventHandler<EventArgs<IEnumerable<Connector>>> OnConnectionsFinalized;

        /// <summary>
        /// The factory method that creates the new connector on execution
        /// </summary>
        public ConnectorFactoryDelegate ConnectorFactory = DefaultConnectorFactoryFunction;

        /// <summary>
        /// Creates a Connector object
        /// </summary>
        /// <param name="startNode">The starting node that the connector was opened on</param>
        /// <returns>An empty Connector</returns>
        public static Connector DefaultConnectorFactoryFunction(NodeEditor editor, params Node[] nodes)
        {
            return new Connector(nodes);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeEditor">Node editor to work on</param>
        /// <param name="priority">When this mode should be processed relative
        /// to other input modes. Lower number == processed first</param>
        public ConnectorInputMode(NodeEditor nodeEditor, int priority = 0) 
            : base(nodeEditor, priority)
        {
        }

        /// <summary>
        /// Determines if the provided node is valid
        /// </summary>
        /// <param name="node">The node to validate</param>
        /// <returns>True if valid, false if not</returns>
        public virtual bool IsNodeValid(Node node)
        {
            if (node == null)
                return false;

            if (IsNodeValidCallback == null)
                return true;

            return IsNodeValidCallback(node);
        }

        /// <summary>
        /// Determines if the provided connection can be made
        /// </summary>
        /// <param name="startNode">The start node for the connection</param>
        /// <param name="endNode">The end node for the connectio</param>
        /// <returns>True if valid, false if not</returns>
        public virtual bool IsConnectionValid(Node startNode, Node endNode)
        {
            if (IsConnectionValidCallback == null)
                return true;

            return IsConnectionValidCallback(startNode, endNode);
        }
        
        /// <summary>
        /// Raises the event that notifies new connections have been finalized
        /// </summary>
        /// <param name="connectors"></param>
        protected void RaiseOnConnectionsFinalized(IEnumerable<Connector> connectors)
        {
            EventArgs<IEnumerable<Connector>> eventArgs = 
                new EventArgs<IEnumerable<Connector>>(connectors);

            OnConnectionsFinalized.SafeInvoke(this, eventArgs);
        }
    }
}