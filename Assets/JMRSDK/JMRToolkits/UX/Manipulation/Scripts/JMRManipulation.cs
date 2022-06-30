// Copyright (c) 2020 JioGlass. All Rights Reserved.


using UnityEngine;
using UnityEngine.EventSystems;
using JMRSDK;
using JMRSDK.InputModule;
using System.Collections;

namespace JMRSDK.InputModule
{
    public class JMRManipulation : JMRCursorModifier, IManipulationHandler, ISwipeHandler
    {

        //Can only be grabbable or rotatable at any single time, it can be neither too.
        /* Long press of manipulation button triggers manipulation start
        * Only if it is grabbale and not rotatble, object snaps to the cursor (gaze or controller) and moves with it as long as the manipulation button is held down. The position of the 
        * object being grabbed should be the same as its position just before the manipulation action started.
        * 
        * Only if it is rotatble and not grabbale, object remains in the same place but mimics controller orientation in all 3 axes. The controller laser pointer will have to disabled in this case.
        * If it is scalable, swiping left and right scales the object. Scaling should have maximum and minimum bounds. It should NOT become smaller than the reticle , nor should it become 
        * as big as that it covers the entire FOV.
        * 
        * 
        * If it is movable, swiping front and back moves the object along the ray. Movement should have maximum and minimum bounds. It should NOT become smaller than the reticle when moved away, 
        * nor should it become as big as that it covers the entire FOV when moved close.
        * 
        * 
        * Object will be removed from raycast list to allow interaction with other 3d objects.
        *
        */
      
        #region SerializeField
        [SerializeField]
        private bool isGrabbale = true;
        public bool IsGrabbale { get => isGrabbale; set => isGrabbale = value; }

        [SerializeField]
        private bool isRotatable = true;
        public bool IsRotatable { get => isRotatable; set => isRotatable = value; }

        [SerializeField]
        private bool isScalable = true;

        [SerializeField]
        private bool isMovable = true;
        public bool IsMovable { get => isMovable; set => isMovable = value; }
        #endregion

        #region Variables
        protected IPointingSource j_selectingFocuser;
        protected IInputSource j_inputSource;


        private float j_GrabInitDist = 2f;
        private float j_CursorDefaultDist = 10f;

        bool IsFocusingManipulatableObject = false;
        GameObject j_FocusedObject;

        /// <summary>
        /// Variable to keep track whether object is being manipulated or not.
        /// </summary>
        private bool isBeingManipulated = false;

        /// <summary>
        /// Scale multiplier. HAS TO BE STRICTLY PRIVATE
        /// </summary>
        private float j_scaleIncrementFactor = 0.2f;

        /// <summary>
        /// Move multiplier. HAS TO BE STRICTLY PRIVATE
        /// </summary>
        private float j_moveIncrementFactor = 1.2f;

        /// <summary>
        /// Variables to avoid scaling down the GameObject too small or it away too far that its not longer visible
        /// </summary>
        private float j_screenRatioForSmall = (float)(Screen.height / 8);
        private float j_smallRatioThreshold = 8.3f;
        private float j_largeRatioThreshold = 35f;
        private Camera cam;


        private float j_MinPoseDistance = 2f;
        private float j_MaxPoseDistance = 8f;
        private float j_PoseTrasholdDistance = 0.5f;

        private bool j_lockFocus = false;
        private GameObject j_FocusedRoot;
        private uint j_sourceId;

        private Transform CSRTransform;

        public bool LockFocus
        {
            get
            {
                return j_lockFocus;
            }
            set
            {
                j_lockFocus = value;
                CheckLockFocus(j_selectingFocuser);
            }
        }

        #endregion

        #region MONO

