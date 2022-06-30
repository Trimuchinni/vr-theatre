// Copyright (c) 2020 JioGlass. All Rights Reserved.
//Not in use
//Developement in progress
#if UNITY_EDITOR

namespace JMRSDK.Toolkit.ThemeSystem
{
	internal interface IThemeSystem<T>
	{
		bool ApplyTheme(ThemeType type, T themeData);
        JMRThemeColorSet GetColorTheme();
    }
}
#endif