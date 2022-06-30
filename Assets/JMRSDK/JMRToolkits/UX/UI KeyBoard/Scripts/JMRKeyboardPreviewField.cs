// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
namespace JMRSDK.InputModule {
    public class JMRKeyboardPreviewField : MonoBehaviour,IGameObjectHide
    {
        [SerializeField]
        private TMPro.TMP_Text previewText;

        public void Hide()
        {
            previewText.text = "";
            gameObject.SetActive(false);
        }

        public void Show()
        {
            previewText.text = "";
            gameObject.SetActive(true);
        }

        public void SetPreviewText(string pTxt)
        {
            previewText.text = pTxt;
        }
    }
}
