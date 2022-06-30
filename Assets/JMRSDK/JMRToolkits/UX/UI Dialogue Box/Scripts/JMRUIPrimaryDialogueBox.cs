// Copyright (c) 2020 JioGlass. All Rights Reserved.

using JMRSDK.InputModule;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace JMRSDK.Toolkit
{
    public class JMRUIPrimaryDialogueBox : JMRBaseThemeAnimator
    {
        [SerializeField]
        private JMRUISecondaryButton yesButton, noButton;
        [SerializeField]
        private TMP_Text messageText;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onYes;
        private UnityEvent OnYesEvent;
        public UnityEvent OnYes { get { if (OnYesEvent == null) { OnYesEvent = new UnityEvent(); }return OnYesEvent;   }  private set { OnYesEvent = value; } }
        [SerializeField]
        private UnityEvent onNo;
        private UnityEvent OnNoEvent;
        public UnityEvent OnNo { get { if (OnNoEvent == null) { OnNoEvent = new UnityEvent(); }return OnNoEvent;   }  private set { OnNoEvent = value; } }

        public override void Awake()
        {
            base.Awake();
            yesButton.OnSelect.AddListener(() => { onYes?.Invoke();OnYes?.Invoke(); gameObject.SetActive(false); });
            noButton.OnSelect.AddListener(() => { onNo?.Invoke();OnNo?.Invoke(); gameObject.SetActive(false); });
            yesButton.OnDeselect.AddListener(() => { onYes?.Invoke();OnYes?.Invoke(); gameObject.SetActive(false); });
            noButton.OnDeselect.AddListener(() => { onNo?.Invoke();OnNo?.Invoke(); gameObject.SetActive(false); });
        }

        public override void OnInteractableChange(bool isInteractable)
        {
            base.OnInteractableChange(isInteractable);
            yesButton.interactable = isInteractable;
            noButton.interactable = isInteractable;
        }

        public void Show(string message)
        {
            messageText.text = message;
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
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
