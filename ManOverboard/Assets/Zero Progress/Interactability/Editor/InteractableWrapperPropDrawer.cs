using UnityEditor;
using ZeroProgress.Common.Editors;

namespace ZeroProgress.Interactions.Editors
{
    [CustomPropertyDrawer(typeof(IInteractableReference))]
    public class InteractableWrapperPropDrawer : InterfaceReferencePropDrawer<IInteractableReference, IInteractable>
    {        
    }
}