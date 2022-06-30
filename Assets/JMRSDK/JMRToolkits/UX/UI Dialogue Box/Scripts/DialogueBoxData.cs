// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System;

namespace JMRSDK.Toolkit.UI
{
    [Serializable]
    public class DialogueBoxData
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DialogButtonType Buttons { get; set; } = DialogButtonType.Close;
    }
}