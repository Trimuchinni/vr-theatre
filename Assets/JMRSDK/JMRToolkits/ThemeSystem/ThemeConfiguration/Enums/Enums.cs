// Copyright (c) 2020 JioGlass. All Rights Reserved.

namespace JMRSDK.Toolkit.ThemeSystem
{
    public enum ThemeLayer
    {
        PrimaryLayer,
        SecondaryLayer,
        SurfaceLayer,
        BackgroundLayer,
        ErrorLayer,
        PrimaryTextLayer,
        SecondaryTextLayer,
        BackgroundTextLayer,
        ErrorTextLayer,
        DisabledLayer,
        SurfaceTextLayer,
        DisabledTextLayer,
        HoverLayer
    }

    public enum ThemeIconStyle
    {
        /// <summary>
        /// Icon texture is set by changing the sprite.
        /// </summary>
        Sprite,
        /// <summary>
        /// Icon texture is set by changing the character.
        /// </summary>
        Text,
        /// <summary>
        /// Hides the icons.
        /// </summary>
        None,
    }

}