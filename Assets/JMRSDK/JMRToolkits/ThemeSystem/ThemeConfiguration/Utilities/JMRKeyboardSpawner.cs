using JMRSDK;
using JMRSDK.Toolkit.UI;
using UnityEngine;

public class JMRKeyboardSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject keyboardPrefab;
    [SerializeField]
    private GameObject keyboardV2Prefab;
    private JMRUIRayCastCamera j_JMRRaycastCamera;
    public static JMRKeyboardSpawner Instance;
    [SerializeField]
    private bool supportV2Keyboard = true;

    private void Awake()
    {
        Instance = this;
    }

    public bool SpawnKeyboard(IKeyboardInput input)
    {
        if (!keyboardPrefab && !supportV2Keyboard)
        {
            return false;
        }
        else if (!keyboardV2Prefab && supportV2Keyboard)
        {
            return false;
        }

        JMRVirtualKeyBoard obj = Instantiate(supportV2Keyboard ? keyboardV2Prefab : keyboardPrefab).GetComponent<JMRVirtualKeyBoard>();
        SetRaycastCameraToCanvas(obj.GetComponent<Canvas>());
        obj.ShowKeyBoard(input);
        return true;
    }

    private void SetRaycastCameraToCanvas(Canvas j_Canvas)
    {
        if (!j_Canvas)
        {
            //Debug.LogError("Cannot find Canvas component to process");
            return;
        }
        if (!j_JMRRaycastCamera)
            j_JMRRaycastCamera = FindObjectOfType<JMRUIRayCastCamera>();
        if (j_JMRRaycastCamera != null)
            j_Canvas.worldCamera = j_JMRRaycastCamera.GetRayCastCamera();
    }
}
