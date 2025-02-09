using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace AGX.Input.Rebinding.Scripts.Runtime.Rebinding
{
    public class RebindOverlay : MonoBehaviour
    {
        private const int TimeOutThreshold = 5;

        [BoxGroup("References"), SerializeField, Required] private CanvasGroup     _canvasGroup;
        [BoxGroup("References"), SerializeField, Required] private TextMeshProUGUI _text;
        [BoxGroup("References"), SerializeField, Required] private TextMeshProUGUI _duplicateWarning;
        [BoxGroup("References"), SerializeField, Required] private Button          _buttonCancel;
        [BoxGroup("References"), SerializeField, Required] private TextMeshProUGUI _buttonCancelText;
        [BoxGroup("References"), SerializeField, Required] private Image           _buttonImageFill;

        // Reference to the default UI input action
        [BoxGroup("References"), SerializeField, Required] private InputSystemUIInputModule _inputSystemUiInputModule;

        private Coroutine _timeoutCoroutine;

        public void Show(Action cancelAction)
        {
            _duplicateWarning.text = "Waiting for input...";

            _inputSystemUiInputModule.enabled = false;

            _buttonCancel.onClick.AddListener(() =>
            {
                Debug.Log("Cancel button clicked");
                SetActive(false);
                cancelAction?.Invoke();
            });
            SetActive(true);

            if (_timeoutCoroutine != null)
                StopCoroutine(_timeoutCoroutine);
            _timeoutCoroutine = StartCoroutine(StartTimeout());
        }

        private IEnumerator StartTimeout()
        {
            float remainingTime = InputManager.TimeoutSeconds;

            _buttonImageFill.fillAmount = 0;

            while (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;

                bool isBeyondThreshold = remainingTime < TimeOutThreshold;

                _buttonCancelText.text = isBeyondThreshold ?
                    $"Cancelling in {Mathf.RoundToInt(remainingTime) + 1}s" :
                    "Click to cancel";

                if (isBeyondThreshold)
                    _buttonImageFill.fillAmount = 1 - remainingTime / InputManager.TimeoutSeconds;
                else
                    _buttonImageFill.fillAmount = 0;

                yield return null;
            }

            _buttonCancelText.text = "Time's up!";
        }

        public void Hide() => SetActive(false);

        private void SetActive(bool isActive)
        {
            _inputSystemUiInputModule.enabled = true;

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


        public void SetIsDuplicate(bool isDuplicate, string binding = "", string action = "")
        {
            _duplicateWarning.gameObject.SetActive(isDuplicate);

            if (!isDuplicate) return;

            _duplicateWarning.text = $"<b>{binding}</b><color=white> is already used for </color><b>{action}</b>";

            _duplicateWarning.transform.DOPunchScale(Vector3.one * 0.1f, 0.25f, 5, 0.5f);
        }
    }
}