using UnityEngine;

namespace ZeroProgress.Common
{
    public class SimpleBoundsEnforcer : MonoBehaviour, IBoundsEnforcer
    {
        public Bounds DefinedBounds;

        public bool IsInBounds(Vector3 Point)
        {
            return DefinedBounds.Contains(Point);
        }

        public Vector3 SetInBounds(Vector3 Point)
        {
            return DefinedBounds.ClosestPoint(Point);
        }
    }
}