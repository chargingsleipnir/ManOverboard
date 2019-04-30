using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.Common2D
{
    public struct TilemapSizeDetails
    {
        public Vector3 GridSize;
        public Vector3 CellSize;

        public TilemapSizeDetails(Vector3 gridSize, Vector3 cellSize)
        {
            GridSize = gridSize;
            CellSize = cellSize;
        }

        /// <summary>
        /// Retrieves the bounds that define a box colliders boundary over a tilemap/grid
        /// </summary>
        /// <returns>The bounds that define the area of the tilemap</returns>
        public Bounds GetColliderBounds()
        {
            Vector3 colliderSize = GridSize.SafeDivideComponents(CellSize);

            Vector3 colliderCenter = colliderSize;
            colliderCenter.y = colliderSize.y;
            colliderCenter.x -= CellSize.x * 2f;

            colliderCenter *= 0.5f;
            colliderCenter.x++;

            return new Bounds(colliderCenter, colliderSize);            
        }
    }
}