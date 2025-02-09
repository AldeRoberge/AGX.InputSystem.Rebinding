using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace AGX.Input.Rebinding.Core.Scripts.Runtime
{
    public class RebindUI : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Required]
        private CanvasGroup _canvasGroup;

        [BoxGroup("References"), SerializeField, Required]
        private Button _closeButton;

        [BoxGroup("References"), SerializeField, Required]
        private Button _openButton;

        private void Awake()
        {
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
            _openButton.onClick.AddListener(OnOpenButtonClicked);
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
            _openButton.onClick.RemoveListener(OnOpenButtonClicked);
        }

        private void OnOpenButtonClicked()
        {
            _canvasGroup.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
            _canvasGroup.DOFade(1, 0.5f);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        private void OnCloseButtonClicked()
        {
            _canvasGroup.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
            _canvasGroup.DOFade(0, 0.5f);
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}