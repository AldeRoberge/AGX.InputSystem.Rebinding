using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AGX.Scripts.Runtime.Rebinder
{
    public class ResetAllBindingsButton : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Required] private Button          _button;
        [BoxGroup("References"), SerializeField, Required] private TextMeshProUGUI _resetText;

        public void Reset()
        {
            if (_button == null) _button = GetComponent<Button>();
        }

        public void Awake()
        {
            InputManager.RebindCountChanged += RebindCountChanged;
            _button.onClick.AddListener(InputManager.ResetAllBindings);
            RebindCountChanged(InputManager.GetRebindCount());
        }

        public void OnDestroy()
        {
            InputManager.RebindCountChanged -= RebindCountChanged;
            _button.onClick.RemoveListener(InputManager.ResetAllBindings);
        }

        private void RebindCountChanged(int count)
        {
            _button.gameObject.SetActive(count > 0);
            _resetText.text = count == 1
                ?
                //
                "<sprite=\"General/General\" name=\"Reset\" tint=1> Reset 1 custom mapping"
                :
                // 
                $"<sprite=\"General/General\" name=\"Reset\" tint=1> Reset {count} custom mappings";
        }
    }
}