        private void Awake()
        {

            if (HostTransform == null)
            {
                HostTransform = transform;
            }
            JMRPointerManager.Instance.PointerSpecificFocusChanged += OnPointerSpecificFocusChanged;
        }
        void Start()
        {
            if (isGrabbale && isRotatable)
            {
                //Debug.LogError("SOMETHING IS WRONG. BOTH CAN'T BE ACTIVE. NEED TO HANDLE IN EDITOR SCRIPT");
                isRotatable = false;
            }
        }
        void LateUpdate() // Use lateupdate here
        {
            if (isRotatable && isBeingManipulated && j_inputSource != null)
            {
                if (!Application.isEditor)
                {
                    Quaternion ControllerRotation;
                    j_inputSource.TryGetPointerRotation(out ControllerRotation);

                    j_FocusedObject.transform.rotation = ControllerRotation;
                }
            }
        }
        private void OnEnable()
        {
            JMRPointerManager.Instance.PointerSpecificFocusChanged += OnPointerSpecificFocusChanged;
        }
        private void OnDisable()
        {
            // PointerManager.Instance.PointerSpecificFocusChanged -= OnPointerSpecificFocusChanged;
        }
        #endregion

        #region Focuser 

        private void CheckLockFocus(IPointingSource focuser)
        {
            // If our previous selecting focuser isn't the same
            if (j_selectingFocuser != null && j_selectingFocuser != focuser)
            {
                // If our focus is currently locked, unlock it before moving on
                if (LockFocus)
                {
                    j_selectingFocuser.FocusLocked = false;
                }
            }

            // Set to the new focuser
            j_selectingFocuser = focuser;
            if (j_selectingFocuser != null)
            {
                j_selectingFocuser.FocusLocked = LockFocus;
            }
        }

        private void LockFocuser(IPointingSource focuser)
        {
            if (focuser != null)
            {
                ReleaseFocuser();
                j_selectingFocuser = focuser;
                j_selectingFocuser.FocusLocked = true;
            }
        }

        private void ReleaseFocuser()
        {
            if (j_selectingFocuser != null)
            {
                j_selectingFocuser.FocusLocked = false;
                j_selectingFocuser = null;
            }
        }
        #endregion

        #region Events

        private void OnPointerSpecificFocusChanged(IPointingSource pointer, GameObject oldFocusedObject, GameObject newFocusedObject)
        {

            PointerSpecificEventData eventData = new PointerSpecificEventData(EventSystem.current);
            eventData.Initialize(pointer);


            if (newFocusedObject != null)
            {
                if (newFocusedObject.GetComponent<JMRManipulation>())
                {
                    IsFocusingManipulatableObject = true;
                    j_FocusedObject = newFocusedObject;
                }
            }

            if (oldFocusedObject != null)
            {

            }

            CheckLockFocus(pointer);
        }

        void IManipulationHandler.OnManipulationCompleted(ManipulationEventData eventData)
        {
            j_sourceId = eventData.SourceId;
            isBeingManipulated = false;

            if (isRotatable)
            {
                ReleaseFocuser();
            }
            if (isGrabbale)
            {
                //Snap object with cursor
               Vector3 cachedPos =  j_FocusedRoot.transform.position;
                if (j_FocusedRoot != null)
                    j_FocusedRoot.transform.parent = null;
                j_FocusedRoot.transform.position = cachedPos;
                JMRPointerManager.Instance.MaxPointerCollisionDistance = j_CursorDefaultDist;
            }
            ReleaseFocuser();

            //Setting JMRPointer Layer masks back to default
            JMRPointerManager.Instance.RaycastLayerMasks = new LayerMask[] { Physics.DefaultRaycastLayers };

            JMRInputManager.Instance.RemoveGlobalListener(this.gameObject);
            //var foundManiputableObjects = FindObjectsOfType<JMRManipulation>();
            //foreach (var MObj in foundManiputableObjects) //TODO //FIXME
            //{
            //    //Debug.LogError("Name............" + MObj);
            //    MObj.gameObject.SetLayerRecursively(0);
            //}
        }

