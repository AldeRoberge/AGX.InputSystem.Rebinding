using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Scripts.Runtime.Rebinder
{ 
    /// <summary>
    /// Used to display icons for InputActionReferences in UI elements.
    /// I.e. to show a Settings icon sprite for the "Open Settings" action.
    /// </summary>
    [Serializable, CreateAssetMenu(fileName = "ActionIconMap", menuName = "AGX/Action Icon Map")]
    public class ActionIconMap : ScriptableObject
    {
        [SerializeField]
        public List<ActionIcon> ActionIcons = new();

        public string GetFor(InputAction inputActionReference)
        {
            foreach (var actionIcon in ActionIcons)
            {
                if (actionIcon.Action == null)
                {
                    Debug.LogWarning("There is a null action in ActionIconMap. Has the value gone missing?");
                    continue;
                }

                if (actionIcon.Action.action == inputActionReference)
                {
                    return $"<sprite=\"General/General\" name=\"{actionIcon.Icon.name}\"> {inputActionReference.name}";
                }
            }

            return inputActionReference.name;
        }
    }

    [Serializable]
    public class ActionIcon
    {
        [BoxGroup("References"), SerializeField, Required]
        public InputActionReference Action;

        [BoxGroup("References"), SerializeField, Required, PreviewField]
        public Sprite Icon;
    }
}