// Copyright (c) 2020 JioGlass. All Rights Reserved.

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace JMRSDK.Toolkit
{
    public static class UIUtilities
    {
        public const int TitleFontSize = 14;

        /// <summary>
        /// Default style for foldouts with bold title
        /// </summary>
        public static readonly GUIStyle BoldFoldoutStyle =
          new GUIStyle(EditorStyles.foldout)
          {
              fontStyle = FontStyle.Bold
          };

        /// <summary>
        /// Default style for foldouts with bold large font size title
        /// </summary>
        public static readonly GUIStyle BoldTitleFoldoutStyle =
          new GUIStyle(EditorStyles.foldout)
          {
              fontStyle = FontStyle.Bold,
              fontSize = TitleFontSize,
          };

        /// <summary>
        /// Default style for foldouts with large font size title
        /// </summary>
        public static readonly GUIStyle TitleFoldoutStyle =
          new GUIStyle(EditorStyles.foldout)
          {
              fontSize = TitleFontSize,
          };

        /// <summary>
        /// Default style for controller mapping buttons
        /// </summary>
        public static readonly GUIStyle ControllerButtonStyle = new GUIStyle("LargeButton")
        {
            imagePosition = ImagePosition.ImageAbove,
            fixedHeight = 128,
            fontStyle = FontStyle.Bold,
            stretchHeight = true,
            stretchWidth = true,
            wordWrap = true,
            fontSize = 10,
        };

        /// <summary>
        /// Default style for bold large font size title
        /// </summary>
        public static readonly GUIStyle BoldLargeTitleStyle = new GUIStyle(EditorStyles.largeLabel)
        {
            fontSize = TitleFontSize,
            fontStyle = FontStyle.Bold,
        };
    }
}
#endif