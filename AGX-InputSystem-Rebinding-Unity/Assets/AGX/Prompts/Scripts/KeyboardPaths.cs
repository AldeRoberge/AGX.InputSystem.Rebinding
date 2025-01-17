using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Prompts.Scripts
{
    public class KeyboardPaths : MonoBehaviour
    {
        void Start()
        {
            // Check if a keyboard is connected
            if (Keyboard.current == null)
            {
                Debug.Log("No keyboard connected.");
                return;
            }

            // Get the connected keyboard
            Keyboard keyboard = Keyboard.current;

            // List to store all paths
            List<string> allPaths = new List<string>();

            // Recursively collect all control paths
            CollectControlPaths(keyboard, allPaths);

            // Print all paths
            foreach (var path in allPaths)
            {
                Debug.Log(path);
            }
        }

        private void CollectControlPaths(InputControl control, List<string> paths)
        {
            // Add the control's path to the list
            paths.Add(control.path);

            try
            {
                Debug.Log($"Control Path: {Keyboard.current[control.path].displayName}");
            }
            catch
            {
                //
            }

            // Iterate through children and collect their paths
            foreach (var childControl in control.children)
            {
                CollectControlPaths(childControl, paths);
            }
        }
    }
}