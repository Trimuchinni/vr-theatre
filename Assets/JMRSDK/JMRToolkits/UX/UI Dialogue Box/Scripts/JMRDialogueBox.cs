// Copyright (c) 2020 JioGlass. All Rights Reserved.
// Script responsibility is to display the data on dialog box.
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JMRSDK.Toolkit.UI
{
    public class JMRDialogueBox : JMRDialogAbstractBase
    {
        #region Properties
        [SerializeField]
        private TMP_Text titleText = null;
        public TMP_Text j_TitleText
        {
            get { return titleText; }
            set { titleText = value; }
        }

        [SerializeField]
        private TMP_Text messageText = null;
        public TMP_Text j_MessageText
        {
            get { return messageText; }
            set { messageText = value; }
        }

        private GameObject[] twoButtonSet;
        #endregion

        #region abstract methods
        /// <summary>
        /// Generate the buttons according to buttons types and buttons assighned.
        /// </summary>
        protected override void GenerateDialogueButtons()
        {
            List<DialogButtonType> buttonTypes = new List<DialogButtonType>();

            foreach (DialogButtonType buttonType in Enum.GetValues(typeof(DialogButtonType)))
            {
                if (buttonType != DialogButtonType.None && j_DialogueData.Buttons.HasFlag(buttonType))
                {
                    buttonTypes.Add(buttonType);
                }
            }

            twoButtonSet = new GameObject[2];

            List<JMRDialogueBoxButton> buttonsOnDialog = GetAllDialogButtons();

            SetButtonsActiveStates(buttonsOnDialog, buttonTypes.Count);

            if (buttonTypes.Count > 0)
            {
                for (int i = 0; i < buttonTypes.Count; ++i)
                {
                    twoButtonSet[i] = buttonsOnDialog[i].gameObject;
                    buttonsOnDialog[i].SetTitle(buttonTypes[i].ToString());
                    buttonsOnDialog[i].ButtonTypeEnum = buttonTypes[i];
                }
            }
        }

        /// <summary>
        /// Set title to dialog box.
        /// and set message to dialog box.
        /// </summary>
        protected override void SetDialogueTitleAndMessage()
        {
            if (titleText != null)
            {
                titleText.text = DialogueData.Title;
            }

            if (messageText != null)
            {
                messageText.text = DialogueData.Message;
            }
        }

        #endregion

        /// <summary>
        /// Close the dialog box on close button click.
        /// </summary>
        public void CloseDialogueBox()
        {
            OnClosed?.Invoke(DialogueData);
            gameObject.SetActive(false);
            JMRLogHandler.Log("Dialogue Box Closed = " + name);
        }

        /// <summary>
        /// Set buttons states.
        /// </summary>
        /// <param name="buttons">list of buttons used.</param>
        /// <param name="count">totel number of buttons types available.</param>
        private void SetButtonsActiveStates(List<JMRDialogueBoxButton> buttons, int buttonTypesCount)
        {
            int buttonsCount = buttons.Count;
            for (int i = 0; i < buttonsCount; i++)
            {                
                buttons[i].ParentDialog = this;
                buttons[i].gameObject.SetActive(i < buttonTypesCount ? true : false);
            }
        }

        /// <summary>
        /// Get the list of buttons available.
        /// </summary>
        /// <returns>List of buttons.</returns>
        private List<JMRDialogueBoxButton> GetAllDialogButtons()
        {
            List<JMRDialogueBoxButton> buttonsOnDialog = new List<JMRDialogueBoxButton>();
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.name == "ButtonParent")
                {
                    var buttons = child.GetComponentsInChildren<JMRDialogueBoxButton>();
                    if (buttons != null)
                    {
                        buttonsOnDialog.AddRange(buttons);
                    }
                }
            }
            return buttonsOnDialog;
        }

    }
}