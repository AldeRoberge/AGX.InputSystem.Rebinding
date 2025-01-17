using System;
using AGX.Prompts.Scripts;
using UnityEditor;
using UnityEngine;

namespace AGX.Prompts.Editor
{
    [CustomEditor(typeof(DeviceInputSprites))]
    public class DeviceInputSpritesEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var deviceInputSprites = (DeviceInputSprites)target;

            if (deviceInputSprites.DeviceTypes.Count == 0)
            {
                // Draw a warning to select a device type
                EditorGUILayout.HelpBox("Please select at least one device type.", MessageType.Warning);
            }

            if (deviceInputSprites.ActionBindingPromptEntries.Count != 0) return;

            // Draw a button that allows to populate with either keyboard
            foreach (var inputDeviceType in deviceInputSprites.DeviceTypes)
            {
                switch (inputDeviceType)
                {
                    case InputDeviceType.Keyboard:
                    {
                        if (GUILayout.Button("Populate with Keyboard"))
                            deviceInputSprites.ActionBindingPromptEntries = DeviceInputSpritesDefaults.GetAllKeyboardActionBindings();

                        break;
                    }
                    case InputDeviceType.GamePad:
                    {
                        if (GUILayout.Button("Populate with GamePad"))
                            deviceInputSprites.ActionBindingPromptEntries = DeviceInputSpritesDefaults.GetAllGamePadActionBindings();

                        break;
                    }
                    case InputDeviceType.Mouse:
                    {
                        if (GUILayout.Button("Populate with Mouse"))
                            deviceInputSprites.ActionBindingPromptEntries = DeviceInputSpritesDefaults.GetAllMouseActionBindings();

                        break;
                    }
                    case InputDeviceType.Touchscreen:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}