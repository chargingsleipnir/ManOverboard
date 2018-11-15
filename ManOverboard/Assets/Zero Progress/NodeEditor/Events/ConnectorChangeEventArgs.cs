using System;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Event args for when a Connector is added
    /// </summary>
    public class ConnectorAddedEventArgs : EventArgs
    {
        /// <summary>
        /// The Connector that was added
        /// </summary>
        public Connector AddedConnector;

        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectorAddedEventArgs()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="addedConnector">The Connector that was added</param>
        public ConnectorAddedEventArgs(Connector addedConnector)
        {
            AddedConnector = addedConnector;
        }
    }

    /// <summary>
    /// Event args for when a Connector is removed
    /// </summary>
    public class ConnectorRemovedEventArgs: EventArgs
    {
        /// <summary>
        /// The Connector that was removed
        /// </summary>
        public Connector RemovedConnector;

        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectorRemovedEventArgs()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="addedConnector">The Connector that was removed</param>
        public ConnectorRemovedEventArgs(Connector removedConnector)
        {
            RemovedConnector = removedConnector;
        }
    }
}