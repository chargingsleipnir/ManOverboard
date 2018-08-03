using System;
using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Class that manages acknowledging the receiving end of a Drag-And-Drop system
    /// </summary>
    public class DragReceiver
    {
        /// <summary>
        /// The function used to determine if the item is valid to be dropped in the
        /// rect that the receiver is defined by
        /// </summary>
        protected Func<UnityEngine.Object[], String[], bool> isDragItemValidFunc;

        /// <summary>
        /// Storage for the last result from the validation function so that the 
        /// validator doesn't need to be constantly executed
        /// </summary>
        protected bool lastValidationResult = false;

        /// <summary>
        /// Store the last mouse position that DragUpdate provided, since DragExited 
        /// gives screen-space coordinates while DragUpdate provides editor coordinates
        /// </summary>
        protected Vector2 lastMousePosition;

        /// <summary>
        /// The indicator to be shown over the dragged object when it is held
        /// over the rectangle represented by this receiver (the mouse icon)
        /// </summary>
        protected DragAndDropVisualMode validDragDropIndicator;

        private bool isInReceiver = false;

        /// <summary>
        /// True if the currently dragged item is within the bounds that this
        /// receiver is defined by, false if not
        /// </summary>
        public bool IsInReceiver
        {
            get { return isInReceiver; }
            protected set
            {
                if (value == isInReceiver)
                    return;

                isInReceiver = value;

                OnIsInReceiverChanged(isInReceiver);
            }
        }

        /// <summary>
        /// The rectangle that defines the space that is defined as 'droppable'
        /// </summary>
        public Rect ReceiverBox;

        /// <summary>
        /// Event fired when a valid item has been dropped on the rectangle
        /// </summary>
        public event EventHandler OnDragComplete;

        /// <summary>
        /// Event fired whenever the drag is considered to have exited the editor
        /// or when the item has been dropped on an invalid location
        /// </summary>
        public event EventHandler OnDragExited;

        /// <summary>
        /// Event fired when an item is dragged over this receiver, but has not
        /// been dropped yet
        /// </summary>
        public event EventHandler OnReceiverEntered;

        /// <summary>
        /// Event fired when an item is dragged out of this receiver
        /// </summary>
        public event EventHandler OnReceiverExited;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Validator"> The function used to determine if the item is valid to be dropped in the
        /// rect that the receiver is defined by</param>
        /// <param name="ValidDropIndicator">The indicator to be shown over the dragged object when it is held
        /// over the rectangle represented by this receiver (the mouse icon)</param>
        public DragReceiver(Func<UnityEngine.Object[], String[], bool> Validator,
            DragAndDropVisualMode ValidDropIndicator)
        {
            isDragItemValidFunc = Validator;
            validDragDropIndicator = ValidDropIndicator;
        }

        /// <summary>
        /// Updates the receiver. If not called in OnInspectorGUI, (or at least in DragUpdated, DragPerform and DragExited event handlers)
        /// will not update and will not fire the required events
        /// </summary>
        public virtual void Update()
        {
            switch (Event.current.type)
            {
                case EventType.DragUpdated:

                    lastMousePosition = Event.current.mousePosition;

                    IsInReceiver = ReceiverBox.Contains(lastMousePosition);

                    HandleDragUpdate();

                    break;

                case EventType.DragPerform:

                    HandleDragPerform();

                    break;

                case EventType.DragExited:

                    HandleDragExited();

                    break;
            }
        }

        /// <summary>
        /// Handles the actions to be taken when the drag is updating
        /// </summary>
        protected virtual void HandleDragUpdate()
        {
            if (IsInReceiver)
            {
                if (lastValidationResult)
                    DragAndDrop.visualMode = validDragDropIndicator;
                else
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

                Event.current.Use();
            }
        }

        /// <summary>
        /// Handles the actions to be taken when the drag and drop has been successfully completed
        /// </summary>
        protected virtual void HandleDragPerform()
        {
            if (IsInReceiver && lastValidationResult)
            {
                Event.current.Use();
                RaiseEvent(OnDragComplete);
                RaiseEvent(OnReceiverExited);
                RaiseEvent(OnDragExited);
            }
        }

        /// <summary>
        /// Handles the actions to be taken when the drag and drop has exited/aborted
        /// </summary>
        protected virtual void HandleDragExited()
        {
            if (IsInReceiver)
            {
                if (lastValidationResult)
                {
                    DragAndDrop.AcceptDrag();
                    Event.current.Use();
                }
                else
                {
                    RaiseEvent(OnReceiverExited);
                    RaiseEvent(OnDragExited);
                }
            }
            else
                RaiseEvent(OnDragExited);
        }

        /// <summary>
        /// Safely raises the specified event (null check)
        /// </summary>
        /// <param name="Handler">The handler to raise</param>
        protected void RaiseEvent(EventHandler Handler)
        {
            if (Handler != null)
                Handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// Action taken whenever the IsInReceiver property has had its value updated
        /// </summary>
        /// <param name="NewValue">The new value of IsInReceiver</param>
        protected virtual void OnIsInReceiverChanged(bool NewValue)
        {
            if (NewValue)
            {
                lastValidationResult = isDragItemValidFunc(DragAndDrop.objectReferences, DragAndDrop.paths);

                if (lastValidationResult)
                    RaiseEvent(OnReceiverEntered);
            }
            else
            {
                if (lastValidationResult)
                    RaiseEvent(OnReceiverExited);

                lastValidationResult = false;
            }
        }

        /// <summary>
        /// A validation function that returns true
        /// </summary>
        /// <param name="ObjectRefs">The object references being dragged</param>
        /// <param name="ObjectPaths">The paths of the references being dragged</param>
        /// <returns>True</returns>
        public static bool IsValidFunc(UnityEngine.Object[] ObjectRefs, String[] ObjectPaths)
        {
            return true;
        }
    }
}