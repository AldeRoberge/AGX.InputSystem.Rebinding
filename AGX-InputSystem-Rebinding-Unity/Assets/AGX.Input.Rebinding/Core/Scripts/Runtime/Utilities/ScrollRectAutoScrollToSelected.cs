using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AGX.Input.Rebinding.Core.Scripts.Runtime.Utilities
{
    public class ScrollRectAutoScrollToSelected : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Required] private ScrollRect scrollRect;

        [BoxGroup("References"), SerializeField, Required] private RectTransform contentPanel;

        [BoxGroup("References"), SerializeField, Required] private float scrollMargin = 20f; // Margin from top/bottom of scroll view

        private void Reset()
        {
            if (scrollRect == null)
                scrollRect = GetComponent<ScrollRect>();

            if (contentPanel == null)
                contentPanel = scrollRect.content;
        }

        private void Update()
        {
            // Check if there's a currently selected UI element
            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
            if (selectedObject == null) return;

            // Ensure the selected object is a child of our scroll content
            if (!selectedObject.transform.IsChildOf(contentPanel.transform)) return;

            // Get the RectTransform of the selected object
            RectTransform selectedRect = selectedObject.GetComponent<RectTransform>();
            if (selectedRect == null) return;

            // Calculate the positions in scroll view space
            Vector3[] selectedCorners = new Vector3[4];
            Vector3[] scrollCorners = new Vector3[4];

            selectedRect.GetWorldCorners(selectedCorners);
            scrollRect.viewport.GetWorldCorners(scrollCorners);

            // Convert positions to local viewport space
            float selectedTop = TransformToViewportSpace(selectedCorners[1]).y;
            float selectedBottom = TransformToViewportSpace(selectedCorners[0]).y;
            float scrollTop = TransformToViewportSpace(scrollCorners[1]).y;
            float scrollBottom = TransformToViewportSpace(scrollCorners[0]).y;

            // Calculate the normalized position to scroll to
            float newVerticalNormalizedPosition = scrollRect.verticalNormalizedPosition;

            // If selected object is above visible area
            if (selectedTop > scrollTop - scrollMargin)
            {
                float overflow = (selectedTop - scrollTop + scrollMargin) / (scrollRect.viewport.rect.height);
                newVerticalNormalizedPosition += overflow;
            }
            // If selected object is below visible area
            else if (selectedBottom < scrollBottom + scrollMargin)
            {
                float overflow = (selectedBottom - scrollBottom - scrollMargin) / (scrollRect.viewport.rect.height);
                newVerticalNormalizedPosition += overflow;
            }

            // Clamp and apply the new scroll position
            newVerticalNormalizedPosition = Mathf.Clamp01(newVerticalNormalizedPosition);
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(
                scrollRect.verticalNormalizedPosition,
                newVerticalNormalizedPosition,
                Time.deltaTime * 10f
            );
        }

        private Vector2 TransformToViewportSpace(Vector3 worldPosition)
        {
            Vector2 viewportPoint = scrollRect.viewport.InverseTransformPoint(worldPosition);
            return viewportPoint;
        }
    }
}