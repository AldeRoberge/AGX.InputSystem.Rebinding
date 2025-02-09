using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AGX.Input.Rebinding.Core.Scripts.Runtime.Rebinding
{
    /// <summary>
    /// Big thanks to Sasquash B for inspiration
    /// https://www.youtube.com/watch?v=u3YdlUW1nx0
    /// </summary>
    public class ActionRebinderSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [BoxGroup("References"), SerializeField, Required]
        private ActionRebindersSelection _parent;

        [BoxGroup("References"), SerializeField, Required]
        private Button _button;

        private void Reset()
        {
            if (_parent == null)
                _parent = FindObjectOfType<ActionRebindersSelection>();

            if (_button == null)
                _button = GetComponentInChildren<Button>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("OnPointerEnter");
            eventData.selectedObject = _button.gameObject;

            _parent.LastSelected = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("OnPointerExit");
            //eventData.selectedObject = null;
        }

        public void OnSelect(BaseEventData eventData)
        {
        }

        public void OnDeselect(BaseEventData eventData)
        {
        }
    }
}