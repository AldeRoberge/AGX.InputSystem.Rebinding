using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Scripts.Runtime.Rebinder
{
    public class ActionRebinders : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Required]    private ActionIconMap _actionIconMap;
        [BoxGroup("References/UI"), SerializeField, Required] private TMP_Text      _actionText;

        [BoxGroup("References"), SerializeField, Required] private InputActionReference _inputActionReference;


        public InputActionReference InputActionReference => _inputActionReference;

        private void OnEnable()
        {
            _actionText.text = _actionIconMap.GetFor(_inputActionReference);
        }
    }
}