// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System;
using System.Collections.Generic;
using JMRSDK.InputModule;
using UnityEngine;
using UnityEngine.UI;
namespace JMRSDK.Toolkit.UI
{

    [RequireComponent(typeof(Animator))]
    public class JMR2DDropDown : MonoBehaviour, ISelectClickHandler, IFocusable
    {
        [SerializeField]
        private bool Enabled = true, lastState;
        [SerializeField]
        private JMRDropDownItem dropDownItem;
        [SerializeField]
        private ScrollRect contentScroll;
        [SerializeField]
        private TMPro.TMP_Text label;
        [SerializeField]
        private List<string> options;
        [SerializeField]
        private UnityEventInt onValueChange;
        private UnityEventInt valueChange;
        public UnityEventInt OnValueChange { get { return valueChange; } set { valueChange = value; } }
        private bool isInitialized;
        private Action prevSelectedItem;
        private bool isExpanded;
        private Animator anim;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnEnable()
        {
            if (!dropDownItem)
            {
                //Debug.LogError("No Dropdown Item Assigned");
                return;
            }

            if (options.Count <= 0)
                return;

            anim = GetComponent<Animator>();

            if (!Enabled)
            {
                anim.SetTrigger("Disabled");
                return;
            }


        }

        private void Update()
        {
            if (lastState == Enabled)
                return;

            if (Enabled)
            {
                isInitialized = true;
                anim.SetTrigger("Normal");
                LoadFromList(options);
            }
            else
            {
                anim.SetTrigger("Disabled");
                isInitialized = false;
            }
            lastState = Enabled;
        }

        public void LoadFromList(List<string> optList, int selectIndex = 0)
        {
            int i = 0;
            foreach (Transform item in contentScroll.content.transform)
            {
                if (i < options.Count)
                {
                    if (!item.gameObject.activeInHierarchy)
                        item.gameObject.SetActive(true);
                    item.GetComponent<JMRDropDownItem>().SetItemData(this, i, optList[i]);
                    if (i == selectIndex)
                        item.GetComponent<JMRDropDownItem>().OnSelectClicked(null);
                    i++;
                }
                else
                    item.gameObject.SetActive(false);
            }

            if (i >= optList.Count)
                return;
            for (int j = i; j < optList.Count; j++)
            {
                if (j == selectIndex)
                    AddNewOption(j, optList[j]).OnSelectClicked(null);
                else
                    AddNewOption(j, optList[j]);
            }

            LayoutRebuilder.MarkLayoutForRebuild(contentScroll.content);

        }

        public void SetEnable(bool flag)
        {
            Enabled = flag;
            if (!Enabled)
                anim.SetTrigger("Disabled");
        }

        public JMRDropDownItem AddNewOption(int index, string option)
        {
            JMRDropDownItem newObj = Instantiate(dropDownItem);
            newObj.transform.SetParent(contentScroll.content);
            newObj.transform.localScale = Vector3.one;
            newObj.transform.localPosition = new Vector3(newObj.transform.localPosition.x, newObj.transform.localPosition.y, 0);
            // newObj.transform.SetAsLastSibling();
            newObj.SetItemData(this, index, option);
            return newObj;
        }

        public void OnSelectClicked(SelectClickEventData eventData)
        {
            if (!Enabled)
                return;
            if (isExpanded)
                Collapse();
            else
                Expand();
        }

        private void Expand()
        {

            anim.SetTrigger("Selected");
            isExpanded = true;
        }

        private void Collapse()
        {

            anim.SetTrigger("Deselected");
            isExpanded = false;
        }

        public void OnItemSelect(int selectedIndex, string name, Action selectAction)
        {
            if (prevSelectedItem != selectAction)
            {
                prevSelectedItem?.Invoke();
                prevSelectedItem = selectAction;
                label.text = name;
                for (int i = 0; i < onValueChange.GetPersistentEventCount(); i++)
                {
                    ((MonoBehaviour)onValueChange.GetPersistentTarget(i)).SendMessage(onValueChange.GetPersistentMethodName(i), selectedIndex);
                }
                OnValueChange?.Invoke(selectedIndex);
            }
            isExpanded = false;

            anim.SetTrigger("Normal");
        }

        public void OnFocusEnter()
        {
            if (!Enabled || isExpanded)
                return;
            anim.SetTrigger("Highlighted");
        }

        public void OnFocusExit()
        {
            if (!Enabled || isExpanded)
                return;

            anim.SetTrigger("Normal");
        }

        public void SelectedIndex(int index)
        {
            //Debug.LogError("Index is : " + index);
        }
    }
}
