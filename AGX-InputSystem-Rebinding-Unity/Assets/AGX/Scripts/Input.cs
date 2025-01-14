using Generator.Scripts.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace AGX.Scripts
{
    public partial class Input : InputActions.IGameplayActions, InputActions.IMenusActions, InputActions.ICheatsActions
    {
        // Gameplay
        public event UnityAction OnStartEvent = delegate { };
        public event UnityAction OnMoveEvent = delegate { };
        public event UnityAction OnJumpEvent = delegate { };


        // Utility for 'MousePosition'
        public Vector2 MousePosition => Mouse.current.position.ReadValue();

        // Menus

        // Cheats

        public void OnStart(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                OnStartEvent.Invoke();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                OnMoveEvent.Invoke();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            OnJumpEvent.Invoke();
        }
    }
}