using System;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// A Menu item that uses an event to respond to activation
    /// </summary>
    public class GUIEventMenuItem : GUIGenericMenuItem
    {
        /// <summary>
        /// The event fired when the menu item is executed
        /// </summary>
        public event EventHandler<GUIMenuItemEventArgs> OnExecuted;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">The path that the menu item can be found at inside the menu</param>
        /// <param name="content">The text/image to be displayed. The text should always be set though,
        public GUIEventMenuItem(string path, GUIContent content) 
            : base(path, content)
        {
        }

        /// <summary>
        /// Executes the event
        /// </summary>
        /// <param name="context">The object that the menu was opened on</param>
        public override void Execute(object context)
        {
            OnExecuted.SafeInvoke(this, new GUIMenuItemEventArgs(this, context));
        }
    }
}