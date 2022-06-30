// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;

namespace JMRSDK.InputModule
{
    [CustomEditor(typeof(JMRControllerKeyBinding)), CanEditMultipleObjects]
    public class Editor_JMRControllerKeyBinding : Editor
    {
        private SerializedProperty currentKeyBinding, Select, Menu, Home, Back, Recenter, VoiceTrigger, IPDCalibration, Manipulation, Fn1, Fn2;
        private JMRControllerKeyBinding targetObject;
        private string currentButtonValue, currentButtonStateValue, changedButtonValue, changedButtonStateValue, defaultButtonValue, defaultButtonStateValue;

        private float timer;
        private bool isUpdating;
        private SerializedProperty inputActionList;
        int customControlCount = 0;


        void OnEnable()
        {
            timer = 0;
            isUpdating = false;
            targetObject = target as JMRControllerKeyBinding;
            customControlCount = targetObject.GetCurrentKeyBinding.CustomActionCount;
            LoadSerializeProperties();
            //targetObject.LoadCurrent();
            targetObject.LoadDefault();
            CalculateDefaultPropertiesValue();
            CalculateCurrentPropertiesValue();
        }

        private void LoadSerializeProperties()
        {
            currentKeyBinding = serializedObject.FindProperty("currentKeyBinding");
            Select = serializedObject.FindProperty("Select");
            Menu = serializedObject.FindProperty("Menu");
            Home = serializedObject.FindProperty("Home");
            Back = serializedObject.FindProperty("Back");
            Recenter = serializedObject.FindProperty("Recenter");
            VoiceTrigger = serializedObject.FindProperty("VoiceTrigger");
            IPDCalibration = serializedObject.FindProperty("IPDCalibration");
            Manipulation = serializedObject.FindProperty("Manipulation");
            Fn1 = serializedObject.FindProperty("Fn1");
            Fn2 = serializedObject.FindProperty("Fn2");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            //if (customControlCount <= 1)          // Commenting this (suggested by Bhumit and Padma)
            //{
            //    if (GUILayout.Button("+ Add Custom Action"))
            //    {
            //        customControlCount++;
            //        targetObject.GetCurrentKeyBinding.CustomActionCount = customControlCount;
            //    }
            //}

            //if (customControlCount >= 1)          // Commenting this (suggested by Bhumit and Padma)
            //{
            //    if (GUILayout.Button("- Remove Custom Action"))
            //    {
            //        customControlCount--;
            //        targetObject.GetCurrentKeyBinding.CustomActionCount = customControlCount;
            //    }
            //}
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(currentKeyBinding);
            EditorGUILayout.PropertyField(Select);
            EditorGUILayout.PropertyField(Menu);
            EditorGUILayout.PropertyField(Home);
            EditorGUILayout.PropertyField(Back);
            EditorGUILayout.PropertyField(Recenter);
            EditorGUILayout.PropertyField(VoiceTrigger);
            EditorGUILayout.PropertyField(IPDCalibration);
            EditorGUILayout.PropertyField(Manipulation);

            EditorGUILayout.PropertyField(Fn1);
            EditorGUILayout.PropertyField(Fn2);

            //if (customControlCount >= 1)          // Commenting this (suggested by Bhumit and Padma)
            //{
            //    EditorGUILayout.PropertyField(Fn1);
            //    if (customControlCount >= 2)
            //        EditorGUILayout.PropertyField(Fn2);
            //}

            EditorGUILayout.BeginHorizontal();
            GUILayout.ExpandWidth(false);

            if (defaultButtonValue != changedButtonValue || defaultButtonStateValue != changedButtonStateValue)
            {
                //if (GUILayout.Button("Set Default"))
                //{
                    isUpdating = false;
                    targetObject.LoadDefault();
                    targetObject.SaveCotrollerData();
                    ResetPropertyValue();
                    targetObject.GetCurrentKeyBinding.CustomActionCount = customControlCount = 0;
                //}
            }

            if (isUpdating)
            {

                if (GUILayout.Button("Reset"))
                {
                    isUpdating = false;
                    targetObject.LoadCurrent();
                    ResetPropertyValue();
                }
                if (GUILayout.Button("Save"))
                {
                    isUpdating = false;
                    targetObject.SaveCotrollerData();
                    ResetPropertyValue();
                }
            }

            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.Repaint)
            {
                if (targetObject.GetCurrentKeyBinding.currentButtonValue != changedButtonValue || targetObject.GetCurrentKeyBinding.currentButtonStateValue != changedButtonStateValue)
                    isUpdating = true;
            }

