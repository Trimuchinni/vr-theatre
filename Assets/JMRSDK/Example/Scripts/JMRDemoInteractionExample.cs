using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JMRSDK.InputModule;
using JMRSDK;
using System;
using System.Collections;

public class JMRDemoInteractionExample : MonoBehaviour, ISwipeHandler
{
    public Transform controllersUI;
    private List<IInputSource> Controllers = new List<IInputSource>();
    private Dictionary<int, int> batteryLevels = new Dictionary<int, int>();
    private Dictionary<int, string> touchStatus = new Dictionary<int, string>();
    private Dictionary<int, string> swipeStatus = new Dictionary<int, string>();
    private Dictionary<int, string> actionStatus = new Dictionary<int, string>();
    private Quaternion _controllerOrientation = new Quaternion();
    private int batteryPercentage;

    [SerializeField]
    private Text interactionData;
    [SerializeField]
    private GameObject rotateObject;
    [SerializeField]
    private Text ErrorText;
    private bool isInitialized;
    private bool isControllerConnected = false;
    AndroidJavaClass androidSystemClockObject;

    private void Start()
    {
        androidSystemClockObject = new AndroidJavaClass("android.os.SystemClock");
    }

    private void Awake()
    {
        JMRInputManager.Instance.AddGlobalListener(gameObject);
    }

    private void OnEnable()
    {
        isInitialized = false;
        skipFirstFrame = true;
        prevLatency = -1;
        Controllers = new List<IInputSource>();
        JMRInteractionManager.OnBatteryUpdate += OnBatteryUpdate;
        JMRInteractionManager.OnDisconnected += OnDisconnect;
        JMRInteractionManager.OnStartScanning += OnStartScan;
        JMRInteractionManager.OnConnected += OnConnect;
        JMRInteractionManager.OnEnvironmentNotSupported += OnEnviromentError;
        StartCoroutine(WaitTilFindController());
        Text text = GetTextElement(0);
        text.text = $"Controller : Online";
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
        JMRInteractionManager.OnBatteryUpdate -= OnBatteryUpdate;
        JMRInteractionManager.OnConnected -= OnConnect;
        JMRInteractionManager.OnDisconnected -= OnDisconnect;
        JMRInteractionManager.OnStartScanning -= OnStartScan;
        JMRInteractionManager.OnEnvironmentNotSupported -= OnEnviromentError;
        isInitialized = false;
        Controllers = new List<IInputSource>();
    }

    string deviceTypeSting = "";
    private void OnStartScan(JMRInteractionManager.InteractionDeviceType devType, int index)
    {
        Log($"OnStartScan({devType}, {index})");
    }

    private void OnDisconnect(JMRInteractionManager.InteractionDeviceType devType, int index, string val)
    {
        Log($"OnDisconnect({devType}, {index}, {val})");
        Text text = GetTextElement(0);
        text.text = $"Controller : Offline";
        isControllerConnected = false;
        latency = -1;
    }

    private void OnConnect(JMRInteractionManager.InteractionDeviceType devType, int index, string val)
    {
        Log($"OnConnect({devType}, {index}, {val})");
        if (!batteryLevels.ContainsKey(index)) batteryLevels.Add(index, -1);
        if (!touchStatus.ContainsKey(index)) touchStatus.Add(index, "None");
        if (!swipeStatus.ContainsKey(index)) swipeStatus.Add(index, "None");
        if (!actionStatus.ContainsKey(index)) actionStatus.Add(index, "None");
        Text text = GetTextElement(0);
        text.text = $"Controller : Online";
        isControllerConnected = true;
        Debug.Log("======> isControllerConnected =========> TRUE");
    }

    private void OnBatteryUpdate(JMRInteractionManager.InteractionDeviceType deviceType, int index, int obj)
    {
        batteryPercentage = obj;
        Log($"onBatteryUpdate({deviceType}, {index}, {obj})");
        batteryLevels[index] = obj;
        //deviceTypeSting = deviceType.ToString();
    }

    long latency = -1, maxLatency = -1, currentTimeStamp = -1,prevLatency = -1;
    bool skipFirstFrame = true;
    private float latencyDisplayDelay = 0.5f;
    private void CalculateLatency(long currTimeStamp)
    {
        if(!skipFirstFrame)
        {
            latency = ((androidSystemClockObject.CallStatic<long>("elapsedRealtimeNanos") - currTimeStamp) / 1000000L);
            maxLatency = latency > maxLatency ? latency : maxLatency;

            if(prevLatency == -1 || timer > latencyDisplayDelay)
            {
                timer = 0;
                prevLatency = latency;
            }
            else
            {
                latency = prevLatency;
            }
        }
        else
        {
            skipFirstFrame = false;
        }
        
    }

