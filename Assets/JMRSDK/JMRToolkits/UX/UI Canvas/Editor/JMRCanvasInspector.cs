// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UEditor = UnityEditor.Editor;
using JMRSDK.Toolkit.UI.Utilities;
using JMRSDK.InputModule;
using JMRSDK;

namespace JMRSDK.Toolkit.UI
{
    /// <summary>
    /// Helper class to set CanvasUtility onto Canvas objects.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Canvas))]
    public class JMRCanvasInspector : UEditor
    {
        #region Static Variables
        private static readonly GUIContent MakeJMRCanvas = new GUIContent("Convert to JMRSDK Canvas");
        private static readonly GUIContent RemoveJMRCanvas = new GUIContent("Convert to Unity Canvas");
        private readonly List<Graphic> j_graphicsRequireScaleMeshEffect = new List<Graphic>();
        #endregion

        #region private properties
        private Type j_CanvasEditorType = null;
        private UEditor j_InternalEditor = null;
        private Canvas j_Canvas = null;
        private JMRInputManager j_IPManager;
        #endregion

        #region Private Variables
        private bool IsRootCanvas = false;
        private bool IsJMRToolkitConfigured = false;
        private float j_Timer;
        private bool IsCanvasHasScene = false;
        #endregion

        #region Editor Action
        //Prefab GUID it might change when you change the prefabs path
        private const string PrefabGUID = "a2b79847880d7884693bc4957ba3a0c4";
        private static string PrefabPath => AssetDatabase.GUIDToAssetPath(PrefabGUID);

        [MenuItem("JioMixedReality/Toolkits/Common/Canvas", false, 2)]
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

                    Undo.RegisterCreatedObjectUndo(Selection.activeObject, $"Create {prefab.name} Object");
                }
            }
        }
        #endregion

        #region Editor
        private void OnEnable()
        {
            j_Timer = 0;
            j_CanvasEditorType = Type.GetType("UnityEditor.CanvasEditor, UnityEditor");

            if (j_CanvasEditorType != null)
            {
                j_InternalEditor = CreateEditor(targets, j_CanvasEditorType);
                j_Canvas = target as Canvas;
                IsRootCanvas = j_Canvas.transform.parent == null || j_Canvas.transform.parent.GetComponentInParent<Canvas>() == null;
                IsCanvasHasScene = j_Canvas.gameObject.scene.IsValid();
            }
            
            // Find id the input manager in scene
            j_IPManager = FindObjectOfType<JMRInputManager>();
            IsJMRToolkitConfigured = (j_IPManager != null) ? true : false;
        }

        private void OnDisable()
        {
            if (j_CanvasEditorType != null)
            {
                MethodInfo onDisable = j_CanvasEditorType.GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic);
                if (onDisable != null)
                {
                    onDisable.Invoke(j_InternalEditor, null);
                }
                DestroyImmediate(j_InternalEditor);
            }

        }

        public override void OnInspectorGUI()
        {
            if (IsRootCanvas && j_Canvas != null)
            {
                ShowMRTKButton();

                List<Graphic> graphics = GetGraphicsWhichRequireScaleMeshEffect(targets);

                if (graphics.Count != 0)
                {
                    EditorGUILayout.HelpBox($"Canvas contains {graphics.Count} {typeof(Graphic).Name}(s) which require a {typeof(JMRScaleMeshEffect).Name} to work with the {JMRStandardShaderUtility.StandardShaderName} shader.", MessageType.Warning);
                    if (GUILayout.Button($"Add {typeof(JMRScaleMeshEffect).Name}(s)"))
                    {
                        foreach (var graphic in graphics)
                        {
                            Undo.AddComponent<JMRScaleMeshEffect>(graphic.gameObject);
                        }
                    }
                }

                EditorGUILayout.Space();

                if (!IsCanvasHasScene) return;

                if (IsJMRToolkitConfigured)
                {
                    if (j_Canvas.worldCamera == null)
                    {
                        SetRaycastCameraToCanvas();
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox($"Input Manager not present in scene. Event camera needed in scene to work canvas properly", MessageType.Error);
                    if (j_Timer >= 0.5f)
                    {
                        if (!j_IPManager)
                            j_IPManager = FindObjectOfType<JMRInputManager>();

                        IsJMRToolkitConfigured = (j_IPManager != null) ? true : false;
                        j_Timer = 0;
                    }
                    j_Timer += Time.deltaTime;
                }
            }

            if (j_InternalEditor != null)
            {
                j_InternalEditor.OnInspectorGUI();
            }
        }

        #endregion
        private bool ShowMRTKButton()
        {
            if (!j_Canvas.rootCanvas)
            {
                return false;
            }

            bool isMRTKCanvas = j_Canvas.GetComponent<JMRCanvasUtility>() != null;

            if (isMRTKCanvas)
            {
                if (GUILayout.Button(RemoveJMRCanvas))
                {
                    EditorApplication.delayCall += () =>
                    {
                        DestroyImmediate(j_Canvas.GetComponent<JMRCanvasUtility>());
                    };

                    isMRTKCanvas = false;
                }

            }
            else
            {
                if (GUILayout.Button(MakeJMRCanvas))
                {
                    if (j_Canvas.GetComponent<GraphicRaycaster>() == null)
                    {
                        Undo.AddComponent<GraphicRaycaster>(j_Canvas.gameObject);
                    }

                    if (j_Canvas.GetComponent<JMRCanvasUtility>() == null)
                    {
                        Undo.AddComponent<JMRCanvasUtility>(j_Canvas.gameObject);
                    }

                    j_Canvas.renderMode = RenderMode.WorldSpace;
                    j_Canvas.worldCamera = null;
                    isMRTKCanvas = true;
                }
            }

            return isMRTKCanvas;
        }

        private List<Graphic> GetGraphicsWhichRequireScaleMeshEffect(UnityEngine.Object[] targets)
        {
            j_graphicsRequireScaleMeshEffect.Clear();

            foreach (UnityEngine.Object target in targets)
            {
                Graphic[] graphics = (target as Canvas).GetComponentsInChildren<Graphic>();

                foreach (Graphic graphic in graphics)
                {
                    if (JMRStandardShaderUtility.IsUsingJMRSDKStandardShader(graphic.material) &&
                        graphic.GetComponent<JMRScaleMeshEffect>() == null)
                    {
                        j_graphicsRequireScaleMeshEffect.Add(graphic);
                    }
                }
            }

            return j_graphicsRequireScaleMeshEffect;
        }

        /// <summary>
        /// Set the UI raycast camera as event camera in canvas
        /// </summary>
        private void SetRaycastCameraToCanvas()
        {
            JMRUIRayCastCamera jmrRaycastCamera = FindObjectOfType<JMRUIRayCastCamera>();
            if (jmrRaycastCamera != null)
                j_Canvas.worldCamera = jmrRaycastCamera.GetRayCastCamera();
        }
    }
}