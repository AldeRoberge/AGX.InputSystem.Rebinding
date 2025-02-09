using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace AGX.Input.Rebinding.Core.Scripts.Runtime.Rebinding
{
    public class ActionRebindersCategory : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Required] private TextMeshProUGUI _title;

        private void Reset()
        {
            if (_title == null)
                _title = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetCategoryName(string category)
        {
            _title.text = category;
        }
    }
}