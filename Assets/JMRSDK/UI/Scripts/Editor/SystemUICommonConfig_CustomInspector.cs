using UnityEditor;
using JMRSDK.UI.ScriptableObjectClasses;
using UnityEngine;
using JMRSDK.UI.Configs;
using System.Text.RegularExpressions;

[CustomEditor(typeof(SO_SystemUICommonConfigs))]
public class SystemUICommonConfig_CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SO_SystemUICommonConfigs so = (SO_SystemUICommonConfigs)target;
        if (GUILayout.Button("Get JSON From Config"))
        {
            so.GetJsonFromConfig();
        }
        //if (GUILayout.Button("Set JSON To Config"))
        //{
        //    so.SetJsonToConfig();
        //}
    }
}

public static class SystemUICommonConfigs_JsonConverterExtension
{
    public static void FromJson(this SO_SystemUICommonConfigs so, string jsonString)
    {
        SystemUICommonConfigs data = JsonUtility.FromJson<SystemUICommonConfigs>(jsonString);

        void ToSOData(SystemUICommonConfigs config)
        {
            so.transitionData = config.transitionData;
            so.skinTemplate = config.skinTemplate;
            so.popupConfigDataContainer = config.popupConfigDataContainer;
        }

        //no need for assetdatabase.create as no so ref here

        ToSOData(data);
    }

    public static string ToJson(this SO_SystemUICommonConfigs so)
    {
        string temp = "";

        SystemUICommonConfigs data = new SystemUICommonConfigs();

        data.transitionData = so.transitionData;
        data.skinTemplate = so.skinTemplate;
        data.popupConfigDataContainer = so.popupConfigDataContainer;

        data.popupConfigDataContainer.ForEach(x =>
        {
            x.prefabPath = AssetDatabase.GetAssetPath(x.prefab).Split('.')[0];
            x.prefabPath = Regex.Replace(x.prefabPath, ".*Resources/", string.Empty);
        });

        temp = JsonUtility.ToJson(data);

        return temp;
    }


    [ContextMenu("Custom/GetJsonFromConfig")]
    public static void GetJsonFromConfig(this SO_SystemUICommonConfigs so)
    {
        so.outputJsonString = so.ToJson();
    }

    [MenuItem("Custom/SetJsonToConfig")]
    public static void SetJsonToConfig(this SO_SystemUICommonConfigs so)
    {
        so.FromJson(so.inputJsonString);
    }

}
