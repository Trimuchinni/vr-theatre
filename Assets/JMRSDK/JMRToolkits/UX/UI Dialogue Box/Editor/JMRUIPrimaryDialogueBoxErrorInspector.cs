// Copyright (c) 2020 JioGlass. All Rights Reserved.

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace JMRSDK.Toolkit.UI.Inspector
{
    [CustomEditor(typeof(JMRUIPrimaryDialogueBoxError))]
    public class JMRUIPrimaryDialogueBoxErrorInspector : Editor
    {
        #region Editor Action

        private const string DialogErrorPrefabGUID = "d99ae56d58779174192c97335e6d28c7";

        private static string DialogErrorPrefabPath => AssetDatabase.GUIDToAssetPath(DialogErrorPrefabGUID);


        [MenuItem("JioMixedReality/Toolkits/V2/Error Dialogbox")]
        static void InstantiateHorizontalPrefab()
        {
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(DialogErrorPrefabPath, typeof(GameObject));

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