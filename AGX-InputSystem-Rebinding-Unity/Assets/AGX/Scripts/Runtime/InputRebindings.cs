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
        [SerializeField, HideLabel]
        private List<InputActionRebinding> inputActionRebindings = new();

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
                actionRebinding.ActionMapRebinders.ActionMapBindings.Add(group);
            }
        }

        /// <summary>
        /// Groups the bindings by their composite type (if any).
        /// </summary>
        private List<GroupedBindingData> GroupBindings(InputAction action)
        {
            var groupedBindings = new List<GroupedBindingData>();
            GroupedBindingData currentGroup = null;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                var binding = action.bindings[i];

                // Check if the binding is part of a composite
                if (binding.isComposite)
                {
                    // We group based on both the composite type (path) and name (to distinguish WASD and Arrow Keys)
                    string compositeIdentifier = $"{binding.path}_{binding.name}";

                    // Start a new group if this is the first binding of a composite (distinct by path + name)
                    if (currentGroup == null || currentGroup.CompositeType != compositeIdentifier)
                    {
                        // Add the previous group if it exists
                        if (currentGroup != null)
                        {
                            groupedBindings.Add(currentGroup);
                        }

                        // Start a new group
                        currentGroup = new GroupedBindingData
                        {
                            Action = action.name,
                            Group = string.Join(";", binding.groups),
                            CompositeType = compositeIdentifier, // Distinguish composites by path and name
                            StartIndex = i,
                            Paths = new List<string>() // Do not include the '2DVector' path
                        };
                    }
                }
                else
                {
                    // If this binding is part of a composite, add it to the current group (without re-creating a new group)
                    if (binding.isPartOfComposite)
                    {
                        if (currentGroup != null)
                        {
                            // Add non-composite individual bindings to the group without re-including the composite path
                            currentGroup.Paths.Add(binding.path);
                        }
                    }
                    else
                    {
                        // For non-composite bindings, close the previous group if exists
                        if (currentGroup != null)
                        {
                            groupedBindings.Add(currentGroup);
                            currentGroup = null;
                        }

                        // Create a new single entry for non-composite bindings
                        groupedBindings.Add(new GroupedBindingData
                        {
                            Action = action.name,
                            Group = string.Join(";", binding.groups),
                            CompositeType = "",
                            StartIndex = i,
                            StopIndex = i,
                            Paths = new List<string> { binding.path }
                        });
                    }
                }
            }

            // Add the last group if it exists
            if (currentGroup != null)
            {
                groupedBindings.Add(currentGroup);
            }

            return groupedBindings;
        }


        /// <summary>
        /// Gets or creates a rebinding entry for a specific action map.
        /// </summary>
        private InputActionRebinding GetOrCreateRebindingEntry(string actionMapName)
        {
            // Try to find the rebinding entry for the given action map name
            var actionRebinding = inputActionRebindings.Find(rebinding => rebinding.ActionMap == actionMapName);

            // If not found, create a new one and add it to the list
            if (actionRebinding == null)
            {
                actionRebinding = new InputActionRebinding { ActionMap = actionMapName };
                inputActionRebindings.Add(actionRebinding);
            }

            return actionRebinding;
        }
    }

    [Serializable]
    public class InputActionRebinding
    {
        [ShowInInspector, SerializeField, ReadOnly]
        public string ActionMap;

        [ShowInInspector, SerializeField, ReadOnly, HideLabel]
        public InputActionRebindingData ActionMapRebinders = new();
    }

    [Serializable]
    public class InputActionRebindingData
    {
        [ShowInInspector, SerializeField, ReadOnly]
        public List<GroupedBindingData> ActionMapBindings = new();
    }

    [Serializable]
    public class GroupedBindingData
    {
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

        public GroupedBindingData()
        {
            StopIndex = StartIndex;
        }
    }
}