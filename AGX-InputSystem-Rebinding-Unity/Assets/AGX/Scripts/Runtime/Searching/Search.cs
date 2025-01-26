﻿using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace AGX.Scripts.Runtime.Searching
{
    public class Search : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _searchField;

        [SerializeField]
        private GameObject _searchablesRoot;

        [SerializeField]
        private GameObject _noResultsGameObject;

        private List<ISearchable> _searchables = new();

        private void Awake()
        {
            if (_searchablesRoot != null)
            {
                _searchables = _searchablesRoot
                    .GetComponentsInChildren<MonoBehaviour>(true)
                    .OfType<ISearchable>()
                    .ToList();
            }

            _searchField.onValueChanged.AddListener(OnSearchInputChanged);

            _noResultsGameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _searchField.onValueChanged.RemoveListener(OnSearchInputChanged);
        }

        private void OnSearchInputChanged(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                // If search query is empty, show all searchables
                SetSearchablesActive(true);
                return;
            }

            // Filter searchables based on keywords
            var matchingSearchables = _searchables.Where(s => s.SearchKeywords.Any(keyword =>
                    keyword.Contains(searchQuery, System.StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (matchingSearchables.Count == 0) 
                Debug.LogWarning("No search result.");

            // Update visibility of searchables in list (filtered by search query)
            foreach (var searchable in _searchables)
            {
                if (searchable is MonoBehaviour monoBehaviour)
                {
                    // ensure it's not the 'no results' object
                    if (monoBehaviour.gameObject == _noResultsGameObject)
                        continue;

                    monoBehaviour.gameObject.SetActive(matchingSearchables.Contains(searchable));
                }
            }

            _noResultsGameObject.SetActive(matchingSearchables.Count == 0);
        }

        private void SetSearchablesActive(bool active)
        {
            foreach (var searchable in _searchables)
            {
                if (searchable is MonoBehaviour monoBehaviour)
                {
                    // ensure it's not the 'no results' object
                    if (monoBehaviour.gameObject == _noResultsGameObject)
                        continue;

                    monoBehaviour.gameObject.SetActive(active);
                }
            }
        }
    }
}