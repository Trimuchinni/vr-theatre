// Copyright (c) 2020 JioGlass. All Rights Reserved.

using JMRSDK.Toolkit.ThemeSystem;
using UnityEngine;
using UnityEngine.Events;
using JMRSDK.InputModule;
using System.Collections;
using System;

namespace JMRSDK.Toolkit
{
    [RequireComponent(typeof(Animator))]
    internal class JMRThemeAnimator : MonoBehaviour, IThemeAnimator
    {
        #region Constants
        // Animation controlled by JMRSDK internally.
        private const string JMRAppearTrigger = "Appear";
        private const string JMRDisappearTrigger = "Disappear";
        private const string JMRDisableTrigger = "Disabled";

        #endregion

        [SerializeField, Header("Property")]
        private JMRInteractableBase interactable;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onFocusEnter, onFocusExit, onPointerDown, onPointerUp, onPointerSelect;

        private Animator themeAnimator;
        private bool IsEnable;

        private Coroutine j_AnimationCoroutine;

        #region Mono

        private void Awake()
        {
            interactable = gameObject.GetComponent<JMRInteractableBase>();
            if (interactable)
            {
                interactable.FocusEnter += OnFocusEnter;
                interactable.FocusExit += OnFocusExit;
                interactable.InputClicked += OnInputClicked;
                interactable.InputDown += OnPointerDown;
                interactable.InputUp += OnPointerUp;
                interactable.InputClicked += OnPointerSelect;

                interactable.OnEnableChange += OnEnableChange;
            }
            themeAnimator = gameObject.GetComponent<Animator>();
        }

        private void Start()
        {

            //TO DO: Remove hardcoded animations names
        }


        private void OnEnable()
        {
            if (interactable)
                IsEnable = interactable.IsEnabled;
            if (!IsEnable)
                themeAnimator.SetTrigger(JMRDisableTrigger);

            //if(themeAnimator != null)
            //    themeAnimator.SetTrigger(JMRAppearTrigger);
        }

        // TO DO : Before disabling the button play it.
        private void OnDisable()
        {
            //gameObject.SetActive(true);
            //themeAnimator.SetTrigger(JMRDisappearTrigger);

            if (IsEnable)
            {

                //if(j_AnimationCoroutine != null)
                //{
                //    StopCoroutine(j_AnimationCoroutine);
                //}
                // j_AnimationCoroutine = StartCoroutine(HoldForAnimationToComplete());
            }
        }

        private void OnDestroy()
        {
            if (interactable == null) return;

            interactable.FocusEnter -= OnFocusEnter;
            interactable.FocusExit -= OnFocusExit;
            interactable.InputDown -= OnPointerDown;
            interactable.InputUp -= OnPointerUp;
            interactable.InputClicked -= OnPointerSelect;
            interactable.OnEnableChange -= OnEnableChange;
            interactable.InputClicked -= OnInputClicked;
        }
        #endregion

        private void OnInputClicked()
        {
            if (!IsEnable)
                return;

            OnPressed();
        }

        private void OnEnableChange(bool obj)
        {
            IsEnable = obj;
        }

        #region PointerActions
        /// <summary>
        /// On Controller button released this method will get triggered
        /// </summary>
        private void OnPointerUp()
        {
            if (!IsEnable)
                return;

            onPointerUp?.Invoke();
        }

        /// <summary>
        /// On Controller button press down this method will get triggered.
        /// </summary>
        private void OnPointerDown()//
        {
            if (!IsEnable)
                return;
            onPointerDown?.Invoke();
        }

        /// <summary>
        /// On Controller button click this method will get triggered.
        /// </summary>
        private void OnPointerSelect()
        {
            if (!IsEnable)
                return;
            onPointerSelect?.Invoke();
        }


        /// <summary>
        /// Once object is not in focus of pointer(Gaze) this method will get triggered
        /// </summary>
        private void OnFocusExit()
        {
            //Debug.Log("On focus exit called");

            if (!IsEnable)
                return;

            OnNormal();
        }

        /// <summary>
        /// Once object focused of pointer(Gaze) this method will get triggered
        /// </summary>
        private void OnFocusEnter()
        {
            //Debug.Log("On focus enter called");
            if (!IsEnable)
                return;
            OnHighlighted();
        }

        #endregion

        #region Public Methods
        public void SetTrigger(string name)
        {
            if (themeAnimator == null) return;

            themeAnimator.SetTrigger(name);
        }
        public void SetBool(string name)
        {
            if (themeAnimator == null) return;

            themeAnimator.SetTrigger(name);
        }

        // TO DO: Remove refrence animators
        public void OnNormal()
        {
            onFocusExit?.Invoke();
        }

        public void OnHighlighted()
        {
            onFocusEnter?.Invoke();
        }

        public void OnSelected()
        {
        }

        public void OnPressed()
        {
        }

        public void OnDisabled()
        {
        }
        #endregion

        #region IEnumerators
        private IEnumerator HoldForAnimationToComplete()
        {
            yield return new WaitForSeconds(0.5f);
            gameObject.SetActive(false);
        }
        #endregion
    }
}