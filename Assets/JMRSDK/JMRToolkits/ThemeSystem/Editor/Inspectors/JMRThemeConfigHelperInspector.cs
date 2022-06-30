// Copyright (c) 2020 JioGlass. All Rights Reserved.

#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using JMRSDK.Toolkit.UI;
using UnityEditor;
using UnityEngine;
using System.Linq;
using JMRSDK.Toolkit.ThemeSystem;
using TMPro;
using UnityEngine.UI;
using TMPro.EditorUtilities;

namespace JMRSDK.Toolkit.Inspector
{
	[CustomEditor(typeof(JMRThemeConfigHelper))]
	[CanEditMultipleObjects]
	internal class JMRThemeConfigHelperInspector : Editor, IThemeHandler
	{
		//Selected icon set refrence
		private SO_ThemeData themeDataRef;

        #region SerializeProperties
        private SerializedProperty j_IconStyleProp;
		private SerializedProperty j_MainLabelTextProp;
		private SerializedProperty j_IconSpriteProp;
		private SerializedProperty j_IconSpriteRendererProp;
		private SerializedProperty j_IconTextHolderProp;
        #endregion

        #region Private variables
        private JMRThemeConfigHelper j_TargetObj;
		private Transform j_TargetObjectTrans;
		private List<Transform> j_TargetObjectChilds;
		private List<LayerCategory> j_FilteredLayerData;
		
		// Coroutine to set values in editor with delay
		private EditorCoroutines.EditorCoroutines.EditorCoroutine j_EditerCoroutine;
        #endregion

        #region Constants

        private const string label_Key = "Label";
		private const string icon_Key = "Icon";
		private const string text_PropertyPath = "m_text";
		private const string mainLabelText_String = "Main Label Text";

        #endregion

        #region Editor Methods
        private void OnEnable()
		{
			j_TargetObj = (JMRThemeConfigHelper)target;

			j_MainLabelTextProp = serializedObject.FindProperty("mainLabelText");
			j_IconStyleProp = serializedObject.FindProperty("iconStyle");
			j_IconSpriteProp = serializedObject.FindProperty("iconSprite");
			j_IconSpriteRendererProp = serializedObject.FindProperty("iconSpriteRenderer");
			j_IconTextHolderProp = serializedObject.FindProperty("iconTextHolder");

			j_TargetObjectTrans = j_TargetObj.transform;
			j_TargetObjectChilds = new List<Transform>();
			j_TargetObjectTrans.GetComponentsInChildren<Transform>(j_TargetObjectChilds);

			CreateFilteredData();

            if (JMRToolkit.JMRThemeManager == null || JMRToolkit.JMRThemeManager.GetIconPreferences() == null)
                return;

            // TO DO : Remove direct dependency
            themeDataRef = JMRToolkit.JMRThemeManager.GetIconPreferences();//AssetDatabase.LoadAssetAtPath<ThemeDataSCRO>("Assets/TMRToolkit/DesignSystem/ThemeConfiguration/ScriptableObjects/ThemeDataSCRO.asset");
        
		}

		/// <summary>
		/// Draw UI for configuring icons and text
		/// </summary>
		public override void OnInspectorGUI()
		{
			EditorGUILayout.PropertyField(j_IconTextHolderProp);

			if (j_IconTextHolderProp.objectReferenceValue != null)
			{
				RenderIconTextGUI();
			}

			serializedObject.ApplyModifiedProperties();
		}

        #endregion

		/// <summary>
		/// Make GUI for icon and text selection on inspector
		/// Change the layout on selecting icon show icons from scriptable object
		/// </summary>
        private void RenderIconTextGUI()
		{
			bool labelFoldout = SessionState.GetBool(label_Key, true);
			bool iconFoldout = SessionState.GetBool(icon_Key, true);

			EditorGUILayout.PropertyField(j_IconStyleProp);

			j_TargetObj.SetIconStyle(j_TargetObj.IconStyle);

			switch (j_TargetObj.IconStyle)
			{
				case ThemeIconStyle.Text:

					using (new EditorGUI.IndentLevelScope(1))
					{
						using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
						{
							labelFoldout = EditorGUILayout.Foldout(labelFoldout, "Labels", true);

							if (labelFoldout)
							{
								EditorGUI.BeginChangeCheck();

								EditorGUILayout.PropertyField(j_MainLabelTextProp);

								if (j_MainLabelTextProp.objectReferenceValue != null)
								{
									Component mainLabelText = (Component)j_MainLabelTextProp.objectReferenceValue;
									SerializedObject labelTextObject = new SerializedObject(mainLabelText);
									SerializedProperty textProp = labelTextObject.FindProperty(text_PropertyPath);

									EditorGUILayout.PropertyField(textProp, new GUIContent(mainLabelText_String));
									EditorGUILayout.Space();

									if (EditorGUI.EndChangeCheck())
									{
										labelTextObject.ApplyModifiedProperties();
									}
								}
							}
						}
					}
					break;
				case ThemeIconStyle.Sprite:

					using (new EditorGUI.IndentLevelScope(1))
					{
						using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
						{
							iconFoldout = EditorGUILayout.Foldout(iconFoldout, icon_Key, true);
							if (iconFoldout && themeDataRef != null)
							{
								DrawIconSpriteEditor(true, themeDataRef.themeData);
							}
						}
					}
					break;
				case ThemeIconStyle.None:
					break;

				default:
					break;
			}
			SessionState.SetBool(label_Key, labelFoldout);
			SessionState.SetBool(icon_Key, iconFoldout);

			serializedObject.ApplyModifiedProperties();

			if (GUI.changed)
            {
				j_EditerCoroutine = EditorCoroutines.EditorCoroutines.StartCoroutine(SetDirtyWithDelay(), this);
			}
		}

