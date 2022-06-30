using UnityEngine;
using JMRSDK.InputModule;
using UnityEngine.UI;
using JMRSDK.Toolkit;

public class JMRDemoDwellAndGazeExample : MonoBehaviour
{
    public Text modeText, PrefferedModeText;
    private bool isControllerConnected;

    [SerializeField]
    private JMRUIPrimaryButton switchButton; 
    private void Start()
    {
        JMRInteractionManager.OnConnected += OnJioGlassControllerConnected;
        JMRInteractionManager.OnDisconnected += OnJioGlassControllerDisconnected;

        PrefferedModeText.text = "Prefferd Mode: " + JMRPointerManager.Instance.PrefferedPointingSource;
    }

    /// <summary>
    /// Update information on JioGlass controller connection 
    /// </summary>
    /// <param name="devicetype"></param>
    /// <param name="index"></param>
    /// <param name="name"></param>
    private void OnJioGlassControllerConnected(JMRInteractionManager.InteractionDeviceType devicetype, int index, string name)
    {
        UpdateText("Controller Connected");
        isControllerConnected = true;    
    }

    /// <summary>
    /// Update information on JioGlass controller disconnection 
    /// </summary>
    /// <param name="devicetype"></param>
    /// <param name="index"></param>
    /// <param name="name"></param>
    private void OnJioGlassControllerDisconnected(JMRInteractionManager.InteractionDeviceType devicetype, int index, string name)
    {
        isControllerConnected = false;
        UpdateText("Controller Disonnected");     
    }

    private void ChangePointingMode()
    {
        SwitchMode();            
    }

    private void UpdateText(string message)
    {
       if(message != null)
        {
            modeText.text = message;
        }
    }


    public void SwitchMode()
    {       
        JMRPointerManager.Instance.SwitchPointingSource();
        PrefferedModeText.text = "Prefferd Mode: " + JMRPointerManager.Instance.PrefferedPointingSource;
    }
  
}
