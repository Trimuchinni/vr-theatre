// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
namespace JMRSDK.Toolkit.UI
{
    public interface IDropdownItem 
    {
        void ItemDeSelected();
        void SetItemData(JMR2DDropDown dropDown,int index, string name);
    }
}
