using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Scripts.Runtime
{
    public class InputRebindingManager : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Required] private InputActionAsset _inputActions;

        [BoxGroup("References"), SerializeField, Required] 
        private InputRebindings _inputRebindings = new();

        [Button]
        private void RebuildList()
        {
            _inputRebindings.Rebuild(_inputActions);
        }
    }
}