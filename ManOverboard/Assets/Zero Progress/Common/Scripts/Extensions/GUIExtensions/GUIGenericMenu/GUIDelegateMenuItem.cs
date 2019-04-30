using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// A menu item that takes a callback with no parameters
    /// </summary>
    public class GUIDelegateMenuItem : GUIGenericMenuItem
    {
        /// <summary>
        /// Callback that executes without any information
        /// </summary>
        public delegate void MenuItemAction();

        /// <summary>
        /// Callback that will be executed
        /// </summary>
        public MenuItemAction Callback;

        public GUIDelegateMenuItem(string path, GUIContent content, MenuItemAction action) 
            : base(path, content)
        {
            Callback = action;
        }

        /// <summary>
        /// Execute the callback
        /// </summary>
        /// <param name="context">The item that the menu was opened for</param>
        public override void Execute(object context)
        {
            if (Callback != null)
                Callback();
        }
    }
}