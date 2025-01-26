using FredericRP.GenericSingleton;
using Generator.Scripts.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace AGX.Scripts.Runtime
{
    [DefaultExecutionOrder(-99999)]
    public class Input : Singleton<Input>, InputActions.IGameplayActions, InputActions.IMenusActions, InputActions.ICheatsActions
    {
        private InputActions? _inputActions;

        public event UnityAction OnStartEvent = delegate { };
        public event UnityAction<Vector2> OnMoveEvent = delegate { };
        public event UnityAction OnJumpEvent = delegate { };
        public event UnityAction OnSneakEvent = delegate { };
        public event UnityAction OnFireEvent = delegate { };
        public Vector2 MousePosition => Mouse.current.position.ReadValue();

        internal void OnEnable()
        {
            Debug.Log("[Input] Enabled.");

            if (_inputActions == null)
            {
                _inputActions = InputManager.InputActions;
                _inputActions.Gameplay.SetCallbacks(this);
                _inputActions.Menus.SetCallbacks(this);
                _inputActions.Cheats.SetCallbacks(this);
            }

#if UNITY_EDITOR
            _inputActions.Cheats.Enable();
#endif

            _inputActions.Gameplay.Enable();
            _inputActions.Menus.Enable();

            RegisterLogging();

            InputManager.LoadBindingOverrides();
            InputManager.RefreshInputDevicePrompt();
        }


        public void RegisterLogging()
        {
            OnJumpEvent += () => Debug.Log("[Input] <b>Jump</b> event invoked.");
            OnMoveEvent += (m) => Debug.Log($"[Input] <b>Move</b> event invoked (value: {m}).");
            OnStartEvent += () => Debug.Log("[Input] <b>Start</b> event invoked.");
            OnSneakEvent += () => Debug.Log("[Input] <b>Sneak</b> event invoked.");
            OnFireEvent += () => Debug.Log("[Input] <b>Fire</b> event invoked.");
        }

        internal void OnDestroy()
        {
            Debug.Log("[Input] Disabled.");
            _inputActions?.Gameplay.Disable();
            _inputActions?.Menus.Disable();
            _inputActions?.Cheats.Disable();
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

        public void OnNewaction(InputAction.CallbackContext context)
        {
            Debug.Log("New action (not implemented).");
        }
    }
}