    private float timer = 0f;
    private void Update()
    {
        if (!isInitialized)
        {
            return;
        }

        timer += Time.deltaTime;
        if (Controllers[0].TryGetRotation(out _controllerOrientation, out currentTimeStamp))
        {
            if (currentTimeStamp > -1)
            {
                CalculateLatency(currentTimeStamp);
                rotateObject.transform.rotation = _controllerOrientation;
            }
        }

        interactionData.text = "Input Device Data" + "\n"
            // + $"Device Type: {deviceTypeSting}" + "\n"
            + "Handedness: " + (Controllers[0].GetHandedness() == Handedness.Universal || Controllers[0].GetHandedness() == Handedness.Right ? "Right" : " Left") + "\n"
            + $"Battery %: {JMRInteractionManager.Instance.GetBatteryPercentage(Controllers[0].inputIndex)}" + "\n"
            + $"Swipe Data: {swipeData}" + "\n"
            + $"Swipe (X,Y): {swipeDelta.ToString()}" + "\n"
            + $"Latency: {latency} ms" + "\n"
            + $"Maximum Latency: {maxLatency} ms" + "\n"
            ;
    }

    private Text GetTextElement(int i)
    {
        Text text;
        if (i < controllersUI.childCount)
        {
            text = controllersUI.GetChild(i).GetComponent<Text>();
        }
        else
        {
            text = Instantiate(controllersUI.GetChild(0), controllersUI).GetComponent<Text>();
            text.gameObject.name = "Text_" + i;
            text.transform.SetParent(controllersUI.transform);
        }

        text.gameObject.SetActive(true);
        return text;
    }

    private void OnEnviromentError(int ErrorCode)
    {
        StartCoroutine(ShowEnvionmentError());
    }


    private IEnumerator ShowEnvionmentError()
    {
        ErrorText.text = "ENVIRONMENT NOT SUPPORTED !!!";
        yield return new WaitForSeconds(5);
        ErrorText.text = "";
    }


    string swipeData = "";
    private void Log(string text)
    {
        Debug.Log("Interaction Example >> " + text);
    }
    Vector2 swipeDelta;
    public void OnTouchStart(TouchEventData eventData, Vector2 TouchData) { touchStatus[(int)eventData.SourceId] = $"Start : {TouchData}"; }
    public void OnTouchStop(TouchEventData eventData, Vector2 TouchData) => touchStatus[(int)eventData.SourceId] = $"Stop : {TouchData}";
    public void OnTouchUpdated(TouchEventData eventData, Vector2 TouchData) => touchStatus[(int)eventData.SourceId] = $"Updated : {TouchData}";
    public void OnSwipeLeft(SwipeEventData eventData, float value) { swipeData = $"Direction: Left, Value: {value}"; swipeStatus[(int)eventData.SourceId] = $"Left : {value}"; }
    public void OnSwipeRight(SwipeEventData eventData, float value) { swipeData = $"Direction: Right, Value: {value}"; swipeStatus[(int)eventData.SourceId] = $"Right : {value}"; }
    public void OnSwipeUp(SwipeEventData eventData, float value) { swipeData = $"Direction: Up, Value: {value}"; swipeStatus[(int)eventData.SourceId] = $"Up : {value}"; }
    public void OnSwipeDown(SwipeEventData eventData, float value) { swipeData = $"Direction: Down, Value: {value}"; swipeStatus[(int)eventData.SourceId] = $"Down : {value}"; }
    public void OnSwipeStarted(SwipeEventData eventData) => swipeStatus[(int)eventData.SourceId] = "Started";
    public void OnSwipeUpdated(SwipeEventData eventData, Vector2 swipeData) { swipeDelta = swipeData; } //=> swipeStatus[(int) eventData.SourceId] = "Updated";
    public void OnSwipeCompleted(SwipeEventData eventData) => swipeStatus[(int)eventData.SourceId] = "Completed";
    public void OnSwipeCanceled(SwipeEventData eventData) => swipeStatus[(int)eventData.SourceId] = "Canceled";
    public void OnSelectDown(SelectEventData eventData) => actionStatus[(int)eventData.SourceId] = "OnSelectDown";
    public void OnSelectUp(SelectEventData eventData) => actionStatus[(int)eventData.SourceId] = "OnSelectUp";
}
