using UnityEngine;
using JMRSDK;
using TMPro;
using JMRSDK.Dialogue;

public class JMRDemoDeviceExample : MonoBehaviour
{
    public TMP_Text LogText;

    private void OnEnable()
    {
        if (!Application.isEditor)
        {
            JMRRigManager.OnDeviceError += OnDeviceError;
        }
    }
    

    private void OnDisable()
    {
        if (!Application.isEditor)
        {
            JMRRigManager.OnDeviceError -= OnDeviceError;
        }
    }

    private void OnDeviceError(string errorMessage)
    {
        JMRSDKDialogueManager.Instance.Show("Device Error", errorMessage, "OK", OnErrorButtonClicked);
    }

    private void Start()
    {
        Invoke("UpdateData", 1f);
    }
    private void UpdateData()
    {

        if (!Application.isEditor)
        {
            LogText.text = "Device API \n" +
                           "Device ManufacturerName :\t" + (string.IsNullOrEmpty(JMRRigManager.Instance.GetManufacturerName()) ? "Not applicable!" : JMRRigManager.Instance.GetManufacturerName()) + "\n" +
                           "Device Protocal version :\t" + (((float)JMRRigManager.Instance.GetProtocolVersion() < 0) ? "Not applicable!" : JMRRigManager.Instance.GetProtocolVersion().ToString())  + "\n" +
                           "HMD Device version :\t" + (string.IsNullOrEmpty(JMRRigManager.Instance.GetHmdDeviceVersion()) ? "Not applicable!" : JMRRigManager.Instance.GetHmdDeviceVersion()) + "\n";
        }
        else
        {
            LogText.text = "Device API \n Device ManufacturerName:\t" + " JioGlass" + " Device code name:\t" + "Emulator";
        }

    }
    public void UpdateDeviceDetails()
    {
        Debug.Log("ButtonClicked");
        UpdateData();
    }

    public void OnErrorButtonClicked()
    {
        JMRSDKDialogueManager.Instance.Hide();
    }

}
