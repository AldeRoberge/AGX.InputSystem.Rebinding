#nullable enable
using System;
using Generator.Scripts.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Scripts.Rebinder
{
    public class InputManager : MonoBehaviour
    {
        private static InputActions? _playerInputActions; // your generated input system class

        public static event Action RebindComplete = delegate { };
        public static event Action RebindCanceled = delegate { };
        public static event Action<InputAction, int> RebindStarted = delegate { };

        public static void StartRebind(string actionName, int bindingIndex, RebindOverlay rebindOverlay, bool excludeMouse)
        {
            _playerInputActions ??= new InputActions();

            var action = _playerInputActions.asset.FindAction(actionName);
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

                if (CheckDuplicateBindings(actionToRebind, bindingIndex, allCompositeParts))
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
                    ? $"{partName} Waiting for input..."
                    : $"{partName} Waiting for input...";

                rebindOverlay.SetText(text);
            }

            RebindStarted?.Invoke(actionToRebind, bindingIndex);
            rebind.Start(); //actually starts the rebinding
        }

        // Only checks for duplicates within the same action map.
        private static bool CheckDuplicateBindings(InputAction actionToRebind, int bindingIndex, bool allCompositeParts = false)
        {
            var newBinding = actionToRebind.bindings[bindingIndex];
            foreach (var binding in actionToRebind.actionMap.bindings)
            {
                if (binding.action == newBinding.action)
                {
                    continue;
                }

                if (binding.effectivePath == newBinding.effectivePath)
                {
                    Debug.Log("Duplicate binding found" + newBinding.effectivePath);
                    return true;
                }
            }

            //Check for duplicate composite bindings
            if (allCompositeParts)
            {
                for (var i = 0; i < bindingIndex; ++i)
                {
                    if (actionToRebind.bindings[i].effectivePath == newBinding.effectivePath)
                    {
                        Debug.Log("Duplicate binding found" + newBinding.effectivePath);
                        return true;
                    }
                }
            }

            return false;
        }

        public static string GetBindingName(string actionName, int bindingIndex)
        {
            _playerInputActions ??= new InputActions();

            var action = _playerInputActions.asset.FindAction(actionName);
            return action.GetBindingDisplayString(bindingIndex);
        }

        private static void SaveBindingOverride(InputAction action)
        {
            for (var i = 0; i < action.bindings.Count; i++)
            {
                PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
            }
        }

        public static void LoadBindingOverride(string actionName)
        {
            _playerInputActions ??= new InputActions();

            var action = _playerInputActions.asset.FindAction(actionName);

            for (var i = 0; i < action.bindings.Count; i++)
            {
                if (!string.IsNullOrEmpty(PlayerPrefs.GetString(action.actionMap + action.name + i)))
                    action.ApplyBindingOverride(i, PlayerPrefs.GetString(action.actionMap + action.name + i));
            }
        }

        public static void ResetBinding(string actionName, int bindingIndex)
        {
            var action = _playerInputActions.asset.FindAction(actionName);

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
                action.RemoveBindingOverride(bindingIndex);

            SaveBindingOverride(action);
        }
    }
}