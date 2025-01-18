using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AGX.Scripts.Runtime.Rebinder
{
    public class ResetAllBindingsButton : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Required] private Button          _buttonReset;
        [BoxGroup("References"), SerializeField, Required] private TextMeshProUGUI _textReset;

        public void Reset()
        {
            if (_buttonReset == null)
                _buttonReset = GetComponent<Button>();
        }

        public void Awake()
        {
            InputManager.RebindCountChanged += RebindCountChanged;
            _buttonReset.onClick.AddListener(InputManager.ResetAllBindings);
            RebindCountChanged(InputManager.GetTotalBindingOverwriteCount());
        }

        public void OnDestroy()
        {
            InputManager.RebindCountChanged -= RebindCountChanged;
            _buttonReset.onClick.RemoveListener(InputManager.ResetAllBindings);
        }

        private void RebindCountChanged(int count)
        {
            _buttonReset.gameObject.SetActive(count > 0);
            
            // INTERNAL : TODO sprites should be from AGX.Resources
            _textReset.text = count == 1
                ?
                //
                "<sprite=\"General/General\" name=\"Close\" tint=1> Reset 1 custom mapping"
                :
                // 
                $"<sprite=\"General/General\" name=\"Close\" tint=1> Reset {count} custom mappings";
        }
    }
}