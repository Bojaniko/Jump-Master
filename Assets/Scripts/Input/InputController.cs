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

        private bool _detectedInputUI;

        private bool _detectedInput;
        private Dictionary<InputQueueHandler, InputAction.CallbackContext> _inputCallbacks;

        private void OnEnable()
        {
            _detectedInputUI = false;
            _detectedInput = false;

            _inputCallbacks = new();

            BindAllInputCallbacks();
        }

        private void OnDisable()
        {
            _inputCallbacks.Clear();
            _inputCallbacks = null;
        }

        private void Update()
        {
            if (!_detectedInput)
                return;

            if (_detectedInputUI)
            {
                ClearDetectedInputCallbacks();
                return;
            }

            ProcessDetectedInputCallbacks();
        }

        private void LateUpdate()
        {
            if (_detectedInputUI)
                _detectedInputUI = false;
        }

        private void ProcessDetectedInputCallbacks()
        {
            InputAction.CallbackContext context;
            foreach (InputQueueHandler callback in _inputCallbacks.Keys)
            {
                if (_inputCallbacks.TryGetValue(callback, out context))
                    callback?.Invoke(context);
            }
            ClearDetectedInputCallbacks();
        }

        private void ClearDetectedInputCallbacks()
        {
            _inputCallbacks.Clear();
            _detectedInput = false;
        }

        public void RegisterInputUI()
        {
            _detectedInputUI = true;
        }

        // ##### BINDING ##### \\

        private void BindAllInputCallbacks()
        {
            _input.Movement.Contact.performed += ctx => BindInputCallback(StartTouch, ctx);
            _input.Movement.Contact.canceled += ctx => BindInputCallback(EndTouch, ctx);

            _input.Movement.Touch.performed += ctx => BindInputCallback(Touch, ctx);
            _input.Movement.Touch.started += ctx => BindInputCallback(HoldStart, ctx);
            _input.Movement.Touch.canceled += HoldCancel;
        }

        private void BindInputCallback(InputQueueHandler callback_delegate, InputAction.CallbackContext callback_context)
        {
            if (!_inputCallbacks.TryAdd(callback_delegate, callback_context))
                return;
            _detectedInput = true;
        }

        // ##### INPUT CALLBACKS ##### \\

        private void HoldCancel(InputAction.CallbackContext context)
        {
            if (context.interaction is SlowTapInteraction)
                OnHoldCancelled?.Invoke();
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
            if (SwipeDetector.Instance.IsSwipeDetected((float)context.startTime))
                return;
            if (context.interaction is SlowTapInteraction)
                OnHoldPerformed?.Invoke();
            if (context.interaction is TapInteraction)
                OnTap?.Invoke();
        }

        private void StartTouch(InputAction.CallbackContext context)
        {
            OnTouchStart?.Invoke(_input.Movement.Position.ReadValue<Vector2>(), (float)context.startTime);
        }

        private void EndTouch(InputAction.CallbackContext context)
        {
            OnTouchEnd?.Invoke(_input.Movement.Position.ReadValue<Vector2>(), (float)context.time);
        }

        // ##### EVENTS ##### \\

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