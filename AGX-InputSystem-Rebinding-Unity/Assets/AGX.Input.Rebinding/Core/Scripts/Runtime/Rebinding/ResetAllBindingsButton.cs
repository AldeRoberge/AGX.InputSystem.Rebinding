using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AGX.Input.Rebinding.Core.Scripts.Runtime.Rebinding
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
            _buttonReset.onClick.AddListener(InputManager.Instance.ResetAllBindings);
            RebindCountChanged(InputManager.Instance.GetTotalBindingOverwriteCount());
        }

        public void OnDestroy()
        {
            InputManager.RebindCountChanged -= RebindCountChanged;
            _buttonReset.onClick.RemoveListener(InputManager.Instance.ResetAllBindings);
        }

        private void RebindCountChanged(int count)
        {
            _buttonReset.gameObject.SetActive(count > 0);

            // INTERNAL : TODO sprites should be from AGX.Resources
            _textReset.text = count == 1 ?
                // One
                "<sprite=\"General/General\" name=\"Cancel\" tint=1> Reset 1 custom mapping" :
                // Many
                $"<sprite=\"General/General\" name=\"Cancel\" tint=1> Reset {count} custom mappings";
        }
    }
}