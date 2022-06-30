// Copyright (c) 2020 JioGlass. All Rights Reserved.

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace JMRSDK.Toolkit.UI.Inspector
{
    [CustomEditor(typeof(JMRScrollRect))]
    public class JMRScrollRectInspector : Editor
    {
        #region Editor Actions

        const string HorizontalPrefabGUID = "b05b4328b2de5a24ea052ee7b57d6de1";
        private static string HorizontalPrefabPath => AssetDatabase.GUIDToAssetPath(HorizontalPrefabGUID);

        const string VerticalPrefabGUID = "0ee7360b1769f1b4fbd52de191c5bd06";
        private static string VerticalPrefabPath => AssetDatabase.GUIDToAssetPath(VerticalPrefabGUID);

        [MenuItem("JioMixedReality/Toolkits/Common/Horizontal Scroll")]
        static void InstantiateHorizontalPrefab()
        {
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(HorizontalPrefabPath, typeof(GameObject));

            if(prefab != null)
            {
                Transform selectedObject = Selection.activeTransform;
                if(selectedObject != null)
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

        [MenuItem("JioMixedReality/Toolkits/Common/Vertical Scroll")]
        static void InstantiateVerticalPrefab()
        {
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(VerticalPrefabPath, typeof(GameObject));

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

        public override void OnInspectorGUI()
        {
            //Add the default stuff
            DrawDefaultInspector();
        }
    }
}
#endif