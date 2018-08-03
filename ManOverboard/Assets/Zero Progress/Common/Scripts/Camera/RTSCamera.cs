using UnityEngine;

namespace ZeroProgress.Common.Cameras
{
    /// <summary>
    /// A simple RTS style camera that allows Zoom and Rotation
    /// </summary>
    public class RTSCamera : MonoBehaviour
    {
        public Camera AttachedCamera;

        protected IBoundsEnforcer boundsChecker;

        [SerializeField]
        private Object BoundsCheckerObject;

        public float MinYRot = 5f;

        public float MaxYRot = 55f;

        public float PivotDistance = 10f;

        public float MinZoom = -10f;

        public float MaxZoom = 10f;

        // TODO: Zoom Limits not implemented
        public float CurrentZoom = 0f;

        // Use this for initialization
        void Start()
        {
            AttachedCamera = this.GetComponentIfNull(AttachedCamera);
            
            if (BoundsCheckerObject == null)
            {
                Debug.LogWarning("Bounds checker is null, cannot keep Camera within bounds");
                return;
            }

            boundsChecker = (IBoundsEnforcer)BoundsCheckerObject;
        }

        private void LateUpdate()
        {
            KeepInBounds();
        }

        protected void KeepInBounds()
        {
            if (boundsChecker == null)
                return;

            AttachedCamera.transform.position = boundsChecker.SetInBounds(AttachedCamera.transform.position);
        }

        public void SetBoundsChecker(IBoundsEnforcer BoundsChecker)
        {
            boundsChecker = BoundsChecker;
        }

        public void Move(float Right, float Forward)
        {
            Vector3 cameraForward = Vector3.ProjectOnPlane(AttachedCamera.transform.forward, Vector3.up);

            Vector3 motion = cameraForward.normalized * Forward;
            motion += AttachedCamera.transform.right * Right;

            AttachedCamera.transform.Translate(motion, Space.World);
        }

        public void Move(Vector2 Motion)
        {
            Move(Motion.x, Motion.y);
        }

        public void Move(Vector3 Motion)
        {
            Move(Motion.x, Motion.y);
            Zoom(Motion.z);
        }

        public void Zoom(float Value)
        {
            AttachedCamera.transform.Translate(new Vector3(0f, 0f, Value), Space.Self);
        }

        public void Rotate(float Right, float Up)
        {
            Rotate(new Vector2(Right, Up));
        }

        public void Rotate(Vector2 Motion)
        {
            Vector3 pivot = AttachedCamera.transform.position + (AttachedCamera.transform.forward * PivotDistance);

            Vector3 currentEuler = AttachedCamera.transform.eulerAngles;

            float deltaAngle = Motion.y;

            if (currentEuler.x + Motion.y > MaxYRot)
                deltaAngle = MaxYRot - currentEuler.x;

            if (currentEuler.x + Motion.y < MinYRot)
                deltaAngle = MinYRot - currentEuler.x;

            AttachedCamera.transform.RotateAround(pivot, AttachedCamera.transform.right, deltaAngle);
            AttachedCamera.transform.RotateAround(pivot, Vector3.up, Motion.x);
        }
    }
}