using UnityEngine;

namespace ZeroProgress.Common.Cameras
{
    /// <summary>
    /// Taken from the KinematicCharacterController v2.0 from Unity Asset Store and
    /// slightly modified
    /// 
    /// Has the camera move around a specified target while taking into account nearby obstacles
    /// </summary>
    public class OrbitCamera : MonoBehaviour
    {
        [Header("Framing")]
        [Tooltip("The camera to manipulate")]
        public Camera Camera;
        [Tooltip("An offset used to position the camera relative to the target (i.e. framing the target)")]
        public Vector2 FollowTransformFraming = new Vector2(0f, 0f);
        [Tooltip("How quick the camera snaps to the follow target")]
        public float FollowingSharpness = 30f;

        [Header("Distance")]
        [Tooltip("The starting/default distance that the camera should be to the target")]
        public float DefaultDistance = 6f;
        [Tooltip("The closest the camera can get to the target")]
        public float MinDistance = 2f;
        [Tooltip("The farthest the camera can be from the target")]
        public float MaxDistance = 10f;
        [Tooltip("How quickly the distance to the target can be modified")]
        public float DistanceMovementSpeed = 10f;
        [Tooltip("How quickly the distance component snaps to the desired value")]
        public float DistanceMovementSharpness = 10f;

        [Header("Rotation")]
        [Tooltip("True to invert input received for the X Axis")]
        public bool InvertX = false;
        [Tooltip("True to invert input received for the Y Axis")]
        public bool InvertY = false;
        [Range(-90f, 90f)]
        [Tooltip("The starting angle on the vertical axis")]
        public float DefaultVerticalAngle = 20f;
        [Range(-90f, 90f)]
        [Tooltip("The minimum angle allowed on the vertical axis")]
        public float MinVerticalAngle = -80f;
        [Range(-90f, 90f)]
        [Tooltip("The maximum angle allowed on the vertical axis")]
        public float MaxVerticalAngle = 80f;
        [Tooltip("How fast the rotation can move")]
        public float RotationSpeed = 10f;
        [Tooltip("How quickly the rotation snaps to the desired rotation")]
        public float RotationSharpness = 30f;

        [Header("Obstruction")]
        [Tooltip("The radius to use when detecting obsctacles near the camera")]
        public float ObstructionCheckRadius = 0.5f;
        [Tooltip("The layers that contain possible camera obstructions")]
        public LayerMask ObstructionLayers = -1;
        [Tooltip("How quickly the camera snaps to a safe position")]
        public float ObstructionSharpness = 10000f;

        /// <summary>
        /// Cached transform reference
        /// </summary>
        public Transform Transform { get; private set; }
        /// <summary>
        /// Vector representing the current plane of motion
        /// </summary>
        public Vector3 PlanarDirection { get; private set; }
        /// <summary>
        /// The transform of the target object
        /// </summary>
        public Transform FollowTransform { get; set; }
        /// <summary>
        /// Any colliders that the camera should ignore when looking for obstructions
        /// </summary>
        public Collider[] IgnoredColliders { get; set; }
        /// <summary>
        /// The desired distance
        /// </summary>
        public float TargetDistance { get; set; }

        [Tooltip("True to use global up for calculations, false to use local up (for cases of unique gravity)")]
        public bool UseGlobalUp = false;

        /// <summary>
        /// How large to make the buffer for colliders
        /// </summary>
        private const int MaxObstructions = 32;

        private bool _distanceIsObstructed;
        private float _currentDistance;
        private float _targetVerticalAngle;
        private RaycastHit _obstructionHit;
        private int _obstructionCount;
        private RaycastHit[] _obstructions = new RaycastHit[MaxObstructions];
        private float _obstructionTime;
        private Vector3 _currentFollowPosition;

