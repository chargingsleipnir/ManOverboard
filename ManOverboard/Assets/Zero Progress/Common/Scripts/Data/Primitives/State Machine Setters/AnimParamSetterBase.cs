using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Base class for a state machine behaviour that sets an animation parameter value
    /// </summary>
    public class AnimParamSetterBase : StateMachineBehaviour
    {
        [Tooltip("The identifier for which animation parameter to be set")]
        public AnimParamReference AnimationParameter;
    }
}