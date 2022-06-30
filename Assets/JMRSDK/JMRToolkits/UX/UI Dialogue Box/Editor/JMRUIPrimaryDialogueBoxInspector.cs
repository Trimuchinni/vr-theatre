// Copyright (c) 2020 JioGlass. All Rights Reserved.

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace JMRSDK.Toolkit.UI.Inspector
{
    [CustomEditor(typeof(JMRUIPrimaryDialogueBox))]
    public class JMRUIPrimaryDialogueBoxInspector : Editor
    {
        #region Editor Action

        private const string DialogPrefabGUID = "ac58f041c899aae4a83a4c9ba01c4916";

        private static string DialogPrefabPath => AssetDatabase.GUIDToAssetPath(DialogPrefabGUID);


        [MenuItem("JioMixedReality/Toolkits/V2/Dialogbox")]
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