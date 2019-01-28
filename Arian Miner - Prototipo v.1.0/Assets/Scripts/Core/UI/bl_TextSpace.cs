using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


namespace Speedbox
{
    [AddComponentMenu("UI/Effects/Letter Spacing", 14)]
#if UNITY_5_2 || UNITY_5_3_OR_NEWER
    public class bl_TextSpace : BaseMeshEffect
#else
    public class LetterSpacing : BaseVertexEffect
#endif
    {
        [SerializeField,Range(0,100)]
        private float m_spacing = 0f;

        protected bl_TextSpace() { }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            spacing = m_spacing;
            base.OnValidate();
        }
#endif

        private Text text
        {
            get { return this.gameObject.GetComponent<Text>(); }
        }

        private string[] GetLines()
        {
            IList<UILineInfo> lineInfos = text.cachedTextGenerator.lines;
            
            string[] lines = new string[lineInfos.Count];

            for (int i = 0; i < lineInfos.Count; i++)
            {
                if ((i + 1) < lineInfos.Count)
                {
                    int length = (lineInfos[i + 1].startCharIdx - 1) - lineInfos[i].startCharIdx;
                    lines[i] = this.text.text.Substring(lineInfos[i].startCharIdx,length);
                }
                else
                {
                    lines[i] = this.text.text.Substring(lineInfos[i].startCharIdx);
                }
            }
            return lines;
        }

        public float spacing
        {
            get { return m_spacing; }
            set
            {
                if (m_spacing == value) return;
                m_spacing = value;
                if (graphic != null) graphic.SetVerticesDirty();
            }
        }
#if UNITY_5_2 || UNITY_5_3_OR_NEWER
        public void ModifyVertices(List<UIVertex> verts)
#else
    public override void ModifyVertices(List<UIVertex> verts)
#endif
        {
            if (!IsActive()) return;

            Text text = GetComponent<Text>();
            if (text == null)
            {
                Debug.LogWarning("LetterSpacing: Missing Text component");
                return;
            }

            string[] lines = GetLines();

            Vector3 pos;
            float letterOffset = spacing * (float)text.fontSize / 100f;
            float alignmentFactor = 0;
            int glyphIdx = 0;
            switch (text.alignment)
            {
                case TextAnchor.LowerLeft:
                case TextAnchor.MiddleLeft:
                case TextAnchor.UpperLeft:
                    alignmentFactor = 0f;
                    break;

                case TextAnchor.LowerCenter:
                case TextAnchor.MiddleCenter:
                case TextAnchor.UpperCenter:
                    alignmentFactor = 0.5f;
                    break;

                case TextAnchor.LowerRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.UpperRight:
                    alignmentFactor = 1f;
                    break;
            }
            for (int lineIdx = 0; lineIdx < lines.Length; lineIdx++)
            {
                string line = lines[lineIdx];
                float lineOffset = (line.Length - 1) * letterOffset * alignmentFactor;

                for (int charIdx = 0; charIdx < line.Length; charIdx++)
                {
                    int idx1 = glyphIdx * 6 + 0;
                    int idx2 = glyphIdx * 6 + 1;
                    int idx3 = glyphIdx * 6 + 2;
                    int idx4 = glyphIdx * 6 + 3;
                    int idx5 = glyphIdx * 6 + 4;
                    int idx6 = glyphIdx * 6 + 5;

                    // Check for truncated text (doesn't generate verts for all characters)
                    if (idx4 > verts.Count - 1) return;

                    UIVertex vert1 = verts[idx1];
                    UIVertex vert2 = verts[idx2];
                    UIVertex vert3 = verts[idx3];
                    UIVertex vert4 = verts[idx4];
                    UIVertex vert5 = verts[idx5];
                    UIVertex vert6 = verts[idx6];

                    pos = Vector3.right * (letterOffset * charIdx - lineOffset);

                    vert1.position += pos;
                    vert2.position += pos;
                    vert3.position += pos;
                    vert4.position += pos;
                    vert5.position += pos;
                    vert6.position += pos;

                    verts[idx1] = vert1;
                    verts[idx2] = vert2;
                    verts[idx3] = vert3;
                    verts[idx4] = vert4;
                    verts[idx5] = vert5;
                    verts[idx6] = vert6;

                    glyphIdx++;
                }

                // Offset for carriage return character that still generates verts
                glyphIdx++;
            }
        }

#if UNITY_5_2
        public override void ModifyMesh(Mesh mesh)
        {
            if (!this.IsActive())
                return;
 
            List<UIVertex> list = new List<UIVertex>();
            using (VertexHelper vertexHelper = new VertexHelper(mesh))
            {
                vertexHelper.GetUIVertexStream(list);
            }
 
            ModifyVertices(list);  // calls the old ModifyVertices which was used on pre 5.2
 
            using (VertexHelper vertexHelper2 = new VertexHelper())
            {
                vertexHelper2.AddUIVertexTriangleStream(list);
                vertexHelper2.FillMesh(mesh);
            }
        }
#elif UNITY_5_3_OR_NEWER
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!this.IsActive())
                return;

            List<UIVertex> vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);

            ModifyVertices(vertexList);

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);
        }
    }
#endif

}