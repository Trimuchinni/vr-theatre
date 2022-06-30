// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Helper class for setting up canvases for use.
/// </summary>
namespace JMRSDK.Toolkit.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    [ExecuteInEditMode]
    public class JMRCanvasUtility : MonoBehaviour
    {
        // On demand update if update is required by some component in childern
        // Mostly getting used by image view
        public Action UIUpdate;
        private Canvas j_Canvas;

        #region Mono

        private void Awake()
        {
            j_Canvas = GetComponent<Canvas>();
        }

        private void OnEnable()
        {
            if (Application.isPlaying)
                StartCoroutine(SetRaycastCameraToCanvas());
        }

        private void Start()
        {
            Canvas j_canvas = GetComponent<Canvas>();
            Debug.Assert(j_canvas != null);

            if (j_canvas.worldCamera == null)
            {
                if (EventSystem.current == null)
                {
                    // JioLogHandler.LogError("No EventSystem detected. UI events will not be propagated to Unity UI.");
                }
            }

        }

        private void Update()
        {
            UIUpdate?.Invoke();
        }

        #endregion

        /// <summary>
        /// Set the UI raycast camera as event camera in canvas
        /// </summary>
        IEnumerator SetRaycastCameraToCanvas()
        {
            JMRUIRayCastCamera jmrRaycastCamera = null;
            do
            {
                yield return new WaitForEndOfFrame();
                jmrRaycastCamera = FindObjectOfType<JMRUIRayCastCamera>();
            } while (!jmrRaycastCamera);
            j_Canvas.worldCamera = jmrRaycastCamera.GetRayCastCamera();
        }
    }
}
