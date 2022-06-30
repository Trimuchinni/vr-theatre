using JMRSDK.InputModule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JMRDemoLocalInteractionExample : MonoBehaviour, ISelectHandler, ISelectClickHandler, IFn1Handler, IFn2Handler, IBackHandler, IFocusable, IVoiceHandler, IManipulationHandler, ITouchHandler
{
    public Text _text;

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

    #region IFocusable
    public void OnFocusEnter() => _text.text = "OnFocusEnter";
    public void OnFocusExit() => _text.text = "OnFocusExit";
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