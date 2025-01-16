using System;
using NaughtyAttributes;
using UnityEngine;

namespace AGX.Scripts.Utilities
{
    public class InputFieldWithClearButton : MonoBehaviour
    {
        [SerializeField, BoxGroup("References")]
        private TMPro.TMP_InputField _inputField;

        [SerializeField, BoxGroup("References")]
        private UnityEngine.UI.Button _clearButton;

        private void Reset()
        {
            if (_inputField == null) _inputField = GetComponent<TMPro.TMP_InputField>();
            if (_clearButton == null) _clearButton = GetComponent<UnityEngine.UI.Button>();
        }

        private void Start()
        {
            _inputField.onValueChanged.AddListener((value) =>
            {
                var hasValue = !string.IsNullOrEmpty(value);
                _clearButton.gameObject.SetActive(hasValue);
            });
            _clearButton.onClick.AddListener(Clear);

            Clear();
        }

        private void Clear()
        {
            _inputField.text = string.Empty;
            _clearButton.gameObject.SetActive(false);
        }
    }
}