namespace ZeroProgress.NodeEditor
{
    /// <summary>
    /// Base class for Input Modes associated with selection capabilities
    /// </summary>
    public abstract class SelectionInputMode : InputMode
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nodeEditor">Editor to work upon</param>
        /// <param name="priority">When this mode should be processed relative
        /// to other input modes. Lower number == processed first</param>
        public SelectionInputMode(NodeEditor nodeEditor, int priority = 0) 
            : base(nodeEditor, priority)
        {
        }

        /// <summary>
        /// Helper for children to check if activation should take place
        /// </summary>
        protected override void CheckActivation()
        {
            if (IsActivated)
            {
                if (!NodeEditor.SelectionAllowed)
                    Deactivate();
            }

            if (!NodeEditor.SelectionAllowed)
                return;

            if (ShouldActivate())
                Activate();
        }
    }
}
