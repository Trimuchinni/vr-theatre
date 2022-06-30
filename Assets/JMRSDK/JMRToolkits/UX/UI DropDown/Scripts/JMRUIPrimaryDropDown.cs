// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JMRSDK.InputModule;
using System;

namespace JMRSDK.Toolkit.UI
{

    [RequireComponent(typeof(Dropdown))]
    public class JMRUIPrimaryDropDown : JMRBaseThemeAnimator
    {
        [SerializeField]
        private TMP_Dropdown dropdown;
        private bool isDropDownExpanded,isLocalFocused,prevInteractState;

        protected override void Update()
        {
            base.Update();

            if(prevInteractState != dropdown.interactable && dropdown.interactable != interactable)
            {
                interactable = dropdown.interactable;
            }

            if (isDropDownExpanded != dropdown.IsExpanded)
            {
                isDropDownExpanded = dropdown.IsExpanded;
                if (dropdown.IsExpanded)
                {
                    base.ChangeToOnSelection();
                }
                else
                {
                    if (isLocalFocused)
                    {
                        base.ChangeToHover();
                    }
                    else
                    {
                        base.ChangeToDefault();
                    }
                }
            }
            prevInteractState = dropdown.interactable;
        }

        public override void OnInteractableChange(bool isInteractable)
        {
            base.OnInteractableChange(isInteractable);
            dropdown.interactable = isInteractable;
        }

        public override void OnSelectClicked(SelectClickEventData eventData)
        {
        }

        public override void OnFocusEnter()
        {
            isLocalFocused = true;
            if (!interactable)
            {
                return;
            }

            if (!dropdown.IsExpanded)
                base.ChangeToHover();
        }

        public override void OnFocusExit()
        {
            isLocalFocused = false;
            if (!interactable)
            {
                return;
            }

            if (!dropdown.IsExpanded)
                base.ChangeToDefault();
        }
    }
}
