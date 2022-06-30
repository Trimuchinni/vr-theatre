using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using JMRSDK.InputModule;

public class JMRDemoInteractionGettersExample : MonoBehaviour
{
    public TMP_Text txt_swipeLeftBool;
    public TMP_Text txt_swipeRightBool;
    public TMP_Text txt_swipeUpBool;
    public TMP_Text txt_swipeDownBool;

    public TMP_Text txt_swipeLeftVal;
    public TMP_Text txt_swipeRightVal;
    public TMP_Text txt_swipeUpVal;
    public TMP_Text txt_swipeDownVal;

    public TMP_Text txt_isTouching;
    public TMP_Text txt_touchVector;

    public TMP_Text txt_GetSelect;

    public TMP_Text txt_SelectDown;
    public TMP_Text txt_BackDown;
    public TMP_Text txt_HomeDown;
    public TMP_Text txt_FunctionDown;
    public TMP_Text txt_VoiceDown;

    public TMP_Text txt_SelectUp;
    public TMP_Text txt_BackUp;
    public TMP_Text txt_HomeUp;
    public TMP_Text txt_FunctionUp;
    public TMP_Text txt_VoiceUp;

    private float swipeLeftVal = 0;
    private float swipeRightVal = 0;
    private float swipeUpVal = 0;
    private float swipeDownVal = 0;

    void Start()
    {
        
    }

    void Update()
    {
        txt_swipeLeftBool.text = JMRInteraction.GetSwipeLeft(out swipeLeftVal).ToString();
        txt_swipeRightBool.text = JMRInteraction.GetSwipeRight(out swipeRightVal).ToString();
        txt_swipeUpBool.text = JMRInteraction.GetSwipeUp(out swipeUpVal).ToString();
        txt_swipeDownBool.text = JMRInteraction.GetSwipeDown(out swipeDownVal).ToString();

        txt_swipeLeftVal.text = swipeLeftVal.ToString();
        txt_swipeRightVal.text = swipeRightVal.ToString();
        txt_swipeUpVal.text = swipeUpVal.ToString();
        txt_swipeDownVal.text = swipeDownVal.ToString();

        txt_isTouching.text = JMRInteraction.IsTouching().ToString();
        txt_touchVector.text = JMRInteraction.GetTouch().ToString();

        txt_GetSelect.text = JMRInteraction.GetSelect().ToString();

        txt_SelectDown.text = JMRInteraction.GetSourceDown(JMRInteractionSourceInfo.Select).ToString();
        txt_BackDown.text = JMRInteraction.GetSourceDown(JMRInteractionSourceInfo.Back).ToString();
        txt_HomeDown.text = JMRInteraction.GetSourceDown(JMRInteractionSourceInfo.Home).ToString();
        txt_FunctionDown.text = JMRInteraction.GetSourceDown(JMRInteractionSourceInfo.Function).ToString();
        txt_VoiceDown.text = JMRInteraction.GetSourceDown(JMRInteractionSourceInfo.Voice).ToString();

        txt_SelectUp.text = JMRInteraction.GetSourceUp(JMRInteractionSourceInfo.Select).ToString();
        txt_BackUp.text = JMRInteraction.GetSourceUp(JMRInteractionSourceInfo.Back).ToString();
        txt_HomeUp.text = JMRInteraction.GetSourceUp(JMRInteractionSourceInfo.Home).ToString();
        txt_FunctionUp.text = JMRInteraction.GetSourceUp(JMRInteractionSourceInfo.Function).ToString();
        txt_VoiceUp.text = JMRInteraction.GetSourceUp(JMRInteractionSourceInfo.Voice).ToString();
    }
}
