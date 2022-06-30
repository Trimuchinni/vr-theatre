using UnityEditor;
using JMRSDK.UI.ScriptableObjectClasses;
using UnityEngine;
using JMRSDK.UI.Configs;
using System.Text.RegularExpressions;
using JMRSDK.UI;
using System;
using JMRSDK.UI.Utils;
using Random = UnityEngine.Random;
using System.Collections.Generic;

[CustomEditor(typeof(SO_PopupConfigData_Container))]
public class PopupConfigDataContainer_CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SO_PopupConfigData_Container so = (SO_PopupConfigData_Container)target;
        if (GUILayout.Button("Get JSON From Config"))
        {
            so.GetJsonTest();
        }
        //if (GUILayout.Button("Set JSON To Config"))
        //{
        //    so.SetJsonToConfig();
        //}
    }
}

public static class PopupConfigData_Container_JsonConverterExtension
{
    [ContextMenu("GetJsonTest")]
    public static void GetJsonTest(this SO_PopupConfigData_Container so)
    {
        so.jsonOutput = so.ToJson();
    }

    [ContextMenu("LoadFromJsonTest")]
    public static void LoadFromJsonTest(this SO_PopupConfigData_Container so)
    {
        PopupConfigContainer test = JsonUtility.FromJson<PopupConfigContainer>(so.jsonInput);
        so.FromJson(test);
    }


    [ContextMenu("FillRandomDataForTest")]
    public static void FillRandomDataForTest(this SO_PopupConfigData_Container so)
    {
        so.popupConfigDataContainer.Clear();

        var containerPath = System.IO.Directory.GetParent(AssetDatabase.GetAssetPath(so)).ToString().Replace("\\", "/");
        if (!AssetDatabase.IsValidFolder(containerPath + "/Entries"))
        {
            AssetDatabase.CreateFolder(containerPath, "Entries");
        }

        for (int i = 0; i < 10; i++)
        {
            PopupConfigData config = new PopupConfigData()
            {
                popUpId = i.ToString(),
                behaviourType = (PopUpBehaviourType)Random.Range(0, 4),
                prefabType = (PopUpPrefabType)Random.Range(0, 2),
                priority = Random.Range(0, 10),
                headertext = i.ToString(),
                headerIconPath = i.ToString(),
                bodyText = DateTime.UtcNow.ToString(),
                interactions = new List<ButtonInteractions>()
                    {
                        new ButtonInteractions() { type = ButtonType.Primary, interactionString = "Primary_" + i.ToString() },
                        new ButtonInteractions() { type = ButtonType.Secondary, interactionString = "Secondary_" + i.ToString() }
                    },
                //  hasLoadingScreen = false,
                // hasProgressbar = false,
                isAutoClose = Random.Range(0, 2) < 1 ? true : false,
                followBehaviours = (FollowBehaviours)Random.Range(0, 3),
                popUpVisibleDuration = 2f,
                transitionData = new ToastTransitionData() { useCommon = true },
                // progressBar = new ProgressBarData() { useCommon = true },
                // loading = new LoadingScreenData() { loadingTitle = "loading : " + i.ToString() },
            };

            var soRef = SO_PopupConfigData.CreateInstance<SO_PopupConfigData>();
            soRef = new SO_PopupConfigData(config);
            AssetDatabase.CreateAsset(soRef, String.Format(System.IO.Directory.GetParent(AssetDatabase.GetAssetPath(so)).ToString().Replace("\\", "/") + "/Entries/{0}.asset", soRef.popUpId));
            so.popupConfigDataContainer.Add(soRef);
        }
    }

    public static void FromJson(this SO_PopupConfigData_Container so, PopupConfigContainer data)
    {
        so.popupConfigDataContainer.Clear();
        //myDict.Clear();
        data.popupConfigData.ForEach(x =>
        {
            var soref = SO_PopupConfigData.CreateInstance<SO_PopupConfigData>();
            soref = new SO_PopupConfigData(x);
            AssetDatabase.CreateAsset(soref, String.Format("Assets/SO/Entries/{0}.asset", x.popUpId.ToString()));

            so.popupConfigDataContainer.Add(soref);
        });
    }

    public static string ToJson(this SO_PopupConfigData_Container so)
    {
        string temp = "";

        PopupConfigContainer wPopupConfigContainer = new PopupConfigContainer();
        //var configs = wPopupConfigContainer.popupConfigData;

        so.popupConfigDataContainer.ForEach(x =>
        {
            wPopupConfigContainer.popupConfigData.Add(new PopupConfigData(x));
        });

        temp = JsonUtility.ToJson(wPopupConfigContainer);

        return temp;
    }
}
