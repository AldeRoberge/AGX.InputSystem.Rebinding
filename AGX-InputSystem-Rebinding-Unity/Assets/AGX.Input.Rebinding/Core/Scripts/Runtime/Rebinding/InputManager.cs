using System;
using System.Collections.Generic;
using System.Text;
using FredericRP.GenericSingleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using Object = UnityEngine.Object;

namespace AGX.Input.Rebinding.Core.Scripts.Runtime.Rebinding
{
    public class InputManager : Singleton<InputManager>
    {
        public const int TimeoutSeconds = 12;

        private const string Mouse             = "Mouse";
        private const string KeyboardEscape    = "<Keyboard>/escape";
        private const string GamepadLeftStick  = "<Gamepad>/leftstick";
        private const string GamepadRightStick = "<Gamepad>/rightstick";

        [BoxGroup("References"), SerializeField, Required]
        private InputActionAsset? _inputActions;

        public InputActionAsset? InputActions
        {
            get
            {
                if (_inputActions == null)
                    Debug.LogError("InputActions asset is not set in the InputManager singleton. \n" +
                                   "1. Create an InputActions game object in the scene. \n" +
                                   "2. Assign the InputActionAsset to the InputManager singleton.");

                return _inputActions;
            }
        }

        public static event Action RebindComplete = delegate { };
        public static event Action RebindCanceled = delegate { };
        public static event Action<InputAction, int> RebindStarted = delegate { };

        private static List<ActionRebinder> _actionRebinders = new();

        public static event Action<int> RebindCountChanged = delegate { };

        private static readonly bool _debug = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            RebindComplete = delegate { };
            RebindCanceled = delegate { };
            RebindStarted = delegate { };
            _actionRebinders = new List<ActionRebinder>();
            RebindCountChanged = delegate { };
        }

        public void RefreshInputDevicePrompt()
        {
            RebindCountChanged?.Invoke(GetTotalBindingOverwriteCount());
        }

        public void StartRebind(string actionName, int bindingIndex, RebindOverlay rebindOverlay, bool includeMouse)
        {
            var action = InputActions.FindAction(actionName);

            if (action == null || action.bindings.Count <= bindingIndex)
            {
                Debug.Log("Couldn't find action or binding");
                return;
            }

            if (action.bindings[bindingIndex].isComposite)
            {
                var firstPartIndex = bindingIndex + 1;
                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                    DoRebind(action, firstPartIndex, rebindOverlay, includeMouse, true);
            }
            else
            {
                DoRebind(action, bindingIndex, rebindOverlay, includeMouse, false);
            }
        }

        private void DoRebind(InputAction actionToRebind, int bindingIndex, RebindOverlay rebindOverlay, bool includeMouse, bool allCompositeParts)
        {
            if (actionToRebind == null || bindingIndex < 0)
                return;

            actionToRebind.Disable();

            var rebind = actionToRebind
                .PerformInteractiveRebinding(bindingIndex)
                .OnMatchWaitForAnother(0.1f)
                .WithTimeout(TimeoutSeconds);

            rebind.OnComplete(operation =>
            {
                actionToRebind.Enable();
                rebindOverlay.Hide();
                operation.Dispose();

                rebindOverlay.SetIsDuplicate(false);

                var duplicateBinding = GetDuplicateBinding(actionToRebind, bindingIndex, allCompositeParts);

                if (duplicateBinding.WasFound)
                {
                    actionToRebind.RemoveBindingOverride(bindingIndex);
                    operation.Dispose();
                    DoRebind(actionToRebind, bindingIndex, rebindOverlay, includeMouse, allCompositeParts);
                    rebindOverlay.SetIsDuplicate(true, duplicateBinding.Binding, duplicateBinding.Action);
                    return;
                }

                if (allCompositeParts)
                {
                    var nextBindingsIndex = bindingIndex + 1;
                    if (nextBindingsIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingsIndex].isPartOfComposite)
                        DoRebind(actionToRebind, nextBindingsIndex, rebindOverlay, includeMouse, true);
                }

                RefreshInputDevicePrompt();
                SaveBindingOverride(actionToRebind);

                RebindComplete?.Invoke();
            });

            rebind.OnCancel(operation =>
            {
                actionToRebind.Enable();
                rebindOverlay?.Hide();
                operation.Dispose();

                RebindCanceled?.Invoke();
            });

            rebind.WithCancelingThrough(KeyboardEscape);

            if (!includeMouse)
                rebind.WithControlsExcluding(Mouse);


            if (_debug)
                Debug.Log($"Rebinding {actionToRebind.name} at index {bindingIndex} for map {actionToRebind.actionMap.name}");

            // Add filtering based on the input action's control type (Keyboard or Gamepad)
            if (actionToRebind.actionMap.name == "Keyboard")
            {
                rebind.WithControlsExcluding(GamepadLeftStick);
                rebind.WithControlsExcluding(GamepadRightStick);
            }
            else if (actionToRebind.actionMap.name == "Gamepad")
            {
                rebind.WithControlsExcluding(KeyboardEscape); // Exclude keyboard escape for gamepad actions
            }

