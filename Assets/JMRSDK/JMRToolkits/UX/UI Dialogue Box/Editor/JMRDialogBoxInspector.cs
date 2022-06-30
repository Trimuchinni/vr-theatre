// Copyright (c) 2020 JioGlass. All Rights Reserved.

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace JMRSDK.Toolkit.UI.Inspector
{
    [CustomEditor(typeof(JMRDialogueBox))]
    public class JMRDialogBoxInspector : Editor
    {
        #region Editor Action

        private const string DialogPrefabGUID = "d86b40624441d014ead9b621e1f4c8a1";

        private const string ErrorDialogPrefabGUID = "66f481c9a87ffc445b55f32e170bb628";

        private static string DialogPrefabPath => AssetDatabase.GUIDToAssetPath(DialogPrefabGUID);
        private static string ErrorDialogPrefabPath => AssetDatabase.GUIDToAssetPath(ErrorDialogPrefabGUID);


        [MenuItem("JioMixedReality/Toolkits/V1/DialogBox")]
        static void InstantiateHorizontalPrefab()
        {
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(DialogPrefabPath, typeof(GameObject));

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

        [MenuItem("JioMixedReality/Toolkits/V1/Error DialogBox")]
        static void InstantiateVerticalPrefab()
        {
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(ErrorDialogPrefabPath, typeof(GameObject));

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

        #region Editor
        public override void OnInspectorGUI()
        {
            //Add the default stuff
            DrawDefaultInspector();
        }
        #endregion
    }
}
#endif