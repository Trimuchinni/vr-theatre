// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using UnityEngine.UI;
using JMRSDK.InputModule;

namespace JMRSDK.Toolkit.UI
{
    public class JMRUISlider : MonoBehaviour, InputModule.ISelectHandler, IFocusable
    {
        #region Enums

        /// <summary>
        /// Setting that indicates one of four directions.
        /// </summary>
        public enum Direction
        {
            LeftToRight,
            RightToLeft,
            BottomToTop,
            TopToBottom,
        }

        [System.Serializable]
        public enum SliderAxis
        {
            Horizontal = 0,
            Vertical = 1
        }

        #endregion

        #region SERIALIZED FIELDS 

        [SerializeField]
        private GameObject sliderHandleParent = null;

        [SerializeField]
        private GameObject sliderHandle = null;

        [SerializeField]
        private float minValue = 0;

        [SerializeField]
        private float maxValue = 1;

        [SerializeField]
        private Image sliderFiller;

        [SerializeField]
        private SliderAxis sliderAxis = SliderAxis.Horizontal;

        [SerializeField]
        private float swipeOffset = 0.01f;

        #endregion

        public bool updateSliderUIDirectly = true;

        #region PUBLIC PROPERTIES
        public GameObject SliderHandleParent
        {
            get
            {
                return sliderHandleParent;
            }
            set
            {
                sliderHandleParent = value;
                InitializeSliderHandle();
            }
        }

        public float MinValue { get { return minValue; } set { if (value != minValue) minValue = value; SetSliderValue(j_SliderValue); } }

        public float MaxValue { get { return maxValue; } set { if (value != maxValue) maxValue = value; SetSliderValue(j_SliderValue); } }

        public float SliderValueUI
        {
            get { return j_SliderValue; }
            set
            {
                SetSliderValue(value);
            }
        }

        private float SliderValue;

        public float NormalizedSliderValue
        {
            get
            {
                if (Mathf.Approximately(minValue, maxValue))
                    return 0;
                return Mathf.InverseLerp(minValue, maxValue, j_SliderValue);
            }
        }
        public Image SliderFiller
        {
            get { return sliderFiller; }
        }
        public SliderAxis CurrentSliderAxis
        {
            get { return sliderAxis; }
            set
            {
                sliderAxis = value;
                UpdateVisuals();
            }
        }

        public Vector3 SliderStartPosition
        {
            get { return transform.TransformPoint(Vector3.right * j_SliderStartDistance); } // GetSliderAxis() * sliderStartDistance); }
            set { j_SliderStartDistance = Vector3.Dot(transform.InverseTransformPoint(value), GetSliderAxis()); }
        }

        public Vector3 SliderEndPosition
        {
            get { return transform.TransformPoint(Vector3.right * j_SliderEndDistance); } // GetSliderAxis() * sliderEndDistance); }
            set { j_SliderEndDistance = Vector3.Dot(transform.InverseTransformPoint(value), GetSliderAxis()); }
        }

        public Vector3 SliderTrackDirection
        {
            get { return SliderEndPosition - SliderStartPosition; }
        }

        #endregion

        #region PRIVATE FIELDS & Properties

        private Camera j_RaycastCam;
        private RectTransform j_SliderRect;
        private float j_TotalWidth;
        private float j_SliderValue = 0;
        private float j_SliderStartDistance = -.5f;
        private float j_SliderEndDistance = .5f;
        private bool isDragging = false;

        private SliderAxis? j_PreviousSliderAxis = null;

        private SliderAxis PreviousSliderAxis
        {
            get
            {
                if (j_PreviousSliderAxis == null)
                {
                    j_PreviousSliderAxis = CurrentSliderAxis;
                }
                return j_PreviousSliderAxis.Value;
            }
            set
            {
                j_PreviousSliderAxis = value;
            }
        }

        private Vector3 j_SliderHandleOffset = Vector3.zero;
        private JMRInteractable j_Interactable;
        private JMRCursor j_Cursor;
        #endregion

        #region Event Handlers
        [Header("Events")]
        public UnityEventFloat OnValueUpdated = new UnityEventFloat();

        #endregion

        #region MonoBehaviour Methods

        private void Start()
        {
            j_Interactable = GetComponent<JMRInteractable>();

            if (j_Interactable)
            {
                j_Interactable.SwipeLeft += OnHorizontalSwipe;
                j_Interactable.SwipeRight += OnHorizontalSwipe;
                j_Interactable.SwipeUp += OnVerticalSwipe;
                j_Interactable.SwipeDown += OnVerticalSwipe;
            }
            else
            {
                throw new MissingComponentException("Interactable component missing from the object");
            }

            if (JMRPointerManager.Instance != null)
                j_RaycastCam = JMRPointerManager.Instance.UIRaycastCamera;
            else
            {
                throw new MissingComponentException("Raycast Camera component missing from the Scene");
            }
            j_SliderRect = GetComponent<RectTransform>();
            j_SliderStartDistance = j_SliderRect.rect.xMin;
            j_SliderEndDistance = j_SliderRect.rect.xMax;
            InitializeSliderHandle();
            OnValueUpdated?.Invoke(j_SliderValue);

            j_Cursor = FindObjectOfType(typeof(JMRCursor)) as JMRCursor;
        }

        private void OnDestroy()
        {
            if (j_Interactable)
            {
                j_Interactable.SwipeLeft -= OnHorizontalSwipe;
                j_Interactable.SwipeRight -= OnHorizontalSwipe;
                j_Interactable.SwipeUp -= OnVerticalSwipe;
                j_Interactable.SwipeDown -= OnVerticalSwipe;
            }
        }

