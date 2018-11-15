using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.NodeEditor;

namespace ZeroProgress.SceneManagementUtility.Editors
{
    public class SceneManagerNodeEditor
    {
        private NodeEditor.NodeEditor nodeEditor = new NodeEditor.NodeEditor();

        public NodeEditor.NodeEditor NodeEditor
        {
            get { return nodeEditor; }
            private set { nodeEditor = value; }
        }
        
        public SerializedSceneManager SerializedManager { get; private set; }

        private GUIGenericMenu nodeContextMenu;
        private GUIGenericMenu connectorContextMenu;

        private Rect nodeWindowNonZero;

        private bool isTransitioning = false;

        private SceneManagerEditorWindow editorWindow;
        
        public void Initialize(SerializedSceneManager serializedManager,
            SceneManagerEditorWindow editorWindow)
        {
            SerializedManager = serializedManager;
            this.editorWindow = editorWindow;

            NodeEditor.ClearAll();
            NodeEditor.ClearInputModes();

            NodeEditor.CanDeleteNode = CanDeleteNode;

            NodeEditor.OnNodeRemoved += NodeEditor_OnNodeRemoved;
            NodeEditor.OnConnectorRemoved += NodeEditor_OnConnectorRemoved;

            NodeEditor.OnSelectionChanged += NodeEditor_OnSelectionChanged;

            new DefaultInputModeConfig().ApplyToNodeEditor(NodeEditor);

            ConnectNodeInputMode nodeConnectorMode = NodeEditor.GetInputMode<ConnectNodeInputMode>();

            nodeConnectorMode.ConnectorFactory = TransitionConnector.TransitionConnectorFactory;
            nodeConnectorMode.OnConnectionsFinalized += NodeConnectorMode_OnConnectionsFinalized;

            InitializeContextMenus();

            PopulateNodes(nodeContextMenu);
            PopulateConnections(connectorContextMenu);

            NodeEditor.RaiseSelectionChanged();
            NodeEditor.SetPanOffset(SerializedManager.SceneNodes.PanOffset);
        }

        public void OnGUI()
        {
            Rect nodeWindow = EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));

            if (Event.current.type == EventType.Repaint)
                nodeWindowNonZero = nodeWindow;

            // Offset mouse position by the size of the sidebar
            // Can't use grouping due to the way zooming is handled
            Vector2 eventPos = Event.current.mousePosition;
            Event.current.mousePosition -= nodeWindow.position;

            if (NodeEditor.OnGUI(nodeWindowNonZero))
            {
                editorWindow.Repaint();
                SerializedManager.SceneNodesObject.Update();
            }

            Event.current.mousePosition = eventPos;

            EditorGUILayout.EndVertical();

