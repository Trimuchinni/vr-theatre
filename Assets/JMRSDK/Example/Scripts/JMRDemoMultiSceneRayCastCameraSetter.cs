using UnityEngine;

namespace JMRSDK.Demo
{
    [RequireComponent(typeof(Canvas))]
    public class JMRDemoMultiSceneRayCastCameraSetter : MonoBehaviour
    {
        private Canvas canavs;
        private JMRUIRayCastCamera rayCastCamera;

        private void Start()
        {
            canavs = GetComponent<Canvas>();
            rayCastCamera = FindObjectOfType<JMRUIRayCastCamera>();
            if(canavs.renderMode == RenderMode.WorldSpace)
            {
                canavs.worldCamera = rayCastCamera.GetRayCastCamera();
            }
        }
    }
}
