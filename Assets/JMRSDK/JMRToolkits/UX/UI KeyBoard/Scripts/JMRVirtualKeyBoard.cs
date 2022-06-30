// Copyright (c) 2020 JioGlass. All Rights Reserved.

using UnityEngine;
using Tesseract.Utility;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Animations;
using JMRSDK.InputModule;
using System;
using JMRSDK.Utilities;

namespace JMRSDK.Toolkit.UI
{
    public delegate void DMessageHandler(string command);

    public class JMRVirtualKeyBoard : JMRPersistance<JMRVirtualKeyBoard>, IMessageHandler, ISelectClickHandler
    {
        #region SERIALIZED FIELDS
        [SerializeField]
        private Animator anim;
        [SerializeField]
        private TextMeshProUGUI suggestedText;
        [SerializeField]
        private GameObject alphabets, special_characters, alphabetsText, special_charactersText;
        [SerializeField]
        private UnityEngine.UI.Button searchBtn;
        [SerializeField]
        private JMRUIPrimaryButton clearBtn;
        [SerializeField]
        private LookAtConstraint lookAt;
        [SerializeField]
        private GameObject Keyboard, VirtualKeyboardNotification;
        #endregion

        #region PRIVATE FIELD
        private DMessageHandler j_handler;
        private bool isUpperCase, isTempUpper;
        private float j_counter, j_doubleTapDelay = 0.25f;
        private string j_prevButton;
        private IKeyboardInput j_input;
        private Transform j_ThisTransform;
        private ConstraintSource j_LookAtSource = new ConstraintSource() { weight = 1 };
        private bool isCleared = false;
        private bool showInputfieldTxt = false;
        private bool isKeyBoardOpenedFirstTime = true;
        private bool isAllCaps = false;
        #endregion

        #region PUBLIC FIELDS
        public bool isShown;
        public event Action OnHideKeyboard;
        #endregion

        #region Static Actions
        public static Action<string> OnKeyPressed = null;
        #endregion

        #region MONO METHODS

        public IKeyboardInput GetCurrentInput()
        {
            return j_input;
        }

        private void OnEnable()
        {
            if (!j_ThisTransform)
                j_ThisTransform = transform;
            JMRInputManager.Instance.AddGlobalListener(gameObject);
            clearBtn.OnClick.AddListener(OnClearButtonClicked);
        }

        private void OnDisable()
        {
            if (JMRInputManager.Instance != null && gameObject)
            {
                JMRInputManager.Instance.RemoveGlobalListener(gameObject);
            }
            clearBtn.OnClick.RemoveAllListeners();
        }

        private string prevText = "";
        [SerializeField]
        private int maxTexEllipsis = 25;

        private void Update()
        {
            if (j_counter <= j_doubleTapDelay)
            {
                j_counter += Time.deltaTime;
            }

            string suggestText = "";
            ellipsistext = "";
            if (j_input != null)
            {
                if(isCleared)
                {
                    suggestText = "";
                    isCleared = false;
                }
                suggestText = j_input.Text;
                int textLength = suggestText.Length;
                if (textLength >= (maxTexEllipsis + 3))
                {
                    suggestText = "..." + suggestText.Substring(textLength - (maxTexEllipsis + 1), maxTexEllipsis + 1);
                }
            }
            prevText = j_input.Text;
            suggestedText.text = suggestText;

            if (j_prevButton == "NEWLINE" && j_input != null)
            {
                HandleMultiLine(j_input);
            }
            clearBtn.gameObject.SetActive(suggestedText.text.Length > 0);
        }



        private void HandleMultiLine(IKeyboardInput input)
        {
            if (input.isMultiLineSupported())
            {
                cachedTex = j_input.Text;
                suggestedText.text = "";
            }
            else
            {
                HideKeyBoard();
              
            }
            j_prevButton = "";
        }

        #endregion
        #region PRIVATE METHODS

        private void OnClearButtonClicked()
        {
            suggestedText.text = cachedTex = prevText = j_input.Text = "";
            isCleared = true;
        }
        #endregion
        #region PUBLIC METHODS

