using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace AGX.Scripts.Runtime
{
    public class ActionRebindersCategory : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Required] private TextMeshProUGUI _title;

        public void SetCategoryName(string category)
        {
            _title.text = category;
        }
    }
}