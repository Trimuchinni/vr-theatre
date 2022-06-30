// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System.Collections.Generic;

namespace JMRSDK.Toolkit.ThemeSystem
{
    internal interface IConfigData
    {
        List<Colors> GetColors();
        List<Icon> GetIcons();
    }
}
