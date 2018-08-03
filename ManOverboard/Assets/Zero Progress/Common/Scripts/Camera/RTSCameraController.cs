using UnityEngine;

namespace ZeroProgress.Common.Cameras
{
    /// <summary>
    /// Class that acts as the intermediary between Input and the RTS Camera
    /// </summary>
    public class RTSCameraController : MonoBehaviour
    {
        public RTSCamera CameraToControl;

        public bool AllowButtonPanning = true;
        public float HorizontalButtonSpeed = 1f;
        public float VerticalButtonSpeed = 1f;

        public bool AllowScreenEdgePanning = true;
        public float HorizontalScreenEdgePanSpeed = 0.5f;
        public float VerticalScreenEdgePanSpeed = 0.5f;

        public float ZoomSpeed = 3.0f;

        public float RightRotateSpeed = 1f;
        public float UpRotateSpeed = 1f;

        [Range(0f, 1f)]
        public float ScreenVerticalMargin = 0.95f;

        [Range(0f, 1f)]
        public float ScreenHorizontalMargin = 0.95f;

        private Vector3 startMousePosition;
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