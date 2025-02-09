using AGX.Input.Rebinding.Core.Scripts.Runtime.Rebinding;
using AGX.Runtime;
using FredericRP.GenericSingleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace AGX.Scripts.Runtime.Input
{
    [DefaultExecutionOrder(-99999)]
    public class InputReader : Singleton<InputReader>, InputActions.IGameplayActions, InputActions.IMenuActions, InputActions.ICheatsActions
    {
        [BoxGroup("References"), SerializeField, Required]
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
                Debug.LogError("[Input] InputActions not set.");
                return;
            }

            _inputActions.Gameplay.SetCallbacks(this);
            _inputActions.Menu.SetCallbacks(this);
            _inputActions.Cheats.SetCallbacks(this);

#if UNITY_EDITOR
            _inputActions.Cheats.Enable();
#endif

            _inputActions.Gameplay.Enable();
            _inputActions.Menu.Enable();

            RegisterLogging();

            InputManager.Instance.LoadBindingOverrides();
            InputManager.Instance.RefreshInputDevicePrompt();
        }

        private void RegisterLogging()
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
            _inputActions?.Menu.Disable();
            _inputActions?.Cheats.Disable();
        }

        public void OnGameplayMove(InputAction.CallbackContext context)
        {
        }

        public void OnGameplayAim(InputAction.CallbackContext context)
        {
        }

        public void OnGameplayDash(InputAction.CallbackContext context)
        {
        }

        public void OnGameplaySneak(InputAction.CallbackContext context)
        {
        }

        public void OnGameplayAttackToggle(InputAction.CallbackContext context)
        {
        }

        public void OnGameplayQuickHeal(InputAction.CallbackContext context)
        {
        }

        public void OnGameplayExit(InputAction.CallbackContext context)
        {
        }

        public void OnGameplayInteract(InputAction.CallbackContext context)
        {
        }

        public void OnGameplayCancel(InputAction.CallbackContext context)
        {
        }


        public void OnDebugToggle(InputAction.CallbackContext context)
        {
        }

        public void OnMenuSettings(InputAction.CallbackContext context)
        {
        }

        public void OnMenuProfile(InputAction.CallbackContext context)
        {
        }

        public void OnMenuMap(InputAction.CallbackContext context)
        {
        }
    }
}