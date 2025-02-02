using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Scripts.Runtime
{
    [CreateAssetMenu(fileName = "InputRebindings", menuName = "AGX/Input Rebindings")]
    public class InputRebindings : ScriptableObject
    {
        public List<ActionMapControls> ActionMapDatas => inputActionRebindings;

        [SerializeField, HideLabel]
        private List<ActionMapControls> inputActionRebindings = new();


        /// <summary>
        /// Rebuilds the rebindings based on the provided InputActionAsset.
        /// </summary>
        /// <param name="inputActions">The InputActionAsset containing the input actions.</param>
        public void Rebuild(InputActionAsset inputActions)
        {
            if (inputActions == null)
            {
                Debug.LogError("InputActionAsset is null.");
                return;
            }

            // Clear existing rebindings before rebuilding.
            inputActionRebindings.Clear();

            // Process each action map in the input action asset
            foreach (var actionMap in inputActions.actionMaps)
            {
                AddRebindingsForActionMap(actionMap);
            }
        }

        /// <summary>
        /// Adds rebindings for each action in the given action map.
        /// </summary>
        private void AddRebindingsForActionMap(InputActionMap actionMap)
        {
            if (actionMap == null) return;

            // Process each action in the action map
            foreach (var action in actionMap.actions)
            {
                AddRebindingsForAction(action, actionMap.name);
            }
        }

        /// <summary>
        /// Adds rebindings for each binding of a given action.
        /// </summary>
        private void AddRebindingsForAction(InputAction action, string actionMapName)
        {
            if (action == null || string.IsNullOrEmpty(actionMapName)) return;

            // Ensure that there's a rebinding entry for this action map
            var actionRebinding = GetOrCreateRebindingEntry(actionMapName);

            // Add rebindings for each binding in the action
            var groupedBindings = GroupBindings(action);
            foreach (var group in groupedBindings)
            {
                // Add the grouped rebinding data for the specific action map and action
                actionRebinding.Controls.Add(group);
            }
        }

        /// <summary>
        /// Groups the bindings by their composite type (if any).
        /// </summary>
        private List<ControlsData> GroupBindings(InputAction action)
        {
            var groupedBindings = new List<ControlsData>();
            ControlsData currentGroup = null;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                var binding = action.bindings[i];

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

                    // Start a new group
                    currentGroup = new ControlsData
                    {
                        InputAction = action,
                        Action = action.name,
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
                        Action = action.name,
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
                currentGroup.StopIndex = action.bindings.Count - 1;
                groupedBindings.Add(currentGroup);
            }

            return groupedBindings;
        }


        /// <summary>
        /// Gets or creates a rebinding entry for a specific action map.
        /// </summary>
        private ActionMapControls GetOrCreateRebindingEntry(string actionMapName)
        {
            // Try to find the rebinding entry for the given action map name
            var actionRebinding = inputActionRebindings.Find(rebinding => rebinding.ActionMap == actionMapName);

            // If not found, create a new one and add it to the list
            if (actionRebinding == null)
            {
                actionRebinding = new ActionMapControls { ActionMap = actionMapName };
                inputActionRebindings.Add(actionRebinding);
            }

            return actionRebinding;
        }
    }

    [Serializable]
    public class ActionMapControls
    {
        [ShowInInspector, SerializeField, ReadOnly]
        public string ActionMap;

        [ShowInInspector, SerializeField, ReadOnly, HideLabel]
        public List<ControlsData> Controls = new();
    }

    [Serializable]
    public class ControlsData
    {
        [ShowInInspector, SerializeField, ReadOnly]
        public InputAction InputAction;

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