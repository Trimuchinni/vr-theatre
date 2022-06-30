// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;
using JMRSDK.Toolkit.ThemeSystem;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;
using JMRSDK.InputModule;
using System.Collections;

namespace JMRSDK.Toolkit.UI
{
    [ExecuteAlways]
    public class JMRThemeConfigHelper : MonoBehaviour, IThemeHandler
    {
        [SerializeField, Tooltip("Optional main label used by UIElement.")]
        private TextMeshProUGUI mainLabelText = null;

        [Header("Sprite Icon")]
        [SerializeField, Tooltip("Optional sprite renderer used for icon.")]
        private Image iconSpriteRenderer = null;

        [SerializeField, Tooltip("Optional sprite used for icon.")]
        private Sprite iconSprite = null;

        [SerializeField, Tooltip("Icon&Text Holder")]
        private Transform iconTextHolder = null;

        [SerializeField, Tooltip("How the icon should be rendered.")]
        private ThemeIconStyle iconStyle = ThemeIconStyle.None;

        private Color cText, cSecondary, cPrimary;

        #region Private Variables
        private JMRInteractable j_Interactable;

        private List<Transform> j_TargetObjectChilds;

        [SerializeField]
        private List<LayerCategory> j_FilteredLayersList;

        #endregion

        /// <summary>
        /// Return filtered layers list
        /// </summary>
        public List<LayerCategory> FilteredLayersList
        {
            set { j_FilteredLayersList = value; }
        }

        /// <summary>
        /// Selected icon style
        /// </summary>
		public ThemeIconStyle IconStyle
        {
            get { return iconStyle; }
            set
            {
                iconStyle = value;
            }
        }

        #region Mono
        void Awake()
        {
            //TODO find all objects
            j_Interactable = GetComponent<JMRInteractable>();
            cText = cSecondary = cPrimary = Color.clear;

            if (j_Interactable != null)
            {
                j_FilteredLayersList = new List<LayerCategory>();

                j_TargetObjectChilds = new List<Transform>();

                InteractableFilter(transform);

                CreateFilteredData();

                j_Interactable.OnEnableChange += OnEnableChange;

                //OnEnableChange(j_Interactable.IsEnabled);
            }

        }


        private void InteractableFilter(Transform parent)
        {
            int childCount = parent.childCount;

            if (childCount <= 0)
                return;

            for (int i = 0; i < childCount; i++)
            {
                Transform temp = parent.GetChild(i);
                JMRInteractable tempInteract = temp.GetComponent<JMRInteractable>();
                if (tempInteract == null)
                {
                    j_TargetObjectChilds.Add(temp);
                    InteractableFilter(temp);
                }
            }
        }

        void OnDestroy()
        {
            if (j_Interactable != null)
                j_Interactable.OnEnableChange -= OnEnableChange;
        }

        #endregion

        private void OnEnableChange(bool isEnabled)
        {
            //Debug.LogError("Enable Changed : " + isEnabled);
            StartCoroutine(WaitForEndOfFrame(isEnabled));
        }

        private IEnumerator WaitForEndOfFrame(bool isEnabled)
        {
            yield return new WaitForEndOfFrame();
            ApplyEnableDisableTheme(isEnabled);
        }


        #region Disable Action

        void CreateFilteredData()
        {
            foreach (int i in Enum.GetValues(typeof(ThemeLayer)))
            {
                LayerCategory tempData = new LayerCategory(((ThemeLayer)i).ToString(), string.Empty);
                var data = j_TargetObjectChilds.Where(x => x.tag.Equals(tempData.layerName));
                if (data.Count() > 0)
                {
                    tempData.layerObjectsPool.AddRange(data);
                    j_FilteredLayersList.Add(tempData);
                }
            }

        }

        private void ApplyEnableDisableTheme(bool IsEnabled)
        {
            if (JMRToolkitManager.Instance == null)
                return;

            for (int i = 0; i < j_FilteredLayersList.Count; i++)
            {
                if (!IsEnabled)
                    ApplyDisabledTheme(j_FilteredLayersList[i].layerObjectsPool, j_FilteredLayersList[i].layerName);
                else
                    ApplyEnabledTheme(j_FilteredLayersList[i].layerObjectsPool, j_FilteredLayersList[i].layerName);
            }
        }

        private void ApplyDisabledTheme(List<Transform> transforms, string layerName)
        {
            for (int i = 0; i < transforms.Count; i++)
            {
                if (layerName.Contains("Text"))
                {
                    if (cText == null || ColorUtility.ToHtmlStringRGB(cText) != JMRToolkitManager.Instance.ThemeColorSet.GetDisableTextHex)
                        cText = GetTextColor(transforms[i]);
                    JMRColorTheme.SetColor(transforms[i], JMRToolkitManager.Instance.ThemeColorSet.GetDisableTextHex);
                }
                else if (layerName.Contains("Secondary"))
                {
                    if (cSecondary == null || ColorUtility.ToHtmlStringRGB(cSecondary) != JMRToolkitManager.Instance.ThemeColorSet.GetDisableTextHex)
                        cSecondary = GetRendererColor(transforms[i]);
                    JMRColorTheme.SetColor(transforms[i], JMRToolkitManager.Instance.ThemeColorSet.GetSecondaryDisableHex);
                }
                else
                {
                    if (cPrimary == null || ColorUtility.ToHtmlStringRGB(cPrimary) != JMRToolkitManager.Instance.ThemeColorSet.GetPrimaryDisableHex)
                        cPrimary = GetRendererColor(transforms[i]);
                    JMRColorTheme.SetColor(transforms[i], JMRToolkitManager.Instance.ThemeColorSet.GetPrimaryDisableHex);
                }
            }
        }

