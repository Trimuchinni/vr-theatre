// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System.Collections.Generic;
using JMRSDK.InputModule;
using UnityEngine;

namespace JMRSDK.Toolkit
{
    public class JMRUIPrimaryCheckBoxButtonGroup : JMRBaseThemeAnimator
    {
        [SerializeField]
        private RectTransform content;
        private List<JMRUIPrimaryCheckBoxButton> checkBoxs = new List<JMRUIPrimaryCheckBoxButton>();

        protected override void OnEnable()
        {
            base.OnEnable();
            checkBoxs.Clear();
            foreach (RectTransform item in content)
            {
                JMRUIPrimaryCheckBoxButton checkBox = item.GetComponent<JMRUIPrimaryCheckBoxButton>();
                if (checkBox)
                {
                    checkBoxs.Add(checkBox);
                }
            }
        }

        public override void OnInteractableChange(bool isInteractable)
        {
            base.OnInteractableChange(isInteractable);
            if (checkBoxs.Count > 0)
            {
                foreach (JMRUIPrimaryCheckBoxButton checkBox in checkBoxs)
                {
                    checkBox.interactable = isInteractable;
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
