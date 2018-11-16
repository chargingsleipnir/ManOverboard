namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Helper to easily configure which default input modes should be enabled in
    /// the Node Editor
    /// </summary>
    public class DefaultInputModeConfig
    {
        public bool BoxSelect = true;
        public bool NodeSelect = true;
        public bool ConnectorSelect = true;
        public bool BackgroundUnselect = true;
        public bool SelectAllNodes = true;

        public bool NodeRightClick = true;
        public bool ConnectorRightClick = true;

        public bool ConnectNodeInput = true;

        public bool NodeDrag = true;

        public bool DeleteKey = true;
        public bool FocusKey = true;
        public bool EscapeKeyCancelAll = true;

        /// <summary>
        /// Applies the desired default behaviours to the provided editor
        /// </summary>
        /// <param name="editor">The Node Editor to apply default input modes to</param>
        public void ApplyToNodeEditor(NodeEditor editor)
        {
            if (BoxSelect)
                editor.EnableBoxSelect();

            if (NodeSelect)
                editor.EnableNodeSelect();

            if (ConnectorSelect)
                editor.EnableConnectorSelect();

            if (BackgroundUnselect)
                editor.EnableBackgroundUnselect();

            if (SelectAllNodes)
                editor.EnableNodeSelectAll();

            if (NodeRightClick)
                editor.EnableNodeRightClick();

            if (ConnectorRightClick)
                editor.EnableConnectorRightClick();

            if (ConnectNodeInput)
                editor.EnableConnectNodeInputMode();

            if (NodeDrag)
                editor.EnableNodeDrag();

            if (DeleteKey)
                editor.EnableDeleteKey();

            if (FocusKey)
                editor.EnableFocusKey();

            if (EscapeKeyCancelAll)
                editor.EnableCancelAll();
        }        
    }
}