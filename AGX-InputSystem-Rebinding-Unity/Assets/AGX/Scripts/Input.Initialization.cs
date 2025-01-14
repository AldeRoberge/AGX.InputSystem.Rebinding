using FredericRP.GenericSingleton;
using Generator.Scripts.Runtime;
using UnityEngine;

namespace AGX.Scripts
{
    public partial class Input : Singleton<Input>
    {
        private InputActions? _gameInput;

        internal void OnDisable()
        {
            Debug.Log("[InputReader] OnDisableAllInput");
            _gameInput?.Gameplay.Disable();
            _gameInput?.Menus.Disable();
            _gameInput?.Cheats.Disable();
        }

        internal void OnEnable()
        {
            Debug.Log("InputReader OnEnableAllInput");

            if (_gameInput == null)
            {
                Debug.Log("[InputReader] OnEnable");
                _gameInput = new InputActions();
                _gameInput.Gameplay.SetCallbacks(this);
                _gameInput.Menus.SetCallbacks(this);
                _gameInput.Cheats.SetCallbacks(this);
            }

#if UNITY_EDITOR
            _gameInput.Cheats.Enable();
#endif

            _gameInput.Gameplay.Enable();
            _gameInput.Menus.Enable();

            RegisterLogging();
        }
    }
}