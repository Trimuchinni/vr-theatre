using JMRSDK;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JMRSDK.Dialogue;

public class JMRDemoCameraExample : MonoBehaviour
{
    public Text LogText;

    public RectTransform PreviewImageRect;
    public RectTransform PreviewRawImageRect;

    // private JMRCameraAPI cameraAPI;
    private string mediacapturepath;

    private string cameraStatus = "Unavailable";

    // public Material renderMat;
    // public Material renderMat2;
    private List<FrameSize> previewResolutionsList = new List<FrameSize>();
    private List<FrameSize> captureResolutionsList = new List<FrameSize>();
    private FrameSize CurrentRes;
    private Texture2D camTexture;
    private Texture2D camTexture2;

    //private bool previewStarted;

    public Button startPreviewButton, stopPreviewButton;
    public Button captureImageButton, captureImageNameButton;
    public Button startRecordButton, startRecordNameButton;
    public Button pauseRecordButton, resumeRecordButton, stopRecordButton;
    public GameObject resolutionControlParent;
    public Dropdown PrevResDropdown;
    public Dropdown CaptureResDropdown;
    
    //Todo: Add function to get state of camera
    private void Start()
    {
        startPreviewButton.gameObject.SetActive(false);
        if (Application.isEditor)
            startPreviewButton.gameObject.SetActive(true);
        pauseRecordButton.gameObject.SetActive(false);
        resumeRecordButton.gameObject.SetActive(false);
        stopPreviewButton.gameObject.SetActive(false);
        captureImageButton.gameObject.SetActive(false);
        captureImageNameButton.gameObject.SetActive(false);
        startRecordButton.gameObject.SetActive(false);
        startRecordNameButton.gameObject.SetActive(false);
        stopRecordButton.gameObject.SetActive(false);
        //resolutionControlParent.SetActive(false);
        InvokeRepeating("DisplayParameters", 1f, 1f);
        Invoke("GetCameraStatus", 1f);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            //previewStarted = false;
            stopPreviewButton.gameObject.SetActive(false);
            captureImageButton.gameObject.SetActive(false);
            captureImageNameButton.gameObject.SetActive(false);
            startRecordButton.gameObject.SetActive(false);
            startRecordNameButton.gameObject.SetActive(false);
            startPreviewButton.gameObject.SetActive(false);
            stopPreviewButton.gameObject.SetActive(false);
        }
        else
        {
            Invoke("GetCameraStatus", 1f);
        }
    }

    private void OnEnable()
    {
        JMRCameraManager.OnCameraConnect += OnCameraConnect;
        JMRCameraManager.OnCameraDisconnect += OnCameraDisconnect;
        JMRCameraManager.OnImageCapture += OnImageCapture;
        JMRCameraManager.OnCameraError += OnError;
        JMRCameraManager.OnVideoRecord += OnVideoRecord;
        JMRCameraManager.OnCameraPreviewStart += OnCameraPreviewStart;
        JMRCameraManager.OnCameraPreviewStop += OnCameraPreviewStop;
        JMRCameraManager.OnRecordingTimeComplete += OnRecordingComplete;

        PrevResDropdown.onValueChanged.AddListener(ResDropDownChange);
        CaptureResDropdown.onValueChanged.AddListener(CaptureResDropDownChange);
    }

    string cameraError = "";
    private void OnError(string obj)
    {
        cameraError = obj;
    }

    private void OnDisable()
    {
        JMRCameraManager.OnCameraConnect -= OnCameraConnect;
        JMRCameraManager.OnCameraDisconnect -= OnCameraDisconnect;
        JMRCameraManager.OnImageCapture -= OnImageCapture;
        JMRCameraManager.OnCameraError -= OnError;
        JMRCameraManager.OnVideoRecord -= OnVideoRecord;
        JMRCameraManager.OnCameraPreviewStart -= OnCameraPreviewStart;
        JMRCameraManager.OnCameraPreviewStop -= OnCameraPreviewStop;
        JMRCameraManager.OnRecordingTimeComplete -= OnRecordingComplete;

        PrevResDropdown.onValueChanged.RemoveListener(ResDropDownChange);
        CaptureResDropdown.onValueChanged.RemoveListener(CaptureResDropDownChange);
    }

    private void GetCameraStatus()
    {
        if (!Application.isEditor)
        {
            //Get Preview Status
            GetCameraState();

            //Get Recording Status
            GetRecordingState();

            //Get Resolution list
            UpdateDrodownLists();

            //Get Orientation of camera
            GetCameraOrientation();
        }
    }

    void GetCameraState()
    {
        Debug.Log("===================Preview Status : " + JMRCameraManager.Instance.IsPreviewing);
        if (JMRCameraManager.Instance.IsPreviewing)
        {
            startPreviewButton.gameObject.SetActive(false);
            stopPreviewButton.gameObject.SetActive(true);
            captureImageButton.gameObject.SetActive(true);
            captureImageNameButton.gameObject.SetActive(true);
            startRecordButton.gameObject.SetActive(true);
            startRecordNameButton.gameObject.SetActive(true);
        }
        else
        {
            if (!JMRCameraManager.Instance.IsAvailable)
            {
                startPreviewButton.gameObject.SetActive(false);
            }
            else 
            {
                startPreviewButton.gameObject.SetActive(true);
            }
            stopPreviewButton.gameObject.SetActive(false);
            captureImageButton.gameObject.SetActive(false);
            captureImageNameButton.gameObject.SetActive(false);
            startRecordButton.gameObject.SetActive(false);
            startRecordNameButton.gameObject.SetActive(false);
        }
    }

    void GetRecordingState()
    {
        Debug.Log("===================Recording Status : " + JMRCameraManager.Instance.IsRecording);
        if (JMRCameraManager.Instance.IsRecording)
        {
            //Recording state
            if (JMRCameraManager.Instance.GetRecordingState() == JMRCameraManager.VideoRecordState.Paused)
            {
                //Paused
                pauseRecordButton.gameObject.SetActive(false);
                resumeRecordButton.gameObject.SetActive(true);
                stopRecordButton.gameObject.SetActive(true);
            }
            else if (JMRCameraManager.Instance.GetRecordingState() == JMRCameraManager.VideoRecordState.Started)
            {
                //Resumed
                pauseRecordButton.gameObject.SetActive(true);
                resumeRecordButton.gameObject.SetActive(false);
                stopRecordButton.gameObject.SetActive(true);
            }
            else if (JMRCameraManager.Instance.GetRecordingState() == JMRCameraManager.VideoRecordState.Resumed)
            {
                //Resumed
                pauseRecordButton.gameObject.SetActive(true);
                resumeRecordButton.gameObject.SetActive(false);
                stopRecordButton.gameObject.SetActive(true);
            }
            PrevResDropdown.interactable = false;
            CaptureResDropdown.interactable = false;
        }
        else
        {
            //Not recording state
            Debug.Log("===============Not Recording");
            pauseRecordButton.gameObject.SetActive(false);
            resumeRecordButton.gameObject.SetActive(false);
            stopRecordButton.gameObject.SetActive(false);
            PrevResDropdown.interactable = true;
            CaptureResDropdown.interactable = true;
        }
    }

    void GetCameraOrientation()
    {
        if (!Application.isEditor)
        {
            Debug.Log("=======Camera Orientation" + JMRCameraManager.Instance.GetCameraOrientation());
            PreviewImageRect.transform.rotation = Quaternion.Euler(0, 0, JMRCameraManager.Instance.GetCameraOrientation());
        }
    }

    string videoRecordStatus = "";
    private void OnVideoRecord(String obj, JMRCameraManager.VideoRecordState state)
    {
        Debug.Log($"OnVideoRecord({state}, {obj}");
        switch (state)
        {
            case JMRCameraManager.VideoRecordState.Started:
                videoRecordStatus = "Record : VIDEO_RECORD_STATE_STARTED";
                pauseRecordButton.gameObject.SetActive(true);
                resumeRecordButton.gameObject.SetActive(false);
                stopRecordButton.gameObject.SetActive(true); 
                PrevResDropdown.interactable = false;
                CaptureResDropdown.interactable = false;
                break;
            case JMRCameraManager.VideoRecordState.Paused:
                videoRecordStatus = "Record : VIDEO_RECORD_STATE_PAUSED";
                pauseRecordButton.gameObject.SetActive(false);
                resumeRecordButton.gameObject.SetActive(true);
                stopRecordButton.gameObject.SetActive(true);
                break;
            case JMRCameraManager.VideoRecordState.Resumed:
                videoRecordStatus = "Record : VIDEO_RECORD_STATE_RESUMED";
                pauseRecordButton.gameObject.SetActive(true);
                resumeRecordButton.gameObject.SetActive(false);
                stopRecordButton.gameObject.SetActive(true);
                break;
            case JMRCameraManager.VideoRecordState.Stopped:
                videoRecordStatus = "Record : VIDEO_RECORD_STATE_STOPPED";
                break;
            case JMRCameraManager.VideoRecordState.Completed:
                videoRecordStatus = "Record : VIDEO_RECORD_STATE_COMPLETED";
                pauseRecordButton.gameObject.SetActive(false);
                resumeRecordButton.gameObject.SetActive(false);
                stopRecordButton.gameObject.SetActive(false);
                PrevResDropdown.interactable = true;
                CaptureResDropdown.interactable = true;
                break;
            default:
                videoRecordStatus = "Record : UNKNOWN STATE";
                break;
        }

        mediacapturepath = obj;
    }

    public void OnCameraPreviewStart()
    {
        Debug.Log("Preview Start");
        startPreviewButton.gameObject.SetActive(false);
        stopPreviewButton.gameObject.SetActive(true);
        captureImageButton.gameObject.SetActive(true);
        captureImageNameButton.gameObject.SetActive(true);
        startRecordButton.gameObject.SetActive(true);
        startRecordNameButton.gameObject.SetActive(true);
    }

    public void OnCameraPreviewStop()
    {
        Debug.Log("Preview Stop");
        startPreviewButton.gameObject.SetActive(true);
        stopPreviewButton.gameObject.SetActive(false);
        captureImageButton.gameObject.SetActive(false);
        captureImageNameButton.gameObject.SetActive(false);
        startRecordButton.gameObject.SetActive(false);
        startRecordNameButton.gameObject.SetActive(false);
    }

    private void OnRecordingComplete()
    {
        JMRSDKDialogueManager.Instance.Show("RECORDING STOPPED!", "Recording time is over!", "OK", HideDialogueBox);
    }

    private void HideDialogueBox()
    {
        JMRSDKDialogueManager.Instance.Hide();
    }

    public void ResDropDownChange(int resolutionindex)
    {
        foreach (var val in previewResolutionsList)
        {
            if (val.frameSizeText == PrevResDropdown.options[resolutionindex].text)
            {

                if (!Application.isEditor)
                    JMRCameraManager.Instance.SetPreviewResolution(val);
            }
        }
    }

    public void CaptureResDropDownChange(int resolutionindex)
    {
        foreach (var val in captureResolutionsList)
        {
            if (val.frameSizeText == CaptureResDropdown.options[resolutionindex].text)
            {
                if (!Application.isEditor)
                    JMRCameraManager.Instance.SetCaptureResolution(val);
            }
        }
    }

    private void OnImageCapture(string obj)
    {
        mediacapturepath = obj;
    }

    private void OnCameraDisconnect()
    {
        cameraStatus = "Disconnect";
        isCamAvailable = false;
    }

    private bool camConnect;
    private bool isCamAvailable = false;

    private void OnCameraConnect()
    {
        try
        {
            Debug.LogError("++++++++++++++++++++++++Camera Connected");
            cameraStatus = "Connect";

            CurrentRes = JMRCameraManager.Instance.GetCurrentCaptureResolution();
            camConnect = true;
            pauseRecordButton.gameObject.SetActive(false);
            resumeRecordButton.gameObject.SetActive(false);
            isCamAvailable = true;

            UpdateDrodownLists();

            Invoke("GetCameraStatus", 1f);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void UpdateDrodownLists()
    {
        PrevResDropdown.ClearOptions();
        previewResolutionsList = JMRCameraManager.Instance.GetPreviewResolutions();

        if (previewResolutionsList != null && previewResolutionsList.Count > 0)
        {
            //Debug.LogError("+++++++++++++++++++++++Preview Resolution count: " + previewResolutionsList.Count);

            FrameSize cr = JMRCameraManager.Instance.GetCurrentPreviewResolution();

            for (int i = 0; i < previewResolutionsList.Count; i++)
            {
                //Debug.LogError("+++++++++++++++++++++++Preview Resolution Data " + i + " " + previewResolutionsList[i].frameSizeText + "=" + previewResolutionsList[i].Width + "x" + previewResolutionsList[i].Height + "@" + previewResolutionsList[i].frameSizeText);
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = previewResolutionsList[i].frameSizeText;
                PrevResDropdown.options.Add(option);

                if (cr.Width == previewResolutionsList[i].Width && cr.Height == previewResolutionsList[i].Height)
                {
                    PrevResDropdown.value = i;
                    PrevResDropdown.captionText.text = option.text;
                    PreviewImageRect.sizeDelta = new Vector2(cr.Width, cr.Height);
                    PreviewRawImageRect.sizeDelta = new Vector2(cr.Width, cr.Height);
                }
            }
        }
        else
        {
            Debug.LogError("Preview Resolution List NULL");
            Dropdown.OptionData option = new Dropdown.OptionData();
        }

        CaptureResDropdown.ClearOptions();
        captureResolutionsList = JMRCameraManager.Instance.GetCaptureResolutions();

        if (captureResolutionsList != null)
        {
            //Debug.LogError("+++++++++++++++++++++++Capture Resolution count: " + captureResolutionsList.Count);

            FrameSize cr = JMRCameraManager.Instance.GetCurrentCaptureResolution();

            for (int i = 0; i < captureResolutionsList.Count; i++)
            {
                //Debug.LogError("+++++++++++++++++++++++Capture Resolution Data " + i + " " + captureResolutionsList[i].frameSizeText + "=" + captureResolutionsList[i].Width + "x" + captureResolutionsList[i].Height + "@" + captureResolutionsList[i].frameSizeText);
                Dropdown.OptionData option = new Dropdown.OptionData();
                option.text = captureResolutionsList[i].frameSizeText;
                CaptureResDropdown.options.Add(option);

                if (cr.Width == captureResolutionsList[i].Width && cr.Height == captureResolutionsList[i].Height)
                {
                    CaptureResDropdown.value = i;
                    CaptureResDropdown.captionText.text = option.text;
                }
            }
        }
        else
        {
            Debug.LogError("Capture Resolution List NULL");
        }
    }

    public void CaptureImage()
    {
        if (!Application.isEditor)
            JMRCameraManager.Instance.CaptureImage();
    }

    public void CaptureImage(string name)
    {
        if (!Application.isEditor)
            JMRCameraManager.Instance.CaptureImage(Application.persistentDataPath + "/" + name);
    }

    public void StartPreview()
    {
        if (!Application.isEditor)
        {
            if(JMRCameraManager.Instance.BindCameraTexture(PreviewImageRect.transform.GetComponent<Image>()))
                JMRCameraManager.Instance.StartPreview();
        }
        else
        {
            startPreviewButton.gameObject.SetActive(false);
            stopPreviewButton.gameObject.SetActive(true);
            captureImageButton.gameObject.SetActive(true);
            captureImageNameButton.gameObject.SetActive(true);
            startRecordButton.gameObject.SetActive(true);
            startRecordNameButton.gameObject.SetActive(true);
        }
    }

    public void StopPreview()
    {
        if (!Application.isEditor)
        {
            JMRCameraManager.Instance.StopPreview();
        }
        else
        {
            startPreviewButton.gameObject.SetActive(true);
            stopPreviewButton.gameObject.SetActive(false);
            captureImageButton.gameObject.SetActive(false);
            captureImageNameButton.gameObject.SetActive(false);
            startRecordButton.gameObject.SetActive(false);
            startRecordNameButton.gameObject.SetActive(false);
        }
    }

    public void StartRecording()
    {
        if (!Application.isEditor)
        {
            if (JMRCameraManager.Instance.StartRecording())
            {
                //pauseRecordButton.gameObject.SetActive(true);
                //resumeRecordButton.gameObject.SetActive(false);
                //stopRecordButton.gameObject.SetActive(true);
                //PrevResDropdown.interactable = false;
                //CaptureResDropdown.interactable = false;
            }
        }
        else
        {
            pauseRecordButton.gameObject.SetActive(true);
            resumeRecordButton.gameObject.SetActive(false);
            stopRecordButton.gameObject.SetActive(true);
            PrevResDropdown.interactable = false;
            CaptureResDropdown.interactable = false;
        }
    }

    public void StartRecording(string name)
    {
        if (JMRCameraManager.Instance.StartRecording(Application.persistentDataPath + "/" + name))
        {
            pauseRecordButton.gameObject.SetActive(true);
            resumeRecordButton.gameObject.SetActive(false);
            stopRecordButton.gameObject.SetActive(true);
        }
    }

    public void StopRecording()
    {
        if (!Application.isEditor)
        {
            JMRCameraManager.Instance.StopRecording();
        }
        else
        {
            pauseRecordButton.gameObject.SetActive(false);
            resumeRecordButton.gameObject.SetActive(false);
            stopRecordButton.gameObject.SetActive(false);
            PrevResDropdown.interactable = true;
            CaptureResDropdown.interactable = true;
        }
    }

    public void PauseRecording()
    {
        if (!Application.isEditor)
        {
            if (JMRCameraManager.Instance.PauseRecording())
            {
                Debug.Log("Recording Paused");
                pauseRecordButton.gameObject.SetActive(false);
                resumeRecordButton.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("Recording is not Paused");
            }
        }
        else
        {
            pauseRecordButton.gameObject.SetActive(false);
            resumeRecordButton.gameObject.SetActive(true);
        }
    }

    public void ResumeRecording()
    {
        if (!Application.isEditor)
        {
            if (JMRCameraManager.Instance.ResumeRecording())
            {
                Debug.Log("Recording Resumed");
                pauseRecordButton.gameObject.SetActive(true);
                resumeRecordButton.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Recording not Resumed");
            }
        }
        else
        {
            pauseRecordButton.gameObject.SetActive(true);
            resumeRecordButton.gameObject.SetActive(false);
        }
    }

    public void SetPreviewRes(int res)
    {
        if (previewResolutionsList != null && res < previewResolutionsList.Count)
        {
            JMRCameraManager.Instance.SetPreviewResolution(previewResolutionsList[res]);
        }
    }

    public void SetCaptureRes(int res)
    {
        if (captureResolutionsList != null && res < captureResolutionsList.Count)
        {
            JMRCameraManager.Instance.SetCaptureResolution(captureResolutionsList[res]);
        }
    }

    private double latency = -1, maxLatency = -1, prevLatency = -1;

    private float latencyDisplayDelay = 0.5f;

    private void CalculateMaxLatency(float latency)
    {
        this.latency = latency;
        maxLatency = this.latency > maxLatency ? this.latency : maxLatency;
    }

    private void LateUpdate()
    {
        if (!Application.isEditor)
        {
            //if (JMRCameraManager.Instance.IsAvailable)
            //{
            if (previewResolutionsList == null || captureResolutionsList == null)
            {
                UpdateDrodownLists();
            }
            //}
        }
        else
        {
            LogText.text = "Camera not available";
        }
    }

    public void DisplayParameters()
    {
        if (!Application.isEditor)
        {
            if (JMRCameraManager.Instance.IsAvailable)
            {
                CalculateMaxLatency(JMRCameraManager.Instance.GetPreviewLatency());

                LogText.text = "Camera API \n"
                                    + "isCamera Available:\t" + (JMRCameraManager.Instance.IsAvailable ? "Connect" : "Disconnect") + "\n"

                                    + "isRecording:\t" + JMRCameraManager.Instance.IsRecording + "\n"

                                    + "RecordingState:\t" + JMRCameraManager.Instance.GetRecordingState() + "\n"

                                    + "Video Record Status:\t" + videoRecordStatus + "\n"

                                    + "Preview Latency:\t" + (JMRCameraManager.Instance.IsPreviewing ? (int)latency + " ms" : "Preview Not Started") + "\n"

                                    + "Preview Max Latency:\t" + (JMRCameraManager.Instance.IsPreviewing ? (int)maxLatency + " ms" : "Preview Not Started") + "\n"

                                    + "Prev Res:\t" + JMRCameraManager.Instance.GetCurrentPreviewResolution().frameSizeText + "\n"

                                    + "Capture Res:\t" + JMRCameraManager.Instance.GetCurrentCaptureResolution().frameSizeText + "\n"

                                    + "Error Text:\t" + cameraError + "\n";
            }
            else
            {
                LogText.text = "Camera not available : " + cameraError;
            }
        }
        else
        {
            LogText.text = "Camera not available";
        }
    }
}
