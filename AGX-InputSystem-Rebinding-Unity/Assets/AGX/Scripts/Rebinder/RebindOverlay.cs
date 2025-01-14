using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace AGX.Scripts.Rebinder
{
    public class RebindOverlay : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField]
        private CanvasGroup _canvasGroup;

        [BoxGroup("References"), SerializeField]
        private TextMeshProUGUI _text;
        
        public void SetActive(bool p0)
        {
            if (p0) 
                gameObject.SetActive(true);

            _canvasGroup.alpha = p0 ? 1 : 0;
            _canvasGroup.blocksRaycasts = p0;
            _canvasGroup.interactable = p0;
        }

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}