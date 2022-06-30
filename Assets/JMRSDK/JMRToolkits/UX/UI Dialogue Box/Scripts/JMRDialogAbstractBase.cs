// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System;
using UnityEngine;

namespace JMRSDK.Toolkit.UI
{
    public abstract class JMRDialogAbstractBase : MonoBehaviour
    {
        // On dialog box close action.
        public Action<DialogueBoxData> OnClosed;

        protected DialogueBoxData j_DialogueData;

        public DialogueBoxData DialogueData => j_DialogueData;

        protected void DisplayDialogue(DialogueBoxData dialogBoxData)
        {
            j_DialogueData = dialogBoxData;
            GenerateDialogueButtons();
            SetDialogueTitleAndMessage();
        }

        #region Static Methods
        /// <summary>
        /// Show the dialog box.
        /// </summary>
        /// <param name="dialogueBox">GameObject refrence.</param>
        /// <param name="dialogueData">Data to display on dialog box with buttons name.</param>
        public static void OpenDialogue(GameObject dialogueBox, DialogueBoxData dialogueData)
        {
            dialogueBox.SetActive(true);
            JMRDialogAbstractBase dialogue = dialogueBox.GetComponent<JMRDialogAbstractBase>();
            if (dialogue) dialogue.DisplayDialogue(dialogueData);
            else JMRLogHandler.LogError("Missing Dialogue Script");
        }

        /// <summary>
        /// Show the dialog box.
        /// </summary>
        /// <param name="dialogueBox">GameObject refrence.</param>
        /// <param name="buttons">Type of button confirm etc.</param>
        /// <param name="title">Title to display on dialog.</param>
        /// <param name="message">Message to display on dialog.</param>
        public static void OpenDialogue(GameObject dialogueBox, DialogButtonType buttons, string title, string message)
        {
            dialogueBox.SetActive(true);
            JMRDialogAbstractBase dialogue = dialogueBox.GetComponent<JMRDialogAbstractBase>();
            if (dialogue)
            {
                DialogueBoxData dbData = new DialogueBoxData
                {
                    Buttons = buttons,
                    Title = title,
                    Message = message
                };

                dialogue.DisplayDialogue(dbData);
            }
            else
            {
                JMRLogHandler.LogError("Missing Dialogue Script");
            }
        }
        #endregion

        protected abstract void GenerateDialogueButtons();
        protected abstract void SetDialogueTitleAndMessage();
    }
}