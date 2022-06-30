// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEditor;
using UnityEngine;

namespace JMRSDK
{
    [CustomEditor(typeof(JMREmulatorKeyBinding)), CanEditMultipleObjects()]
    public class Editor_JMREmulatorKeyBinding : Editor
    {
        private SerializedProperty CurrentEditorKeyBinding;
        private float timer;
        private bool isUpdating;
        private JMREmulatorKeyBinding targetObject;
        private string currentState, prevState, defaultState;

        void OnEnable()
        {
            timer = 0;
            isUpdating = false;
            targetObject = target as JMREmulatorKeyBinding;
            currentState = prevState = targetObject.GetCurrentEditorKeyBinding.currentState;
            //targetObject.LoadCurrentData();
            targetObject.LoadDefault();
            CalculateDefaultState();
            CalculateCurrentState();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.BeginHorizontal();
            GUILayout.ExpandWidth(false);
            if (currentState != defaultState)
            {
                //if (GUILayout.Button("Default"))
                //{
                    targetObject.LoadDefault();
                    targetObject.SaveData();
                    isUpdating = false;
                    CalculateCurrentState();
                    targetObject.GetCurrentEditorKeyBinding.currentState = currentState;
                    prevState = currentState;
                //}
            }

            if (isUpdating)
            {
                if (currentState != prevState)
                {
                    if (GUILayout.Button("Save"))
                    {
                        targetObject.SaveData();
                        isUpdating = false;
                        CalculateCurrentState();
                        targetObject.GetCurrentEditorKeyBinding.currentState = currentState;
                        prevState = currentState;
                    }
                    if (GUILayout.Button("Reset"))
                    {
                        targetObject.LoadCurrentData();
                        isUpdating = false;
                        CalculateCurrentState();
                        targetObject.GetCurrentEditorKeyBinding.currentState = currentState;
                        prevState = currentState;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if (!isUpdating && Event.current.type == EventType.Repaint)
            {
                timer += Time.deltaTime;
                if (timer >= 1)
                    CalculateCurrentState();
            }
        }

        private void CalculateDefaultState()
        {
            defaultState = targetObject.GetCurrentEditorKeyBinding.GetDefaultKeyBinding.GetSelectButton().ToString().ToString() + targetObject.GetCurrentEditorKeyBinding.GetDefaultKeyBinding.GetHomeButton().ToString()
                             + targetObject.GetCurrentEditorKeyBinding.GetDefaultKeyBinding.GetAppButton().ToString() + targetObject.GetCurrentEditorKeyBinding.GetDefaultKeyBinding.GetTrackPadButton().ToString()
                             + targetObject.GetCurrentEditorKeyBinding.GetDefaultKeyBinding.GetSwipeUpButton().ToString() + targetObject.GetCurrentEditorKeyBinding.GetDefaultKeyBinding.GetSwipeDownButton().ToString()
                             + targetObject.GetCurrentEditorKeyBinding.GetDefaultKeyBinding.GetSwipeLeftButton().ToString() + targetObject.GetCurrentEditorKeyBinding.GetDefaultKeyBinding.GetSwipeRightButton().ToString();
        }

        private void CalculateCurrentState()
        {
            timer = 0;
            currentState = targetObject.GetSelectButton().ToString() + targetObject.GetHomeButton().ToString()
                             + targetObject.GetAppButton().ToString() + targetObject.GetTrackPadButton().ToString()
                             + targetObject.GetSwipeUpButton().ToString() + targetObject.GetSwipeDownButton().ToString()
                             + targetObject.GetSwipeLeftButton().ToString() + targetObject.GetSwipeRightButton().ToString();

            if (currentState != prevState)
                isUpdating = true;
        }
    }
}
