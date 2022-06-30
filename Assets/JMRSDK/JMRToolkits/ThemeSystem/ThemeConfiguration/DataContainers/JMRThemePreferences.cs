// Copyright (c) 2020 JioGlass. All Rights Reserved.

#if UNITY_EDITOR

using UnityEngine;

namespace JMRSDK.Toolkit.ThemeSystem
{
    public class JMRThemePreferences : ScriptableObject
    {
        internal bool IsValueChaged = false;
        [SerializeField]
        private ConfigData j_ConfigData;

        internal ConfigData ConfigurationData
        {
            get { return j_ConfigData; }
            set
            {
                if (value != j_ConfigData)
                {
                    IsValueChaged = true;
                }
                else
                {
                    IsValueChaged = false;
                }
                j_ConfigData = value;
            }
        }


    }
}

#endif