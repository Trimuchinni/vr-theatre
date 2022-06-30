// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using UnityEngine.UI;

namespace JMRSDK.Toolkit.UI.Utilities
{
    /// <summary>
    /// ScaleMeshEffect will store scaling information into UV channel attributes during UI mesh construction. 
    /// </summary>
    [RequireComponent(typeof(RectTransform), typeof(Graphic))]
    public class JMRScaleMeshEffect : BaseMeshEffect
    {
        #region Methods
        /// <summary>
        /// Enforces the canvas to use UV channel attributes.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            // Make sure the canvas is configured to use UV channel attributes for scaling data.
            Canvas j_Canvas = GetComponentInParent<Canvas>();

            if (j_Canvas != null)
            {
                j_Canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord2;
                j_Canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord3;
            }
        }

        /// <summary>
        /// Stores scaling information into UV channel attributes during UI mesh construction.
        /// </summary>
        /// <param name="_vhelper">The vertex helper to populate new vertex data.</param>
        public override void ModifyMesh(VertexHelper _vhelper)
        {
            RectTransform j_RectTransform = transform as RectTransform;

            //Vector 2 xy scale into UV channel 2.
            Vector2 j_Scale = new Vector2(j_RectTransform.rect.width * j_RectTransform.localScale.x,
                                    j_RectTransform.rect.height * j_RectTransform.localScale.y);

            Canvas j_Canvas = GetComponentInParent<Canvas>();

            //z scale into x and a flag indicating this value comes from a ScaleMeshEffect into y into UV channel 3.
            Vector2 zDepth = new Vector2((j_Canvas ? (1.0f / j_Canvas.transform.lossyScale.z) : 1.0f) * j_RectTransform.localScale.z,
                                    -1.0f);

            UIVertex j_Vertex = new UIVertex();

            for (var i = 0; i < _vhelper.currentVertCount; ++i)
            {
                _vhelper.PopulateUIVertex(ref j_Vertex, i);
                j_Vertex.uv2 = j_Scale;
                j_Vertex.uv3 = zDepth;
                _vhelper.SetUIVertex(j_Vertex, i);
            }
        }

        #endregion
    }
}
