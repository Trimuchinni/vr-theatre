// Copyright (c) 2020 JioGlass. All Rights Reserved.

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using JMRSDK.Toolkit.UI;
using System.Threading.Tasks;
using System;

namespace JMRSDK.Toolkit.ThemeSystem
{
    internal class JMRToolkitConfigurationWindow : EditorWindow
    {
        #region In Developement
        private IThemeSystem<List<Colors>> j_ColorDesignSystem;
        private IThemeSystem<List<Icon>> j_IconsDesignSystem;

        internal IThemeSystem<List<Colors>> ColorDesignSystem { get => j_ColorDesignSystem; set => j_ColorDesignSystem = value; }
        internal IThemeSystem<List<Icon>> IconsDesignSystem { get => j_IconsDesignSystem; set => j_IconsDesignSystem = value; }
        #endregion

        public System.Action applyDesignBtnEvent;

        #region private variables
        private const string j_WindowTitle = "Toolkit Configuration";

        private static List<Colors> j_CurrentColorsData;
        private static SO_ThemeData j_CurrentIconsData;

        private GUIStyle j_CenteredStyle;
        private Vector2 j_ScrollPos;
        public static Texture2D jioGlassLogo;

        private Color[] j_LayerTempPool;
        private bool isNewWindowCreated = false;
        private string j_SelectedIconSetPath;

        private static JMRToolkitConfigurationWindow j_ConfigWindow;
        private static bool j_WindowCreatedFromMenu;

        private const string LogoGUID = "80e50489150ff374daf81a630a9a2eb5";
        private static string LogoPath => AssetDatabase.GUIDToAssetPath(LogoGUID);

        #endregion

        #region Editor Window Actions

        [MenuItem("JioMixedReality/ Toolkits Configuration", false, 1)]
        static void CreateToolkitConfigWindow()
        {
            j_WindowCreatedFromMenu = true;
            if (JMRToolkit.JMRThemeManager == null)
            {
                JMRToolkit.Init();
            }


            if ((JMRToolkitConfigurationWindow)GetWindow(typeof(JMRToolkitConfigurationWindow)) == null)
            {
                j_ConfigWindow = CreateWindow<JMRToolkitConfigurationWindow>();
            }
            else
            {
                j_ConfigWindow = (JMRToolkitConfigurationWindow)GetWindow(typeof(JMRToolkitConfigurationWindow));
            }
            j_ConfigWindow.titleContent = new GUIContent(j_WindowTitle);
            j_ConfigWindow.minSize = new Vector2(600, 300);
            j_ConfigWindow.Init();
            j_ConfigWindow.Show();

            //}
            //else
            //{
            //    Debug.Log("TMRTOOLKIT THEMEMANAGER NOT FOUND ! ");
            //}
        }
        #endregion

        private void Init()
        {
            jioGlassLogo = AssetDatabase.LoadAssetAtPath<Texture2D>(LogoPath);

            if (JMRToolkit.JMRThemeManager.JMRThemeConfigurator == null || !JMRToolkit.JMRThemeManager.JMRThemeConfigurator.isInitialized)
            {
                JMRToolkit.JMRThemeManager.ReInitialize();
            }

            InitializeConfigurationWindow();
        }

        void InitializeConfigurationWindow()
        {

            //Debug.Log("Configuration window initialized");
            // initialise params such as activeProfile
            j_CurrentColorsData = JMRToolkit.JMRThemeManager.GetColorPreferences();
            j_CurrentIconsData = JMRToolkit.JMRThemeManager.GetIconPreferences();

            if (j_CurrentColorsData != null)
                j_LayerTempPool = new Color[j_CurrentColorsData.Count];
            else
                EditorUtility.DisplayDialog("Info", "Jio Mixed Reality Window Initialization failed!" + Environment.NewLine + " Press OK and reopen from menu.", "OK");

            isNewWindowCreated = true;
        }

        private void OnDestroy()
        {
            j_WindowCreatedFromMenu = false;
        }

        #region Editor Window methods
        private void OnEnable()
        {
            if (!j_WindowCreatedFromMenu || EditorApplication.isPlaying)
            {
                Delayed();
            }
        }

        async static void Delayed()
        {
            await Task.Delay(2100);
            JMRToolkitConfigurationWindow configWin = (JMRToolkitConfigurationWindow)GetWindow(typeof(JMRToolkitConfigurationWindow)); configWin.InitializeConfigurationWindow();
        }

