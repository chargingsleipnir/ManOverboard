using ZeroProgress.Common;

namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Base class for input modes associated with opening context menus
    /// </summary>
    public abstract class ContextMenuInput : InputMode
    {
        /// <summary>
        /// The currently active menu
        /// </summary>
        protected GUIGenericMenu activeMenu = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeEditor">The node editor this input mode works on</param>
        /// <param name="priority">Priority of the input mode</param>
        public ContextMenuInput(NodeEditor nodeEditor, int priority = 0) 
            : base(nodeEditor, priority)
        {
        }

        /// <summary>
        /// Checks if the context menu should be opened
        /// </summary>
        public override void BeforeEditorEvents()
        {
            CheckActivation();
        }

        /// <summary>
        /// Clean up
        /// </summary>
        protected override void OnDeactivated()
        {
            if (activeMenu == null)
                return;

            activeMenu.OnHide -= ActiveMenu_OnHide;
            activeMenu = null;
            NodeEditor.ClearContextMenu();
        }

        /// <summary>
        /// Helper for updating the active menu cached in this class
        /// and in the Node Editor
        /// </summary>
        /// <param name="menu">The menu to be set</param>
        /// <param name="menuOwner">The owner of the menu</param>
        protected void UpdateActiveMenu(GUIGenericMenu menu, System.Object menuOwner)
        {
            if (activeMenu != null)
            {
                activeMenu.OnHide -= ActiveMenu_OnHide;
                activeMenu.Hide();
            }

            activeMenu = menu;

            if (activeMenu != null)
            {
                activeMenu.OnHide += ActiveMenu_OnHide;
                NodeEditor.ChangeActiveContextMenu(menu, menuOwner);
            }
        }

        /// <summary>
        /// Response to the context menu being hidden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void ActiveMenu_OnHide(object sender, System.EventArgs e)
        {
            Deactivate();
        }
    }
}