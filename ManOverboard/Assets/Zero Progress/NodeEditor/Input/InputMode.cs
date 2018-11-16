using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// An Input Mode for the Node Editor used to perform
    /// modifications on the nodes and connections in it
    /// </summary>
    public abstract class InputMode
    {
        /// <summary>
        /// True if this input mode is in an active state, false if not
        /// </summary>
        public bool IsActivated { get; private set; }

        /// <summary>
        /// The node editor to work upon
        /// </summary>
        public NodeEditor NodeEditor { get; private set; }

        /// <summary>
        /// Lower numbers receive higher priority (will be processed first)
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeEditor">Editor to work upon</param>
        /// <param name="priority">When this mode should be processed relative
        /// to other input modes. Lower number == processed first</param>
        public InputMode(NodeEditor nodeEditor, int priority = 0)
        {
            if (nodeEditor == null)
                throw new ArgumentNullException("Receiver cannot be null");

            NodeEditor = nodeEditor;
            Priority = priority;
        }

        /// <summary>
        /// Fired before the editor handles events. Allows
        /// this input to use the event before the editor
        /// gets a chance to
        /// </summary>
        public virtual void BeforeEditorEvents() { }

        /// <summary>
        /// Fired after the editor handles events. Allows
        /// this input to catch events not handled by the editor
        /// </summary>
        public virtual void AfterEditorEvents() { }

        /// <summary>
        /// Fired before the nodes of the editor are rendered.
        /// Allows rendering things behind nodes.
        /// </summary>
        public virtual void BeforeNodeRender() { }

        /// <summary>
        /// Fired after the nodes of the editor are rendered.
        /// Allows rendering things on top of the nodes.
        /// </summary>
        public virtual void AfterNodeRender() { }

        /// <summary>
        /// Fired when this input mode is activated to handle
        /// any initialization
        /// </summary>
        protected virtual void OnActivated() { }

        /// <summary>
        /// Fired when this input mode is deactivated to
        /// handle any cleanup
        /// </summary>
        protected virtual void OnDeactivated() { }

        /// <summary>
        /// Fired when this input mode should have its
        /// active operations aborted
        /// </summary>
        protected virtual void OnCancelled() { }

        /// <summary>
        /// Checks if this input mode should be activated
        /// </summary>
        /// <returns>True to activate, false if not</returns>
        protected abstract bool ShouldActivate();
        
        /// <summary>
        /// Activates the input mode
        /// </summary>
        public void Activate()
        {
            if (IsActivated)
                return;

            IsActivated = true;
            OnActivated();
        }

        /// <summary>
        /// Aborts the input mode
        /// </summary>
        public void Cancel()
        {
            if (!IsActivated)
                return;

            OnCancelled();
            Deactivate();
        }

        /// <summary>
        /// Deactivates the input mode
        /// </summary>
        public void Deactivate()
        {
            if (!IsActivated)
                return;

            IsActivated = false;
            OnDeactivated();
        }

        /// <summary>
        /// Helper for children to check if activation should take place
        /// </summary>
        protected virtual void CheckActivation()
        {
            if (IsActivated)
                return;

            if (ShouldActivate())
                Activate();
        }
    }
}
