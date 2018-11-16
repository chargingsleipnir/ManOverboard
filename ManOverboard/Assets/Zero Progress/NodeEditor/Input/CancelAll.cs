using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Used to cancel all currently active input modes
    /// </summary>
    public class CancelAll : InputMode
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeEditor">Node editor to work on</param>
        /// <param name="priority">When this mode should be processed relative
        /// to other input modes. Lower number == processed first</param>
        public CancelAll(NodeEditor nodeEditor, int priority = 0) 
            : base(nodeEditor, priority)
        {
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
        /// Executes after the node editor has processed its events
        /// </summary>
        public override void AfterEditorEvents()
        {
            CheckActivation();

            if (!IsActivated)
                return;

            if (Event.current.type != EventType.KeyDown)
                return;

            if (Event.current.keyCode == KeyCode.Escape)
            {
                NodeEditor.CancelInputModes();
                NodeEditor.UnselectAll();
            }
        }
    }
}