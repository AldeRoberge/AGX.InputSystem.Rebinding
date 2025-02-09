using System;
using System.Collections.Generic;
using System.Linq;
using FredericRP.GenericSingleton;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

namespace AGX.Input.Rebinding.Core.Scripts.Runtime.Input
{
    public class InputDeviceManager : Singleton<InputDeviceManager>
    {
        public static event Action<InputDevice> OnDeviceConnected = delegate { };
        public static event Action<InputDevice> OnDeviceDisconnected = delegate { };
        public static event Action<InputDevice> OnDeviceUsed = delegate { };

        private InputDevice _currentDevice;

        public static InputDevice CurrentDevice => Instance._currentDevice;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            OnDeviceConnected = delegate { };
            OnDeviceDisconnected = delegate { };
            OnDeviceUsed = delegate { };
        }

        private void OnEnable()
        {
            InputSystem.onDeviceChange += HandleDeviceChange;
            InputSystem.onEvent += OnInputSystemEvent;
        }

        private void OnDisable()
        {
            InputSystem.onDeviceChange -= HandleDeviceChange;
            InputSystem.onEvent -= OnInputSystemEvent;
        }

        private void OnInputSystemEvent(InputEventPtr eventPtr, InputDevice device)
        {
            // Only process state events (actual input)
            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
                return;

            // Check if the device is one we care about
            if (device is Keyboard or Gamepad)
            {
                // For Keyboard and Gamepad, check if there's actual input data
                if (HasActualInput(device))
                {
                    UpdateCurrentDevice(device);
                }
            }
            else if (device is Mouse mouse)
            {
                // For Mouse, check for significant movement or button press
                if (HasSignificantMouseInput(mouse))
                {
                    UpdateCurrentDevice(device);
                }
            }
        }

        private bool HasActualInput(InputDevice device)
        {
            switch (device)
            {
                case Keyboard keyboard:
                    return keyboard.anyKey.isPressed;

                case Gamepad gamepad:
                    // Check for any button press or significant stick/trigger movement
                    return gamepad.allControls.Any(control =>
                    {
                        if (control is ButtonControl button)
                            return button.isPressed;
                        if (control is StickControl stick)
                            return stick.ReadValue().magnitude > 0.1f;
                        if (control is AxisControl axis)
                            return Math.Abs(axis.ReadValue()) > 0.1f;
                        return false;
                    });

                default:
                    return false;
            }
        }

        private bool HasSignificantMouseInput(Mouse mouse)
        {
            // Check for button presses
            if (mouse.leftButton.isPressed || mouse.rightButton.isPressed || mouse.middleButton.isPressed)
                return true;

            // Check for significant mouse movement
            Vector2 delta = mouse.delta.ReadValue();
            if (delta.magnitude > 0.5f) // Adjust threshold as needed
                return true;

            // Check for scroll
            Vector2 scroll = mouse.scroll.ReadValue();
            if (scroll.magnitude > 0)
                return true;

            return false;
        }

        private void UpdateCurrentDevice(InputDevice device)
        {
            if (_currentDevice != device)
            {
                _currentDevice = device;
                OnDeviceUsed?.Invoke(device);
                Debug.Log($"Input device switched to: {device.name}");
            }
        }

        private void HandleDeviceChange(InputDevice device, InputDeviceChange change)
        {
            Debug.Log($"Device {device} changed: {change}");

            switch (change)
            {
                case InputDeviceChange.Added:
                case InputDeviceChange.Reconnected:
                    OnDeviceConnected?.Invoke(device);
                    // If no current device, set this as current
                    if (_currentDevice == null)
                    {
                        UpdateCurrentDevice(device);
                    }

                    break;

                case InputDeviceChange.Removed:
                case InputDeviceChange.Disconnected:
                    OnDeviceDisconnected?.Invoke(device);
                    if (_currentDevice == device)
                    {
                        var fallback = GetFallbackDevice();
                        if (fallback != null)
                        {
                            UpdateCurrentDevice(fallback);
                        }
                        else
                        {
                            _currentDevice = null;
                        }
                    }

                    break;
            }
        }


        private void DetectDeviceUsage()
        {
            foreach (var device in InputSystem.devices)
            {
                if (device is Keyboard or Gamepad or Pointer)
                {
                    if (device.wasUpdatedThisFrame && device != _currentDevice)
                    {
                        _currentDevice = device;
                        OnDeviceUsed?.Invoke(device);
                    }
                }
            }
        }

        private InputDevice GetFallbackDevice()
        {
            // Try to find a keyboard first
            var keyboard = InputSystem.GetDevice<Keyboard>();
            if (keyboard != null) return keyboard;

            // Then try gamepad
            var gamepad = InputSystem.GetDevice<Gamepad>();
            if (gamepad != null) return gamepad;

            // Otherwise, return null
            return null;
        }


        public static List<string> GetBindingDisplayStrings(string controlScheme, InputAction action)
        {
            var bindingDisplayList = new List<string>();
            // Create a binding mask for the control scheme.
            var bindingMask = InputBinding.MaskByGroup(controlScheme);
            int bindingCount = action.bindings.Count;

            for (int i = 0; i < bindingCount; i++)
            {
                var binding = action.bindings[i];

                // Skip bindings that don't match the control scheme.
                if (!bindingMask.Matches(binding))
                {
                    continue;
                }


                // For a simple binding (not composite and not part of one), convert its effective path.
                if (binding.isComposite == false && binding.isPartOfComposite == false)
                {
                    Debug.Log($"Simple Binding: {binding.effectivePath}");
                    bindingDisplayList.Add(binding.effectivePath);
                }
                // For composite bindings, process the composite parent and its children.
                else if (binding.isComposite || binding.isPartOfComposite)
                {
                    Debug.Log($"Composite Binding: {binding.effectivePath}");

                    int j = i;

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
                bindingDisplayList.Add("???");
            }

            // (Optional) Log the final list.
            Debug.Log($"Binding Display List: {string.Join(", ", bindingDisplayList)}");

            return bindingDisplayList;
        }


        public static bool IsDeviceOfScheme(InputDevice device, string scheme)
        {
            return scheme.ToLower() switch
            {
                "keyboard" => device is Keyboard,
                "gamepad" => device is Gamepad,
                _ => false
            };
        }

        public static string GetCurrentControlScheme()
        {
            return CurrentDevice switch
            {
                Keyboard => "Keyboard",
                Gamepad => "Gamepad",
                Pointer => "Mouse",
                _ => "Unknown"
            };
        }

        public static bool IsSchemeActive(string scheme)
        {
            if (CurrentDevice == null) return false;
            return IsDeviceOfScheme(CurrentDevice, scheme);
        }
    }
}