using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Scripts.Runtime.Utilities
{
    public class PrintAllPaths : MonoBehaviour
    {
        public void Start()
        {
            StringBuilder all = new StringBuilder();

            Debug.Log("Keyboard:");
            foreach (var allKeyboardActionBinding in DeviceInputSpritesDefaults.GetAllKeyboardActionBindings())
            {
                Debug.Log(allKeyboardActionBinding.Path);
                all.AppendLine(allKeyboardActionBinding.Path);
            }

            Debug.Log("All paths:" + all);

            all.Clear();


            Debug.Log("Gamepad:");
            foreach (var allGamePadActionBinding in DeviceInputSpritesDefaults.GetAllGamePadActionBindings())
            {
                Debug.Log(allGamePadActionBinding.Path);
                all.AppendLine(allGamePadActionBinding.Path);
            }

            Debug.Log("All paths:" + all);

            all.Clear();

            Debug.Log("Mouse:");

            foreach (var allMouseActionBinding in DeviceInputSpritesDefaults.GetAllMouseActionBindings())
            {
                Debug.Log(allMouseActionBinding.Path);
                all.AppendLine(allMouseActionBinding.Path);
            }

            Debug.Log("All paths:" + all);

            all.Clear();
            
            // TODO next device should be : Touchscreen
            
        }
    }

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

    public class ActionBindingPromptEntry
    {
        public string DisplayName { get; set; }
        public string Path { get; set; }
        public string DeviceName { get; set; }
    }
}