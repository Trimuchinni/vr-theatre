// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using JMRSDK.InputModule;

namespace JMRSDK.Toolkit.UI
{
    [RequireComponent(typeof(Animator))]
    public class JMRDropDownItem : MonoBehaviour, IDropdownItem, ISelectClickHandler, IFocusable
    {
        [SerializeField]
        private GameObject selectedSprite;
        [SerializeField]
        private TMPro.TMP_Text prim;
        private int index;
        private Animator anim;
        private JMR2DDropDown dropDown;
        string currName;
        private bool isSelected;

        public void Awake()
        {
            anim = GetComponent<Animator>();
            selectedSprite.SetActive(false);
        }

        public void ItemDeSelected()
        {
            isSelected = false;
            selectedSprite.SetActive(false);
        }

        public void SetItemData(JMR2DDropDown dropDown, int index, string name)
        {
            this.dropDown = dropDown;
            this.index = index;
            currName = name;
            prim.text = name;
        }

        public void OnSelectClicked(SelectClickEventData eventData)
        {
            dropDown.OnItemSelect(index, currName, ItemDeSelected);
            if (isSelected)
                return;
            isSelected = true;
            selectedSprite.SetActive(true);
        }

        public void OnFocusEnter()
        {

            ////Debug.LogError(EventSystem.current.currentInputModule.)
            // if (EventSystem.current.ga.GetInstanceID() == gameObject.GetInstanceID())
            anim.SetTrigger("Highlighted");
        }

        public void OnFocusExit()
        {
            anim.SetTrigger("Normal");
        }
    }
}
