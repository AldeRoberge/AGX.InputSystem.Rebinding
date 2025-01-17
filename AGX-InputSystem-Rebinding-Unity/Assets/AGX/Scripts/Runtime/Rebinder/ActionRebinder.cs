using System;
using System.Text;
using AGX.Scripts.Runtime.Prompts;
using AGX.Scripts.Runtime.Searching;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

namespace AGX.Scripts.Runtime.Rebinder
{
    /// <summary>
    /// Allows rebinding an action to a new key/button/mouse input.
    /// </summary>
    public class ActionRebinder : MonoBehaviour, ISearchable
    {
        [BoxGroup("References"), SerializeField] private ActionRebinders _actionRebinders;

        [BoxGroup("References"), SerializeField] private bool                              _mouseIncluded;
        [BoxGroup("References"), SerializeField] private InputBinding.DisplayStringOptions _displayStringOptions;


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

            if (_actionRebinders.InputActionReference != null)
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
        }

        private void OnValidate()
        {
            GetBindingInfo(_selectedBinding);
            UpdateUI();

            name = $"Input Action Rebinder ({_actionName}:{_bindingIndex})";
        }

        private void GetBindingInfo(int selectedBinding)
        {
            _actionName = _actionRebinders.InputActionReference.action.name;

            if (_actionRebinders.InputActionReference.action.bindings.Count > _selectedBinding)
            {
                _selectedBinding = selectedBinding;
                _inputBinding = _actionRebinders.InputActionReference.action.bindings[_selectedBinding];
                _bindingIndex = _selectedBinding;
            }
        }

        [Button]
        internal void UpdateUI()
        {
            var startIndex = _bindingIndex;


            var action = InputManager.GetAction(_actionName);
            var bindings = InputManager.GetBindings(_actionName);

            var endIndex = startIndex;

            for (int i = 0; i < bindings.Count; i++)
                Debug.Log($"Binding {i}: {bindings[i].name} - {bindings[i].effectivePath} - {bindings[i].isComposite} - {bindings[i].isPartOfComposite}");

            if (startIndex < 0 || startIndex >= bindings.Count)
                Debug.LogError($"Invalid binding index: {startIndex}, Action: {action.name}");

            bool isRootOfComposite = bindings[startIndex].isComposite && !bindings[startIndex].isPartOfComposite;

            if (!isRootOfComposite)
            {
                Debug.Log($"Binding at index {startIndex} for {action.name} is not the root of a composite \n" +
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
                        break;

                    if (bindings[i].isPartOfComposite)
                        endIndex = i;
                }
            }

            Debug.Log($"Start Index: {startIndex} End Index: {endIndex} for {action.bindings.Count} bindings of {_actionRebinders.InputActionReference.action.name}");

            StringBuilder fullString = new StringBuilder();

            for (int i = startIndex; i <= endIndex; i++)
            {
                var b = action.bindings[i];

                // ensure this input binding is not the start of a composite
                if (b.isComposite)
                    continue;

                // get the action as a string like '/Keyboard/anyKey'
                var tmpSprite = InputDevicePrompts.GetSprite(b.effectivePath);

                Debug.Log($"Binding {i}: name {b.name} - path {b.path} - effective path {b.effectivePath} - sprite {tmpSprite}");

                fullString.Append(tmpSprite);
            }

            /* IF ANYTHING GOES WRONG
             *   _textRebind.text = Application.isPlaying
                    // From the input manager
                    ? InputManager.GetBindingName(_actionName, _bindingIndex)
                    : action.GetBindingDisplayString(_bindingIndex); // From the input action reference
             */

            _textRebind.text = fullString.ToString();

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