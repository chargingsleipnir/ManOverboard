using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Interface for editors to implement when they look
    /// to render within a single area (such as in a reorderable list)
    /// </summary>
    public interface IInlineEditor
    {
        /// <summary>
        /// The height the editor requires for rendering inline
        /// </summary>
        /// <returns>The desired height</returns>
        float GetInlineEditorHeight();

        /// <summary>
        /// Rendering logic to render inline
        /// </summary>
        void OnInlineEditorGUI(Rect drawArea);
    }
}