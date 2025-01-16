using AGX.Scripts.Runtime.Rebinder;
using FredericRP.GenericSingleton;
using Generator.Scripts.Runtime;
using InputSystemActionPrompts.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace AGX.Scripts.Runtime
{
    [DefaultExecutionOrder(-99999)]
    public class Input : Singleton<Input>, InputActions.IGameplayActions, InputActions.IMenusActions, InputActions.ICheatsActions
    {
        private InputActions? _gameInput;

        public event UnityAction OnStartEvent = delegate { };
        public event UnityAction<Vector2> OnMoveEvent = delegate { };
        public event UnityAction OnJumpEvent = delegate { };
        public event UnityAction OnSneakEvent = delegate { };
        public event UnityAction OnFireEvent = delegate { };
        public Vector2 MousePosition => Mouse.current.position.ReadValue();

        internal void OnEnable()
        {
            Debug.Log("InputReader OnEnableAllInput");

            if (_gameInput == null)
            {
                Debug.Log("[InputReader] OnEnable");
                _gameInput = InputManager.InputActions;
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

            InputManager.LoadBindingOverrides();
            InputManager.RefreshInputDevicePrompt();
        }


        public void RegisterLogging()
        {
            OnJumpEvent += () => Debug.Log("[InputLogging] <b>Jump</b> event was successfully invoked.");
            OnMoveEvent += (m) => Debug.Log($"[InputLogging] <b>Move</b> event was successfully invoked (value: {m}).");
            OnStartEvent += () => Debug.Log("[InputLogging] <b>Start</b> event was successfully invoked.");
            OnSneakEvent += () => Debug.Log("[InputLogging] <b>Sneak</b> event was successfully invoked.");
            OnFireEvent += () => Debug.Log("[InputLogging] <b>Fire</b> event was successfully invoked.");
        }

        internal void OnDisable()
        {
            Debug.Log("[InputReader] OnDisableAllInput");
            _gameInput?.Gameplay.Disable();
            _gameInput?.Menus.Disable();
            _gameInput?.Cheats.Disable();
        }


        public void OnStart(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                OnStartEvent.Invoke();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.phase is InputActionPhase.Performed or InputActionPhase.Canceled)
                OnMoveEvent.Invoke(context.ReadValue<Vector2>());
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                OnJumpEvent.Invoke();
        }

        public void OnSneak(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                OnSneakEvent.Invoke();
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                OnFireEvent.Invoke();
        }
    }
}