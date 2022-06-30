// Copyright (c) 2020 JioGlass. All Rights Reserved.
// Root class for design system
// Responsible for managing all the extended features in future
#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMRSDK.Toolkit.ThemeSystem;
using UnityEditor;
using UnityEngine.Rendering;

namespace JMRSDK.Toolkit
{
    [InitializeOnLoad]
    internal class JMRToolkit
    {
        // Assets GUID's Don't change
        private const string GlobalDataGUID = "13265d18bb3a0a04da2f92c82df3b8ac";
        private const string ToolkitManagerPrefabGUID = "af3febcd08d01c942b82a75ebb3dc8c7";

        #region Static variables
        private static SO_GlobalData globalData;
        private static JMRThemeManager j_JmrThemeManager;

        private static string ToolkitManagerPrefabPath => AssetDatabase.GUIDToAssetPath(ToolkitManagerPrefabGUID);

        public static JMRThemeManager JMRThemeManager
        {
            get
            {
                if (j_JmrThemeManager == null)
                {
                    PrepareToolkitEditor();
                }
                return j_JmrThemeManager;
            }
        }

        private static string GlobalDatapath = AssetDatabase.GUIDToAssetPath(GlobalDataGUID);
        #endregion

        public static SO_GlobalData GetGlobalData()
        {
            return globalData;
        }

        public static void Init()
        {
            PrepareToolkitEditor();
        }

        static JMRToolkit()
        {
            Init();
        }

        //private class AssetPostprocessor : UnityEditor.AssetPostprocessor
        //{
        //    public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        //    {
        //        //Debug.LogError("prepare toolkit ");
        //        //if(!Application.isPlaying)
        //        Init();
        //    }
        //}

        private static void PrepareToolkitEditor()
        {
            if (j_JmrThemeManager == null)
                j_JmrThemeManager = ScriptableObject.CreateInstance<JMRThemeManager>();

            //TO DO : Use global data for all static values
            //globalData = (GlobalData)AssetDatabase.LoadAssetAtPath(GlobalDatapath, typeof(GlobalData));
            //if (globalData == null)
            //{
            //    //Debug.LogError("No data file Present in system");
            //}
        }

        public void OnUtilities()
        {
            //create Utilities window and show it
        }

        public void OnToolBox()
        {
            //create Toolbox window and show it
        }

        public void OnHelpAndUpdate()
        {
            //create Help and update window, show it
        }

        static void ImportCallback(string packageName)
        {
            PrepareToolkitEditor();
            AssetDatabase.importPackageCompleted -= ImportCallback;
        }

        private static bool IsToolkitManagerExist()
        {
            var rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            List<JMRToolkitManager> componentResult = new List<JMRToolkitManager>();

            foreach (var rootObject in rootObjects)
            {
                componentResult.AddRange(rootObject.GetComponentsInChildren<JMRToolkitManager>());
            }

            if (componentResult.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
#endif