        /// <summary>
        /// Show Keyboard.
        /// </summary>
        /// <param name="j_InputField"></param>
        /// 
        private string cachedTex = "";

        public void OnSelectClicked(SelectClickEventData eventData)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
           
            if (eventData.selectedObject == null || eventData.selectedObject.transform.root.GetComponent<JMRVirtualKeyBoard>() == null)
            {
              
                if (eventData.selectedObject == null || (eventData.selectedObject.tag != "Search" && (eventData.selectedObject.GetComponent<IKeyboardInput>() == null || eventData.selectedObject.GetComponent<IKeyboardInput>() != j_input)) && (eventData.selectedObject.tag != "Clear" && (eventData.selectedObject.GetComponent<IKeyboardInput>() == null || eventData.selectedObject.GetComponent<IKeyboardInput>() != j_input)))
                {
                        HideKeyBoard();
                }
            }
        }
        private void EnableKeyBoard(IKeyboardInput j_InputField)
        {
            if (j_InputField.isVirtualKeyBoard())
            {
                Keyboard.SetActive(false);
                VirtualKeyboardNotification.SetActive(true);
            }
            else
            {

                Keyboard.SetActive(true);
                VirtualKeyboardNotification.SetActive(false);
            }
        }
        public void ShowKeyBoard(IKeyboardInput j_InputField)
        {
            
            if (j_InputField == j_input)
            {
              
                return;
            }
          
            if (!isListeningForVoiceCommand)
            {
                JMRVoiceManager.OnSpeechResults += OnSpeechResult;
                JMRVoiceManager.OnSpeechError += OnSpeechError;
                JMRVoiceManager.OnSpeechCancelled += OnSpeechCancelled;
            }
            
            if (!gameObject.activeInHierarchy)
            {
         
                gameObject.SetActive(true);
                EnableKeyBoard(j_InputField);
            }
            else
            {
                EnableKeyBoard(j_InputField);
            }
            


            if (j_InputField != null && j_InputField != j_input)
            {
                if (j_input != null)
                {
                    Debug.Log("VK==> Calling ONDESELCT 7777777777777777777777777777777777777 ");
                    j_input.OnDeselect();
                }

                MonoBehaviour inputField = (MonoBehaviour)j_InputField;
                if (j_InputField.j_KeyboardPosition != null)
                {
                    if(j_InputField.isVirtualKeyBoard())
                    {
                       
                        transform.position = j_InputField.v_KeyboardPosition.position;
                        transform.rotation = j_InputField.v_KeyboardPosition.rotation;
                      
                    }
                    else
                    {
                        transform.position = j_InputField.j_KeyboardPosition.position;
                        transform.rotation = j_InputField.j_KeyboardPosition.rotation;
                    }
                    
                }
                else
                {
                   
                        
                        if (inputField.transform.position.y >= 0)
                        {

                            transform.position = inputField.transform.position + Vector3.down * 0.4f;
                            transform.rotation = inputField.transform.rotation;
                        }
                        else
                        {
                            transform.position = inputField.transform.position + Vector3.up * 0.4f;
                            transform.rotation = inputField.transform.rotation;
                        }
                    
                   
                }

                this.j_input = j_InputField;
                cachedTex = string.IsNullOrEmpty(j_input.Text) ? "" : j_input.Text;
                if (string.IsNullOrEmpty(cachedTex))
                {
                    StartCoroutine(WaitTillEOF());
                }
                prevText = "";
                isShown = true;
                showInputfieldTxt = true;
            }
        }
        public void ResetPosition(MonoBehaviour inputField)
        {
            Debug.LogError(inputField.GetComponent<RectTransform>().rect.width);
        }
        /// <summary>
        /// Hide Keyboard.
        /// </summary>sdf
        public void HideKeyBoard(bool hideWitoutNotify = false,
            bool hideWithoutDeselect = false,
            bool notCheckIsKeyboarActive = false)
        {
            if (!notCheckIsKeyboarActive && !gameObject.activeSelf)
                return;

            // anim.SetTrigger("Hide");
            if (j_input != null && !hideWitoutNotify)
            {
                Constants.SearchString = j_input.Text;
                j_input.EditEnd();
                if (!hideWithoutDeselect)
                {
                    Debug.Log("VK==> Calling ONDESELCT 888888888888888888888888888888 ");
                    j_input.OnDeselect();
                }
            }
            else if (voiceCommandField != null && !hideWitoutNotify)
            {
                Constants.SearchString = voiceCommandField.Text;
                voiceCommandField.EditEnd();
                if (!hideWithoutDeselect)
                {
                    Debug.Log("VK==> Calling ONDESELCT 9999999999999999999999999999999999999999 ");
                    voiceCommandField.OnDeselect();
                }
            }
            else if (hideWitoutNotify)
            {
                if (!hideWithoutDeselect)
                {
                    Debug.Log("VK==> Calling ONDESELCT 1001010101010101001001010101111111111111 ");
                    j_input.OnDeselect();
                }
            }

            if (!isListeningForVoiceCommand)
            {
                JMRVoiceManager.OnSpeechResults -= OnSpeechResult;
                JMRVoiceManager.OnSpeechError -= OnSpeechError;
                JMRVoiceManager.OnSpeechCancelled -= OnSpeechCancelled;
            }
            isShown = false;
            if (!hideWitoutNotify && !hideWithoutDeselect)
            {
                this.j_input = null;
            }
            else if(hideWitoutNotify)
            {
                this.j_input = null;
            }
            special_characters.SetActive(false);
            alphabets.gameObject.SetActive(true);
            if (special_charactersText != null)
            {
                special_charactersText.SetActive(false);
            }
            if (alphabetsText != null)
            {
                alphabetsText.gameObject.SetActive(true);
            }
            gameObject.SetActive(false);
            OnHideKeyboard?.Invoke();
        }
        

