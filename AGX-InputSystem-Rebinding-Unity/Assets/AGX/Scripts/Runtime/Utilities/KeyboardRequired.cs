using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Scripts.Runtime.Utilities
{
    public class KeyboardRequired : MonoBehaviour
    {
        private bool keyboardDetected;

        void Start()
        {
            InputSystem.onDeviceChange += (device, change) =>
            {
                if (device is Keyboard)
                    CheckKeyboardStatus();
            };
        }

        private void CheckKeyboardStatus()
        {
            if (Keyboard.current != null && !keyboardDetected)
            {
                keyboardDetected = true;
                Debug.Log($"Keyboard plugged in. Component {name} enabled.");
                gameObject.SetActive(true);
            }
            else if (Keyboard.current == null && keyboardDetected)
            {
                keyboardDetected = false;
                Debug.LogWarning($"Keyboard unplugged. Component {name} disabled.");
                gameObject.SetActive(false);
            }
        }
    }
}