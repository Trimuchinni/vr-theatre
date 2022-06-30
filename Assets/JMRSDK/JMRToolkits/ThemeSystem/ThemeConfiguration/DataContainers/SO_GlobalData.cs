// Copyright (c) 2020 JioGlass. All Rights Reserved.

#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMRSDK.Toolkit
{
    public class SO_GlobalData : ScriptableObject
    {
        public string AppearTrigger = "";
        public string EnterTrigger = "";
        public string ExitTrigger = "";
        public string InteractTrigger = "";
        public string DisappearTrigger = "";

        public string iconSetPath = "";
        public string colorSetPath = "Assets/JMRToolkit/DesignSystem/ThemeConfiguration/Colors";

        public enum LayerType
        {
            Primary = 0,
            Secondry,
            Background,
            Error,
            Primarytext,
            Secondarytext,
            Backgroundtext,
            Errortext
        }
    }
}
#endif