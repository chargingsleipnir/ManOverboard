using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extensions for the Texture2D class
    /// </summary>
    public static class Texture2DExtensions
    {
        /// <summary>
        /// Creates a simple texture of the specified colour
        /// </summary>
        /// <param name="width">The width of the texture</param>
        /// <param name="height">The height of the texture</param>
        /// <param name="color">The colour that all pixels will be</param>
        /// <returns>The created texture</returns>
        public static Texture2D MakeTexture(int width, int height, Color color)
        {
            Texture2D newtexture = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newtexture.SetPixel(x, y, color);
                }
            }

            newtexture.Apply(false);
            return newtexture;
        }
    }
}