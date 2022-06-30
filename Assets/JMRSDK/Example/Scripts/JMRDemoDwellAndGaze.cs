using UnityEngine;
using JMRSDK.InputModule;
using UnityEngine.UI;

public class JMRDemoDwellAndGaze : MonoBehaviour, IDwellHandler
{
    [SerializeField] private Text statusText;
    public void OnDwellCancel()
    {
        UpdateText(" OnDwellCancel :: " + gameObject.name );
    }

    public void OnDwellCompleted()
    {
        UpdateText("OnDwellCompleted :: " + gameObject.name);
    }

    public void OnDwellStart()
    {
        UpdateText("OnDwellStart :: " + gameObject.name);
    }


    void UpdateText(string text = "")
    {
        if(statusText != null)
        {
            statusText.text = text;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
