using JMRSDK.InputModule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JMRDemoGlobalInteractionExample : MonoBehaviour, ISelectHandler, ISelectClickHandler, IFn1Handler, IFn2Handler, IBackHandler, IVoiceHandler, IManipulationHandler, ITouchHandler
{
    public Text _text;

    void Start()
    {
        
    }
    private void OnEnable()
    {
        JMRInputManager.Instance.AddGlobalListener(gameObject);
    }
    private void OnDisable()
    {
        if(gameObject!=null && JMRInputManager.Instance!=null)
        JMRInputManager.Instance.RemoveGlobalListener(gameObject);
    }
    void OnDestroy()
    {
        if (gameObject != null && JMRInputManager.Instance != null)
            JMRInputManager.Instance.RemoveGlobalListener(gameObject);
    }

    #region ISelectClickHandler
    public void OnSelectDown(SelectEventData eventData) => _text.text = "OnSelectDown";
    public void OnSelectUp(SelectEventData eventData) => _text.text = "OnSelectUp";
    #endregion

    #region ISelectClickHandler
    public void OnSelectClicked(SelectClickEventData eventData) => _text.text = "OnSelectClick";
    #endregion

    #region IFn1Handler
    public void OnFn1Action() => _text.text = "OnFn1";
    #endregion

    #region IFn2Handler
    public void OnFn2Action() => _text.text = "OnFn2";
    #endregion

    #region IBackHandler
    public void OnBackAction() => _text.text = "OnBack";
    #endregion

    #region IVoiceAction
    public void OnVoiceAction() => _text.text = "OnVoiceAction";
    #endregion

    #region IManipulationHandler
    public void OnManipulationStarted(ManipulationEventData eventData) => _text.text = "OnManipulationStarted";
    public void OnManipulationUpdated(ManipulationEventData eventData) => _text.text = "OnManipulationUpdated";
    public void OnManipulationCompleted(ManipulationEventData eventData) => _text.text = "OnManipulationCompleted";
    #endregion

    #region ITouchHandler
    public void OnTouchStart(TouchEventData eventData, Vector2 TouchData) => _text.text = "OnTouchStart";
    public void OnTouchStop(TouchEventData eventData, Vector2 TouchData) => _text.text = "OnTouchStop";
    public void OnTouchUpdated(TouchEventData eventData, Vector2 TouchData) => _text.text = "OnTouchUpdated";
    #endregion
}