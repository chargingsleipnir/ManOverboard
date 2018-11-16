using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Menu item that takes a static parameter that gets passed to the callback
    /// </summary>
    public class GUIParamDelegateMenuItem : GUIGenericMenuItem
    {
        /// <summary>
        /// Callback that takes a parameter of any type
        /// </summary>
        /// <param name="data"></param>
        public delegate void MenuItemAction(GUIMenuItemParamEventArgs args);

        /// <summary>
        /// Callback that will be executed
        /// </summary>
        public MenuItemAction Callback;

        /// <summary>
        /// The data to be passed to the callback
        /// </summary>
        public System.Object UserData;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path of the menu item</param>
        /// <param name="content">Text of the menu item itself</param>
        /// <param name="action">The action to be executed on click</param>
        /// <param name="userData">Data to be passed to the callback</param>
        public GUIParamDelegateMenuItem(string path, GUIContent content,
            MenuItemAction action, object userData = null) 
            : base(path, content)
        {
            Callback = action;
            UserData = userData;
        }

        /// <summary>
        /// Executes the callback
        /// </summary>
        /// <param name="context">The item that the menu was opened for</param>
        public override void Execute(System.Object context)
        {
            if (Callback != null)
                Callback(new GUIMenuItemParamEventArgs(this, context, UserData));
        }
    }
}