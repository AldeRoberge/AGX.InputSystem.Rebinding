using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AGX.Scripts.Runtime.Utilities
{
    /// <summary>
    /// Allows hiding the placeholder immediately when the input field is selected without waiting
    /// for the user to start typing.
    /// </summary>
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFieldHidePlaceholderOnSelection : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [BoxGroup("References"), SerializeField, Required]
        private TMP_InputField _inputField;

        // Since the TMP_InputField controls the activation state of the placeholder,
        // We create an empty parent object to control the activation state of the placeholder.
        [BoxGroup("References"), SerializeField, Required]
        private GameObject _placeholder;

        private void Reset()
        {
            if (_inputField == null) _inputField = GetComponent<TMP_InputField>();
        }

        public void OnSelect(BaseEventData eventData)
        {
            Debug.Log("OnSelect");

            if (_placeholder != null)
                _placeholder.SetActive(false);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            Debug.Log("OnDeselect");

            if (string.IsNullOrEmpty(_inputField.text))
                ShowPlaceholder();
        }

        public void ShowPlaceholder()
        {
            if (_placeholder != null)
                _placeholder.SetActive(true);
        }
    }
}