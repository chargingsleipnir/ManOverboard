using UnityEngine;

namespace ZeroProgress.Common
{
    public class GameEventInvoker : MonoBehaviour
    {
        [SerializeField]
        private GameEvent gameEventToRaise;

        public void RaiseEvent()
        {
            gameEventToRaise.RaiseEvent();
        }
    }
}
