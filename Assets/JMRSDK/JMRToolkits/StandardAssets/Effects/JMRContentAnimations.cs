using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using JMRSDK.InputModule;

namespace JMRSDK.Toolkit
{
    public class JMRContentAnimations : MonoBehaviour, IFocusable
    {
        #region Serialize Field
        [SerializeField]
        private RectTransform contentRect, hScroll; // Content Rect if any scale and move required
        [SerializeField]
        private GameObject contentContainer, hTint;
        [SerializeField]
        private float hScroll_ScaleValue, scaleTime, content_ZPosition, content_MoveTime, content_ScaleValue;
        #endregion

        private Sequence j_S;
        private ScrollRect j_CurrentScroll;                                                                                       // Start is called before the first frame update
        private bool isFocused = false;
        private bool j_CancelFoucExit;
        private JMRContentAnimations j_ParentAnimation;
        private int cacheIndex = -1;


        private void OnEnable()
        {
            if (j_CurrentScroll)
            {
                if(cacheIndex!=-1)
                transform.SetSiblingIndex(cacheIndex);
            }

            if (j_ParentAnimation)
            {
                j_ParentAnimation.OnFocusExit();
            }
            j_CancelFoucExit = false;
            isFocused = false;
        }

        void Start()
        {
            j_S = DOTween.Sequence();
            if (!gameObject.GetComponent<ScrollRect>())
                j_CurrentScroll = GetScrollParent(transform);
            if (j_CurrentScroll)
                j_ParentAnimation = j_CurrentScroll.GetComponent<JMRContentAnimations>();
        }

        // Update is called once per frame
        void Update()
        {

        }
        ScrollRect GetScrollParent(Transform t)
        {
            if (t.parent != null)
            {
                ScrollRect scroll = t.parent.GetComponent<ScrollRect>();
                if (scroll != null) { return scroll; }
                else return GetScrollParent(t.parent);
            }
            return null;
        }

        public void OnContentHighlighted()
        {
            j_S.Append(contentRect.DOAnchorPos3DZ(content_ZPosition, content_MoveTime, false));
            j_S.Append(contentContainer.transform.DOScale(content_ScaleValue, scaleTime));
            if (hTint)
                hTint.SetActive(false);
            if (hScroll)
            {
                //j_S.Append(hScroll.transform.DOScale(hScroll_ScaleValue, scaleTime));
                j_S.Append(hScroll.transform.DOLocalMoveZ(-100, scaleTime));
            }
        }
        public void OnContentRelease()
        {

            j_S.Append(contentRect.DOAnchorPos3DZ(0, 0.5f, false));
            j_S.Append(contentContainer.transform.DOScale(1, 0.5f));
            if (hTint)
                hTint.SetActive(true);
            if (hScroll)
            {
                //j_S.Append(hScroll.transform.DOScale(1, scaleTime));
                j_S.Append(hScroll.transform.DOLocalMoveZ(0, scaleTime));
            }
        }
        public void OnContentClick()
        {


        }

        public void OnFocusEnter()
        {
            if (Time.time < 1)
                return;

            if (transform.parent.childCount > 1)
            {
                cacheIndex = transform.GetSiblingIndex();
                transform.SetAsLastSibling();
            }

            if (j_ParentAnimation)
            {
                j_ParentAnimation.OnFocusEnter();
            }

            j_CancelFoucExit = true;
            if (isFocused)
            {
                return;
            }
            isFocused = true;
            OnContentHighlighted();
        }
        public void OnFocusExit()
        {
            if (Time.time < 1)
                return;

            if (transform.parent.childCount > 1)
            {
                transform.SetSiblingIndex(cacheIndex);
                //transform.SetAsLastSibling();
            }

            if (j_ParentAnimation)
            {
                j_ParentAnimation.OnFocusExit();
            }
            j_CancelFoucExit = false;

            if (this.gameObject.activeInHierarchy)
                StartCoroutine(WaitforEndOfFrameToRelease());
        }

        IEnumerator WaitforEndOfFrameToRelease()
        {
            yield return new WaitForEndOfFrame();
            if (!j_CancelFoucExit)
            {
                isFocused = false;

                OnContentRelease();

            }
        }
    }
}
