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
        private const string Gamepad  = "Gamepad";
        private const string Keyboard = "Keyboard";

        [BoxGroup("References"), SerializeField, Required]
        private TextMeshProUGUI? _text;

        [BoxGroup("References"), SerializeField, Required, OnValueChanged(nameof(UpdateText))]
        private InputActionAsset? _inputAction;

        [BoxGroup("Data"), SerializeField, Required, OnValueChanged(nameof(UpdateText))]
        private string _textTemplate = "Press {0} to do action.";

        [BoxGroup("Data"), SerializeField, Required, OnValueChanged(nameof(UpdateText))]
        private InputActionReference? _action;

        [BoxGroup("Data"), SerializeField, Required, OnValueChanged(nameof(UpdateText))]
        private bool _showActionSprite;

        [BoxGroup("Data/Advanced"), SerializeField, Required]
        private bool _overwriteActionName;

        [BoxGroup("Data/Advanced"), SerializeField, Required, ShowIf("_overwriteActionName")]
        private string _customActionName;

        [BoxGroup("Debug"), ShowInInspector, ReadOnly]
        private string _currentControlScheme = Keyboard;

        [BoxGroup("Debug"), ShowInInspector, ReadOnly]
        private string ActionName => _action?.action?.name;

        private void Reset()
        {
            if (_text == null)
                _text = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            UpdateText();
            InputDeviceManager.OnDeviceConnected += OnDeviceConnected;
            InputDeviceManager.OnDeviceDisconnected += OnDeviceDisconnected;
            InputDeviceManager.OnDeviceUsed += OnDeviceUsed;

            // Initialize current control scheme
            _currentControlScheme = InputDeviceManager.CurrentDevice switch
            {
                UnityEngine.InputSystem.Gamepad => Gamepad,
                UnityEngine.InputSystem.Keyboard or Mouse => Keyboard,
                _ => _currentControlScheme
            };
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
                _currentControlScheme = Keyboard;
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
                case UnityEngine.InputSystem.Gamepad:
                    Debug.Log("Gamepad connected");
                    _currentControlScheme = Gamepad;
                    break;
                case UnityEngine.InputSystem.Keyboard or Mouse:
                    Debug.Log("Keyboard connected");
                    _currentControlScheme = Keyboard;
                    break;
                default:
                    Debug.LogWarning($"Unknown device type: {device}");
                    break;
            }
        }

        private bool IsCurrentDevice(InputDevice device)
        {
            return (device is Gamepad && _currentControlScheme == Gamepad) ||
                   (device is Keyboard && _currentControlScheme == Keyboard);
        }

        private void UpdateText()
        {
            // Ensure required references exist.
            if (_inputAction == null || _text == null)
                return;

            if (_action == null)
            {
                Debug.LogWarning("No action reference set.");
                _text.text = string.Format(_textTemplate, "???");
                return;
            }

            // Get the binding display string for the current control scheme
            var bindingDisplay = BindingDisplayStringUtils.GetBindingDisplayStrings(_currentControlScheme, _action);


            // Remove any "Chat/" prefix from the action name.

            var cleanedActionName = _overwriteActionName ?
                _customActionName :
                ActionName;

            var actionSprite = SpriteMapReference.GetSprite(cleanedActionName);


            var allSprites = new StringBuilder();

            foreach (var binding in bindingDisplay)
            {
                // Format the binding display based on the control scheme
                var formattedBinding = FormatBindingForDisplay(binding);

                var bindingSprite = SpriteMapReference.GetSprite(formattedBinding);

                allSprites.Append(string.IsNullOrEmpty(bindingSprite) ?
                    formattedBinding :
                    bindingSprite);
            }

            var hintText = _showActionSprite ?
                string.Format(_textTemplate, allSprites, $"{actionSprite} {cleanedActionName}") :
                string.Format(_textTemplate, allSprites, $"{cleanedActionName}");

            _text.text = hintText;
        }

        private static string FormatBindingForDisplay(string bindingDisplay)
        {
            if (string.IsNullOrEmpty(bindingDisplay))
                return "";

            // Remove '<' and '>' characters
            var cleanBinding = bindingDisplay.Replace("<", "").Replace(">", "");

            // Add a '/' prefix if not present
            if (!cleanBinding.StartsWith("/"))
                cleanBinding = "/" + cleanBinding;

            return cleanBinding;
        }
    }
}