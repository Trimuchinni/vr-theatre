// Copyright (c) 2020 JioGlass. All Rights Reserved.

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.IO;
using JMRSDK.Toolkit.UI;

namespace JMRSDK.Toolkit.ThemeSystem
{
    public class ThemeConfigurator
    {
        #region Static properties
        public  bool isInitialized = false;
        private static JMRThemePreferences designPreferences;
        private static JMRThemeColorSet colorSet;
        private static string designSystemConfiguration;
        #endregion

        #region const fields
        private const string SchemaFileName = "DesignSystemSchema.json";
        private const string PreferenceFileName = "DesignPreferences.asset";
        private const string ColorSetFileName = "ThemeColorSet.asset";

        //layers for runtime
        private const string PrimaryDisableLayerName = "PrimaryDisabledLayer";
        private const string DisbletextLayerName = "DisabledTextLayer";
        private const string SecondaryDisbleLayerName = "SecondaryDisabledLayer";
        #endregion

        #region files path
        //Path for refrences 
        //private const string schemaPath = "JMRToolkit/DesignSystem/ThemeConfiguration/Schema/DesignSystemSchema.json";
        //private const string PreferencesFilePath = "Assets/JMRToolkit/DesignSystem/ThemeConfiguration/Schema/DesignPreferences.asset";
        //private const string ColorSetFilePath = "Assets/JMRToolkit/DesignSystem/ThemeConfiguration/Colors/ThemeColorSet.asset";
        //private const JMRTKModuleType MODULE = JMRTKModuleType.Schema;
        #endregion

        #region Private Properties
        private const string SchemaDataGUID = "0223ab07ba5599a46869bd341a436bc0";
        private const string PreferenceDataGUID = "0cf6950e3836bb649b833ce423114c43";
        private const string ColorDataGUID = "bf4eb900838c47c4e921b5b128028374";

        private string SchemaFilePath => AssetDatabase.GUIDToAssetPath(SchemaDataGUID);
        private string PreferenceFilePath => AssetDatabase.GUIDToAssetPath(PreferenceDataGUID);
        private string ColorSetFilePath => AssetDatabase.GUIDToAssetPath(ColorDataGUID);
        #endregion

        public ThemeConfigurator()
        {
            Initialize();
        }

        void Initialize()
        {
            isInitialized = ConfigureDesignSystemPreferances();
        }

        //Always Take design system values from SCRO only
        //If SCRO not available load from json file
        private bool ConfigureDesignSystemPreferances()
        {
            if (string.IsNullOrEmpty(SchemaFilePath) || string.IsNullOrEmpty(PreferenceFilePath)) return false;

            designSystemConfiguration = File.ReadAllText(SchemaFilePath);

            if (string.IsNullOrEmpty(designSystemConfiguration))
            {
                //Debug.LogError("No data file Present in system");
                return false;
            }
            else
            {
                designPreferences = (JMRThemePreferences)AssetDatabase.LoadAssetAtPath(PreferenceFilePath, typeof(JMRThemePreferences));

                 if(designPreferences == null)
                 {
                    ConfigData designSystemConfig = JsonUtility.FromJson<ConfigData>(designSystemConfiguration);
                    designPreferences = ScriptableObject.CreateInstance<JMRThemePreferences>();
                    UnityEditor.AssetDatabase.CreateAsset(designPreferences, PreferenceFilePath);
                    designPreferences.ConfigurationData = designSystemConfig;
                }

                SaveConfigData();
            }
            return true;
        }

        /// <summary>
        /// Set color preferences in file for future use
        /// </summary>
        /// <param name="colorsPref">List of colors</param>
        /// <returns>true is saved without any issue</returns>
        public bool SetColorPreferences(List<Colors> colorsPref)
        {
            try
            {
                ConfigData tempConfig = designPreferences.ConfigurationData.ShallowCopy();//new ConfigData();
                tempConfig.colors = colorsPref;
                designPreferences.ConfigurationData = tempConfig;
                SaveConfigData();
                SetColorsInColorSet();
            }
            catch (Exception)
            {
                //Debug.LogError("Color apply failed : " + e.Message);
                return false;
            }
            return true;
        }

        //TO DO : Make Icon selection with GUID only once we have more than 1 icons pack
        public bool SetIconPreferences(string selectedAssetPath)
        {
            if (string.IsNullOrEmpty(selectedAssetPath) || designPreferences.ConfigurationData.SelectedIconSetPath.Equals(selectedAssetPath)) return false;

            ConfigData tempConfig = designPreferences.ConfigurationData.ShallowCopy();
            tempConfig.SelectedIconSetPath = selectedAssetPath;

            designPreferences.ConfigurationData = tempConfig;
            SaveConfigData();
            return true;
        }

        /// <summary>
        /// Fetch the colors from file
        /// </summary>
        /// <returns>List of colors for layers</returns>
        public List<Colors> GetColorPreferences()
        {
            if (designPreferences == null) return null;

            return designPreferences.ConfigurationData.colors;
        }

        //public List<Icon> GetIconPreferences()
        //{
        //    if (designPreferences == null) return null;

        //    return designPreferences.configData.icons;
        //}

        /// <summary>
        /// Currently saved icons set from file
        /// </summary>
        /// <returns>selected icons theme data</returns>
        public SO_ThemeData GetIconPreferences()
        {
            if (designPreferences == null) return null;

            SO_ThemeData iconSet = (SO_ThemeData)AssetDatabase.LoadAssetAtPath(designPreferences.ConfigurationData.SelectedIconSetPath, typeof(SO_ThemeData));
            return iconSet;
        }

        internal Toolboxes GetToolBoxPreferences()
        {
            return null;
        }

        public bool MapToolkitLayersToSystem()
        {
            return false;
        }

        /// <summary>
        /// Set colors for runtime
        /// </summary>
        /// <returns>true if saved successfully</returns>
        public bool SetColorsInColorSet()
        {
            bool isColorSaved = false;

            if (string.IsNullOrEmpty(ColorSetFilePath)) return isColorSaved;

            colorSet = (JMRThemeColorSet)AssetDatabase.LoadAssetAtPath(ColorSetFilePath, typeof(JMRThemeColorSet));

            List<Colors> systemColors = designPreferences.ConfigurationData.colors;

            if (colorSet != null)
            {
                for (int i = 0; i < systemColors.Count; i++)
                {
                    if (systemColors[i].name.Equals(PrimaryDisableLayerName))
                    {
                        colorSet.primaryDisabledColorHex = systemColors[i].color;
                    }else if (systemColors[i].name.Equals(SecondaryDisbleLayerName))
                    {
                        colorSet.secondaryDisabledColorhex = systemColors[i].color;
                    }
                    else if (systemColors[i].name.Equals(DisbletextLayerName))
                    {
                        colorSet.disabledTextColorHex = systemColors[i].color;
                    }
                }
                isColorSaved = true;
            }

            return isColorSaved;
        }

        /// <summary>
        /// Save the data in SCRO
        /// </summary>
        private void SaveConfigData()
        {
            if (designPreferences == null || !designPreferences.IsValueChaged)
            {
                return;
            }

            //[PADMA][TODO]
            EditorUtility.SetDirty(designPreferences);
            //designPreferences.SetDirty();
            AssetDatabase.SaveAssets();
            // Becuase of unity bug most of the time assets are not getting loaded so we are forcing that to load
            AssetDatabase.Refresh();

            // File.WriteAllText(string.Format("{0}/{1}", Application.dataPath, SchemaFilePath), JsonUtility.ToJson(designPreferences.configData));
        }
    }
}
#endif