// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using System.Collections.Generic;

namespace JMRSDK.Toolkit.ThemeSystem
{
    // Temporary Interface. To be replaced by actual interface used.
    public interface IThemeHandler
    {
        bool ChangeColor(string themeColor);
        bool ChangeIcon(Sprite themeIcon);
        List<LayerCategory> GetThemeLayers();
        GameObject GetGameObject();
    }
}
