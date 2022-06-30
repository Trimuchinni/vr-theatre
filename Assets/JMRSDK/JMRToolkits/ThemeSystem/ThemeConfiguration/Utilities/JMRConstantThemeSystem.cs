// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;

public class JMRConstantThemeSystem
{

    internal static class JMRButtonStates
    {
        public const string APPEAR = "Appear";
        public const string DISAPPEAR = "Disappear";
        public const string DEFAULT = "Default";
        public const string HOVER = "Hover";
        public const string PRESSED = "Pressed";
        public const string DISABLED = "Disabled";
        public const string ONSELECTION = "OnSelect";
        public const string ONHOVERSELECTION = "OnHoverSelect";
        public const string ONHOVERSELECTIONCLICK = "OnClickHoverSelect";
    }

    public enum SelectableState
    {
        None,
        Appear,
        Disappear,
        Default,
        Hover,
        Disabled,
        Pressed,
        OnSelection,
        OnSelectionHover,
        OnselectionHoverClick
    }
}