        /// <summary>
        /// Register New Key.
        /// </summary>
        /// <param name="key"></param>
        public void RegisterKey(IMessageHandler Key)
        {
            this.j_handler += Key.HandleMessage;
        }
        private string ellipsistext = string.Empty;
        /// <summary>
        /// Handle Virtual Keyboard Actions.
        /// </summary>
        /// <param name="command"></param>
        public void HandleMessage(string command)
        {
            if(!isUpperCase)
                j_counter =0;

            switch (command)
            {
                case Constants.CASE_TAP:
                    if (!isKeyBoardOpenedFirstTime)
                    {
                        if (j_prevButton == command && j_counter < j_doubleTapDelay)
                        {
                            isAllCaps = true;
                            j_counter = j_doubleTapDelay + 1;
                            isUpperCase = isTempUpper = true;
                        }
                        else
                        {
                            if (isUpperCase)
                            {
                                isUpperCase = isTempUpper = false;
                                isAllCaps = false;
                            }
                            else
                            {
                                isTempUpper = !isTempUpper;
                            }
                        }
                    }
                    else
                    {
                        if(isKeyBoardOpenedFirstTime)
                        {
                            isUpperCase = isTempUpper = true;
                        }
                        else
                        {
                            isUpperCase = !isUpperCase;
                            isTempUpper = !isTempUpper;
                        }
                        isKeyBoardOpenedFirstTime = false;
                    }

                    string cntrl = isTempUpper ? Constants.CASE_UPPER : Constants.CASE_LOWER;
                    
                    if (j_handler != null)
                        j_handler(cntrl);
                    break;
                case Constants.BACK_SPACE:
                    if (j_input == null || j_input.Text.Length <= 0)
                        break;
                    if (j_prevButton == command && j_counter < j_doubleTapDelay)
                    {
                        j_input.Text = "";
                        suggestedText.text = cachedTex = "";
                        ellipsistext = "";
                    }
                    else
                    {
                        j_input.Text = j_input.Text.Substring(0, j_input.Text.Length - 1);
                        if (j_input.Text.Length < cachedTex.Length)
                            cachedTex = j_input.Text;// j_input.Text.Substring(cachedTex.Length, j_input.Text.Length - cachedTex.Length);
                    }
                    break;
                case Constants.ALPHABETS:
                    if (alphabets.activeInHierarchy)
                        break;
                    special_characters.SetActive(false);
                    alphabets.gameObject.SetActive(true);
                    special_charactersText.SetActive(false);
                    alphabetsText.gameObject.SetActive(true); 
                    break;
                case Constants.SPECIAL_CHARACTERS:
                    if (special_characters.activeInHierarchy)
                        break;
                    alphabets.gameObject.SetActive(false);
                    special_characters.SetActive(true);
                    if (special_charactersText != null)
                    {
                        special_charactersText.SetActive(true);
                    }
                    if (alphabetsText != null)
                    {
                        alphabetsText.gameObject.SetActive(false);
                    }
                    break;
                case Constants.ENTER:
                    j_input.HandleKeyboardEnterKey();
                    HideKeyBoard();
                   
                    break;
                case Constants.NEWLINE:
                    if (j_input.isMultiLineSupported())
                    {
                        j_input.Text += "\n";
                    }
                    break;
                case Constants.VOICECOMMAND:
                    if (j_input != null)
                    {
                        JMRVoiceManager.Instance.ShowVoiceToolkit();
                        voiceCommandField = j_input;
                        isListeningForVoiceCommand = true;
                        HideKeyBoard(true, true);
                      
                    }
                    break;
                default:
                    if (command == " " && (j_input == null))
                        break;
                    j_input.Text += command;
                    break;
            }

            if (j_prevButton != Constants.CASE_TAP)
                j_counter = 0;

            if (isTempUpper && Constants.CASE_TAP != command && !isAllCaps)
            {
                j_handler(Constants.CASE_LOWER);
                isUpperCase = isTempUpper = false;
            }
            if (command != Constants.CASE_TAP)
            {
                OnKeyPressed?.Invoke(command);
            }
            j_prevButton = command;
        }


