// Copyright (c) 2020 JioGlass. All Rights Reserved.

namespace JMRSDK.Toolkit.ThemeSystem
{
    public interface IActionState
    {
        void OnDefaultState();
        void OnHoverState();
        void OnPressedState();
        void OnDisabledState();
        void SetTrigger(string trigger);
    }
}