        void IManipulationHandler.OnManipulationStarted(ManipulationEventData eventData)
        {
            j_sourceId = eventData.SourceId;
            isBeingManipulated = true;
            j_inputSource = eventData.InputSource;
            JMRInputManager.Instance.AddGlobalListener(this.gameObject);
            //var foundManiputableObjects = FindObjectsOfType<JMRManipulation>();
            //foreach (var MObj in foundManiputableObjects) //TODO //FIXME
            //{
            //    MObj.gameObject.SetLayerRecursively(2);
            //}

            //Changing the raycast layer of JMRPointer to avoid any other elements being snapped upon.
            int manipulationLayerMask = LayerMask.GetMask(LayerMask.LayerToName(11));
            JMRPointerManager.Instance.RaycastLayerMasks = new LayerMask[] { manipulationLayerMask };

            if (IsFocusingManipulatableObject)
            {
                if (isRotatable)
                {
                    LockFocuser(j_selectingFocuser);
                }
                if (isGrabbale)
                {
                    if (CSRTransform == null)
                    {
                        var CSR = FindObjectOfType(typeof(JMRSDK.InputModule.JMRCursor)) as JMRSDK.InputModule.JMRCursor;
                        CSRTransform = CSR.transform;
                    }

                    
                    j_GrabInitDist =  Vector3.Distance(CSRTransform.position, JMRPointerManager.Instance.GetPointerSourceTansform().position);

                    j_CursorDefaultDist = JMRPointerManager.Instance.MaxPointerCollisionDistance;
                    JMRPointerManager.Instance.MaxPointerCollisionDistance = j_GrabInitDist;
                    j_FocusedRoot = j_FocusedObject.transform.root.gameObject;
                    StartCoroutine(WaitToAdjustPosition(j_FocusedRoot.transform.position, j_FocusedRoot.transform.rotation, j_FocusedRoot, CSRTransform));
                }
            }


            ////Make child of cursor or update to cursor location on every update, whatever is suitable.
            //Add into ignore raycast list
        }

        IEnumerator WaitToAdjustPosition(Vector3 pos,Quaternion rot,GameObject root_obj,Transform parent_obj)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            root_obj.transform.parent = parent_obj;
            root_obj.transform.position = pos;
            root_obj.transform.rotation = rot;
        }

        #endregion

        #region MovingInZ
        void ISwipeHandler.OnSwipeDown(SwipeEventData data, float eventData)
        {
            if (!isMovable || !isBeingManipulated)
                return;

            var value = Mathf.Abs(eventData);
            value = value * j_moveIncrementFactor;

            if (LargerThanRequired() || BeforeInteractionRange())
            {
                return;
            }

            if (Vector3.Distance(transform.position, JMRPointerManager.Instance.PointerOrigin) >= j_MinPoseDistance + j_PoseTrasholdDistance)
                j_FocusedObject.transform.position += j_selectingFocuser.Rays[0].Direction * -value;
        }

        void ISwipeHandler.OnSwipeUp(SwipeEventData data, float eventData)
        {
            if (!isMovable || !isBeingManipulated)
                return;

            var value = Mathf.Abs(eventData);
            value = value * j_moveIncrementFactor;

            if (SmallerThanRequired() || BeyondInteractionRange())
            {
                return;
            }

            if (Vector3.Distance(transform.position, JMRPointerManager.Instance.PointerOrigin) <= j_MaxPoseDistance - j_PoseTrasholdDistance)
                j_FocusedObject.transform.position += j_selectingFocuser.Rays[0].Direction * value;
        }
        #endregion

        #region Scaling
        void ISwipeHandler.OnSwipeLeft(SwipeEventData data, float eventData)
        {
            if (!isScalable || !isBeingManipulated)
                return;

            // Checking if the object is smaller than required then don't scale it down farther
            if (SmallerThanRequired())
            {
                return;
            }
            var value = Mathf.Abs(eventData);
            value = value * j_scaleIncrementFactor;

            transform.localScale = new Vector3(transform.localScale.x - value, transform.localScale.y - value, transform.localScale.z - value);//transform.localScale * eventData;
        }

