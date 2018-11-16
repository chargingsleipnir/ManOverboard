using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Class used to help with defining and handling a zoomable area 
    /// within the editor window
    /// </summary>
    public class ZoomableAreaHelper
    {
        /// <summary>
        /// The min value for any zoom property
        /// </summary>
        private const float ABSOLUTE_MIN_ZOOM = 0.001f;

        private float minZoom = 0.3f;

        public float MinZoom
        {
            get { return minZoom; }
            set
            {
                minZoom = value;
                ClampZooms();
            }
        }


        private float maxZoom = 1.5f;

        public float MaxZoom
        {
            get { return maxZoom; }
            set
            {
                maxZoom = value;
                ClampZooms();
            }
        }

        private float currentZoom = 1f;

        public float CurrentZoom
        {
            get { return currentZoom; }
            set
            {
                currentZoom = value;
                ClampZooms();
            }
        }

        public Vector2 CurrentPanOffset { get; set; }

        public bool AllowPanning { get; set; }

        public bool AllowZooming { get; set; }

        public Rect ZoomArea { get; set; }
        
        public ZoomableAreaHelper()
        {
            AllowPanning = true;
            AllowZooming = true;
        }

        /// <summary>
        /// Ensures that all zoom-related values are valid
        /// </summary>
        private void ClampZooms()
        {
            if (MinZoom < ABSOLUTE_MIN_ZOOM)
                minZoom = ABSOLUTE_MIN_ZOOM;

            if (MaxZoom < ABSOLUTE_MIN_ZOOM)
                maxZoom = ABSOLUTE_MIN_ZOOM;

            if (MinZoom > MaxZoom)
                maxZoom = minZoom;

            if (MaxZoom < MinZoom)
                minZoom = maxZoom;

            currentZoom = Mathf.Clamp(CurrentZoom, MinZoom, MaxZoom);
        }
        
        public Rect BeginZoomArea(Rect zoomArea)
        {
            ZoomArea = GUIExtensions.BeginZoomableArea(CurrentZoom, zoomArea);
            return ZoomArea;
        }

        public void EndZoomArea()
        {
            GUIExtensions.EndZoomableArea();
        }

        public void PanHorizontally(float value)
        {
            CurrentPanOffset = new Vector2(CurrentPanOffset.x + value, CurrentPanOffset.y);
        }

        public void PanVertically(float value)
        {
            CurrentPanOffset = new Vector2(CurrentPanOffset.x, CurrentPanOffset.y + value);
        }

        public void HandleEvents()
        {
            if (Event.current.type == EventType.ScrollWheel)
            {
                if (!AllowZooming)
                    return;

                Vector2 delta = Event.current.delta;
                float zoomDelta = -delta.y / 150.0f;
                CurrentZoom += zoomDelta;
                
                Event.current.Use();
            }
            else if (Event.current.type == EventType.MouseDrag &&
                ((Event.current.button == 0 && Event.current.modifiers == EventModifiers.Alt) ||
                Event.current.button == 2))
            {
                if (!AllowPanning)
                    return;

                Vector2 delta = Event.current.delta;
                delta /= CurrentZoom;

                CurrentPanOffset += delta;

                Event.current.Use();
            }
        }

    }
}