using System.Collections.Generic;
using UnityEngine.InputSystem;
using AGX.Prompts.Scripts;
using UnityEngine;

namespace AGX.Prompts.Editor
{
    public static class DeviceInputSpritesDefaults
    {
        public static List<ActionBindingPromptEntry> GetAllKeyboardActionBindings()
        {
            if (Keyboard.current == null)
            {
                Debug.LogWarning("No keyboard connected.");
                return new List<ActionBindingPromptEntry>();
            }

            return CollectActionBindings(Keyboard.current);
        }

        public static List<ActionBindingPromptEntry> GetAllGamePadActionBindings()
        {
            if (Gamepad.current == null)
            {
                Debug.LogWarning("No gamepad connected.");
                return new List<ActionBindingPromptEntry>();
            }

            return CollectActionBindings(Gamepad.current);
        }

        public static List<ActionBindingPromptEntry> GetAllMouseActionBindings()
        {
            if (Mouse.current == null)
            {
                Debug.LogWarning("No mouse connected.");
                return new List<ActionBindingPromptEntry>();
            }

            return CollectActionBindings(Mouse.current);
        }

        private static List<ActionBindingPromptEntry> CollectActionBindings(InputDevice device)
        {
            List<ActionBindingPromptEntry> actionBindings = new List<ActionBindingPromptEntry>();

            void TraverseControls(InputControl control)
            {
                // Create an ActionBindingPromptEntry for each control and add it to the list
                actionBindings.Add(new ActionBindingPromptEntry
                {
                    DisplayName = control.displayName,
                    Path = control.path,
                    DeviceName = device.displayName
                });

                // Recursively traverse child controls
                foreach (var childControl in control.children)
                {
                    TraverseControls(childControl);
                }
            }

            // Start traversal from the device root
            TraverseControls(device);
            return actionBindings;
        }
    }
}