        private void ApplyEnabledTheme(List<Transform> transforms, string layerName)
        {
            for (int i = 0; i < transforms.Count; i++)
            {
                if (cText != Color.clear &&  layerName.Contains("Text"))
                {
                    SetTextColor(transforms[i], cText);
                }
                else if (cSecondary != Color.clear && layerName.Contains("Secondary"))
                {
                    SetRendererColor(transforms[i], cSecondary);
                }
                else if(cPrimary != Color.clear)
                {
                    SetRendererColor(transforms[i], cPrimary);
                }
            }
        }


        private void SetTextColor(Transform obj, Color clr)
        {
            TextMeshProUGUI temp2 = obj.GetComponent<TextMeshProUGUI>();
            if (temp2)
            {
                temp2.color = clr;
                return;
            }
            TextMeshPro temp3 = obj.GetComponent<TextMeshPro>();
            if (temp3)
            {
                temp3.color = clr;
                return;
            }
            Text temp1 = obj.GetComponent<Text>();
            if (temp1)
            {
                temp1.color = clr;
                return;
            }

        }

        private void SetRendererColor(Transform obj, Color clr)
        {
            Image temp1 = obj.GetComponent<Image>();
            if (temp1)
            {
                temp1.color = clr;
                return;
            }
            SpriteRenderer temp2 = obj.GetComponent<SpriteRenderer>();
            if (temp2)
            {
                temp2.color = clr;
                return;
            }
            MeshRenderer temp3 = obj.GetComponent<MeshRenderer>();
            if (temp3)
            {
                temp3.material.color = clr;
                return;
            }
        }

        private Color GetTextColor(Transform obj)
        {
            TextMeshProUGUI temp2 = obj.GetComponent<TextMeshProUGUI>();
            if (temp2)
                return temp2.color;
            TextMeshPro temp3 = obj.GetComponent<TextMeshPro>();
            if (temp3)
                return temp3.color;
            Text temp1 = obj.GetComponent<Text>();
            if (temp1)
                return temp1.color;
            return new Color(1, 1, 1);
        }

        private Color GetRendererColor(Transform obj)
        {
            Image temp1 = obj.GetComponent<Image>();
            if (temp1)
                return temp1.color;
            SpriteRenderer temp2 = obj.GetComponent<SpriteRenderer>();
            if (temp2)
                return temp2.color;
            MeshRenderer temp3 = obj.GetComponent<MeshRenderer>();
            if (temp3)
                return temp3.material.color;
            return new Color(1, 1, 1);
        }


        #endregion

        public void SetSpriteIcon(Sprite newIconSprite)
        {
            if (newIconSprite == null)
            {
                return;
            }

            if (iconSpriteRenderer == null)
            {
                Debug.LogWarning("No icon sprite renderer in " + name + " - not setting custom icon sprite.");
                return;
            }

            SetIconStyle(ThemeIconStyle.Sprite);

            iconSprite = newIconSprite;

            if (iconSpriteRenderer.sprite != iconSprite)
            {
                iconSpriteRenderer.sprite = newIconSprite;
            }

        }

        public void SetText(string newText)
        {
            if (newText == null)
                return;

            SetIconStyle(ThemeIconStyle.Text);

            if (mainLabelText != null)
            {
                mainLabelText.text = newText;
            }
        }

        public void SetIconStyle(ThemeIconStyle newStyle)
        {
            iconStyle = newStyle;
            switch (IconStyle)
            {
                case ThemeIconStyle.Text:
                    if (mainLabelText != null) { mainLabelText.gameObject.SetActive(true); }
                    if (iconSpriteRenderer != null) { iconSpriteRenderer.gameObject.SetActive(false); }

                    break;

                case ThemeIconStyle.Sprite:
                    if (mainLabelText != null) { mainLabelText.gameObject.SetActive(false); }
                    if (iconSpriteRenderer != null) { iconSpriteRenderer.gameObject.SetActive(true); }

                    break;

                case ThemeIconStyle.None:
                    if (mainLabelText != null) { mainLabelText.gameObject.SetActive(false); }
                    if (iconSpriteRenderer != null) { iconSpriteRenderer.gameObject.SetActive(false); }

                    break;
            }
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
            if (j_FilteredLayersList == null) return new List<LayerCategory>();
            return j_FilteredLayersList;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}
