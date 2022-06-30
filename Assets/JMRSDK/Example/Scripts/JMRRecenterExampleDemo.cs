using UnityEngine;
using JMRSDK.InputModule;
using UnityEngine.UI;
using System.Diagnostics;

public class JMRRecenterExampleDemo : MonoBehaviour
{
    public Text RecenterEventStatus;

    Stopwatch stopWatch = new Stopwatch();

    float value;

    void Start()
    {
        JMRSystemActions.Instance.OnRecenterStart.AddListener(RecenterStart);
        JMRSystemActions.Instance.OnRecenterEnd.AddListener(RecenterCompleted);
        JMRSystemActions.Instance.OnRecenterCancelled.AddListener(RecenterCancel);
    }


    void RecenterStart()
    {      
        RecenterEventStatus.text = "Recenter Start";
        stopWatch.Reset();
        stopWatch.Start();
    }

    void RecenterCancel()
    {       
        RecenterEventStatus.text = "Recenter Cancel";
        stopWatch.Stop();
        UnityEngine.Debug.Log("Recenter Time :"+stopWatch.ElapsedTicks);
    }

    public void RecenterCompleted()
    {       
        RecenterEventStatus.text = "Recenter End";
        stopWatch.Stop();
        UnityEngine.Debug.Log("Recenter Time :" + stopWatch.ElapsedTicks);
        value = stopWatch.ElapsedTicks;       
    }

    
}