        private void OnGUI()
        {
            BeginWindows();

            j_CenteredStyle = new GUIStyle(GUI.skin.GetStyle("Label"))
            {
                alignment = TextAnchor.UpperCenter,
                wordWrap = true
            };

            RenderJMRToolkitLogo();

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (var scrollView = new EditorGUILayout.ScrollViewScope(j_ScrollPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true)))
                {
                    j_ScrollPos = scrollView.scrollPosition;

                    RenderColorSelection();
                    RenderIconSelection();
                }
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply", GUILayout.MaxWidth(100)))
            {
                ApplyTheme(ThemeType.Color);
            }

            EndWindows();

        }
        #endregion

        private void RenderJMRToolkitLogo()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(jioGlassLogo, GUILayout.MaxHeight(96f));
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }

        public static bool DrawSectionFoldoutWithKey(string headerName, string preferenceKey = null, GUIStyle style = null, bool defaultOpen = true)
        {
            bool showPref = SessionState.GetBool(preferenceKey, defaultOpen);
            bool show = DrawSectionFoldout(headerName, showPref, style);
            if (show != showPref)
            {
                SessionState.SetBool(preferenceKey, show);
            }

            return show;
        }

        public static bool DrawSectionFoldout(string headerName, bool open = true, GUIStyle style = null)
        {
            if (style == null)
            {
                style = EditorStyles.foldout;
            }

            using (new EditorGUI.IndentLevelScope())
            {
                return EditorGUILayout.Foldout(open, headerName, true, style);
            }
        }

        public static void DrawDivider()
        {
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
        }

        private void RenderColorSelection()
        {

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                string key = "Colors";
                if (DrawSectionFoldoutWithKey(key, key, UIUtilities.BoldTitleFoldoutStyle))
                {
                    DrawDivider();
                    EditorGUILayout.Space();

                    if (j_CurrentColorsData == null)
                        return;

                    // create/update toolkit configuration windows UI and bind controls
                    if (isNewWindowCreated)
                    {
                        for (int layerIndex = 0; layerIndex < j_CurrentColorsData.Count; layerIndex++)
                        {
                            if (ColorUtility.TryParseHtmlString(j_CurrentColorsData[layerIndex].color, out j_LayerTempPool[layerIndex]))
                            {
                                j_LayerTempPool[layerIndex] = EditorGUILayout.ColorField(j_CurrentColorsData[layerIndex].name, j_LayerTempPool[layerIndex]);
                            }
                        }
                        isNewWindowCreated = false;
                    }
                    else
                    {

                        for (int layerIndex = 0; layerIndex < j_CurrentColorsData.Count; layerIndex++)
                        {
                            j_LayerTempPool[layerIndex] = EditorGUILayout.ColorField(j_CurrentColorsData[layerIndex].name, j_LayerTempPool[layerIndex]);
                            j_CurrentColorsData[layerIndex].color = "#" + ColorUtility.ToHtmlStringRGB(j_LayerTempPool[layerIndex]);
                        }
                    }
                }
            }
        }

        private void RenderIconSelection()
        {
            //if (j_CurrentIconsData == null)//|| !isGUIReady)
            //    return;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                string key = "Icons";
                if (DrawSectionFoldoutWithKey(key, key, UIUtilities.BoldTitleFoldoutStyle))
                {
                    DrawDivider();

                    EditorGUILayout.Space();
                    j_CurrentIconsData = EditorGUILayout.ObjectField("Icons", j_CurrentIconsData, typeof(SO_ThemeData), false) as SO_ThemeData;
                    j_SelectedIconSetPath = AssetDatabase.GetAssetPath(j_CurrentIconsData);
                }
            }
        }

        /// <summary>
        /// Apply color or icon theme after selection
        /// </summary>
        /// <param name="themeType"></param>
        /// <returns></returns>
        private bool ApplyTheme(ThemeType themeType)
        {
            bool success = false;

            if (j_CurrentIconsData != null)
                JMRToolkit.JMRThemeManager.SetIconPreferences(j_CurrentIconsData, j_SelectedIconSetPath);

            JMRToolkit.JMRThemeManager.ApplyColorTheme(j_CurrentColorsData);
            
            return success;
        }

    }
}
#endif