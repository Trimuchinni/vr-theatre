// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using UnityEngine.Events;
using JMRSDK.InputModule;
namespace JMRSDK.Toolkit.UI
{
    [RequireComponent(typeof(JMRInteractable))]
    public class JMRUIButton : MonoBehaviour
    {
        #region Events
        /// <summary>
        /// Exposed event on unity inspector for user to directly drag drop objects.
        /// This event will get triggered on button click.
        /// </summary>
        public UnityEvent onButtonClick;
        #endregion

        #region Private Properties

        //Cache interactable property.
        private JMRInteractable interactable;
        //Check if component is disabled or enabled.
        private bool isEnabled;

        #endregion

        #region Mono
        private void Start()
        {
            interactable = gameObject.GetComponent<JMRInteractable>();
            interactable.InputClicked += OnButtonClick;
            interactable.OnEnableChange += OnEnableChange;
            isEnabled = interactable.IsEnabled;
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

        #region Events Action

        private void OnEnableChange(bool obj)
        {
            isEnabled = obj;
        }

        private void OnButtonClick()
        {
            if (!isEnabled)
                return;

            onButtonClick?.Invoke();
        }
        #endregion
    }
}
