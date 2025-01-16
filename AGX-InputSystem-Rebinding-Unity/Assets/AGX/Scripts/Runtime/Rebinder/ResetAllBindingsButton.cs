using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace AGX.Scripts.Runtime.Rebinder
{
    public class ResetAllBindingsButton : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField] private Button _button;

        public void Reset()
        {
            if (_button == null)
                _button = GetComponent<Button>();
        }

        public void OnEnable()
        {
            _button.onClick.AddListener(InputManager.ResetAllBindings);
        }

        public void OnDestroy()
        {
            _button.onClick.RemoveListener(InputManager.ResetAllBindings);
        }
    }
}