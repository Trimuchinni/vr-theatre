// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JMRSDK.InputModule;
using System;

namespace JMRSDK.Toolkit.UI
{

    [RequireComponent(typeof(Toggle))]
    public class JMRUIPrimaryDropDownItem : JMRBaseThemeAnimator
    {
        [SerializeField]
        private Toggle toggle;
        private bool isOn;

        protected override void Update()
        {
            base.Update();

            if(isOn != toggle.isOn)
            {
                if (toggle.isOn)
                {
                    ChangeToOnSelection();
                }
                else
                {
                    base.ChangeToDefault();
                }
                isOn = toggle.isOn;
            }
        }

        public override void OnSelectClicked(SelectClickEventData eventData)
        {
        }

        public override void OnFocusEnter()
        {
            jmrThemeAnimator.Rebind();
            if (toggle.isOn)
            {
                base.ChangeToOnSelectionHover();
            }
            else
            {
                base.ChangeToHover();
            }
        }

        public override void OnFocusExit()
        {
            if (toggle.isOn)
            {
                ChangeToOnSelection();
            }
            else
            {
                base.ChangeToDefault();
            }
        }

        protected override void ChangeToDefault()
        {
        }
    }
}
