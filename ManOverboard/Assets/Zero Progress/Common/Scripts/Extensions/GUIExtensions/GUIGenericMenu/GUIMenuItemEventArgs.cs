using System;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Event args for the menu item that was selected
    /// </summary>
    public class GUIMenuItemEventArgs : EventArgs
    {
        /// <summary>
        /// The item that launched the event
        /// </summary>
        public GUIGenericMenuItem SelectedItem;

        /// <summary>
        /// The context the menu was opened for
        /// </summary>
        public System.Object MenuContext;

        /// <summary>
        /// The text of the selected item
        /// </summary>
        public string ItemText;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="selectedItem">The item that launched the event</param>
        /// <param name="menuContext">The context the menu was opened for</param>
        public GUIMenuItemEventArgs(GUIGenericMenuItem selectedItem,
            System.Object menuContext)
        {
            SelectedItem = selectedItem;
            MenuContext = menuContext;
            ItemText = selectedItem.Display.text;
        }
    }

    /// <summary>
    /// Event args with an additional entry for a custom parameter value
    /// </summary>
    public class GUIMenuItemParamEventArgs : GUIMenuItemEventArgs
    {
        /// <summary>
        /// The data stored with the menu item to be passed along
        /// to the execution logic
        /// </summary>
        public System.Object ParamValue;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="selectedItem">The item that launched the event</param>
        /// <param name="menuContext">The context the menu was opened for</param>
        /// <param name="paramValue">The data stored with the menu item to be passed along</param>
        public GUIMenuItemParamEventArgs(GUIGenericMenuItem selectedItem, 
            object menuContext, System.Object paramValue) 
            : base(selectedItem, menuContext)
        {
            ParamValue = paramValue;
        }
    }
}