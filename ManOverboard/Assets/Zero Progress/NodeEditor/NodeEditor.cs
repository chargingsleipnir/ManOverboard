using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Provides an extensible interface for node-based gui representations
    /// </summary>
    public class NodeEditor
    {
        /// <summary>
        /// Color to be used for selected items
        /// </summary>
        public static readonly Color DefaultSelectionColor = new Color(0.57f, 0.78f, 0.96f, 1f);

        /// <summary>
        /// Delegate used to determine the validity of the node for
        /// the desired operation
        /// </summary>
        /// <param name="node">The node to check</param>
        /// <returns>True if the node is valid for the operation, false if not</returns>
        public delegate bool IsValidNodeCheck(Node node);

        /// <summary>
        /// Node used to represent the mouse position
        /// </summary>
        public readonly NoPanNode MouseNode = new NoPanNode();

        /// <summary>
        /// Default background grid rendering
        /// </summary>
        /// <param name="editorArea">The area that defines the node</param>
        public static void DrawBackgroundGrid(Rect editorArea)
        {
            GUIExtensions.DrawGrid(editorArea, 20f, Color.black, 0.2f);
            GUIExtensions.DrawGrid(editorArea, 100f, Color.black, 0.4f);
        }

        /// <summary>
        /// Callback to be used for rendering the node editors background
        /// </summary>
        /// <param name="editorArea">The area that represents the node editor</param>
        public delegate void DrawBackgroundCallback(Rect editorArea);

        /// <summary>
        /// Callback to be used for rendering the node editors background
        /// </summary>
        /// <param name="editorArea">The area that represents the node editor</param>
        public DrawBackgroundCallback OnDrawBackground = DrawBackgroundGrid;

        /// <summary>
        /// True if selection inputs are allowed to activate, false
        /// if selection isn't permissible
        /// </summary>
        public bool SelectionAllowed { get; set; }

        /// <summary>
        /// Event fired whenever a node is added
        /// </summary>
        public event EventHandler<NodeAddedEventArgs> OnNodeAdded;

        /// <summary>
        /// Event fired whenever a node is removed
        /// </summary>
        public event EventHandler<NodeRemovedEventArgs> OnNodeRemoved;

        /// <summary>
        /// Event fired whenever a connection is made between nodes
        /// </summary>
        public event EventHandler<ConnectorAddedEventArgs> OnConnectorAdded;

        /// <summary>
        /// Event fired whenever a connection is removed
        /// </summary>
        public event EventHandler<ConnectorRemovedEventArgs> OnConnectorRemoved;

        /// <summary>
        /// Event fired whenever the current selection is changed
        /// </summary>
        public event EventHandler OnSelectionChanged;

        /// <summary>
        /// Defines the area of the node editor with zooming and panning capabilities
        /// </summary>
        protected ZoomableAreaHelper zoomArea = new ZoomableAreaHelper();

        /// <summary>
        /// The collection of nodes currently being drawn
        /// </summary>
        public List<Node> Nodes { get; protected set; }

        /// <summary>
        /// The collection of connectors between nodes currently being rendered
        /// </summary>
        public List<Connector> Connectors { get; protected set; }

        /// <summary>
        /// Input capabilities that can change the state and contents of the editor
        /// </summary>
        public List<InputMode> InputModes { get; protected set; }

        /// <summary>
        /// The rectangle that defines the active draw area
        /// </summary>
        public Rect NodeEditorRect { get; protected set; }

        /// <summary>
        /// A callback to determine if the node is removable
        /// 
        /// Is not used when clearing the nodes
        /// </summary>
        public IsValidNodeCheck CanDeleteNode = null;

        /// <summary>
        /// Flag returned when drawn to indicate if an action occured that
        /// requires the area to be repainted
        /// </summary>
        private bool needsRepaint = false;

        /// <summary>
        /// The mouse position scaled with the current zoom level
        /// </summary>
        private Vector2 scaledMousePosition;
        
        /// <summary>
        /// The currently active context menu
        /// </summary>
        private GUIGenericMenu activeContextMenu;

        /// <summary>
        /// What the context menu was opened on
        /// </summary>
        private System.Object contextMenuOwner = null;

        /// <summary>
        /// The event type before it was used, for any input modes that
        /// need to do processing on used events
        /// </summary>
        private EventType preUsedEventType;

        /// <summary>
        /// Constructor
        /// </summary>
        public NodeEditor()
        {
            SelectionAllowed = true;
            Nodes = new List<Node>();
            Connectors = new List<Connector>();
            InputModes = new List<InputMode>();
        }

        /// <summary>
        /// Get the type of the event before it was used
        /// 
        /// Used primarily for input modes that need to cache data
        /// for particular events
        /// </summary>
        /// <returns></returns>
        public EventType GetPreUsedEventType()
        {
            return preUsedEventType;
        }

        /// <summary>
        /// Gets the offset from the panning of the window
        /// </summary>
        /// <returns>The offset from panning</returns>
        public Vector2 GetPanOffset()
        {
            return zoomArea.CurrentPanOffset;
        }

        /// <summary>
        /// Sets the panning value
        /// </summary>
        /// <param name="offset">Panning value</param>
        public void SetPanOffset(Vector2 offset)
        {
            zoomArea.CurrentPanOffset = offset;
        }

        /// <summary>
        /// Gets the mouse position with the current zoom level applied
        /// </summary>
        /// <returns>The zoomed mouse position</returns>
        public Vector2 GetZoomedMousePosition()
        {
            return scaledMousePosition;
        }

        /// <summary>
        /// Get the mouse position with the zoom and panning applied
        /// </summary>
        /// <returns>The mouse position modified by the zoom and pan value</returns>
        public Vector2 GetMousePanAndZoom()
        {
            return (GetZoomedMousePosition() - GetPanOffset());
        }

        /// <summary>
        /// Apply the current zoom to the provided vector
        /// </summary>
        /// <param name="vector">The vector to transform</param>
        /// <returns>The modified vector</returns>
        public Vector2 ApplyZoomToVector(Vector2 vector)
        {
            return vector / zoomArea.CurrentZoom;
        }

        /// <summary>
        /// Allows zooming the editor
        /// </summary>
        public void EnableZoom()
        {
            zoomArea.AllowZooming = true;
        }

        /// <summary>
        /// Disables zooming of the editor
        /// </summary>
        public void DisableZoom()
        {
            zoomArea.AllowZooming = false;
        }

        /// <summary>
        /// Gets the current zoom constraints
        /// </summary>
        /// <returns>Vector where x is minzoom and y is maxzoom</returns>
        public Vector2 GetZoomConstraints()
        {
            return new Vector2(zoomArea.MinZoom, zoomArea.MaxZoom);
        }

        /// <summary>
        /// Sets the min and max allowable zooms
        /// </summary>
        /// <param name="minZoom">The smallest zoom allowed</param>
        /// <param name="maxZoom">The highest zoom allowed</param>
        public void SetZoomConstraints(float minZoom, float maxZoom)
        {
            zoomArea.MinZoom = minZoom;
            zoomArea.MaxZoom = maxZoom;
        }

        /// <summary>
        /// Indicates that the editor should be repainted
        /// </summary>
        public void FlagRepaint()
        {
            needsRepaint = true;
        }

        /// <summary>
        /// Raises the selection changed event
        /// </summary>
        public void RaiseSelectionChanged()
        {
            OnSelectionChanged.SafeInvoke(this);
        }

        /// <summary>
        /// Clears the data associated with the current context menu
        /// </summary>
        public void ClearContextMenu()
        {
            if (activeContextMenu != null)
                activeContextMenu.Hide();

            activeContextMenu = null;
            contextMenuOwner = null;
        }

        /// <summary>
        /// Retrieves the current context menu
        /// </summary>
        /// <returns>Context menu that is currently used, or null if there are none</returns>
        public GUIGenericMenu GetContextMenu()
        {
            return activeContextMenu;
        }

        /// <summary>
        /// Change the context menu that is currently active
        /// </summary>
        /// <param name="newMenu">The menu to show</param>
        /// <param name="owner">The object that the context menu was opened on</param>
        public void ChangeActiveContextMenu(GUIGenericMenu newMenu, System.Object owner)
        {
            if (activeContextMenu != null)
                activeContextMenu.Hide();

            activeContextMenu = newMenu;
            contextMenuOwner = owner;

            if (activeContextMenu != null)
                activeContextMenu.Show(owner);
        }

        /// <summary>
        /// Unselects all nodes and connectors
        /// </summary>
        public void UnselectAll()
        {
            UnselectAllNodes();
            UnselectAllConnectors();
        }

        /// <summary>
        /// Removes all nodes and connectors in the editor
        /// </summary>
        public void ClearAll()
        {
            ClearAllNodes();
            ClearAllConnectors();
        }

        #region Input Mode Management

        #region Input Mode Factory Methods

        /// <summary>
        /// Enables the default box select mode for nodes and connectors
        /// </summary>
        /// <param name="priority">The priority to assign this mode in relation to other inputs</param>
        public NodeEditor EnableBoxSelect(int priority = 1)
        {
            EnableInputMode<BoxSelectMode>(priority);
            return this;
        }

        /// <summary>
        /// Enables the default node selection
        /// </summary>
        /// <param name="priority">The priority to assign this mode in relation to other inputs</param>
        public NodeEditor EnableNodeSelect(int priority = 1)
        {
            EnableInputMode<NodeSelectMode>(priority);
            return this;
        }

        /// <summary>
        /// Enables the default connector selection
        /// </summary>
        /// <param name="priority">The priority to assign this mode in relation to other inputs</param>
        /// <returns>This node editor</returns>
        public NodeEditor EnableConnectorSelect(int priority = 2)
        {
            EnableInputMode<ConnectorSelectMode>(priority);
            return this;
        }

        /// <summary>
        /// Enables the default behaviour for when the background is clicked
        /// (which is to unselect anything currently selected
        /// </summary>
        /// <param name="priority">The priority to assign this mode in relation to other inputs</param>
        /// <returns>This node editor</returns>
        public NodeEditor EnableBackgroundUnselect(int priority = 10)
        {
            EnableInputMode<BackgroundClickedUnselector>(priority);
            return this;
        }

        /// <summary>
        /// Enables Node select all
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        public NodeEditor EnableNodeSelectAll(int priority = 1)
        {
            EnableInputMode<SelectAllNodesMode>(priority);
            return this;
        }

        /// <summary>
        /// Enables the input mode for handling cancellation of any active inputs
        /// </summary>
        /// <param name="priority">The priority to assign this mode in relation to other inputs</param>
        /// <returns>This node editor</returns>
        public NodeEditor EnableCancelAll(int priority = 4)
        {
            EnableInputMode<CancelAll>(priority);
            return this;
        }

        /// <summary>
        /// Enables the node context menu to be opened on right click
        /// </summary>
        /// <param name="priority">The priority to assign this mode in relation to other inputs</param>
        public NodeEditor EnableNodeRightClick(int priority = 0)
        {
            EnableInputMode<NodeRightClickMode>(priority);
            return this;
        }

        /// <summary>
        /// Enables the connector context menu to be opened on right click
        /// </summary>
        /// <param name="priority">The priority to assign this mode in relation to other inputs</param>
        public NodeEditor EnableConnectorRightClick(int priority = 2)
        {
            EnableInputMode<ConnectorRightClickMode>(priority);
            return this;
        }

        /// <summary>
        /// Enables the ability to drag selected nodes
        /// </summary>
        /// <param name="priority">The priority to assign this mode in relation to other inputs</param>
        public NodeEditor EnableNodeDrag(int priority = 2)
        {
            EnableInputMode<SelectedNodeDragMode>(priority);
            return this;
        }

        /// <summary>
        /// Enables deletion using the delete key
        /// </summary>
        /// <param name="priority">The priority to assign this mode in relation to other inputs</param>
        public NodeEditor EnableDeleteKey(int priority = 0)
        {
            EnableInputMode<DeleteKeyInput>(priority);
            return this;
        }

        /// <summary>
        /// Enables snapping the current pan to the selected nodes
        /// (or the average node position if none selected). If there are
        /// no nodes, resets to Vector2.zero
        /// </summary>
        /// <param name="priority">The priority to assign this mode in relation to other inputs</param>
        /// <returns>This editor</returns>
        public NodeEditor EnableFocusKey(int priority = 3)
        {
            EnableInputMode<FocusInput>(priority);
            return this;
        }

        /// <summary>
        /// Enables the input mode for making connections between nodes
        /// </summary>
        /// <param name="priority">The priority to assign this mode in relation to other inputs</param>
        /// <returns>This editor</returns>
        public NodeEditor EnableConnectNodeInputMode(int priority = 0)
        {
            EnableInputMode<ConnectNodeInputMode>(priority);
            return this;
        }

        #endregion

        /// <summary>
        /// Removes all input modes currently associated with the node editor
        /// </summary>
        public void ClearInputModes()
        {
            InputModes.Clear();
        }

        /// <summary>
        /// Creates and adds the specified input mode
        /// </summary>
        /// <typeparam name="T">The type of input mode to create</typeparam>
        /// <param name="priority">The priority to assign this mode in relation to other inputs</param>
        public void EnableInputMode<T>(int priority) where T:InputMode
        {
            Type modeType = typeof(T);

            EnableInputMode(
                (InputMode)Activator.CreateInstance(modeType, this, priority));
        }

        /// <summary>
        /// Add an input mode
        /// </summary>
        /// <param name="inputMode">The input mode to add</param>
        public void EnableInputMode(InputMode inputMode)
        {
            InputModes.Add(inputMode);
            SortInputModes();
        }

        /// <summary>
        /// Get the input mode of the specified type
        /// </summary>
        /// <typeparam name="T">The type to search for</typeparam>
        /// <returns>The found input mode that matches the type, or null if not found</returns>
        public T GetInputMode<T>() where T: InputMode
        {
            return GetInputMode(typeof(T)) as T;
        }

        /// <summary>
        /// Get the input mode of the specified type
        /// </summary>
        /// <param name="modeType">The type of input mode to retrieve</param>
        /// <returns>The found input mode that matches the type, or null if not found</returns>
        public InputMode GetInputMode(Type modeType)
        {
            return InputModes.Find((x) => x.GetType().IsOrIsSubclassOf(modeType));
        }

        /// <summary>
        /// Iterates over the enabled input modes and calls cancel on each
        /// </summary>
        public void CancelInputModes()
        {
            foreach (InputMode inputMode in InputModes)
            {
                inputMode.Cancel();
            }
        }
        
        /// <summary>
        /// Sort the input modes
        /// </summary>
        private void SortInputModes()
        {
            InputModes.Sort((x, y) => x.Priority.CompareTo(y.Priority));
        }

        /// <summary>
        /// Helper to process the current event with each input mode
        /// </summary>
        /// <param name="isPre">True if calling before the Editor events, false
        /// for afterwards</param>
        protected void ProcessInputEvents(bool isPre)
        {
            for (int i = 0; i < InputModes.Count; i++)
            {
                if (isPre)
                    InputModes[i].BeforeEditorEvents();
                else
                    InputModes[i].AfterEditorEvents();
            }
        }

        /// <summary>
        /// Helper to process the rendering of input modes
        /// </summary>
        /// <param name="isPre">True if before editor rendering, false
        /// for afterwards</param>
        protected void ProcessInputRenders(bool isPre)
        {
            for (int i = 0; i < InputModes.Count; i++)
            {
                if (isPre)
                    InputModes[i].BeforeNodeRender();
                else
                    InputModes[i].AfterNodeRender();
            }
        }
        #endregion

        #region Node Management 

        /// <summary>
        /// Gets the first node that is under the mouse
        /// </summary>
        /// <returns>The node underneath, or null if none found</returns>
        public Node GetNodeUnderMouse()
        {
            return GetNodesUnderMouse().FirstOrDefault();
        }

        /// <summary>
        /// Gets all of the nodes found underneath the mouse
        /// </summary>
        /// <returns>Collection of nodes found underneath the mouse</returns>
        public IEnumerable<Node> GetNodesUnderMouse()
        {
            return Nodes.Where((x) => GetNodeRect(x).Contains(scaledMousePosition));
        }

        /// <summary>
        /// Gets the node hosting the current context menu
        /// </summary>
        /// <returns>The node the context menu was opened on, or null if
        /// the menu isn't from a node or there's no context menu active</returns>
        public Node GetNodeUnderContextMenu()
        {
            return contextMenuOwner as Node;
        }

        /// <summary>
        /// Adds a node
        /// </summary>
        /// <param name="newNode">The node to be added</param>
        public virtual void AddNode(Node newNode)
        {
            if (newNode == null)
                throw new ArgumentNullException("Cannot add null node");

            Nodes.Add(newNode);
            OnNodeAdded.SafeInvoke(this, new NodeAddedEventArgs(newNode));
        }

        /// <summary>
        /// Removes a node
        /// </summary>
        /// <param name="node">Removes the specified node</param>
        public void RemoveNode(Node node)
        {
            if (CanDeleteNode != null && !CanDeleteNode(node))
                return;

            Nodes.Remove(node);
            OnNodeRemoved.SafeInvoke(this, new NodeRemovedEventArgs(node));
        }

        /// <summary>
        /// Removes all nodes that currently have their selected
        /// status to true
        /// </summary>
        public void RemoveSelectedNodes()
        {
            Node[] selectedNodes = GetSelectedNodes().ToArray();

            for (int i = selectedNodes.Length - 1; i >= 0; i--)
            {
                RemoveNode(selectedNodes[i]);
            }

            RaiseSelectionChanged();
        }

        /// <summary>
        /// Removes all nodes from the editor
        /// </summary>
        public void ClearAllNodes()
        {
            Nodes.Clear();
        }

        /// <summary>
        /// Get all nodes that are currently selected
        /// </summary>
        /// <returns>Collection of all selected nodes</returns>
        public IEnumerable<Node> GetSelectedNodes()
        {
            return Nodes.Where((x) => x.IsSelected());
        }

        /// <summary>
        /// Get all nodes in the provided rectangle
        /// </summary>
        /// <param name="area">The area to get the nodes within</param>
        /// <param name="allowOverlaps">True to allow overlap with the rectangle,
        /// false to require full containment</param>
        /// <returns>The collection of nodes within the provided rectangle</returns>
        public IEnumerable<Node> GetNodesInRect(Rect area, bool allowOverlaps = false)
        {
            return Nodes.Where((x) =>
            {
                Rect nodeRect = GetNodeRect(x);

                return area.Contains(nodeRect) ||
                    (allowOverlaps && area.Overlaps(nodeRect));
            });
        }

        /// <summary>
        /// Gets the number of nodes that are currently selected
        /// </summary>
        /// <returns>The number of selected items</returns>
        public int GetSelectedNodesCount()
        {
            return Nodes.Count((x) => x.IsSelected());
        }

        /// <summary>
        /// Selects all nodes
        /// </summary>
        public void SelectAllNodes()
        {
            foreach (Node node in Nodes)
            {
                node.Select();
            }

            FlagRepaint();
        }

        /// <summary>
        /// Unselects all nodes
        /// </summary>
        public void UnselectAllNodes()
        {
            foreach (Node node in Nodes)
            {
                node.UnSelect();
            }

            FlagRepaint();
        }

        /// <summary>
        /// Check to see if all the nodes are currently selected
        /// </summary>
        /// <returns></returns>
        public bool AreAllNodesSelected()
        {
            foreach (Node node in Nodes)
            {
                if (node.IsSelectable && !node.IsSelected())
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the rectangle of the node with the current panning applied
        /// </summary>
        /// <param name="node">The node to get the rectangle for</param>
        /// <returns>The rectangle that defines the nodes space</returns>
        protected Rect GetNodeRect(Node node)
        {
            return node.GetNodeRect(zoomArea.CurrentPanOffset);
        }

        #endregion

        #region Connector Management

        /// <summary>
        /// Gets the connector currently underneath the mouse
        /// </summary>
        /// <returns>The connector under the mouse</returns>
        public Connector GetNodeConnectorUnderMouse()
        {
            return GetNodeConnectorsUnderMouse().FirstOrDefault();
        }

        /// <summary>
        /// Gets all connectors found underneath the mouse position
        /// </summary>
        /// <returns>All connectors found under the mouse</returns>
        public IEnumerable<Connector> GetNodeConnectorsUnderMouse()
        {
            return Connectors.Where((x) => x.ContainsPoint(
                GetZoomedMousePosition(), Vector2.one * 7f, GetPanOffset()));
        }

        /// <summary>
        /// Gets the connector associated with the active context menu
        /// </summary>
        /// <returns>The connector associated with the context menu, or null
        /// if the context menu object isn't a connector or if there is no active
        /// context menu</returns>
        public Connector GetConnectorUnderContextMenu()
        {
            return contextMenuOwner as Connector;
        }

        /// <summary>
        /// Adds a connector
        /// </summary>
        /// <param name="connector">Connector to be added</param>
        public virtual void AddConnector(Connector connector)
        {
            if (connector == null)
                throw new ArgumentNullException("Cannot add null connector");

            Connectors.Add(connector);
            OnConnectorAdded.SafeInvoke(this, new ConnectorAddedEventArgs(connector));
        }

        /// <summary>
        /// Removes a connector
        /// </summary>
        /// <param name="connector">The connector to be removed</param>
        /// <param name="notifyListeners">True to raise the ConnectorRemove event,
        /// false to remove this connector silently</param>
        public void RemoveConnector(Connector connector, bool notifyListeners = true)
        {
            Connectors.Remove(connector);
            if(notifyListeners)
                OnConnectorRemoved.SafeInvoke(this, new ConnectorRemovedEventArgs(connector));
        }

        /// <summary>
        /// Removes all connectors that currently have their selected status
        /// set to true
        /// </summary>
        public void RemoveSelectedConnectors()
        {
            Connector[] selectedConnectors = GetSelectedConnectors().ToArray();

            for (int i = selectedConnectors.Length - 1; i >= 0; i--)
            {
                RemoveConnector(selectedConnectors[i]);
            }

            RaiseSelectionChanged();
        }

        /// <summary>
        /// Removes all connectors from this node editor
        /// </summary>
        public void ClearAllConnectors()
        {
            Connectors.Clear();
        }

        /// <summary>
        /// Gets all currently selected connections
        /// </summary>
        /// <returns>Collection of connections currently selected</returns>
        public IEnumerable<Connector> GetSelectedConnectors()
        {
            return Connectors.Where((x) => x.IsSelected());
        }

        /// <summary>
        /// Gets the number of selected connectors
        /// </summary>
        /// <returns>The number of selected connectors</returns>
        public int GetSelectedConnectorsCount()
        {
            return Connectors.Count((x) => x.IsSelected());
        }

        /// <summary>
        /// Selects all connectors
        /// </summary>
        public void SelectAllConnectors()
        {
            foreach (Connector connector in Connectors)
            {
                connector.Select();
            }

            FlagRepaint();
        }

        /// <summary>
        /// Unselects all connectors
        /// </summary>
        public void UnselectAllConnectors()
        {
            foreach (Connector connector in Connectors)
            {
                connector.UnSelect();
            }

            FlagRepaint();
        }

        /// <summary>
        /// Get all connectors in the provided rectangle
        /// </summary>
        /// <param name="area">The area to get the connectors within</param>
        /// <param name="allowOverlaps">True to allow overlap with the rectangle,
        /// false to require full containment</param>
        /// <returns>The collection of connectors within the provided rectangle</returns>
        public IEnumerable<Connector> GetConnectorsInRect(Rect area, bool allowOverlaps = false)
        {
            return Connectors.Where((x) =>
                x.ContainedByRect(area, GetPanOffset(), allowOverlaps));
        }
        #endregion

        /// <summary>
        /// Handles the events and rendering for the node editor
        /// </summary>
        /// <param name="nodeEditorArea">The area that defines where the node editor
        /// can display</param>
        /// <returns>True if repaint is required, false if not</returns>
        public virtual bool OnGUI(Rect nodeEditorArea)
        {
            RefreshCachedValues();
            
            if (activeContextMenu != null)
                activeContextMenu.HandleEvents();

            ProcessInputEvents(true);

            foreach (Node node in Nodes)
            {
                if (node.HandleEvents(this, zoomArea.CurrentPanOffset))
                    needsRepaint = true;
            }

            switch (Event.current.type)
            {
                case EventType.Repaint:
                case EventType.Layout:
                    Draw(nodeEditorArea);

                    // Need a group here to make sure that the mouse positions retrieved
                    // within the context menu are correct
                    GUI.BeginGroup(nodeEditorArea);
                    if (activeContextMenu != null)
                    {
                        activeContextMenu.Draw();

                        if (activeContextMenu.IsShown)
                            FlagRepaint();
                    }
                    GUI.EndGroup();
                    break;
            }

            ProcessInputEvents(false);

            zoomArea.HandleEvents();

            return needsRepaint;
        }
        
        /// <summary>
        /// Prepares all the cached values. Should be called at the beginning of OnGUI
        /// </summary>
        protected virtual void RefreshCachedValues()
        {
            preUsedEventType = Event.current.type;

            scaledMousePosition = Event.current.mousePosition / zoomArea.CurrentZoom;

            MouseNode.NodeRect.position = scaledMousePosition;

            needsRepaint = false;

        }

        /// <summary>
        /// Draws the node editor and all of its components
        /// </summary>
        /// <param name="nodeEditorArea">The rectangle that defines the editor area</param>
        protected virtual void Draw(Rect nodeEditorArea)
        {
            NodeEditorRect = zoomArea.BeginZoomArea(nodeEditorArea);

            if (OnDrawBackground != null && Event.current.type == EventType.Repaint)
                OnDrawBackground(NodeEditorRect.AddPosition(zoomArea.CurrentPanOffset));

            ProcessInputRenders(true);

            foreach (Connector connector in Connectors)
            {
                connector.Draw(zoomArea.CurrentPanOffset);
            }

            foreach (Node node in Nodes)
            {
                node.Draw(zoomArea.CurrentPanOffset);
            }

            ProcessInputRenders(false);

            zoomArea.EndZoomArea();
        }
    }
}
