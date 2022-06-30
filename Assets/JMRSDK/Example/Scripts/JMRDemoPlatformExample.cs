using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JMRSDK;
using UnityEngine.UI;
using System;
using TMPro;

public class JMRDemoPlatformExample : MonoBehaviour
{
    public TMP_Text LogText;

    string batteryLevel = string.Empty;
    string updateData = string.Empty;
    string chargingStatus = string.Empty;
    string lowBatteryStatus = string.Empty;

    private void OnEnable()
    {
        if (!Application.isEditor)
        {
            JMRPlatformManager.OnBatteryLevelChanged+= onBatteryLevelChanged;
            JMRPlatformManager.OnCharging += onCharging;
            JMRPlatformManager.OnLowBattery += onLowBattery;
        }
    }
    

    private void OnDisable()
    {
        if (!Application.isEditor)
        {
            JMRPlatformManager.OnBatteryLevelChanged -= onBatteryLevelChanged;
            JMRPlatformManager.OnCharging -= onCharging;
            JMRPlatformManager.OnLowBattery -= onLowBattery;
        }
    }
    
    private void onBatteryLevelChanged(int level)
    {
        batteryLevel = "\n Battery Level:\t"+level.ToString();
        UpdateLogData();
    }
    private void onLowBattery(bool status)
    {
        lowBatteryStatus = "\n Battery Low:\t" + status;
        UpdateLogData();
    }

    private void onCharging(bool status)
    {
        chargingStatus = "\n Charging Status:\t" + status;
        UpdateLogData();
    }

    void UpdateLogData()
    {
        LogText.text = updateData + batteryLevel + lowBatteryStatus + chargingStatus;
    }

    private void Start()
    {
        Invoke("UpdateData", 1f);
        // UpdateData();
    }
    private void UpdateData()
    {

        if (!Application.isEditor)
        {
            LogText.text = "Platform API \n" +
                           "Platform ManufacturerName:\t" + JMRPlatformManager.Instance.GetManufacturerName() + "\n" +
                           "Platform code name:\t" + JMRPlatformManager.Instance.GetCodeName() + "\n" +
                            "Platform BuildNumber:\t" + JMRPlatformManager.Instance.GetBuildNumber() + "\n" +
                           "Platform OsApiLevel:\t" + JMRPlatformManager.Instance.GetOsApiLevel() + "\n" +
                           "Platform OsVersion:\t" + JMRPlatformManager.Instance.GetOsVersion() + "\n" +
                           "Platform OsVersionName:\t" + JMRPlatformManager.Instance.GetOsVersionName() + "\n" +
                           "Platform GetSecurityPatchLevel:\t" + JMRPlatformManager.Instance.GetSecurityPatchLevel() + "\n" +
                           "Platform WifiMac:\t" + JMRPlatformManager.Instance.GetWifiMac() + "\n" +
                           "Platform ExtendedDisplaySupported:\t" + JMRPlatformManager.Instance.isExtendedDisplaySupported() + "\n";
            updateData = LogText.text;
            //+ "\n"
            //+ "Platform Security patch:\t" + platformAPI.getSecurityPatchLevel() + "\n"
            //+ "Platform build Number:\t" + platformAPI.getBuildNumber() + "\n"
            //+ "Platform version:\t" + platformAPI.getVersion() + "\n"
            //+ "Platform Level:\t" + platformAPI.getLevel() + "\n"
            //+ "Platform Ver Name:\t" + platformAPI.getVersionName() + "\n"
            //+ "Platform Wifi Mac:\t" + platformAPI.getWifiMac() + "\n"
            //+ "Platform sdk version:\t" + platformAPI.getSDKVersion() + "\n"
            //+ "Platform Sdk Level:\t" + platformAPI.getSDKLevel() + "\n"
            //+ "Platform sdk Name:\t" + platformAPI.getSDKVersionName() + "\n"
            //+ "Platform service ver:\t" + platformAPI.getServiceVersion() + "\n"
            //+ "Platform isExtendedDisplaySupported:\t" + platformAPI.isExtendedDisplaySupported() + "\n"
            //;
        }
        else
        {
            LogText.text = "Platform API \n Platform ManufacturerName:\t" + " JioGlass" + " Platform code name:\t" + "Emulator";
        }

    }
    public void UpdatePlatformDetails()
    {
        Debug.Log("ButtonClicked");
        UpdateData();
    }
}
