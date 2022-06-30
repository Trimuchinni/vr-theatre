// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System;
using UnityEngine;
using JMRSDK.InputModule;

namespace JMRSDK.Toolkit.UI
{
    [RequireComponent(typeof(JMRInteractable), typeof(Animator))]
    public class JMRUIRadioButton : MonoBehaviour
    {
        #region Serialized Properties and events
        [SerializeField, Header("Property: Default state of radio button.")]
        private bool IsOn;
        public bool isOn { get => IsOn; set { IsOn = value; } }
        [Header("Events")]
        [SerializeField]
        private UnityEventBool onValueChanged;
        private UnityEventBool valueChanged;
        public UnityEventBool OnValueChanged { get { if (valueChanged == null) valueChanged = new UnityEventBool(); return valueChanged; } set { valueChanged = value; } }
        #endregion

        //Cache interactable
        private JMRInteractable interactable;

        private Animator j_Animator;

        private bool IsSelected, IsEnabled;

        private Action<int, bool, Action> j_GroupParent;

        private int currentIndex;
        private bool j_prevradioState;

        private void Awake()
        {
        }

        private void OnEnable()
        {
            if (!j_Animator)
            {
                j_Animator = gameObject.GetComponent<Animator>();
                interactable = gameObject.GetComponent<JMRInteractable>();
            }
            IsEnabled = interactable.IsEnabled;
            if (IsOn && IsEnabled)
            {
                IsSelected = j_prevradioState = !IsOn;
                SetRadioButtonOnOff(IsOn);
            }
            if (j_GroupParent != null || !IsEnabled)
                return;
            j_Animator.SetTrigger("Appear");
        }

        #region Mono
        private void Start()
        {
            if (interactable)
            {
                interactable.InputClicked += OnButtonClick;
                interactable.OnEnableChange += OnEnableChange;
            }
        }

        private void Update()
        {
            if (j_prevradioState == IsOn)
                return;

            SetRadioButtonOnOff(IsOn);
        }

        private void OnDestroy()
        {
            if (interactable)
            {
                interactable.InputClicked -= OnButtonClick;
                interactable.OnEnableChange -= OnEnableChange;
            }
        }

        #endregion

        #region Events Actions
        private void OnEnableChange(bool obj)
        {
            IsEnabled = obj;
        }

        private void OnButtonClick()
        {
            if (!IsEnabled || (j_GroupParent!=null && isOn))
                return;

            SetRadioButtonOnOff(!IsSelected);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Chenge the animator states of Radio Button.
        /// </summary>
        /// <param name="state">radio button state.</param>
        public void SetRadioButtonOnOff(bool state)
        {
            if (state == IsSelected)
                return;
            if (state)
            {
                if (j_GroupParent == null)
                    SetDynamicValueChange(true);
                else
                    j_GroupParent?.Invoke(currentIndex, true, ResetButton);
                //TO DO : Remove hardcoded trigger.
                j_Animator.SetBool("On", true);
            }
            else
            {
                if (j_GroupParent == null)
                    SetDynamicValueChange(false);
                //TO DO : Remove hardcoded trigger.
                j_Animator.SetBool("On", false);
            }
            IsSelected = state;
            j_prevradioState = IsOn = state;
        }

        /// <summary>
        /// Register child checkboxes to checkbox group.
        /// </summary>
        /// <param name="index"> Checkbox index.</param>
        /// <param name="groupParent">Chackbox group object.</param>
        public void RegisterForRadioGroup(int index, Action<int, bool, Action> groupParent)
        {
            this.currentIndex = index;
            this.j_GroupParent = groupParent;
        }

        public void UnRegisterForRadioGroup(int index, Action<int, bool, Action> groupParent)
        {
            this.currentIndex = index;
            this.j_GroupParent = groupParent;
        }

        #endregion

        #region private Methods

        private void ResetButton()
        {
            if (IsSelected == false)
                return;

            SetRadioButtonOnOff(false);
        }

        public void SetDynamicValueChange(bool value)
        {
            for (int i = 0; i < onValueChanged.GetPersistentEventCount(); i++)
            {
                ((MonoBehaviour)onValueChanged.GetPersistentTarget(i)).SendMessage(onValueChanged.GetPersistentMethodName(i), value);
            }
            OnValueChanged?.Invoke(value);
        }
        #endregion
    }
}
