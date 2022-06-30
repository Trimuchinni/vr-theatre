// Copyright (c) 2020 JioGlass. All Rights Reserved.
// Theme colors to be used at runtime
// Remaining colors we ar setting at compile time using preferences.

using UnityEngine;

namespace JMRSDK.Toolkit.ThemeSystem
{
    [CreateAssetMenu(fileName = "ColorSet", menuName = "CreateColorSet", order = 1)]
    public class JMRThemeColorSet : ScriptableObject
    {
        // layers color
        [SerializeField]
        internal string primaryDisabledColorHex = "#CCCCCC", secondaryDisabledColorhex = "#D8D8D8", disabledTextColorHex = "#727272";

        public string GetPrimaryDisableHex => primaryDisabledColorHex;
        public string GetSecondaryDisableHex => secondaryDisabledColorhex;

        public string GetDisableTextHex => disabledTextColorHex;
    }
}
