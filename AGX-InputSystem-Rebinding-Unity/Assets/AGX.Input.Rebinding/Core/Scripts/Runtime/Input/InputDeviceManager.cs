using System;
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

        private InputDevice? _currentDevice;

        public static InputDevice? CurrentDevice => Instance?._currentDevice;

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
            var delta = mouse.delta.ReadValue();
            if (delta.magnitude > 0.5f)
                return true;

            // Check for scroll
            var scroll = mouse.scroll.ReadValue();
            return scroll.magnitude > 0;
        }

        private void UpdateCurrentDevice(InputDevice device)
        {
            if (_currentDevice == device) return;
            _currentDevice = device;
            OnDeviceUsed?.Invoke(device);
            Debug.Log($"Input device switched to: {device.name}");
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

        private InputDevice GetFallbackDevice()
        {
            // Try to find a keyboard first
            var keyboard = InputSystem.GetDevice<Keyboard>();
            if (keyboard != null) return keyboard;

            // Then try gamepad
            var gamepad = InputSystem.GetDevice<Gamepad>();
            if (gamepad != null)
                return gamepad;

            // Then try mouse
            var mouse = InputSystem.GetDevice<Mouse>();
            if (mouse != null)
                return mouse;

            Debug.LogWarning("No fallback device found");
            return null;
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