            if (isTransitioning)
                editorWindow.Repaint();
        }

        /// <summary>
        /// Calculates where the new node position should be based on current mouse position
        /// </summary>
        /// <returns>The position to add the new node to</returns>
        public Vector2 GetNewNodePosition()
        {
            return NodeEditor.ApplyZoomToVector(Event.current.mousePosition -
                    NodeEditor.NodeEditorRect.position) - NodeEditor.GetPanOffset();
        }

        public void UpdateActiveTransition(SceneModel initialScene, SceneModel destScene)
        {
            if (initialScene == null)
            {
                isTransitioning = false;
                return;
            }

            TransitionConnector anyConnector = null;

            bool foundMatch = false;

            foreach (Connector connector in NodeEditor.Connectors)
            {
                TransitionConnector transConnect = connector as TransitionConnector;

                if (transConnect == null)
                    continue;

                if (destScene == null)
                    transConnect.IsCurrentlyTransitioning = false;
                else
                {
                    SceneNode startNode = transConnect.GetStartNode() as SceneNode;
                    SceneNode endNode = transConnect.GetEndNode() as SceneNode;

                    if (startNode == null || endNode == null)
                        continue;

                    if (startNode.SceneId == SceneManagerController.ANY_SCENE_ID &&
                        endNode.SceneId == destScene.SceneId)
                        anyConnector = transConnect;

                    transConnect.IsCurrentlyTransitioning =
                        (startNode.SceneId == initialScene.SceneId &&
                        endNode.SceneId == destScene.SceneId);

                    if (transConnect.IsCurrentlyTransitioning)
                    {
                        foundMatch = true;
                        break;
                    }
                }
            }

            if (foundMatch)
                isTransitioning = true;
            else if (anyConnector != null)
            {
                anyConnector.IsCurrentlyTransitioning = true;
                isTransitioning = true;
            }
            else
                isTransitioning = false;
        }

        private void NodeConnectorMode_OnConnectionsFinalized(object sender, Common.EventArgs<IEnumerable<Connector>> e)
        {
            foreach (Connector connector in e.Value)
            {
                TransitionConnector current = connector as TransitionConnector;

                if (current == null)
                    continue;

                SceneNode startNode = connector.GetStartNode() as SceneNode;
                SceneNode endNode = connector.GetEndNode() as SceneNode;

                TransitionConnector existingOut = null, existingIn = null;

                // Iterate existing connectors to determine if there are any duplicates
                // and if there are, increment the display count
                foreach (Connector item in NodeEditor.Connectors)
                {
                    // Looking for other connections that are made between the same nodes
                    // so need to skip if we are on the connector being finalized
                    if (item == connector)
                        continue;

                    TransitionConnector existing = item as TransitionConnector;

                    if (existing == null)
                        continue;

                    bool isOutbound = existing.GetStartNode().NodeData == startNode.NodeData &&
                        existing.GetEndNode().NodeData == endNode.NodeData;

                    bool isInbound = existing.GetStartNode().NodeData == endNode.NodeData &&
                        existing.GetEndNode().NodeData == startNode.NodeData;

                    if (isOutbound)
                        existingOut = existing;

                    if (isInbound)
                        existingIn = existing;
                }

                bool isSelfTransition = endNode.SceneId == startNode.SceneId;

                if (existingOut != null)
                {
                    existingOut.ConnectionCount++;
                    NodeEditor.RemoveConnector(connector, notifyListeners: false);
                }

                if (existingIn != null && !isSelfTransition)
                    current.ShiftOver = existingIn.ShiftOver = true;

                if (existingIn != null && existingOut != null && !isSelfTransition)
                    existingIn.ShiftOver = existingOut.ShiftOver = current.ShiftOver = true;

                if (isSelfTransition)
                {
                    Node lastNode = connector.GetEndNode();
                    connector.RemoveNode(lastNode);
                    AddSelfTransitionNodes(current, lastNode);
                    connector.AddNode(lastNode);
                }

                Undo.RecordObject(SerializedManager.TargetManager, "Undo Create Transition");
                startNode.SceneInfo.CreateNewTransition(endNode.SceneId);
                SerializedManager.SerializedManager.Update();
            }
        }

        private void NodeEditor_OnSelectionChanged(object sender, System.EventArgs e)
        {
            int selectedNodesCount = NodeEditor.GetSelectedNodesCount();
            int selectedConnectorsCount = NodeEditor.GetSelectedConnectorsCount();

            if (selectedNodesCount > 0 || selectedConnectorsCount > 0)
            {
                if (Selection.activeObject != SerializedManager.SceneNodes)
                    Selection.activeObject = SerializedManager.SceneNodes;
            }
        }

        private void NodeEditor_OnConnectorRemoved(object sender, ConnectorRemovedEventArgs e)
        {
            TransitionConnector transition = e.RemovedConnector as TransitionConnector;

            if (transition == null)
                return;

            SceneNode startNode = transition.GetStartNode() as SceneNode;
            SceneNode endNode = transition.GetEndNode() as SceneNode;

            Undo.RegisterCompleteObjectUndo(SerializedManager.TargetManager, "Remove Connector");

            if (endNode != null)
                startNode.SceneInfo.Transitions.RemoveAll(
                    (x) => x.DestinationSceneId == endNode.SceneId);

            SerializedManager.SerializedManager.Update();
            editorWindow.FlagRefresh();
        }

        private void NodeEditor_OnNodeRemoved(object sender, NodeRemovedEventArgs e)
        {
            SceneNode sceneNode = e.RemovedNode as SceneNode;

            if (sceneNode == null)
                return;

            SerializedManager.DeleteSceneNode(sceneNode.SceneId, false);
            
            SerializedManager.SerializedManager.Update();
            editorWindow.FlagRefresh();
        }

        /// <summary>
        /// Filter to determine deletable nodes
        /// </summary>
        /// <param name="node">The node to evaluate</param>
        /// <returns>True if deletable, false if not</returns>
        private bool CanDeleteNode(Node node)
        {
            SceneNode sceneNode = node as SceneNode;

            if (sceneNode == null)
                return true;

            if (sceneNode.SceneInfo == SerializedManager.TargetManager.AnyScene)
                return false;

            return true;
        }

        private void InitializeContextMenus()
        {
            ConnectNodeInputMode nodeConnector = NodeEditor.GetInputMode<ConnectNodeInputMode>();

            nodeContextMenu = new GUIGenericMenu();

            if (EditorApplication.isPlaying)
            {
                nodeContextMenu.AddMenuItem(string.Empty, "Load Scene", LoadScene);
                nodeContextMenu.AddSeparator();
            }

            nodeContextMenu.AddMenuItem(string.Empty,
                "Make Transition", nodeConnector.Activate);
            nodeContextMenu.AddMenuItem(string.Empty, "Set as Entry", SetAsEntryNode);
            nodeContextMenu.AddSeparator();
            nodeContextMenu.AddMenuItem(string.Empty, "Delete", DeleteEditorComponent, false);
            nodeContextMenu.AddMenuItem(string.Empty, "Delete and Exclude Scene", DeleteEditorComponent, true);

            connectorContextMenu = new GUIGenericMenu();
            connectorContextMenu.AddMenuItem(string.Empty, "Delete", DeleteEditorComponent);
        }

        private void LoadScene(GUIMenuItemParamEventArgs args)
        {
            SceneNode node = args.MenuContext as SceneNode;

            if (node == null)
                return;

            if (node.SceneId == SceneManagerController.ANY_SCENE_ID)
                return;

            SerializedManager.TargetManager.TransitionToScene(node.SceneInfo);
        }

        private void SetAsEntryNode(GUIMenuItemEventArgs args)
        {
            SceneNode node = args.MenuContext as SceneNode;

            if (node == null)
            {
                Debug.LogError("Menu Context is not a scene node, cannot set as entry");
                return;
            }

            SerializedManager.SetEntryScene(node.SceneId);
        }

        private void DeleteEditorComponent(GUIMenuItemParamEventArgs args)
        {
            bool addToExclusion = false;
            if (args.ParamValue is bool)
                addToExclusion = (bool)args.ParamValue;

            IEnumerable<Node> selectedNodes = NodeEditor.GetSelectedNodes();

            foreach (Node node in selectedNodes)
            {
                SceneNode sceneNode = node as SceneNode;

                if (sceneNode == null)
                    continue;

                SerializedManager.DeleteSceneNode(
                    sceneNode.SceneId, addToExclusion);
            }

            NodeEditor.RemoveSelectedNodes();
            NodeEditor.RemoveSelectedConnectors();

            editorWindow.Repaint();
        }

        private void RemoveAssociatedConnectors(int sceneId)
        {
            for (int i = NodeEditor.Connectors.Count - 1; i >= 0; i--)
            {
                TransitionConnector transConnect = NodeEditor.Connectors[i] as TransitionConnector;

                if (transConnect == null)
                    continue;

                SceneNode startNode = transConnect.GetStartNode() as SceneNode;
                SceneNode endNode = transConnect.GetEndNode() as SceneNode;

                if (startNode.SceneId == sceneId ||
                    endNode.SceneId == sceneId)
                    NodeEditor.RemoveConnector(transConnect);
            }
        }

        private void AddSelfTransitionNodes(TransitionConnector connector, Node node)
        {
            Vector2 offset = Vector2.zero;
            offset.y = -20f;

            connector.AddNode(new RelativeNode(node, offset));

            offset.x = -node.NodeRect.width * 0.5f + 20f;
            connector.AddNode(new RelativeNode(node, offset));

            offset.x = -(node.NodeRect.width * 0.5f + 10f);

            connector.AddNode(new RelativeNode(node, offset));

            offset.y = 0f;
            connector.AddNode(new RelativeNode(node, offset));
        }

        /// <summary>
        /// Fills the node editor with nodes that represent the scene data
        /// </summary>
        /// <param name="nodeContextMenu">The context menu to be given to each node</param>
        private void PopulateNodes(GUIGenericMenu nodeContextMenu)
        {
            SceneNodeCollection nodes = SerializedManager.SceneNodes;

            foreach (SceneNode node in nodes.SceneNodes)
            {
                node.ContextMenu = nodeContextMenu;

                node.IsEntryScene = SerializedManager.IsEntryScene(node.SceneId);

                if (node.SceneId == SceneManagerController.ANY_SCENE_ID)
                    node.SceneInfo = SerializedManager.TargetManager.AnyScene;
                else
                    node.SceneInfo = SerializedManager.TargetManager.GetSceneById(node.SceneId);

                NodeEditor.AddNode(node);
            }
        }

        private void PopulateConnections(GUIGenericMenu contextMenu)
        {
            SceneNodeCollection nodes = SerializedManager.SceneNodes;

            List<TransitionConnector> madeTransitions = new List<TransitionConnector>();

            foreach (SceneNode node in nodes.SceneNodes)
            {
                foreach (SceneTransition transition in node.SceneInfo.Transitions)
                {
                    TransitionConnector existingOut = madeTransitions.Find((x) =>
                    {
                        return ((SceneNode)x.GetStartNode()).SceneId == node.SceneId &&
                            ((SceneNode)x.GetEndNode()).SceneId == transition.DestinationSceneId;
                    });

                    if (existingOut != null)
                    {
                        existingOut.ConnectionCount++;
                        continue;
                    }

                    TransitionConnector existingIn = madeTransitions.Find((x) =>
                    {
                        return ((SceneNode)x.GetStartNode()).SceneId == transition.DestinationSceneId &&
                                ((SceneNode)x.GetEndNode()).SceneId == node.SceneId;
                    });

                    TransitionConnector newConnection = new TransitionConnector();

                    newConnection.ContextMenu = contextMenu;
                    newConnection.AddNode(node);

                    if (node.SceneId == transition.DestinationSceneId)
                        AddSelfTransitionNodes(newConnection, node);

                    newConnection.AddNode(nodes.SceneNodes.Find((x) =>
                        x.SceneId == transition.DestinationSceneId));

                    if (existingIn != null)
                    {
                        newConnection.ShiftOver = true;
                        existingIn.ShiftOver = true;
                    }

                    NodeEditor.AddConnector(newConnection);
                    madeTransitions.Add(newConnection);
                }
            }
        }

    }
}