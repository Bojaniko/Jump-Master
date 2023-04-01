using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

using JumpMaster.LevelControllers;

namespace JumpMaster.Controls
{
    public class InputController : LevelControllerInitializable
    {

        private static InputController s_instance;

        public static InputController Instance
        {
            get
            {
                return s_instance;
            }
            private set
            {
                if (s_instance == null)
                    s_instance = value;
                else
                    Debug.LogError("You can have only one instance of an Input Controller!");
            }
        }

        protected override void Initialize()
        {
            Instance = this;

            _input = new();
            _input.Enable();
        }

        private InputActions _input;

        private delegate void InputQueueHandler(InputAction.CallbackContext context);

        private bool m_detectedInput;
        private Dictionary<InputQueueHandler, InputAction.CallbackContext> m_inputCallbacks;

        private void Update()
        {
            if (!m_detectedInput)
                return;

            ProcessAllInputCallbacks();

            m_detectedInput = false;
        }

        private void OnEnable()
        {
            m_detectedInput = false;

            m_inputCallbacks = new();

            RegisterAllInputCallbacks();
        }

        private void OnDisable()
        {
            m_inputCallbacks.Clear();
            m_inputCallbacks = null;
        }

        private void ProcessAllInputCallbacks()
        {
            InputAction.CallbackContext context;
            foreach (InputQueueHandler callback in m_inputCallbacks.Keys)
            {
                if (m_inputCallbacks.TryGetValue(callback, out context))
                    callback.Invoke(context);
            }
            m_inputCallbacks.Clear();
        }

        private void RegisterAllInputCallbacks()
        {
            _input.Movement.Contact.performed += ctx => RegisterInputCallback(StartTouch, ctx);
            _input.Movement.Contact.canceled += ctx => RegisterInputCallback(EndTouch, ctx);

            _input.Movement.Touch.performed += ctx => RegisterInputCallback(Touch, ctx);
            _input.Movement.Touch.started += ctx => RegisterInputCallback(HoldStart, ctx);
            _input.Movement.Touch.canceled += ctx => RegisterInputCallback(HoldCancel, ctx);
        }

        private void RegisterInputCallback(InputQueueHandler callback_delegate, InputAction.CallbackContext callback_context)
        {
            if (!m_inputCallbacks.TryAdd(callback_delegate, callback_context))
                return;
            m_detectedInput = true;
        }

        private void HoldCancel(InputAction.CallbackContext context)
        {
            if (context.interaction is SlowTapInteraction)
            {
                if (OnHoldCancelled != null)
                    OnHoldCancelled();
            }
        }

        private void HoldStart(InputAction.CallbackContext context)
        {
            if (context.interaction is SlowTapInteraction)
            {
                if (OnHoldStarted != null)
                {
                    SlowTapInteraction interaction = context.interaction as SlowTapInteraction;
                    OnHoldStarted(_input.Movement.Position.ReadValue<Vector2>(), interaction.duration);
                }
            }
        }

        private void Touch(InputAction.CallbackContext context)
        {
            if (SwipeDetector.Instance.DetectedSwipes.Contains((float)context.startTime))
                return;
            if (context.interaction is SlowTapInteraction)
            {
                if (OnHoldPerformed != null)
                    OnHoldPerformed();
            }
            if (context.interaction is TapInteraction)
            {
                if (OnTap != null)
                    OnTap();
            }
        }

        private void StartTouch(InputAction.CallbackContext context)
        {
            if (OnTouchStart != null)
            {
                OnTouchStart(_input.Movement.Position.ReadValue<Vector2>(), (float)context.startTime);
            }
        }

        private void EndTouch(InputAction.CallbackContext context)
        {
            if (OnTouchEnd != null)
                OnTouchEnd(_input.Movement.Position.ReadValue<Vector2>(), (float)context.time);
        }

        public delegate void SwipeEventHandler(Vector2 position, float time);
        public event SwipeEventHandler OnTouchStart;
        public event SwipeEventHandler OnTouchEnd;

        public delegate void HoldEventHandler();
        public event HoldEventHandler OnHoldPerformed;

        public delegate void HoldStartEventhandler(Vector2 position, float min_hold_duration);
        public event HoldStartEventhandler OnHoldStarted;

        public delegate void HoldCancelEventHandler();
        public event HoldCancelEventHandler OnHoldCancelled;

        public delegate void TapEventHandler();
        public event TapEventHandler OnTap;
    }
}