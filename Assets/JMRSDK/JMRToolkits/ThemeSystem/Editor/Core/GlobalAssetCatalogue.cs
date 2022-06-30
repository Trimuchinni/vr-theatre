// Copyright (c) 2020 JioGlass. All Rights Reserved.
// Class responsibility is search the design system elements from project
#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace JMRSDK.Toolkit.ThemeSystem
{
    internal static class GlobalAssetCatalogue
    {
        private static List<GameObject> prefabsList = new List<GameObject>();

        private static List<Transform> targetObjectChilds;

        private static void SearchPrefabsInScene()
        {
            prefabsList.Clear();

            prefabsList.AddRange(GetAllActiveInScene());
        }

        /// <summary>
        /// Search Prefabs in the project, excluding the ones in the scene and apply color to their child objects with assigned layers.
        /// </summary>
        public static void SearchAndApplyColor()
        {
            JMRProgressDialog.ToggleProgressDialog(true, JMRToolkit.JMRThemeManager.m_ProgressBarData);

            // Get all the assets of type Prefab in the project.
            var guids = AssetDatabase.FindAssets("t:Prefab");

            // Loop through the above fetched assets
            for (int i = 0; i < guids.Length; i++)
            {
                string myObjectPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(myObjectPath);

                // Check if the object exists. If yes also check if it has IThemeHandler component.
                if (obj && obj.GetComponent<IThemeHandler>() != null)
                {
                    // Create an instance of the object fetched.
                    GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(obj);

                    // Apply color the the layers
                    JMRToolkit.JMRThemeManager.ApplyColorToLayersInPrefab(prefabInstance);

                    // Apply changes made to the prefab instance created
                    PrefabUtility.ApplyPrefabInstance(prefabInstance.gameObject, InteractionMode.AutomatedAction);
                    AssetDatabase.SaveAssets();

                    // Destroy the instance
                    GameObject.DestroyImmediate(prefabInstance);
                }

                // Display color application progress
                JMRToolkit.JMRThemeManager.m_CompletionPercentage = ((float)i / guids.Length);
                JMRToolkit.JMRThemeManager.m_ProgressBarData.progressText = string.Format("Applying in Project... {0:P0}", JMRToolkit.JMRThemeManager.m_CompletionPercentage);
                JMRToolkit.JMRThemeManager.m_ProgressBarData.progress = JMRToolkit.JMRThemeManager.m_CompletionPercentage;
                JMRProgressDialog.ToggleProgressDialog(true, JMRToolkit.JMRThemeManager.m_ProgressBarData);
            }

            JMRProgressDialog.ToggleProgressDialog(false);
        }

        // This script finds all objects in scene
        private static GameObject[] GetAllActiveInScene()
        {
            var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            List<IThemeHandler> componentResult = new List<IThemeHandler>();
            foreach (var rootObject in rootObjects)
            {
                componentResult.AddRange(rootObject.GetComponentsInChildren<IThemeHandler>(true));
            }

            List<GameObject> objectResult = new List<GameObject>();
            foreach (var result in componentResult)
            {
                objectResult.Add(result.GetGameObject());
            }

            //Debug.Log("Objects list count : " + objectResult.Count);
            return objectResult.ToArray();
        }


        public static void SearchByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string key = string.Format("t:{0}", typeof(T)).ToString().Replace("UnityEngine.", "");
            var guids = AssetDatabase.FindAssets(key);

            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                    JMRLogHandler.Log("Name = " + asset.name);
                }
            }
        }

        public static List<GameObject> FindAllDesignAssetsInScene()
        {
            SearchPrefabsInScene();
            return prefabsList;
        }

        public static List<LayerCategory> FilterAllDesignAssetsInProject()
        {
            targetObjectChilds = new List<Transform>();

            List<LayerCategory> layerCategoriesInThemePrefab = new List<LayerCategory>();

            List<GameObject> allObjects = FindAllDesignAssetsInScene();

            //Debug.Log("Prefabs count : " + allObjects.Count);

            for (int i = 0; i < allObjects.Count; i++)
            {
                if (allObjects[i].GetComponent<IThemeHandler>() != null)
                {
                    targetObjectChilds.AddRange(allObjects[i].transform.GetComponentsInChildren<Transform>(true));
                }
            }

            return GetFilteredData();
        }


        public static List<Transform> GetDesignAssetsInSpecifiedLayer(ThemeLayer themeLayer, List<Transform> assetList)
        {
            List<LayerCategory> filteredObjects = FilterAllDesignAssetsInProject();

            return filteredObjects.Where(x => x.layerName.Equals(themeLayer.ToString())).First().layerObjectsPool;
        }

        private static List<LayerCategory> GetFilteredData()
        {
            //TODO find all objects
            List<LayerCategory> filteredLayerData = new List<LayerCategory>();

            List<ThemeSystem.Colors> layerColorData = JMRToolkit.JMRThemeManager.GetColorPreferences();

            for (int colorIndex = 0; colorIndex < layerColorData.Count; colorIndex++)
            {
                LayerCategory tempData = new LayerCategory(layerColorData[colorIndex].name, layerColorData[colorIndex].color);
                var data = targetObjectChilds.Where(x => x.tag.Equals(tempData.layerName));
                if (data.Count() > 0)
                {
                    tempData.layerObjectsPool.AddRange(data);
                    filteredLayerData.Add(tempData);
                }
            }
            return filteredLayerData;
        }
    }
}
#endif