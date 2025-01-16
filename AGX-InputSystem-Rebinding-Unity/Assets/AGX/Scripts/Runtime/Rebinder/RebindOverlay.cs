using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace AGX.Scripts.Runtime.Rebinder
{
    public class RebindOverlay : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField]
        private CanvasGroup _canvasGroup;

        [BoxGroup("References"), SerializeField]
        private TextMeshProUGUI _text;
        
        public void SetActive(bool isActive)
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