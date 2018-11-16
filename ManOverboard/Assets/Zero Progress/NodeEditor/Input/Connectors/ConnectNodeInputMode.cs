using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;
using System.Linq;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Used usually as a context menu response for making a connection
    /// between one or more nodes
    /// </summary>
    public class ConnectNodeInputMode : ConnectorMakerMode
    {
        /// <summary>
        /// Possible options for how a connection reacts to 
        /// multiple selected nodes
        /// </summary>
        public enum ConnectionStyle
        {
            FIRST_SELECTED,
            LAST_SELECTED,
            MULTIPLE
        }

        /// <summary>
        /// How to select nodes for connections if multiple are selected
        /// </summary>
        public ConnectionStyle ConnectionLogic { get; set; }

        /// <summary>
        /// Active connections being formed (stored here so we can remove
        /// them from the editor if this operation is cancelled)
        /// </summary>
        private List<Connector> newConnections = new List<Connector>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeEditor">Node editor to work on</param>
        /// <param name="priority">When this mode should be processed relative
        /// to other input modes. Lower number == processed first</param>
        public ConnectNodeInputMode(NodeEditor nodeEditor, int priority = 0) 
            : base(nodeEditor, priority)
        {
            ConnectionLogic = ConnectionStyle.MULTIPLE;
        }

        /// <summary>
        /// No conditions for activation
        /// </summary>
        /// <returns>True</returns>
        protected override bool ShouldActivate()
        {
            return true;
        }

        /// <summary>
        /// Creates the necessary connections when first activated
        /// </summary>
        protected override void OnActivated()
        {
            newConnections.Clear();

            InitializeConnections();
            
            if (newConnections.Count == 0)
                Deactivate();
            else
                NodeEditor.SelectionAllowed = false;
        }

        /// <summary>
        /// Handles removing the created connections when the operation is cancelled
        /// </summary>
        protected override void OnCancelled()
        {
            foreach (Connector connector in newConnections)
            {
                NodeEditor.RemoveConnector(connector);
            }
        }

        /// <summary>
        /// Handles clean up on deactivation
        /// </summary>
        protected override void OnDeactivated()
        {
            newConnections.Clear();

            NodeEditor.SelectionAllowed = true;
        }

        /// <summary>
        /// Checks for a destination node to attach the active connections to
        /// </summary>
        public override void BeforeEditorEvents()
        {
            if (!IsActivated)
                return;

            if (Event.current.IsRightClickEvent())
            {
                Cancel();
                return;
            }

            NodeEditor.FlagRepaint();

            if (!Event.current.IsLeftClickEvent())
                return;

            Node selectedNode = NodeEditor.GetNodeUnderMouse();

            if (!IsNodeValid(selectedNode))
                return;

            for (int i = 0; i < newConnections.Count; i++)
            {
                Connector connector = newConnections[i];

                if (!IsConnectionValid(connector.GetStartNode(), selectedNode))
                {
                    NodeEditor.RemoveConnector(connector);
                    newConnections.RemoveAt(i);
                    continue;
                }

                connector.RemoveNode(NodeEditor.MouseNode);
                connector.AddNode(selectedNode);
            }

            if (newConnections.Count > 0)
                RaiseOnConnectionsFinalized(newConnections);

            Deactivate();
        }

        /// <summary>
        /// Initializes the connections based on the selected connection logic
        /// </summary>
        private void InitializeConnections()
        {
            IEnumerable<Node> selectedNodes = NodeEditor.GetSelectedNodes();

            switch (ConnectionLogic)
            {
                case ConnectionStyle.FIRST_SELECTED:
                    selectedNodes = new Node[] { selectedNodes.FirstOrDefault() };
                    break;
                case ConnectionStyle.LAST_SELECTED:
                    selectedNodes = new Node[] { selectedNodes.LastOrDefault() };
                    break;
            }
            
            foreach (Node node in selectedNodes)
            {
                if (node == null)
                    continue;

                Connector connector = ConnectorFactory(NodeEditor, node, NodeEditor.MouseNode);
                NodeEditor.AddConnector(connector);
                newConnections.Add(connector);
            }
        }
    }
}