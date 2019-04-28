using UnityEngine.Tilemaps;

namespace ZeroProgress.Common2D
{
    /// <summary>
    /// Extensions for the Unity Tilemap class
    /// </summary>
    public static class TilemapExtensions
    {
        /// <summary>
        /// Retrieves the sizing information of this tilemap
        /// </summary>
        /// <param name="thisTilemap">The tilemap to get the size information of</param>
        /// <returns>The populated size details</returns>
        public static TilemapSizeDetails GetSizeDetails(this Tilemap thisTilemap)
        {
            return new TilemapSizeDetails(thisTilemap.size, thisTilemap.cellSize);
        }
    }
}