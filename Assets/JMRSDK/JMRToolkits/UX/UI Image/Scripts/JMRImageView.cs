// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using UnityEngine.UI;

namespace JMRSDK.Toolkit.UI
{
    [ExecuteInEditMode]
    public class JMRImageView : MonoBehaviour
    {
        public Material MainMat;

        #region Serialized Properties
        [SerializeField]
        private bool IsUpdate;

        [Header("MASK")]
        [SerializeField]
        private Color maskColor;

        [SerializeField]
        private Texture2D mask;

        [Range(0, 1)]
        [SerializeField]
        private float invertMask, blendMask;


        [Header("CIRCLE ANIMATION")]
        [SerializeField]
        private Color circleColor;

        [SerializeField]
        private Vector4 position;

        [Range(0, 5)]
        [SerializeField]
        private float radius;
        [Range(0, 10)]
        [SerializeField]
        private float softness;
        [Range(0, 1)]
        [SerializeField]
        private float alpha;

        [SerializeField]
        private Vector4 resizeCircleXY;

        public bool UpdatePerFrame { get => IsUpdate; set => IsUpdate = value; }

        #endregion

        //Cache image component refrence.
        private Image j_ImageComponent;

        //Cache canvas refrence.
        private JMRCanvasUtility j_CurrentCanvas;

        private const string j_MaterialInstanceName = "mi_UIMaterial";

        #region Mono

        private void Awake()
        {
            if (IsUpdate)
            {
                j_CurrentCanvas = transform.GetComponentInParent<JMRCanvasUtility>();
            }
        }


        private void Start()
        {
            if (GetComponent<Image>() != null)
            {
                j_ImageComponent = GetComponent<Image>();
                if (MainMat == null)
                {
                    MainMat = new Material(Shader.Find("UI/UI_Master_Shader"));
                    j_ImageComponent.material = MainMat;
                    j_ImageComponent.material.name = j_MaterialInstanceName;
                }
            }
        }


        private void OnEnable()
        {
            if (IsUpdate)
            {
                if (j_CurrentCanvas != null)
                    j_CurrentCanvas.UIUpdate += UpdateShaderVal;
            }
        }

        private void OnDisable()
        {
            if (IsUpdate)
            {
                if (j_CurrentCanvas != null)
                    j_CurrentCanvas.UIUpdate -= UpdateShaderVal;
            }
        }

        ////TO DO: Replace it with on demand update.
        //private void Update()
        //{
        //    if (!IsUpdate) return;

        //    UpdateShader();
        //}

        #endregion

        #region Events

        /// <summary>
        /// Update the shader properties at runtime specially designed for animations.
        /// </summary>
        private void UpdateShaderVal()
        {
            if (MainMat != null)
            {
                MainMat.SetColor("_MaskColor", maskColor);
                MainMat.SetTexture("_MaskTex", mask);
                MainMat.SetFloat("_InvertMask", invertMask);
                MainMat.SetFloat("_BlendAmount", blendMask);
                MainMat.SetVector("_CircleColor", circleColor);
                MainMat.SetVector("_Position", position);
                MainMat.SetFloat("_Radius", radius);
                MainMat.SetFloat("_Softness", softness);
                MainMat.SetFloat("_Alpha", alpha);
                MainMat.SetVector("_ResizeCircle", resizeCircleXY);
            }
        }

        #endregion

    }
}