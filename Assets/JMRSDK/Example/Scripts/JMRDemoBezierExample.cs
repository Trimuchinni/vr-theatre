using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMRSDK.InputModule;


public class JMRDemoBezierExample : MonoBehaviour, IManipulationHandler
{
    public void OnManipulationCompleted(ManipulationEventData eventData)
    {
        JMRPointerManager.Instance.FocusLocked = false;
    }

    public void OnManipulationStarted(ManipulationEventData eventData)
    {
        //Bezier is activated upon focus lock
        JMRPointerManager.Instance.FocusLocked = true;
    }

    public void OnManipulationUpdated(ManipulationEventData eventData)
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
