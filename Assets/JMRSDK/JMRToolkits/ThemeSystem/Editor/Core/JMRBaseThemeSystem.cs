// Copyright (c) 2020 JioGlass. All Rights Reserved.
// Not in use
// part of design system  still in developement

#if UNITY_EDITOR

using System.Collections.Generic;

namespace JMRSDK.Toolkit.ThemeSystem
{
	internal class JMRBaseThemeSystem : IThemeSystem<List<Colors>>
	{
        List<Colors> selectedColorsTheme;

        JMRThemeColorSet colorSet;

        public JMRBaseThemeSystem()
        {
            
        }

        public bool ApplyTheme(ThemeType type, List<Colors> themeData)
        {
            return true;
        }

        public JMRThemeColorSet GetColorTheme()
        {
            return colorSet;
        }

        //public object OpenIconSelector(object themeData)
        //{
        //    //show the icon selector window
        //}
    }
}
#endif