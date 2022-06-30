// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[InitializeOnLoad]
public static class LayerUtils
{
    static LayerUtils()
    {
        CreateLayer();
    }

    static void CreateLayer()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

        SerializedProperty layers = tagManager.FindProperty("layers");
        string[] layerNames = { "Head", "Left", "Right" };
        bool ExistLayer = false;
        if (layers == null || !layers.isArray)
        {
            Debug.LogWarning("Can't set up the layers.  It's possible the format of the layers and tags data has changed in this version of Unity.");
            Debug.LogWarning("Layers is null: " + (layers == null));
            return;
        }

        foreach (string layerName in layerNames)
        {


            for (int i = 8; i < layers.arraySize; i++)
            {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);
                if (layerSP.stringValue == layerName)
                {
                    ExistLayer = true;
                    break;
                }
                else
                {
                    ExistLayer = false;

                }

            }
            for (int j = 8; j < layers.arraySize; j++)
            {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(j);
                if (layerSP.stringValue == "" && !ExistLayer)
                {
                    layerSP.stringValue = layerName;
                    tagManager.ApplyModifiedProperties();
                    
                    break;
                }
            }

        }
    }
}