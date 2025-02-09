using System;
using System.Collections.Generic;
using AGX.Scripts.Runtime.Icons;
using AGX.Scripts.Runtime.Searching;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Input.Rebinding.Scripts.Runtime.Rebinding
{
    public class ActionRebinders : MonoBehaviour, ISearchable
    {
        [BoxGroup("Settings"), SerializeField, Required, OnValueChanged(nameof(Wait1FrameAndUpdateUI))] private InputActionReference _inputAction;

        [BoxGroup("References"), SerializeField, Required] private TMP_Text      _actionText;
        [BoxGroup("References"), SerializeField, Required] private RebindOverlay _rebindOverlay;

        [BoxGroup("References"), SerializeField, Required] private ActionRebinder _keyboardPrimary;
        [BoxGroup("References"), SerializeField, Required] private ActionRebinder _keyboardSecondary;
        [BoxGroup("References"), SerializeField, Required] private ActionRebinder _gamepadPrimary;


        public InputAction InputAction => _inputAction;

        public RebindOverlay RebindingOverlay => _rebindOverlay;


        private void Wait1FrameAndUpdateUI()
        {
            DOVirtual.DelayedCall(0.1f, UpdateUI);
        }


        private void OnEnable()
        {
            UpdateText();
            UpdateUI();
        }


        [Button]
        private void UpdateUI()
        {
            name = $"Input Action Rebinders ({_inputAction.name})";

            if (_inputAction == null || _inputAction.action == null)
            {
                Debug.LogError("Input action reference is null.");
                return;
            }

            var bindings = GroupBindings(_inputAction.action);

            if (bindings.Count is < 3 or > 3)
            {
                Debug.LogError($"Expected at least 3 bindings (Keyboard1, Keyboard2, Gamepad), but found: {bindings.Count}");
                return;
            }

            // Assign indexes in the fixed order
            _keyboardPrimary.SetBindingIndex(bindings[0].StartIndex);
            _keyboardSecondary.SetBindingIndex(bindings[1].StartIndex);
            _gamepadPrimary.SetBindingIndex(bindings[2].StartIndex);

            Debug.Log($"Assigned Indexes: Keyboard Primary={bindings[0].StartIndex}, Keyboard Secondary={bindings[1].StartIndex}, Gamepad Primary={bindings[2].StartIndex}");

            UpdateText();
        }

        /// <summary>
        /// Groups the bindings by their composite type (if any).
        /// </summary>
        private List<ControlsData?> GroupBindings(InputAction inputAction)
        {
            var groupedBindings = new List<ControlsData?>();
            ControlsData? currentGroup = null;

            for (int i = 0; i < inputAction.bindings.Count; i++)
            {
                var binding = inputAction.bindings[i];

                if (binding.isComposite)
                {
                    // Start a new group for this composite
                    string compositeIdentifier = $"{binding.path}_{binding.name}";
                    if (currentGroup != null)
                    {
                        // Finalize the previous group
                        currentGroup.StopIndex = i - 1;
                        groupedBindings.Add(currentGroup);
                    }

                    if (inputAction == null)
                    {
                        Debug.LogError("Null input action");
                        continue;
                    }

                    Debug.Log("Input action is not null");

                    var reference = InputActionReference.Create(inputAction);

                    if (reference == null)
                    {
                        Debug.LogError("Null reference");
                    }
                    else
                    {
                        Debug.Log("Reference is not null");
                    }

                    Debug.Log(reference.GetType().FullName);

                    // Start a new group
                    currentGroup = new ControlsData
                    {
                        InputActionReference = reference,
                        Action = inputAction.name,
                        Group = binding.groups,
                        CompositeType = compositeIdentifier,
                        StartIndex = i,
                        Paths = new List<string>()
                    };
                }
                else if (binding.isPartOfComposite)
                {
                    // Add this binding to the current group
                    if (currentGroup != null)
                    {
                        currentGroup.Paths.Add(binding.path);
                        currentGroup.StopIndex = i; // Update StopIndex as this is part of the composite
                    }
                }
                else
                {
                    // Handle non-composite bindings
                    if (currentGroup != null)
                    {
                        // Finalize the current group
                        currentGroup.StopIndex = i - 1;
                        groupedBindings.Add(currentGroup);
                        currentGroup = null;
                    }

                    // Add this as a standalone binding
                    groupedBindings.Add(new ControlsData
                    {
                        Action = inputAction.name,
                        Group = binding.groups,
                        CompositeType = "",
                        StartIndex = i,
                        StopIndex = i,
                        Paths = new List<string> { binding.path }
                    });
                }
            }

            // Add the last group if it exists
            if (currentGroup != null)
            {
                currentGroup.StopIndex = inputAction.bindings.Count - 1;
                groupedBindings.Add(currentGroup);
            }

            return groupedBindings;
        }


        public void Awake()
        {
            if (_inputAction == null)
            {
                Debug.LogError("Null input action");
                return;
            }

            //Debug.Log($"{_inputAction.name}, {_inputAction.action?.name}");

            UpdateText();
        }

        private void UpdateText()
        {
            var icon = SpriteMapReference.GetSprite(_inputAction.action.name);


            var actionMapName = _inputAction.action.actionMap.name;

            // Remove the action map prefix of the name of the action
            var actionName = _inputAction.action.name.Replace(actionMapName, "");

            _actionText.text = icon + " " + actionName;
        }


        public string[] SearchKeywords
        {
            get
            {
                var actionRebinds = gameObject.GetComponentsInChildren<ActionRebinder>();
                var actionNames = new string[actionRebinds.Length];
                for (var i = 0; i < actionRebinds.Length; i++) actionNames[i] = actionRebinds[i].ActionName;
                return actionNames;
            }
        }
    }

    [Serializable]
    public class ControlsData
    {
        [ShowInInspector, SerializeField, ReadOnly]
        public InputActionReference InputActionReference;

        [ShowInInspector, SerializeField, ReadOnly]
        public InputBinding InputBinding;

        [ShowInInspector, SerializeField, ReadOnly]
        public string Action;

        [ShowInInspector, SerializeField, ReadOnly]
        public string Group;

        [ShowInInspector, SerializeField, ReadOnly]
        public string CompositeType;

        [ShowInInspector, SerializeField, ReadOnly]
        public int StartIndex;

        [ShowInInspector, SerializeField, ReadOnly]
        public int StopIndex;

        [ShowInInspector, SerializeField, ReadOnly]
        public List<string> Paths = new();

        public ControlsData()
        {
            StopIndex = StartIndex;
        }
    }
}