            if (!isUpdating)
            {
                timer += Time.deltaTime;
                if (timer >= 0.5f)
                {
                    CalculateCurrentPropertiesValue();
                    timer = 0;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void ResetPropertyValue()
        {
            CalculateCurrentPropertiesValue();
            targetObject.GetCurrentKeyBinding.currentButtonValue = changedButtonValue;
            targetObject.GetCurrentKeyBinding.currentButtonStateValue = changedButtonStateValue;
            currentButtonValue = changedButtonValue;
            currentButtonStateValue = changedButtonStateValue;
        }

        private void CalculateCurrentPropertiesValue()
        {
            changedButtonValue = targetObject.GetSelectBinding().ControllerButton.ToString() + targetObject.GetMenuBinding().ControllerButton.ToString()
                + targetObject.GetHomeBinding().ControllerButton.ToString() + targetObject.GetBackBinding().ControllerButton.ToString()
                + targetObject.GetRecenterBinding().ControllerButton.ToString() + targetObject.GetVoiceTriggerBinding().ControllerButton.ToString()
                + targetObject.GetManipulationBinding().ControllerButton.ToString()
                + targetObject.GetFn1Binding().ControllerButton.ToString()
                + targetObject.GetFn2Binding().ControllerButton.ToString();

            changedButtonStateValue = targetObject.GetMenuBinding().ControllerButtonState.ToString() + targetObject.GetHomeBinding().ControllerButtonState.ToString()
                + targetObject.GetBackBinding().ControllerButtonState.ToString() + targetObject.GetRecenterBinding().ControllerButtonState.ToString()
                + targetObject.GetVoiceTriggerBinding().ControllerButtonState.ToString() + targetObject.GetManipulationBinding().RotationSource.ToString()
                + targetObject.GetFn1Binding().ControllerButtonState.ToString() + targetObject.GetFn2Binding().ControllerButtonState.ToString();
        }

        private void CalculateDefaultPropertiesValue()
        {
            defaultButtonValue = targetObject.GetCurrentKeyBinding.GetDefaultControllerKeyBinding.GetSelectBinding().ControllerButton.ToString()
                + targetObject.GetCurrentKeyBinding.GetDefaultControllerKeyBinding.GetMenuBinding().ControllerButton.ToString()
                + targetObject.GetCurrentKeyBinding.GetDefaultControllerKeyBinding.GetHomeBinding().ControllerButton.ToString()
                + targetObject.GetCurrentKeyBinding.GetDefaultControllerKeyBinding.GetBackBinding().ControllerButton.ToString()
                + targetObject.GetCurrentKeyBinding.GetDefaultControllerKeyBinding.GetRecenterBinding().ControllerButton.ToString()
                + targetObject.GetCurrentKeyBinding.GetDefaultControllerKeyBinding.GetVoiceTriggerBinding().ControllerButton.ToString()
                + targetObject.GetCurrentKeyBinding.GetDefaultControllerKeyBinding.GetManipulationBinding().ControllerButton.ToString()
                + targetObject.GetCurrentKeyBinding.GetDefaultControllerKeyBinding.GetFn1Binding().ControllerButton.ToString()
                + targetObject.GetCurrentKeyBinding.GetDefaultControllerKeyBinding.GetFn2Binding().ControllerButton.ToString();

            defaultButtonStateValue = targetObject.GetCurrentKeyBinding.GetDefaultControllerKeyBinding.GetMenuBinding().ControllerButtonState.ToString()
                + targetObject.GetCurrentKeyBinding.GetDefaultControllerKeyBinding.GetHomeBinding().ControllerButtonState.ToString()
                + targetObject.GetCurrentKeyBinding.GetDefaultControllerKeyBinding.GetBackBinding().ControllerButtonState.ToString()
                + targetObject.GetCurrentKeyBinding.GetDefaultControllerKeyBinding.GetRecenterBinding().ControllerButtonState.ToString()
                + targetObject.GetCurrentKeyBinding.GetDefaultControllerKeyBinding.GetVoiceTriggerBinding().ControllerButtonState.ToString()
                + targetObject.GetCurrentKeyBinding.GetDefaultControllerKeyBinding.GetManipulationBinding().RotationSource.ToString()
                + targetObject.GetCurrentKeyBinding.GetDefaultControllerKeyBinding.GetFn1Binding().ControllerButtonState.ToString()
                + targetObject.GetCurrentKeyBinding.GetDefaultControllerKeyBinding.GetFn2Binding().ControllerButtonState.ToString();
        }
    }
}
