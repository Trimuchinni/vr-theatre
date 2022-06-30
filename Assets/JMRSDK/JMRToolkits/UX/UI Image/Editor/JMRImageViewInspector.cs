// Copyright (c) 2020 JioGlass. All Rights Reserved.

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace JMRSDK.Toolkit.UI.Inspector
{
    [CustomEditor(typeof(JMRImageView))]
    [CanEditMultipleObjects]
    public class JMRImageViewInspector : Editor
    {

        #region Editor Actions
        const string PrefabGUID = "a407aeee4e10cfa429cab1877af1db6c";
        private static string PrefabPath => AssetDatabase.GUIDToAssetPath(PrefabGUID);

        [MenuItem("JioMixedReality/Toolkits/Common/Image View")]
        static void InstantiatePrefab()
        {
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(PrefabPath, typeof(GameObject));

            if (prefab != null)
            {
                Transform selectedObject = Selection.activeTransform;
                if (selectedObject != null)
                {
                    Selection.activeObject = PrefabUtility.InstantiatePrefab(prefab as GameObject, selectedObject);
                }
                else
                {
                    Selection.activeObject = PrefabUtility.InstantiatePrefab(prefab as GameObject);
                }

                if (Selection.activeObject != null)
                {
                    PrefabUtility.UnpackPrefabInstance(Selection.activeGameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                    //Force position the instantiated prefab if pos are not set currectly on prefab settings.
                    Selection.activeTransform.localPosition = Vector3.zero;

                    Undo.RegisterCreatedObjectUndo(Selection.activeObject, $"Create {prefab.name} Object");
                }

            }
        }
        #endregion


        private const string UIShaderGUID = "b34e0c5196e23284cbd4657c8c6e3b65";
        private const string j_MaterialInstanceName = "mi_UIMaterial";

        #region Private Variables
        private string j_UIShaderPath;

        private JMRImageView j_TargetObj;
        private Image j_ImgComponent;
        private Material j_MasterShaderMatInstance;
        #endregion

        #region Serialized Properties
        private SerializedProperty maskColor;
        private SerializedProperty mask;
        private SerializedProperty invertMask;
        private SerializedProperty blendMask;
        private SerializedProperty circleColor;

        private SerializedProperty position;
        private SerializedProperty radius;
        private SerializedProperty softness;
        private SerializedProperty alpha;
        private SerializedProperty resizeCircleXY;

        #endregion

        #region Editor

        private void Awake()
        {
            ////Debug.LogError("On awake called");

            //Cache the path shader.
            j_UIShaderPath = AssetDatabase.GUIDToAssetPath(UIShaderGUID);
        }

        private void OnEnable()
        {
            ////Debug.LogError("On enable called");
            j_TargetObj = (JMRImageView)target;

            maskColor = serializedObject.FindProperty("maskColor");
            mask = serializedObject.FindProperty("mask");
            invertMask = serializedObject.FindProperty("invertMask");
            blendMask = serializedObject.FindProperty("blendMask");
            circleColor = serializedObject.FindProperty("circleColor");

            position = serializedObject.FindProperty("position");
            radius = serializedObject.FindProperty("radius");
            softness = serializedObject.FindProperty("softness");
            alpha = serializedObject.FindProperty("alpha");
            resizeCircleXY = serializedObject.FindProperty("resizeCircleXY");

            if (j_TargetObj.GetComponent<Image>() != null)
            {
                j_ImgComponent = j_TargetObj.GetComponent<Image>();
                if (j_TargetObj.MainMat == null)
                {
                    j_MasterShaderMatInstance = new Material(AssetDatabase.LoadAssetAtPath<Shader>(j_UIShaderPath));
                    j_MasterShaderMatInstance.name = j_MaterialInstanceName;
                    j_TargetObj.MainMat = j_MasterShaderMatInstance;
                    j_ImgComponent.material = j_MasterShaderMatInstance;
                }
                else
                {
                    j_MasterShaderMatInstance = j_TargetObj.MainMat;
                    j_ImgComponent.material = j_MasterShaderMatInstance;
                }
            }
        }

        // TO DO : update the shader properties on Update
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
            if (j_MasterShaderMatInstance != null)
            {
                j_MasterShaderMatInstance.SetColor("_MaskColor", maskColor.colorValue);
                j_MasterShaderMatInstance.SetTexture("_MaskTex", (Texture)mask.objectReferenceValue);
                j_MasterShaderMatInstance.SetFloat("_InvertMask", invertMask.floatValue);
                j_MasterShaderMatInstance.SetFloat("_BlendAmount", blendMask.floatValue);
                j_MasterShaderMatInstance.SetVector("_CircleColor", circleColor.colorValue);
                j_MasterShaderMatInstance.SetVector("_Position", position.vector4Value);
                j_MasterShaderMatInstance.SetFloat("_Radius", radius.floatValue);
                j_MasterShaderMatInstance.SetFloat("_Softness", softness.floatValue);
                j_MasterShaderMatInstance.SetFloat("_Alpha", alpha.floatValue);
                j_MasterShaderMatInstance.SetVector("_ResizeCircle", resizeCircleXY.vector4Value);

                j_TargetObj.MainMat = j_MasterShaderMatInstance;
            }
        }

        #endregion
    }
}
#endif