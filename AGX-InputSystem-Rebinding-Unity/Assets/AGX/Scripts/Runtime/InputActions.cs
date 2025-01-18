//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.2
//     from Assets/AGX/Scripts/Runtime/InputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Generator.Scripts.Runtime
{
    public partial class @InputActions: IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @InputActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""d25675f7-9a60-4db0-b9d5-039bd18912c9"",
            ""actions"": [
                {
                    ""name"": ""Start"",
                    ""type"": ""Button"",
                    ""id"": ""1cd71775-0470-4aea-8ecd-b66126317c39"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""d99c0fcd-4841-49b5-9eca-7615cddf07e9"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""3460ac16-4532-4d3d-b52c-bafb5890bcb6"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Sneak"",
                    ""type"": ""Button"",
                    ""id"": ""92efd1f5-3097-4c15-9c30-1bc9c6a17649"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""Button"",
                    ""id"": ""8fa0f0c0-88a1-4abd-bad0-73580979dd6d"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""a328aee8-7adf-4807-abe3-745bbd2277fa"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard"",
                    ""action"": ""Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0d4e2bba-5210-4d68-8ebe-545353487a1f"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard"",
                    ""action"": ""Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""01e64c5f-6405-458f-b4f3-b0a977fbddde"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1542eda1-0673-44af-ba38-bc5444b4d044"",
                    ""path"": ""<XInputController>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Start"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD [Keyboard]"",
                    ""id"": ""72bf64a3-0cc9-45bd-ad6c-2f9a4e11ac3d"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""001d1cdb-6e02-429d-9e6e-5424010439c1"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""671b6fb0-9200-4634-abd7-de78b063d17d"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""4c792ab8-c8d0-48a9-89c5-5387861d1e87"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""9e7fbaca-7efa-4673-a889-f6057ac67108"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrows [Keyboard]"",
                    ""id"": ""3b7da0cc-0181-4794-8a67-06f42ae59ac9"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f68803d8-5a00-4832-a8dd-b4f5d44aee72"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""64415251-9308-4d89-8eed-f93f3244a187"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""757fcbcf-2962-460e-8af5-607c7ade0377"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""e8aca109-604e-49d3-b7bc-2e8e6bda6467"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""622e9e6f-6969-4bce-9a02-24f37dca8f36"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d813fed6-6b8a-44be-8a64-e30cde5b27a3"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9014b9fe-7159-486a-8243-3ed0733dd9f8"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": "";Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""00dc0527-4d3f-4952-a173-8463678a01c5"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Sneak"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""abc596e7-e716-4f1a-9d47-142fb27e2a85"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Menus"",
            ""id"": ""aa094381-84d1-4976-ad72-91bf1e34864f"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""b2396ce0-9e30-4219-a6c6-0e888e379938"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""a3d9bdbe-c05e-498b-a112-9df28826102e"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f5215842-123a-4af5-bdb0-d45d97134820"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Cheats"",
            ""id"": ""135a8b2b-eac9-4fcf-8bd2-3e6256fa2cdb"",
            ""actions"": [],
            ""bindings"": []
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Gameplay
            m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
            m_Gameplay_Start = m_Gameplay.FindAction("Start", throwIfNotFound: true);
            m_Gameplay_Move = m_Gameplay.FindAction("Move", throwIfNotFound: true);
            m_Gameplay_Jump = m_Gameplay.FindAction("Jump", throwIfNotFound: true);
            m_Gameplay_Sneak = m_Gameplay.FindAction("Sneak", throwIfNotFound: true);
            m_Gameplay_Fire = m_Gameplay.FindAction("Fire", throwIfNotFound: true);
            // Menus
            m_Menus = asset.FindActionMap("Menus", throwIfNotFound: true);
            m_Menus_Newaction = m_Menus.FindAction("New action", throwIfNotFound: true);
            // Cheats
            m_Cheats = asset.FindActionMap("Cheats", throwIfNotFound: true);
        }

        ~@InputActions()
        {
            UnityEngine.Debug.Assert(!m_Gameplay.enabled, "This will cause a leak and performance issues, InputActions.Gameplay.Disable() has not been called.");
            UnityEngine.Debug.Assert(!m_Menus.enabled, "This will cause a leak and performance issues, InputActions.Menus.Disable() has not been called.");
            UnityEngine.Debug.Assert(!m_Cheats.enabled, "This will cause a leak and performance issues, InputActions.Cheats.Disable() has not been called.");
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }

        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // Gameplay
        private readonly InputActionMap m_Gameplay;
        private List<IGameplayActions> m_GameplayActionsCallbackInterfaces = new List<IGameplayActions>();
        private readonly InputAction m_Gameplay_Start;
        private readonly InputAction m_Gameplay_Move;
        private readonly InputAction m_Gameplay_Jump;
        private readonly InputAction m_Gameplay_Sneak;
        private readonly InputAction m_Gameplay_Fire;
        public struct GameplayActions
        {
            private @InputActions m_Wrapper;
            public GameplayActions(@InputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Start => m_Wrapper.m_Gameplay_Start;
            public InputAction @Move => m_Wrapper.m_Gameplay_Move;
            public InputAction @Jump => m_Wrapper.m_Gameplay_Jump;
            public InputAction @Sneak => m_Wrapper.m_Gameplay_Sneak;
            public InputAction @Fire => m_Wrapper.m_Gameplay_Fire;
            public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
            public void AddCallbacks(IGameplayActions instance)
            {
                if (instance == null || m_Wrapper.m_GameplayActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_GameplayActionsCallbackInterfaces.Add(instance);
                @Start.started += instance.OnStart;
                @Start.performed += instance.OnStart;
                @Start.canceled += instance.OnStart;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Sneak.started += instance.OnSneak;
                @Sneak.performed += instance.OnSneak;
                @Sneak.canceled += instance.OnSneak;
                @Fire.started += instance.OnFire;
                @Fire.performed += instance.OnFire;
                @Fire.canceled += instance.OnFire;
            }

            private void UnregisterCallbacks(IGameplayActions instance)
            {
                @Start.started -= instance.OnStart;
                @Start.performed -= instance.OnStart;
                @Start.canceled -= instance.OnStart;
                @Move.started -= instance.OnMove;
                @Move.performed -= instance.OnMove;
                @Move.canceled -= instance.OnMove;
                @Jump.started -= instance.OnJump;
                @Jump.performed -= instance.OnJump;
                @Jump.canceled -= instance.OnJump;
                @Sneak.started -= instance.OnSneak;
                @Sneak.performed -= instance.OnSneak;
                @Sneak.canceled -= instance.OnSneak;
                @Fire.started -= instance.OnFire;
                @Fire.performed -= instance.OnFire;
                @Fire.canceled -= instance.OnFire;
            }

            public void RemoveCallbacks(IGameplayActions instance)
            {
                if (m_Wrapper.m_GameplayActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IGameplayActions instance)
            {
                foreach (var item in m_Wrapper.m_GameplayActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_GameplayActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public GameplayActions @Gameplay => new GameplayActions(this);

        // Menus
        private readonly InputActionMap m_Menus;
        private List<IMenusActions> m_MenusActionsCallbackInterfaces = new List<IMenusActions>();
        private readonly InputAction m_Menus_Newaction;
        public struct MenusActions
        {
            private @InputActions m_Wrapper;
            public MenusActions(@InputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Newaction => m_Wrapper.m_Menus_Newaction;
            public InputActionMap Get() { return m_Wrapper.m_Menus; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(MenusActions set) { return set.Get(); }
            public void AddCallbacks(IMenusActions instance)
            {
                if (instance == null || m_Wrapper.m_MenusActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_MenusActionsCallbackInterfaces.Add(instance);
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
            }

            private void UnregisterCallbacks(IMenusActions instance)
            {
                @Newaction.started -= instance.OnNewaction;
                @Newaction.performed -= instance.OnNewaction;
                @Newaction.canceled -= instance.OnNewaction;
            }

            public void RemoveCallbacks(IMenusActions instance)
            {
                if (m_Wrapper.m_MenusActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(IMenusActions instance)
            {
                foreach (var item in m_Wrapper.m_MenusActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_MenusActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public MenusActions @Menus => new MenusActions(this);

        // Cheats
        private readonly InputActionMap m_Cheats;
        private List<ICheatsActions> m_CheatsActionsCallbackInterfaces = new List<ICheatsActions>();
        public struct CheatsActions
        {
            private @InputActions m_Wrapper;
            public CheatsActions(@InputActions wrapper) { m_Wrapper = wrapper; }
            public InputActionMap Get() { return m_Wrapper.m_Cheats; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(CheatsActions set) { return set.Get(); }
            public void AddCallbacks(ICheatsActions instance)
            {
                if (instance == null || m_Wrapper.m_CheatsActionsCallbackInterfaces.Contains(instance)) return;
                m_Wrapper.m_CheatsActionsCallbackInterfaces.Add(instance);
            }

            private void UnregisterCallbacks(ICheatsActions instance)
            {
            }

            public void RemoveCallbacks(ICheatsActions instance)
            {
                if (m_Wrapper.m_CheatsActionsCallbackInterfaces.Remove(instance))
                    UnregisterCallbacks(instance);
            }

            public void SetCallbacks(ICheatsActions instance)
            {
                foreach (var item in m_Wrapper.m_CheatsActionsCallbackInterfaces)
                    UnregisterCallbacks(item);
                m_Wrapper.m_CheatsActionsCallbackInterfaces.Clear();
                AddCallbacks(instance);
            }
        }
        public CheatsActions @Cheats => new CheatsActions(this);
        private int m_GamepadSchemeIndex = -1;
        public InputControlScheme GamepadScheme
        {
            get
            {
                if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
                return asset.controlSchemes[m_GamepadSchemeIndex];
            }
        }
        private int m_KeyboardSchemeIndex = -1;
        public InputControlScheme KeyboardScheme
        {
            get
            {
                if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
                return asset.controlSchemes[m_KeyboardSchemeIndex];
            }
        }
        public interface IGameplayActions
        {
            void OnStart(InputAction.CallbackContext context);
            void OnMove(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
            void OnSneak(InputAction.CallbackContext context);
            void OnFire(InputAction.CallbackContext context);
        }
        public interface IMenusActions
        {
            void OnNewaction(InputAction.CallbackContext context);
        }
        public interface ICheatsActions
        {
        }
    }
}
