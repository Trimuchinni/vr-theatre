// Copyright (c) 2020 JioGlass. All Rights Reserved.
#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JMRSDK.Toolkit.UI
{
	[System.Serializable]
	public class JMRThemeIconSet
	{
		[SerializeField]
		public List<Texture> iconPool;

		[SerializeField]
		private Sprite[] spriteIcons = new Sprite[0];

		public Sprite[] SpriteIcons => spriteIcons;

		private const int maxButtonSize = 75;
		private const int maxButtonsPerColumn = 6;

        /// <summary>
        /// Draws a selectable grid of icons.
        /// </summary>
        /// <returns>True if a new icon was selected.</returns>
        public bool EditorDrawIconSelector(Sprite currentSprite, out Sprite newSprite, int indentLevel = 0)
		{
			newSprite = null;

			int currentSelection = -1;
			for (int i = 0; i < iconPool.Count; i++)
			{
				if (iconPool[i] == currentSprite)
				{
					currentSelection = i;
					break;
				}
			}

			using (new EditorGUI.IndentLevelScope(indentLevel))
			{
				float height = maxButtonSize * ((float)iconPool.Count / maxButtonsPerColumn);
				var maxHeight = GUILayout.MaxHeight(height);
#if UNITY_2019_3_OR_NEWER
				int newSelection = GUILayout.SelectionGrid(currentSelection, iconPool.ToArray(), maxButtonsPerColumn, maxHeight);
#else
                var maxWidth = GUILayout.MaxWidth(maxButtonSize * maxButtonsPerColumn);
                int newSelection = GUILayout.SelectionGrid(currentSelection, spriteIconTextures, maxButtonsPerColumn, maxHeight, maxWidth);
#endif
				if (newSelection >= 0 && newSelection != currentSelection)
				{
					newSprite = Sprite.Create((Texture2D)iconPool[newSelection], new Rect(0, 0, iconPool[newSelection].width, iconPool[newSelection].height), Vector2.zero);
				}
			}

			return newSprite != null;
		}

	}
}
#endif