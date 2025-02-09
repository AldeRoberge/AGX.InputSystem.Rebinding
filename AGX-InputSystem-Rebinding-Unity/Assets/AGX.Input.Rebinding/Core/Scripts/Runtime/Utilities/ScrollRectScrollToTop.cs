using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace AGX.Scripts.Runtime.Utilities
{
    /// <summary>
    /// Scrolls a ScrollRect to the top when the object is enabled.
    /// </summary>
    public class ScrollRectScrollToTop : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Required] private ScrollRect _scrollRect;

        private void Reset()
        {
            if (_scrollRect == null)
                _scrollRect = GetComponent<ScrollRect>();
        }

        private void Start()
        {
            ResetScrollPosition();
        }

        private void ResetScrollPosition()
        {
            // Ensure layout updates before setting position
            Canvas.ForceUpdateCanvases();

            // Set scroll position to the top
            _scrollRect.verticalNormalizedPosition = 1;
        }
    }
}