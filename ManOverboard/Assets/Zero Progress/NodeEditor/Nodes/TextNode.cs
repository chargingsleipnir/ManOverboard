using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.NodeEditor
{
    public class TextNode : Node
    {
        public string NodeText = string.Empty;

        public bool CalcSize = true;

        public bool RestrictSize = true;

        public Vector2 WidthRestriction = new Vector2(150f, 300f);
        public Vector2 HeightRestriction = new Vector2(0f, 120f);

        public Color NormalColor = Color.blue;
        public Color SelectedColor = Color.green;

        protected GUIContent nodeTextContent = null;

        public TextNode(Vector2 position, string nodeText, 
            System.Object nodeData = null, GUIGenericMenu contextMenu = null)
            : this(new Rect(position, Vector2.one), nodeText, nodeData, contextMenu)
        {
        }

        public TextNode(Rect nodeRect, string nodeText, 
            System.Object nodeData = null, GUIGenericMenu contextMenu = null)
            : base(nodeRect, nodeData, contextMenu)
        {
            NodeText = nodeText;
        }

        public override void Draw(Vector2 nodeOffset)
        {
            if (Event.current.type != EventType.Repaint)
                return;

            GUIStyle nodeStyle = GetNodeStyle();

            InitializeGUIContent(nodeStyle);

            Rect renderRect = GetNodeRect(nodeOffset);

            Color drawColor = IsSelected() ? SelectedColor : NormalColor;

            GUIExtensions.ColouredBox(renderRect, drawColor, nodeTextContent, nodeStyle);
        }

        protected void InitializeGUIContent(GUIStyle nodeStyle)
        {
            if (CalcSize &&
                (nodeTextContent == null || nodeTextContent.text != NodeText))
            {
                nodeTextContent = new GUIContent(NodeText);

                NodeRect.size = nodeStyle.CalcSize(nodeTextContent);

                if (RestrictSize)
                {
                    NodeRect.width = Mathf.Clamp(NodeRect.width,
                        WidthRestriction.x, WidthRestriction.y);
                    NodeRect.height = Mathf.Clamp(NodeRect.height,
                        HeightRestriction.x, HeightRestriction.y);
                }
                NodeRect.width = Mathf.Max(NodeRect.width, 150f);
            }
        }
    }
}