// Copyright (c) 2020 JioGlass. All Rights Reserved.

using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using JMRSDK.InputModule;
using JMRSDK.Utilities;

namespace JMRSDK.Toolkit.UI
{
    /// <summary>
    /// Editable text input field.
    /// </summary>
    public class JMRUIPrimaryInputField : JMRBaseThemeAnimator, UnityEngine.EventSystems.ISelectHandler,
        IUpdateSelectedHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IPointerClickHandler,
        ISubmitHandler,
        IPointerDownHandler,
        ILayoutElement,
        IScrollHandler,
        ICanvasElement, IKeyboardInput
    {
        #region ENUMS
        // Setting the content type acts as a shortcut for setting a combination of InputType, CharacterValidation, LineType, and TouchScreenKeyboardType
        public enum ContentType
        {
            Standard,
            Autocorrected,
            IntegerNumber,
            DecimalNumber,
            Alphanumeric,
            Name,
            EmailAddress,
            Password,
            Pin,
            Custom
        }

        public enum InputType
        {
            Standard,
            AutoCorrect,
            Password,
        }

        public enum CharacterValidation
        {
            None,
            Digit,
            Integer,
            Decimal,
            Alphanumeric,
            Name,
            Regex,
            EmailAddress,
            CustomValidator
        }

        public enum LineType
        {
            SingleLine,
            MultiLineSubmit,
            MultiLineNewline
        }

        protected enum EditState
        {
            Continue,
            Finish
        }
        #endregion

        #region PRIVATE PROPERTIES
        static string j_Clipboard
        {
            get
            {
                return GUIUtility.systemCopyBuffer;
            }
            set
            {
                GUIUtility.systemCopyBuffer = value;
            }
        }
        private BaseInput j_InputSystem
        {
            get
            {
                if (EventSystem.current && EventSystem.current.currentInputModule)
                    return EventSystem.current.currentInputModule.input;
                return null;
            }
        }

        private string j_CompositionString
        {
            get { return j_InputSystem != null ? j_InputSystem.compositionString : UnityEngine.Input.compositionString; }
        }

        private bool j_KeyboardUsingEvents()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.tvOS:
                    return false;
                default:
                    return true;
            }
        }

        private bool Selected { get { return j_StringPositionInternal != j_StringSelectPositionInternal; } }
        #endregion

        #region PROTECTED PROPERTIES
        protected Mesh j_MeshProp
        {
            get
            {
                if (j_Mesh == null)
                    j_Mesh = new Mesh();
                return j_Mesh;
            }
        }

        /// <summary>
        /// Current position of the cursor.
        /// Getters are public Setters are protected
        /// </summary>
        protected int j_CaretPositionInternal { get { return j_CaretPosition + j_CompositionString.Length; } set { j_CaretPosition = value; ClampCaretPos(ref j_CaretPosition); } }
        protected int j_StringPositionInternal { get { return j_StringPosition + j_CompositionString.Length; } set { j_StringPosition = value; ClampStringPos(ref j_StringPosition); } }

        protected int j_CaretSelectPositionInternal { get { return j_CaretSelectPosition + j_CompositionString.Length; } set { j_CaretSelectPosition = value; ClampCaretPos(ref j_CaretSelectPosition); } }
        protected int j_StringSelectPositionInternal { get { return j_StringSelectPosition + j_CompositionString.Length; } set { j_StringSelectPosition = value; ClampStringPos(ref j_StringSelectPosition); } }
        
        private bool isFieldSelected;
        #endregion

        #region PUBLIC PROPERTIES


        /// <summary>
        /// See ILayoutElement.minWidth.
        /// </summary>
        public virtual float minWidth { get { return 0; } }

        /// <summary>
        /// Get the displayed with of all input characters.
        /// </summary>
        public virtual float preferredWidth
        {
            get
            {
                if (TextComponent == null)
                    return 0;

                return textComponent.preferredWidth + 16 + caretWidth + 1;
            }
        }

        /// <summary>
        /// See ILayoutElement.flexibleWidth.
        /// </summary>
        public virtual float flexibleWidth { get { return -1; } }

        /// <summary>
        /// See ILayoutElement.minHeight.
        /// </summary>
        public virtual float minHeight { get { return 0; } }

        /// <summary>
        /// Get the height of all the text if constrained to the height of the RectTransform.
        /// </summary>
        public virtual float preferredHeight
        {
            get
            {
                if (TextComponent == null)
                    return 0;

                return textComponent.preferredHeight + 16;
            }
        }

        /// <summary>
        /// See ILayoutElement.flexibleHeight.
        /// </summary>
        public virtual float flexibleHeight { get { return -1; } }

        /// <summary>
        /// See ILayoutElement.layoutPriority.
        /// </summary>
        public virtual int layoutPriority { get { return 1; } }
        /// <summary>
        /// Should the mobile keyboard input be hidden.
        /// </summary>
        public bool HideMobileInput
        {
            get
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                    case RuntimePlatform.IPhonePlayer:
                    case RuntimePlatform.tvOS:
                        return hideMobileInput;
                    default:
                        return true;
                }
            }

            set
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                    case RuntimePlatform.IPhonePlayer:
                    case RuntimePlatform.tvOS:
                        SetPropertyUtility.SetStruct(ref hideMobileInput, value);
                        break;
                    default:
                        hideMobileInput = true;
                        break;
                }
            }
        }

        public bool HideSoftKeyboard
        {
            get
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                    case RuntimePlatform.IPhonePlayer:
                    case RuntimePlatform.tvOS:
                    case RuntimePlatform.WSAPlayerX86:
                    case RuntimePlatform.WSAPlayerX64:
                    case RuntimePlatform.WSAPlayerARM:
                        return hideSoftKeyboard;
                    default:
                        return true;
                }
            }

            set
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                    case RuntimePlatform.IPhonePlayer:
                    case RuntimePlatform.tvOS:
                    case RuntimePlatform.WSAPlayerX86:
                    case RuntimePlatform.WSAPlayerX64:
                    case RuntimePlatform.WSAPlayerARM:
                        SetPropertyUtility.SetStruct(ref hideSoftKeyboard, value);
                        break;
                    default:
                        hideSoftKeyboard = true;
                        break;
                }

                if (hideSoftKeyboard == true && j_SoftKeyboard != null && TouchScreenKeyboard.isSupported && j_SoftKeyboard.active)
                {
                    j_SoftKeyboard.active = false;
                    j_SoftKeyboard = null;
                }
            }
        }

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                SetText(value);
            }
        }

        public bool Focused
        {
            get { return j_AllowInput; }
        }

        public float CaretBlinkRate
        {
            get { return caretBlinkRate; }
            set
            {
                if (SetPropertyUtility.SetStruct(ref caretBlinkRate, value))
                {
                    if (j_AllowInput)
                        SetCaretActive();
                }
            }
        }

        public int CaretWidth { get { return caretWidth; } set { if (SetPropertyUtility.SetStruct(ref caretWidth, value)) MarkGeometryAsDirty(); } }

        public RectTransform TextViewport { get { return textViewport; } set { SetPropertyUtility.SetClass(ref textViewport, value); } }

        public TMP_Text TextComponent
        {
            get { return textComponent; }
            set
            {
                if (SetPropertyUtility.SetClass(ref textComponent, value))
                {
                    SetTextComponentWrapMode();
                }
            }
        }

        //public TMP_Text placeholderTextComponent { get { return m_PlaceholderTextComponent; } set { SetPropertyUtility.SetClass(ref m_PlaceholderTextComponent, value); } }

        public Graphic Placeholder { get { return placeholder; } set { SetPropertyUtility.SetClass(ref placeholder, value); } }

        public Scrollbar VerticalScrollbar
        {
            get { return verticalScrollbar; }
            set
            {
                if (verticalScrollbar != null)
                    verticalScrollbar.onValueChanged.RemoveListener(OnScrollbarValueChange);

                SetPropertyUtility.SetClass(ref verticalScrollbar, value);

                if (verticalScrollbar)
                {
                    verticalScrollbar.onValueChanged.AddListener(OnScrollbarValueChange);

                }
            }
        }

        public float ScrollSensitivity { get { return scrollSensitivity; } set { if (SetPropertyUtility.SetStruct(ref scrollSensitivity, value)) MarkGeometryAsDirty(); } }

        public Color CaretColor { get { return CustomCaretColor ? caretColor : TextComponent.color; } set { if (SetPropertyUtility.SetColor(ref caretColor, value)) MarkGeometryAsDirty(); } }

        public bool CustomCaretColor { get { return customCaretColor; } set { if (customCaretColor != value) { customCaretColor = value; MarkGeometryAsDirty(); } } }

        public Color SelectionColor { get { return selectionColor; } set { if (SetPropertyUtility.SetColor(ref selectionColor, value)) MarkGeometryAsDirty(); } }


        private TouchScreenKeyboardEvent OnTouchScreenKeyboardStatusChanged { get { return touchScreenKeyboardStatusChanged; } set { SetPropertyUtility.SetClass(ref touchScreenKeyboardStatusChanged, value); } }
        public OnValidateInput ValidateInput { get { return validateInput; } set { SetPropertyUtility.SetClass(ref validateInput, value); } }
        public int CharacterLimit
        {
            get { return characterLimit; }
            set
            {
                if (SetPropertyUtility.SetStruct(ref characterLimit, Math.Max(0, value)))
                {
                    UpdateLabel();
                    if (j_SoftKeyboard != null)
                        j_SoftKeyboard.characterLimit = value;
                }
            }
        }

        //public bool isInteractableControl { set { if ( } }

        /// <summary>
        /// Set the point size on both Placeholder and Input text object.
        /// </summary>
        public float PointSize
        {
            get { return globalPointSize; }
            set
            {
                if (SetPropertyUtility.SetStruct(ref globalPointSize, Math.Max(0, value)))
                {
                    SetGlobalPointSize(globalPointSize);
                    UpdateLabel();
                }
            }
        }

        /// <summary>
        /// Sets the Font Asset on both Placeholder and Input child objects.
        /// </summary>
        public TMP_FontAsset FontAsset
        {
            get { return globalFontAsset; }
            set
            {
                if (SetPropertyUtility.SetClass(ref globalFontAsset, value))
                {
                    SetGlobalFontAsset(globalFontAsset);
                    UpdateLabel();
                }
            }
        }

        public bool OnFocusSelectAll
        {
            get { return focusSelectAll; }
            set { focusSelectAll = value; }
        }

        /// <summary>
        /// Determines if the text and caret position as well as selection will be reset when the input field is deactivated.
        /// </summary>
        public bool ResetOnDeActivation
        {
            get { return resetOnDeActivation; }
            set { resetOnDeActivation = value; }
        }

        /// <summary>
        /// Controls whether the original text is restored when pressing "ESC".
        /// </summary>
        public bool RestoreOriginalTextOnEscape
        {
            get { return restoreOriginalTextOnEscape; }
            set { restoreOriginalTextOnEscape = value; }
        }

        /// <summary>
        /// Is Rich Text editing allowed?
        /// </summary>
        public bool RichTextEditingAllowed
        {
            get { return richTextEditingAllowed; }
            set { richTextEditingAllowed = value; }
        }

        public LineType LineTypeProp
        {
            get { return lineType; }
            set
            {
                if (SetPropertyUtility.SetStruct(ref lineType, value))
                {
                    SetToCustomIfContentTypeIsNot(ContentType.Standard, ContentType.Autocorrected);
                    SetTextComponentWrapMode();
                }
            }
        }

        /// <summary>
        /// Limits the number of lines of text in the Input Field.
        /// </summary>
        public int LineLimit
        {
            get { return lineLimit; }
            set
            {
                if (lineType == LineType.SingleLine)
                    lineLimit = 1;
                else
                    SetPropertyUtility.SetStruct(ref lineLimit, value);

            }
        }

        // Content Type related
        public ContentType ContentTypeProp { get { return contentType; } set { if (SetPropertyUtility.SetStruct(ref contentType, value)) EnforceContentType(); } }

        public InputType InputTypeProp { get { return inputType; } set { if (SetPropertyUtility.SetStruct(ref inputType, value)) SetToCustom(); } }

        public TouchScreenKeyboardType KeyboardType
        {
            get { return keyboardType; }
            set
            {
                if (SetPropertyUtility.SetStruct(ref keyboardType, value))
                    SetToCustom();
            }
        }

        public CharacterValidation CharacterValidationProp { get { return characterValidation; } set { if (SetPropertyUtility.SetStruct(ref characterValidation, value)) SetToCustom(); } }

        /// <summary>
        /// Sets the Input Validation to use a Custom Input Validation script.
        /// </summary>
        public TMP_InputValidator InputValidator
        {
            get { return inputValidator; }
            set { if (SetPropertyUtility.SetClass(ref inputValidator, value)) SetToCustom(CharacterValidation.CustomValidator); }
        }

        public bool ReadOnly { get { return readOnly; } set { readOnly = value; } }

        public bool RichText { get { return richText; } set { richText = value; SetTextComponentRichTextMode(); } }

        // Derived property
        public bool MultiLine { get { return lineType == LineType.MultiLineNewline || LineTypeProp == LineType.MultiLineSubmit; } }
        // Not shown in Inspector.
        public char AsteriskChar { get { return asteriskChar; } set { if (SetPropertyUtility.SetStruct(ref asteriskChar, value)) UpdateLabel(); } }
        public bool Canceled { get { return j_WasCanceled; } }

        /// <summary>
        /// Get: Returns the focus position as thats the position that moves around even during selection.
        /// Set: Set both the anchor and focus position such that a selection doesn't happen
        /// </summary>
        public int CaretPosition
        {
            get { return j_CaretSelectPositionInternal; }
            set { SelectionAnchorPosition = value; SelectionFocusPosition = value; j_StringPositionDirty = true; }
        }

        /// <summary>
        /// Get: Returns the fixed position of selection
        /// Set: If compositionString is 0 set the fixed position
        /// </summary>
        public int SelectionAnchorPosition
        {
            get
            {
                return j_CaretPositionInternal;
            }

            set
            {
                if (j_CompositionString.Length != 0)
                    return;

                j_CaretPositionInternal = value;
                j_StringPositionDirty = true;
            }
        }

        /// <summary>
        /// Get: Returns the variable position of selection
        /// Set: If compositionString is 0 set the variable position
        /// </summary>
        public int SelectionFocusPosition
        {
            get
            {
                return j_CaretSelectPositionInternal;
            }
            set
            {
                if (j_CompositionString.Length != 0)
                    return;

                j_CaretSelectPositionInternal = value;
                j_StringPositionDirty = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int StringPosition
        {
            get { return j_StringSelectPositionInternal; }
            set { SelectionStringAnchorPosition = value; SelectionStringFocusPosition = value; j_CaretPositionDirty = true; }
        }

        /// <summary>
        /// The fixed position of the selection in the raw string which may contains rich text.
        /// </summary>
        public int SelectionStringAnchorPosition
        {
            get
            {
                return j_StringPositionInternal;
            }

            set
            {
                if (j_CompositionString.Length != 0)
                    return;

                j_StringPositionInternal = value;
                j_CaretPositionDirty = true;
            }
        }

        /// <summary>
        /// The variable position of the selection in the raw string which may contains rich text.
        /// </summary>
        public int SelectionStringFocusPosition
        {
            get
            {
                return j_StringSelectPositionInternal;
            }
            set
            {
                if (j_CompositionString.Length != 0)
                    return;

                j_StringSelectPositionInternal = value;
                j_CaretPositionDirty = true;
            }
        }


        #endregion

        #region PUBLIC FIELDS
        [SerializeField, Header("Keyboard Settings")]
        private Transform KeyboardSpawnPoint;
        [SerializeField]
        private Transform VirtualKeyboardSpawnPoint;
        public float KeyBoardPositionOffset = 0.045f;
        public bool IsError;
        public delegate char OnValidateInput(string text, int charIndex, char addedChar);
        #endregion

        #region SERIALIZED FIELDS
        [SerializeField]
        protected RectTransform textViewport;

        [SerializeField]
        protected TMP_Text textComponent;

        [SerializeField]
        protected Graphic placeholder;
        [SerializeField]
        private JMRUIPrimaryButton clearButton;
        public bool supportMultiLine = false;

        //[SerializeField]
        protected Scrollbar verticalScrollbar;
       // [SerializeField]
      //  private GameObject VirtualKeyboardNote;
        //[SerializeField]
        protected TMP_ScrollbarEventHandler verticalScrollbarEventHandler;
        public Transform j_KeyboardPosition { get => KeyboardSpawnPoint; set => KeyboardSpawnPoint = value; }
        public Transform v_KeyboardPosition { get => VirtualKeyboardSpawnPoint; set => VirtualKeyboardSpawnPoint = value; }
        public SubmitEvent Submit { get; set; }
        /// <summary>
        /// Event delegates triggered when the input field submits its data.
        /// </summary>
        [SerializeField]
        private SubmitEvent submit;

        public OnChangeEvent ValueChanged { get; set; }
        /// <summary>
        /// Event delegates triggered when the input field changes its data.
        /// </summary>
        [SerializeField]
        private OnChangeEvent valueChanged;

        /// <summary>
        /// 
        /// </summary>
        //[SerializeField]
        protected float scrollSensitivity = 1.0f;

        //[SerializeField]
        //protected TMP_Text m_PlaceholderTextComponent;

        //[SerializeField]
        private ContentType contentType = ContentType.Standard;

        /// <summary>
        /// Type of data expected by the input field.
        /// </summary>
        //[SerializeField]
        private InputType inputType = InputType.Standard;

        /// <summary>
        /// The character used to hide text in password field.
        /// </summary>
        //[SerializeField]
        private char asteriskChar = '*';

        /// <summary>
        /// Keyboard type applies to mobile keyboards that get shown.
        /// </summary>
        //[SerializeField]
        private TouchScreenKeyboardType keyboardType = TouchScreenKeyboardType.Default;

        //[SerializeField]
        private LineType lineType = LineType.MultiLineNewline;

        /// <summary>
        /// Should hide mobile input field part of the virtual keyboard.
        /// </summary>
        //[SerializeField]
        private bool hideMobileInput = true;

        /// <summary>
        /// Should hide soft / virtual keyboard.
        /// </summary>
        //[SerializeField]
        private bool hideSoftKeyboard = true;

        /// <summary>
        /// What kind of validation to use with the input field's data.
        /// </summary>
        //[SerializeField]
        private CharacterValidation characterValidation = CharacterValidation.None;

        /// <summary>
        /// The Regex expression used for validating the text input.
        /// </summary>
        //[SerializeField]
        private string regexValue = string.Empty;

        /// <summary>
        /// The point sized used by the placeholder and input text object.
        /// </summary>
        //[SerializeField]
        private float globalPointSize = 14;

        /// <summary>
        /// Maximum number of characters allowed before input no longer works.
        /// </summary>
        //[SerializeField]
        private int characterLimit = 0;

        /// <summary>
        /// Event delegates triggered when the input field submits its data.
        /// </summary>

        //[SerializeField]
        private Color caretColor = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);

        //[SerializeField]
        private bool customCaretColor = false;

        //[SerializeField]
        private Color selectionColor = new Color(168f / 255f, 206f / 255f, 255f / 255f, 192f / 255f);

        /// <summary>
        /// Text Text used to display the input's value.
        /// </summary>
        //[SerializeField]
        //[TextArea(5, 10)]
        protected string text = string.Empty;
        string IKeyboardInput.Text { get => text; set => SetText(value); }

        //[SerializeField]
        //[Range(0f, 4f)]
        private float caretBlinkRate = 0.85f;

        //[SerializeField]
        //[Range(1, 5)]
        private int caretWidth = 1;

        //[SerializeField]
        private bool readOnly = false;

        //[SerializeField]
        private bool richText = true;

        //[SerializeField]
        protected TMP_FontAsset globalFontAsset;

        /// <summary>
        /// Determines if the whole text will be selected when focused.
        /// </summary>

        //[SerializeField]
        protected bool focusSelectAll = true;

        //[SerializeField]
        protected bool resetOnDeActivation = true;

        //[SerializeField]
        private bool restoreOriginalTextOnEscape = true;

        //[SerializeField]
        protected bool richTextEditingAllowed = false;

        //[SerializeField]
        protected int lineLimit = 0;

        //[SerializeField]
        protected TMP_InputValidator inputValidator = null;
        #endregion

        #region PROTECTED FIELDS
        protected TouchScreenKeyboard j_SoftKeyboard;
        protected int j_StringPosition = 0;
        protected int j_StringSelectPosition = 0;
        protected int j_CaretPosition = 0;
        protected int j_CaretSelectPosition = 0;
        protected UIVertex[] j_CursorVerts = null;
        [NonSerialized]
        protected Mesh j_Mesh;
        protected bool j_CaretVisible;
        protected RectTransform j_TextComponentRectTransform;
        protected bool j_SelectAll;
        private bool j_SelectionStillActive = false;
        private bool j_ReleaseSelection = false;
        #endregion

        #region SERIALIZED EVENTS
        [Serializable]
        public class SubmitEvent : UnityEvent<string> { }

        [Serializable]
        public class OnChangeEvent : UnityEvent<string> { }

        //[Serializable]
        public class SelectionEvent : UnityEvent<string> { }

        //[Serializable]
        public class TextSelectionEvent : UnityEvent<string, int, int> { }

        //[Serializable]
        public class TouchScreenKeyboardEvent : UnityEvent<TouchScreenKeyboard.Status> { }

        //[SerializeField]
        private SubmitEvent endEdit = new SubmitEvent();


        /// <summary>
        /// Event delegates triggered when the input field is focused.
        /// </summary>
        //[SerializeField]
        private SelectionEvent select = new SelectionEvent();

        /// <summary>
        /// Event delegates triggered when the input field focus is lost.
        /// </summary>
       //[SerializeField]
        private SelectionEvent deselect = new SelectionEvent();

        /// <summary>
        /// Event delegates triggered when the text is selected / highlighted.
        /// </summary>
        //[SerializeField]
        private TextSelectionEvent textSelection = new TextSelectionEvent();

        /// <summary>
        /// Event delegates triggered when text is no longer select / highlighted.
        /// </summary>
        //[SerializeField]
        private TextSelectionEvent endTextSelection = new TextSelectionEvent();


        /// <summary>
        /// Event delegates triggered when the status of the TouchScreenKeyboard changes.
        /// </summary>
        //[SerializeField]
        private TouchScreenKeyboardEvent touchScreenKeyboardStatusChanged = new TouchScreenKeyboardEvent();

        /// <summary>
        /// Custom validation callback.
        /// </summary>
        [SerializeField]
        private OnValidateInput validateInput;

        #endregion

        #region PRIVATE FIELDS
        private bool j_IsDrivenByLayoutComponents = false;
        /// <summary>
        /// Used to keep track of scroll position
        /// </summary>
        private float j_ScrollPosition;
        private RectTransform j_CaretRectTrans = null;
        private CanvasRenderer j_CachedInputRenderer;
        private Vector2 j_LastPosition;
        private bool j_AllowInput = false;
        private bool j_ShouldActivateNextUpdate = false;
        private bool j_UpdateDrag = false;
        private bool j_DragPositionOutOfBounds = false;
        private const float j_KHScrollSpeed = 0.05f;
        private const float j_KVScrollSpeed = 0.10f;
        private Coroutine j_BlinkCoroutine = null;
        private float j_BlinkStartTime = 0.0f;
        private Coroutine j_DragCoroutine = null;
        private string j_OriginalText = "";
        private bool j_WasCanceled = false;
        private bool j_HasDoneFocusTransition = false;
        private WaitForSecondsRealtime j_WaitForSecondsRealtime;
        private bool j_PreventCallback = false;
        private bool j_TouchKeyboardAllowsInPlaceEditing = false;
        private bool j_TextComponentUpdateRequired = false;
        private bool j_ScrollbarUpdateRequired = false;
        private bool j_UpdatingScrollbarValues = false;
        private bool j_LastKeyBackspace = false;
        private float j_PointerDownClickStartTime;
        private float j_KeyDownStartTime;
        private float j_DoubleClickDelay = 0.5f;
        private GameObject j_SelectedObject;
        private bool j_Selected;
        private bool j_StringPositionDirty;
        private bool j_CaretPositionDirty;
        private bool j_ForceRectTransformAdjustment;
        /// <summary>
        /// Handle the specified event.
        /// </summary>
        private Event j_ProcessingEvent = new Event();


        static private readonly char[] j_KSeparators = { ' ', '.', ',', '\t', '\r', '\n' };
        // Doesn't include dot and @ on purpose! See usage for details.
        const string j_KEmailSpecialCharacters = "!#$%&'*+-/=?^_`{|}~";

        #endregion

        #region CONSTRUCTOR
        protected JMRUIPrimaryInputField()
        {
            SetTextComponentWrapMode();
        }
        #endregion

        #region MONO METHODS

#if UNITY_EDITOR
        // Remember: This is NOT related to text validation!
        // This is Unity's own OnValidate method which is invoked when changing values in the Inspector.
        private void OnValidate()
        {
            // base.OnValidate();
            EnforceContentType();

            characterLimit = Math.Max(0, characterLimit);

            //This can be invoked before OnEnabled is called. So we shouldn't be accessing other objects, before OnEnable is called.
            if (!gameObject.activeSelf)
                return;

            SetTextComponentRichTextMode();

            UpdateLabel();
            if (j_AllowInput)
                SetCaretActive();
        }
#endif

        protected override void OnEnable()
        {
            //Debug.Log("*** OnEnable() *** - " + this.name);

            base.OnEnable();
            if(isVirtualKeyBoard())
            {
                clearButton.gameObject.SetActive(false);
            }
         //   onSelectionInputField += OnSelectionInputField;
            if (clearButton == null && GetComponentInChildren<JMRUIPrimaryButton>())
            {
                clearButton = GetComponentInChildren<JMRUIPrimaryButton>();
            }

            wasError = !IsError;
            if (Submit == null)
            {
                Submit = new SubmitEvent();
            }
            if (ValueChanged == null)
            {
                ValueChanged = new OnChangeEvent();
            }
            if (text == null)
                text = string.Empty;

            if (Application.isPlaying)
            {
                if (j_CachedInputRenderer == null && textComponent != null)
                {
                    // Check if Input Field is driven by any layout components
                    j_IsDrivenByLayoutComponents = GetComponent<ILayoutController>() != null ? true : false;

                    GameObject go = new GameObject(transform.name + " Input Caret", typeof(RectTransform));

                    // Add MaskableGraphic Component
                    TMP_SelectionCaret caret = go.AddComponent<TMP_SelectionCaret>();
                    caret.raycastTarget = false;
                    caret.color = Color.clear;

                    go.hideFlags = HideFlags.DontSave;
                    go.transform.SetParent(textComponent.transform.parent);
                    go.transform.SetAsFirstSibling();
                    go.layer = gameObject.layer;

                    j_CaretRectTrans = go.GetComponent<RectTransform>();
                    j_CachedInputRenderer = go.GetComponent<CanvasRenderer>();
                    j_CachedInputRenderer.SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);

                    // Needed as if any layout is present we want the caret to always be the same as the text area.
                    go.AddComponent<LayoutElement>().ignoreLayout = true;

                    AssignPositioningIfNeeded();
                }
            }

            // If we have a cached renderer then we had OnDisable called so just restore the material.
            if (j_CachedInputRenderer != null)
                j_CachedInputRenderer.SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);

            if (textComponent != null)
            {
                textComponent.RegisterDirtyVerticesCallback(MarkGeometryAsDirty);
                textComponent.RegisterDirtyVerticesCallback(UpdateLabel);
                //m_TextComponent.ignoreRectMaskCulling = multiLine;

                //m_DefaultTransformPosition = m_TextComponent.rectTransform.localPosition;

                // Cache reference to Vertical Scrollbar RectTransform and add listener.
                if (verticalScrollbar != null)
                {
                    //textComponent.ignoreRectMaskCulling = true;
                    verticalScrollbar.onValueChanged.AddListener(OnScrollbarValueChange);
                }

                UpdateLabel();
            }
            if (clearButton != null)
            {
                clearButton.OnClick.AddListener(onClearButtonClicked);
                Debug.Log("here2");
                if(!isVirtualKeyBoard())
                clearButton.gameObject.SetActive(textComponent.text.Length > 1);
            }
            // Subscribe to event fired when text object has been regenerated.
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
        }
        #region Virtual Keyboard
        private void SubscribeSoftKeyboardCallbacks()
        {
            JMRInteractionManager.onSoftInputDismissed += OnSoftKeyboardDismissedCallback;
            JMRInteractionManager.onSoftKeyboardTextInput += OnSoftKeyboardTextInput;
        }
        private void UnSubscribeSoftKeyboardCallbacks()
        {
            JMRInteractionManager.onSoftInputDismissed -= OnSoftKeyboardDismissedCallback;
            JMRInteractionManager.onSoftKeyboardTextInput -= OnSoftKeyboardTextInput;
        }
        private void OnSoftKeyboardDismissedCallback(int toSource, int deviceType)
        {
            MainThreadDispatcher.Execute(() => OnSoftKeyboardDismissed(toSource, deviceType));
        }
        public void OnSoftKeyboardTextInput(int source, string textValue)
        {
            MainThreadDispatcher.Execute(() => SoftKeyboardTextInput(textValue));
        }
        private void OnSoftKeyboardDismissed(int toSource, int deviceType)
        {
            JMRVirtualKeyBoard.Instance.HideKeyBoard();
            
        }
        public void SoftKeyboardTextInput(string textValue)
        {
            SetText(textValue);
        }
        public void RequestSoftInput()
        {
            JMRInteractionManager.Instance.requestSoftInput(GetContentType(), Text);
        }
        public void SetSoftKeyboardTextInput(string text)
        {
           JMRInteractionManager.Instance.setSoftKeyboardTextInput(text);
        }
        public int GetContentType()
        {
            int Value = 0;

            if (LineTypeProp == LineType.MultiLineSubmit)
            {
                Value = JMRInteractionManager.Instance.TYPE_TEXT_FLAG_MULTI_LINE;
                return Value;
            }
            switch (ContentTypeProp)
            {

                case ContentType.Standard:
                    Value = JMRInteractionManager.Instance.TYPE_CLASS_TEXT;

                    break;
                case ContentType.IntegerNumber:
                    Value = JMRInteractionManager.Instance.TYPE_CLASS_NUMBER;

                    break;
                case ContentType.Password:
                    Value = JMRInteractionManager.Instance.TYPE_TEXT_VARIATION_PASSWORD;
                    break;
                case ContentType.DecimalNumber:
                    Value = JMRInteractionManager.Instance.TYPE_NUMBER_FLAG_DECIMAL;
                    break;
                case ContentType.EmailAddress:
                    Value = JMRInteractionManager.Instance.TYPE_TEXT_VARIATION_EMAIL_ADDRESS;
                    break;

                default:
                    Value = JMRInteractionManager.Instance.TYPE_CLASS_TEXT;
                    break;
            }
            return Value;
        }
        private bool IsVirtualKeyBoardConncted()
        {
            if (!Application.isEditor)
            {
                return JMRInteractionManager.Instance.isDeviceConnected((int)JMRInteractionManager.InteractionDeviceType.VIRTUAL_CONTROLLER, 0);
            }
            return false;
        }
        
        #endregion
        public override void OnSelectClicked(SelectClickEventData eventData)
        {
          
            if (!isSelected)
            {
             
                if (IsVirtualKeyBoardConncted())
                {                   
                  
                    if (eventData.selectedObject == null || eventData.selectedObject.tag == "Clear")
                    {
                      
                        return;
                    }
                    else
                    {
                       
                        base.OnSelectClicked(eventData);
                        ActivateInputFieldInternal();
                        SubscribeSoftKeyboardCallbacks();
                        RequestSoftInput();
                    }
                }
                else
                {
                   
                    base.OnSelectClicked(eventData);
                    ActivateInputFieldInternal();
                }

                if(!isVirtualKeyBoard())
                clearButton.gameObject.SetActive(textComponent.text.Length > 1);
            }
 
         
        }
            /*  public void DeselectPreviousInputField(SelectClickEventData eventData)
        {
            
            if (lastInputField != null)
            {
                JMRUIPrimaryInputField previousIF = (JMRUIPrimaryInputField)lastInputField;
               
                if (lastInputField != this )
                {
                    previousIF.OnDeselect();
                }
            }
           
            onSelectionInputField?.Invoke(this);
        }*/
        protected override void OnDisable()
        {
            // the coroutine will be terminated, so this will ensure it restarts when we are next activated
            base.OnDisable();
            
            j_BlinkCoroutine = null;

            DeactivateInputField();
            if (textComponent != null)
            {
                textComponent.UnregisterDirtyVerticesCallback(MarkGeometryAsDirty);
                textComponent.UnregisterDirtyVerticesCallback(UpdateLabel);

                if (verticalScrollbar != null)
                    verticalScrollbar.onValueChanged.RemoveListener(OnScrollbarValueChange);

            }
            CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);

            // Clear needs to be called otherwise sync never happens as the object is disabled.
            if (j_CachedInputRenderer != null)
                j_CachedInputRenderer.Clear();

            if (j_Mesh != null)
                DestroyImmediate(j_Mesh);
            j_Mesh = null;

            // Unsubscribe to event triggered when text object has been regenerated
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
            if (clearButton != null)
                clearButton.OnClick.RemoveAllListeners();
            //base.OnDisable();
        }

        private bool wasError;
        Animator anim;
        /// <summary>
        /// Update the text based on input.
        /// </summary>
        // TODO: Make LateUpdate a coroutine instead. Allows us to control the update to only be when the field is active.
        private void LateUpdate()
        {
           
         

            if (!InteractableActive())
                return;

            if (wasError != IsError)
            {
                if (anim == null)
                {
                    anim = GetComponent<Animator>();
                }
                anim.SetBool("OnError", IsError);
                wasError = IsError;
            }

            // Only activate if we are not already activated.
            if (j_ShouldActivateNextUpdate)
            {
                if (!Focused)
                {
                   // ActivateInputFieldInternal();
                    j_ShouldActivateNextUpdate = false;
                    return;
                }

                // Reset as we are already activated.
                j_ShouldActivateNextUpdate = false;
            }

            // Update Scrollbar if needed
            if (j_ScrollbarUpdateRequired)
            {
                UpdateScrollbar();
                j_ScrollbarUpdateRequired = false;
            }
            if (hideMobileInput)
            {
                j_CaretPositionInternal = j_CaretSelectPositionInternal = GetCaretPositionFromStringIndex(Text.Length);
                return;
            }
            else if (InPlaceEditing() && j_KeyboardUsingEvents() || !Focused)
            {
                return;
            }

            AssignPositioningIfNeeded();

            if (j_SoftKeyboard == null || j_SoftKeyboard.status != TouchScreenKeyboard.Status.Visible)
            {
                if (j_SoftKeyboard != null)
                {
                    if (!readOnly)
                        Text = j_SoftKeyboard.text;

                    if (j_SoftKeyboard.status == TouchScreenKeyboard.Status.LostFocus)
                        SendTouchScreenKeyboardStatusChanged();

                    if (j_SoftKeyboard.status == TouchScreenKeyboard.Status.Canceled)
                    {
                        j_ReleaseSelection = true;
                        j_WasCanceled = true;
                        SendTouchScreenKeyboardStatusChanged();
                    }

                    if (j_SoftKeyboard.status == TouchScreenKeyboard.Status.Done)
                    {
                        j_ReleaseSelection = true;
                        OnSubmit(null);
                        SendTouchScreenKeyboardStatusChanged();
                    }
                }
              
                OnDeselect();
                return;
            }

            string val = j_SoftKeyboard.text;


            if (text != val)
            {
                if (readOnly)
                {
                    j_SoftKeyboard.text = text;
                }
                else
                {
                    text = "";

                    for (int i = 0; i < val.Length; ++i)
                    {
                        char c = val[i];

                        if (c == '\r' || (int)c == 3)
                            c = '\n';

                        if (ValidateInput != null)
                            c = ValidateInput(text, text.Length, c);
                        else if (CharacterValidationProp != CharacterValidation.None)
                            c = Validate(text, text.Length, c);

                        if (LineTypeProp == LineType.MultiLineSubmit && c == '\n')
                        {
                            j_SoftKeyboard.text = text;

                            OnSubmit(null);
                            Debug.LogError("Calling From Late Update22222");
                            OnDeselect();
                            return;
                        }

                        if (c != 0)
                            text += c;
                    }

                    if (CharacterLimit > 0 && text.Length > CharacterLimit)
                        text = text.Substring(0, CharacterLimit);

                    UpdateStringPositionFromKeyboard();

                    // Set keyboard text before updating label, as we might have changed it with validation
                    // and update label will take the old value from keyboard if we don't change it here
                    if (text != val)
                        j_SoftKeyboard.text = text;

                    SendOnValueChangedAndUpdateLabel();
                }
            }
            else if (hideMobileInput && Application.platform == RuntimePlatform.Android)
            {
                UpdateStringPositionFromKeyboard();
            }

            //else if (m_HideMobileInput) // m_Keyboard.canSetSelection
            //{
            //    int length = stringPositionInternal < stringSelectPositionInternal ? stringSelectPositionInternal - stringPositionInternal : stringPositionInternal - stringSelectPositionInternal;
            //    m_SoftKeyboard.selection = new RangeInt(stringPositionInternal < stringSelectPositionInternal ? stringPositionInternal : stringSelectPositionInternal, length);
            //}
            //else if (!m_HideMobileInput) // m_Keyboard.canGetSelection)
            //{
            //    UpdateStringPositionFromKeyboard();
            //}

            if (j_SoftKeyboard.status != TouchScreenKeyboard.Status.Visible)
            {
                if (j_SoftKeyboard.status == TouchScreenKeyboard.Status.Canceled)
                    j_WasCanceled = true;

                Debug.LogError("Calling From Late Update33333");
                OnDeselect();
            }
        }


        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Set Input field's current text value without invoke onValueChanged. This is not necessarily the same as what is visible on screen.
        /// </summary>
        public void SetTextWithoutNotify(string input)
        {
            SetText(input, false);
        }


        /// <summary>
        /// Move to the end of the text.
        /// </summary>
        /// <param name="shift"></param>
        public void MoveTextEnd(bool shift)
        {
            if (richTextEditingAllowed)
            {
                int position = Text.Length;

                if (shift)
                {
                    j_StringSelectPositionInternal = position;
                }
                else
                {
                    j_StringPositionInternal = position;
                    j_StringSelectPositionInternal = j_StringPositionInternal;
                }
            }
            else
            {
                int position = textComponent.textInfo.characterCount - 1;

                if (shift)
                {
                    j_CaretSelectPositionInternal = position;
                    j_StringSelectPositionInternal = GetStringIndexFromCaretPosition(position);
                }
                else
                {
                    j_CaretPositionInternal = j_CaretSelectPositionInternal = position;
                    j_StringSelectPositionInternal = j_StringPositionInternal = GetStringIndexFromCaretPosition(position);
                }
            }

            UpdateLabel();
        }

        /// <summary>
        /// Move to the start of the text.
        /// </summary>
        /// <param name="shift"></param>
        public void MoveTextStart(bool shift)
        {
            if (richTextEditingAllowed)
            {
                int position = 0;

                if (shift)
                {
                    j_StringSelectPositionInternal = position;
                }
                else
                {
                    j_StringPositionInternal = position;
                    j_StringSelectPositionInternal = j_StringPositionInternal;
                }
            }
            else
            {
                int position = 0;

                if (shift)
                {
                    j_CaretSelectPositionInternal = position;
                    j_StringSelectPositionInternal = GetStringIndexFromCaretPosition(position);
                }
                else
                {
                    j_CaretPositionInternal = j_CaretSelectPositionInternal = position;
                    j_StringSelectPositionInternal = j_StringPositionInternal = GetStringIndexFromCaretPosition(position);
                }
            }

            UpdateLabel();
        }


        /// <summary>
        /// Move to the end of the current line of text.
        /// </summary>
        /// <param name="shift"></param>
        public void MoveToEndOfLine(bool shift, bool ctrl)
        {
            // Get the line the caret is currently located on.
            int currentLine = textComponent.textInfo.characterInfo[j_CaretPositionInternal].lineNumber;

            // Get the last character of the given line.
            int characterIndex = ctrl == true ? textComponent.textInfo.characterCount - 1 : textComponent.textInfo.lineInfo[currentLine].lastCharacterIndex;

            int position = textComponent.textInfo.characterInfo[characterIndex].index;

            if (shift)
            {
                j_StringSelectPositionInternal = position;

                j_CaretSelectPositionInternal = characterIndex;
            }
            else
            {
                j_StringPositionInternal = position;
                j_StringSelectPositionInternal = j_StringPositionInternal;

                j_CaretSelectPositionInternal = j_CaretPositionInternal = characterIndex;
            }

            UpdateLabel();
        }

        /// <summary>
        /// Move to the start of the current line of text.
        /// </summary>
        /// <param name="shift"></param>
        public void MoveToStartOfLine(bool shift, bool ctrl)
        {
            // Get the line the caret is currently located on.
            int currentLine = textComponent.textInfo.characterInfo[j_CaretPositionInternal].lineNumber;

            // Get the first character of the given line.
            int characterIndex = ctrl == true ? 0 : textComponent.textInfo.lineInfo[currentLine].firstCharacterIndex;

            int position = 0;
            if (characterIndex > 0)
                position = textComponent.textInfo.characterInfo[characterIndex - 1].index + textComponent.textInfo.characterInfo[characterIndex - 1].stringLength;

            if (shift)
            {
                j_StringSelectPositionInternal = position;

                j_CaretSelectPositionInternal = characterIndex;
            }
            else
            {
                j_StringPositionInternal = position;
                j_StringSelectPositionInternal = j_StringPositionInternal;

                j_CaretSelectPositionInternal = j_CaretPositionInternal = characterIndex;
            }

            UpdateLabel();
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (!InteractableActive())
                return;

            if (!MayDrag(eventData))
                return;

            j_UpdateDrag = true;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!InteractableActive())
                return;

            if (!MayDrag(eventData))
                return;

            int insertionIndex = TMP_TextUtilities.GetCursorIndexFromPosition(textComponent, eventData.position, eventData.pressEventCamera, out CaretPosition insertionSide);

            if (richTextEditingAllowed)
            {
                if (insertionSide == TMPro.CaretPosition.Left)
                {
                    j_StringSelectPositionInternal = textComponent.textInfo.characterInfo[insertionIndex].index;
                }
                else if (insertionSide == TMPro.CaretPosition.Right)
                {
                    j_StringSelectPositionInternal = textComponent.textInfo.characterInfo[insertionIndex].index + textComponent.textInfo.characterInfo[insertionIndex].stringLength;
                }
            }
            else
            {
                if (insertionSide == TMPro.CaretPosition.Left)
                {
                    j_StringSelectPositionInternal = insertionIndex == 0
                        ? textComponent.textInfo.characterInfo[0].index
                        : textComponent.textInfo.characterInfo[insertionIndex - 1].index + textComponent.textInfo.characterInfo[insertionIndex - 1].stringLength;
                }
                else if (insertionSide == TMPro.CaretPosition.Right)
                {
                    j_StringSelectPositionInternal = textComponent.textInfo.characterInfo[insertionIndex].index + textComponent.textInfo.characterInfo[insertionIndex].stringLength;
                }
            }

            j_CaretSelectPositionInternal = GetCaretPositionFromStringIndex(j_StringSelectPositionInternal);

            MarkGeometryAsDirty();

            j_DragPositionOutOfBounds = !RectTransformUtility.RectangleContainsScreenPoint(TextViewport, eventData.position, eventData.pressEventCamera);
            if (j_DragPositionOutOfBounds && j_DragCoroutine == null)
                j_DragCoroutine = StartCoroutine(MouseDragOutsideRect(eventData));

            eventData.Use();

