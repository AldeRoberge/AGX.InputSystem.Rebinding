using AGX.Scripts.Runtime.Searching;
using InputSystemActionPrompts.Runtime;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AGX.Scripts.Runtime.Rebinder
{
    /// <summary>
    /// Allows rebinding an action to a new key/button/mouse input.
    /// </summary>
    public class ActionRebinder : MonoBehaviour, ISearchable
    {
        [BoxGroup("References"), SerializeField, Required] private InputActionReference              _inputActionReference;
        [BoxGroup("References"), SerializeField, Required] private ActionIconMap                     _actionIconMap;
        [BoxGroup("References"), SerializeField]           private bool                              _mouseIncluded;
        [BoxGroup("References"), SerializeField]           private InputBinding.DisplayStringOptions _displayStringOptions;

        [BoxGroup("References/UI"), SerializeField, Required] private TMP_Text      _actionText;
        [BoxGroup("References/UI"), SerializeField, Required] private Button        _buttonRebind;
        [BoxGroup("References/UI"), SerializeField, Required] private RebindOverlay _rebindOverlay;

        [BoxGroup("References"), SerializeField, Range(0, 5)] private int _selectedBinding;

        [BoxGroup("References/UI"), SerializeField, Required] private TMP_Text _textRebind;
        [BoxGroup("References/UI"), SerializeField, Required] private Button   _buttonReset;

        [ShowNonSerializedField] private InputBinding _inputBinding;

        [ShowNonSerializedField] private int    _bindingIndex;
        [ShowNonSerializedField] private string _actionName;

        [ShowNonSerializedField] private bool _isDirty;
        public string ActionName => _actionName;
        public int BindingIndex => _bindingIndex;

        private void OnEnable()
        {
            _buttonRebind.onClick.AddListener(DoRebind);
            _buttonReset.onClick.AddListener(ResetBinding);

            InputDevicePromptSystem.OnInitialized += UpdateUI;

            if (_inputActionReference != null)
            {
                GetBindingInfo(_selectedBinding);
                UpdateUI();
            }

            InputManager.RegisterRebind(this);

            InputManager.RebindComplete += UpdateUI;
            InputManager.RebindCanceled += UpdateUI;
        }

        private void OnDestroy()
        {
            InputManager.RebindComplete -= UpdateUI;
            InputManager.RebindCanceled -= UpdateUI;

            InputDevicePromptSystem.OnInitialized -= UpdateUI;
        }

        private void OnValidate()
        {
            GetBindingInfo(_selectedBinding);
            UpdateUI();

            name = $"Input Action Rebinder ({_actionName}:{_bindingIndex})";
        }

        private void GetBindingInfo(int selectedBinding)
        {
            _actionName = _inputActionReference.action.name;

            if (_inputActionReference.action.bindings.Count > _selectedBinding)
            {
                _selectedBinding = selectedBinding;
                _inputBinding = _inputActionReference.action.bindings[_selectedBinding];
                _bindingIndex = _selectedBinding;
            }
        }

        internal void UpdateUI()
        {
            _actionText.text = _actionIconMap.GetFor(_inputActionReference);

            // [Map/Action] i.e. [Player/Move]
            var txt = $"[{_inputActionReference.action.actionMap.name}/{_inputActionReference.action.name}]";
            // Debug.Log(txt);


            // Get the range of the binding index for the current composite group
            var startIndex = _bindingIndex; // Assume this is the index of the composite's root binding

// Get all bindings for the action
            var bindings = _inputActionReference.action.bindings;

            // Initialize the endIndex to the start index
            var endIndex = startIndex;

   
            /*
             for (int i = 0; i < bindings.Count; i++)
                Debug.Log($"Binding {i}: {bindings[i].name} - {bindings[i].path} - {bindings[i].isComposite} - {bindings[i].isPartOfComposite}");
            */

// Validate startIndex
            if (startIndex < 0 || startIndex >= bindings.Count)
            {
                Debug.LogError($"Invalid binding index: {startIndex}, Action: {_inputActionReference.action.name}");
            }

            bool isRootOfComposite = bindings[startIndex].isComposite && !bindings[startIndex].isPartOfComposite;


            if (!isRootOfComposite)
            {
                Debug.LogError($"Binding at index {startIndex} for {_inputActionReference.action.name} is not the root of a composite \n" +
                               $"Binding: {bindings[startIndex].name}\n" +
                               $"Path: {bindings[startIndex].path}\n" +
                               $"Is Composite: {bindings[startIndex].isComposite}\n" +
                               $"Is Part of Composite: {bindings[startIndex].isPartOfComposite}");
            }
            else
            {
                for (int i = startIndex + 1; i < bindings.Count; i++)
                {
                    bool isNewComposite = bindings[i].isComposite && !bindings[i].isPartOfComposite;

                    // We found the start of a new composite, stop processing
                    if (isNewComposite)
                    {
                        break;
                    }

                    if (bindings[i].isPartOfComposite)
                    {
                        endIndex = i;
                    }
                }
            }

// Log the start and end indices for debugging
            Debug.Log($"Composite Range: Start Index: {startIndex}, End Index: {endIndex}, Action: {_inputActionReference.action.name}");

            Debug.Log($"Start Index: {startIndex} End Index: {endIndex} for {_inputActionReference.action.bindings.Count} bindings of {_inputActionReference.action.name}");

            var result = InputDevicePromptSystem.InsertPromptSprites(txt, _bindingIndex, endIndex);

            if (result.Contains(InputDevicePromptSystem.MISSING_PROMPT) ||
                result.Contains(InputDevicePromptSystem.WAITING_FOR_INITIALIZATION))
            {
                _textRebind.text = Application.isPlaying
                    // From the input manager
                    ? InputManager.GetBindingName(_actionName, _bindingIndex)
                    : _inputActionReference.action.GetBindingDisplayString(_bindingIndex); // From the input action reference
            }
            else
            {
                _textRebind.text = result;
            }

            _isDirty = InputManager.IsBindingOverriden(_actionName, _bindingIndex);
            _buttonReset.gameObject.SetActive(_isDirty);
        }

        private void DoRebind()
        {
            InputManager.StartRebind(_actionName, _bindingIndex, _rebindOverlay, !_mouseIncluded);
        }

        private void ResetBinding()
        {
            InputManager.ResetBinding(this);
            UpdateUI();
        }

        [Button]
        public void SelectKeyboardAndMouseBindingInfo()
        {
            GetBindingInfo(0); // Change number to K&M binding if not 0
            UpdateUI();
        }

        [Button]
        public void SelectControllerBindingInfo()
        {
            GetBindingInfo(1); // Change number to Controller binding if not 1
            UpdateUI();
        }

        [Button]
        public void ContollerVectorBindingInfo()
        {
            GetBindingInfo(5); // Change number to controller move binding
            UpdateUI();
        }

        public void DoReset()
        {
            ResetBinding();
        }

        public string[] SearchKeywords => new[]
        {
            _actionName
        };
    }
}