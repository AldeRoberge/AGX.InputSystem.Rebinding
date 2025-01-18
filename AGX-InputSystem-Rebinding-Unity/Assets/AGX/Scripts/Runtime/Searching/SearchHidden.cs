using UnityEngine;

namespace AGX.Scripts.Runtime.Searching
{
    // Use this on game objects you want automatically hidden (filtered out) when searching.
    public class SearchHidden : MonoBehaviour, ISearchable
    {
        public string[] SearchKeywords => null;
    }
}