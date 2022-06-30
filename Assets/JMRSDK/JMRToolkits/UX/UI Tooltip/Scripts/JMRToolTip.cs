// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using UnityEngine.Animations;
using JMRSDK;

namespace JMRSDK.Toolkit.UI
{
    public enum Direction
    {
        Top,
        Bottom
    }

    public class JMRToolTip : MonoBehaviour
    {
        #region Serialized Fields and Properties

        [SerializeField] private GameObject target = null;
        public GameObject Target { get { return target; } set { target = value; } }        

        [SerializeField] private RectTransform contentParent = null;
        public RectTransform ContentParent { get { return contentParent; } set { contentParent = value; } }

        [SerializeField] private float offset;

        private RectTransform j_RectTransform;

        public Direction ToolTipDirection;

        [SerializeField] private bool isLookAtCamera = true;

        #endregion


        #region Mono

        private void Start()
        {
            if(target == null)
            {
                JMRLogHandler.LogError("JioGlass : target not set");
                return;
            }

            j_RectTransform = GetComponent<RectTransform>();
            if(j_RectTransform == null)
            {
                throw new MissingComponentException("RectTransform Missing");
            }

            MapTooltipToGameobject(j_RectTransform);
        }

        private void LateUpdate()
        {
            if (isLookAtCamera)
            {
                Vector3 upVector = new Vector3(JMRCameraUtility.Main.transform.rotation.x, JMRCameraUtility.Main.transform.rotation.y, 0);
                transform.LookAt(transform.position + JMRCameraUtility.Main.transform.rotation * Vector3.forward, Quaternion.Euler(upVector) * Vector3.up);
            }
        }
        #endregion

        #region Developer facing API
        /// <summary>
        /// Initialize tooltip with target.
        /// </summary>
        /// <param name="target"> Target GameObject.</param>
        public void MapTooltipToGameobject(RectTransform target)
        {
            if ((JMRCameraUtility.Main == null)) return;
            Transform camTransform = JMRCameraUtility.Main.transform;

            target.anchoredPosition = GetOffsetPosition(this.target.transform);
            ConstraintSource cs = new ConstraintSource() { sourceTransform = camTransform, weight = 1 };
            contentParent.GetComponent<LookAtConstraint>().AddSource(cs);
            contentParent.GetComponent<LookAtConstraint>().constraintActive = true;
        }

        public void ShowTooltip()
        {
            gameObject.SetActive(true);
        }

        public void HideTooltip()
        {
            gameObject.SetActive(false);
        }

        #endregion

        #region Private Methods

        private Vector3 GetPointClosestToTarget(Transform target,  RectTransform contentRect)
        {
            Vector3 currentPoint;
            Vector3 nearPoint = Vector3.zero;
            Vector3 targetPos = target.position;
            float nearDist = Mathf.Infinity;
            Vector3[] corners = new Vector3[4];

            contentRect.GetWorldCorners(corners);

            for(int i = 0; i < corners.Length; i++)
            {
                currentPoint = corners[i];
                float sqrDist = (targetPos - currentPoint).sqrMagnitude;
                if(sqrDist < nearDist)
                {
                    nearDist = sqrDist;
                    nearPoint = currentPoint;
                }
            }

            return nearPoint;
        }

        private Vector2 GetOffsetPosition(Transform target)
        {
            float width = 0;
            float height = 0;
            RectTransform rt = target.GetComponent<RectTransform>();
            if (rt) { width = rt.rect.width; height = rt.rect.height; }
            else { Debug.Log("Bounding box missing"); return Vector3.zero; }

            return (rt.anchoredPosition - new Vector2(0, height + offset));
        }

        public void UpdateAnchorsWithDirection(Direction direction)
        {
            
        }
        #endregion
    }
}