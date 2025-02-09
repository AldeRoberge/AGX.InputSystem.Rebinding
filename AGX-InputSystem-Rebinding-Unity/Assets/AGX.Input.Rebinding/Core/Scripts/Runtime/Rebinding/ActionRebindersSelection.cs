using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AGX.Input.Rebinding.Core.Scripts.Runtime.Rebinding
{
    public class ActionRebindersSelection : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Required]
        private ActionRebinderSelection[] _actionRebinderSelections = Array.Empty<ActionRebinderSelection>();

        public ActionRebinderSelection LastSelected { get; set; }

        private void Reset()
        {
            if (_actionRebinderSelections.Length == 0)
                _actionRebinderSelections = GetComponentsInChildren<ActionRebinderSelection>();
        }

        private void OnEnable()
        {
            StartCoroutine(WaitAndSelect());
        }

        private System.Collections.IEnumerator WaitAndSelect()
        {
            yield return null;
            yield return null;
            yield return null;
            EventSystem.current.SetSelectedGameObject(_actionRebinderSelections[0].gameObject);
        }
    }
}