// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using UnityEngine.Events;

namespace JMRSDK.Toolkit
{
    public class JMRUISecondaryButton : JMRBaseThemeAnimator
    {
        [SerializeField]
        private bool isOn;
        public bool IsOn { get { return isOn; } set { isOn = value; } }
        [Header("Events")]
        [SerializeField]
        private UnityEvent onSelect,onDeselect;
        private UnityEvent OnSelectEvent,OnDeselectEvent;
        public UnityEvent OnSelect { get { if (OnSelectEvent == null) { OnSelectEvent = new UnityEvent(); }return OnSelectEvent;   }  private set { OnSelectEvent = value; } }
        public UnityEvent OnDeselect { get { if (OnDeselectEvent == null) { OnDeselectEvent = new UnityEvent(); } return OnDeselectEvent; } private set { OnDeselectEvent = value; } }
        public override void Awake()
        {
            base.Awake();
            isSelected = IsOn;
        }

        protected override void Update()
        {
            base.Update();
            if (isSelected != IsOn)
            {
                base.OnSelectClicked(null);
            }
        }

        protected override void OnObjectSelect()
        {
            IsOn = isSelected;
            base.OnObjectSelect();
            onSelect?.Invoke();
            OnSelect?.Invoke();
        }

        protected override void OnObjectDeselect()
        {
            IsOn = isSelected;
            base.OnObjectDeselect();
            onDeselect?.Invoke();
            OnDeselect?.Invoke();
        }
    }
}
