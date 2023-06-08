//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Scripts/01 Input/InputActions.inputactions
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

namespace JumpMaster.Controls
{
    public partial class @InputActions : IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @InputActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""UI"",
            ""id"": ""f0bcebc8-1e6a-489f-94c4-2c869777f8cd"",
            ""actions"": [
                {
                    ""name"": ""Touch"",
                    ""type"": ""PassThrough"",
                    ""id"": ""59146b80-66a7-4fd3-9f9b-68794088e272"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Position"",
                    ""type"": ""PassThrough"",
                    ""id"": ""1e8c2c67-11b5-48a5-8784-2e15bac9eaeb"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""5ff7c9c8-34e3-468c-8301-0e1a84697779"",
                    ""path"": ""<Touchscreen>/touch*/Press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touchscreen"",
                    ""action"": ""Touch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ff3119b0-2b43-4f3b-b47d-76e507c4a815"",
                    ""path"": ""<Touchscreen>/primaryTouch/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touchscreen"",
                    ""action"": ""Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Movement"",
            ""id"": ""348cdfb2-301c-4f0e-960f-762efbd70581"",
            ""actions"": [
                {
                    ""name"": ""Contact"",
                    ""type"": ""PassThrough"",
                    ""id"": ""e0799cdd-15df-4c0f-8dbc-95cc6cdb1852"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Position"",
                    ""type"": ""PassThrough"",
                    ""id"": ""dc12042d-aee9-454e-9336-90bfffa80f0f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f032f669-01ab-4408-892d-5d2d26b5e851"",
                    ""path"": ""<Touchscreen>/primaryTouch/press"",
                    ""interactions"": ""Press(behavior=2)"",
                    ""processors"": """",
                    ""groups"": ""Touchscreen"",
                    ""action"": ""Contact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b5df2fa5-5a9b-46f9-8b29-b6a4833ac76f"",
                    ""path"": ""<Touchscreen>/primaryTouch/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touchscreen"",
                    ""action"": ""Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Touchscreen"",
            ""bindingGroup"": ""Touchscreen"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // UI
            m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
            m_UI_Touch = m_UI.FindAction("Touch", throwIfNotFound: true);
            m_UI_Position = m_UI.FindAction("Position", throwIfNotFound: true);
            // Movement
            m_Movement = asset.FindActionMap("Movement", throwIfNotFound: true);
            m_Movement_Contact = m_Movement.FindAction("Contact", throwIfNotFound: true);
            m_Movement_Position = m_Movement.FindAction("Position", throwIfNotFound: true);
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

        // UI
        private readonly InputActionMap m_UI;
        private IUIActions m_UIActionsCallbackInterface;
        private readonly InputAction m_UI_Touch;
        private readonly InputAction m_UI_Position;
        public struct UIActions
        {
            private @InputActions m_Wrapper;
            public UIActions(@InputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Touch => m_Wrapper.m_UI_Touch;
            public InputAction @Position => m_Wrapper.m_UI_Position;
            public InputActionMap Get() { return m_Wrapper.m_UI; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
            public void SetCallbacks(IUIActions instance)
            {
                if (m_Wrapper.m_UIActionsCallbackInterface != null)
                {
                    @Touch.started -= m_Wrapper.m_UIActionsCallbackInterface.OnTouch;
                    @Touch.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnTouch;
                    @Touch.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnTouch;
                    @Position.started -= m_Wrapper.m_UIActionsCallbackInterface.OnPosition;
                    @Position.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnPosition;
                    @Position.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnPosition;
                }
                m_Wrapper.m_UIActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Touch.started += instance.OnTouch;
                    @Touch.performed += instance.OnTouch;
                    @Touch.canceled += instance.OnTouch;
                    @Position.started += instance.OnPosition;
                    @Position.performed += instance.OnPosition;
                    @Position.canceled += instance.OnPosition;
                }
            }
        }
        public UIActions @UI => new UIActions(this);

        // Movement
        private readonly InputActionMap m_Movement;
        private IMovementActions m_MovementActionsCallbackInterface;
        private readonly InputAction m_Movement_Contact;
        private readonly InputAction m_Movement_Position;
        public struct MovementActions
        {
            private @InputActions m_Wrapper;
            public MovementActions(@InputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Contact => m_Wrapper.m_Movement_Contact;
            public InputAction @Position => m_Wrapper.m_Movement_Position;
            public InputActionMap Get() { return m_Wrapper.m_Movement; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(MovementActions set) { return set.Get(); }
            public void SetCallbacks(IMovementActions instance)
            {
                if (m_Wrapper.m_MovementActionsCallbackInterface != null)
                {
                    @Contact.started -= m_Wrapper.m_MovementActionsCallbackInterface.OnContact;
                    @Contact.performed -= m_Wrapper.m_MovementActionsCallbackInterface.OnContact;
                    @Contact.canceled -= m_Wrapper.m_MovementActionsCallbackInterface.OnContact;
                    @Position.started -= m_Wrapper.m_MovementActionsCallbackInterface.OnPosition;
                    @Position.performed -= m_Wrapper.m_MovementActionsCallbackInterface.OnPosition;
                    @Position.canceled -= m_Wrapper.m_MovementActionsCallbackInterface.OnPosition;
                }
                m_Wrapper.m_MovementActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Contact.started += instance.OnContact;
                    @Contact.performed += instance.OnContact;
                    @Contact.canceled += instance.OnContact;
                    @Position.started += instance.OnPosition;
                    @Position.performed += instance.OnPosition;
                    @Position.canceled += instance.OnPosition;
                }
            }
        }
        public MovementActions @Movement => new MovementActions(this);
        private int m_TouchscreenSchemeIndex = -1;
        public InputControlScheme TouchscreenScheme
        {
            get
            {
                if (m_TouchscreenSchemeIndex == -1) m_TouchscreenSchemeIndex = asset.FindControlSchemeIndex("Touchscreen");
                return asset.controlSchemes[m_TouchscreenSchemeIndex];
            }
        }
        public interface IUIActions
        {
            void OnTouch(InputAction.CallbackContext context);
            void OnPosition(InputAction.CallbackContext context);
        }
        public interface IMovementActions
        {
            void OnContact(InputAction.CallbackContext context);
            void OnPosition(InputAction.CallbackContext context);
        }
    }
}