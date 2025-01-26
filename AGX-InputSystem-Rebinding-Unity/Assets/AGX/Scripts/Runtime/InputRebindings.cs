using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Scripts.Runtime
{
    [CreateAssetMenu(fileName = "InputRebindings", menuName = "AGX/Input Rebindings")]
    public class InputRebindings : ScriptableObject
    {
        // List to store rebindings for each action
        [SerializeField] private List<InputActionRebinding> _inputActionRebindings = new();

        public void Rebuild(InputActionAsset inputActions)
        {
            // Clear existing data before rebuilding
            _inputActionRebindings.Clear();

            // Loop through all action maps in the InputActionAsset
            foreach (var actionMap in inputActions.actionMaps)
            {
                foreach (var action in actionMap.actions)
                {
                    // Check if the action is rebindable
                    bool isRebindable = action.bindings.Count > 0;

                    // Create an InputActionRebinding and add it to the list
                    var rebinding = new InputActionRebinding
                    {
                        ActionMap = actionMap.name,
                        IsRebindable = isRebindable
                    };

                    _inputActionRebindings.Add(rebinding);
                }
            }
        }
    }

    [Serializable]
    public class InputActionRebinding
    {
        public string ActionMap { get; set; }
        public bool IsRebindable { get; set; }
    }
}