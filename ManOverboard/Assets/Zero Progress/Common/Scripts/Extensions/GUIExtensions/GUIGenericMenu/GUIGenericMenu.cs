using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// A replacement for the Editor Generic Menu that works with the
    /// GUI system
    /// </summary>
    public class GUIGenericMenu
    {
        /// <summary>
        /// The minimum width of the menu
        /// </summary>
        public static float MinWidth = 150f;

        /// <summary>
        /// The style used for the menu
        /// </summary>
        private GUIStyle panelStyle;

        /// <summary>
        /// Event fired when the menu is shown
        /// </summary>
        public event EventHandler OnShow;

        /// <summary>
        /// Event fired when the menu is hidden
        /// </summary>
        public event EventHandler OnHide;

        /// <summary>
        /// The item this menu was opened on to be passed to activated menu items
        /// </summary>
        public System.Object ContextObject { get; set; }

        /// <summary>
        /// True if currently rendered, false if not
        /// </summary>
        public bool IsShown { get; private set; }

        /// <summary>
        /// Grouping of items where the key is the path of the item and
        /// the value is the collection of items to be rendered. For root
        /// items, string.Empty should be used
        /// </summary>
        protected Dictionary<string, List<GUIGenericMenuItem>> groupedMenuItems =
            new Dictionary<string, List<GUIGenericMenuItem>>();
        
        /// <summary>
        /// The currently expanded sub-groups
        /// </summary>
        internal List<GUIGroupMenuItem> selectedPaths = new List<GUIGroupMenuItem>();

        /// <summary>
        /// Where to be rendered
        /// </summary>
        private Vector2 drawPosition;

        /// <summary>
        /// The currently highlighted item
        /// </summary>
        private GUIGenericMenuItem selectedItem = null;

        /// <summary>
        /// The rectangle of the deepest child group being rendered (allows us
        /// to see if the mouse has fallen out of bounds of a child group)
        /// </summary>
        private Rect deepestRect = Rect.zero;

        /// <summary>
        /// True if a child can be removed when the mouse exits the menu, false if not.
        /// This makes it so that only the deepest child is closed when the mouse leaves
        /// the menu, instead of losing all progress through the tree when the mouse
        /// slips out of the area
        /// </summary>
        private bool canRemove = true;

        /// <summary>
        /// Adds a separator item to the menu at the provided path
        /// </summary>
        /// <param name="path">The path to add the item to</param>
        public void AddSeparator(string path = "")
        {
            AddMenuItem(new GUIMenuItemSeparator(path));
        }

        /// <summary>
        /// Adds a parameterized delegate menu item to the root
        /// </summary>
        /// <param name="text">The text to be displayed in the menu</param>
        /// <param name="callback">The callback to be executed on click</param>
        /// <param name="callbackData">Any static data to be passed along</param>
        public GUIParamDelegateMenuItem AddMenuItem(string text, 
            GUIParamDelegateMenuItem.MenuItemAction callback, 
            System.Object callbackData = null)
        {
            return AddMenuItem(string.Empty, text, callback, callbackData);
        }

        /// <summary>
        /// Adds a parameterized delegate menu item to the root
        /// </summary>
        /// <param name="path">The path in the menu to place the item</param>
        /// <param name="text">The text to be displayed in the menu</param>
        /// <param name="callback">The callback to be executed on click</param>
        /// <param name="callbackData">Any static data to be passed along</param>
        /// <returns>The created menu item</returns>
        public GUIParamDelegateMenuItem AddMenuItem(string path, string text,
            GUIParamDelegateMenuItem.MenuItemAction callback, 
            System.Object callbackData = null)
        {
            GUIParamDelegateMenuItem newMenuItem = new GUIParamDelegateMenuItem(path,
                new GUIContent(text), callback, callbackData);
            AddMenuItem(newMenuItem);
            return newMenuItem;
        }

        /// <summary>
        /// Adds a delegate menu item
        /// </summary>
        /// <param name="path">The path in the menu to place the item</param>
        /// <param name="text">The text to be displayed in the menu</param>
        /// <param name="callback">The callback to be executed on click</param>
        /// <returns>The created menu item</returns>
        public GUIDelegateMenuItem AddMenuItem(string path, string text,
            GUIDelegateMenuItem.MenuItemAction callback = null)
        {
            GUIDelegateMenuItem newMenuItem = new GUIDelegateMenuItem(path, new GUIContent(text), callback);
            AddMenuItem(newMenuItem);
            return newMenuItem;
        }

        /// <summary>
        /// Adds an event menu item
        /// </summary>
        /// <param name="path">The path in the menu to place the item</param>
        /// <param name="text">The text to be displayed in the menu</param>
        /// <returns>The created menu item</returns>
        public GUIEventMenuItem AddMenuItem(string path, string text)
        {
            GUIEventMenuItem newMenuItem = new GUIEventMenuItem(path, new GUIContent(text));
            AddMenuItem(newMenuItem);
            return newMenuItem;
        }

        /// <summary>
        /// Adds the provided menu item to the list
        /// </summary>
        /// <param name="item">The item to be added</param>
        public void AddMenuItem(GUIGenericMenuItem item)
        {
            AddHierarchy(item);
            Internal_AddMenuItem(item);
        }
        
        /// <summary>
        /// Renders the menu item if it's currently shown
        /// </summary>
        public void Draw()
        {
            selectedItem = null;

            if (!IsShown)
                return;
            
            List<GUIGenericMenuItem> rootGroup;

            if (!groupedMenuItems.TryGetValue(string.Empty, out rootGroup))
                return;
                        
            DrawGroup(new Rect(drawPosition, Vector2.zero), string.Empty, rootGroup);
            
            for (int i = 0; i < selectedPaths.Count; i++)
            {
                GUIGroupMenuItem groupItem = selectedPaths[i];
                List<GUIGenericMenuItem> nestedGroup;
                
                string nextGroup = groupItem.Path;

                if (!string.IsNullOrEmpty(nextGroup))
                    nextGroup += "/";

                nextGroup += groupItem.Display.text;
                
                if (!groupedMenuItems.TryGetValue(nextGroup, out nestedGroup))
                    continue;

                Rect rect = DrawGroup(groupItem.PositionRect, nextGroup, nestedGroup);

                if (i == selectedPaths.Count - 1)
                    deepestRect = rect;
            }
        }

        /// <summary>
        /// Show the menu where the mouse is currently located,
        /// providing the context that it was opened on
        /// </summary>
        /// <param name="context">The item that the menu was opened on</param>
        public void Show(System.Object context)
        {
            Show(Event.current.mousePosition, context);
        }

        /// <summary>
        /// Show the menu, providing the context that it was opened on
        /// </summary>
        /// <param name="renderLocation">Where to display the menu</param>
        /// <param name="context">The item that the menu was opened on</param>
        public void Show(Vector2 renderLocation, System.Object context)
        {
            if (IsShown && context == ContextObject)
                return;

            IsShown = true;
            ContextObject = context;
            drawPosition = renderLocation;
            canRemove = true;
            selectedPaths.Clear();
            OnShow.SafeInvoke(this);
        }

        /// <summary>
        /// Hide the menu
        /// </summary>
        public void Hide()
        {
            if (!IsShown)
                return;

            IsShown = false;
            ContextObject = null;
            OnHide.SafeInvoke(this);
        }

        /// <summary>
        /// Handle the non-render related events
        /// </summary>
        public void HandleEvents()
        {
            if (!IsShown)
                return;

            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    if (selectedItem != null)
                    {
                        selectedItem.Execute(ContextObject);
                        if (!(selectedItem is GUIGroupMenuItem))
                            Hide();
                        Event.current.Use();
                    }
                    else
                        Hide();
                    break;
                case EventType.MouseDrag:
                    Event.current.Use();
                    break;
                case EventType.KeyDown:
                    break;
                case EventType.KeyUp:
                    break;
            }
        }
        
        /// <summary>
        /// Renders the current group of menu items
        /// </summary>
        /// <param name="parentRect">The rectangle of the parent</param>
        /// <param name="path">The path of the group to be rendered</param>
        /// <param name="groupItems">The collection of menu items at the provided path</param>
        /// <returns>The rectangle of the group</returns>
        private Rect DrawGroup(Rect parentRect, string path, IEnumerable<GUIGenericMenuItem> groupItems)
        {
            InitializeStyles();

            Vector2 groupSize = GetGroupSize(groupItems);
                        
            Rect renderArea = GetSafeDisplayRect(parentRect,
                new Rect(parentRect.position, groupSize));
            
            if (!CanDisplay(path, groupItems))
                return Rect.zero;

            GUI.Box(renderArea, "", panelStyle);
            
            Rect itemRect = new Rect(renderArea.position, groupSize);

            foreach (GUIGenericMenuItem item in groupItems)
            {
                itemRect.height = item.GetSize().y;

                bool isItemSelected = IsItemSelected(item, itemRect);

                item.Draw(itemRect, isItemSelected, ContextObject);

                itemRect.y = itemRect.yMax;
            }

            return renderArea;
        }

        /// <summary>
        /// Determines if the provided item is currently selected
        /// Applies specialized logic in the case of a Group item
        /// </summary>
        /// <param name="item">The item to query</param>
        /// <param name="itemRect">The rectangle of the item</param>
        /// <returns>True if selected, false if not</returns>
        private bool IsItemSelected(GUIGenericMenuItem item, Rect itemRect)
        {
            if (itemRect.Contains(Event.current.mousePosition))
            {
                canRemove = true;
                selectedItem = item;
                return true;
            }
            
            if (item is GUIGroupMenuItem && 
                selectedPaths.Find((x) => x.Path.Equals(item.Path)) != null)
                return true;
            
            return false;
        }

        /// <summary>
        /// Initialize menu-related styles
        /// </summary>
        private void InitializeStyles()
        {
            if (panelStyle == null)
            {
                panelStyle = new GUIStyle(GUI.skin.box);
                panelStyle.normal.background =
                    Texture2DExtensions.MakeTexture(1, 1, new Color(0.94f, 0.94f, 0.94f, 1f));
            }
        }

        /// <summary>
        /// Parse the path for the menu item and add any group items required
        /// </summary>
        /// <param name="menuItem"></param>
        private void AddHierarchy(GUIGenericMenuItem menuItem)
        {
            if (menuItem.Path.Trim() == string.Empty)
                return;

            string[] splits = menuItem.Path.Split(new char[] { '/' });
            
            for (int i = 0; i < splits.Length; i++)
            {
                string currentPath = string.Empty;

                for (int c = 0; c < i; c++)
                {
                    if (!string.IsNullOrEmpty(currentPath))
                        currentPath += "/";
                    currentPath += splits[c];
                }

                Internal_AddMenuItem(new GUIGroupMenuItem(currentPath,
                    new GUIContent(splits[i]), AddPathToActive));
            }
        }

        /// <summary>
        /// Add a menu item to the appropriate list stored within the dictionary
        /// </summary>
        /// <param name="menuItem">The item to be added</param>
        private void Internal_AddMenuItem(GUIGenericMenuItem menuItem)
        {
            List<GUIGenericMenuItem> existing;

            if (!groupedMenuItems.TryGetValue(menuItem.Path, out existing))
            {
                existing = new List<GUIGenericMenuItem>();
                groupedMenuItems.Add(menuItem.Path, existing);
            }
            
            GUIGenericMenuItem existingItem = existing.Find((x) =>
               !string.IsNullOrEmpty(x.Display.text) && x.Display.text == menuItem.Display.text);

            if (existingItem != null)
                return;

            existing.Add(menuItem);
        }

        /// <summary>
        /// Callback for group menu items to handle adding the group
        /// to the collection of selected items
        /// </summary>
        /// <param name="groupObject">The group menu item to be added to the currently
        /// selected</param>
        private void AddPathToActive(GUIMenuItemParamEventArgs args)
        {
            GUIGroupMenuItem groupItem = args.SelectedItem as GUIGroupMenuItem;

            if (groupItem == null)
                return;
            
            selectedPaths.AddUnique(groupItem);
        }

        /// <summary>
        /// Get the size of the rectangle needed to display all items in the group
        /// </summary>
        /// <param name="groupItems">The items in the group</param>
        /// <returns>The size of the group</returns>
        private Vector2 GetGroupSize(IEnumerable<GUIGenericMenuItem> groupItems)
        {
            float height = 0f;
            float width = 0f;
            
            foreach (GUIGenericMenuItem item in groupItems)
            {
                Vector2 itemSize = item.GetSize();

                height += itemSize.y;

                if (itemSize.x > width)
                    width = itemSize.x;
                
            }

            return new Vector2(Mathf.Max(width, MinWidth), height);
        }

        /// <summary>
        /// Adjusts the provided Rect to a valid position that does
        /// not sit nearly off the screen
        /// </summary>
        /// <param name="rect">The rect to adjust if passing the screen size</param>
        /// <returns>The unmodified rect if within bounds, otherwise
        /// a rect adjusted to be within bounds</returns>
        private Rect GetSafeDisplayRect(Rect parentRect, Rect drawRect)
        {
            drawRect.x = parentRect.xMax - 5f;

            return drawRect;
        }

        /// <summary>
        /// Determines if the item can be displayed
        /// </summary>
        /// <param name="groupPath">The path of the group to evaluate</param>
        /// <param name="items">The items that will be displayed</param>
        /// <returns>True if the group can be displayed, false if not</returns>
        private bool CanDisplay(string groupPath,
            IEnumerable<GUIGenericMenuItem> items)
        {
            if (selectedPaths.Count < 1)
                return true;

            GUIGroupMenuItem deepestGroup = selectedPaths[selectedPaths.Count - 1];

            if (!items.Contains(deepestGroup))
                return true;
            
            if (deepestGroup.PositionRect.Contains(Event.current.mousePosition) ||
                deepestRect.Contains(Event.current.mousePosition))
                return true;

            if (!canRemove)
                return true;

            selectedPaths.RemoveAt(selectedPaths.Count - 1);
            canRemove = false;
            GUIUtility.ExitGUI();
            return false;
        }
    }
}