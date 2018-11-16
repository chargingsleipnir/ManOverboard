using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Selects all nodes
    /// </summary>
    public class SelectAllNodesMode : SelectionInputMode
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeEditor">Node editor to work on</param>
        /// <param name="priority">When this mode should be processed relative
        /// to other input modes. Lower number == processed first</param>
        public SelectAllNodesMode(NodeEditor nodeEditor, int priority = 0) 
            : base(nodeEditor, priority)
        {
        }

        /// <summary>
        /// Executes after the node editor has processed its events
        /// </summary>
        protected override bool ShouldActivate()
        {
            return true;
        }

        /// <summary>
        /// Before the editor events
        /// </summary>
        public override void BeforeEditorEvents()
        {
            CheckActivation();

            if (!IsActivated)
                return;

            if (Event.current.type != EventType.KeyDown)
                return;

            if (Event.current.keyCode == KeyCode.A && 
                Event.current.modifiers == EventModifiers.Shift)
            {
                if (NodeEditor.AreAllNodesSelected())
                    NodeEditor.UnselectAllNodes();
                else
                    NodeEditor.SelectAllNodes();
            }

        }
    }
}