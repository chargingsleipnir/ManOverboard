using UnityEngine;

namespace ZeroProgress.Common.Cameras
{
    /// <summary>
    /// Class that acts as the intermediary between Input and the RTS Camera
    /// </summary>
    public class RTSCameraController : MonoBehaviour
    {
        [Tooltip("RTS Camera Logic to control")]
        public RTSCamera CameraToControl;

        [Tooltip("True to allow panning with input buttons")]
        public bool AllowButtonPanning = true;
        [Tooltip("Speed to be applied to horizontal input button")]
        public float HorizontalButtonSpeed = 1f;
        [Tooltip("Speed to be applied to vertical input button")]
        public float VerticalButtonSpeed = 1f;

        [Tooltip("True to allow panning when the cursor is at the screen edge")]
        public bool AllowScreenEdgePanning = true;
        [Tooltip("Speed to be applied to horizontal screen edge recognition")]
        public float HorizontalScreenEdgePanSpeed = 0.5f;
        [Tooltip("Speed to be applied to vertical screen edge recognition")]
        public float VerticalScreenEdgePanSpeed = 0.5f;

        [Tooltip("Speed applied to zoom inputs")]
        public float ZoomSpeed = 3.0f;

        [Tooltip("Speed applied to rotation along the right axis in relation to the camera to the pivot point")]
        public float RightRotateSpeed = 1f;
        [Tooltip("Speed applied to rotation along the vertical axis in relation to the camera to the pivot point")]
        public float UpRotateSpeed = 1f;

        [Range(0f, 1f)]
        [Tooltip("The point that normalized input must exceed to be considered vertical screen-edge input")]
        public float ScreenVerticalMargin = 0.95f;

        [Range(0f, 1f)]
        [Tooltip("The point that normalized input must exceed to be considered horizontal screen-edge input")]
        public float ScreenHorizontalMargin = 0.95f;

        /// <summary>
        /// The mouse position cached for determining delta movement
        /// </summary>
        private Vector3 startMousePosition;

        /// <summary>
        /// Cache for whether or not the button was part of previously
        /// </summary>
        private bool wasButtonDown = false;
        
        void Start()
        {
            CameraToControl = this.GetComponentIfNull(CameraToControl);            
        }
        
        void Update()
        {
            if (CameraToControl == null)
                return;

            Vector3 motionInput = GetButtonInput() + GetEdgeOfScreenInput();
            CameraToControl.Move(motionInput);

            Vector2 rotationInput = GetRotationInput();
            CameraToControl.Rotate(rotationInput);
        }

        /// <summary>
        /// Retrieves all input related to the button keys (joystick/keyboard)
        /// </summary>
        /// <returns>The vector3 that represents all the mouse input</returns>
        private Vector3 GetButtonInput()
        {
            if (!AllowButtonPanning)
                return Vector3.zero;

            // TODO: Make this configurable through inspector
            float x = Input.GetAxis("Horizontal") * HorizontalButtonSpeed;
            float y = Input.GetAxis("Vertical") * VerticalButtonSpeed;

            float z = Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Retrieves input related to the mouse position in relation to the edge of the screen
        /// </summary>
        /// <returns>The vector3 that represents the input from the edge of the screen</returns>
        private Vector3 GetEdgeOfScreenInput()
        {
            if (!AllowScreenEdgePanning)
                return Vector3.zero;

            float x = 0f;
            float y = 0f;

            if (Input.mousePosition.x <= Screen.width * (1f - ScreenHorizontalMargin))
                x = -1f;

            if (Input.mousePosition.x >= Screen.width * ScreenHorizontalMargin)
                x = 1f;

            if (Input.mousePosition.y <= Screen.height * (1f - ScreenVerticalMargin))
                y = -1f;

            if (Input.mousePosition.y >= Screen.height * ScreenVerticalMargin)
                y = 1f;

            return new Vector3(x * HorizontalScreenEdgePanSpeed, y * VerticalScreenEdgePanSpeed, 0f);
        }

        /// <summary>
        /// Retrieve the input related to the rotation
        /// </summary>
        /// <returns>Vector2 representing the rotation input</returns>
        private Vector2 GetRotationInput()
        {
            if (Input.GetButton("Fire3"))
            {
                if (!wasButtonDown)
                {
                    startMousePosition = Input.mousePosition;
                    wasButtonDown = true;
                }

                Vector2 mouseDelta = new Vector2(startMousePosition.x - Input.mousePosition.x,
                    startMousePosition.y - Input.mousePosition.y);

                mouseDelta.Normalize();
                mouseDelta.x *= RightRotateSpeed;
                mouseDelta.y *= UpRotateSpeed;

                return mouseDelta;
            }
            else
                wasButtonDown = false;

            return Vector2.zero;
        }
    }
}