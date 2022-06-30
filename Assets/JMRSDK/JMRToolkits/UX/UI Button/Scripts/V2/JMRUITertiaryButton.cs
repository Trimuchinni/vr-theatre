// Copyright (c) 2020 JioGlass. All Rights Reserved.

using JMRSDK.InputModule;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace JMRSDK.Toolkit
{
    public class JMRUITertiaryButton : JMRBaseThemeAnimator
    {
        [SerializeField]
        private bool isOn;
        public bool IsOn { get { return isOn; } set { isOn = value; } }
        [Header("Events")]
        [SerializeField]
        private UnityEventBool onValueChanged;
        [SerializeField]
        public UnityEvent OnSelect, OnDeselect;
        private UnityEventBool valueChanged;
        public UnityEventBool OnValueChanged { get { if (valueChanged == null) valueChanged = new UnityEventBool(); return valueChanged; } set { valueChanged = value; } }
        public Action<JMRUITertiaryButton> parentClickHandler;

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
            OnSelect?.Invoke();
            SetDynamicValueChange(true);
        }

        protected override void OnObjectDeselect()
        {
            IsOn = isSelected;
            base.OnObjectDeselect();
            OnDeselect?.Invoke();
            SetDynamicValueChange(false);
        }

        public override void OnSelectClicked(SelectClickEventData eventData)
        {
            if (parentClickHandler == null)
            {
                base.OnSelectClicked(eventData);
            }
            else
            {
                parentClickHandler.Invoke(this);
            }
        }

        private void SetDynamicValueChange(bool value)
        {
            if (onValueChanged != null)
            {
                for (int i = 0; i < onValueChanged.GetPersistentEventCount(); i++)
                {
                    ((MonoBehaviour)onValueChanged.GetPersistentTarget(i)).SendMessage(onValueChanged.GetPersistentMethodName(i), value);
                }
            }
            OnValueChanged?.Invoke(value);
        }
    }
}
