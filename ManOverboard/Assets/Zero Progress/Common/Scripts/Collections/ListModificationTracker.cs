using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// The supported types of modification that
    /// the ListModificationTracker supports
    /// </summary>
    public enum ListModificationType
    {
        ADD,
        REMOVE
    }

    /// <summary>
    /// This class is intended to track changes that should be applied
    /// to a list after it has completed an iteration
    /// </summary>
    public sealed class ListModificationTracker<ElementType>
    {
        public delegate void ListChangeAction(ElementType element);

        private Stack<KeyValuePair<ElementType, ListModificationType>> modificationsStack = 
            new Stack<KeyValuePair<ElementType, ListModificationType>>();

        /// <summary>
        /// Records a change to the list
        /// </summary>
        /// <param name="item">The item to be modified</param>
        /// <param name="modification">The modification type applied</param>
        public void RecordModification(ElementType item, ListModificationType modification)
        {
            modificationsStack.Push(
                new KeyValuePair<ElementType, ListModificationType>(item, modification));
        }

        /// <summary>
        /// Records an Add modification
        /// </summary>
        /// <param name="item">The item to be added</param>
        public void RecordAddModification(ElementType item)
        {
            RecordModification(item, ListModificationType.ADD);
        }

        /// <summary>
        /// Records an element removal modification
        /// </summary>
        /// <param name="item">The item to remove</param>
        public void RecordRemoveModification(ElementType item)
        {
            RecordModification(item, ListModificationType.REMOVE);
        }

        /// <summary>
        /// Applies the stack to the provided list
        /// </summary>
        /// <param name="list">The list to be modified</param>
        /// <param name="addUnique">True to use AddUnique, false to use Add</param>
        public void ApplyStackToList(IList<ElementType> list, bool addUnique = false)
        {
            ListChangeAction listAddAction = list.Add;

            if (addUnique)
                listAddAction = list.AddUnique;

            ApplyStack(listAddAction, (x) => list.Remove(x));
        }

        /// <summary>
        /// Applies the stack, being provided an Add Item action and
        /// a Remove Item action
        /// </summary>
        /// <param name="addCallback"></param>
        /// <param name="removeCallback"></param>
        public void ApplyStack(ListChangeAction addCallback, ListChangeAction removeCallback)
        {
            while (modificationsStack.Count > 0)
            {
                KeyValuePair<ElementType, ListModificationType> mod =
                    modificationsStack.Pop();

                switch (mod.Value)
                {
                    case ListModificationType.ADD:
                        addCallback(mod.Key);
                        break;
                    case ListModificationType.REMOVE:
                        removeCallback(mod.Key);
                        break;
                }
            }
        }
    }
}