#if TMP_DEBUG_MODE
                Debug.Log("Caret Position: " + caretPositionInternal + " Selection Position: " + caretSelectPositionInternal + "  String Position: " + stringPositionInternal + " String Select Position: " + stringSelectPositionInternal);
#endif
        }


        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!InteractableActive())
                return;

            if (!MayDrag(eventData))
                return;

            j_UpdateDrag = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            

            if (!InteractableActive())
                return;
            if (!MayDrag(eventData))
                return;

            EventSystem.current.SetSelectedGameObject(gameObject, eventData);

            bool hadFocusBefore = j_AllowInput;
            //base.OnPointerDown(eventData);

            //if (InPlaceEditing() == false)
            //{
            //    if (j_SoftKeyboard == null || !j_SoftKeyboard.active)
            //    {
            //        OnSelect(eventData);
            //        return;
            //    }
            //}

            bool shift = UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);

            // Check for Double Click
            bool isDoubleClick = false;
            float timeStamp = Time.unscaledTime;

            if (j_PointerDownClickStartTime + j_DoubleClickDelay > timeStamp)
                isDoubleClick = true;

            j_PointerDownClickStartTime = timeStamp;

            // Only set caret position if we didn't just get focus now.
            // Otherwise it will overwrite the select all on focus.
            //if (hadFocusBefore || !focusSelectAll)
            //{
            int insertionIndex = TMP_TextUtilities.GetCursorIndexFromPosition(textComponent, eventData.position, eventData.pressEventCamera, out CaretPosition insertionSide);

            if (shift)
            {
                if (richTextEditingAllowed)
                {
                    if (insertionSide == TMPro.CaretPosition.Left)
                    {
                        j_StringSelectPositionInternal = textComponent.textInfo.characterInfo[insertionIndex].index;
                    }
                    else if (insertionSide == TMPro.CaretPosition.Right)
                    {
                        j_StringSelectPositionInternal = textComponent.textInfo.characterInfo[insertionIndex].index + textComponent.textInfo.characterInfo[insertionIndex].stringLength;
                    }
                }
                else
                {
                    if (insertionSide == TMPro.CaretPosition.Left)
                    {
                        j_StringSelectPositionInternal = insertionIndex == 0
                            ? textComponent.textInfo.characterInfo[0].index
                            : textComponent.textInfo.characterInfo[insertionIndex - 1].index + textComponent.textInfo.characterInfo[insertionIndex - 1].stringLength;
                    }
                    else if (insertionSide == TMPro.CaretPosition.Right)
                    {
                        j_StringSelectPositionInternal = textComponent.textInfo.characterInfo[insertionIndex].index + textComponent.textInfo.characterInfo[insertionIndex].stringLength;
                    }
                }
            }
            else
            {
                if (richTextEditingAllowed)
                {
                    if (insertionSide == TMPro.CaretPosition.Left)
                    {
                        j_StringPositionInternal = j_StringSelectPositionInternal = textComponent.textInfo.characterInfo[insertionIndex].index;
                    }
                    else if (insertionSide == TMPro.CaretPosition.Right)
                    {
                        j_StringPositionInternal = j_StringSelectPositionInternal = textComponent.textInfo.characterInfo[insertionIndex].index + textComponent.textInfo.characterInfo[insertionIndex].stringLength;
                    }
                }
                else
                {
                    if (insertionSide == TMPro.CaretPosition.Left)
                    {
                        j_StringPositionInternal = j_StringSelectPositionInternal = insertionIndex == 0
                            ? textComponent.textInfo.characterInfo[0].index
                            : textComponent.textInfo.characterInfo[insertionIndex - 1].index + textComponent.textInfo.characterInfo[insertionIndex - 1].stringLength;
                    }
                    else if (insertionSide == TMPro.CaretPosition.Right)
                    {
                        j_StringPositionInternal = j_StringSelectPositionInternal = textComponent.textInfo.characterInfo[insertionIndex].index + textComponent.textInfo.characterInfo[insertionIndex].stringLength;
                    }
                }
            }

            // Select current character
            j_CaretPositionInternal = j_CaretSelectPositionInternal = GetCaretPositionFromStringIndex(j_StringPositionInternal);

            //if (isDoubleClick)
            //{
            //    int wordIndex = TMP_TextUtilities.FindIntersectingWord(textComponent, eventData.position, eventData.pressEventCamera);

            //    if (wordIndex != -1)
            //    {
            //        // TODO: Should behavior be different if rich text editing is enabled or not?

            //        // Select current word
            //        j_CaretPositionInternal = textComponent.textInfo.wordInfo[wordIndex].firstCharacterIndex;
            //        j_CaretSelectPositionInternal = textComponent.textInfo.wordInfo[wordIndex].lastCharacterIndex + 1;

            //        j_StringPositionInternal = textComponent.textInfo.characterInfo[j_CaretPositionInternal].index;
            //        j_StringSelectPositionInternal = textComponent.textInfo.characterInfo[j_CaretSelectPositionInternal - 1].index + textComponent.textInfo.characterInfo[j_CaretSelectPositionInternal - 1].stringLength;
            //    }
            //    else
            //    {
            //        // Select current character
            //        j_CaretPositionInternal = insertionIndex;
            //        j_CaretSelectPositionInternal = j_CaretPositionInternal + 1;

            //        j_StringPositionInternal = textComponent.textInfo.characterInfo[insertionIndex].index;
            //        j_StringSelectPositionInternal = j_StringPositionInternal + textComponent.textInfo.characterInfo[insertionIndex].stringLength;
            //    }
            //}
            //else
            //{
            //    j_CaretPositionInternal = j_CaretSelectPositionInternal = GetCaretPositionFromStringIndex(j_StringPositionInternal);
            //}
            // }

            UpdateLabel();
            eventData.Use();

