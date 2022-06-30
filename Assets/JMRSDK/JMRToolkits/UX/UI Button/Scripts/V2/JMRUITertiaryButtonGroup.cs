// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System.Collections.Generic;
using JMRSDK.InputModule;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JMRSDK.Toolkit
{
    public class JMRUITertiaryButtonGroup : JMRBaseThemeAnimator
    {
        public bool allowSwitchOff;
        [SerializeField]
        private RectTransform content;
        private List<JMRUITertiaryButton> tertaryButtons = new List<JMRUITertiaryButton>();
        private JMRUITertiaryButton prevTertaryButton;

        protected override void OnEnable()
        {
            base.OnEnable();
            tertaryButtons.Clear();
            JMRUITertiaryButton tempTertaryButton = null;
            bool anyTertaryButtonOn = false;
            int i = 0;
            foreach (RectTransform item in content)
            {
                JMRUITertiaryButton tertaryButton = item.GetComponent<JMRUITertiaryButton>();
                if (tertaryButton)
                {
                    if (i == 0)
                    {
                        tempTertaryButton = tertaryButton;
                    }
                    if (tertaryButton.IsOn && !anyTertaryButtonOn)
                    {
                        anyTertaryButtonOn = true;
                        tempTertaryButton = tertaryButton;
                    }
                    else if (anyTertaryButtonOn) { tertaryButton.IsOn = false; }

                    tertaryButton.parentClickHandler = OnItemClick;
                    tertaryButtons.Add(tertaryButton);
                    i++;
                }
            }

            if (tempTertaryButton != null)
            {
                if (!anyTertaryButtonOn && !allowSwitchOff)
                {
                    tempTertaryButton.IsOn = true;
                }
                prevTertaryButton = tempTertaryButton;
            }
        }

        private void OnItemClick(JMRUITertiaryButton currentTertaryButton)
        {
            if (prevTertaryButton != currentTertaryButton)
            {
                if (prevTertaryButton != null)
                {
                    prevTertaryButton.IsOn = false;
                }
                currentTertaryButton.IsOn = true;
            }
            else
            {
                if (allowSwitchOff)
                {
                    currentTertaryButton.IsOn = !currentTertaryButton.IsOn;
                }
            }
            prevTertaryButton = currentTertaryButton;
        }

        public override void OnInteractableChange(bool isInteractable)
        {
            base.OnInteractableChange(isInteractable);
            if (tertaryButtons.Count > 0)
            {
                foreach (JMRUITertiaryButton tertaryButton in tertaryButtons)
                {
                    tertaryButton.interactable = isInteractable;
                }
            }
        }

        public override void OnFocusEnter()
        {
        }

        public override void OnFocusExit()
        {
        }

        public override void OnSelectClicked(SelectClickEventData eventData)
        {
        }
    }
}
