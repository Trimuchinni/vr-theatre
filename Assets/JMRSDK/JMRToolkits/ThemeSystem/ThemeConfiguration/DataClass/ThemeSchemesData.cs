// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;
using System;

namespace JMRSDK.Toolkit.ThemeSystem
{
    public enum ThemeType
    {
        None,
        Color,
        Icon
    }

    [Serializable]
    public partial class ColorsScheme
    {
        public Color JMRtkPrimaryColor;
        public Color JMRtkSecondryColor;
        public Color JMRtkBackgroundColor;
        public Color JMRtkErrorColor;
        public Color JMRtkPrimaryTextColor;
        public Color JMRtkSecondryTextColor;
        public Color JMRtkBackgroundTextColor;
        public Color JMRtkErrorTextColor;
    }

    [Serializable]
    public partial class iconsScheme
    {
        //public ThemeIconSet TmrtkDarkIconsSet;
    }

    [System.Serializable]
    public class LayerCategory
    {
        public string layerName, colorValueHex;
        public List<Transform> layerObjectsPool;

        public LayerCategory(string layerName, string colorValueHex)
        {
            this.layerName = layerName;
            this.colorValueHex = colorValueHex;
            this.layerObjectsPool = new List<Transform>();
        }
    }
}
