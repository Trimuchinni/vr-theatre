// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using JMRSDK.InputModule;
using System.Collections;

namespace JMRSDK.Toolkit
{
    [RequireComponent(typeof(Animator))]
    public class JMRBaseThemeAnimator : JMRUIInteractable, IFocusable, ISelectClickHandler
    {
        public bool showAppearAnimation = true;
        protected Animator jmrThemeAnimator;
        private JMRConstantThemeSystem.SelectableState jmrCurrentSelectableState = JMRConstantThemeSystem.SelectableState.None;
        private bool isInteractable, isGlobal, isInitialized = false, isClicked = false, isGlobalAdded = false;
        private float timer = 0.2f, checkISelectDelay = 0.2f;

        public virtual void Awake()
        {
            isInteractable = interactable;
            jmrThemeAnimator = GetComponent<Animator>();
        }

        protected virtual void OnEnable()
        {
            isGlobal = !global;
            isFocused = false;

            if (interactable)
            {
                if (!isInitialized)
                {
                    StartCoroutine(SetEndAnimatorEvents());
                }
                else
                {
                    if (showAppearAnimation)
                    {
                        ChangeSelectableState(JMRConstantThemeSystem.SelectableState.Appear);
                    }
                    else
                    {
                        LoadState();
                    }
                }
            }
            else
            {
                ChangeSelectableState(JMRConstantThemeSystem.SelectableState.Disabled);
            }
        }

        protected virtual void Update()
        {
            timer += Time.deltaTime;
            if (timer >= checkISelectDelay)
            {
                if (isInteractable != interactable)
                {
                    isInteractable = interactable;
                    OnInteractableChange(interactable);
                    if (interactable)
                    {
                        LoadState();
                    }
                    else
                    {
                        ChangeSelectableState(JMRConstantThemeSystem.SelectableState.Disabled);
                    }
                }

                if (isGlobal != global)
                {
                    isGlobal = global;
                    if (global)
                    {
                        isGlobalAdded = true;
                        JMRInputManager.Instance.AddGlobalListener(gameObject);
                    }
                    else if (isGlobalAdded)
                    {
                        isGlobalAdded = false;
                        JMRInputManager.Instance.RemoveGlobalListener(gameObject);
                    }
                }

                timer = 0;
            }
        }

        IEnumerator SetEndAnimatorEvents()
        {
            if (jmrThemeAnimator.runtimeAnimatorController == null)
                yield break;

            AnimationClip[] clips = jmrThemeAnimator.runtimeAnimatorController.animationClips;
            int count = (clips.Length + 1) / 2;
            int cntrl = 0;
            foreach (AnimationClip clip in clips)
            {
                cntrl += 1;
                AnimationEvent evt = null;
                bool isAnimationEventAdded = false;
                foreach (AnimationEvent animEvent in clip.events)
                {
                    if (animEvent.functionName == "OnAnimationEnd")
                    {
                        isAnimationEventAdded = true;
                        break;
                    }
                }
                if (!isAnimationEventAdded)
                {
                    evt = new AnimationEvent();
                    evt.time = clip.length;
                    evt.functionName = "OnAnimationEnd";
                    clip.AddEvent(evt);
                }
                if (cntrl == count)
                {
                    count = 0;
                    yield return new WaitForEndOfFrame();
                }
            }
            yield return new WaitForEndOfFrame();

            isInitialized = true;
            if (showAppearAnimation)
            {
                ChangeSelectableState(JMRConstantThemeSystem.SelectableState.Appear);
            }
            else
            {
                LoadState();
            }
        }

        private void LoadState()
        {
            if (isClicked)
            {
                if (isSelected)
                {
                    ChangeSelectableState(JMRConstantThemeSystem.SelectableState.OnselectionHoverClick);
                }
                else
                {
                    ChangeSelectableState(JMRConstantThemeSystem.SelectableState.Pressed);
                }
                isClicked = false;
            }
            else if (isFocused)
            {
                if (isSelected && jmrCurrentSelectableState != JMRConstantThemeSystem.SelectableState.OnSelectionHover)
                {
                    ChangeSelectableState(JMRConstantThemeSystem.SelectableState.OnSelectionHover);
                }
                else if (!isSelected && jmrCurrentSelectableState != JMRConstantThemeSystem.SelectableState.Hover)
                {
                    ChangeSelectableState(JMRConstantThemeSystem.SelectableState.Hover);
                }
            }
            else if (!isFocused)
            {
                if (isSelected && jmrCurrentSelectableState != JMRConstantThemeSystem.SelectableState.OnSelection)
                {
                    ChangeSelectableState(JMRConstantThemeSystem.SelectableState.OnSelection);
                }
                else if (!isSelected && jmrCurrentSelectableState != JMRConstantThemeSystem.SelectableState.Default)
                {
                    ChangeSelectableState(JMRConstantThemeSystem.SelectableState.Default);
                }
            }
        }

