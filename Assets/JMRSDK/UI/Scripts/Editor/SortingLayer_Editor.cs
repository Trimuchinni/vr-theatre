using UnityEngine;
using UnityEditor;

public class SortingLayer_Editor : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("JioMixedReality/SystemUI/UpdateSortingLayer")]
    public static void CreateSortingLayer()
    {
        string layerName = "SystemUI";
        int sourceIndex = -1;

        var serializedObject = new SerializedObject(AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset"));
        var sortingLayers = serializedObject.FindProperty("m_SortingLayers");
        for (int i = 0; i < sortingLayers.arraySize; i++)
            if (sortingLayers.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue.Equals(layerName))
            {
                sourceIndex = i;
                break;
            }

        if (sourceIndex != -1)
        {
            sortingLayers.MoveArrayElement(sourceIndex, sortingLayers.arraySize - 1);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        sortingLayers.InsertArrayElementAtIndex(sortingLayers.arraySize);
        var newLayer = sortingLayers.GetArrayElementAtIndex(sortingLayers.arraySize - 1);
        newLayer.FindPropertyRelative("name").stringValue = layerName;
        newLayer.FindPropertyRelative("uniqueID").intValue = layerName.GetHashCode(); /* some unique number */
        serializedObject.ApplyModifiedProperties();
    }

#endif
}
