// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using JMRSDK.InputModule;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace JMRSDK.Toolkit.UI
{
    [RequireComponent(typeof(JMRInteractable), typeof(Animator))]
    public class JMRUIToggle : MonoBehaviour
    {
        #region Serialized Properties and Events

        [SerializeField, Header("Property: Default state of radio button.")]
        private bool IsOn;
        public bool isOn { get => IsOn; set { IsOn = value; } }

        [SerializeField, Header("Text Component")]
        private TMP_Text j_ToggleText;

        [Header("Toggle State Strings")]
        [SerializeField] private string isOnString;
        [SerializeField] private string isOffString;

        [Header("Events")]
        [SerializeField]
        private UnityEventBool onValueChanged;
        private UnityEventBool valueChanged;
        public UnityEventBool OnValueChanged { get { if (valueChanged == null) valueChanged = new UnityEventBool(); return valueChanged; } set { valueChanged = value; } }


        #endregion

        private bool IsEnabled;
        private Animator j_Animator;
        private Vector3 pos;
        //interactable cache
        private JMRInteractable j_Interactable;
        private Toggle tg;
        private bool j_prevToggleState;

        #region Mono
        private void Start()
        {
            j_Animator = gameObject.GetComponent<Animator>();
            j_Animator.keepAnimatorControllerStateOnDisable = false;
            j_Interactable = gameObject.GetComponent<JMRInteractable>();

            j_Interactable.InputClicked += OnButtonClick;
            j_Interactable.OnEnableChange += OnEnableChange;

            if (j_ToggleText) { j_ToggleText.text = isOffString; }

            IsEnabled = j_Interactable.IsEnabled;
            if (IsOn)
                SetToggleOnOff(isOn);

        }

        private void Update()
        {
            if (j_prevToggleState == IsOn)
                return;

            SetToggleOnOff(IsOn);
        }

        private void OnEnable()
        {
            if (j_Animator)
                SetToggleOnOff(isOn);
        }

        private void OnDisable()
        {
            j_Animator.SetTrigger("Disappear");
        }

        private void OnDestroy()
        {
            if (j_Interactable)
            {
                j_Interactable.InputClicked -= OnButtonClick;
                j_Interactable.OnEnableChange -= OnEnableChange;
            }
        }
        #endregion

        #region Action Events
        private void OnEnableChange(bool obj)
        {
            IsEnabled = obj;
        }

        private void OnButtonClick()
        {
            if (!IsEnabled)
                return;

            isOn = !isOn;
            SetToggleOnOff(isOn);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Chenge the animator states of Toggle Button.
        /// </summary>
        /// <param name="currState">Toggle button state.</param>

        private void SetToggleOnOff(bool currState)
        {
            if (currState)
            {
                SetDynamicValueChange(true);
                //TO DO : Remove hardcoded trigger.
                j_Animator.SetBool("On", true);
                if (j_ToggleText) { j_ToggleText.text = isOnString; }
            }
            else
            {
                SetDynamicValueChange(false);
                //TO DO : Remove hardcoded trigger.
                j_Animator.SetBool("On", false);
                if (j_ToggleText) { j_ToggleText.text = isOffString; }
            }
            j_prevToggleState = IsOn;
        }

        /// <summary>
        /// On Toggle button state change event.
        /// </summary>
        private void SetDynamicValueChange(bool value)
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
