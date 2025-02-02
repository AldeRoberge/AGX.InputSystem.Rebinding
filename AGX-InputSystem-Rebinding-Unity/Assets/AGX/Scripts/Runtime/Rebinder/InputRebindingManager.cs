using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Scripts.Runtime.Rebinder
{
    public class InputRebindingManager : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Required] private InputActionAsset _inputActions;

        [BoxGroup("References"), SerializeField, Required, ShowInInspector, ReadOnly]
        private InputRebindings _inputRebindings = new();

        [Button, PropertyOrder(-1)]
        private void RebuildList()
        {
            _inputRebindings = InputRebindings.Create(_inputActions);
        }
    }
}