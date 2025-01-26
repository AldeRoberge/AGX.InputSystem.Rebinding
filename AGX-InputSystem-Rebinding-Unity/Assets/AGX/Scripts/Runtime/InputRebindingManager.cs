using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Scripts.Runtime
{
    public class InputRebindingManager : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Required] private InputActionAsset _inputActions;

        [BoxGroup("References"), SerializeField, Required, InlineEditor]
        private InputRebindings _inputRebindings = new();

        [Button, PropertyOrder(-1)]
        private void RebuildList()
        {
            _inputRebindings.Rebuild(_inputActions);
        }
    }
}