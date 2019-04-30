using UnityEngine;

namespace ZeroProgress.Common.Cameras
{
    /// <summary>
    /// A simple RTS style camera that allows Zoom and Rotation
    /// </summary>
    public class RTSCamera : MonoBehaviour
    {
        [Tooltip("The camera to control")]
        public Camera AttachedCamera;

        protected IBoundsEnforcer boundsChecker;

        [SerializeField]
        [Tooltip("The UnityObject that implements the IBoundsEnforcer interface to keep the camera within a certain area")]
        private Object BoundsCheckerObject;

        [Tooltip("The minimum value allowed on the vertical axis")]
        public float MinYRot = 5f;

        [Tooltip("The maximum value allowed on the vertical axis")]
        public float MaxYRot = 55f;

        [Tooltip("Distance to maintain from a specified pivot point")]
        public float PivotDistance = 10f;

        [Tooltip("The minimum zoom allowed")]
        public float MinZoom = -10f;

        [Tooltip("The maximum zoom allowed")]
        public float MaxZoom = 10f;

        // TODO: Zoom Limits not implemented
        [Tooltip("The current value for the zoom")]
        public float CurrentZoom = 0f;
        
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

        /// <summary>
        /// Used to make sure the camera is within bounds if the bounds checker is implemented
        /// </summary>
        protected void KeepInBounds()
        {
            if (boundsChecker == null)
                return;

            AttachedCamera.transform.position = boundsChecker.SetInBounds(AttachedCamera.transform.position);
        }

        /// <summary>
        /// Define the bounds that the camera must respect
        /// </summary>
        /// <param name="BoundsChecker">The bounds to be used</param>
        public void SetBoundsChecker(IBoundsEnforcer BoundsChecker)
        {
            boundsChecker = BoundsChecker;
        }

        /// <summary>
        /// Moves the camera along the specified axis
        /// </summary>
        /// <param name="Right">The amount to move along the cameras right</param>
        /// <param name="Forward">The amount to move along the cameras forward</param>
        public void Move(float Right, float Forward)
        {
            Vector3 cameraForward = Vector3.ProjectOnPlane(AttachedCamera.transform.forward, Vector3.up);

            Vector3 motion = cameraForward.normalized * Forward;
            motion += AttachedCamera.transform.right * Right;

            AttachedCamera.transform.Translate(motion, Space.World);
        }

        /// <summary>
        /// Moves the camera along the specified axis
        /// </summary>
        /// <param name="Motion">The amount to move on the cameras right and forward</param>
        public void Move(Vector2 Motion)
        {
            Move(Motion.x, Motion.y);
        }

        /// <summary>
        /// Moves and zooms the camera
        /// </summary>
        /// <param name="Motion">Camera motion on x, y and zoom on z</param>
        public void Move(Vector3 Motion)
        {
            Move(Motion.x, Motion.y);
            Zoom(Motion.z);
        }

        /// <summary>
        /// Zooms the camera towards the pivot point
        /// </summary>
        /// <param name="Value">The amount to zoom</param>
        public void Zoom(float Value)
        {
            AttachedCamera.transform.Translate(new Vector3(0f, 0f, Value), Space.Self);
        }

        /// <summary>
        /// Rotation to apply in relation to the pivot point
        /// </summary>
        /// <param name="Right">Rotates the item around the pivot left and right</param>
        /// <param name="Up">Rotates the item around the pivot up and down</param>
        public void Rotate(float Right, float Up)
        {
            Rotate(new Vector2(Right, Up));
        }

        /// <summary>
        /// Rotation to apply in relation to the pivot point
        /// </summary>
        /// <param name="Motion">X is the motion to the left and right in relation to the camera to the pivot, and 
        /// y is the motion to the up and down in relation to the camera to the pivot</param>
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