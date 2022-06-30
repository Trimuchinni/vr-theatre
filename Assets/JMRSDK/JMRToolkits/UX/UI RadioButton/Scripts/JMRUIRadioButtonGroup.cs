// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace JMRSDK.Toolkit.UI
{
    public class JMRUIRadioButtonGroup : MonoBehaviour
    {
        #region Serialized properties
        [SerializeField] private UnityEventInt onItemSelected, onItemDeselected;
        private UnityEventInt itemSelected, itemDeselected;

        public UnityEventInt OnItemSelected { get { if (itemSelected == null) itemSelected = new UnityEventInt(); return itemSelected; } set { itemSelected = value; } }
        public UnityEventInt OnItemDeselected { get { if (itemDeselected == null) itemDeselected = new UnityEventInt(); return itemDeselected; } set { itemDeselected = value; } }
        #endregion

        //List of radio buttons present in children
        private List<JMRUIRadioButton> j_RadioButtons;
        //Action when current selection changed
        private Action j_CurrentSelectedButton;
        //Selected radio button in childs
        private int j_CurrentSelectedIndex;

        #region Mono
        public void OnEnable()
        {
            j_RadioButtons = new List<JMRUIRadioButton>();
            //int i = 0;
            //foreach (JMRUIRadioButton rButton in transform.GetComponentsInChildren<JMRUIRadioButton>())
            //{
            //    j_RadioButtons.Add(rButton);
            //    rButton.RegisterForRadioGroup(i, SetCurrentButtonSelected);
            //    i++;
            //}


            JMRUIRadioButton[] childBtns = transform.GetComponentsInChildren<JMRUIRadioButton>();
            //Debug.LogError("Child count : " + childBtns.Length);
            for (int i = 0; i < childBtns.Length; i++)
            {
                //Debug.LogError("Child Conunt index : " + i);
                j_RadioButtons.Add(childBtns[i]);
                childBtns[i].RegisterForRadioGroup(i, SetCurrentButtonSelected);
            }
        }

        private void OnDisable()
        {
            if (j_RadioButtons != null)
                j_RadioButtons.Clear();
            
            JMRUIRadioButton[] childBtns = transform.GetComponentsInChildren<JMRUIRadioButton>();
            //Debug.LogError("Child count : " + childBtns.Length);
            for (int i = 0; i < childBtns.Length; i++)
            {
                //Debug.LogError("Child Conunt index : " + i);
                childBtns[i].UnRegisterForRadioGroup(i, SetCurrentButtonSelected);
            }

            if (j_CurrentSelectedButton != null)
                j_CurrentSelectedButton = null;
        }

        public void OnDestroy()
        {
            if (j_CurrentSelectedButton != null)
                j_CurrentSelectedButton = null;
        }

        #endregion

        /// <summary>
        /// Set the selected radio button index in child.
        /// </summary>
        /// <param name="index">send the message with index.</param>
        /// <param name="val">Value if true or false.</param>
        /// <param name="currentSelectedButton"> action for callback.</param>
        public void SetCurrentButtonSelected(int index, bool val, Action currentSelectedButton)
        {
            if (this.j_CurrentSelectedButton != null)
            {
                this.j_CurrentSelectedButton.Invoke();
                for (int i = 0; i < onItemDeselected.GetPersistentEventCount(); i++)
                {
                    ((MonoBehaviour)onItemDeselected.GetPersistentTarget(i)).SendMessage(onItemDeselected.GetPersistentMethodName(i), j_CurrentSelectedIndex);
                }
                OnItemDeselected?.Invoke(j_CurrentSelectedIndex);
                j_RadioButtons[j_CurrentSelectedIndex].SetDynamicValueChange(false);
            }

            this.j_CurrentSelectedButton = currentSelectedButton;
            j_CurrentSelectedIndex = index;
            if (this.j_CurrentSelectedButton != null)
            {
                for (int i = 0; i < onItemSelected.GetPersistentEventCount(); i++)
                {
                    ((MonoBehaviour)onItemSelected.GetPersistentTarget(i)).SendMessage(onItemSelected.GetPersistentMethodName(i), j_CurrentSelectedIndex);
                }
                OnItemSelected?.Invoke(j_CurrentSelectedIndex);
            }
            j_RadioButtons[index].SetDynamicValueChange(val);
        }
    }
}
