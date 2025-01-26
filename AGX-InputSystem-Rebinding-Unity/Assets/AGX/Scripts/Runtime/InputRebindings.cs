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
                    // Create rebindings for each binding of this action
                    for (var index = 0; index < action.bindings.Count; index++)
                    {
                        var binding = action.bindings[index];
                        var rebinding = new InputActionRebinding
                        {
                            ActionMap = actionMap.name,
                            IsRebindable = true,
                            BindingIndex = index,
                            InputActionReference = new InputActionReference(action)
                        };

                        _inputActionRebindings.Add(rebinding);
                    }
                }
            }
        }
    }

    [Serializable]
    public class InputActionRebinding
    {
        [ShowInInspector, SerializeField]
        public string ActionMap;

        [ShowInInspector, SerializeField]
        public bool IsRebindable;

        [ShowInInspector, SerializeField]
        public int BindingIndex;

        [ShowInInspector, SerializeField]
        public InputActionReference InputActionReference;
    }
}