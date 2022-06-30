// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace JMRSDK.Toolkit.UI
{
    public class JMRUICheckBoxButtonGroup : MonoBehaviour
    {
        #region Serialized Properties and private fields
        [SerializeField] private UnityEventInt onItemSelected, onItemDeselected;
        private UnityEventInt itemSelected, itemDeselected;
        public UnityEventInt OnItemSelected { get { if (itemSelected == null) itemSelected = new UnityEventInt(); return itemSelected; } set { itemSelected = value; } }
        public UnityEventInt OnItemDeselected { get { if (itemDeselected == null) itemDeselected = new UnityEventInt(); return itemDeselected; } set { itemDeselected = value; } }

        private List<JMRUICheckBoxButton> j_CheckboxButtons;
        /// <summary>
        /// Selected checkbox index in children.
        /// </summary>
        private int j_CurrentSelectedIndex;
        #endregion
        #region Action
        /// <summary>
        /// Action when any checkbox in children is selected.
        /// </summary>
        private Action j_CurrentSelectedButton;
        #endregion

        #region Mono
        public void Awake()
        {
            j_CheckboxButtons = new List<JMRUICheckBoxButton>();

            //int i = 0;
            //foreach (JMRUICheckBoxButton rButton in transform.GetComponentsInChildren<JMRUICheckBoxButton>())
            //{
            //    j_CheckboxButtons.Add(rButton);
            //    rButton.RegisterForCheckBoxGroup(i, SetCurrentButtonSelected);
            //    i++;
            //}

            JMRUICheckBoxButton[] childBtns = transform.GetComponentsInChildren<JMRUICheckBoxButton>();
            for (int i = 0; i < childBtns.Length; i++)
            {
                j_CheckboxButtons.Add(childBtns[i]);
                childBtns[i].RegisterForCheckBoxGroup(i, SetCurrentButtonSelected);
            }
        }

        public void OnDestroy()
        {
            if (j_CurrentSelectedButton != null)
                j_CurrentSelectedButton = null;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Set the selected checkbox button.
        /// </summary>
        /// <param name="index"> int index of selected checkbox.</param>
        /// <param name="val">value of selected checkbox.</param>
        public void SetCurrentButtonSelected(int index, bool val)
        {
            j_CurrentSelectedIndex = index;

            if (val)
            {
                for (int i = 0; i < onItemSelected.GetPersistentEventCount(); i++)
                {
                    ((MonoBehaviour)onItemSelected.GetPersistentTarget(i)).SendMessage(onItemSelected.GetPersistentMethodName(i), j_CurrentSelectedIndex);
                }
                OnItemSelected?.Invoke(j_CurrentSelectedIndex);
            }
            else
            {
                for (int i = 0; i < onItemDeselected.GetPersistentEventCount(); i++)
                {
                    ((MonoBehaviour)onItemDeselected.GetPersistentTarget(i)).SendMessage(onItemDeselected.GetPersistentMethodName(i), j_CurrentSelectedIndex);
                }
                OnItemDeselected?.Invoke(j_CurrentSelectedIndex);
            }
            j_CheckboxButtons[index].SetDynamicValueChange(val);
        }
        #endregion
    }
}