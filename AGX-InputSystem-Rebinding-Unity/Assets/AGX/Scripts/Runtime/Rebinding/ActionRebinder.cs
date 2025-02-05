using System;
using System.Text;
using AGX.Scripts.Runtime.Icons;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AGX.Scripts.Runtime.Rebinder
{
    /// <summary>
    /// Allows rebinding an action to a new input.
    /// </summary>
    public class ActionRebinder : MonoBehaviour
    {
        public string ActionName => _actionName;
        public int BindingStartIndexIndex => _selectedBindingStartIndex;

        [BoxGroup("References"), SerializeField, Required] private ActionRebinders _actionRebinders;
        [BoxGroup("References"), SerializeField, Required] private TMP_Text        _textRebind;
        [BoxGroup("References"), SerializeField, Required] private Button          _buttonReset;
        [BoxGroup("References"), SerializeField, Required] private Button          _buttonRebind;
        [BoxGroup("References"), SerializeField]           private bool            _mouseIncluded;

        [BoxGroup("References"), SerializeField, OnValueChanged(nameof(UpdateButtonBasedOnCanBeRebinded))] private bool _canBeRebinded = true;

        private bool _debug = false;

        /// <summary>
        /// The selected binding index.
        /// To get the first binding use index 0.
        /// Usually, the second binding will be at index 1, but if the first binding is a composite of 2 controls, the next binding will be at index 5.
        /// </summary>
        [BoxGroup("References"), SerializeField, ReadOnly] private int _selectedBindingStartIndex;

        [BoxGroup("Debug"), ShowInInspector, ReadOnly] private string _actionName;
        [BoxGroup("Debug"), ShowInInspector, ReadOnly] private bool   _isDirty;


        internal void SetBindingIndex(int index)
        {
            _selectedBindingStartIndex = index;
            UpdateUI();
            name = $"Input Action Rebinder ({_actionName}:{_selectedBindingStartIndex})";
        }

        private void UpdateButtonBasedOnCanBeRebinded()
        {
            _buttonRebind.interactable = _canBeRebinded;
            _buttonReset.interactable = _canBeRebinded;
        }

        private void Awake()
        {
            UpdateButtonBasedOnCanBeRebinded();

            _buttonRebind.onClick.AddListener(DoRebind);
            _buttonReset.onClick.AddListener(ResetBinding);

            name = $"Input Action Rebinder ({_actionName}:{_selectedBindingStartIndex})";

            GetBindingInfo();
            UpdateUI();


            InputManager.RegisterRebind(this);

            InputManager.RebindComplete += UpdateUI;
            InputManager.RebindCanceled += UpdateUI;
        }


        private void OnDestroy()
        {
            InputManager.RebindComplete -= UpdateUI;
            InputManager.RebindCanceled -= UpdateUI;
        }


        private void GetBindingInfo()
        {
            if (_actionRebinders == null)
            {
                Debug.LogError("Action rebinders is null!");
                return;
            }

            if (_actionRebinders.InputAction == null)
            {
                Debug.LogError("Action rebinders input action reference is null!");
                return;
            }

            _actionName = _actionRebinders.InputAction.name;
        }

        internal void UpdateUI()
        {
            var startIndex = _selectedBindingStartIndex;

            if (string.IsNullOrEmpty(_actionName))
            {
                GetBindingInfo();
            }

            var action = InputManager.GetAction(_actionName);
            var bindings = InputManager.GetBindings(_actionName);

            var endIndex = startIndex;

            if (_debug)
                for (var i = 0; i < bindings.Count; i++)
                    Debug.Log($"Binding {i}: {bindings[i].name} - {bindings[i].effectivePath} - {bindings[i].isComposite} - {bindings[i].isPartOfComposite}");


            if (startIndex < 0)
            {
                Debug.LogError($"Invalid binding index: {startIndex}, Action: {action.name}");
                return;
            }

            if (startIndex >= bindings.Count)
            {
                Debug.LogError($"Binding index out of range: {startIndex} out of {bindings.Count}, Action: {action.name}");
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
                Debug.Log($"Start Index: {startIndex} End Index: {endIndex} for {action.bindings.Count} bindings of {_actionRebinders.InputAction.name}");

            var fullString = new StringBuilder();

            for (var i = startIndex; i <= endIndex; i++)
            {
                var inputBinding = action.bindings[i];

                // ensure this input binding is not the start of a composite
                if (inputBinding.isComposite)
                    continue;


                var effectivePathParsed = inputBinding.effectivePath;


                // remove < and >
                effectivePathParsed = effectivePathParsed.Replace("<", "").Replace(">", "");

                // add / at the beginning
                if (!effectivePathParsed.StartsWith("/"))
                    effectivePathParsed = "/" + effectivePathParsed;

                if (effectivePathParsed != "/")
                {
                    var actualSpritePath = SpriteMapReference.GetSprite(effectivePathParsed);

                    if (_debug)
                        Debug.Log($"Binding {i}: name {inputBinding.name} - path {inputBinding.path} - effective path {inputBinding.effectivePath} - sprite {actualSpritePath}");

                    fullString.Append(actualSpritePath);
                }
                else
                {
                    fullString.Append("");
                }
            }

            /* IF ANYTHING GOES WRONG
             *   _textRebind.text = Application.isPlaying
                    // From the input manager
                    ? InputManager.GetBindingName(_actionName, _bindingIndex)
                    : action.GetBindingDisplayString(_bindingIndex); // From the input action reference
             */

            var val = fullString.ToString();


            _textRebind.text = val;

            _isDirty = InputManager.IsBindingOverriden(_actionName, _selectedBindingStartIndex);
            _buttonReset.gameObject.SetActive(_isDirty);
        }

        private void DoRebind()
        {
            InputManager.StartRebind(_actionName, _selectedBindingStartIndex, _actionRebinders.RebindingOverlay, _mouseIncluded);
        }

        private void ResetBinding()
        {
            InputManager.ResetBinding(this);
            UpdateUI();
        }
    }
}