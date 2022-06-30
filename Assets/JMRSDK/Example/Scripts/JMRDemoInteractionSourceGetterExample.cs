using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMRSDK;
using JMRSDK.InputModule;
using UnityEngine.UI;

public class JMRDemoInteractionSourceGetterExample : MonoBehaviour
{
    IInputSource GetSourceByHandedness;
    IInputSource CurrentSource;
    List<IInputSource> GetSources = new List<IInputSource>();

    [SerializeField]
    private Text J_Title;

    [SerializeField]
    private Text J_CurrentSelectTryGetButton;
    [SerializeField]
    private Text J_CurrentBackTryGetButton;
    [SerializeField]
    private Text J_CurrentHomeTryGetButton;
    [SerializeField]
    private Text J_CurrentFunctionTryGetButton;

    Handedness SourceHand = Handedness.Left;

    [SerializeField]
    private Text J_CurrentSourceRotation;


    [SerializeField]
    private Text J_CurrentSourcePosition;

    Quaternion SourceRotation;
    Vector3 SourcePosition;

    void Start()
    {
        //Example to get the current source
        CurrentSource = JMRInteractionManager.Instance.GetCurrentSource();

        //Example to get a list of all sources connected to the device
        GetSources = JMRInteractionManager.Instance.GetSources();
    }

    // Update is called once per frame
    void Update()
    {

        if (CurrentSource != null)
        {

            SourceHand = CurrentSource.GetHandedness();
            J_Title.text = "Controller input source :" + SourceHand;

            // Example to get button states from the source.
            J_CurrentSelectTryGetButton.text = "TryGetSelect() :" + CurrentSource.TryGetSelect().ToString();
            J_CurrentBackTryGetButton.text = "TryGetBack() :" + CurrentSource.TryGetBack().ToString();
            J_CurrentHomeTryGetButton.text = "TryGetHome() :" + CurrentSource.TryGetHome().ToString();
            J_CurrentFunctionTryGetButton.text = "TryGetFunctionButton() :" + CurrentSource.TryGetFunctionButton().ToString();

            // Example to get source rotation
            if (CurrentSource.TryGetRotation(out SourceRotation))
                J_CurrentSourceRotation.text = "TryGetRotation(out SourceRotation) :" + SourceRotation.ToString();

            //Example to get source position
            //NOTE: current Source doesn't provide positional data
            if (CurrentSource.TryGetPosition(out SourcePosition))
                J_CurrentSourcePosition.text = "TryGetPosition(out SourcePosition) :" + SourcePosition.ToString();

        }
        else
        {
            if (CurrentSource == null)
            {
                CurrentSource = JMRInteractionManager.Instance.GetCurrentSource();
            }

            // CurrentSource = JMRInteractionManager.Instance.GetSourceByHandedness(Handedness.Right);
        }
    }
    public void SwitchpointingSource()
    {
        if (!Application.isEditor)
        {
            JMRPointerManager.Instance.SwitchPointingSource();
        }
    }
    public void DisableHomeLaunch()
    {
        if (!Application.isEditor)
        {
            JMRInteractionManager.Instance.DisableHomeLaunch(true);
        }
    }
    public void EnableHomeLaunch()
    {
        if (!Application.isEditor)
        {
            JMRInteractionManager.Instance.DisableHomeLaunch(false);
        }
    }
}
