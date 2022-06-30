// Copyright (c) 2020 JioGlass. All Rights Reserved.
///Toolkit manager in scene is for managing the design system behaviour at runtime

using JMRSDK.Toolkit.ThemeSystem;
using JMRSDK.Toolkit.UI;
using UnityEditor;
using UnityEngine;

namespace JMRSDK.Toolkit
{
    public class JMRToolkitManager : MonoBehaviour
    {
        #region ToolkitInstance
        private static JMRToolkitManager _instance;
        public static JMRToolkitManager Instance
        {
            get => _instance;
        }
        #endregion

        [SerializeField] private JMRThemeColorSet themesColorData;

        public JMRThemeColorSet ThemeColorSet => themesColorData;

        #region Mono Callbacks
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }          
        }
        #endregion
    }
}
