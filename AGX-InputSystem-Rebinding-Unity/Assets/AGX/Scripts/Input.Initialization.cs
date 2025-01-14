using AGX.Scripts.Rebinder;
using FredericRP.GenericSingleton;
using Generator.Scripts.Runtime;
using UnityEngine;

namespace AGX.Scripts
{
    public partial class Input : Singleton<Input>, InputActions.IGameplayActions, InputActions.IMenusActions, InputActions.ICheatsActions
    {
        private InputActions? _gameInput;
        
        private InputManager _inputManager;

        internal void OnEnable()
        {
            Debug.Log("InputReader OnEnableAllInput");

            _inputManager ??= new InputManager();

            if (_gameInput == null)
            {
                Debug.Log("[InputReader] OnEnable");
                _gameInput = _inputManager.GetInputActions();
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


        internal void OnDisable()
        {
            Debug.Log("[InputReader] OnDisableAllInput");
            _gameInput?.Gameplay.Disable();
            _gameInput?.Menus.Disable();
            _gameInput?.Cheats.Disable();
        }
    }
}