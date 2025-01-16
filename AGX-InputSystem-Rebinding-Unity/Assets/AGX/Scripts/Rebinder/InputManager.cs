using System;
using Generator.Scripts.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace AGX.Scripts.Rebinder
{
    public class InputManager
    {
        private static InputActions? inputActions;

        public static InputActions InputActions => inputActions ??= new InputActions();

        public static event Action RebindComplete = delegate { };
        public static event Action RebindCanceled = delegate { };
        public static event Action<InputAction, int> RebindStarted = delegate { };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            RebindComplete = delegate { };
            RebindCanceled = delegate { };
            RebindStarted = delegate { };
            inputActions = new InputActions();
        }

        public static void StartRebind(string actionName, int bindingIndex, RebindOverlay rebindOverlay, bool excludeMouse)
        {
            var action = InputActions.asset.FindAction(actionName);

            if (action == null || action.bindings.Count <= bindingIndex)
            {
                Debug.Log("Couldn't find action or binding");
                return;
            }

            if (action.bindings[bindingIndex].isComposite)
            {
                var firstPartIndex = bindingIndex + 1;
                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                    DoRebind(action, firstPartIndex, rebindOverlay, excludeMouse, true);
            }
            else
            {
                DoRebind(action, bindingIndex, rebindOverlay, excludeMouse, false);
            }
        }

        private static void DoRebind(InputAction actionToRebind, int bindingIndex, RebindOverlay rebindOverlay, bool excludeMouse, bool allCompositeParts)
        {
            if (actionToRebind == null || bindingIndex < 0)
                return;

            actionToRebind.Disable();

            var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);

            rebind.OnComplete(operation =>
            {
                actionToRebind.Enable();
                rebindOverlay?.SetActive(false);
                operation.Dispose();

                if (IsDuplicateBinding(actionToRebind, bindingIndex, allCompositeParts))
                {
                    actionToRebind.RemoveBindingOverride(bindingIndex);
                    operation.Dispose();
                    DoRebind(actionToRebind, bindingIndex, rebindOverlay, excludeMouse, allCompositeParts);
                    return;
                }

                if (allCompositeParts)
                {
                    var nextBindingsIndex = bindingIndex + 1;
                    if (nextBindingsIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingsIndex].isPartOfComposite)
                        DoRebind(actionToRebind, nextBindingsIndex, rebindOverlay, excludeMouse, true);
                }

                SaveBindingOverride(actionToRebind);
                RebindComplete?.Invoke();
            });

            rebind.OnCancel(operation =>
            {
                actionToRebind.Enable();
                rebindOverlay?.SetActive(false);
                operation.Dispose();

                RebindCanceled?.Invoke();
            });

            rebind.WithCancelingThrough("<Keyboard>/escape");

            if (excludeMouse)
                rebind.WithControlsExcluding("Mouse");

            rebind.WithControlsExcluding("<Gamepad>/leftstick");
            rebind.WithControlsExcluding("<Gamepad>/rightstick");

            var partName = default(string);
            if (actionToRebind.bindings[bindingIndex].isPartOfComposite)
                partName = $"Binding '{actionToRebind.bindings[bindingIndex].name}'.";

            rebindOverlay?.SetActive(true);
            if (rebindOverlay != null)
            {
                var text = !string.IsNullOrEmpty(actionToRebind.expectedControlType)
                    ? $"{partName} Waiting for input ({actionToRebind.expectedControlType})..."
                    : $"{partName} Waiting for input (Any)...";

                rebindOverlay.SetText(text);
            }

            RebindStarted?.Invoke(actionToRebind, bindingIndex);
            rebind.Start(); //actually starts the rebinding
        }

        // Only checks for duplicates within the same action map.
        private static bool IsDuplicateBinding(InputAction actionToRebind, int bindingIndex, bool allCompositeParts = false)
        {
            var newBinding = actionToRebind.bindings[bindingIndex];

            // Check for duplicate bindings
            foreach (var binding in actionToRebind.actionMap.bindings)
            {
                // Skip the binding we're currently rebinding.
                if (binding.action == newBinding.action)
                    continue;

                // Skip different paths
                if (binding.effectivePath != newBinding.effectivePath)
                    continue;

                Debug.LogWarning($"Duplicate binding found: {newBinding.effectivePath}");
                return false;
            }

            //Check for duplicate (composite) bindings
            if (allCompositeParts)
            {
                for (var i = 0; i < bindingIndex; ++i)
                {
                    // Skip different paths
                    if (actionToRebind.bindings[i].effectivePath != newBinding.effectivePath)
                        continue;

                    Debug.LogWarning($"Duplicate binding found: {newBinding.effectivePath}");
                    return false;
                }
            }

            return false;
        }

        public static string GetBindingName(string actionName, int bindingIndex)
        {
            var action = InputActions.asset.FindAction(actionName);
            var displayString = action.GetBindingDisplayString(bindingIndex);
            return displayString;
        }

        private static void SaveBindingOverride(InputAction action)
        {
            for (var i = 0; i < action.bindings.Count; i++)
            {
                var key = $"Input-Binding-{action.actionMap.name}-{action.name}-{i}";
                var value = action.bindings[i].overridePath;

                PlayerPrefs.SetString(key, value);
            }
        }

        public static void LoadBindingOverride(string actionName)
        {
            var action = InputActions.asset.FindAction(actionName);

            for (var i = 0; i < action.bindings.Count; i++)
            {
                var key = $"Input-Binding-{action.actionMap.name}-{action.name}-{i}";

                var storedOverride = PlayerPrefs.GetString(key);

                if (!string.IsNullOrEmpty(storedOverride))
                    action.ApplyBindingOverride(i, storedOverride);
            }
        }

        public static void ResetAllBindings()
        {
            Debug.Log("Resetting all bindings");

            // find all rebind controls in the scene
            var rebindControls = Object.FindObjectsOfType<RebindControls>();

            foreach (var rebindControl in rebindControls)
                ResetBinding(rebindControl);
        }

        public static void ResetBinding(RebindControls rebindControls)
        {
            var actionName = rebindControls.ActionName;
            var bindingIndex = rebindControls.BindingIndex;

            var action = InputActions.asset.FindAction(actionName);

            if (action == null || action.bindings.Count <= bindingIndex)
            {
                Debug.Log("Could not find action or binding");
                return;
            }

            if (action.bindings[bindingIndex].isComposite)
            {
                for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
                    action.RemoveBindingOverride(i);
            }
            else
            {
                action.RemoveBindingOverride(bindingIndex);
            }

            SaveBindingOverride(action);

            rebindControls.UpdateUI();
        }

        public static bool IsBindingChanged(string actionName, int bindingIndex)
        {
            var action = InputActions.asset.FindAction(actionName);

            if (action == null || action.bindings.Count <= bindingIndex)
            {
                Debug.LogError($"Action '{actionName}' or binding at index {bindingIndex} not found.");
                return false;
            }

            // Check if the override path exists and differs from the original path
            var originalPath = action.bindings[bindingIndex].path;
            var overridePath = action.bindings[bindingIndex].overridePath;

            var isChanged = !string.IsNullOrEmpty(overridePath) && overridePath != originalPath;

            Debug.Log($"IsBindingChanged: {isChanged}, Original: {originalPath}, Override: {overridePath}");

            return isChanged;
        }
    }
}