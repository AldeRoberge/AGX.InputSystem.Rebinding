using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Scripts.Runtime
{
    /// <summary>
    /// Used to display icons for InputActionReferences in UI elements.
    /// I.e. to show a Settings icon sprite for the "Open Settings" action.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "ActionIconMap", menuName = "AGX/Action Icon Map")]
    public class ActionIconMap : ScriptableObject
    {
        [SerializeField]
        public List<ActionIcon> ActionIcons = new();

        public string GetFor(InputActionReference inputActionReference)
        {
            foreach (var actionIcon in ActionIcons)
            {
                if (actionIcon.Action == inputActionReference)
                {
                    return $"<sprite=\"General/General\" name=\"{actionIcon.Icon.name}\"> {inputActionReference.action.name}";
                }
            }

            return inputActionReference.action.name;
        }
    }

    [Serializable]
    public class ActionIcon
    {
        public InputActionReference Action;

        [ShowAssetPreview]
        public Sprite Icon;
    }
}