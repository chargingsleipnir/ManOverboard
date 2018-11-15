using System;
using UnityEditor;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.NodeEditor;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Node that represents the scene information
    /// </summary>
    [Serializable]
    public class SceneNode : Node
    {
        private const float MIN_NODE_WIDTH = 300f;
        
        private static GUIStyle entryNodeStyle;

        private const string entryText = "Entry";
        
        /// <summary>
        /// The id of the scene this node represents
        /// </summary>
        public int SceneId;
        
        /// <summary>
        /// The scene model information associated with this node
        /// </summary>
        public SceneModel SceneInfo
        {
            get { return NodeData as SceneModel; }
            set
            {
                NodeData = value;

                if (value != null)
                    SceneId = value.SceneId;
            }
        }

        /// <summary>
        /// True if this node represents the scene that the game starts on, false
        /// if not
        /// </summary>
        public bool IsEntryScene { get; set; }

        /// <summary>
        /// True if this node is the active scene
        /// </summary>
        public bool IsActiveScene { get; set; }

        /// <summary>
        /// The name of the scene to be displayed
        /// </summary>
        private GUIContent sceneName;

        private GUIStyle nodeStyle;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">Position of the node</param>
        /// <param name="model">The scene model information to be stored with this node</param>
        public SceneNode(Vector2 position, SceneModel model) 
            : base(position, model)
        {
            SceneId = model.SceneId;
        }

        /// <summary>
        /// Draws the scene node
        /// </summary>
        /// <param name="nodeOffset">The offset caused by panning</param>
        public override void Draw(Vector2 nodeOffset)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            InitializeNode();

            Rect nodeRect = GetRenderRect(nodeOffset);

            Color color = Color.white;

            GUIContent display = sceneName;

            if (SceneId == SceneManagerController.ANY_SCENE_ID)
                color = SceneManagerResources.AnySceneColor;

            if (IsEntryScene)
            {
                display = new GUIContent(entryText + ":" + Environment.NewLine + sceneName.text);
                color = SceneManagerResources.EntrySceneColor;
            }

            if (IsActiveScene)
                color = SceneManagerResources.ActiveSceneColor;          

            GUIExtensions.BeginColouring(color);
            nodeStyle.Draw(nodeRect, display, true, IsSelected(), true, false);
            GUIExtensions.EndColouring();
        }

        /// <summary>
        /// Prepares all render-related information for the node
        /// </summary>
        private void InitializeNode()
        {
            nodeStyle = GetNodeStyle();
            
            if (sceneName == null || SceneInfo.SceneName != sceneName.text)
            {
                string nicifiedSceneName = ObjectNames.NicifyVariableName(SceneInfo.SceneName);

                sceneName = new GUIContent(nicifiedSceneName);

                NodeRect.size = nodeStyle.CalcSize(sceneName);
            }
        }

        protected override GUIStyle GetNodeStyle()
        {
            if (SceneInfo.IsUnlocked)
                return GUI.skin.GetCustomStyle(SceneManagerResources.SceneNodeOpenStyleName);
            else
                return GUI.skin.GetCustomStyle(SceneManagerResources.SceneNodeClosedStyleName);
        }

        public override Rect GetNodeRect(Vector2 offset)
        {
            // Offset the rectangle to not require full containment for selection
            Rect rect = new Rect(NodeRect);

            rect.position += offset;

            rect.y -= (rect.height * 0.5f);
            rect.x -= (rect.width * 0.5f);

            rect.size = rect.size * 0.75f;
            rect.x += rect.width * 0.25f;
            rect.y += rect.height * 0.25f;

            return rect;
        }

        private Rect GetRenderRect(Vector2 offset)
        {
            // Render in the correct location (done separately from the collision rect)
            Rect rect = new Rect(NodeRect);
            rect.position += offset - (rect.size * 0.5f);
            
            return rect;
        }
    }
}