        private void OnAnimationEnd()
        {
            if (!interactable)
            {
                return;
            }
            LoadState();
        }

        public virtual void OnFocusEnter()
        {
            isFocused = true;
            if (!interactable)
            {
                return;
            }
            LoadState();
        }

        public virtual void OnFocusExit()
        {
            isFocused = false;
            if (!interactable)
            {
                return;
            }
            LoadState();
        }

        public override void OnSelectClicked(SelectClickEventData eventData)
        {
            if (!interactable)
            {
                return;
            }

            isClicked = true;
            isSelected = !isSelected;
            if (isSelected)
            {
                OnObjectSelect();
            }
            else
            {
                OnObjectDeselect();
            }
            LoadState();
        }

        private void ChangeSelectableState(JMRConstantThemeSystem.SelectableState state)
        {
            if (jmrCurrentSelectableState == JMRConstantThemeSystem.SelectableState.Hover && !isFocused)
            {
                //Debug.LogError("Rebinding");
                //jmrThemeAnimator.Rebind();
            }
            switch (state)
            {
                case JMRConstantThemeSystem.SelectableState.Appear:
                    ChangeToAppear();
                    break;
                case JMRConstantThemeSystem.SelectableState.Disappear:
                    ChangeToDisappear();
                    break;
                case JMRConstantThemeSystem.SelectableState.Default:
                    ChangeToDefault();
                    break;
                case JMRConstantThemeSystem.SelectableState.Hover:
                    ChangeToHover();
                    break;
                case JMRConstantThemeSystem.SelectableState.Disabled:
                    ChangeToDisabled();
                    break;
                case JMRConstantThemeSystem.SelectableState.Pressed:
                    ChangeToPressed();
                    break;
                case JMRConstantThemeSystem.SelectableState.OnSelection:
                    ChangeToOnSelection();
                    break;
                case JMRConstantThemeSystem.SelectableState.OnSelectionHover:
                    ChangeToOnSelectionHover();
                    break;
                case JMRConstantThemeSystem.SelectableState.OnselectionHoverClick:
                    ChangeToOnselectionHoverClick();
                    break;
            }
            jmrCurrentSelectableState = state;
        }

        protected virtual void ChangeToAppear()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.APPEAR);
        }

        protected virtual void ChangeToDisappear()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.DISAPPEAR);
        }

        protected virtual void ChangeToDefault()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.DEFAULT);
        }

        protected virtual void ChangeToHover()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.HOVER);
            //Trigger Haptics
            //JMRInteractionManager.Instance.TriggerHaptics(JMRInteractionManager.Instance.HAPTICS_HOVER, JMRInteractionManager.Instance.HAPTICS_INTENSITY_MEDIUM, 0);
        }

        protected virtual void ChangeToDisabled()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.DISABLED);
        }

        protected virtual void ChangeToPressed()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.PRESSED);
        }

        protected virtual void ChangeToOnSelection()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.ONSELECTION);
        }

        protected virtual void ChangeToOnSelectionHover()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.ONHOVERSELECTION);
        }

        protected virtual void ChangeToOnselectionHoverClick()
        {
            jmrThemeAnimator.SetTrigger(JMRConstantThemeSystem.JMRButtonStates.ONHOVERSELECTIONCLICK);
        }

        //Dont remove the method is used in child
        public virtual void OnInteractableChange(bool isInteractable)
        {

        }

        //Dont remove the method is used in child
        protected virtual void OnDisable()
        {
            if (isGlobalAdded)
            {
                JMRInputManager.Instance.RemoveGlobalListener(gameObject);
            }
        }

        //Dont remove the method is used in child
        protected virtual void OnObjectSelect()
        {
        }

        //Dont remove the method is used in child
        protected virtual void OnObjectDeselect()
        {
        }
    }
}
