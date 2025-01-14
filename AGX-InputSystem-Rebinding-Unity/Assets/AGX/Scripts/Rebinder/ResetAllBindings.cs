using UnityEngine;
using UnityEngine.InputSystem;

namespace AGX.Scripts.Rebinder
{
    public class ResetAllBindings : MonoBehaviour
    {
        [SerializeField]
        private InputActionAsset inputActions;

        public void ResetBindings()
        {
            foreach ( InputActionMap map in inputActions.actionMaps)
            {
                map.RemoveAllBindingOverrides();
            }
            PlayerPrefs.DeleteKey("rebinds");
        }
    }
}