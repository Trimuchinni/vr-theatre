// Copyright (c) 2020 JioGlass. All Rights Reserved.

namespace Tesseract.Utility
{
    public interface IMessageHandler
    {
        void HandleMessage(string command);
        void HandleMessage(string type,string msg);
    }
}
