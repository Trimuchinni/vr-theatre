// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using UnityEngine.Events;

namespace JMRSDK.Toolkit
{
    public class JMRUIPrimaryButton : JMRBaseThemeAnimator
    {
        [Header("Events")]
        [SerializeField]
        private UnityEvent onClick;
        private UnityEvent OnClickEvent;
        public UnityEvent OnClick { get { if (OnClickEvent == null) { OnClickEvent = new UnityEvent(); }return OnClickEvent;   }  private set { OnClickEvent = value; } }

        protected override void OnObjectSelect()
        {
            base.OnObjectSelect();
            onClick?.Invoke();
            OnClick?.Invoke();
        }

        protected override void OnObjectDeselect()
        {
            base.OnObjectDeselect();
            onClick?.Invoke();
            OnClick?.Invoke();
        }

        protected override void ChangeToPressed()
        {
            base.ChangeToOnselectionHoverClick();
        }

        protected override void ChangeToOnSelection()
        {
            base.ChangeToDefault();
        }

        protected override void ChangeToOnSelectionHover()
        {
            base.ChangeToHover();
        }
    }
}
