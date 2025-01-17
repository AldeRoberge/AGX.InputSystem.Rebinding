using System;
using System.Collections.Generic;
using Generator.Scripts.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Object = UnityEngine.Object;

namespace AGX.Scripts.Runtime.Rebinder
{
    public class InputManager
    {
        private const string KeyboardEscape    = "<Keyboard>/escape";
        private const string Mouse             = "Mouse";
        private const string GamepadLeftstick  = "<Gamepad>/leftstick";
        private const string GamepadRightstick = "<Gamepad>/rightstick";

        private static InputActions? inputActions;

        public static InputActions InputActions => inputActions ??= new InputActions();

        public static event Action RebindComplete = delegate { };
        public static event Action RebindCanceled = delegate { };
        public static event Action<InputAction, int> RebindStarted = delegate { };

        private static List<ActionRebinder> _actionRebinders = new();

        public static event Action<int> RebindCountChanged = delegate { };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            RebindComplete = delegate { };
            RebindCanceled = delegate { };
            RebindStarted = delegate { };
            inputActions = new InputActions();
            _actionRebinders = new List<ActionRebinder>();
            RebindCountChanged = delegate { };
        }

        public static void RefreshInputDevicePrompt()
        {
            // We have to do the following because, well
            // The InputManager does some changes to the asset itself :).. you see....
            // So we must... Pass it to the InputDevicePromptSystem so that it shows the changes we make to it :)
            // :) :D
            // GitHub Copilot says : "I'm an AI, I don't have feelings, but I'm here to help you write code."
            // Very cringe-pilled, Copilot. Very cringe-pilled.

            UpdateRebindCount();
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

                RefreshInputDevicePrompt();
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

            rebind.WithCancelingThrough(KeyboardEscape);

            if (excludeMouse)
                rebind.WithControlsExcluding(Mouse);

            Debug.Log($"Rebinding {actionToRebind.name} at index {bindingIndex} for map {actionToRebind.actionMap.name}");


            // Add filtering based on the input action's control type (Keyboard or Gamepad)
            if (actionToRebind.actionMap.name == "Keyboard")
            {
                rebind.WithControlsExcluding(GamepadLeftstick);
                rebind.WithControlsExcluding(GamepadRightstick);
            }
            else if (actionToRebind.actionMap.name == "Gamepad")
            {
                rebind.WithControlsExcluding(KeyboardEscape); // Exclude keyboard escape for gamepad actions
            }

            rebindOverlay?.SetActive(true);
            if (rebindOverlay != null)
            {
                var partName = string.Empty;
                if (actionToRebind.bindings[bindingIndex].isPartOfComposite)
                    partName = $"Binding '{actionToRebind.bindings[bindingIndex].name}'.";

                string text;

                if (excludeMouse)
                {
                    text = !string.IsNullOrEmpty(actionToRebind.expectedControlType)
                        ? $"{partName} Press any button ({actionToRebind.expectedControlType})..."
                        : $"{partName} Press any button...";
                }
                else
                {
                    text = !string.IsNullOrEmpty(actionToRebind.expectedControlType)
                        ? $"{partName} Press any button ({actionToRebind.expectedControlType}) or mouse button..."
                        : $"{partName} Press any button or mouse button...";
                }

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

            // Check for duplicate (composite) bindings
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

        public static void LoadBindingOverrides()
        {
            // Iterate over all action maps in the InputActions asset
            foreach (var actionMap in InputActions.asset.actionMaps)
            {
                foreach (var action in actionMap.actions)
                {
                    for (var i = 0; i < action.bindings.Count; i++)
                    {
                        var key = $"Input-Binding-{action.actionMap.name}-{action.name}-{i}";

                        var storedOverride = PlayerPrefs.GetString(key);

                        if (!string.IsNullOrEmpty(storedOverride))
                        {
                            Debug.Log($"Applying binding override for {action.name} at index {i}: {storedOverride}");
                            action.ApplyBindingOverride(i, storedOverride);
                        }
                    }
                }
            }
        }


        public static void ResetAllBindings()
        {
            Debug.Log("Resetting all bindings");

            // find all rebind controls in the scene
            var rebindControls = Object.FindObjectsOfType<ActionRebinder>();

            foreach (var rebindControl in rebindControls)
                ResetBinding(rebindControl);
        }

        public static void ResetBinding(ActionRebinder actionRebinder)
        {
            var actionName = actionRebinder.ActionName;
            var bindingIndex = actionRebinder.BindingIndex;

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


            RefreshInputDevicePrompt();

            actionRebinder.UpdateUI();
        }

        public static bool IsBindingOverriden(string actionName, int bindingIndex)
        {
            InputAction action = InputActions.asset.FindAction(actionName);

            if (action == null || action.bindings.Count <= bindingIndex)
            {
                Debug.LogError($"Action '{actionName}' or binding at index {bindingIndex} not found.");
                return false;
            }

            // Find the root binding index if this is part of a composite
            var rootBindingIndex = bindingIndex;
            while (rootBindingIndex > 0 && action.bindings[rootBindingIndex].isPartOfComposite)
            {
                rootBindingIndex--;
            }

            var isComposite = action.bindings[rootBindingIndex].isComposite;

            if (isComposite)
            {
                // Check all parts of the composite binding
                for (var i = rootBindingIndex + 1; i < action.bindings.Count; i++)
                {
                    var partBinding = action.bindings[i];
                    if (!partBinding.isPartOfComposite)
                        break;

                    var originalPath = partBinding.path;
                    var overridePath = partBinding.overridePath;

                    if (!string.IsNullOrEmpty(overridePath) && overridePath != originalPath)
                    {
                        // Debug.Log($"Composite binding part changed: {partBinding.name}, Original: {originalPath}, Override: {overridePath}");
                        return true;
                    }
                }

                Debug.Log("No changes found in composite binding parts.");
                return false;
            }
            else
            {
                // Check the single binding
                var originalPath = action.bindings[bindingIndex].path;
                var overridePath = action.bindings[bindingIndex].overridePath;

                var isChanged = !string.IsNullOrEmpty(overridePath) && overridePath != originalPath;

                /*
                    Debug.Log(isChanged
                        ? $"The binding <b>{originalPath}</b> has been changed to {overridePath}"
                        : $"The binding <b>{originalPath}</b> has not been changed.");
                */

                return isChanged;
            }
        }

        public static void RegisterRebind(ActionRebinder actionRebinder)
        {
            _actionRebinders ??= new List<ActionRebinder>();
            _actionRebinders.Add(actionRebinder);
        }

        public static void UpdateRebindCount()
        {
            RebindCountChanged?.Invoke(GetTotalBindingOverwriteCount());
        }

        internal static int GetTotalBindingOverwriteCount()
        {
            int count = 0;

            foreach (var rebindControl in _actionRebinders)
            {
                if (IsBindingOverriden(rebindControl.ActionName, rebindControl.BindingIndex))
                {
                    count++;
                }
            }

            return count;
        }

        public static ReadOnlyArray<InputBinding> GetBindings(string actionName)
        {
            var action = InputActions.asset.FindAction(actionName);
            return action.bindings;
        }

        public static InputAction GetAction(string actionName)
        {
            return InputActions.asset.FindAction(actionName);
        }
    }
}