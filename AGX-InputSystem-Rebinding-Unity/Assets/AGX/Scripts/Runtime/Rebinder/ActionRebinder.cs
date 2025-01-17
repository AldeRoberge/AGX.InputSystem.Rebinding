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
        [BoxGroup("References"), SerializeField, Required]    private InputActionReference              _inputActionReference;
        [BoxGroup("References"), SerializeField, Required]    private ActionIconMap                     _actionIconMap;
        [BoxGroup("References"), SerializeField]              private bool                              _mouseIncluded;
        [BoxGroup("References"), SerializeField]              private InputBinding.DisplayStringOptions _displayStringOptions;

        [BoxGroup("References/UI"), SerializeField, Required] private TMP_Text      _actionText;
        [BoxGroup("References/UI"), SerializeField, Required] private Button        _buttonRebind;
        [BoxGroup("References/UI"), SerializeField, Required] private RebindOverlay _rebindOverlay;


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
            if (_inputActionReference == null)
                return;

            GetBindingInfo(_selectedBinding);
            UpdateUI();

            name = $"Input Action Rebinder ({_actionName})";
        }

        private void GetBindingInfo(int selectedBinding)
        {
            if (_inputActionReference.action != null)
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
            if (_actionText != null)
                _actionText.text = _actionIconMap.GetFor(_inputActionReference);

            if (_textRebind == null) return;

            // [Map/Action] i.e. [Player/Move]
            var txt = $"[{_inputActionReference.action.actionMap.name}/{_inputActionReference.action.name}]";

            // Debug.Log($"Updating UI for {actionName} binding {bindingIndex} with text {txt}");

            var result = InputDevicePromptSystem.InsertPromptSprites(txt);

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

            _isDirty = InputManager.IsBindingChanged(_actionName, _bindingIndex);
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

        public void SelectKeyboardAndMouseBindingInfo()
        {
            GetBindingInfo(0); // Change number to K&M binding if not 0
            UpdateUI();
        }

        public void SelectControllerBindingInfo()
        {
            GetBindingInfo(1); // Change number to Controller binding if not 1
            UpdateUI();
        }

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