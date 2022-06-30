// Copyright (c) 2020 JioGlass. All Rights Reserved.

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace JMRSDK.Toolkit.UI.Inspector
{
    [CustomEditor(typeof(JMRToolTip))]
    public class JMRUITooltipInspector : Editor
    {
        #region Editor Actions

        const string PrefabGUID = "58463fcc02e10e746bb7489111ab00cd";

        private static string PrefabPath => AssetDatabase.GUIDToAssetPath(PrefabGUID);

        [MenuItem("JioMixedReality/Toolkits/Common/ToolTip")]
        static void InstantiatePrefab()
        {
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(PrefabPath, typeof(GameObject));

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
        #endregion

        public SerializedProperty DirectionProperty;
        public SerializedProperty panelRectTransformProperty;

        private JMRToolTip toolTipTarget;
        private void OnEnable()
        {
            toolTipTarget = target as JMRToolTip;
        }

        public override void OnInspectorGUI()
        {
            DirectionProperty = serializedObject.FindProperty("ToolTipDirection");
          //Add the default stuff
            DrawDefaultInspector();

            serializedObject.Update();

            //EditorGUILayout.PropertyField(Direction_Property);

            Direction j_TooltipDirection = (Direction)DirectionProperty.enumValueIndex;

            switch (j_TooltipDirection)
            {
                case Direction.Top:
                    break;

                case Direction.Bottom:

                    break;

                default:
                    break;
            }

        }
    }
}
#endif