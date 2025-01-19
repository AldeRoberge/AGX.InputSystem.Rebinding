using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AGX.Scripts.Runtime.Utilities
{
    public class MatchFontSize : MonoBehaviour
    {
        [SerializeField] private List<TextMeshProUGUI> _texts = new();

        private       float _lastUpdateTime;
        private const float UpdateIntervalSeconds = 0.01f;

        private void Start()
        {
            if (_texts.Count == 0) return; // Early exit if there are no texts to adjust

            // Adjust font size at the start
            AdjustFontSize();
        }

        private void Update()
        {
            // Check if enough time has passed since the last update
            if (Time.time - _lastUpdateTime >= UpdateIntervalSeconds)
            {
                _lastUpdateTime = Time.time;
                AdjustFontSize();
            }
        }

        private void AdjustFontSize()
        {
            EnableAutoSizing();
            var smallestFontSize = GetSmallestAutoSizedFontSize();
            ApplyFontSize(smallestFontSize);
        }

        private void EnableAutoSizing()
        {
            // Enable auto sizing for all valid TextMeshProUGUI components
            foreach (var text in _texts)
            {
                if (text != null)
                {
                    text.enableAutoSizing = true;
                    text.ForceMeshUpdate(); // Force mesh update to ensure auto-sizing is applied
                }
            }
        }

        private float GetSmallestAutoSizedFontSize()
        {
            var smallestFontSize = float.MaxValue;

            // Loop through each text and find the smallest font size after auto-sizing
            foreach (var text in _texts)
            {
                if (text == null) continue; // Skip null entries

                // Get the resulting font size after auto-sizing has been applied
                var currentFontSize = text.fontSize;

                // Update smallest font size if a smaller one is found
                if (currentFontSize < smallestFontSize)
                {
                    smallestFontSize = currentFontSize;
                }
            }

            return smallestFontSize;
        }

        private void ApplyFontSize(float fontSize)
        {
            // Apply the smallest font size and disable auto-sizing
            foreach (var text in _texts)
            {
                if (text != null)
                {
                    text.fontSize = fontSize;
                    text.enableAutoSizing = false; // Disable auto-sizing after applying the font size
                }
            }
        }
    }
}