using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Input.Rebinding.Core.Scripts.Runtime.Input
{
    public static class BindingDisplayStringUtils
    {
        /// <summary>
        /// Retrieves a list of display-friendly binding strings for a given input action and control scheme.
        /// </summary>
        /// <param name="controlScheme">The control scheme to filter bindings (e.g., "Keyboard" or "Gamepad").</param>
        /// <param name="action">The input action whose bindings are being retrieved.</param>
        /// <returns>A list of strings representing the display-friendly bindings. Returns "???" if no bindings match.</returns>
        public static List<string> GetBindingDisplayStrings(string controlScheme, InputAction action)
        {
            var bindingDisplayList = new List<string>();
            // Create a binding mask for the control scheme.
            var bindingMask = InputBinding.MaskByGroup(controlScheme);
            var bindingCount = action.bindings.Count;

            for (var i = 0; i < bindingCount; i++)
            {
                var binding = action.bindings[i];

                // Skip bindings that don't match the control scheme.
                if (!bindingMask.Matches(binding))
                {
                    continue;
                }

                // For a simple binding (not composite and not part of one), convert its effective path.
                if (binding is { isComposite: false, isPartOfComposite: false })
                {
                    Debug.Log($"Simple Binding: {binding.effectivePath}");
                    bindingDisplayList.Add(binding.effectivePath);
                }
                // For composite bindings, process the composite parent and its children.
                else if (binding.isComposite || binding.isPartOfComposite)
                {
                    Debug.Log($"Composite Binding: {binding.effectivePath}");

                    var j = i;

                    if (binding.isComposite)
                        j += 1;

                    // Process children (bindings marked as part of composite).
                    while (j < bindingCount && action.bindings[j].isPartOfComposite)
                    {
                        var child = action.bindings[j];
                        if (bindingMask.Matches(child))
                        {
                            Debug.Log($"Composite Child: {child.effectivePath}");
                            bindingDisplayList.Add(child.effectivePath);
                        }

                        j++;
                    }

                    // Skip the composite children we already processed.
                    i = j - 1;

                    break;
                }
                // Bindings that are part of a composite will be handled as children.
            }

            // If nothing was added, return a fallback string.
            if (bindingDisplayList.Count == 0)
            {
                Debug.LogWarning("No bindings found for control scheme.");
                bindingDisplayList.Add("???");
            }

            // (Optional) Log the final list.
            Debug.Log($"Binding Display List: {string.Join(", ", bindingDisplayList)}");

            return bindingDisplayList;
        }
    }
}