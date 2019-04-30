using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.NodeEditor
{ 
    /// <summary>
    /// A toggleable mode to select pairings of nodes to connect.
    /// Once activated, click on a node to start a connection, click
    /// on another to complete the connection
    /// </summary>
    public class ConnectorMakerMode : ConnectorInputMode
    {
        /// <summary>
        /// The key to look for activation
        /// </summary>
        public KeyCode ActivationKey = KeyCode.T;

        /// <summary>
        /// Any modifiers that are required for activation
        /// </summary>
        public EventModifiers Modifier = EventModifiers.Alt;

        /// <summary>
        /// The first node selected
        /// </summary>
        private Node startNode;

        /// <summary>
        /// The active connection that follows the mouse until a
        /// closing node is selected
        /// </summary>
        private Connector connection;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeEditor">Node editor to work on</param>
        /// <param name="priority">When this mode should be processed relative
        /// to other input modes. Lower number == processed first</param>
        public ConnectorMakerMode(NodeEditor nodeEditor, int priority = 0) 
            : base(nodeEditor, priority)
        {
        }

        /// <summary>
        /// Should activate only if the required key combination is pressed
        /// </summary>
        /// <returns>True if should activate, false if not</returns>
        protected override bool ShouldActivate()
        {
            if (Event.current.type != EventType.KeyDown)
                return false;

            if (ActivationKey == KeyCode.None && Modifier == EventModifiers.None)
                return false;

            return InputKeysPressed();
        }

        /// <summary>
        /// Prepares the node editor for connection mode
        /// </summary>
        protected override void OnActivated()
        {
            NodeEditor.SelectionAllowed = false;
        }

        /// <summary>
        /// Cleans up
        /// </summary>
        protected override void OnDeactivated()
        {
            NodeEditor.SelectionAllowed = true;
            connection = null;
            startNode = null;
        }

        /// <summary>
        /// Aborts the active transition
        /// </summary>
        protected override void OnCancelled()
        {
            if (startNode != null)
            {
                NodeEditor.RemoveNode(startNode);
                NodeEditor.RemoveConnector(connection);
            }
        }

        /// <summary>
        /// Handles checking for clicked nodes
        /// </summary>
        public override void BeforeEditorEvents()
        {
            CheckActivation();

            if (!IsActivated)
                return;

            if (Event.current.IsLeftClickEvent())
            {
                Node selectedNode = NodeEditor.GetNodeUnderMouse();

                if (selectedNode == null || !IsNodeValid(selectedNode))
                    return;

                MakeConnection(selectedNode);
            }
            else if (Event.current.IsRightClickEvent())
            {
                if (startNode != null)
                {
                    NodeEditor.RemoveConnector(connection);
                    startNode = null;
                    connection = null;
                }
            }
        }

        /// <summary>
        /// Handles making the connection, whether it's starting a new
        /// one or completing the existing one
        /// </summary>
        /// <param name="selectedNode">The node to react to</param>
        private void MakeConnection(Node selectedNode)
        {
            if (startNode == null)
            {
                startNode = selectedNode;
                connection = ConnectorFactory(NodeEditor, startNode, NodeEditor.MouseNode);
                NodeEditor.AddConnector(connection);
                NodeEditor.AddNode(startNode);
            }
            else
            {
                connection.RemoveNode(NodeEditor.MouseNode);
                connection.AddNode(selectedNode);
                startNode = null;
                connection = null;
            }
        }

        /// <summary>
        /// Helper to check if the keys are pressed
        /// </summary>
        /// <returns>True if the keys are pressed, false if not</returns>
        private bool InputKeysPressed()
        {
            bool keyPressed = ActivationKey == KeyCode.None ||
                Event.current.keyCode == ActivationKey;

            bool modifierActive = Modifier == EventModifiers.None ||
                Event.current.modifiers == Modifier;

            return keyPressed && modifierActive;

        }
    }
}