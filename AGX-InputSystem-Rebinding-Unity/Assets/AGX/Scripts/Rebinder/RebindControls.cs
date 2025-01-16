using AGX.Scripts.Searching;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AGX.Scripts.Rebinder
{
    public class RebindControls : MonoBehaviour, ISearchable
    {
        [BoxGroup("References"), SerializeField]              private InputActionReference              _inputActionReference;
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
        [ShowNonSerializedField] private int    selectBinding;
        [ShowNonSerializedField] private string actionName;

        [ShowNonSerializedField] private bool _isDirty;
        public string ActionName => actionName;
        public int BindingIndex => bindingIndex;

        private void OnEnable()
        {
            _rebindButton.onClick.AddListener(DoRebind);
            _resetButton.onClick.AddListener(ResetBinding);

            if (_inputActionReference != null)
            {
                InputManager.LoadBindingOverride(actionName);
                GetBindingInfo(selectBinding);
                UpdateUI();
            }

            InputManager.RebindComplete += UpdateUI;
            InputManager.RebindCanceled += UpdateUI;
        }

        private void OnDestroy()
        {
            InputManager.RebindComplete -= UpdateUI;
            InputManager.RebindCanceled -= UpdateUI;
        }

        private void OnValidate()
        {
            if (_inputActionReference == null)
                return;

            GetBindingInfo(selectBinding);
            UpdateUI();

            name = $"Input Action Rebinder ({actionName})";
        }

        private void GetBindingInfo(int selectBinding)
        {
            if (_inputActionReference.action != null)
                actionName = _inputActionReference.action.name;

            if (_inputActionReference.action.bindings.Count > _selectedBinding)
            {
                _inputBinding = _inputActionReference.action.bindings[_selectedBinding = selectBinding];
                bindingIndex = _selectedBinding;
            }
        }

        internal void UpdateUI()
        {
            Debug.Log($"Updating UI for {actionName} binding {bindingIndex}");

            if (_actionText != null)
                _actionText.text = actionName;

            if (_rebindText == null) return;

            if (Application.isPlaying)
                _rebindText.text = InputManager.GetBindingName(actionName, bindingIndex);
            else
                _rebindText.text = _inputActionReference.action.GetBindingDisplayString(bindingIndex);

            _isDirty = InputManager.IsBindingChanged(actionName, bindingIndex);
            _resetButton.gameObject.SetActive(_isDirty);

            Debug.Log($"IsDirty: {_isDirty}");
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