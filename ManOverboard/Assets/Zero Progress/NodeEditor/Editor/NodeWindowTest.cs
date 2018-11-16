using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.NodeEditor;

public class NodeWindowTest : EditorWindow {

    [MenuItem("Window/Node Based Editor")]
    private static void OpenWindow()
    {
        NodeWindowTest window = GetWindow<NodeWindowTest>();
        window.wantsMouseMove = true;
        window.wantsMouseEnterLeaveWindow = true;
        window.titleContent = new GUIContent("Node Based Editor");
    }

    private NodeEditor nodeEditor;
    private ConnectNodeInputMode connectorMode;

    private void OnGUI()
    {
        if (nodeEditor == null)
        {
            GUIGenericMenu nodeMenu = new GUIGenericMenu();

            nodeMenu.AddMenuItem("Make Transition", StartConnection);
            nodeMenu.AddMenuItem("Create Node", CreateNewNode);
            nodeMenu.AddMenuItem("Create/Items/", "Node");
            nodeMenu.AddMenuItem("Delete Node", Delete);

            nodeEditor = new NodeEditor();
            nodeEditor.EnableBoxSelect();
            nodeEditor.EnableNodeSelect();
            nodeEditor.EnableConnectorSelect();
            nodeEditor.EnableNodeDrag();
            nodeEditor.EnableNodeRightClick();
            nodeEditor.EnableConnectorRightClick();
            nodeEditor.EnableInputMode<ConnectorMakerMode>(2);

            connectorMode = new ConnectNodeInputMode(nodeEditor, 2);
            nodeEditor.EnableInputMode(connectorMode);

            Node node1 = new TextNode(Vector2.zero, "Test", null, nodeMenu);
            Node node2 = new TextNode(Vector2.one * 50f, "Test 2", null, nodeMenu);
            
            GUIGenericMenu connectMenu = new GUIGenericMenu();
            connectMenu.AddMenuItem("Delete Connection", DeleteConnection);
            nodeEditor.AddNode(node1);
            nodeEditor.AddNode(node2);
            //nodeEditor.AddConnector(new Connector(connectMenu, node1, node2));
           // nodeEditor.AddConnector(new Connector(connectMenu, node1, nodeEditor.MouseNode));
        }
        
        if (nodeEditor.OnGUI(position.WithPosition(Vector2.zero)))
        {
            Repaint();
        }
    }

    private void StartConnection(System.Object param)
    {
        connectorMode.Activate();
    }

    private void CreateNewNode(System.Object param)
    {
        Vector2 newPos = nodeEditor.GetMousePanAndZoom();

        nodeEditor.AddNode(new TextNode(newPos, "New Node"));
    }

    private void Delete(System.Object param)
    {
        TextNode node = nodeEditor.GetNodeUnderContextMenu() as TextNode;

        string nodeName = node.NodeText;
        Debug.Log("Delete called on: " + nodeName);
    }

    private void DeleteConnection(System.Object param)
    {
        //Connector connector = nodeEditor.GetConnectorUnderContextMenu();

        Debug.Log("Delete called on connector");
    }
}
