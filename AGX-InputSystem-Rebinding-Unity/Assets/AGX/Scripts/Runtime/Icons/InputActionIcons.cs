using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Scripts.Runtime.Icons
{
    /// <summary>
    /// Used to display icons for InputActionReferences in UI elements.
    /// I.e. to show a Settings icon sprite for the "Open Settings" action.
    /// </summary>
    [Serializable, CreateAssetMenu(fileName = "ActionIconMap", menuName = "AGX/Action Icon Map")]
    public class ActionIconMap : ScriptableObject
    {
       
    }

    [Serializable]
    public class ActionIcon
    {
        [BoxGroup("References"), SerializeField, Required, HideLabel]
        public InputActionReference Action;

        [BoxGroup("References"), SerializeField, Required, PreviewField]
        public Sprite Icon;
    }
}