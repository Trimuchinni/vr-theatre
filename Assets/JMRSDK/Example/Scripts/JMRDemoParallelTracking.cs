using UnityEngine;
using JMRSDK.InputModule;
using System;
using System.Collections.Generic;
using System.Collections;

namespace JMRSDK
{
    internal class JMRDemoParallelTracking : MonoBehaviour
    {
        public Transform headCubeParent,controllerCubeParent;
        private Quaternion _controllerOrientation = new Quaternion();
        private List<IInputSource> Controllers = new List<IInputSource>();
        private bool isInitialized;

        private void OnEnable()
        {
            isInitialized = false;
            Controllers = new List<IInputSource>();
            JMRTrackerManager.OnHeadPosition += OnHeadPosition;
            JMRTrackerManager.OnHeadRotation += OnHeadRotation;
            StartCoroutine(WaitTilFindController());
        }

        private IEnumerator WaitTilFindController()
        {
            do
            {
                Controllers = JMRInteractionManager.Instance.GetSources();
                yield return new WaitForEndOfFrame();
            } while (Controllers.Count == 0);
            isInitialized = true;
        }

        private void OnDisable()
        {
            Controllers = null;
            JMRTrackerManager.OnHeadPosition -= OnHeadPosition;
            JMRTrackerManager.OnHeadRotation -= OnHeadRotation;
        }

        private void Update()
        {
            if (!isInitialized)
            {
                return;
            }

            if (Controllers[0].TryGetRotation(out _controllerOrientation))
            {
                controllerCubeParent.transform.rotation = _controllerOrientation;
            }
        }

        private void OnHeadPosition(Vector3 obj)
        {
            //headCubeParent.localPosition = obj;
        }

        private void OnHeadRotation(Quaternion obj)
        {
            headCubeParent.localRotation = obj;
        }
    }
}
