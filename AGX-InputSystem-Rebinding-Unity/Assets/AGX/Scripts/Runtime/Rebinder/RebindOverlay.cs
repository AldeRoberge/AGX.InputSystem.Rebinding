using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AGX.Scripts.Runtime.Rebinder
{
    public class RebindOverlay : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Required]
        private CanvasGroup _canvasGroup;

        [BoxGroup("References"), SerializeField, Required]
        private TextMeshProUGUI _text;

        [BoxGroup("References"), SerializeField, Required]
        private Button _buttonCancel;
        
        public void Show(Action cancelAction)
        {
            SetActive(true);
            _buttonCancel.onClick.AddListener(() =>
            {
                SetActive(false);
                cancelAction?.Invoke();
            });
        }

        public void Hide() => SetActive(false);

        private void SetActive(bool isActive)
        {
            if (isActive)
                gameObject.SetActive(true);

            _canvasGroup.alpha = isActive ? 1 : 0;
            _canvasGroup.blocksRaycasts = isActive;
            _canvasGroup.interactable = isActive;
        }

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}