        void OnValidate()
        {
            DefaultDistance = Mathf.Clamp(DefaultDistance, MinDistance, MaxDistance);
            DefaultVerticalAngle = Mathf.Clamp(DefaultVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
        }

        void Awake()
        {
            Transform = this.transform;

            _currentDistance = DefaultDistance;
            TargetDistance = _currentDistance;

            _targetVerticalAngle = 0f;

            PlanarDirection = Vector3.forward;
        }
        
        /// <summary>
        /// Sets the transform that this camera should follow
        /// </summary>
        /// <param name="followTransform">The transform to keep track of</param>
        public void SetFollowTransform(Transform followTransform)
        {
            FollowTransform = followTransform;
            PlanarDirection = followTransform.forward;
            _currentFollowPosition = FollowTransform.position;
        }

        /// <summary>
        /// Update the orbit camera with new inputs
        /// </summary>
        /// <param name="deltaTime">The current frames' delta time</param>
        /// <param name="zoomInput">The amount to zoom in/out</param>
        /// <param name="rotationInput">The amount to rotate in 3 dimensions</param>
        public void UpdateWithInput(float deltaTime, float zoomInput, Vector3 rotationInput)
        {
            if (!FollowTransform)
                return;

            // Invert rotation inputs where necessary
            if (InvertX)
            {
                rotationInput.x *= -1f;
            }
            if (InvertY)
            {
                rotationInput.y *= -1f;
            }

            Vector3 transformUpVector = FollowTransform.up;

            if (UseGlobalUp)
                transformUpVector = Vector3.up;

            // Process rotation input
            Quaternion rotationFromInput = Quaternion.Euler(transformUpVector * (rotationInput.x * RotationSpeed));
            PlanarDirection = rotationFromInput * PlanarDirection;
            PlanarDirection = Vector3.Cross(transformUpVector, Vector3.Cross(PlanarDirection, transformUpVector));
            _targetVerticalAngle -= (rotationInput.y * RotationSpeed);
            _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, MinVerticalAngle, MaxVerticalAngle);

            // Process distance input
            if (_distanceIsObstructed && Mathf.Abs(zoomInput) > 0f)
            {
                TargetDistance = _currentDistance;
            }
            TargetDistance += zoomInput * DistanceMovementSpeed;
            TargetDistance = Mathf.Clamp(TargetDistance, MinDistance, MaxDistance);

            // Find the smoothed follow position
            _currentFollowPosition = Vector3.Lerp(_currentFollowPosition, FollowTransform.position, 1f - Mathf.Exp(-FollowingSharpness * deltaTime));

            if (PlanarDirection == Vector3.zero)
            {
                PlanarDirection = new Vector3(0f, 0f, .1f);
                Debug.Log("Corrected Planar");
            }
            // Calculate smoothed rotation
            Quaternion planarRot = Quaternion.LookRotation(PlanarDirection, transformUpVector);
            Quaternion verticalRot = Quaternion.Euler(_targetVerticalAngle, 0, 0);
            Quaternion targetRotation = Quaternion.Slerp(Transform.rotation, planarRot * verticalRot, 1f - Mathf.Exp(-RotationSharpness * deltaTime));

            // Apply rotation
            Transform.rotation = targetRotation;

            // Handle obstructions
            {
                RaycastHit closestHit = new RaycastHit();
                closestHit.distance = Mathf.Infinity;
                _obstructionCount = Physics.SphereCastNonAlloc(_currentFollowPosition, ObstructionCheckRadius, -Transform.forward, _obstructions, TargetDistance, ObstructionLayers, QueryTriggerInteraction.Ignore);
                for (int i = 0; i < _obstructionCount; i++)
                {
                    bool isIgnored = false;
                    for (int j = 0; j < IgnoredColliders.Length; j++)
                    {
                        if (IgnoredColliders[j] == _obstructions[i].collider)
                        {
                            isIgnored = true;
                            break;
                        }
                    }

                    if (!isIgnored && _obstructions[i].distance < closestHit.distance && _obstructions[i].distance > 0)
                    {
                        closestHit = _obstructions[i];
                    }
                }

                // If obstructions detected
                if (closestHit.distance < Mathf.Infinity)
                {
                    _distanceIsObstructed = true;
                    _currentDistance = Mathf.Lerp(_currentDistance, closestHit.distance, 1 - Mathf.Exp(-ObstructionSharpness * deltaTime));
                }
                // If no obstruction
                else
                {
                    _distanceIsObstructed = false;
                    _currentDistance = Mathf.Lerp(_currentDistance, TargetDistance, 1 - Mathf.Exp(-DistanceMovementSharpness * deltaTime));
                }
            }

            // Find the smoothed camera orbit position
            Vector3 targetPosition = _currentFollowPosition - ((targetRotation * Vector3.forward) * _currentDistance);

            // Handle framing
            targetPosition += Transform.right * FollowTransformFraming.x;
            targetPosition += Transform.up * FollowTransformFraming.y;

            // Apply position
            Transform.position = targetPosition;

        }
    }
}