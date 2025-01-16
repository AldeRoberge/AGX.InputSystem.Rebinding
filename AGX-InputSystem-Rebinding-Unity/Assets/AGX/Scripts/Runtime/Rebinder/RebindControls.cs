using AGX.Scripts.Runtime.Searching;
using InputSystemActionPrompts;
using InputSystemActionPrompts.Runtime;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AGX.Scripts.Runtime.Rebinder
{
    public class RebindControls : MonoBehaviour, ISearchable
    {
        [BoxGroup("References"), SerializeField]              private InputActionReference              _inputActionReference;
        [BoxGroup("References"), SerializeField]              private ActionIconMap                     _inputActionMap;
        [BoxGroup("References"), SerializeField]              private bool                              _mouseIncluded;
        [BoxGroup("References"), SerializeField, Range(0, 5)] private int                               _selectedBinding;
        [BoxGroup("References"), SerializeField]              private InputBinding.DisplayStringOptions _displayStringOptions;
        [BoxGroup("References"), SerializeField, ReadOnly]    private InputBinding                      _inputBinding;

        [BoxGroup("References/UI"), SerializeField, Required] private TMP_Text      _actionText;
        [BoxGroup("References/UI"), SerializeField, Required] private Button        _rebindButton;
        [BoxGroup("References/UI"), SerializeField, Required] private RebindOverlay _rebindOverlay;
        [BoxGroup("References/UI"), SerializeField, Required] private TMP_Text      _rebindText;
        [BoxGroup("References/UI"), SerializeField, Required] private Button        _resetButton;

        [ShowNonSerializedField] private int    bindingIndex;
        [ShowNonSerializedField] private string actionName;

        [ShowNonSerializedField] private bool _isDirty;
        public string ActionName => actionName;
        public int BindingIndex => bindingIndex;

        private void OnEnable()
        {
            _rebindButton.onClick.AddListener(DoRebind);
            _resetButton.onClick.AddListener(ResetBinding);

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

            name = $"Input Action Rebinder ({actionName})";
        }

        private void GetBindingInfo(int selectedBinding)
        {
            if (_inputActionReference.action != null)
                actionName = _inputActionReference.action.name;

            if (_inputActionReference.action.bindings.Count > _selectedBinding)
            {
                _selectedBinding = selectedBinding;
                _inputBinding = _inputActionReference.action.bindings[_selectedBinding];
                bindingIndex = _selectedBinding;
            }
        }

        internal void UpdateUI()
        {
            if (_actionText != null)
                _actionText.text = _inputActionMap.GetFor(_inputActionReference);

            if (_rebindText == null) return;

            // [Map/Action] i.e. [Player/Move]
            var txt = $"[{_inputActionReference.action.actionMap.name}/{_inputActionReference.action.name}]";

            // Debug.Log($"Updating UI for {actionName} binding {bindingIndex} with text {txt}");

            var result = InputDevicePromptSystem.InsertPromptSprites(txt);

            if (result.Contains(InputDevicePromptSystem.MISSING_PROMPT) ||
                result.Contains(InputDevicePromptSystem.WAITING_FOR_INITIALIZATION))
            {
                _rebindText.text = Application.isPlaying
                    // From the input manager
                    ? InputManager.GetBindingName(actionName, bindingIndex)
                    : _inputActionReference.action.GetBindingDisplayString(bindingIndex); // From the input action reference
            }
            else
            {
                _rebindText.text = result;
            }

            _isDirty = InputManager.IsBindingChanged(actionName, bindingIndex);
            _resetButton.gameObject.SetActive(_isDirty);
        }

        private void DoRebind()
        {
            InputManager.StartRebind(actionName, bindingIndex, _rebindOverlay, !_mouseIncluded);
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
            actionName
        };
    }
}