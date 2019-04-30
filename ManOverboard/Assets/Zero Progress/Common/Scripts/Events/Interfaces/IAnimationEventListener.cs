using UnityEngine;
using UnityEngine.Animations;

namespace ZeroProgress.Common
{
    public class StateMachineEventArgs
    {
        public Animator Animator;
        public AnimatorStateInfo StateInfo;
        public int LayerIndex;
        public AnimatorControllerPlayable ControllerPlayable;

        public StateMachineEventArgs()
        {

        }

        public StateMachineEventArgs(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            : this(animator, stateInfo, layerIndex, AnimatorControllerPlayable.Null)
        {

        }

        public StateMachineEventArgs(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            Animator = animator;
            StateInfo = stateInfo;
            LayerIndex = layerIndex;
            ControllerPlayable = controller;
        }
    }

    /// <summary>
    /// Interface for animation event responders to implement
    /// </summary>
    public interface IAnimationEventListener
    {
        /// <summary>
        /// The 'callback' for when an animation event has been fired
        /// </summary>
        /// <param name="EventLabel">An identifier for the event that's been fired</param>
        /// <param name="args">Animation details related to the fired event</param>
        void ReceiveEvent(string EventLabel, StateMachineEventArgs args);
    }
}