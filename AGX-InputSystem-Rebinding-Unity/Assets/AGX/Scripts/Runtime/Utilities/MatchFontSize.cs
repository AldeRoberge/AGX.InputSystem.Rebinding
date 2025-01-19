using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// From https://discussions.unity.com/t/textmeshpro-precull-dorebuilds-performance/762282/27
// https://github.com/mitay-walle/Unity3d-TMPro-Text-AutoSize
namespace AGX.Scripts.Runtime.Utilities
{
    public enum ResizePattern
    {
        IgnoreRichText,
        AllCharacters,
    }

    [ExecuteAlways]
    public class TMPTextAutoSize : MonoBehaviour
    {
        [SerializeField] private List<TMP_Text> _texts = new();
        [SerializeField] private ResizePattern  _pattern;
        [SerializeField] private bool           _executeOnUpdate = true;
        private                  int            _currentIndex;

        private void Update()
        {
            if (_executeOnUpdate) Execute();
            OnUpdateCheck();
        }

        private void Execute()
        {
            if (_texts.Count == 0) return;

            var count = _texts.Count;

            var index = 0;
            float maxLength = 0;

            for (var i = 0; i < count; i++)
            {
                float length = 0;

                switch (_pattern)
                {
                    case ResizePattern.IgnoreRichText:
                        length = _texts[i].GetParsedText().Length;
                        break;

                    case ResizePattern.AllCharacters:
                        length = _texts[i].text.Length;

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (length > maxLength)
                {
                    maxLength = length;
                    index = i;
                }
            }

            if (_currentIndex != index)
            {
                OnChanged(index);
            }
        }

        private void OnChanged(int index)
        {
            // Disable auto size on previous
            _texts[_currentIndex].enableAutoSizing = false;

            _currentIndex = index;

            // Force an update of the candidate text object so we can retrieve its optimum point size.
            _texts[index].enableAutoSizing = true;
            _texts[index].ForceMeshUpdate();
        }

        private void OnUpdateCheck()
        {
            var optimumPointSize = _texts[_currentIndex].fontSize;

            // Iterate over all other text objects to set the point size
            var count = _texts.Count;

            for (var i = 0; i < count; i++)
            {
                if (_currentIndex == i) continue;

                _texts[i].enableAutoSizing = false;

                _texts[i].fontSize = optimumPointSize;
            }
        }
    }
}