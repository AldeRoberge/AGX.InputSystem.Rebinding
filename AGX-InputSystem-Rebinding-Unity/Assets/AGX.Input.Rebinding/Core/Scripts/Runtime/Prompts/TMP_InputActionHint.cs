using System;
using System.Collections.Generic;
using System.Text;
using AGX.Input.Rebinding.Core.Scripts.Runtime.Input;
using AGX.Scripts.Runtime.Icons;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Input.Rebinding.Core.Scripts.Runtime.Prompts
{
    [ExecuteAlways]
    public class TMPInputActionHint : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Required]
        private TextMeshProUGUI? _text;

        [BoxGroup("References"), SerializeField, Required, OnValueChanged(nameof(UpdateText))]
        private InputActionAsset? _inputAction;

        [BoxGroup("Data"), SerializeField, Required, OnValueChanged(nameof(UpdateText))]
        private InputActionReference? _action;

        [BoxGroup("Data"), SerializeField, Required, OnValueChanged(nameof(UpdateText))]
        private string _hintTextTemplate = "Press {0} to do action.";

        [BoxGroup("Data"), SerializeField, Required, OnValueChanged(nameof(UpdateText))]
        private bool _showActionSprite;

        [BoxGroup("Debug"), ShowInInspector, ReadOnly]
        private string _currentControlScheme = "Keyboard";

        [BoxGroup("Debug"), ShowInInspector, ReadOnly]
        private string ActionName => _action?.action?.name;

        private void Reset()
        {
            if (_text == null) _text = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            UpdateText();
            InputDeviceManager.OnDeviceConnected += OnDeviceConnected;
            InputDeviceManager.OnDeviceDisconnected += OnDeviceDisconnected;
            InputDeviceManager.OnDeviceUsed += OnDeviceUsed;

            // Initialize current control scheme
            if (InputDeviceManager.CurrentDevice is Gamepad)
            {
                _currentControlScheme = "Gamepad";
            }
        }

        private void OnDisable()
        {
            InputDeviceManager.OnDeviceConnected -= OnDeviceConnected;
            InputDeviceManager.OnDeviceDisconnected -= OnDeviceDisconnected;
            InputDeviceManager.OnDeviceUsed -= OnDeviceUsed;
        }

        private void OnDeviceConnected(InputDevice device)
        {
            UpdateCurrentControlScheme(device);
            UpdateText();
        }

        private void OnDeviceDisconnected(InputDevice device)
        {
            // If the disconnected device was the current one, fall back to keyboard
            if (IsCurrentDevice(device))
            {
                _currentControlScheme = "Keyboard";
            }

            UpdateText();
        }

        private void OnDeviceUsed(InputDevice device)
        {
            UpdateCurrentControlScheme(device);
            UpdateText();
        }

        private void UpdateCurrentControlScheme(InputDevice device)
        {
            Debug.Log($"Device used: {device}");

            switch (device)
            {
                case Gamepad:
                    Debug.Log("Gamepad connected");
                    _currentControlScheme = "Gamepad";
                    break;
                case Keyboard or Mouse:
                    Debug.Log("Keyboard connected");
                    _currentControlScheme = "Keyboard";
                    break;
                default:
                    Debug.LogWarning($"Unknown device type: {device}");
                    break;
            }
        }

        private bool IsCurrentDevice(InputDevice device)
        {
            return (device is Gamepad && _currentControlScheme == "Gamepad") ||
                   (device is Keyboard && _currentControlScheme == "Keyboard");
        }

        private void UpdateText()
        {
            // Ensure required references exist.
            if (_inputAction == null || _text == null)
                return;

            if (_action == null)
            {
                Debug.LogWarning("No action reference set.");
                _text.text = string.Format(_hintTextTemplate, "???");
                return;
            }

            // Get the binding display string for the current control scheme
            List<string> bindingDisplay = InputDeviceManager.GetBindingDisplayStrings(_currentControlScheme, _action);


            // Remove any "Chat/" prefix from the action name.
            string cleanedActionName = ActionName.Replace("Chat/", "");
            string actionSprite = SpriteMapReference.GetSprite(cleanedActionName);


            StringBuilder allSprites = new StringBuilder();

            foreach (string binding in bindingDisplay)
            {
                // Format the binding display based on the control scheme
                string formattedBinding = FormatBindingForDisplay(binding);

                string bindingSprite = SpriteMapReference.GetSprite(formattedBinding);

                if (string.IsNullOrEmpty(bindingSprite))
                {
                    allSprites.Append(formattedBinding);
                }
                else
                {
                    allSprites.Append(bindingSprite);
                }
            }

/*
            // Build the hint text based on whether to show the action sprite.
            string hintText = _showActionSprite ?
                string.Format(_hintTextTemplate, bindingSprite, $"{actionSprite} {cleanedActionName}") :
                string.Format(_hintTextTemplate, bindingSprite, $"{cleanedActionName}");*/


            // Build the hint text based on whether to show the action sprite.
            string hintText = _showActionSprite ?
                string.Format(_hintTextTemplate, allSprites, $"{actionSprite} {cleanedActionName}") :
                string.Format(_hintTextTemplate, allSprites, $"{cleanedActionName}");

            _text.text = hintText;
        }

        private string FormatBindingForDisplay(string bindingDisplay)
        {
            // Remove '<' and '>' characters
            string cleanBinding = bindingDisplay.Replace("<", "").Replace(">", "");

            // Add a '/' prefix if it doesn't already exist
            if (!cleanBinding.StartsWith("/"))
                cleanBinding = "/" + cleanBinding;


            return cleanBinding;
        }
    }
}