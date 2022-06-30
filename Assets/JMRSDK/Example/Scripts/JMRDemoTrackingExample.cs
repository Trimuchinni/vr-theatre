using JMRSDK;
using System;
using UnityEngine;
using UnityEngine.UI;

public class JMRDemoTrackingExample : MonoBehaviour
{
    public Text LogText;
    public Transform cubeParent;

    #region LatencyTest
    AndroidJavaClass androidSystemClockObject;
    private long timestampNs;
    int latencytrashhold = 5;
    int lessthenTrashold = 0;
    int graterthenTrashold = 0;

    public float timeInterval = 60; // interval in seconds
    float actualTime;
    float Percentile = 0f;
    #endregion

    private void Awake()
    {
        #region LatencyTest
        androidSystemClockObject = new AndroidJavaClass("android.os.SystemClock");
        timeInterval = 60; // set interval for 5 minutes
        actualTime = timeInterval; // set actual time left until next update
        #endregion
    }


    private void Start()
    {
        androidSystemClockObject = new AndroidJavaClass("android.os.SystemClock");
    }

    private void OnEnable()
    {
        skipFirstFrame = true;
        prevLatency = -1;
        JMRTrackerManager.OnHeadPosition += OnHeadPosition;
        JMRTrackerManager.OnHeadRotation += OnHeadRotation;
        JMRTrackerManager.OnHeadPoseTimestamp += OnHeadPoseTimestamp;
    }

    private void OnDisable()
    {
        JMRTrackerManager.OnHeadPosition -= OnHeadPosition;
        JMRTrackerManager.OnHeadRotation -= OnHeadRotation;
        JMRTrackerManager.OnHeadPoseTimestamp -= OnHeadPoseTimestamp;
    }

    private void OnHeadPosition(Vector3 obj)
    {
        cubeParent.localPosition = obj;
    }

    private void OnHeadRotation(Quaternion obj)
    {
        cubeParent.localRotation = obj;
    }

    private long maxDelay = -1, latency = -1, prevLatency = -1, currentTimeStamp = -1;
    private bool skipFirstFrame = true;
    private float latencyDisplayDelay = 0.5f;
    private void OnHeadPoseTimestamp(long currTimeStamp)
    {

    }

    private void CalculateLatency(long currTimeStamp)
    {
        if (!skipFirstFrame)
        {
            latency = ((androidSystemClockObject.CallStatic<long>("elapsedRealtimeNanos") - currTimeStamp) / 1000000L);
            maxDelay = latency > maxDelay ? latency : maxDelay;

            if (prevLatency == -1 || timer > latencyDisplayDelay)
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

    private void CalculatePercentile(long currTimeStamp)
    {
        #region LatencyTest
        timestampNs = currTimeStamp;
        var latancy = ((androidSystemClockObject.CallStatic<long>("elapsedRealtimeNanos") - timestampNs) / 1000000L);
        if (latancy < latencytrashhold)
        {
            lessthenTrashold += 1;
            //Debug.LogWarning("Tracking Pose Latency  delay between frmes  " + latancy);
        }
        else
        {
            graterthenTrashold += 1;
            //Debug.LogWarning("Tracking Pose Latency  delay between frmes  " + latancy);
        }

        actualTime -= Time.deltaTime; // subtract the time taken to render last frame

        if (actualTime <= 0) // if time runs out, do your update
        {
            Percentile = (lessthenTrashold * 100) / (lessthenTrashold + graterthenTrashold);
           // Debug.LogError("Lessthen count " + lessthenTrashold + " graterthen count :" + graterthenTrashold);
           //Debug.LogError("Tracking latancy percentage, Less then" + latencytrashhold + "trashold is :" + Percentile + "%");

            actualTime = timeInterval; // reset the timer
        }

        #endregion
    }


    private float timer = 0f;
    private void Update()
    {
        timer += Time.deltaTime;

        currentTimeStamp = JMRTrackerManager.Instance.GetHeadPoseTimestamp();

        if (currentTimeStamp > -1)
        {
            CalculateLatency(currentTimeStamp);
            CalculatePercentile(currentTimeStamp);
        }


        LogText.text = $"Tracking API\n" +
                       $"Position : {cubeParent.localPosition}\n" +
                       $"Rotation : {cubeParent.localRotation}\n" +
                       $"Latency : {latency} ms\n" +
                       $"Maximum Delay : {maxDelay.ToString()} ms\n" +
                       $"Latency percentile under 5 ms/Min  :{Percentile} %";
    }
}