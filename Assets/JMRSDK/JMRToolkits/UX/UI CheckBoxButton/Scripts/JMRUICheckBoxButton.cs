// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System;
using UnityEngine;
using JMRSDK.InputModule;
using System.Collections;

namespace JMRSDK.Toolkit.UI
{
    [RequireComponent(typeof(JMRInteractable), typeof(Animator))]
    public class JMRUICheckBoxButton : MonoBehaviour
    {
        #region Serailaized fields

        /// <summary>
        /// Set the default state of checkbox.
        /// </summary>
        [Header("Property")]
        [SerializeField]
        private bool IsOn;
        public bool isOn { get => IsOn; set => IsOn = value; }
        private bool prevState;

        /// <summary>
        /// Trigger event when check box value changed.
        /// </summary>
        [Header("Events")]
        [SerializeField]
        private UnityEventBool onValueChanged;
        private UnityEventBool valueChanged;
        public UnityEventBool OnValueChanged { get { if (valueChanged == null) valueChanged = new UnityEventBool(); return valueChanged; } set { valueChanged = value; } }

        #endregion

        #region Private variables
        //Cache interactable value
        private JMRInteractable interactable;

        private Animator j_Animator;
        private bool IsSelected, IsEnabled;
        private int j_CurrentIndex;

        private Action<int, bool> j_GroupParent = null;

        #endregion

        #region Mono
        private void OnEnable()
        {

            if (!interactable)
            {
                prevState = IsSelected = !IsOn;
                interactable = gameObject.GetComponent<JMRInteractable>();

                interactable.InputClicked += OnButtonClick;
                interactable.OnEnableChange += OnEnableChange;

                IsEnabled = interactable.IsEnabled;

                j_Animator = gameObject.GetComponent<Animator>();
            }

            if (j_GroupParent == null)
            {
                StartCoroutine(WaitTillAppear());
            }
            else if (IsOn)
            {
                SetCheckBoxButtonOnOff(IsOn);
            }
        }

        private IEnumerator WaitTillAppear()
        {
            j_Animator.SetTrigger("Appear");
            yield return new WaitForSeconds(0.2f);
            if (IsOn)
                SetCheckBoxButtonOnOff(IsOn);

        }

        private void OnDisable()
        {
            if (interactable)
            {
                interactable.InputClicked -= OnButtonClick;
                interactable.OnEnableChange -= OnEnableChange;
            }
            interactable = null;
            j_Animator = null;
        }

        private void Update()
        {
            if (prevState == IsOn)
                return;

            SetCheckBoxButtonOnOff(IsOn);
        }

        #endregion

        #region Events Actions
        private void OnEnableChange(bool obj)
        {
            IsEnabled = obj;
        }

        private void OnButtonClick()
        {
            if (!IsEnabled)
                return;

            SetCheckBoxButtonOnOff(!IsOn);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Chenge the animator states of check box.
        /// </summary>
        /// <param name="isOn"></param>
        public void SetCheckBoxButtonOnOff(bool isOn)
        {
            if (isOn == IsSelected)
                return;

            if (isOn)
            {
                if (j_GroupParent == null)
                    SetDynamicValueChange(true);
                else
                    j_GroupParent?.Invoke(j_CurrentIndex, true);
                j_Animator.SetBool("On", true);

            }
            else
            {
                if (j_GroupParent == null)
                    SetDynamicValueChange(false);
                else
                    j_GroupParent?.Invoke(j_CurrentIndex, false);
                j_Animator.SetBool("On", false);

            }

            IsSelected = IsOn = prevState = isOn;
        }

        /// <summary>
        /// Register child checkboxes to checkbox group.
        /// </summary>
        /// <param name="index"> Checkbox index.</param>
        /// <param name="groupParent">Chackbox group object.</param>
        public void RegisterForCheckBoxGroup(int index, Action<int, bool> groupParent)
        {
            this.j_CurrentIndex = index;
            this.j_GroupParent = groupParent;
        }

        #endregion

        #region Private methods
        private void ResetButton()
        {
            if (IsSelected == false)
                return;

            SetCheckBoxButtonOnOff(false);
        }

        public void SetDynamicValueChange(bool value)
        {
            for (int i = 0; i < onValueChanged.GetPersistentEventCount(); i++)
            {
                ((MonoBehaviour)onValueChanged.GetPersistentTarget(i)).SendMessage(onValueChanged.GetPersistentMethodName(i), value);
            }
            OnValueChanged?.Invoke(value);
        }

        public void PrintValue(bool val)
        {
            //Debug.LogError(val);
        }
        #endregion
    }
}