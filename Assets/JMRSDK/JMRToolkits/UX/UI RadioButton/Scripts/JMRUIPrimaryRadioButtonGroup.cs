// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System;
using System.Collections.Generic;
using JMRSDK.InputModule;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JMRSDK.Toolkit
{
    public class JMRUIPrimaryRadioButtonGroup : JMRBaseThemeAnimator
    {
        public bool allowSwitchOff;
        [SerializeField]
        private RectTransform content;
        private List<JMRUIPrimaryRadioButton> radioButtons = new List<JMRUIPrimaryRadioButton>();
        private JMRUIPrimaryRadioButton prevRadioButton;

        protected override void OnEnable()
        {
            base.OnEnable();
            radioButtons.Clear();
            JMRUIPrimaryRadioButton tempRadioButton = null;
            bool anyRadioButtonOn = false;
            int i = 0;
            foreach (RectTransform item in content)
            {
                JMRUIPrimaryRadioButton radioButton = item.GetComponent<JMRUIPrimaryRadioButton>();
                if (radioButton)
                {
                    if (i == 0)
                    {
                        tempRadioButton = radioButton;
                    }
                    if (radioButton.IsOn && !anyRadioButtonOn)
                    {
                        anyRadioButtonOn = true;
                        tempRadioButton = radioButton;
                    }
                    else if(anyRadioButtonOn) { radioButton.IsOn = false; }
                    radioButton.parentClickHandler = OnItemClick;
                    radioButtons.Add(radioButton);
                    i++;
                }
            }

            if(tempRadioButton != null)
            {
                if(!anyRadioButtonOn && !allowSwitchOff)
                {
                    tempRadioButton.IsOn = true;
                }
                prevRadioButton = tempRadioButton;
            }
        }

        public void OnItemClick(JMRUIPrimaryRadioButton currentRadioButton)
        {
            if (prevRadioButton != currentRadioButton)
            {
                if(prevRadioButton != null)
                {
                    prevRadioButton.IsOn = false;
                }
                currentRadioButton.IsOn = true;
            }
            else
            {
                if (allowSwitchOff)
                {
                    currentRadioButton.IsOn = !currentRadioButton.IsOn;
                }
            }
            prevRadioButton = currentRadioButton;
        }

        public override void OnInteractableChange(bool isInteractable)
        {
            base.OnInteractableChange(isInteractable);
            if (radioButtons.Count > 0)
            {
                foreach (JMRUIPrimaryRadioButton radioButton in radioButtons)
                {
                    radioButton.interactable = isInteractable;
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
