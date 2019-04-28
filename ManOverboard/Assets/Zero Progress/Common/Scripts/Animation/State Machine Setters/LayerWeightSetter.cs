using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Common
{
    public class LayerWeightSetter : StateMachineBehaviour
    {
        [Tooltip("-1 to use the layer this behaviour is a part of, 1 or greater to specify another layer (Base Layer cannot be set)")]
        public int LayerIndex = -1;

        public bool ApplyOnEnter = false;

        public float OnEnterWeight = 1f;

        public bool ApplyOnExit = false;

        public float OnExitWeight = 1f;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            SetLayerWeight(animator, ApplyOnEnter, layerIndex, OnEnterWeight);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetLayerWeight(animator, ApplyOnExit, layerIndex, OnExitWeight);
        }

        private void SetLayerWeight(Animator animator, bool apply, int layerIndex, float weight)
        {
            if (!apply)
                return;

            int layerIndexToSet = LayerIndex;

            if (LayerIndex < 1)
                layerIndexToSet = layerIndex;

            animator.SetLayerWeight(layerIndexToSet, weight);
        }
    }
}