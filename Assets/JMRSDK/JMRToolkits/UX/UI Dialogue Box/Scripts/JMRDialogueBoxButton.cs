// Copyright (c) 2020 JioGlass. All Rights Reserved.

using TMPro;
using UnityEngine;

namespace JMRSDK.Toolkit.UI
{
    public class JMRDialogueBoxButton : MonoBehaviour
    {
        #region Properties
        [SerializeField]
        private TMP_Text buttonText;
        public TMP_Text ButtonText
        {
            get => buttonText;
            set => buttonText = value;
        }

        public JMRDialogueBox ParentDialog { get; set; }

        public DialogButtonType ButtonTypeEnum;

        #endregion

        /// <summary>
        /// Action on button click.
        /// </summary>
        /// <param name="obj">Gamobject refrence.</param>
        public void OnButtonClicked(GameObject obj)
        {
            if (ParentDialog != null)
            {
                //ParentDialog.CloseDialogueBox();
            }
        }

        /// <summary>
        /// Set button title.
        /// </summary>
        /// <param name="title">Title string.</param>
        public void SetTitle(string title)
        {
            if (ButtonText)
            {
                ButtonText.text = title;
            }
        }
    }
}