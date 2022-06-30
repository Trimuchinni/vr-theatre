using System;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;

namespace JMRSDK.InputModule
{
    [CustomEditor(typeof(JMRManipulation)), CanEditMultipleObjects]
    public class Editor_Manupulation : Editor
    {
        private SerializedProperty isGrabbale, isRotatable, isScalable, isMovable, SnapToCenter;
        private JMRManipulation targetObject;
        private bool setGrabable, setRotatable;


        void OnEnable()
        {
            targetObject = target as JMRManipulation;
            LoadSerializeProperties();
        }

        private void LoadSerializeProperties()
        {
            SnapToCenter = serializedObject.FindProperty("SnapToCenter");
            isGrabbale = serializedObject.FindProperty("isGrabbale");
            isRotatable = serializedObject.FindProperty("isRotatable");
            isScalable = serializedObject.FindProperty("isScalable");
            isMovable = serializedObject.FindProperty("isMovable");
        }

        public override void OnInspectorGUI()
        {
            //EditorGUILayout.BeginHorizontal();

            //if (false)
            //{

            EditorGUILayout.PropertyField(SnapToCenter);
            EditorGUILayout.PropertyField(isGrabbale);
                EditorGUILayout.PropertyField(isRotatable);
                if (setRotatable || setGrabable)
                    EditorGUILayout.PropertyField(isScalable);
                if (setGrabable)
                    EditorGUILayout.PropertyField(isMovable);

                //EditorGUILayout.EndHorizontal();

                if (Event.current.type == EventType.Repaint)
                {
                    if (!setGrabable && targetObject.IsGrabbale)
                    {
                        setGrabable = true;
                        targetObject.IsRotatable = false;
                    }
                    else if (setGrabable && !targetObject.IsGrabbale)
                    {
                        setGrabable = false;
                    }
                    if (!setRotatable && targetObject.IsRotatable)
                    {
                        setRotatable = true;
                        targetObject.IsMovable = false;
                        targetObject.IsGrabbale = false;
                    }
                    else if (setRotatable && !targetObject.IsRotatable)
                    {
                        setRotatable = false;
                    }

                }
                serializedObject.ApplyModifiedProperties();
            //}
            //else { DrawDefaultInspector(); }
        }
    }
}
