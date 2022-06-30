// Copyright (c) 2020 JioGlass. All Rights Reserved.
using System.Collections.Generic;

namespace JMRSDK.Toolkit.ThemeSystem
{

    /// <summary>
    /// The design system configuration mapping class
    /// </summary>
    [System.Serializable]
    internal class ConfigData : IConfigData
    {
        public Toolboxes toolboxes;
        public List<Colors> colors;
        public List<Icon> icons;
        public string SelectedIconSetPath;

        public ConfigData ShallowCopy()
        {
            return (ConfigData)this.MemberwiseClone();
        }

        public List<Colors> GetColors()
        {
            if(colors == null)
                return new List<Colors>();

            return colors;
        }

        public List<Icon> GetIcons()
        {
            if (icons == null)
                return new List<Icon>();

            return icons;
        }
    }

    [System.Serializable]
    internal class Toolboxes
    {
        public Buttons buttons;
    }

    [System.Serializable]
    internal class Buttons
    {
        public string id;
        public string type;
        public string title;
        public string description;
        public bool _default;
        public bool[] examples;
    }

    [System.Serializable]
    public class Colors
    {
        public string name;
        public string color;
    }

    [System.Serializable]
    public class Icon
    {
        public string title;
        public string type;
        public string icons;
    }

    [System.Serializable]
    internal class JMRTKLayers
    {
        public string primaryPlate;
        public string secondaryPlate;
    }
}