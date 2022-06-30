// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using TMPro;
using Tesseract.Utility;
using JMRSDK.InputModule;
using UnityEngine.EventSystems;

namespace JMRSDK.Toolkit.UI
{

    public class JMRVirtualKeyboardKeys : MonoBehaviour, IMessageHandler, IPointerDownHandler, IFocusable
    {
        #region SERIALIZED FIELDS
        [SerializeField]
        private JMRVirtualKeyBoard keyBoard;
        [SerializeField]
        private TextMeshProUGUI keyText, highlightText;
        [SerializeField]
        private string content;
        [SerializeField]
        private bool isAction;
        #endregion

        #region PRIVATE FIELDS
        private Animator j_animator;
        private float j_timer;
        private UnityEngine.UI.Button j_clickBtn;
        #endregion

        #region MONO METHODS
        private void Start()
        {
            j_animator = gameObject.GetComponent<Animator>();
            if (!keyBoard)
            {
                //Debug.LogError("Keyboard variable Un assigned");
                return;
            }
            keyBoard.RegisterKey(this);
        }

        private void LateUpdate()
        {
            if (j_timer <= 0)
                return;

            j_timer -= Time.deltaTime;
            if(j_timer<=0)
                j_animator.SetTrigger("Default");
        }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Handle Keyboard Actions.
        /// </summary>
        /// <param name="command"></param>
        public void HandleMessage(string command)
        {
            if (keyText != null)
            {
                switch (command)
                {
                    case Constants.CASE_UPPER:
                        if (isAction)
                            break;
                        try
                        {
                            keyText.text = keyText.text.ToUpper();
                        }
                        catch (System.Exception) { }//Debug.LogError(gameObject.name); }
                        break;
                    case Constants.CASE_LOWER:
                        if (isAction)
                            break;
                        keyText.text = keyText.text.ToLower();
                        break;
                }
            }
            if (highlightText != null) 
                highlightText.text = keyText.text;
        }

        /// <summary>
        /// Handle Message from Keyboard.
        /// </summary>
        /// <param name="type",name="msg"></param>
        public void HandleMessage(string type, string msg)
        {
        }

        private void RestTriggers()
        {
            j_animator.ResetTrigger("Pressed");
            j_animator.ResetTrigger("Default");
            j_animator.ResetTrigger("Hover");

        }

        /// <summary>
        /// Handle Focus Enter From Inputmodule.
        /// </summary>
        public void OnFocusEnter()
        {
            RestTriggers();
            j_timer = 0;
            j_animator.SetTrigger("Hover");
            //Trigger Haptics
            //JMRInteractionManager.Instance.TriggerHaptics(JMRInteractionManager.Instance.HAPTICS_HOVER, JMRInteractionManager.Instance.HAPTICS_INTENSITY_MEDIUM, 0);
        }

        /// <summary>
        /// Handle Focus Exit From Inputmodule.
        /// </summary>
        public void OnFocusExit()
        {
            RestTriggers();
            j_timer = 0f;
            j_animator.SetTrigger("Default");
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            RestTriggers();
            string res = isAction ? content : keyText.text;
            keyBoard.HandleMessage(res);
            j_animator.SetTrigger("Pressed");
        }

        #endregion
    }
}
