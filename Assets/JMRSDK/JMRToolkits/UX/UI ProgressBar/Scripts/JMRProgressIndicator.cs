// Copyright (c) 2020 JioGlass. All Rights Reserved.
/// <summary>
/// Displays progress bar and (optionally) percentage text.
/// Progress indicator run's asyncronously.
/// </summary>

using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JMRSDK.Toolkit.UI
{
    public class JMRProgressIndicator : MonoBehaviour
    {
        const float SmoothProgressSpeed = 0.25f;

        #region Public Properties

        public Transform MainTransform { get { return transform; } }
        public JMRProgressIndicatorState State { get { return j_State; } }
        public float Progress { get; set; }
        public float TargetProgress { get { return targetProgress; } set { targetProgress = value; } }

        public UnityEvent OnProgressComplete { get => onProgressComplete; set => onProgressComplete = value; }
        #endregion

        #region Serialized properties
        [SerializeField]
        private Animator progressAnim;

        // The animated progress bar object
        [SerializeField]
        private Image progressBar = null;
        [SerializeField]
        private Image progressBarGlow = null;

        // The progress text used by all non-'None' progress styles
        [SerializeField]
        private TMP_Text progressText = null;

        [Tooltip("Format to use when converting progress number to string.")]
        [SerializeField]
        private string progressStringFormat = "{0}";

        //Whether to display the percentage as text in addition to the loading bar.
        private bool displayPercentage = true;

        /// <summary>
        /// Target for smooth progress, 0-1. Serialized to test in editor.
        /// </summary>
        [SerializeField]
        [Range(0f, 1f)]
        private float targetProgress = 0f;

        [SerializeField] private UnityEvent onProgressComplete;

        #endregion

        #region PrivateVariables

        private float j_SmoothProgress = 0f;
        private float j_LastSmoothProgress = -1;
        private JMRProgressIndicatorState j_State = JMRProgressIndicatorState.Closed;
        private float j_ProgressFill = 0;
        private bool j_IsInfinite = false;
        private float j_InfiniteFillTime = 1f;
        private float j_InfiniteTimer = 0f;
        private float j_SmoothProgressDoubleDigit = 0;
        private bool IsPercentageEnabled = false;
        #endregion

        // User facing API's for progress indicator.
        #region Public Methods

        /// <summary>
        /// Show Progress indicator.
        /// </summary>
        /// <param name="IsInfinite">set true if you want infinite progress.</param>
        public void Show(bool IsInfinite)
        {
            j_IsInfinite = IsInfinite;
            ActivateProgressObject();
        }

        /// <summary>
        /// Hide Progress Indicator.
        /// </summary>
        public void Hide()
        {
            if (j_State == JMRProgressIndicatorState.Closed || j_State == JMRProgressIndicatorState.Closing)
            {
                return;
            }

            j_IsInfinite = false;
            targetProgress = 0f;
            ActivateProgressObject();
        }

        /// <summary>
        /// Set progress to progress indicator continuesly.
        /// user defined progress.
        /// </summary>
        /// <param name="progress">Progress in float range is 0 to 1.</param>
        public void SetProgress(float progress)
        {
            if (j_IsInfinite)
                return;

            targetProgress = progress;
        }

        #endregion

        #region Async Methods
        private async void ActivateProgressObject()
        {
            // If the indicator is opening or closing, wait for that to finish before trying to open / close it
            // Otherwise the indicator will display an error and take no action
            await AwaitTransitionAsync();

            switch (State)
            {
                case JMRProgressIndicatorState.Closed:
                    OpenProgressIndicator();
                    break;

                case JMRProgressIndicatorState.Open:
                    await CloseAsync();
                    break;
            }
        }

        private async void OpenProgressIndicator()
        {
            await OpenAsync();

            if (!j_IsInfinite)
            {
                while (targetProgress < 1f)
                {
                    await Task.Yield();
                }

                await CloseAsync();
            }
            else
            {
                while (j_State == JMRProgressIndicatorState.Open)
                {
                    await Task.Yield();
                }
            }
        }


        private async Task OpenAsync()
        {
            if (j_State != JMRProgressIndicatorState.Closed)
            {
                throw new System.Exception("Can't open in state " + j_State);
            }

            j_SmoothProgress = 0;
            j_LastSmoothProgress = 0;

            displayPercentage = !j_IsInfinite;
            IsPercentageEnabled = displayPercentage;

            gameObject.SetActive(true);

            // TO DO : Remove hardcoding of animation
            if (progressAnim != null && !progressAnim.GetCurrentAnimatorStateInfo(0).IsName("Appear"))
            {
                progressAnim.SetTrigger("Appear");
            }

            j_State = JMRProgressIndicatorState.Opening;

            await Task.Yield();

            j_State = JMRProgressIndicatorState.Open;
        }

        private async Task CloseAsync()
        {
            if (j_State != JMRProgressIndicatorState.Open)
            {
                throw new System.Exception("Can't close in state " + j_State);
            }

            j_State = JMRProgressIndicatorState.Closing;

            if (progressAnim != null && !progressAnim.GetCurrentAnimatorStateInfo(0).IsName("Disappear"))
            {
                progressAnim.SetTrigger("Disappear");
            }

            await Task.Delay(500);

            j_State = JMRProgressIndicatorState.Closed;

            gameObject.SetActive(false);
        }

        private async Task AwaitTransitionAsync()
        {
            while (isActiveAndEnabled)
            {
                switch (j_State)
                {
                    case JMRProgressIndicatorState.Open:
                    case JMRProgressIndicatorState.Closed:
                        return;

                    default:
                        break;
                }

                await Task.Yield();
            }
        }

        #endregion

        #region Mono
        private void LateUpdate()
        {
            if (j_State != JMRProgressIndicatorState.Open)
            {
                return;
            }

             progressText.enabled = displayPercentage;

            if (j_IsInfinite)
            {
                if (j_InfiniteTimer < j_InfiniteFillTime)
                {
                    j_InfiniteTimer += Time.deltaTime;
                    progressBar.fillAmount = progressBarGlow.fillAmount = j_InfiniteTimer / j_InfiniteFillTime;
                }
                else
                    j_InfiniteTimer = 0;
            }
            else if(IsPercentageEnabled)
            {

                j_SmoothProgress = Mathf.Lerp(j_SmoothProgress, targetProgress, SmoothProgressSpeed);

                if (j_SmoothProgress > 0.99f)
                {
                    j_SmoothProgress = 1f;
                }

                j_ProgressFill = Mathf.Clamp(j_SmoothProgress, 0.01f, 1f);
                progressBar.fillAmount = progressBarGlow.fillAmount = j_ProgressFill;      

                if (j_LastSmoothProgress != j_SmoothProgress)
                {
                    j_SmoothProgressDoubleDigit = (int)(j_SmoothProgress * 100);
                    progressText.text = string.Format(progressStringFormat, j_SmoothProgressDoubleDigit);
                    j_LastSmoothProgress = j_SmoothProgress;
                }

                Progress = Mathf.Lerp(0, 100, j_SmoothProgress);
                if (Progress >= 100)
                {
                    IsPercentageEnabled = false;
                    onProgressComplete?.Invoke();
                }
            }
        }
        #endregion

    }
}
