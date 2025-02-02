using System;
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
        [BoxGroup("Settings"), SerializeField, Required] private InputActionReference _inputAction;

        [BoxGroup("References"), SerializeField, Required] private ActionIconMap _actionIconMap;
        [BoxGroup("References"), SerializeField, Required] private TMP_Text      _actionText;
        [BoxGroup("References"), SerializeField, Required] private RebindOverlay _rebindOverlay;


        public InputAction InputAction => _inputAction;

        public RebindOverlay RebindingOverlay => _rebindOverlay;

        private void OnActionPerformed(InputAction.CallbackContext obj)
        {
            // Pops the title in yellow when the action is performed
            _actionText.DOKill();
            _actionText.color = Color.yellow;
            _actionText.DOColor(Color.white, 2f);
        }

        private void UpdateText()
        {
            _actionText.text = _actionIconMap.GetFor(_inputAction);

            name = $"Input Action Rebinders ({_inputAction.name})";
        }


        public void SetBindingData(ControlsData controlsData)
        {
            _inputAction = controlsData.InputActionReference;

            if (_inputAction == null)
            {
                Debug.LogError("Null input action");
            }

            Debug.Log(_inputAction.name + ", " + _inputAction.action?.name);

            UpdateText();

            _inputAction.action.performed += OnActionPerformed;
        }

        public string[] SearchKeywords
        {
            get
            {
                var actionRebinds = gameObject.GetComponentsInChildren<ActionRebinder>();
                var actionNames = new string[actionRebinds.Length];
                for (var i = 0; i < actionRebinds.Length; i++) actionNames[i] = actionRebinds[i].ActionName;
                return actionNames;
            }
        }
    }
}