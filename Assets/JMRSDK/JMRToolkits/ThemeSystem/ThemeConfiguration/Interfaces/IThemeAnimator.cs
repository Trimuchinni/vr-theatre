// Copyright (c) 2020 JioGlass. All Rights Reserved.

namespace JMRSDK.Toolkit.ThemeSystem
{
    internal interface IThemeAnimator
    {
        void SetTrigger(string name);

        void OnNormal();
        void OnHighlighted();
        void OnSelected();
        void OnPressed();
        void OnDisabled();
    }
}

