// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using UnityEngine.Events;
using TMPro;
using JMRSDK.InputModule;

namespace JMRSDK.Toolkit
{
    public class JMRUIPrimaryDialogueBoxError : JMRBaseThemeAnimator
    {
        [SerializeField]
        private JMRUISecondaryButton closeButton;
        [SerializeField]
        private TMP_Text titleText,messageText;

        [Header("Events")]
        [SerializeField]
        private UnityEvent onClose;
        private UnityEvent OnCloseEvent;
        public UnityEvent OnClose { get { if (OnCloseEvent == null) { OnCloseEvent = new UnityEvent(); }return OnCloseEvent;   }  private set { OnCloseEvent = value; } }

        public override void Awake()
        {
            base.Awake();
            closeButton.OnSelect.AddListener(() => { onClose?.Invoke();OnClose?.Invoke(); gameObject.SetActive(false); });
            closeButton.OnDeselect.AddListener(() => { onClose?.Invoke();OnClose?.Invoke(); gameObject.SetActive(false); });
        }

        public override void OnInteractableChange(bool isInteractable)
        {
            base.OnInteractableChange(isInteractable);
            closeButton.interactable = isInteractable;
        }

        public void Show(string title,string message)
        {
            titleText.text = title;
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