        private IKeyboardInput voiceCommandField = null;
        private bool isListeningForVoiceCommand = false;
        private void OnSpeechError(string obj)
        {
            JMRVoiceManager.Instance.HideVoiceToolkit();
            if (isListeningForVoiceCommand && voiceCommandField != null)
            {
                isListeningForVoiceCommand = false;
                if (obj == "ERROR_NO_MATCH")
                {
                    HideKeyBoard();
                  
                }
                
                else
                    ShowKeyBoard(voiceCommandField);
            }
            voiceCommandField = null;
        }

        private void OnSpeechCancelled(string arg1, long arg2)
        {
            JMRVoiceManager.Instance.HideVoiceToolkit();
            if (isListeningForVoiceCommand && voiceCommandField != null)
            {
                isListeningForVoiceCommand = false;
                HideKeyBoard();
               
            }
            voiceCommandField = null;
        }

        private void OnSpeechResult(string command, long timestamp)
        {
            JMRVoiceManager.Instance.HideVoiceToolkit();
            if (isListeningForVoiceCommand && voiceCommandField != null)
            {
                prevText = voiceCommandField.Text = command;
                isListeningForVoiceCommand = false;
                HideKeyBoard(false, false, true);

            }
            voiceCommandField = null;
        }

        /// <summary>
        /// Handle Virtual Keyboard Actions.
        /// </summary>
        /// <param name="type",name="msg"></param>
        public void HandleMessage(string type, string msg)
        {
        }

        /// <summary>
        /// Handle On Search Action.
        /// </summary>
        /// <param name="listener"></param>
        public void OnSearch(UnityAction listener)
        {
        }

        #endregion        

        #region ENUMERATORS

        System.Collections.IEnumerator WaitTillEOF()
        {
            yield return new WaitForEndOfFrame();

            if (string.IsNullOrEmpty(j_input.Text))
            {
                isKeyBoardOpenedFirstTime = true;
                HandleMessage(Constants.CASE_TAP);
            }
            // HideKeyBoard();
            while (JMRCameraUtility.Main == null)
            {
                yield return null;
            }

            // Set the parameters for LookAtConstraint for the Keyboard to look at Camera.
            j_LookAtSource.sourceTransform = JMRCameraUtility.Main.transform;
             lookAt.SetSource(0, j_LookAtSource);
            lookAt.constraintActive = false;

            isUpperCase = isTempUpper = true;
            isAllCaps = false;
        }

        #endregion
    }

}