#if TMP_DEBUG_MODE
                Debug.Log("Caret Position: " + caretPositionInternal + " Selection Position: " + caretSelectPositionInternal + "  String Position: " + stringPositionInternal + " String Select Position: " + stringSelectPositionInternal);
#endif
        }

        public void ProcessEvent(Event e)
        {
            if (!InteractableActive())
                return;
            KeyPressed(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnUpdateSelected(BaseEventData eventData)
        {
            if (!InteractableActive())
                return;
            if (!Focused)
                return;

            bool consumedEvent = false;
            while (Event.PopEvent(j_ProcessingEvent))
            {
                if (j_ProcessingEvent.rawType == EventType.KeyDown)
                {
                    //Debug.Log("Event: " + m_ProcessingEvent.ToString());

                    consumedEvent = true;
                    var shouldContinue = KeyPressed(j_ProcessingEvent);
                    if (shouldContinue == EditState.Finish)
                    {
                        SendOnSubmit();
                        DeactivateInputField();
                        break;
                    }
                }

                switch (j_ProcessingEvent.type)
                {
                    case EventType.ValidateCommand:
                    case EventType.ExecuteCommand:
                        switch (j_ProcessingEvent.commandName)
                        {
                            case "SelectAll":
                                SelectAll();
                                consumedEvent = true;
                                break;
                        }
                        break;
                }
            }

            if (consumedEvent)
                UpdateLabel();

            eventData.Use();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnScroll(PointerEventData eventData)
        {
            if (!InteractableActive())
                return;

            if (textComponent.preferredHeight < TextViewport.rect.height) return;

            float scrollDirection = -eventData.scrollDelta.y;

            j_ScrollPosition = j_ScrollPosition + (1f / textComponent.textInfo.lineCount) * scrollDirection * scrollSensitivity;

            j_ScrollPosition = Mathf.Clamp01(j_ScrollPosition);

            AdjustTextPositionRelativeToViewport(j_ScrollPosition);

            // Disable focus until user re-selected the input field.
            j_AllowInput = false;

            if (verticalScrollbar)
            {
                j_UpdatingScrollbarValues = true;
                verticalScrollbar.value = j_ScrollPosition;
            }

            //Debug.Log("Scroll Position:" + m_ScrollPosition);
        }

        public void ForceLabelUpdate()
        {
            if (!InteractableActive())
                return;
            UpdateLabel();
        }

        public void Rebuild(CanvasUpdate update)
        {
            if (!InteractableActive())
                return;
            switch (update)
            {
                case CanvasUpdate.LatePreRender:
                    UpdateGeometry();
                    break;
            }
        }

        public void LayoutComplete()
        { }

        public void GraphicUpdateComplete()
        { }

        public void ActivateInputField()
        {
            if (!InteractableActive())
                return;
            if (textComponent == null || textComponent.font == null || !gameObject.activeSelf)
                return;

            if (Focused)
            {
                if (j_SoftKeyboard != null && !j_SoftKeyboard.active)
                {
                    j_SoftKeyboard.active = true;
                    j_SoftKeyboard.text = text;
                }
            }

            j_ShouldActivateNextUpdate = true;
        }
        public void OnSelect(BaseEventData eventData)
        {
            
            if (!InteractableActive())
                return;

            //base.OnSelect(eventData);

            SendOnFocus();
            
            ActivateInputField();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
           Debug.LogError("OnPointerClick");
            
            if (!InteractableActive())
                return;
            
          

            if (true)//Bhumit
            {
                if (JMRVirtualKeyBoard.Instance)
                {
                    JMRVirtualKeyBoard.Instance.ShowKeyBoard(this);
                }
                else
                {
                    if (!JMRKeyboardSpawner.Instance.SpawnKeyboard(this))
                            Debug.LogError("Cant find keyboard prefab");
                    
                }


            }

            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            ActivateInputField();
            isFieldSelected = true;
        }

        public void OnControlClick()
        {
            //Debug.Log("Input Field control click...");
        }

        public void ReleaseSelection()
        {
            j_SelectionStillActive = false;
            MarkGeometryAsDirty();
        }

        public void DeactivateInputField(bool clearSelection = false)
        {
            if (!InteractableActive())
                return;
            //Debug.Log("Deactivate Input Field...");

            // Not activated do nothing.
            if (!j_AllowInput)
                return;

            j_HasDoneFocusTransition = false;
            j_AllowInput = false;

            if (placeholder != null)
                placeholder.enabled = string.IsNullOrEmpty(text);

            if (textComponent != null)
            {
                if (j_WasCanceled && restoreOriginalTextOnEscape)
                    Text = j_OriginalText;

                if (j_SoftKeyboard != null)
                {
                    j_SoftKeyboard.active = false;
                    j_SoftKeyboard = null;
                }

                j_SelectionStillActive = true;

                if (resetOnDeActivation || j_ReleaseSelection)
                {
                    //m_StringPosition = m_StringSelectPosition = 0;
                    //m_CaretPosition = m_CaretSelectPosition = 0;
                    //m_TextComponent.rectTransform.localPosition = m_DefaultTransformPosition;

                    //if (caretRectTrans != null)
                    //    caretRectTrans.localPosition = Vector3.zero;

                    j_SelectionStillActive = false;
                    j_ReleaseSelection = false;
                    j_SelectedObject = null;
                }

                SendOnEndEdit();
                SendOnEndTextSelection();

                //inputSystem.imeCompositionMode = IMECompositionMode.Auto;
            }

            MarkGeometryAsDirty();

            // Scrollbar should be updated.
            j_ScrollbarUpdateRequired = true;
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            if (!InteractableActive())
                return;
            //Debug.Log("OnSubmit()");

            if (!gameObject.activeSelf)
                return;

            if (!Focused)
                j_ShouldActivateNextUpdate = true;

            SendOnSubmit();
        }

        /// <summary>
        /// See ILayoutElement.CalculateLayoutInputHorizontal.
        /// </summary>
        public virtual void CalculateLayoutInputHorizontal()
        { }

        /// <summary>
        /// See ILayoutElement.CalculateLayoutInputVertical.
        /// </summary>
        public virtual void CalculateLayoutInputVertical()
        { }


        /// <summary>
        /// Function to conveniently set the point size of both Placeholder and Input Field text object.
        /// </summary>
        /// <param name="pointSize"></param>
        public void SetGlobalPointSize(float pointSize)
        {
            TMP_Text placeholderTextComponent = placeholder as TMP_Text;

            if (placeholderTextComponent != null) placeholderTextComponent.fontSize = pointSize;
            TextComponent.fontSize = pointSize;
        }

        /// <summary>
        /// Function to conveniently set the Font Asset of both Placeholder and Input Field text object.
        /// </summary>
        /// <param name="fontAsset"></param>
        public void SetGlobalFontAsset(TMP_FontAsset fontAsset)
        {
            TMP_Text placeholderTextComponent = placeholder as TMP_Text;

            if (placeholderTextComponent != null) placeholderTextComponent.font = fontAsset;
            TextComponent.font = fontAsset;

        }

        public bool IsDestroyed()
        {
            return false;
        }

        #endregion

        #region DEFAULT METHODS
        void SetText(string value, bool sendCallback = true)
        {
            if (this.Text == value)
                return;

            if (value == null)
                value = "";

            value = value.Replace("\0", string.Empty); // remove embedded nulls

            text = value;

            if (Application.isEditor && !Application.isPlaying)
            {
                SendOnValueChangedAndUpdateLabel();
                return;
            }


            if (j_SoftKeyboard != null)
                j_SoftKeyboard.text = text;

            if (j_StringPosition > text.Length)
                j_StringPosition = j_StringSelectPosition = text.Length;
            else if (j_StringSelectPosition > text.Length)
                j_StringSelectPosition = text.Length;

            // Set RectTransform relative position to top of viewport.
            AdjustTextPositionRelativeToViewport(0);

            j_ForceRectTransformAdjustment = true;

            j_TextComponentUpdateRequired = true;
            UpdateLabel();

            if (sendCallback)
                SendDynamicValueChange(Text);
        }

        void SetCaretVisible()
        {
            if (!j_AllowInput)
                return;

            j_CaretVisible = true;
            j_BlinkStartTime = Time.unscaledTime;
            SetCaretActive();
        }

        // SetCaretActive will not set the caret immediately visible - it will wait for the next time to blink.
        // However, it will handle things correctly if the blink speed changed from zero to non-zero or non-zero to zero.
        void SetCaretActive()
        {
            if (!j_AllowInput)
                return;

            if (caretBlinkRate > 0.0f)
            {
                if (j_BlinkCoroutine == null)
                    j_BlinkCoroutine = StartCoroutine(CaretBlink());
            }
            else
            {
                j_CaretVisible = true;
            }
        }

        void UpdateStringPositionFromKeyboard()
        {
            // TODO: Might want to add null check here.
            var selectionRange = j_SoftKeyboard.selection;

            if (selectionRange.start == 0 && selectionRange.length == 0)
                return;

            var selectionStart = selectionRange.start;
            var selectionEnd = selectionRange.end;

            var stringPositionChanged = false;

            if (j_StringPositionInternal != selectionStart)
            {
                stringPositionChanged = true;
                j_StringPositionInternal = selectionStart;

                j_CaretPositionInternal = GetCaretPositionFromStringIndex(j_StringPositionInternal);
            }

            if (j_StringSelectPositionInternal != selectionEnd)
            {
                j_StringSelectPositionInternal = selectionEnd;
                stringPositionChanged = true;

                j_CaretSelectPositionInternal = GetCaretPositionFromStringIndex(j_StringSelectPositionInternal);
            }

            if (stringPositionChanged)
            {
                j_BlinkStartTime = Time.unscaledTime;

                UpdateLabel();
            }
        }

        void UpdateScrollbar()
        {
            // Update Scrollbar
            if (verticalScrollbar)
            {
                float size = TextViewport.rect.height / textComponent.preferredHeight;

                j_UpdatingScrollbarValues = true;

                verticalScrollbar.size = size;

                j_ScrollPosition = verticalScrollbar.value = textComponent.rectTransform.anchoredPosition.y / (textComponent.preferredHeight - TextViewport.rect.height);

                //m_VerticalScrollbar.numberOfSteps = (int)(m_TextComponent.textInfo.lineCount / 0.25f); // Replace by scroll sensitivity.

                //Debug.Log("Updating Scrollbar... Value: " + m_VerticalScrollbar.value);
            }
        }


        /// <summary>
        /// Function to update the vertical position of the text container when OnValueChanged event is received from the Scrollbar.
        /// </summary>
        /// <param name="value"></param>
        void OnScrollbarValueChange(float value)
        {
            if (!InteractableActive())
                return;
            if (j_UpdatingScrollbarValues) { j_UpdatingScrollbarValues = false; return; }

            if (value < 0 || value > 1) return;

            AdjustTextPositionRelativeToViewport(value);

            j_ScrollPosition = value;

            //Debug.Log("Scrollbar value is: " + value + "  Transform POS: " + m_TextComponent.rectTransform.anchoredPosition);
        }

        /// <summary>
        /// Adjusts the relative position of the body of the text relative to the viewport.
        /// </summary>
        /// <param name="relativePosition"></param>
        void AdjustTextPositionRelativeToViewport(float relativePosition)
        {
            //Debug.Log("- Adjusting vertical text position to " + relativePosition);
            if (TextViewport == null)
                return;

            TMP_TextInfo textInfo = textComponent.textInfo;

            // Check to make sure we have valid data and lines to query.
            if (textInfo == null || textInfo.lineInfo == null || textInfo.lineCount == 0 || textInfo.lineCount > textInfo.lineInfo.Length) return;

            //m_TextComponent.rectTransform.anchoredPosition = new Vector2(m_TextComponent.rectTransform.anchoredPosition.x, (textHeight - viewportHeight) * relativePosition);
            textComponent.rectTransform.anchoredPosition = new Vector2(textComponent.rectTransform.anchoredPosition.x, (textComponent.preferredHeight - TextViewport.rect.height) * relativePosition);

            AssignPositioningIfNeeded();

            //Debug.Log("Text height: " + m_TextComponent.preferredHeight + "  Viewport height: " + m_TextViewport.rect.height + "  Adjusted RectTransform anchordedPosition:" + m_TextComponent.rectTransform.anchoredPosition + "  Text Bounds: " + m_TextComponent.bounds.ToString("f3"));
        }

        void SetTextComponentWrapMode()
        {
            if (textComponent == null)
                return;

            if (MultiLine)
                textComponent.enableWordWrapping = true;
            else
                textComponent.enableWordWrapping = false;
        }

        // Control Rich Text option on the text component.
        void SetTextComponentRichTextMode()
        {
            if (textComponent == null)
                return;

            textComponent.richText = richText;
        }

        void SetToCustomIfContentTypeIsNot(params ContentType[] allowedContentTypes)
        {
            if (ContentTypeProp == ContentType.Custom)
                return;

            for (int i = 0; i < allowedContentTypes.Length; i++)
                if (ContentTypeProp == allowedContentTypes[i])
                    return;

            ContentTypeProp = ContentType.Custom;
        }

        void SetToCustom()
        {
            if (ContentTypeProp == ContentType.Custom)
                return;

            ContentTypeProp = ContentType.Custom;
        }

        void SetToCustom(CharacterValidation characterValidation)
        {
            if (ContentTypeProp == ContentType.Custom)
            {
                characterValidation = CharacterValidation.CustomValidator;
                return;
            }

            ContentTypeProp = ContentType.Custom;
            characterValidation = CharacterValidation.CustomValidator;
        }

        #endregion

        #region PRIVATE METHODS
        private void onClearButtonClicked()
        {
            SetSoftKeyboardTextInput("");
            textComponent.text = text = "";
        }
        private bool InteractableActive()
        {
            return interactable;
        }


        /// <summary>
        /// Method used to update the tracking of the caret position when the text object has been regenerated.
        /// </summary>
        /// <param name="obj"></param>
        private void ON_TEXT_CHANGED(UnityEngine.Object obj)
        {
            if (clearButton != null)
            {
                if(isFieldSelected && !isVirtualKeyBoard())
                {
                clearButton.gameObject.SetActive(textComponent.text.Length > 1);
                }
            }

            if (!InteractableActive())
                return;

            if (obj == textComponent && Application.isPlaying && j_CompositionString.Length == 0)
            {
                j_CaretPositionInternal = GetCaretPositionFromStringIndex(j_StringPositionInternal);
                j_CaretSelectPositionInternal = GetCaretPositionFromStringIndex(j_StringSelectPositionInternal);

#if TMP_DEBUG_MODE
                    Debug.Log("Caret Position: " + caretPositionInternal + " Selection Position: " + caretSelectPositionInternal + "  String Position: " + stringPositionInternal + " String Select Position: " + stringSelectPositionInternal);
#endif
            }
        }

        private void OnFocus()
        {
            if (!InteractableActive())
                return;
            if (focusSelectAll)
                SelectAll();
        }

        private void SelectAll()
        {
            j_SelectAll = true;
            j_StringPositionInternal = Text.Length;
            j_StringSelectPositionInternal = 0;
        }

        private bool InPlaceEditing()
        {
            if (j_TouchKeyboardAllowsInPlaceEditing || (TouchScreenKeyboard.isSupported && (Application.platform == RuntimePlatform.WSAPlayerX86 || Application.platform == RuntimePlatform.WSAPlayerX64 || Application.platform == RuntimePlatform.WSAPlayerARM)))
                return true;

            if (TouchScreenKeyboard.isSupported && HideSoftKeyboard)
                return true;

            if (TouchScreenKeyboard.isSupported && HideSoftKeyboard == false && HideMobileInput == false)
                return false;

            return true;
        }

        private bool MayDrag(PointerEventData eventData)
        {
            return gameObject.activeSelf &&
                   eventData.button == PointerEventData.InputButton.Left &&
                   textComponent != null &&
                   (j_SoftKeyboard == null || HideSoftKeyboard || HideMobileInput);
        }



        private string GetSelectedString()
        {
            if (!Selected)
                return "";

            int startPos = j_StringPositionInternal;
            int endPos = j_StringSelectPositionInternal;

            // Ensure pos is always less then selPos to make the code simpler
            if (startPos > endPos)
            {
                int temp = startPos;
                startPos = endPos;
                endPos = temp;
            }

            //for (int i = m_CaretPosition; i < m_CaretSelectPosition; i++)
            //{
            //    Debug.Log("Character [" + m_TextComponent.textInfo.characterInfo[i].character + "] using Style [" + m_TextComponent.textInfo.characterInfo[i].style + "] has been selected.");
            //}


            return Text.Substring(startPos, endPos - startPos);
        }

        private int FindNextWordBegin()
        {
            if (j_StringSelectPositionInternal + 1 >= Text.Length)
                return Text.Length;

            int spaceLoc = Text.IndexOfAny(j_KSeparators, j_StringSelectPositionInternal + 1);

            if (spaceLoc == -1)
                spaceLoc = Text.Length;
            else
                spaceLoc++;

            return spaceLoc;
        }

        private void MoveRight(bool shift, bool ctrl)
        {
            if (Selected && !shift)
            {
                // By convention, if we have a selection and move right without holding shift,
                // we just place the cursor at the end.
                j_StringPositionInternal = j_StringSelectPositionInternal = Mathf.Max(j_StringPositionInternal, j_StringSelectPositionInternal);
                j_CaretPositionInternal = j_CaretSelectPositionInternal = GetCaretPositionFromStringIndex(j_StringSelectPositionInternal);

#if TMP_DEBUG_MODE
                    Debug.Log("Caret Position: " + caretPositionInternal + " Selection Position: " + caretSelectPositionInternal + "  String Position: " + stringPositionInternal + " String Select Position: " + stringSelectPositionInternal);
#endif
                return;
            }

            int position;
            if (ctrl)
                position = FindNextWordBegin();
            else
            {
                if (richTextEditingAllowed)
                {
                    // Special handling for Surrogate pairs and Diacritical marks.
                    if (j_StringSelectPositionInternal < Text.Length && char.IsHighSurrogate(Text[j_StringSelectPositionInternal]))
                        position = j_StringSelectPositionInternal + 2;
                    else
                        position = j_StringSelectPositionInternal + 1;
                }
                else
                {
                    position = textComponent.textInfo.characterInfo[j_CaretSelectPositionInternal].index + textComponent.textInfo.characterInfo[j_CaretSelectPositionInternal].stringLength;
                }

            }

            if (shift)
            {
                j_StringSelectPositionInternal = position;
                j_CaretSelectPositionInternal = GetCaretPositionFromStringIndex(j_StringSelectPositionInternal);
            }
            else
            {
                j_StringSelectPositionInternal = j_StringPositionInternal = position;

                // Only increase caret position as we cross character boundary.
                if (j_StringPositionInternal >= textComponent.textInfo.characterInfo[j_CaretPositionInternal].index + textComponent.textInfo.characterInfo[j_CaretPositionInternal].stringLength)
                    j_CaretSelectPositionInternal = j_CaretPositionInternal = GetCaretPositionFromStringIndex(j_StringSelectPositionInternal);
            }

#if TMP_DEBUG_MODE
                Debug.Log("Caret Position: " + caretPositionInternal + "  Selection Position: " + caretSelectPositionInternal + "  String Position: " + stringPositionInternal + "  String Select Position: " + stringSelectPositionInternal);
#endif
        }

        private int FindPrevWordBegin()
        {
            if (j_StringSelectPositionInternal - 2 < 0)
                return 0;

            int spaceLoc = Text.LastIndexOfAny(j_KSeparators, j_StringSelectPositionInternal - 2);

            if (spaceLoc == -1)
                spaceLoc = 0;
            else
                spaceLoc++;

            return spaceLoc;
        }

        private void MoveLeft(bool shift, bool ctrl)
        {
            if (Selected && !shift)
            {
                // By convention, if we have a selection and move left without holding shift,
                // we just place the cursor at the start.
                j_StringPositionInternal = j_StringSelectPositionInternal = Mathf.Min(j_StringPositionInternal, j_StringSelectPositionInternal);
                j_CaretPositionInternal = j_CaretSelectPositionInternal = GetCaretPositionFromStringIndex(j_StringSelectPositionInternal);

#if TMP_DEBUG_MODE
                    Debug.Log("Caret Position: " + caretPositionInternal + " Selection Position: " + caretSelectPositionInternal + "  String Position: " + stringPositionInternal + " String Select Position: " + stringSelectPositionInternal);
#endif
                return;
            }

            int position;
            if (ctrl)
                position = FindPrevWordBegin();
            else
            {
                if (richTextEditingAllowed)
                {
                    // Special handling for Surrogate pairs and Diacritical marks.
                    if (j_StringSelectPositionInternal > 0 && char.IsLowSurrogate(Text[j_StringSelectPositionInternal - 1]))
                        position = j_StringSelectPositionInternal - 2;
                    else
                        position = j_StringSelectPositionInternal - 1;
                }
                else
                {
                    //position = GetStringIndexFromCaretPosition(caretSelectPositionInternal - 1);
                    position = j_CaretSelectPositionInternal < 2
                        ? textComponent.textInfo.characterInfo[0].index
                        : textComponent.textInfo.characterInfo[j_CaretSelectPositionInternal - 2].index + textComponent.textInfo.characterInfo[j_CaretSelectPositionInternal - 2].stringLength;
                }
            }

            if (shift)
            {
                j_StringSelectPositionInternal = position;
                j_CaretSelectPositionInternal = GetCaretPositionFromStringIndex(j_StringSelectPositionInternal);
            }
            else
            {
                j_StringSelectPositionInternal = j_StringPositionInternal = position;

                // Only decrease caret position as we cross character boundary. 
                if (j_CaretPositionInternal > 0 && j_StringPositionInternal <= textComponent.textInfo.characterInfo[j_CaretPositionInternal - 1].index)
                    j_CaretSelectPositionInternal = j_CaretPositionInternal = GetCaretPositionFromStringIndex(j_StringSelectPositionInternal);
            }

#if TMP_DEBUG_MODE
                Debug.Log("Caret Position: " + caretPositionInternal + "  Selection Position: " + caretSelectPositionInternal + "  String Position: " + stringPositionInternal + "  String Select Position: " + stringSelectPositionInternal);
#endif
        }


        private int LineUpCharacterPosition(int originalPos, bool goToFirstChar)
        {
            if (originalPos >= textComponent.textInfo.characterCount)
                originalPos -= 1;

            TMP_CharacterInfo originChar = textComponent.textInfo.characterInfo[originalPos];
            int originLine = originChar.lineNumber;

            // We are on the first line return first character
            if (originLine - 1 < 0)
                return goToFirstChar ? 0 : originalPos;

            int endCharIdx = textComponent.textInfo.lineInfo[originLine].firstCharacterIndex - 1;

            int closest = -1;
            float distance = TMP_Math.FLOAT_MAX;
            float range = 0;

            for (int i = textComponent.textInfo.lineInfo[originLine - 1].firstCharacterIndex; i < endCharIdx; ++i)
            {
                TMP_CharacterInfo currentChar = textComponent.textInfo.characterInfo[i];

                float d = originChar.origin - currentChar.origin;
                float r = d / (currentChar.xAdvance - currentChar.origin);

                if (r >= 0 && r <= 1)
                {
                    if (r < 0.5f)
                        return i;
                    else
                        return i + 1;
                }

                d = Mathf.Abs(d);

                if (d < distance)
                {
                    closest = i;
                    distance = d;
                    range = r;
                }
            }

            if (closest == -1) return endCharIdx;

            //Debug.Log("Returning nearest character with Range = " + range);

            if (range < 0.5f)
                return closest;
            else
                return closest + 1;
        }


        private int LineDownCharacterPosition(int originalPos, bool goToLastChar)
        {
            if (originalPos >= textComponent.textInfo.characterCount)
                return textComponent.textInfo.characterCount - 1; // text.Length;

            TMP_CharacterInfo originChar = textComponent.textInfo.characterInfo[originalPos];
            int originLine = originChar.lineNumber;

            //// We are on the last line return last character
            if (originLine + 1 >= textComponent.textInfo.lineCount)
                return goToLastChar ? textComponent.textInfo.characterCount - 1 : originalPos;

            // Need to determine end line for next line.
            int endCharIdx = textComponent.textInfo.lineInfo[originLine + 1].lastCharacterIndex;

            int closest = -1;
            float distance = TMP_Math.FLOAT_MAX;
            float range = 0;

            for (int i = textComponent.textInfo.lineInfo[originLine + 1].firstCharacterIndex; i < endCharIdx; ++i)
            {
                TMP_CharacterInfo currentChar = textComponent.textInfo.characterInfo[i];

                float d = originChar.origin - currentChar.origin;
                float r = d / (currentChar.xAdvance - currentChar.origin);

                if (r >= 0 && r <= 1)
                {
                    if (r < 0.5f)
                        return i;
                    else
                        return i + 1;
                }

                d = Mathf.Abs(d);

                if (d < distance)
                {
                    closest = i;
                    distance = d;
                    range = r;
                }
            }

            if (closest == -1) return endCharIdx;

            //Debug.Log("Returning nearest character with Range = " + range);

            if (range < 0.5f)
                return closest;
            else
                return closest + 1;
        }


        private int PageUpCharacterPosition(int originalPos, bool goToFirstChar)
        {
            if (originalPos >= textComponent.textInfo.characterCount)
                originalPos -= 1;

            TMP_CharacterInfo originChar = textComponent.textInfo.characterInfo[originalPos];
            int originLine = originChar.lineNumber;

            // We are on the first line return first character
            if (originLine - 1 < 0)
                return goToFirstChar ? 0 : originalPos;

            float viewportHeight = TextViewport.rect.height;

            int newLine = originLine - 1;
            // Iterate through each subsequent line to find the first baseline that is not visible in the viewport.
            for (; newLine > 0; newLine--)
            {
                if (textComponent.textInfo.lineInfo[newLine].baseline > textComponent.textInfo.lineInfo[originLine].baseline + viewportHeight)
                    break;
            }

            int endCharIdx = textComponent.textInfo.lineInfo[newLine].lastCharacterIndex;

            int closest = -1;
            float distance = TMP_Math.FLOAT_MAX;
            float range = 0;

            for (int i = textComponent.textInfo.lineInfo[newLine].firstCharacterIndex; i < endCharIdx; ++i)
            {
                TMP_CharacterInfo currentChar = textComponent.textInfo.characterInfo[i];

                float d = originChar.origin - currentChar.origin;
                float r = d / (currentChar.xAdvance - currentChar.origin);

                if (r >= 0 && r <= 1)
                {
                    if (r < 0.5f)
                        return i;
                    else
                        return i + 1;
                }

                d = Mathf.Abs(d);

                if (d < distance)
                {
                    closest = i;
                    distance = d;
                    range = r;
                }
            }

            if (closest == -1) return endCharIdx;

            //Debug.Log("Returning nearest character with Range = " + range);

            if (range < 0.5f)
                return closest;
            else
                return closest + 1;
        }


        private int PageDownCharacterPosition(int originalPos, bool goToLastChar)
        {
            if (originalPos >= textComponent.textInfo.characterCount)
                return textComponent.textInfo.characterCount - 1;

            TMP_CharacterInfo originChar = textComponent.textInfo.characterInfo[originalPos];
            int originLine = originChar.lineNumber;

            // We are on the last line return last character
            if (originLine + 1 >= textComponent.textInfo.lineCount)
                return goToLastChar ? textComponent.textInfo.characterCount - 1 : originalPos;

            float viewportHeight = TextViewport.rect.height;

            int newLine = originLine + 1;
            // Iterate through each subsequent line to find the first baseline that is not visible in the viewport.
            for (; newLine < textComponent.textInfo.lineCount - 1; newLine++)
            {
                if (textComponent.textInfo.lineInfo[newLine].baseline < textComponent.textInfo.lineInfo[originLine].baseline - viewportHeight)
                    break;
            }

            // Need to determine end line for next line.
            int endCharIdx = textComponent.textInfo.lineInfo[newLine].lastCharacterIndex;

            int closest = -1;
            float distance = TMP_Math.FLOAT_MAX;
            float range = 0;

            for (int i = textComponent.textInfo.lineInfo[newLine].firstCharacterIndex; i < endCharIdx; ++i)
            {
                TMP_CharacterInfo currentChar = textComponent.textInfo.characterInfo[i];

                float d = originChar.origin - currentChar.origin;
                float r = d / (currentChar.xAdvance - currentChar.origin);

                if (r >= 0 && r <= 1)
                {
                    if (r < 0.5f)
                        return i;
                    else
                        return i + 1;
                }

                d = Mathf.Abs(d);

                if (d < distance)
                {
                    closest = i;
                    distance = d;
                    range = r;
                }
            }

            if (closest == -1) return endCharIdx;

            if (range < 0.5f)
                return closest;
            else
                return closest + 1;
        }


        private void MoveDown(bool shift)
        {
            MoveDown(shift, true);
        }


        private void MoveDown(bool shift, bool goToLastChar)
        {
            if (Selected && !shift)
            {
                // If we have a selection and press down without shift,
                // set caret to end of selection before we move it down.
                j_CaretPositionInternal = j_CaretSelectPositionInternal = Mathf.Max(j_CaretPositionInternal, j_CaretSelectPositionInternal);
            }

            int position = MultiLine ? LineDownCharacterPosition(j_CaretSelectPositionInternal, goToLastChar) : textComponent.textInfo.characterCount - 1; // text.Length;

            if (shift)
            {
                j_CaretSelectPositionInternal = position;
                j_StringSelectPositionInternal = GetStringIndexFromCaretPosition(j_CaretSelectPositionInternal);
            }
            else
            {
                j_CaretSelectPositionInternal = j_CaretPositionInternal = position;
                j_StringSelectPositionInternal = j_StringPositionInternal = GetStringIndexFromCaretPosition(j_CaretSelectPositionInternal);
            }

#if TMP_DEBUG_MODE
                Debug.Log("Caret Position: " + caretPositionInternal + " Selection Position: " + caretSelectPositionInternal + "  String Position: " + stringPositionInternal + " String Select Position: " + stringSelectPositionInternal);
#endif
        }

        private void MoveUp(bool shift)
        {
            MoveUp(shift, true);
        }


        private void MoveUp(bool shift, bool goToFirstChar)
        {
            if (Selected && !shift)
            {
                // If we have a selection and press up without shift,
                // set caret position to start of selection before we move it up.
                j_CaretPositionInternal = j_CaretSelectPositionInternal = Mathf.Min(j_CaretPositionInternal, j_CaretSelectPositionInternal);
            }

            int position = MultiLine ? LineUpCharacterPosition(j_CaretSelectPositionInternal, goToFirstChar) : 0;

            if (shift)
            {
                j_CaretSelectPositionInternal = position;
                j_StringSelectPositionInternal = GetStringIndexFromCaretPosition(j_CaretSelectPositionInternal);
            }
            else
            {
                j_CaretSelectPositionInternal = j_CaretPositionInternal = position;
                j_StringSelectPositionInternal = j_StringPositionInternal = GetStringIndexFromCaretPosition(j_CaretSelectPositionInternal);
            }

#if TMP_DEBUG_MODE
                Debug.Log("Caret Position: " + caretPositionInternal + " Selection Position: " + caretSelectPositionInternal + "  String Position: " + stringPositionInternal + " String Select Position: " + stringSelectPositionInternal);
#endif
        }


        private void MovePageUp(bool shift)
        {
            MovePageUp(shift, true);
        }

        private void MovePageUp(bool shift, bool goToFirstChar)
        {
            if (Selected && !shift)
            {
                // If we have a selection and press up without shift,
                // set caret position to start of selection before we move it up.
                j_CaretPositionInternal = j_CaretSelectPositionInternal = Mathf.Min(j_CaretPositionInternal, j_CaretSelectPositionInternal);
            }

            int position = MultiLine ? PageUpCharacterPosition(j_CaretSelectPositionInternal, goToFirstChar) : 0;

            if (shift)
            {
                j_CaretSelectPositionInternal = position;
                j_StringSelectPositionInternal = GetStringIndexFromCaretPosition(j_CaretSelectPositionInternal);
            }
            else
            {
                j_CaretSelectPositionInternal = j_CaretPositionInternal = position;
                j_StringSelectPositionInternal = j_StringPositionInternal = GetStringIndexFromCaretPosition(j_CaretSelectPositionInternal);
            }


            // Scroll to top of viewport
            //int currentLine = m_TextComponent.textInfo.characterInfo[position].lineNumber;
            //float lineAscender = m_TextComponent.textInfo.lineInfo[currentLine].ascender;

            // Adjust text area up or down if not in single line mode.
            if (lineType != LineType.SingleLine)
            {
                float offset = TextViewport.rect.height; // m_TextViewport.rect.yMax - (m_TextComponent.rectTransform.anchoredPosition.y + lineAscender);

                float topTextBounds = textComponent.rectTransform.position.y + textComponent.textBounds.max.y;
                float topViewportBounds = TextViewport.position.y + TextViewport.rect.yMax;

                offset = topViewportBounds > topTextBounds + offset ? offset : topViewportBounds - topTextBounds;

                textComponent.rectTransform.anchoredPosition += new Vector2(0, offset);
                AssignPositioningIfNeeded();
                j_ScrollbarUpdateRequired = true;
            }

#if TMP_DEBUG_MODE
                Debug.Log("Caret Position: " + caretPositionInternal + " Selection Position: " + caretSelectPositionInternal + "  String Position: " + stringPositionInternal + " String Select Position: " + stringSelectPositionInternal);
#endif

        }


        private void MovePageDown(bool shift)
        {
            MovePageDown(shift, true);
        }

        private void MovePageDown(bool shift, bool goToLastChar)
        {
            if (Selected && !shift)
            {
                // If we have a selection and press down without shift,
                // set caret to end of selection before we move it down.
                j_CaretPositionInternal = j_CaretSelectPositionInternal = Mathf.Max(j_CaretPositionInternal, j_CaretSelectPositionInternal);
            }

            int position = MultiLine ? PageDownCharacterPosition(j_CaretSelectPositionInternal, goToLastChar) : textComponent.textInfo.characterCount - 1;

            if (shift)
            {
                j_CaretSelectPositionInternal = position;
                j_StringSelectPositionInternal = GetStringIndexFromCaretPosition(j_CaretSelectPositionInternal);
            }
            else
            {
                j_CaretSelectPositionInternal = j_CaretPositionInternal = position;
                j_StringSelectPositionInternal = j_StringPositionInternal = GetStringIndexFromCaretPosition(j_CaretSelectPositionInternal);
            }

            // Scroll to top of viewport
            //int currentLine = m_TextComponent.textInfo.characterInfo[position].lineNumber;
            //float lineAscender = m_TextComponent.textInfo.lineInfo[currentLine].ascender;

            // Adjust text area up or down if not in single line mode.
            if (lineType != LineType.SingleLine)
            {
                float offset = TextViewport.rect.height; // m_TextViewport.rect.yMax - (m_TextComponent.rectTransform.anchoredPosition.y + lineAscender);

                float bottomTextBounds = textComponent.rectTransform.position.y + textComponent.textBounds.min.y;
                float bottomViewportBounds = TextViewport.position.y + TextViewport.rect.yMin;

                offset = bottomViewportBounds > bottomTextBounds + offset ? offset : bottomViewportBounds - bottomTextBounds;

                textComponent.rectTransform.anchoredPosition += new Vector2(0, offset);
                AssignPositioningIfNeeded();
                j_ScrollbarUpdateRequired = true;
            }

#if TMP_DEBUG_MODE
                Debug.Log("Caret Position: " + caretPositionInternal + " Selection Position: " + caretSelectPositionInternal + "  String Position: " + stringPositionInternal + " String Select Position: " + stringSelectPositionInternal);
#endif

        }

        private void Delete()
        {
            if (readOnly)
                return;

            if (j_StringPositionInternal == j_StringSelectPositionInternal)
                return;

            if (richTextEditingAllowed || j_SelectAll)
            {
                // Handling of Delete when Rich Text is allowed.
                if (j_StringPositionInternal < j_StringSelectPositionInternal)
                {
                    text = Text.Remove(j_StringPositionInternal, j_StringSelectPositionInternal - j_StringPositionInternal);
                    j_StringSelectPositionInternal = j_StringPositionInternal;
                }
                else
                {
                    text = Text.Remove(j_StringSelectPositionInternal, j_StringPositionInternal - j_StringSelectPositionInternal);
                    j_StringPositionInternal = j_StringSelectPositionInternal;
                }

                j_SelectAll = false;
            }
            else
            {
                if (j_CaretPositionInternal < j_CaretSelectPositionInternal)
                {
                    j_StringPositionInternal = textComponent.textInfo.characterInfo[j_CaretPositionInternal].index;
                    j_StringSelectPositionInternal = textComponent.textInfo.characterInfo[j_CaretSelectPositionInternal - 1].index + textComponent.textInfo.characterInfo[j_CaretSelectPositionInternal - 1].stringLength;

                    text = Text.Remove(j_StringPositionInternal, j_StringSelectPositionInternal - j_StringPositionInternal);

                    j_StringSelectPositionInternal = j_StringPositionInternal;
                    j_CaretSelectPositionInternal = j_CaretPositionInternal;
                }
                else
                {
                    j_StringPositionInternal = textComponent.textInfo.characterInfo[j_CaretPositionInternal - 1].index + textComponent.textInfo.characterInfo[j_CaretPositionInternal - 1].stringLength;
                    j_StringSelectPositionInternal = textComponent.textInfo.characterInfo[j_CaretSelectPositionInternal].index;

                    text = Text.Remove(j_StringSelectPositionInternal, j_StringPositionInternal - j_StringSelectPositionInternal);

                    j_StringPositionInternal = j_StringSelectPositionInternal;
                    j_CaretPositionInternal = j_CaretSelectPositionInternal;
                }
            }

#if TMP_DEBUG_MODE
                Debug.Log("Caret Position: " + caretPositionInternal + " Selection Position: " + caretSelectPositionInternal + "  String Position: " + stringPositionInternal + " String Select Position: " + stringSelectPositionInternal);
#endif
        }

        /// <summary>
        /// Handling of DEL key
        /// </summary>
        private void DeleteKey()
        {
            if (readOnly)
                return;

            if (Selected)
            {
                Delete();
                UpdateTouchKeyboardFromEditChanges();
                SendOnValueChangedAndUpdateLabel();
            }
            else
            {
                if (richTextEditingAllowed)
                {
                    if (j_StringPositionInternal < Text.Length)
                    {
                        // Special handling for Surrogate Pairs
                        if (char.IsHighSurrogate(Text[j_StringPositionInternal]))
                            text = Text.Remove(j_StringPositionInternal, 2);
                        else
                            text = Text.Remove(j_StringPositionInternal, 1);

                        UpdateTouchKeyboardFromEditChanges();
                        SendOnValueChangedAndUpdateLabel();
                    }
                }
                else
                {
                    if (j_CaretPositionInternal < textComponent.textInfo.characterCount - 1)
                    {
                        int numberOfCharactersToRemove = textComponent.textInfo.characterInfo[j_CaretPositionInternal].stringLength;

                        // Adjust string position to skip any potential rich text tags.
                        int nextCharacterStringPosition = textComponent.textInfo.characterInfo[j_CaretPositionInternal].index;

                        text = Text.Remove(nextCharacterStringPosition, numberOfCharactersToRemove);

                        SendOnValueChangedAndUpdateLabel();
                    }
                }
            }

#if TMP_DEBUG_MODE
                Debug.Log("Caret Position: " + caretPositionInternal + " Selection Position: " + caretSelectPositionInternal + "  String Position: " + stringPositionInternal + " String Select Position: " + stringSelectPositionInternal);
#endif
        }

        /// <summary>
        /// Handling of Backspace key
        /// </summary>
        private void Backspace()
        {
            if (readOnly)
                return;

            if (Selected)
            {
                Delete();
                UpdateTouchKeyboardFromEditChanges();
                SendOnValueChangedAndUpdateLabel();
            }
            else
            {
                if (richTextEditingAllowed)
                {
                    if (j_StringPositionInternal > 0)
                    {
                        int numberOfCharactersToRemove = 1;

                        // Special handling for Surrogate pairs and Diacritical marks
                        if (char.IsLowSurrogate(Text[j_StringPositionInternal - 1]))
                            numberOfCharactersToRemove = 2;

                        j_StringSelectPositionInternal = j_StringPositionInternal = j_StringPositionInternal - numberOfCharactersToRemove;

                        text = Text.Remove(j_StringPositionInternal, numberOfCharactersToRemove);

                        j_CaretSelectPositionInternal = j_CaretPositionInternal = j_CaretPositionInternal - 1;

                        j_LastKeyBackspace = true;

                        UpdateTouchKeyboardFromEditChanges();
                        SendOnValueChangedAndUpdateLabel();
                    }
                }
                else
                {
                    if (j_CaretPositionInternal > 0)
                    {
                        int numberOfCharactersToRemove = textComponent.textInfo.characterInfo[j_CaretPositionInternal - 1].stringLength;

                        // Delete the previous character
                        text = Text.Remove(textComponent.textInfo.characterInfo[j_CaretPositionInternal - 1].index, numberOfCharactersToRemove);

                        // Get new adjusted string position
                        j_StringSelectPositionInternal = j_StringPositionInternal = j_CaretPositionInternal < 2
                            ? textComponent.textInfo.characterInfo[0].index
                            : textComponent.textInfo.characterInfo[j_CaretPositionInternal - 2].index + textComponent.textInfo.characterInfo[j_CaretPositionInternal - 2].stringLength;

                        j_CaretSelectPositionInternal = j_CaretPositionInternal = j_CaretPositionInternal - 1;
                    }

                    j_LastKeyBackspace = true;

                    UpdateTouchKeyboardFromEditChanges();
                    SendOnValueChangedAndUpdateLabel();
                }

            }

#if TMP_DEBUG_MODE
                Debug.Log("Caret Position: " + caretPositionInternal + " Selection Position: " + caretSelectPositionInternal + "  String Position: " + stringPositionInternal + " String Select Position: " + stringSelectPositionInternal);
#endif
        }



        // Insert the character and update the label.
        private void Insert(char c)
        {
            if (readOnly)
                return;

            string replaceString = c.ToString();
            Delete();

            // Can't go past the character limit
            if (CharacterLimit > 0 && Text.Length >= CharacterLimit)
                return;

            text = Text.Insert(j_StringPosition, replaceString);

            if (!char.IsHighSurrogate(c))
                j_CaretSelectPositionInternal = j_CaretPositionInternal += 1;

            j_StringSelectPositionInternal = j_StringPositionInternal += 1;

            UpdateTouchKeyboardFromEditChanges();
            SendOnValueChanged();

#if TMP_DEBUG_MODE
                Debug.Log("Caret Position: " + caretPositionInternal + " Selection Position: " + caretSelectPositionInternal + "  String Position: " + stringPositionInternal + " String Select Position: " + stringSelectPositionInternal);
#endif
        }

        private void UpdateTouchKeyboardFromEditChanges()
        {
            // Update the TouchKeyboard's text from edit changes
            // if in-place editing is allowed
            if (j_SoftKeyboard != null && InPlaceEditing())
            {
                j_SoftKeyboard.text = text;
            }
        }

        private void SendOnValueChangedAndUpdateLabel()
        {
            if (!InteractableActive())
                return;
            UpdateLabel();
            SendOnValueChanged();
        }

        private void SendOnValueChanged()
        {
            if (!InteractableActive())
                return;
            if (ValueChanged != null)
                valueChanged.Invoke(Text);
        }




        private int GetCaretPositionFromStringIndex(int stringIndex)
        {
            int count = textComponent.textInfo.characterCount;

            for (int i = 0; i < count; i++)
            {
                if (textComponent.textInfo.characterInfo[i].index >= stringIndex)
                    return i;
            }

            return count;
        }

        /// <summary>
        /// Returns / places the caret before the given character at the string index.
        /// </summary>
        /// <param name="stringIndex"></param>
        /// <returns></returns>
        private int GetMinCaretPositionFromStringIndex(int stringIndex)
        {
            int count = textComponent.textInfo.characterCount;

            for (int i = 0; i < count; i++)
            {
                if (stringIndex < textComponent.textInfo.characterInfo[i].index + textComponent.textInfo.characterInfo[i].stringLength)
                    return i;
            }

            return count;
        }

        /// <summary>
        /// Returns / places the caret after the given character at the string index.
        /// </summary>
        /// <param name="stringIndex"></param>
        /// <returns></returns>
        private int GetMaxCaretPositionFromStringIndex(int stringIndex)
        {
            int count = textComponent.textInfo.characterCount;

            for (int i = 0; i < count; i++)
            {
                if (textComponent.textInfo.characterInfo[i].index >= stringIndex)
                    return i;
            }

            return count;
        }

        private int GetStringIndexFromCaretPosition(int caretPosition)
        {
            // Clamp values between 0 and character count.
            ClampCaretPos(ref caretPosition);

            return textComponent.textInfo.characterInfo[caretPosition].index;
        }




        private void MarkGeometryAsDirty()
        {
#if UNITY_EDITOR
#if UNITY_2018_3_OR_NEWER
            if (!Application.isPlaying || UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
                return;
#else
                if (!Application.isPlaying || UnityEditor.PrefabUtility.GetPrefabObject(gameObject) != null)
                    return;
#endif
#endif

            CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
        }



        private void UpdateGeometry()
        {
            if (Application.isEditor && !Application.isPlaying)
                return;

            // No need to draw a cursor on mobile as its handled by the devices keyboard.
            if (InPlaceEditing() == false)
                return;

            if (j_CachedInputRenderer == null)
                return;

            OnFillVBO(j_MeshProp);

            j_CachedInputRenderer.SetMesh(j_MeshProp);
        }


        /// <summary>
        /// Method to keep the Caret RectTransform properties in sync with the text object's RectTransform
        /// </summary>
        private void AssignPositioningIfNeeded()
        {
            if (textComponent != null && j_CaretRectTrans != null &&
                (j_CaretRectTrans.localPosition != textComponent.rectTransform.localPosition ||
                 j_CaretRectTrans.localRotation != textComponent.rectTransform.localRotation ||
                 j_CaretRectTrans.localScale != textComponent.rectTransform.localScale ||
                 j_CaretRectTrans.anchorMin != textComponent.rectTransform.anchorMin ||
                 j_CaretRectTrans.anchorMax != textComponent.rectTransform.anchorMax ||
                 j_CaretRectTrans.anchoredPosition != textComponent.rectTransform.anchoredPosition ||
                 j_CaretRectTrans.sizeDelta != textComponent.rectTransform.sizeDelta ||
                 j_CaretRectTrans.pivot != textComponent.rectTransform.pivot))
            {
                j_CaretRectTrans.localPosition = textComponent.rectTransform.localPosition;
                j_CaretRectTrans.localRotation = textComponent.rectTransform.localRotation;
                j_CaretRectTrans.localScale = textComponent.rectTransform.localScale;
                j_CaretRectTrans.anchorMin = textComponent.rectTransform.anchorMin;
                j_CaretRectTrans.anchorMax = textComponent.rectTransform.anchorMax;
                j_CaretRectTrans.anchoredPosition = textComponent.rectTransform.anchoredPosition;
                j_CaretRectTrans.sizeDelta = textComponent.rectTransform.sizeDelta;
                j_CaretRectTrans.pivot = textComponent.rectTransform.pivot;

                // Get updated world corners of viewport.
                //m_TextViewport.GetLocalCorners(m_ViewportCorners);
            }
        }


        private void OnFillVBO(Mesh vbo)
        {
            using (var helper = new VertexHelper())
            {
                if (!Focused && !j_SelectionStillActive)
                {
                    helper.FillMesh(vbo);
                    return;
                }

                if (j_StringPositionDirty)
                {
                    j_StringPositionInternal = GetStringIndexFromCaretPosition(j_CaretPosition);
                    j_StringSelectPositionInternal = GetStringIndexFromCaretPosition(j_CaretSelectPosition);
                    j_StringPositionDirty = false;
                }

                if (j_CaretPositionDirty)
                {
                    j_CaretPositionInternal = GetCaretPositionFromStringIndex(j_StringPositionInternal);
                    j_CaretSelectPositionInternal = GetCaretPositionFromStringIndex(j_StringSelectPositionInternal);
                    j_CaretPositionDirty = false;
                }
                GenerateCaret(helper, Vector2.zero);

                //if (!Selected && !readOnly)
                //{
                //    GenerateCaret(helper, Vector2.zero);
                //    SendOnEndTextSelection();
                //}
                //else
                //{
                //    GenerateHightlight(helper, Vector2.zero);
                //    SendOnTextSelection();
                //}

                helper.FillMesh(vbo);
            }
        }

        int first = 0;
        private void GenerateCaret(VertexHelper vbo, Vector2 roundingOffset)
        {
            if (!j_CaretVisible)
                return;

            if (j_CursorVerts == null)
            {
                CreateCursorVerts();
            }

            float width = caretWidth;

            // TODO: Optimize to only update the caret position when needed.

            Vector2 startPosition = Vector2.zero;
            float height = 0;
            TMP_CharacterInfo currentCharacter;

            int currentLine = textComponent.textInfo.characterInfo[j_CaretPositionInternal].lineNumber;

            // Caret is positioned at the origin for the first character of each lines and at the advance for subsequent characters.
            if (j_CaretPositionInternal == textComponent.textInfo.lineInfo[currentLine].firstCharacterIndex)
            {
                currentCharacter = textComponent.textInfo.characterInfo[j_CaretPositionInternal];
                startPosition = new Vector2(currentCharacter.origin, currentCharacter.descender);
                height = currentCharacter.ascender - currentCharacter.descender;
            }
            else
            {
                currentCharacter = textComponent.textInfo.characterInfo[j_CaretPositionInternal - 1];
                startPosition = new Vector2(currentCharacter.xAdvance, currentCharacter.descender);
                height = currentCharacter.ascender - currentCharacter.descender;
            }

            if (j_SoftKeyboard != null)
                j_SoftKeyboard.selection = new RangeInt(j_StringPositionInternal, 0);


            // Adjust the position of the RectTransform based on the caret position in the viewport (only if we have focus).
            if (Focused && startPosition != j_LastPosition || j_ForceRectTransformAdjustment)
                AdjustRectTransformRelativeToViewport(startPosition, height, currentCharacter.isVisible);

            j_LastPosition = startPosition;

            // Clamp Caret height
            float top = startPosition.y + height;
            float bottom = top - height;

            // Minor tweak to address caret potentially being too thin based on canvas scaler values.
            float scale = textComponent.canvas.scaleFactor;

            j_CursorVerts[0].position = new Vector3(startPosition.x, bottom, 0.0f);
            j_CursorVerts[1].position = new Vector3(startPosition.x, top, 0.0f);
            j_CursorVerts[2].position = new Vector3(startPosition.x + (width + 1) / scale, top, 0.0f);
            j_CursorVerts[3].position = new Vector3(startPosition.x + (width + 1) / scale, bottom, 0.0f);

            // Set Vertex Color for the caret color.
            j_CursorVerts[0].color = CaretColor;
            j_CursorVerts[1].color = CaretColor;
            j_CursorVerts[2].color = CaretColor;
            j_CursorVerts[3].color = CaretColor;

            vbo.AddUIVertexQuad(j_CursorVerts);

            int screenHeight = Screen.height;
            // Removed multiple display support until it supports none native resolutions(case 741751)
            //int displayIndex = m_TextComponent.canvas.targetDisplay;
            //if (Screen.fullScreen && displayIndex < Display.displays.Length)
            //    screenHeight = Display.displays[displayIndex].renderingHeight;

            startPosition.y = screenHeight - startPosition.y;
            //inputSystem.compositionCursorPos = startPosition;

            //#if TMP_DEBUG_MODE
            //Debug.Log("Caret position updated at frame: " + Time.frameCount);
            //#endif
        }


        private void CreateCursorVerts()
        {
            j_CursorVerts = new UIVertex[4];

            for (int i = 0; i < j_CursorVerts.Length; i++)
            {
                j_CursorVerts[i] = UIVertex.simpleVert;
                j_CursorVerts[i].uv0 = Vector2.zero;
            }
        }


        private void GenerateHightlight(VertexHelper vbo, Vector2 roundingOffset)
        {
            TMP_TextInfo textInfo = textComponent.textInfo;

            j_CaretPositionInternal = j_CaretPosition = GetCaretPositionFromStringIndex(j_StringPositionInternal);
            j_CaretSelectPositionInternal = j_CaretSelectPosition = GetCaretPositionFromStringIndex(j_StringSelectPositionInternal);

            if (j_SoftKeyboard != null)
            {
                int stringPosition = j_CaretPositionInternal < j_CaretSelectPositionInternal ? textInfo.characterInfo[j_CaretPositionInternal].index : textInfo.characterInfo[j_CaretSelectPositionInternal].index;
                int length = j_CaretPositionInternal < j_CaretSelectPositionInternal ? j_StringSelectPositionInternal - stringPosition : j_StringPositionInternal - stringPosition;
                j_SoftKeyboard.selection = new RangeInt(stringPosition, length);
            }

            // Adjust text RectTranform position to make sure it is visible in viewport.
            Vector2 caretPosition;
            float height = 0;
            if (j_CaretSelectPositionInternal < textInfo.characterCount)
            {
                caretPosition = new Vector2(textInfo.characterInfo[j_CaretSelectPositionInternal].origin, textInfo.characterInfo[j_CaretSelectPositionInternal].descender);
                height = textInfo.characterInfo[j_CaretSelectPositionInternal].ascender - textInfo.characterInfo[j_CaretSelectPositionInternal].descender;
            }
            else
            {
                caretPosition = new Vector2(textInfo.characterInfo[j_CaretSelectPositionInternal - 1].xAdvance, textInfo.characterInfo[j_CaretSelectPositionInternal - 1].descender);
                height = textInfo.characterInfo[j_CaretSelectPositionInternal - 1].ascender - textInfo.characterInfo[j_CaretSelectPositionInternal - 1].descender;
            }

            // TODO: Don't adjust the position of the RectTransform if Reset On Deactivation is disabled
            // and we just selected the Input Field again.
            AdjustRectTransformRelativeToViewport(caretPosition, height, true);

            int startChar = Mathf.Max(0, j_CaretPositionInternal);
            int endChar = Mathf.Max(0, j_CaretSelectPositionInternal);

            // Ensure pos is always less then selPos to make the code simpler
            if (startChar > endChar)
            {
                int temp = startChar;
                startChar = endChar;
                endChar = temp;
            }

            endChar -= 1;

            //Debug.Log("Updating Highlight... Caret Position: " + startChar + " Caret Select POS: " + endChar);


            int currentLineIndex = textInfo.characterInfo[startChar].lineNumber;
            int nextLineStartIdx = textInfo.lineInfo[currentLineIndex].lastCharacterIndex;

            UIVertex vert = UIVertex.simpleVert;
            vert.uv0 = Vector2.zero;
            vert.color = SelectionColor;

            int currentChar = startChar;
            while (currentChar <= endChar && currentChar < textInfo.characterCount)
            {
                if (currentChar == nextLineStartIdx || currentChar == endChar)
                {
                    TMP_CharacterInfo startCharInfo = textInfo.characterInfo[startChar];
                    TMP_CharacterInfo endCharInfo = textInfo.characterInfo[currentChar];

                    // Extra check to handle Carriage Return
                    if (currentChar > 0 && endCharInfo.character == 10 && textInfo.characterInfo[currentChar - 1].character == 13)
                        endCharInfo = textInfo.characterInfo[currentChar - 1];

                    Vector2 startPosition = new Vector2(startCharInfo.origin, textInfo.lineInfo[currentLineIndex].ascender);
                    Vector2 endPosition = new Vector2(endCharInfo.xAdvance, textInfo.lineInfo[currentLineIndex].descender);

                    var startIndex = vbo.currentVertCount;
                    vert.position = new Vector3(startPosition.x, endPosition.y, 0.0f);
                    vbo.AddVert(vert);

                    vert.position = new Vector3(endPosition.x, endPosition.y, 0.0f);
                    vbo.AddVert(vert);

                    vert.position = new Vector3(endPosition.x, startPosition.y, 0.0f);
                    vbo.AddVert(vert);

                    vert.position = new Vector3(startPosition.x, startPosition.y, 0.0f);
                    vbo.AddVert(vert);

                    vbo.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
                    vbo.AddTriangle(startIndex + 2, startIndex + 3, startIndex + 0);

                    startChar = currentChar + 1;
                    currentLineIndex++;

                    if (currentLineIndex < textInfo.lineCount)
                        nextLineStartIdx = textInfo.lineInfo[currentLineIndex].lastCharacterIndex;
                }
                currentChar++;
            }

            // Scrollbar should be updated.
            j_ScrollbarUpdateRequired = true;

            //#if TMP_DEBUG_MODE
            //    Debug.Log("Text selection updated at frame: " + Time.frameCount);
            //#endif
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="height"></param>
        /// <param name="isCharVisible"></param>
        private void AdjustRectTransformRelativeToViewport(Vector2 startPosition, float height, bool isCharVisible)
        {
            //Debug.Log("Adjusting transform position relative to viewport.");

            if (TextViewport == null || j_IsDrivenByLayoutComponents)
                return;

            float viewportMin = TextViewport.rect.xMin;
            float viewportMax = TextViewport.rect.xMax;

            //Debug.Log("Viewport Rect: " + viewportMax + "  Start Position: " + startPosition);
            // Adjust the position of the RectTransform based on the caret position in the viewport.
            float rightOffset = viewportMax - (textComponent.rectTransform.anchoredPosition.x + startPosition.x + textComponent.margin.z + caretWidth);
            if (rightOffset < 0f)
            {
                if (!MultiLine || (MultiLine && isCharVisible))
                {
                    //Debug.Log("Shifting text to the right by " + rightOffset.ToString("f3"));
                    textComponent.rectTransform.anchoredPosition += new Vector2(rightOffset, 0);

                    AssignPositioningIfNeeded();
                }
            }

            float leftOffset = (textComponent.rectTransform.anchoredPosition.x + startPosition.x - textComponent.margin.x) - viewportMin;
            if (leftOffset < 0f)
            {
                //Debug.Log("Shifting text to the left by " + leftOffset.ToString("f3"));
                textComponent.rectTransform.anchoredPosition += new Vector2(-leftOffset, 0);
                AssignPositioningIfNeeded();
            }


            // Adjust text area up or down if not in single line mode.
            if (lineType != LineType.SingleLine)
            {
                float topOffset = TextViewport.rect.yMax - (textComponent.rectTransform.anchoredPosition.y + startPosition.y + height);
                if (topOffset < -0.0001f)
                {
                    textComponent.rectTransform.anchoredPosition += new Vector2(0, topOffset);
                    AssignPositioningIfNeeded();
                    j_ScrollbarUpdateRequired = true;
                }

                float bottomOffset = (textComponent.rectTransform.anchoredPosition.y + startPosition.y) - TextViewport.rect.yMin;
                if (bottomOffset < 0f)
                {
                    textComponent.rectTransform.anchoredPosition -= new Vector2(0, bottomOffset);
                    AssignPositioningIfNeeded();
                    j_ScrollbarUpdateRequired = true;
                }
            }

            // Special handling of backspace
            if (j_LastKeyBackspace)
            {
                float firstCharPosition = textComponent.rectTransform.anchoredPosition.x + textComponent.textInfo.characterInfo[0].origin - textComponent.margin.x;
                float lastCharPosition = textComponent.rectTransform.anchoredPosition.x + textComponent.textInfo.characterInfo[textComponent.textInfo.characterCount - 1].origin + textComponent.margin.z;

                // Check if caret is at the left most position of the viewport
                if (textComponent.rectTransform.anchoredPosition.x + startPosition.x <= viewportMin + 0.0001f)
                {
                    if (firstCharPosition < viewportMin)
                    {
                        float offset = Mathf.Min((viewportMax - viewportMin) / 2, viewportMin - firstCharPosition);
                        textComponent.rectTransform.anchoredPosition += new Vector2(offset, 0);
                        AssignPositioningIfNeeded();
                    }
                }
                else if (lastCharPosition < viewportMax && firstCharPosition < viewportMin)
                {
                    float offset = Mathf.Min(viewportMax - lastCharPosition, viewportMin - firstCharPosition);

                    textComponent.rectTransform.anchoredPosition += new Vector2(offset, 0);
                    AssignPositioningIfNeeded();
                }

                j_LastKeyBackspace = false;
            }

            j_ForceRectTransformAdjustment = false;
        }

        private void ActivateInputFieldInternal()
        {
            if (EventSystem.current == null)
                return;

            if (EventSystem.current.currentSelectedGameObject != gameObject)
                EventSystem.current.SetSelectedGameObject(gameObject);

            if (TouchScreenKeyboard.isSupported && HideSoftKeyboard == false)
            {
                //if (inputSystem.touchSupported)
                //{
                //    TouchScreenKeyboard.hideInput = shouldHideMobileInput;
                //}

                if (HideSoftKeyboard == false && readOnly == false && ContentTypeProp != ContentType.Custom)
                {
                    j_SoftKeyboard = (InputTypeProp == InputType.Password) ?
                        TouchScreenKeyboard.Open(text, KeyboardType, false, MultiLine, true, false, "", CharacterLimit) :
                        TouchScreenKeyboard.Open(text, KeyboardType, InputTypeProp == InputType.AutoCorrect, MultiLine, false, false, "", CharacterLimit);

                    if (HideMobileInput == false)
                    {
                        MoveTextEnd(false);
                    }
                    else
                    {
                        OnFocus();

                        // Opening the soft keyboard sets its selection to the end of the text. 
                        // As such, we set the selection to match the Input Field's internal selection.
                        if (j_SoftKeyboard != null)
                        {
                            int length = j_StringPositionInternal < j_StringSelectPositionInternal ? j_StringSelectPositionInternal - j_StringPositionInternal : j_StringPositionInternal - j_StringSelectPositionInternal;
                            j_SoftKeyboard.selection = new RangeInt(j_StringPositionInternal < j_StringSelectPositionInternal ? j_StringPositionInternal : j_StringSelectPositionInternal, length);
                        }
                    }
                }

                // Cache the value of isInPlaceEditingAllowed, because on UWP this involves calling into native code
                // The value only needs to be updated once when the TouchKeyboard is opened.
#if UNITY_2019_1_OR_NEWER
                j_TouchKeyboardAllowsInPlaceEditing = TouchScreenKeyboard.isInPlaceEditingAllowed;
#endif
            }
            else
            {
                //if (!TouchScreenKeyboard.isSupported)
                //    inputSystem.imeCompositionMode = IMECompositionMode.On;

                OnFocus();
            }

            j_AllowInput = true;
            j_OriginalText = Text;
            j_WasCanceled = false;
            SetCaretVisible();
            UpdateLabel();
        }


        /// <summary>
        /// Delegate function for getting keyboard input
        /// </summary>
        /// <param name="newText"></param>
        private void Keyboard_OnTextUpdated(string newText)
        {
            Text = newText;
        }

        /// <summary>
        /// Delegate function for getting keyboard input
        /// </summary>
        /// <param name="sender"></param>
        private void Keyboard_OnClosed(object sender, EventArgs e)
        {
            // Unsubscribe from delegate functions
        }



        //public virtual void OnLostFocus(BaseEventData eventData)
        //{
        //    if (!IsActive() || !IsInteractable())
        //        return;
        //}

        private void EnforceContentType()
        {
            switch (ContentTypeProp)
            {
                case ContentType.Standard:
                    {
                        // Don't enforce line type for this content type.
                        inputType = InputType.Standard;
                        keyboardType = TouchScreenKeyboardType.Default;
                        characterValidation = CharacterValidation.None;
                        break;
                    }
                case ContentType.Autocorrected:
                    {
                        // Don't enforce line type for this content type.
                        inputType = InputType.AutoCorrect;
                        keyboardType = TouchScreenKeyboardType.Default;
                        characterValidation = CharacterValidation.None;
                        break;
                    }
                case ContentType.IntegerNumber:
                    {
                        lineType = LineType.SingleLine;
                        inputType = InputType.Standard;
                        keyboardType = TouchScreenKeyboardType.NumberPad;
                        characterValidation = CharacterValidation.Integer;
                        break;
                    }
                case ContentType.DecimalNumber:
                    {
                        lineType = LineType.SingleLine;
                        inputType = InputType.Standard;
                        keyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
                        characterValidation = CharacterValidation.Decimal;
                        break;
                    }
                case ContentType.Alphanumeric:
                    {
                        lineType = LineType.SingleLine;
                        inputType = InputType.Standard;
                        keyboardType = TouchScreenKeyboardType.ASCIICapable;
                        characterValidation = CharacterValidation.Alphanumeric;
                        break;
                    }
                case ContentType.Name:
                    {
                        lineType = LineType.SingleLine;
                        inputType = InputType.Standard;
                        keyboardType = TouchScreenKeyboardType.Default;
                        characterValidation = CharacterValidation.Name;
                        break;
                    }
                case ContentType.EmailAddress:
                    {
                        lineType = LineType.SingleLine;
                        inputType = InputType.Standard;
                        keyboardType = TouchScreenKeyboardType.EmailAddress;
                        characterValidation = CharacterValidation.EmailAddress;
                        break;
                    }
                case ContentType.Password:
                    {
                        lineType = LineType.SingleLine;
                        inputType = InputType.Password;
                        keyboardType = TouchScreenKeyboardType.Default;
                        characterValidation = CharacterValidation.None;
                        break;
                    }
                case ContentType.Pin:
                    {
                        lineType = LineType.SingleLine;
                        inputType = InputType.Password;
                        keyboardType = TouchScreenKeyboardType.NumberPad;
                        characterValidation = CharacterValidation.Digit;
                        break;
                    }
                default:
                    {
                        // Includes Custom type. Nothing should be enforced.
                        break;
                    }
            }

            SetTextComponentWrapMode();
        }

        #endregion

        #region PROTECTED METHODS

        protected void ClampStringPos(ref int pos)
        {
            if (pos < 0)
                pos = 0;
            else if (pos > Text.Length)
                pos = Text.Length;
        }

        protected void ClampCaretPos(ref int pos)
        {
            if (pos < 0)
                pos = 0;
            else if (pos > textComponent.textInfo.characterCount - 1)
                pos = textComponent.textInfo.characterCount - 1;
        }

        protected EditState KeyPressed(Event evt)
        {
            var currentEventModifiers = evt.modifiers;
            bool ctrl = SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX ? (currentEventModifiers & EventModifiers.Command) != 0 : (currentEventModifiers & EventModifiers.Control) != 0;
            bool shift = (currentEventModifiers & EventModifiers.Shift) != 0;
            bool alt = (currentEventModifiers & EventModifiers.Alt) != 0;
            bool ctrlOnly = ctrl && !alt && !shift;

            switch (evt.keyCode)
            {
                case KeyCode.Backspace:
                    {
                        Backspace();
                        return EditState.Continue;
                    }

                case KeyCode.Delete:
                    {
                        DeleteKey();
                        return EditState.Continue;
                    }

                case KeyCode.Home:
                    {
                        MoveToStartOfLine(shift, ctrl);
                        return EditState.Continue;
                    }

                case KeyCode.End:
                    {
                        MoveToEndOfLine(shift, ctrl);
                        return EditState.Continue;
                    }

                // Select All
                case KeyCode.A:
                    {
                        if (ctrlOnly)
                        {
                            SelectAll();
                            return EditState.Continue;
                        }
                        break;
                    }

                // Copy
                case KeyCode.C:
                    {
                        if (ctrlOnly)
                        {
                            if (InputTypeProp != InputType.Password)
                                j_Clipboard = GetSelectedString();
                            else
                                j_Clipboard = "";
                            return EditState.Continue;
                        }
                        break;
                    }

                // Paste
                case KeyCode.V:
                    {
                        if (ctrlOnly)
                        {
                            Append(j_Clipboard);
                            return EditState.Continue;
                        }
                        break;
                    }

                // Cut
                case KeyCode.X:
                    {
                        if (ctrlOnly)
                        {
                            if (InputTypeProp != InputType.Password)
                                j_Clipboard = GetSelectedString();
                            else
                                j_Clipboard = "";
                            Delete();
                            UpdateTouchKeyboardFromEditChanges();
                            SendOnValueChangedAndUpdateLabel();
                            return EditState.Continue;
                        }
                        break;
                    }

                case KeyCode.LeftArrow:
                    {
                        MoveLeft(shift, ctrl);
                        return EditState.Continue;
                    }

                case KeyCode.RightArrow:
                    {
                        MoveRight(shift, ctrl);
                        return EditState.Continue;
                    }

                case KeyCode.UpArrow:
                    {
                        MoveUp(shift);
                        return EditState.Continue;
                    }

                case KeyCode.DownArrow:
                    {
                        MoveDown(shift);
                        return EditState.Continue;
                    }

                case KeyCode.PageUp:
                    {
                        MovePageUp(shift);
                        return EditState.Continue;
                    }

                case KeyCode.PageDown:
                    {
                        MovePageDown(shift);
                        return EditState.Continue;
                    }

                // Submit
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    {
                        if (LineTypeProp != LineType.MultiLineNewline)
                        {
                            j_ReleaseSelection = true;
                            return EditState.Finish;
                        }
                        break;
                    }

                case KeyCode.Escape:
                    {
                        j_ReleaseSelection = true;
                        j_WasCanceled = true;
                        return EditState.Finish;
                    }
            }

            char c = evt.character;

            // Don't allow return chars or tabulator key to be entered into single line fields.
            if (!MultiLine && (c == '\t' || c == '\r' || c == 10))
                return EditState.Continue;

            // Convert carriage return and end-of-text characters to newline.
            if (c == '\r' || (int)c == 3)
                c = '\n';

            if (IsValidChar(c))
            {
                Append(c);
            }

            if (c == 0)
            {
                if (j_CompositionString.Length > 0)
                {
                    UpdateLabel();
                }
            }
            return EditState.Continue;
        }

        protected virtual bool IsValidChar(char c)
        {
            // Delete key on mac
            if ((int)c == 127)
                return false;
            // Accept newline and tab
            if (c == '\t' || c == '\n')
                return true;

            return true;

            // With the addition of Dynamic support, I think this will best be handled by the text component.
            //return m_TextComponent.font.HasCharacter(c, true);
        }

        /// <summary>
        /// Append the specified text to the end of the current.
        /// </summary>
        protected virtual void Append(string input)
        {
            if (readOnly)
                return;

            if (InPlaceEditing() == false)
                return;

            for (int i = 0, imax = input.Length; i < imax; ++i)
            {
                char c = input[i];

                if (c >= ' ' || c == '\t' || c == '\r' || c == 10 || c == '\n')
                {
                    Append(c);
                }
            }
        }

        protected virtual void Append(char input)
        {
            if (readOnly)
                return;

            if (InPlaceEditing() == false)
                return;

            // If we have an input validator, validate the input first
            if (ValidateInput != null)
                input = ValidateInput(Text, j_StringPositionInternal, input);
            else if (CharacterValidationProp == CharacterValidation.CustomValidator)
            {
                input = Validate(Text, j_StringPositionInternal, input);

                if (input == 0) return;

                SendOnValueChanged();
                UpdateLabel();

                return;
            }
            else if (CharacterValidationProp != CharacterValidation.None)
                input = Validate(Text, j_StringPositionInternal, input);

            // If the input is invalid, skip it
            if (input == 0)
                return;

            // Append the character and update the label
            Insert(input);
        }

        /// <summary>
        /// Submit the input field's text.
        /// </summary>

        public void HandleKeyboardEnterKey()
        {
        }

        public void EditEnd()
        {
            if (!InteractableActive())
                return;

            SendDynamicSubmit(Text);
        }

        public void OnTextChanged()
        {
            if (!InteractableActive())
                return;

            ValueChanged?.Invoke(Text);
            valueChanged?.Invoke(Text);
        }
        public bool isVirtualKeyBoard()
        {
            return IsVirtualKeyBoardConncted();
        }
        
        public void OnDeselect()
        {
            if (!InteractableActive())
                return;

            if (isSelected)
            {
                base.OnSelectClicked(null);
                
                if (IsVirtualKeyBoardConncted())
                {
                    UnSubscribeSoftKeyboardCallbacks();
                }
                if(!isVirtualKeyBoard())
                {
                clearButton.gameObject.SetActive(false);
                }
            }
            DeactivateInputField();
            isFieldSelected = false;
            //base.OnDeselect(eventData);
            SendOnFocusLost();
        }

        protected void SendOnEndEdit()
        {
            if (!InteractableActive())
                return;
        }

        protected void SendOnSubmit()
        {
            if (!InteractableActive())
                return;
            SendDynamicSubmit(Text);
        }
        private void SendDynamicSubmit(string typedText)
        {
            if (submit != null)
            {
                for (int i = 0; i < submit.GetPersistentEventCount(); i++)
                {
                    ((MonoBehaviour)submit.GetPersistentTarget(i)).SendMessage(submit.GetPersistentMethodName(i), typedText);
                }
            }
            Submit?.Invoke(typedText);
        }
        private void SendDynamicValueChange(string typedText)
        {
            if (valueChanged != null)
            {
                for (int i = 0; i < valueChanged.GetPersistentEventCount(); i++)
                {
                    ((MonoBehaviour)valueChanged.GetPersistentTarget(i)).SendMessage(valueChanged.GetPersistentMethodName(i), typedText);
                }
            }
            ValueChanged?.Invoke(typedText);
        }

        protected void SendOnFocus()
        {
            if (!InteractableActive())
                return;
        }

        protected void SendOnFocusLost()
        {
            if (!InteractableActive())
                return;
        }

        protected void SendOnTextSelection()
        {
            if (!InteractableActive())
                return;
            j_Selected = true;

        }

        protected void SendOnEndTextSelection()
        {
            if (!InteractableActive())
                return;
            if (!j_Selected) return;

            j_Selected = false;
        }

        protected void SendTouchScreenKeyboardStatusChanged()
        {
            if (!InteractableActive())
                return;
            if (OnTouchScreenKeyboardStatusChanged != null)
                OnTouchScreenKeyboardStatusChanged.Invoke(j_SoftKeyboard.status);
        }


        /// <summary>
        /// Update the visual text Text.
        /// </summary>

        protected void UpdateLabel()
        {
            if (textComponent != null && textComponent.font != null && j_PreventCallback == false)
            {
                // Prevent callback from the text component as we assign new text. This is to prevent a recursive call.
                j_PreventCallback = true;

                string fullText;
                if (j_CompositionString.Length > 0)
                {
                    fullText = Text.Substring(0, j_StringPosition) + j_CompositionString + Text.Substring(j_StringPosition);

                    // Should adjust caret position
                    //Debug.Log("Handling IME Input... [" + compositionString + "] of length [" + compositionString.Length + "] at StringPosition [" + m_StringPosition + "]");
                    //for (int i = 0; i < compositionString.Length; i++)
                    //    Debug.Log((uint)compositionString[i]);
                }
                else
                {
                    fullText = Text;
                    //Debug.Log("Handling Input... [" + text + "]");
                }

                string processed;
                if (InputTypeProp == InputType.Password)
                    processed = new string(AsteriskChar, fullText.Length);
                else
                    processed = fullText;

                bool isEmpty = string.IsNullOrEmpty(fullText);

                if (placeholder != null)
                    placeholder.enabled = isEmpty;

                if (!isEmpty)
                {
                    SetCaretVisible();
                }

                textComponent.text = processed + "\u200B"; // Extra space is added for Caret tracking.

                // Special handling to limit the number of lines of text in the Input Field.
                if (lineLimit > 0)
                {
                    textComponent.ForceMeshUpdate();

                    // Check if text exceeds maximum number of lines.
                    if (textComponent.textInfo.lineCount > lineLimit)
                    {
                        int lastValidCharacterIndex = textComponent.textInfo.lineInfo[lineLimit - 1].lastCharacterIndex;
                        int characterStringIndex = textComponent.textInfo.characterInfo[lastValidCharacterIndex].index + textComponent.textInfo.characterInfo[lastValidCharacterIndex].stringLength;
                        Text = processed.Remove(characterStringIndex, processed.Length - characterStringIndex);
                        textComponent.text = Text + "\u200B";
                    }
                }

                if (j_TextComponentUpdateRequired)
                {
                    j_TextComponentUpdateRequired = false;
                    textComponent.ForceMeshUpdate();
                }

                MarkGeometryAsDirty();

                // Scrollbar should be updated.
                j_ScrollbarUpdateRequired = true;

                j_PreventCallback = false;
            }
        }

        /// <summary>
        /// Validate the specified input.
        /// </summary>
        protected char Validate(string text, int pos, char ch)
        {
            // Validation is disabled
            if (CharacterValidationProp == CharacterValidation.None || !enabled)
                return ch;

            if (CharacterValidationProp == CharacterValidation.Integer || CharacterValidationProp == CharacterValidation.Decimal)
            {
                // Integer and decimal
                bool cursorBeforeDash = (pos == 0 && text.Length > 0 && text[0] == '-');
                bool selectionAtStart = j_StringPositionInternal == 0 || j_StringSelectPositionInternal == 0;
                if (!cursorBeforeDash)
                {
                    if (ch >= '0' && ch <= '9') return ch;
                    if (ch == '-' && (pos == 0 || selectionAtStart)) return ch;
                    if (ch == '.' && CharacterValidationProp == CharacterValidation.Decimal && !text.Contains(".")) return ch;
                }
            }
            else if (CharacterValidationProp == CharacterValidation.Digit)
            {
                if (ch >= '0' && ch <= '9') return ch;
            }
            else if (CharacterValidationProp == CharacterValidation.Alphanumeric)
            {
                // All alphanumeric characters
                if (ch >= 'A' && ch <= 'Z') return ch;
                if (ch >= 'a' && ch <= 'z') return ch;
                if (ch >= '0' && ch <= '9') return ch;
            }
            else if (CharacterValidationProp == CharacterValidation.Name)
            {
                char lastChar = (text.Length > 0) ? text[Mathf.Clamp(pos, 0, text.Length - 1)] : ' ';
                char nextChar = (text.Length > 0) ? text[Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n';

                if (char.IsLetter(ch))
                {
                    // Space followed by a letter -- make sure it's capitalized
                    if (char.IsLower(ch) && lastChar == ' ')
                        return char.ToUpper(ch);

                    // Uppercase letters are only allowed after spaces (and apostrophes)
                    if (char.IsUpper(ch) && lastChar != ' ' && lastChar != '\'')
                        return char.ToLower(ch);

                    // If character was already in correct case, return it as-is.
                    // Also, letters that are neither upper nor lower case are always allowed.
                    return ch;
                }
                else if (ch == '\'')
                {
                    // Don't allow more than one apostrophe
                    if (lastChar != ' ' && lastChar != '\'' && nextChar != '\'' && !text.Contains("'"))
                        return ch;
                }
                else if (ch == ' ')
                {
                    // Don't allow more than one space in a row
                    if (lastChar != ' ' && lastChar != '\'' && nextChar != ' ' && nextChar != '\'')
                        return ch;
                }
            }
            else if (CharacterValidationProp == CharacterValidation.EmailAddress)
            {
                // From StackOverflow about allowed characters in email addresses:
                // Uppercase and lowercase English letters (a-z, A-Z)
                // Digits 0 to 9
                // Characters ! # $ % & ' * + - / = ? ^ _ ` { | } ~
                // Character . (dot, period, full stop) provided that it is not the first or last character,
                // and provided also that it does not appear two or more times consecutively.

                if (ch >= 'A' && ch <= 'Z') return ch;
                if (ch >= 'a' && ch <= 'z') return ch;
                if (ch >= '0' && ch <= '9') return ch;
                if (ch == '@' && text.IndexOf('@') == -1) return ch;
                if (j_KEmailSpecialCharacters.IndexOf(ch) != -1) return ch;
                if (ch == '.')
                {
                    char lastChar = (text.Length > 0) ? text[Mathf.Clamp(pos, 0, text.Length - 1)] : ' ';
                    char nextChar = (text.Length > 0) ? text[Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n';
                    if (lastChar != '.' && nextChar != '.')
                        return ch;
                }
            }
            else if (CharacterValidationProp == CharacterValidation.Regex)
            {
                // Regex expression
                if (Regex.IsMatch(ch.ToString(), regexValue))
                {
                    return ch;
                }
            }
            else if (CharacterValidationProp == CharacterValidation.CustomValidator)
            {
                if (inputValidator != null)
                {
                    char c = inputValidator.Validate(ref text, ref pos, ch);
                    this.text = text;
                    j_StringSelectPositionInternal = j_StringPositionInternal = pos;
                    return c;
                }
            }
            return (char)0;
        }


        #endregion

        #region IENUMERATORS
        IEnumerator CaretBlink()
        {
            // Always ensure caret is initially visible since it can otherwise be confusing for a moment.
            j_CaretVisible = true;
            yield return null;

            while ((Focused || j_SelectionStillActive) && caretBlinkRate > 0)
            {
                // the blink rate is expressed as a frequency
                float blinkPeriod = 1f / caretBlinkRate;

                // the caret should be ON if we are in the first half of the blink period
                bool blinkState = (Time.unscaledTime - j_BlinkStartTime) % blinkPeriod < blinkPeriod / 2;
                if (j_CaretVisible != blinkState)
                {
                    j_CaretVisible = blinkState;
                    //if (!Selected)
                    MarkGeometryAsDirty();
                }

                // Then wait again.
                yield return null;
            }
            j_BlinkCoroutine = null;
        }

        IEnumerator MouseDragOutsideRect(PointerEventData eventData)
        {
            while (j_UpdateDrag && j_DragPositionOutOfBounds)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(TextViewport, eventData.position, eventData.pressEventCamera, out Vector2 localMousePos);

                Rect rect = TextViewport.rect;

                if (MultiLine)
                {
                    if (localMousePos.y > rect.yMax)
                        MoveUp(true, true);
                    else if (localMousePos.y < rect.yMin)
                        MoveDown(true, true);
                }
                else
                {
                    if (localMousePos.x < rect.xMin)
                        MoveLeft(true, false);
                    else if (localMousePos.x > rect.xMax)
                        MoveRight(true, false);
                }

                UpdateLabel();

                float delay = MultiLine ? j_KVScrollSpeed : j_KHScrollSpeed;

                if (j_WaitForSecondsRealtime == null)
                    j_WaitForSecondsRealtime = new WaitForSecondsRealtime(delay);
                else
                    j_WaitForSecondsRealtime.waitTime = delay;

                yield return j_WaitForSecondsRealtime;
            }
            j_DragCoroutine = null;
        }

        public bool isMultiLineSupported()
        {
            return supportMultiLine;
        }
        #endregion

        Transform ICanvasElement.transform => null;
    }
}