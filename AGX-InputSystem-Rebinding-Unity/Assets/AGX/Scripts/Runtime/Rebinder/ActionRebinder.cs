using System.Text;
using AGX.Scripts.Runtime.Prompts;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AGX.Scripts.Runtime.Rebinder
{
    /// <summary>
    /// Allows rebinding an action to a new key/button/mouse input.
    /// </summary>
    public class ActionRebinder : MonoBehaviour
    {
        public string ActionName => _actionName;
        public int BindingIndex => _selectedBinding;

        [BoxGroup("References"), SerializeField, Required] private ActionRebinders _actionRebinders;
        [BoxGroup("References"), SerializeField, Required] private TMP_Text        _textRebind;
        [BoxGroup("References"), SerializeField, Required] private Button          _buttonReset;
        [BoxGroup("References"), SerializeField, Required] private Button          _buttonRebind;
        [BoxGroup("References"), SerializeField]           private bool            _mouseIncluded;
        [BoxGroup("References"), SerializeField]           private bool            _canBeRebinded = true;

        [SerializeField] private bool _debug;

        /// <summary>
        /// The selected binding index.
        /// To get the first binding use index 0.
        /// Usually, the second binding will be at index 1, but if the first binding is a composite of 2 controls, the next binding will be at index 5.
        /// </summary>
        [BoxGroup("References"), SerializeField] private int _selectedBinding;

        [ShowNonSerializedField] private string _actionName;
        [ShowNonSerializedField] private bool   _isDirty;

        private void OnEnable()
        {
            if (!_canBeRebinded)
            {
                _buttonRebind.interactable = false;
                _buttonReset.interactable = false;

                _buttonRebind.gameObject.SetActive(false);
            }
            else
            {
                _buttonRebind.onClick.AddListener(DoRebind);
                _buttonReset.onClick.AddListener(ResetBinding);
            }

            if (_actionRebinders.InputActionReference != null)
            {
                GetBindingInfo();
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
            GetBindingInfo();
            UpdateUI();

            name = $"Input Action Rebinder ({_actionName}:{_selectedBinding})";
        }

        private void GetBindingInfo()
        {
            if (_actionRebinders == null)
            {
                Debug.LogError("Action rebinders is null!");
                return;
            }

            if (_actionRebinders.InputActionReference == null)
            {
                Debug.LogError("Action rebinders input action reference is null!");
                return;
            }

            _actionName = _actionRebinders.InputActionReference.action.name;
        }

        [Button]
        internal void UpdateUI()
        {
            var startIndex = _selectedBinding;

            if (string.IsNullOrEmpty(_actionName))
            {
                GetBindingInfo();
            }
            
            _buttonRebind.gameObject.SetActive(_canBeRebinded);

            var action = InputManager.GetAction(_actionName);
            var bindings = InputManager.GetBindings(_actionName);

            var endIndex = startIndex;

            if (_debug)
                for (var i = 0; i < bindings.Count; i++)
                    Debug.Log($"Binding {i}: {bindings[i].name} - {bindings[i].effectivePath} - {bindings[i].isComposite} - {bindings[i].isPartOfComposite}");

            if (startIndex < 0 || startIndex >= bindings.Count)
            {
                Debug.LogError($"Invalid binding index: {startIndex}, Action: {action.name}");
                return;
            }

            var isRootOfComposite = bindings[startIndex].isComposite && !bindings[startIndex].isPartOfComposite;

            if (!isRootOfComposite)
            {
                if (_debug)
                    Debug.Log($"Binding at index {startIndex} for {action.name} is not the root of a composite \n" +
                              $"Binding: {bindings[startIndex].name}\n" +
                              $"Path: {bindings[startIndex].path}\n" +
                              $"Is Composite: {bindings[startIndex].isComposite}\n" +
                              $"Is Part of Composite: {bindings[startIndex].isPartOfComposite}");
            }
            else
            {
                for (var i = startIndex + 1; i < bindings.Count; i++)
                {
                    var isNewComposite = bindings[i].isComposite && !bindings[i].isPartOfComposite;

                    // We found the start of a new composite, stop processing
                    if (isNewComposite)
                        break;

                    if (bindings[i].isPartOfComposite)
                        endIndex = i;
                }
            }

            if (_debug)
                Debug.Log($"Start Index: {startIndex} End Index: {endIndex} for {action.bindings.Count} bindings of {_actionRebinders.InputActionReference.action.name}");

            var fullString = new StringBuilder();

            for (var i = startIndex; i <= endIndex; i++)
            {
                var b = action.bindings[i];

                // ensure this input binding is not the start of a composite
                if (b.isComposite)
                    continue;

                // get the action as a string like '/Keyboard/anyKey'
                var tmpSprite = InputDevicePrompts.GetSprite(b.effectivePath);

                if (_debug)
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

            _isDirty = InputManager.IsBindingOverriden(_actionName, _selectedBinding);
            _buttonReset.gameObject.SetActive(_isDirty);
        }

        private void DoRebind()
        {
            InputManager.StartRebind(_actionName, _selectedBinding, _actionRebinders.RebindingOverlay, _mouseIncluded);
        }

        private void ResetBinding()
        {
            InputManager.ResetBinding(this);
            UpdateUI();
        }
    }
}