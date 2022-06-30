using System.Collections.Generic;
using UnityEngine;
using JMRSDK.InputModule;
using JMRSDK;
using System;
using TMPro;

[Serializable]
public class InitResult
{
    public List<string> languages;
}
namespace JMRSDK
{
    internal class JMRDemoVoiceExample : MonoBehaviour, IVoiceHandler
    {
        public TMP_Text LogText;
        private string spEvents;
        private string spPartialResult;
        private string spError;
        private string spResult;
        private string spIntents;
        private string Hotkeyword;
        private string initResult;
        private string languageCode;
        private string spCancelReason;
        private string spEndTime;
        private DateTime speechReadyTime, speechResultTime, partialResultTime;
        private string speechIntentResultDelayTime, speechReadyTimeString, speechResultDelayTime, partialToFinalDelay, resultToIntentDelay;

        private void OnEnable()
        {
            JMRVoiceManager.OnInit += OnInit;
            JMRVoiceManager.OnSpeechEvent += SpeechEvent;
            JMRVoiceManager.OnSpeechError += SpeechError;
            JMRVoiceManager.OnSpeechPartialResults += SpeechPartialResult;
            JMRVoiceManager.OnSpeechIntents += SpeechIntents;
            JMRVoiceManager.OnSpeechResults += SpeachResult;
            JMRVoiceManager.OnSpeechCancelled += SpeechCancelled;
            JMRVoiceManager.OnSpeechSessionEnd += SpeechEnd;
            JMRVoiceManager.OnHotwordDetected += HotkeywordDetected;

        }

        private void OnDisable()
        {
            JMRVoiceManager.OnInit -= OnInit;
            JMRVoiceManager.OnSpeechEvent -= SpeechEvent;
            JMRVoiceManager.OnSpeechError -= SpeechError;
            JMRVoiceManager.OnSpeechPartialResults -= SpeechPartialResult;
            JMRVoiceManager.OnSpeechIntents -= SpeechIntents;
            JMRVoiceManager.OnSpeechResults -= SpeachResult;
            JMRVoiceManager.OnSpeechCancelled -= SpeechCancelled;
            JMRVoiceManager.OnSpeechSessionEnd -= SpeechEnd;
            JMRVoiceManager.OnHotwordDetected -= HotkeywordDetected;
        }

        private void OnInit(string obj)
        {
            Debug.Log("ON INIT : " + obj);
            initResult = obj;
            InitResult result = JsonUtility.FromJson<InitResult>(initResult);
            languageCode = result.languages[0];
            Debug.Log(languageCode);
        }

        private void SpeechError(string err)
        {
            spError = err;
        }

        private void HotkeywordDetected(int arg1, float arg2)
        {
            Hotkeyword = "HotKeyword " + arg1 + " " + arg2;
        }

        private void SpeachResult(string obj, long ts)
        {
            speechResultTime = DateTime.Now;
            partialToFinalDelay = DateTime.Now.Subtract(partialResultTime).TotalMilliseconds.ToString();
            speechResultDelayTime = DateTime.Now.Subtract(speechReadyTime).TotalMilliseconds.ToString();
            spResult = obj;
        }
        private void SpeechIntents(string obj, long ts)
        {
            resultToIntentDelay = DateTime.Now.Subtract(speechResultTime).TotalMilliseconds.ToString();
            speechIntentResultDelayTime = DateTime.Now.Subtract(speechReadyTime).TotalMilliseconds.ToString();
            spIntents = obj;
        }
        private void SpeechPartialResult(string obj, long ts)
        {
            partialResultTime = DateTime.Now;
            spPartialResult = obj;
        }

        private void SpeechCancelled(string reason, long ts)
        {
            spCancelReason = reason;
        }

        private void SpeechEnd(long ts)
        {
            spEndTime = ts.ToString();
        }

        private void SpeechEvent(JMRVoiceManager.SpeechEvent obj, long ts)
        {
            Debug.LogError("Speech Event : " + obj.ToString());
            spEvents = obj.ToString();
            if (obj.ToString() == "Beginning")
                spEvents = "EVENT_SPEECH_BEGINNING";
            else if (obj.ToString() == "Ready")
            {

                partialToFinalDelay= resultToIntentDelay = speechIntentResultDelayTime = speechReadyTimeString = speechResultDelayTime = "";
                DateTime baseDate = DateTime.Today;
                speechReadyTime = DateTime.Now;
                speechReadyTimeString = string.Format("{0:T}", DateTime.Now) + $", MilliSeconds : {DateTime.Now.Subtract(baseDate).TotalMilliseconds}";
                spEvents = "EVENT_SPEECH_READY";
            }
            else if (obj.ToString() == "End")
                spEvents = "EVENT_SPEECH_END";
        }

        public void OnVoiceAction()
        {
            spEvents = "SPEECH_ACTION_STARTING";
        }

        public void StartListening()
        {
            if (!Application.isEditor)
            {
                if (string.IsNullOrEmpty(languageCode))
                    JMRVoiceManager.Instance.StartListening();
                else
                    JMRVoiceManager.Instance.StartListening();
            }
            else
                LogText.text = "This cannot be tested in Editor, Please make android build to test this";
        }
        public void StopListening()
        {
            if (!Application.isEditor)
                JMRVoiceManager.Instance.StopListening();
            else
                LogText.text = "This cannot be tested in Editor, Please make android build to test this";
        }

        public void CancelListening()
        {
            if (!Application.isEditor)
                JMRVoiceManager.Instance.CancelListening();
            else
                LogText.text = "This cannot be tested in Editor, Please make android build to test this";
        }

        public void RetryInit()
        {
            if (!Application.isEditor)
                JMRVoiceManager.Instance.RetryInitialization();
            else
                LogText.text = "This cannot be tested in Editor, Please make android build to test this";
        }

        private void LateUpdate()
        {
            if (!Application.isEditor)
            {
                LogText.text = "Voice API \n "
                 + "Voice init:\t" + initResult + "\t Language Code:\t" + languageCode + "\n"
                 + "Voice isSpeechRecognizerAvailable:\t" + JMRVoiceManager.Instance.IsSpeechRecognizerAvailable + "\n"
                 + "Voice Events:\t" + spEvents + "\n"
                 + "Speech Ready Time:\t" + speechReadyTimeString + "\n"
                 + "Speech STT Delay:\t" + speechResultDelayTime + "\n"
                 + "Speech STT to STI Delay:\t" + speechIntentResultDelayTime + "\n"
                 + "Speech Partial to STT:\t" + partialToFinalDelay + "\n"
                 + "Speech STI Delay:\t" + resultToIntentDelay + "\n"
                 + "Speech EndTime:\t" + spEndTime + "\n"
                 + "Speech Cancelled : " + spCancelReason + "\n"
                 + "Voice Partial Result:\t" + spPartialResult + "\n"
                 + "Voice Result:\t" + spResult + "\n"
                 + "Voice Intents:\t" + spIntents + "\n"
                 + "Voice Error:\t" + spError + "\n"
                 + "Voice HotKeyword:\t" + Hotkeyword + "\n"
                 ;
            }
        }
    }
}
