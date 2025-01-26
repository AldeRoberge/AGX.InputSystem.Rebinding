using AGX.Scripts.Runtime.Searching;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Scripts.Runtime.Rebinder
{
    public class ActionRebinders : MonoBehaviour, ISearchable
    {
        [BoxGroup("Settings"), SerializeField, Required] private InputActionReference _inputActionReference;

        [BoxGroup("References"), SerializeField, Required] private ActionIconMap _actionIconMap;
        [BoxGroup("References"), SerializeField, Required] private TMP_Text      _actionText;
        [BoxGroup("References"), SerializeField, Required] private RebindOverlay _rebindOverlay;


        public InputActionReference InputActionReference => _inputActionReference;

        public RebindOverlay RebindingOverlay => _rebindOverlay;

        private void OnValidate()
        {
            UpdateText();
        }

        private void OnEnable()
        {
            UpdateText();

            _inputActionReference.action.performed += ActionOnperformed;
        }

        private void ActionOnperformed(InputAction.CallbackContext obj)
        {
            _actionText.DOKill();
            _actionText.color = Color.yellow;
            _actionText.DOColor(Color.white, 2f);

            /*
            _actionText.transform.DOComplete();
            _actionText.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f);
            */
        }

        private void UpdateText()
        {
            _actionText.text = _actionIconMap.GetFor(_inputActionReference);

            name = $"Input Action Rebinders ({_inputActionReference.action.name})";
        }

        public string[] SearchKeywords => GetActionsNames();

        private string[] GetActionsNames()
        {
            // Get children 'ActionRebinder' components on this object.
            var actionRebinds = gameObject.GetComponentsInChildren<ActionRebinder>();

            // Create a string array with the same size as the number of children.
            var actionNames = new string[actionRebinds.Length];

            // Loop through each child and set the action name to the array.
            for (var i = 0; i < actionRebinds.Length; i++)
            {
                actionNames[i] = actionRebinds[i].ActionName;
            }

            return actionNames;
        }
    }
}