            if (rebindOverlay != null)
            {
                rebindOverlay.Show(() => { rebind.Cancel(); });

                var text = new StringBuilder();

                text.Append("<color=white>Press any key");

                if (includeMouse) text.Append(" or mouse button");

                text.Append($" for </color>{actionToRebind.name}");

                text.Append(actionToRebind.bindings[bindingIndex].isPartOfComposite ?
                    $" '{actionToRebind.bindings[bindingIndex].name}'<color=white>...</color>" :
                    "<color=white>...</color>");

                rebindOverlay.SetText(text.ToString());
            }

            RebindStarted?.Invoke(actionToRebind, bindingIndex);
            rebind.Start(); //actually starts the rebinding
        }

        private class DuplicateBinding
        {
            public bool   WasFound;
            public string Action;
            public string Binding;

            public static DuplicateBinding None => new() { WasFound = false };
        }

        private DuplicateBinding GetDuplicateBinding(InputAction actionToRebind, int bindingIndex, bool allCompositeParts = false)
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

                // Get the localized name for the control (e.g., "Espace" for the Space key on the keyboard)
                var getBindingName = InputControlPath.ToHumanReadableString(newBinding.effectivePath);

                return new DuplicateBinding
                {
                    Action = binding.action,
                    WasFound = true,
                    Binding = getBindingName
                };
            }

            // Check for duplicate (composite) bindings
            if (!allCompositeParts)
                return DuplicateBinding.None;

            for (var i = 0; i < bindingIndex; ++i)
            {
                // Skip different paths
                if (actionToRebind.bindings[i].effectivePath != newBinding.effectivePath)
                    continue;

                Debug.LogWarning($"Duplicate binding found: {newBinding.effectivePath}");
                return new DuplicateBinding
                {
                    Binding = actionToRebind.bindings[i].action,
                    WasFound = true,
                    Action = actionToRebind.name
                };
            }

            return DuplicateBinding.None;
        }

        /// <summary>
        /// TODO we should export this to JSON so that it's easy to save to a single PlayerPrefs slot
        /// </summary>
        /// <param name="action"></param>
        private void SaveBindingOverride(InputAction action)
        {
            for (var i = 0; i < action.bindings.Count; i++)
            {
                var key = GetPlayerPrefKey(action, i);
                var value = action.bindings[i].overridePath;

                PlayerPrefs.SetString(key, value);
            }
        }

        public void LoadBindingOverrides()
        {
            // Iterate over all action maps in the InputActions asset
            foreach (var actionMap in InputActions.actionMaps)
            {
                foreach (var action in actionMap.actions)
                {
                    for (var i = 0; i < action.bindings.Count; i++)
                    {
                        var key = GetPlayerPrefKey(action, i);

                        var storedOverride = PlayerPrefs.GetString(key);

                        // No stored override, skip
                        if (string.IsNullOrEmpty(storedOverride)) continue;

                        Debug.Log($"Applying binding override for {action.name} at index {i}: {storedOverride}");
                        action.ApplyBindingOverride(i, storedOverride);
                    }
                }
            }
        }

        private string GetPlayerPrefKey(InputAction action, int bindingIndex)
        {
            var key = $"Input-Binding-{action.actionMap.name}-{action.name}-{bindingIndex}";
            return key;
        }

        public void ResetAllBindings()
        {
            // Find all rebind controls in the scene
            var rebindControls = Object.FindObjectsOfType<ActionRebinder>();

            Debug.Log($"Resetting all {rebindControls.Length} bindings...");

            foreach (var rebindControl in rebindControls)
                ResetBinding(rebindControl);
        }

        public void ResetBinding(ActionRebinder actionRebinder)
        {
            var actionName = actionRebinder.ActionName;
            var bindingIndex = actionRebinder.BindingStartIndexIndex;

            var action = InputActions.FindAction(actionName);

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

        public bool IsBindingOverriden(string actionName, int bindingIndex)
        {
            var action = InputActions.FindAction(actionName);

            if (action == null)
            {
                Debug.LogError($"Action '{actionName}' not found.");
                return false;
            }

            if (action.bindings.Count <= bindingIndex)
            {
                Debug.LogError($"Binding index {bindingIndex} out of range for action '{actionName}', which only has {action.bindings.Count} bindings.");
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

                if (_debug)
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

        internal int GetTotalBindingOverwriteCount()
        {
            var count = 0;

            foreach (var rebindControl in _actionRebinders)
            {
                if (IsBindingOverriden(rebindControl.ActionName, rebindControl.BindingStartIndexIndex))
                {
                    count++;
                }
            }

            return count;
        }

        public ReadOnlyArray<InputBinding> GetBindings(string actionName)
        {
            var action = InputActions.FindAction(actionName);

            if (action == null)
            {
                Debug.LogError($"Could not find action with name '{actionName}' in action map {InputActions.name}...");
                return new ReadOnlyArray<InputBinding>();
            }

            return action.bindings;
        }

        public InputAction GetAction(string actionName)
        {
            return InputActions.FindAction(actionName);
        }
    }
}