		/// <summary>
		/// Try to force update the scene and object to refresh
		/// </summary>
		private IEnumerator SetDirtyWithDelay()
        {
			yield return new WaitForFixedUpdate();
			EditorUtility.SetDirty(j_TargetObjectTrans);
			SceneView.RepaintAll();//.Repaint();
			j_EditerCoroutine = null;
		}


		private void DrawIconSpriteEditor(bool showComponents, UI.JMRThemeIconSet iconSet)
		{
			if (showComponents)
			{
				EditorGUILayout.PropertyField(j_IconSpriteRendererProp);
			}

			Sprite currentIconSprite = null;

			if (j_IconSpriteRendererProp.objectReferenceValue != null)
			{
				currentIconSprite = ((Image)j_IconSpriteRendererProp.objectReferenceValue).sprite;
			}
			else
			{
				EditorGUILayout.HelpBox("This object has no icon sprite rendered assigned.", MessageType.Warning);
				return;
			}

			EditorGUILayout.Space();
			if (iconSet != null)
			{
				Sprite newIconSprite;
				if (iconSet.EditorDrawIconSelector(currentIconSprite, out newIconSprite, 1))
				{
					j_IconSpriteProp.objectReferenceValue = newIconSprite;
					j_TargetObj.SetSpriteIcon(newIconSprite);
				}
			}
			else
			{
				EditorGUILayout.HelpBox("No icon set assigned. You can specify custom icons set from toolkit config window.", MessageType.Info);
				EditorGUILayout.PropertyField(j_IconSpriteProp);
			}
		}

		private void RenderFilteredData()
		{
			using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
			{
				for (int dataIndex = 0; dataIndex < j_FilteredLayerData.Count; dataIndex++)
				{
					string key = j_FilteredLayerData[dataIndex].layerName;
					if (DrawSectionFoldoutWithKey(key, key, UIUtilities.BoldTitleFoldoutStyle))
					{
						DrawDivider();
						EditorGUILayout.Space();

						for (int objIndex = 0; objIndex < j_FilteredLayerData[dataIndex].layerObjectsPool.Count; objIndex++)
						{
							Color tempColor;
							if (ColorUtility.TryParseHtmlString(j_FilteredLayerData[dataIndex].colorValueHex, out tempColor))
							{
								EditorGUILayout.ColorField(j_FilteredLayerData[dataIndex].layerObjectsPool[objIndex].name, tempColor);
							}
						}
					}
				}
			}
		}

		void CreateFilteredData()
		{
            if (JMRToolkit.JMRThemeManager == null || JMRToolkit.JMRThemeManager.GetColorPreferences() == null)
                return;

			//TODO find all objects
			j_FilteredLayerData = new List<LayerCategory>();

			List<ThemeSystem.Colors> layerColorData = JMRToolkit.JMRThemeManager.GetColorPreferences();

			for (int colorIndex = 0; colorIndex < layerColorData.Count; colorIndex++)
			{
				LayerCategory tempData = new LayerCategory(layerColorData[colorIndex].name, layerColorData[colorIndex].color);
				var data = j_TargetObjectChilds.Where(x => x.tag.Equals(tempData.layerName));
				if (data.Count() > 0)
				{
					tempData.layerObjectsPool.AddRange(data);
					j_FilteredLayerData.Add(tempData);
				}
			}

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

		public bool ChangeColor(string themeColor)
		{
			return false;
		}

		public bool ChangeIcon(Sprite themeIcon)
		{
			return false;
		}

		public List<LayerCategory> GetThemeLayers()
		{
			if (j_FilteredLayerData == null) return new List<LayerCategory>();
			return j_FilteredLayerData;
		}

        public GameObject GetGameObject()
        {
            throw new System.NotImplementedException();
            
        }

	}
}
#endif