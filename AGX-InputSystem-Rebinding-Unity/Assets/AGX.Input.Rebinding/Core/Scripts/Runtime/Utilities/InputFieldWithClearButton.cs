using Sirenix.OdinInspector;
using UnityEngine;

namespace AGX.Scripts.Runtime.Utilities
{
    public class InputFieldWithClearButton : MonoBehaviour
    {
        [SerializeField, BoxGroup("References"), Required]
        private TMPro.TMP_InputField _inputField;

        [SerializeField, BoxGroup("References"), Required]
        private UnityEngine.UI.Button _clearButton;


        [SerializeField, BoxGroup("References")]
        private InputFieldHidePlaceholderOnSelection _inputFieldHidePlaceholderOnSelection;

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

            if (_inputFieldHidePlaceholderOnSelection != null)
                _inputFieldHidePlaceholderOnSelection.ShowPlaceholder();
        }
    }
}