using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JMRSDK;

public class JMRDemoAudioExample : MonoBehaviour
{
    public Dropdown modesDropDown;
    public Text currentModeText;

    private JMRAudioManager audioManager;
    private int[] modes;
    private bool enableLogs = true;

    private void Start()
    {
        audioManager = JMRAudioManager.Instance;
    }

    public void GetSupportedEqualizerModes()
    {
        Debug.Log("get supported equalizer called");
        if(audioManager == null)
        {
            if(enableLogs)
                Debug.Log("audio manager is null");
            return;
        }
        modes = audioManager.GetSupportedEqualizerModes();
        if(modes == null)
        {
            if(enableLogs)
                Debug.Log("modes are null");
            return;
        }

        List<string> ModeNames = new List<string>();
        

         if(enableLogs)
                Debug.Log("mode count " + modes.Length);

        for(int i = 0; i<modes.Length; i++)
        {
            if (enableLogs)
                Debug.Log("mode fetched " + modes[i]);
            if (i == audioManager.MODE_MOVIES)
            {
                ModeNames.Add("MODE_MOVIES");
            }
            else if (modes[i] == audioManager.MODE_MUSIC)
            {
                ModeNames.Add("MODE_MUSIC");
            }
            else if (modes[i] == audioManager.MODE_SPORTS)
            {
                ModeNames.Add("MODE_SPORTS");
            }
            else if (modes[i] == audioManager.MODE_PRIVACY_VOICECALL)
            {
                ModeNames.Add("MODE_PRIVACY_VOICECALL");
            }
            else if (modes[i] == audioManager.MODE_FLAT)
            {
                ModeNames.Add("MODE_FLAT");
            }
            else if (modes[i] == audioManager.MODE_SPOKEN_WORD)
            {
                ModeNames.Add("MODE_SPOKEN_WORD");
            }
            else if (modes[i] == audioManager.MODE_GAME)
            {
                ModeNames.Add("MODE_GAME");
            }
            else if (modes[i] == audioManager.MODE_BASS_BOOST_EFFECT)
            {
                ModeNames.Add("MODE_BASS_BOOST_EFFECT");
            }
            else if(modes[i] == audioManager.MODE_LOUDNESS_ENHANCER)
            {
                ModeNames.Add("MODE_LOUDNESS_ENHANCER");
            }
            else if (modes[i] == audioManager.MODE_ACOUSTIC)
            {
                ModeNames.Add("MODE_ACOUSTIC");
            }
            else if (modes[i] == audioManager.MODE_ELECTRONICS)
            {
                ModeNames.Add("MODE_ELECTRONICS");
            }
        }

        modesDropDown.options.Clear();
        modesDropDown.AddOptions(ModeNames);
    }

    public void SetEqualizerMode()
    {
        if (modesDropDown.options[modesDropDown.value] == null)
            return;

        if(enableLogs)
            print("AE : SETTING DATA " + modesDropDown.options[modesDropDown.value].text);

        switch (modesDropDown.options[modesDropDown.value].text)
        {
            case "MODE_MOVIES":
                audioManager.SetEqualizerMode(audioManager.MODE_MOVIES);
                break;
            case "MODE_MUSIC":
                audioManager.SetEqualizerMode(audioManager.MODE_MUSIC);
                break;
            case "MODE_SPORTS":
                audioManager.SetEqualizerMode(audioManager.MODE_SPORTS);
                break;
            case "MODE_PRIVACY_VOICECALL":
                audioManager.SetEqualizerMode(audioManager.MODE_PRIVACY_VOICECALL);
                break;
            case "MODE_FLAT":
                audioManager.SetEqualizerMode(audioManager.MODE_FLAT);
                break;
            case "MODE_SPOKEN_WORD":
                audioManager.SetEqualizerMode(audioManager.MODE_SPOKEN_WORD);
                break;
            case "MODE_GAME":
                audioManager.SetEqualizerMode(audioManager.MODE_GAME);
                break;
            case "MODE_BASS_BOOST_EFFECT":
                audioManager.SetEqualizerMode(audioManager.MODE_BASS_BOOST_EFFECT);
                break;
            case "MODE_LOUDNESS_ENHANCER":
                audioManager.SetEqualizerMode(audioManager.MODE_LOUDNESS_ENHANCER);
                break;
            case "MODE_ACOUSTIC":
                audioManager.SetEqualizerMode(audioManager.MODE_ACOUSTIC);
                break;
            case "MODE_ELECTRONICS":
                audioManager.SetEqualizerMode(audioManager.MODE_ELECTRONICS);
                break;
        }
    }

    public void GetCurrentEqualizerMode()
    {
        int Mode = audioManager.GetEqualizerMode();
        if (Mode == null)
            return;

        if(enableLogs)
            print("current mode fetched " + Mode);

        if (Mode == audioManager.MODE_MOVIES)
        {
            currentModeText.text = "MODE_MOVIES";
        }
        else if (Mode == audioManager.MODE_MUSIC)
        {
            currentModeText.text = "MODE_MUSIC";
        }
        else if (Mode == audioManager.MODE_SPORTS)
        {
            currentModeText.text = "MODE_SPORTS";
        }
        else if (Mode == audioManager.MODE_PRIVACY_VOICECALL)
        {
            currentModeText.text = "MODE_PRIVACY_VOICECALL";
        }
        else if (Mode == audioManager.MODE_FLAT)
        {
            currentModeText.text = "MODE_FLAT";
        }
        else if (Mode == audioManager.MODE_SPOKEN_WORD)
        {
            currentModeText.text = "MODE_SPOKEN_WORD";
        }
        else if (Mode == audioManager.MODE_GAME)
        {
            currentModeText.text = "MODE_GAME";
        }
        else if (Mode == audioManager.MODE_BASS_BOOST_EFFECT)
        {
            currentModeText.text = "MODE_BASS_BOOST_EFFECT";
        }
        else if (Mode == audioManager.MODE_LOUDNESS_ENHANCER)
        {
            currentModeText.text = "MODE_LOUDNESS_ENHANCER";
        }
        else if (Mode == audioManager.MODE_ACOUSTIC)
        {
            currentModeText.text = "MODE_ACOUSTIC";
        }
        else if (Mode == audioManager.MODE_ELECTRONICS)
        {
            currentModeText.text = "MODE_ELECTRONICS";
        }
    }
}
