using UnityEngine;
using UnityEngine.SceneManagement;
using JMRSDK.InputModule;

public class JMRDemoTestGlobalBackAction : MonoBehaviour, IBackHandler
{
    [SerializeField]
    private GameObject homebutton;

    private void OnEnable()
    {
        JMRInputManager.Instance.AddGlobalListener(gameObject);
    }

    private void OnDisable()
    {
        if (JMRInputManager.Instance != null)
        {
            JMRInputManager.Instance.RemoveGlobalListener(gameObject);
        }
    }

    public void OnBackAction()
    {
        if (SceneManager.GetActiveScene().buildIndex != 4 && SceneManager.GetActiveScene().buildIndex != 0)
        {
            SceneManager.LoadScene(0);
        }
    }

    public void OnButtonBackAction()
    {
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 4)
        {
            homebutton.SetActive(false);
        }
        else
        {
            homebutton.SetActive(true);
        }
    }
}
