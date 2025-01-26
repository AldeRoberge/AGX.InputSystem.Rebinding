using System;
using LitMotion;
using LitMotion.Extensions;
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
        private TextMeshProUGUI _duplicateWarning;

        [BoxGroup("References"), SerializeField, Required]
        private Button _buttonCancel;

        public void Show(Action cancelAction)
        {
            _buttonCancel.onClick.AddListener(() =>
            {
                Debug.Log("Cancel button clicked");
                SetActive(false);
                cancelAction?.Invoke();
            });
            SetActive(true);
        }

        public void Hide() => SetActive(false);

        private void SetActive(bool isActive)
        {
            if (isActive)
                gameObject.SetActive(true);

            _canvasGroup.alpha = isActive ?
                1 :
                0;
            _canvasGroup.blocksRaycasts = isActive;
            _canvasGroup.interactable = isActive;
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        private MotionHandle _punchScaleTween;

        public void SetIsDuplicate(bool p0, string action = "", string binding = "")
        {
            Debug.Log($"SetIsDuplicate({p0})");
            
            _duplicateWarning.gameObject.SetActive(p0);

            if (p0)
            {
                _duplicateWarning.text = $"Action {action} is already binded to {binding}";

                if (_punchScaleTween != default && _punchScaleTween.IsActive())
                {
                    _punchScaleTween.TryComplete();
                }

                _punchScaleTween = LMotion.Punch.Create(_duplicateWarning.transform.position.x, 3f, 0.5f) // Create a Punch motion (regular damping oscillation)
                    .WithFrequency(5) // Specify oscillation count
                    .WithDampingRatio(0f) // Specify damping ratio
                    .BindToPositionX(_duplicateWarning.transform); // Bind to transform.position.x
            }
        }
    }
}