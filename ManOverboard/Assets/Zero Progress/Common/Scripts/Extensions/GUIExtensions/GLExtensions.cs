using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extension methods for the GL class
    /// </summary>
    public static class GLExtensions
    {
        /// <summary>
        /// The material used for lines with no Z depth
        /// </summary>
        private static Material lineZOffMat;

        /// <summary>
        /// Draws a white line
        /// </summary>
        /// <param name="a">Point a</param>
        /// <param name="b">Point b</param>
        public static void DrawGUILineZDepthOff(Vector3 a, Vector3 b)
        {
            DrawGUILineZDepthOff(a, b, Color.white);
        }

        /// <summary>
        /// Draws a line with no z depth with the provided color
        /// </summary>
        /// <param name="a">Point a</param>
        /// <param name="b">Point b</param>
        /// <param name="color">The color to make the line</param>
        public static void DrawGUILineZDepthOff(Vector3 a, Vector3 b, Color color)
        {
            DrawLines(new Vector3[] { a, b }, lineZOffMat, GUI.matrix, color);
        }

        /// <summary>
        /// Draws a line with the provided material and color
        /// </summary>
        /// <param name="a">Point a</param>
        /// <param name="b">Point b</param>
        /// <param name="mat">The material for the line</param>
        /// <param name="color">The color to make the line</param>
        public static void DrawLines(Vector3[] points, 
            Material mat, Matrix4x4 matrix, Color color)
        {
            if (mat == null)
            {
                InitializeMaterials();
                mat = lineZOffMat;
            }

            BeginDrawLines(mat, matrix, color);

            foreach (Vector3 point in points)
            {
                GL.Vertex(point);
            }

            EndDraw();
        }

        /// <summary>
        /// Begins drawing lines with the material that has Z Depth off
        /// using the current GUI.matrix
        /// </summary>
        /// <param name="color">The color to draw the line</param>
        public static void BeginGUIDrawLinesZDepthOff(Color color)
        {
            InitializeMaterials();
            BeginDrawLines(lineZOffMat, GUI.matrix, color);
        }

        /// <summary>
        /// Begin drawing GL lines with the provided material, matrix and color
        /// </summary>
        /// <param name="lineMat">The material to render the lines with</param>
        /// <param name="matrix">The matrix to render with</param>
        /// <param name="color">The color to assign to the material</param>
        public static void BeginDrawLines(Material lineMat, Matrix4x4 matrix, Color color)
        {
            GL.PushMatrix();
            GL.MultMatrix(matrix);

            GL.Begin(GL.LINES);

            lineMat.color = color;

            lineMat.SetPass(0);

            GL.Color(color);
        }

        /// <summary>
        /// Add a vector to the line
        /// </summary>
        /// <param name="point">The point to add</param>
        public static void AddLineVertex(Vector3 point)
        {
            GL.Vertex(point);
        }

        /// <summary>
        /// Finish drawing the lines
        /// </summary>
        public static void EndDraw()
        {
            GL.PopMatrix();
            GL.End();
        }

        /// <summary>
        /// Begins drawing lines with the material that has Z Depth off
        /// using the current GUI.matrix
        /// </summary>
        /// <param name="color">The color to draw the line</param>
        public static void BeginGUIDrawTriangleZDepthOff(Color color)
        {
            InitializeMaterials();
            BeginDrawTriangles(lineZOffMat, GUI.matrix, color);

        }

        /// <summary>
        /// Prepares GL for rendering triangles
        /// </summary>
        /// <param name="triangleMat">The material to be used</param>
        /// <param name="matrix">Matrix for transformations</param>
        /// <param name="color">Color of the triangle</param>
        public static void BeginDrawTriangles(Material triangleMat, Matrix4x4 matrix, Color color)
        {
            GL.PushMatrix();
            GL.MultMatrix(matrix);

            GL.Begin(GL.TRIANGLES);

            triangleMat.color = color;
            triangleMat.SetPass(0);

            GL.Color(color);
        }

        /// <summary>
        /// Helper to draw a single triangle in a GUI context with the Z Depth off
        /// </summary>
        /// <param name="midPoint">The midpoint that defines the center of the triangle</param>
        /// <param name="scale">Scale to apply to the points</param>
        /// <param name="rotation">Rotation to apply to the triangle</param>
        /// <param name="color">Color of the triangle</param>
        public static void DrawGUITriangleZDepthOff(Vector3 midPoint, Vector3 scale, Quaternion rotation, Color color)
        {
            BeginDrawTriangles(lineZOffMat, GUI.matrix, color);
            AddTriangle(midPoint, scale, rotation);
            EndDraw();
        }

        /// <summary>
        /// Defines a simple triangle around the provided midpoint and then
        /// scales and rotates it. Applies each vertex using GL.Vertex
        /// </summary>
        /// <param name="midPoint">The midpoint that defines the center of the triangle</param>
        /// <param name="scale">Scale to apply to the points</param>
        /// <param name="rotation">Rotation to apply to the triangle</param>
        public static void AddTriangle(Vector3 midPoint, Vector3 scale, Quaternion rotation)
        {
            Vector3[] trianglePoints = new Vector3[3]
            {
                    midPoint + (rotation * Vector3.Scale(new Vector2(0.5f, 0.0f), scale)),
                    midPoint + (rotation * Vector3.Scale(new Vector2(0.0f, 0.5f), scale)),
                    midPoint + (rotation * Vector3.Scale(new Vector2(0.0f, -0.5f), scale))
            };

            foreach (Vector3 vector in trianglePoints)
            {
                GL.Vertex(vector);
            }
        }

        /// <summary>
        /// Initialize internal materials
        /// </summary>
        private static void InitializeMaterials()
        {
            if (lineZOffMat == null)
            {
                Shader shader1 = Shader.Find("Unlit/LineZOff");
                lineZOffMat = new Material(shader1);
            }
        }
    }
}