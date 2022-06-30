// Copyright (c) 2020 JioGlass. All Rights Reserved.

using JMRSDK.InputModule;
using UnityEngine;

namespace JMRSDK.Toolkit
{
    public class JMRUIInteractable : MonoBehaviour
    {
        public bool interactable = true;
        public bool global = false;
        protected bool isSelected = false;
        protected bool isFocused = false;

        public bool GetIsFocused()
        {
            return isFocused;
        }

        public virtual void OnSelectClicked(SelectClickEventData eventData)
        {

        }
    }
}
