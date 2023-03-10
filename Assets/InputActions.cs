//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/InputActions.inputactions
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
            ""id"": ""e50e73b9-0cd6-4aa3-86ce-292b5de062ee"",
            ""actions"": [
                {
                    ""name"": ""Touch"",
                    ""type"": ""PassThrough"",
                    ""id"": ""c3d39190-b814-46bb-b8b6-3ce5a1d15b0a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Position"",
                    ""type"": ""PassThrough"",
                    ""id"": ""2ff26d15-2b3d-4359-9b56-1696a0f76b27"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""29949562-2e55-4307-926b-23666f72c6d2"",
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
                    ""id"": ""d7c9839f-547c-4012-9133-b047ff44f132"",
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
            ""name"": ""Touch"",
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
                },
                {
                    ""name"": ""Touch"",
                    ""type"": ""Button"",
                    ""id"": ""89458bf6-8acb-4bc7-b3e8-f13cd3b45726"",
                    ""expectedControlType"": ""Button"",
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
                    ""interactions"": ""Press"",
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
                },
                {
                    ""name"": """",
                    ""id"": ""f6e113d5-306c-4efc-9276-b54289c4dd15"",
                    ""path"": ""<Touchscreen>/primaryTouch/press"",
                    ""interactions"": ""Tap,SlowTap(duration=0.35)"",
                    ""processors"": """",
                    ""groups"": ""Touchscreen"",
                    ""action"": ""Touch"",
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
        // Touch
        m_Touch = asset.FindActionMap("Touch", throwIfNotFound: true);
        m_Touch_Contact = m_Touch.FindAction("Contact", throwIfNotFound: true);
        m_Touch_Position = m_Touch.FindAction("Position", throwIfNotFound: true);
        m_Touch_Touch = m_Touch.FindAction("Touch", throwIfNotFound: true);
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

    // Touch
    private readonly InputActionMap m_Touch;
    private ITouchActions m_TouchActionsCallbackInterface;
    private readonly InputAction m_Touch_Contact;
    private readonly InputAction m_Touch_Position;
    private readonly InputAction m_Touch_Touch;
    public struct TouchActions
    {
        private @InputActions m_Wrapper;
        public TouchActions(@InputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Contact => m_Wrapper.m_Touch_Contact;
        public InputAction @Position => m_Wrapper.m_Touch_Position;
        public InputAction @Touch => m_Wrapper.m_Touch_Touch;
        public InputActionMap Get() { return m_Wrapper.m_Touch; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TouchActions set) { return set.Get(); }
        public void SetCallbacks(ITouchActions instance)
        {
            if (m_Wrapper.m_TouchActionsCallbackInterface != null)
            {
                @Contact.started -= m_Wrapper.m_TouchActionsCallbackInterface.OnContact;
                @Contact.performed -= m_Wrapper.m_TouchActionsCallbackInterface.OnContact;
                @Contact.canceled -= m_Wrapper.m_TouchActionsCallbackInterface.OnContact;
                @Position.started -= m_Wrapper.m_TouchActionsCallbackInterface.OnPosition;
                @Position.performed -= m_Wrapper.m_TouchActionsCallbackInterface.OnPosition;
                @Position.canceled -= m_Wrapper.m_TouchActionsCallbackInterface.OnPosition;
                @Touch.started -= m_Wrapper.m_TouchActionsCallbackInterface.OnTouch;
                @Touch.performed -= m_Wrapper.m_TouchActionsCallbackInterface.OnTouch;
                @Touch.canceled -= m_Wrapper.m_TouchActionsCallbackInterface.OnTouch;
            }
            m_Wrapper.m_TouchActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Contact.started += instance.OnContact;
                @Contact.performed += instance.OnContact;
                @Contact.canceled += instance.OnContact;
                @Position.started += instance.OnPosition;
                @Position.performed += instance.OnPosition;
                @Position.canceled += instance.OnPosition;
                @Touch.started += instance.OnTouch;
                @Touch.performed += instance.OnTouch;
                @Touch.canceled += instance.OnTouch;
            }
        }
    }
    public TouchActions @Touch => new TouchActions(this);
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
    public interface ITouchActions
    {
        void OnContact(InputAction.CallbackContext context);
        void OnPosition(InputAction.CallbackContext context);
        void OnTouch(InputAction.CallbackContext context);
    }
}
