using System;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace AGX.Prompts.Scripts
{
    public enum InputDeviceType
    {
        Mouse,
        Keyboard,
        GamePad,
        Touchscreen
    }

    /// <summary>
    /// A single entry for an action and the corresponding sprite to use
    /// </summary>
    ///
    [Serializable]
    public class ActionBindingPromptEntry
    {
        /// <summary>
        /// The Action Binding path, eg "<Gamepad>/leftStick"
        /// As described here - https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/manual/ActionBindings.html
        /// </summary>
        [FormerlySerializedAs("ActionBindingPath")] [Required]
        public string Path;

        /// <summary>
        /// The name of the corresponding sprite. We assume the TMP Sprite asset and source sprite are
        /// synced up and have the same name 
        /// </summary>
        [ShowAssetPreview, Required]
        public Sprite PromptSprite;


        public string DisplayName;
        
        public string DeviceName;
    }

    [Serializable]
    [CreateAssetMenu(fileName = "New Device Input Sprites", menuName = "AGX/Prompts/DeviceInputSprites", order = 1)]
    public class DeviceInputSprites : ScriptableObject
    {
        /// <summary>
        /// The types of devices supported by this asset (can be multiple eg mouse/keyboard)
        /// </summary>
        public List<InputDeviceType> DeviceTypes = new();

        /// <summary>
        /// Device names that can be used to identify this device
        /// </summary>
        public string[] DeviceNames;

        /// <summary>
        /// The TextMeshPro Sprite Asset which contains prompt icons for this device
        /// </summary>
        public TMP_SpriteAsset SpriteAsset;

        /// <summary>
        /// A list of all the action bindings and their corresponding prompt icons
        /// </summary>
        public List<ActionBindingPromptEntry> ActionBindingPromptEntries;
    }
}