        private void OnValidate()
        {
            CurrentSliderAxis = sliderAxis;
        }

        private void Update()
        {
            if (isDragging)
            {
                UpdatePosition();
            }
        }

        #endregion

        #region Private Methods

        private void InitializeSliderHandle()
        {
            var startToHandle = sliderHandleParent.transform.position - SliderStartPosition;
            var handleProjectedOnTrack = SliderStartPosition + Vector3.Project(startToHandle, SliderTrackDirection);
            j_SliderHandleOffset = sliderHandleParent.transform.position - handleProjectedOnTrack;

            UpdateJMRUISliderUI();
        }

        private void UpdateJMRUISliderUI()
        {
            if (sliderFiller)
            {
                if (j_SliderValue > minValue)
                    sliderFiller.fillAmount = (j_SliderValue - minValue) / (maxValue - minValue);
                else
                    sliderFiller.fillAmount = 0;
            }

            var newSliderPos = SliderStartPosition + j_SliderHandleOffset + SliderTrackDirection * sliderFiller.fillAmount;
            sliderHandleParent.transform.position = newSliderPos;
        }

        private void OnHorizontalSwipe(SwipeEventData data, float swipeDelta)
        {
            if (!j_Interactable.IsEnabled)
                return;
            ////Debug.LogError("My value H : " + swipeDelta);
            SliderValue += (maxValue * swipeDelta) * swipeOffset;

            if (updateSliderUIDirectly)
                SliderValueUI = SliderValue;

            OnValueUpdated?.Invoke(SliderValue);
            //SliderValue = Mathf.Clamp01(SliderValue);
        }

        private void OnVerticalSwipe(SwipeEventData data, float swipeDelta)
        {
            if (!j_Interactable.IsEnabled)
                return;
            ////Debug.LogError("My value V : " + swipeDelta);
            SliderValue += (maxValue * swipeDelta) * swipeOffset;
            
            if (updateSliderUIDirectly)
                SliderValueUI = SliderValue;

            OnValueUpdated?.Invoke(SliderValue);
            //SliderValue = Mathf.Clamp01(SliderValue);
        }

        //calculates the normalized value accroding to click position on slider to be set in the fillamount
        void UpdatePosition()
        {
            if (!j_Interactable.IsEnabled)
                return;

            if(j_Cursor == null)
            {
                j_Cursor = FindObjectOfType(typeof(JMRCursor)) as JMRCursor;
            }

            //Debug.Log("J raycast cam : " + j_RaycastCam);
            if (j_RaycastCam == null || !j_Cursor)
                return;
            //Vector3 point = eventData.position;
            //point.z = sliderFiller.transform.position.z;
            //Vector3 clickPoint = j_RaycastCam.ScreenToWorldPoint(point);
            Vector3 clickPoint = j_Cursor.Position;
            Vector3[] worldPoints = new Vector3[4];
            sliderFiller.GetComponent<RectTransform>().GetWorldCorners(worldPoints);
            float normalizedValue = Mathf.InverseLerp(worldPoints[0].x, worldPoints[2].x, clickPoint.x);
            SliderValue = normalizedValue;

            if (updateSliderUIDirectly)
                SliderValueUI = SliderValue;

            OnValueUpdated?.Invoke(SliderValue);
        }

        private void UpdateVisuals()
        {
            if (PreviousSliderAxis != sliderAxis)
            {
                //UpdateSliderHandleOrientation();
                UpdateSliderOrientation();

                PreviousSliderAxis = sliderAxis;
            }
        }

        private void UpdateSliderOrientation()
        {
            switch (sliderAxis)
            {
                case SliderAxis.Horizontal:
                    transform.localRotation = Quaternion.identity;
                    break;
                case SliderAxis.Vertical:
                    transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                    break;
            }
        }

        private void UpdateSliderHandleOrientation()
        {
            if (sliderHandleParent)
            {
                sliderHandleParent.transform.localPosition = Vector3.zero;

                switch (sliderAxis)
                {
                    case SliderAxis.Horizontal:
                        sliderHandleParent.transform.localRotation = Quaternion.identity;
                        break;
                    case SliderAxis.Vertical:
                        sliderHandleParent.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
                        break;
                }
            }
        }

        private Vector3 GetSliderAxis()
        {
            switch (sliderAxis)
            {
                case SliderAxis.Horizontal:
                    return Vector3.right;
                case SliderAxis.Vertical:
                    return Vector3.up;
                default:
                    throw new System.ArgumentOutOfRangeException("Invalid slider axis");
            }
        }

        private void SetSliderValue(float sValue)
        {
            // Clamp the input
            float newValue = ClampValue(sValue);

            // If the stepped value doesn't match the last one, it's time to update
            if (j_SliderValue == newValue)
                return;

            j_SliderValue = newValue;
            UpdateJMRUISliderUI();
        }

        float ClampValue(float input)
        {
            float newValue = Mathf.Clamp(input, minValue, maxValue);
            return newValue;
        }

        public void OnFocusEnter()
        {
        }

        public void OnFocusExit()
        {
            isDragging = false;
        }

        public void OnSelectDown(SelectEventData eventData)
        {
            isDragging = true;
        }

        public void OnSelectUp(SelectEventData eventData)
        {
            isDragging = false;
        }


        #endregion
    }
}