using UnityEngine;
using JMRSDK;
using TMPro;
using System;
using UnityEngine.UI;

public class JMRDemoDisplayExample : MonoBehaviour
{
    public TMP_Text LogText;
    public TMP_Text EventText;
    public TMP_Text GetterText;

    public Toggle BrightnessToggle;
    public Toggle DisplayToggle;
    public Toggle PowerToggle;
    public Toggle PowerStateToggle;
    private void Start()
    {
        BrightnessToggle.onValueChanged.AddListener(delegate {
            ToggleBrightnessMode(BrightnessToggle);
        });

        PowerToggle.onValueChanged.AddListener(delegate {
            TogglePowerMode(PowerToggle);
        });

        DisplayToggle.onValueChanged.AddListener(delegate {
            ToggleDisplayMode(DisplayToggle);
        });

        PowerStateToggle.onValueChanged.AddListener(delegate {
            TogglePowerState(PowerStateToggle);
        });
    }

    private void OnEnable()
    {
        JMRDisplayManager.onError += OnError;
        JMRDisplayManager.onBrightnessChange += OnBrightnessChanged;
        JMRDisplayManager.onContrastChange += OnContrastChanged;
        JMRDisplayManager.onBrightnessModeChange += OnContrastChanged;
        JMRDisplayManager.onConnect += OnConnect;
        JMRDisplayManager.onDisconnect += OnDisconnect;
        JMRDisplayManager.onDisplayModeChange += OnDisplayModeChange;
        JMRDisplayManager.onPowerModeChange += OnPowerModeChange;
        JMRDisplayManager.onPowerStateChange += OnPowerStateChange;

    }

    private void OnDisable()
    {
        JMRDisplayManager.onError -= OnError;
        JMRDisplayManager.onBrightnessChange -= OnBrightnessChanged;
        JMRDisplayManager.onContrastChange -= OnContrastChanged;
        JMRDisplayManager.onBrightnessModeChange += OnBrightnessModeChange;
        JMRDisplayManager.onConnect += OnConnect;
        JMRDisplayManager.onDisconnect += OnDisconnect;
        JMRDisplayManager.onDisplayModeChange += OnDisplayModeChange;
        JMRDisplayManager.onPowerModeChange += OnPowerModeChange;
        JMRDisplayManager.onPowerStateChange += OnPowerStateChange;
    }

    private void OnError(string error)
    {
        ShowEventText(error);
    }

    private void OnBrightnessChanged(int value)
    {
        EventText.text = $"Display Brightness changed : {value}";
    }
    private void OnContrastChanged(int value)
    {
        EventText.text = $"Display Contrast changed : {value}";
    }

    private void OnBrightnessModeChange(int value)
    {
        EventText.text = $"Display Brightness Mode changed : {value}";
    }

    public void SetBrightnessValue(float value)
    {
        float Percentage = value * 100;   // Value is between 0 - 1, so percetage calculation would be (value/1)*100, i.e. value*100
        JMRDisplayManager.Instance.SetBrightness((int)Percentage);
    }

    public void ToggleBrightnessMode(Toggle value)
    {
        Debug.Log("[display demo] : Brighness mode toggled 1 " + value.isOn);
        JMRDisplayManager.Instance.SetDisplayBrightnessMode((value.isOn) ?1:0);
    }

    public void TogglePowerMode(Toggle value)
    {
        JMRDisplayManager.Instance.SetDisplayPowerMode((value.isOn) ? 1 : 0); 
    }

    public void TogglePowerState(Toggle value)
    {
        JMRDisplayManager.Instance.SetDisplayPowerState((value.isOn) ? 1 : 0);
    }

    public void ToggleDisplayMode(Toggle value)
    {
        Debug.Log("[display demo] : display mode toggled 1 " + value.isOn);
        JMRDisplayManager.Instance.SetDisplayMode((value.isOn) ? 2 : 3);
    }

    public void ButtonActions(string Action)
    {
        switch(Action)
        {
            case "GetBrightnessMode":
                GetterText.text = JMRDisplayManager.Instance.GetDisplayBrightnessMode().ToString();                
                break;
            case "GetPowerMode":
                GetterText.text = JMRDisplayManager.Instance.GetDisplayPowerControlMode().ToString();
                break;
            case "GetDisplayMode":
                GetterText.text = JMRDisplayManager.Instance.GetDisplayMode().ToString();
                break;
            case "GetPowerState":
                GetterText.text = JMRDisplayManager.Instance.GetDisplayPowerState().ToString();
                break;
        }
    }



    private void OnConnect()
    {
        EventText.text = "Display Connected";
    }

    private void OnDisconnect()
    {
        EventText.text = "Display Disconnected";
    }

    private void OnDisplayModeChange(int value)
    {
        EventText.text = $"Display Mode changed : {value}";
    }
    private void OnPowerModeChange(int value)
    {
        EventText.text = $"Display Power Mode changed : {value}";
    }

    private void OnPowerStateChange(int value)
    {
        EventText.text = $"Display Power State changed : {value}";
    }

    public void ShowEventText(string s)
    {
        EventText.text = $"Display status: {s}";
    }

    private bool iePrevConnected;
    private void LateUpdate()
    {
        if (!Application.isEditor && JMRDisplayManager.Instance != null)
        {
            if (iePrevConnected != JMRDisplayManager.Instance.IsConnected())
            {
                if (JMRDisplayManager.Instance.IsConnected())
                {
                    ShowEventText("Connected");
                }
                else
                {
                    ShowEventText("Disconnected");
                }
            }

            LogText.text = "Display API \n "
                           + "Display \t" + JMRDisplayManager.Instance.GetDisplayMode() + "\n"
                           + "Display Brightness:\t" + (((float)JMRDisplayManager.Instance.GetDisplayBrightness() < 0) ? "Not applicable!" : JMRDisplayManager.Instance.GetDisplayBrightness().ToString()) + "\n"
                           + "Display Power State\t" + (((float)JMRDisplayManager.Instance.GetDisplayPowerState() < 0) ? "Not applicable!" : JMRDisplayManager.Instance.GetDisplayPowerState().ToString()) + "\n"
                           + "Display Power Control Mode \t" + (((float)JMRDisplayManager.Instance.GetDisplayPowerControlMode() < 0) ? "Not applicable!" : JMRDisplayManager.Instance.GetDisplayPowerControlMode().ToString()) + "\n"
                           + "Device Brightness Mode \t" + (((float)JMRDisplayManager.Instance.GetDisplayBrightnessMode() < 0) ? "Not applicable!" : JMRDisplayManager.Instance.GetDisplayBrightnessMode().ToString()) + "\n"
                ;
            iePrevConnected = JMRDisplayManager.Instance.IsConnected();
        }
        else
        {
            LogText.text = "Display Information will displayed here!";
        }
    }
}
