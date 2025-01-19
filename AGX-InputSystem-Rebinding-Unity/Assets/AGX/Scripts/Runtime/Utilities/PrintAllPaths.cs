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
            StringBuilder allPaths = new StringBuilder();

            // Process each device type
            //PrintDevicePaths(Keyboard.current, "Keyboard", allPaths);
            //PrintDevicePaths(Gamepad.current, "Gamepad", allPaths);
            //PrintDevicePaths(Mouse.current, "Mouse", allPaths);
            PrintDevicePaths(Touchscreen.current, "Touchscreen", allPaths); // Added touchscreen
        }

        private void PrintDevicePaths(InputDevice device, string deviceName, StringBuilder allPaths)
        {
            if (device == null)
            {
                Debug.LogWarning($"No {deviceName} connected.");
                return;
            }

            Debug.Log($"{deviceName}:");
            foreach (var binding in DeviceInputSpritesDefaults.GetActionBindingsForDevice(device))
            {
                Debug.Log(binding.Path);
                allPaths.AppendLine(binding.Path);
            }

            Debug.Log("All paths: " + allPaths);
            allPaths.Clear();
        }
    }

    public static class DeviceInputSpritesDefaults
    {
        public static List<ActionBindingPromptEntry> GetActionBindingsForDevice(InputDevice device)
        {
            List<ActionBindingPromptEntry> actionBindings = new List<ActionBindingPromptEntry>();

            void TraverseControls(InputControl control)
            {
                actionBindings.Add(new ActionBindingPromptEntry
                {
                    DisplayName = control.displayName,
                    Path = control.path,
                    DeviceName = device.displayName
                });

                foreach (var childControl in control.children)
                {
                    TraverseControls(childControl);
                }
            }

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