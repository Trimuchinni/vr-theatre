// Copyright (c) 2020 JioGlass. All Rights Reserved.
// Theme manager is responsible for applying colors and icons in design system
#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Linq;
using JMRSDK.Toolkit.UI;
using System.Collections;

namespace JMRSDK.Toolkit.ThemeSystem
{
    internal class JMRThemeManager : Editor
    {
#region Static properies
        // For retaining in memory
        static List<Colors> currentColorsList;
        static SO_ThemeData currentIconsList;
#endregion

#region Private variables
        internal ThemeConfigurator JMRThemeConfigurator;

        private List<LayerCategory> availableAssetsMap;

        internal ProgressBarData m_ProgressBarData;

        internal float m_CompletionPercentage;

        private float m_EditorLoadDelay = 2f;
#endregion

#region Getters
        internal List<Colors> GetColorPreferences()
        {
            return currentColorsList;
        }

        internal SO_ThemeData GetIconPreferences()
        {
            return currentIconsList;
        }
#endregion

#region Editor methods
        private void OnEnable()
        {
            if (Application.isPlaying)
                return;

            EditorCoroutines.EditorCoroutines.StartCoroutine(AssetPostProceesingComplete(), this);
        }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
        }
#endregion

        internal void SetIconPreferences(SO_ThemeData themePrefs, string assetPath)
        {
            currentIconsList = themePrefs;
            JMRThemeConfigurator.SetIconPreferences(assetPath);
        }

        private IEnumerator AssetPostProceesingComplete()
        {
            yield return new WaitForSeconds(m_EditorLoadDelay);
            Init();
        }

        /// <summary>
        /// Reinitialze the configuration for design system for runtime use
        /// </summary>
        public void ReInitialize()
        {
            Init();
        }

        /// <summary>
        /// Initialze the configuration for design system
        /// And load initial values to use
        /// </summary>
        private void Init()
        {
            if(JMRThemeConfigurator == null)
                JMRThemeConfigurator = new ThemeConfigurator();

            currentColorsList = JMRThemeConfigurator.GetColorPreferences();
            currentIconsList = JMRThemeConfigurator.GetIconPreferences();

            m_ProgressBarData = new ProgressBarData("Searching ...", "0%", m_CompletionPercentage);

            // TO DO: Do this only once in a life time
            AddTagsToTagManager();
        }

        /// <summary>
        /// Search prefabs in project and in scene and change the colors on different layers in them
        /// </summary>
        /// <param name="colorDesignSet">A set of colors to be applied on different layers</param>
        /// <returns>bool : true if the color changing process is successfull and false if not</returns>
        internal bool ApplyColorTheme(List<Colors> colorDesignSet)
        {
            JMRProgressDialog.ToggleProgressDialog(true, m_ProgressBarData);

            JMRThemeConfigurator.SetColorPreferences(colorDesignSet);

            List<LayerCategory> availableAssetsMap = OnAssetListSearch();

            return OnAssetListApply(availableAssetsMap, ThemeType.Color);
        }

        /// <summary>
        /// Search prefabs in project and in scene and change the icons on different layers in them
        /// </summary>
        /// <returns>bool : true if the icon changing process is successfull and false if not</returns>
        public bool ApplyIconTheme()
        {
            return OnAssetListApply(OnAssetListSearch(), ThemeType.Icon);
        }

        private List<LayerCategory> OnAssetListSearch()
        {
            availableAssetsMap = GlobalAssetCatalogue.FilterAllDesignAssetsInProject();
            return availableAssetsMap;
        }

        private bool OnAssetListApply(List<LayerCategory> assetsMap, ThemeType themeType)
        {
            int layerCount = 0;

            foreach (int i in Enum.GetValues(typeof(ThemeLayer)))
            {
                string tempvar = Enum.Parse(typeof(ThemeLayer), i.ToString()).ToString();

                var assetsData = assetsMap.Where(x => x.layerName.Equals(tempvar));

                if (assetsData.Count() > 0)
                {
                    layerCount++;
                    //Show progress on progressbar
                    m_CompletionPercentage = (layerCount / Enum.GetValues(typeof(ThemeLayer)).Length) * 100;
                    m_ProgressBarData.progressText = string.Format("Applying in Scene... {0}%", m_CompletionPercentage);
                    m_ProgressBarData.progress = m_CompletionPercentage;

                    JMRProgressDialog.ToggleProgressDialog(true, m_ProgressBarData);

                    if (ApplyColorOnSpecifiedLayer((ThemeLayer)i, themeType, assetsData.First().layerObjectsPool)) { continue; }
                    else { continue; }
                }
                else { continue; }
            }

            JMRProgressDialog.ToggleProgressDialog(false);

            // Search and apply colors to the layers in the prefabs in the project excluding the ones in the scene
            GlobalAssetCatalogue.SearchAndApplyColor();

            return true;
        }

        private bool ApplyColorOnSpecifiedLayer(ThemeLayer themelayer, ThemeType themeType, List<Transform> assetsList)
        {
            return ApplyOnObjects(themelayer, assetsList, themeType);
        }

        private bool ApplyOnObjects(ThemeLayer themelayer, List<Transform> layeredObjects, ThemeType themeType)
        {

            if (layeredObjects != null && layeredObjects.Count > 0)
            {
                foreach (Transform obj in layeredObjects)
                {
                    for (int i = 0; i < currentColorsList.Count; i++)
                    {
                        if (currentColorsList[i].name.Equals(themelayer.ToString()))
                            JMRColorTheme.SetColor(obj, currentColorsList[i].color);
                    }
                }
            }
            else
            {

                return false;
            }

            return true;
        }

        /// <summary>
        /// If tags are not there in system for the first time
        /// create all the tag layers required by system
        /// </summary>
        private void AddTagsToTagManager()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            // Adding a Tag
            string[] tags = Enum.GetNames(typeof(ThemeLayer));

            for (int x = 0; x < tags.Length; x++)
            {
                // First check if it is not already present
                bool found = false;
                for (int i = 0; i < tagsProp.arraySize; i++)
                {
                    SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                    if (t.stringValue.Equals(tags[x])) { found = true; break; }
                }

                // if not found, add it
                if (!found)
                {
                    tagsProp.InsertArrayElementAtIndex(0);
                    SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
                    n.stringValue = tags[x];
                    tagManager.ApplyModifiedPropertiesWithoutUndo();
                }

            }
        }

        public void ApplyColorToLayersInPrefab(GameObject prefabAsset)
        {
            List<ThemeSystem.Colors> layerColorData = GetColorPreferences();
            List<Transform> childObjects = new List<Transform>();
            if (prefabAsset.GetComponent<IThemeHandler>() != null)
            {
                childObjects.AddRange(prefabAsset.transform.GetComponentsInChildren<Transform>(true));
            }

            for (int i = 0; i < childObjects.Count; i++)
            {
                bool isLayerTagPresent = layerColorData.Exists(x => x.name.Equals(childObjects[i].tag));
                if (isLayerTagPresent)
                {
                    ThemeSystem.Colors colorElement = layerColorData.Find(x => x.name.Equals(childObjects[i].tag));
                    JMRColorTheme.SetColor(childObjects[i], colorElement.color);
                }
            }
        }
    }
}
#endif