        void ISwipeHandler.OnSwipeRight(SwipeEventData data, float eventData)
        {

            if (!isScalable || !isBeingManipulated)
                return;


            if (LargerThanRequired())
            {
                return;
            }

            var value = Mathf.Abs(eventData);
            value = value * j_scaleIncrementFactor;

            transform.localScale = new Vector3(transform.localScale.x + value, transform.localScale.y + value, transform.localScale.z + value);
        }
        #endregion

        #region CheckCondition
        /// <summary>
        /// This functions checks whether the gameObject is  in Interactionrange 
        /// </summary>
        /// <returns><c>true</c>, if goes beyound range, <c>false</c> otherwise.</returns>
        private bool BeyondInteractionRange()
        {
            if (Vector3.Distance(transform.position, JMRPointerManager.Instance.PointerOrigin) >= j_MaxPoseDistance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This functions checks whether the gameObject is  in Interactionrange 
        /// </summary>
        /// <returns><c>true</c>, if comes closer then expected  range, <c>false</c> otherwise.</returns>
        private bool BeforeInteractionRange()
        {
            if (Vector3.Distance(transform.position, JMRPointerManager.Instance.PointerOrigin) <= j_MinPoseDistance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This functions checks whether the gameObject is below a certain visible size
        /// </summary>
        /// <returns><c>true</c>, if smaller than required, <c>false</c> otherwise.</returns>
        private bool SmallerThanRequired()
        {
            Bounds bounds = new Bounds();
            foreach (var r in transform.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(r.bounds);
            }
            if (cam == null)
            {
                cam = Camera.main;//TMRManager.Instance.GetCamera(); //Get the mono camera in the head transform 
            }

            Vector3 a = cam.WorldToScreenPoint(transform.position);
            Vector3 b = new Vector3(a.x, a.y + j_screenRatioForSmall, a.z);
            float c = (cam.ScreenToWorldPoint(a) - cam.ScreenToWorldPoint(b)).magnitude;
            float ratio = bounds.size.magnitude / c;

            if (ratio < j_smallRatioThreshold)
                return true;

            return false;
        }
        /// <summary>
        /// This functions checks whether the gameObject is Over a certain visible size
        /// </summary>
        /// <returns><c>true</c>, if Larger than required, <c>false</c> otherwise.</returns>
        private bool LargerThanRequired()
        {
            Bounds bounds = new Bounds();
            foreach (var r in transform.GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(r.bounds);
            }
            if (cam == null)
            {
                cam = Camera.main;//TMRManager.Instance.GetCamera(); //Get the mono camera in the head transform 
            }

            Vector3 a = cam.WorldToScreenPoint(transform.position);
            Vector3 b = new Vector3(a.x, a.y + j_screenRatioForSmall, a.z);
            float c = (cam.ScreenToWorldPoint(a) - cam.ScreenToWorldPoint(b)).magnitude;
            float ratio = bounds.size.magnitude / c;

            if (ratio > j_largeRatioThreshold)
                return true;

            return false;
        }
        #endregion

        #region NotImplemented

        void IManipulationHandler.OnManipulationUpdated(ManipulationEventData eventData)
        {

        }

        void ISwipeHandler.OnSwipeCanceled(SwipeEventData data)
        {
            //throw new System.NotImplementedException();
        }

        void ISwipeHandler.OnSwipeCompleted(SwipeEventData data)
        {
            // throw new System.NotImplementedException();
        }


        void ISwipeHandler.OnSwipeStarted(SwipeEventData data)
        {
            //throw new System.NotImplementedException();
        }

        void ISwipeHandler.OnSwipeUpdated(SwipeEventData data, Vector2 swipeData)
        {
            // //Debug.LogError("Updating swipe : " + swipeData);
        }
        #endregion
    }
}