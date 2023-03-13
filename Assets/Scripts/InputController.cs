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

        private void Start()
        {
            _input.Touch.Contact.performed += ctx => StartTouchPrimary(ctx);
            _input.Touch.Contact.canceled += ctx => EndTouchPrimary(ctx);

            _input.Touch.Touch.performed += ctx => Touch(ctx);
            _input.Touch.Touch.started += ctx => HoldStart(ctx);
            _input.Touch.Touch.canceled += ctx => HoldCancel(ctx);
        }

        private void HoldCancel(InputAction.CallbackContext context)
        {
            if (context.interaction is SlowTapInteraction)
            {
                if (OnHoldCancelled != null) OnHoldCancelled();
            }
        }

        private void HoldStart(InputAction.CallbackContext context)
        {
            if (context.interaction is SlowTapInteraction)
            {
                if (OnHoldStarted != null)
                {
                    SlowTapInteraction interaction = context.interaction as SlowTapInteraction;
                    OnHoldStarted(_input.Touch.Position.ReadValue<Vector2>(), interaction.duration);
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
                bool was_paused = LevelController.Paused;

                if (was_paused == false && OnTap != null)
                    OnTap();
            }
        }

        private void StartTouchPrimary(InputAction.CallbackContext context)
        {
            if (OnTouchStart != null)
            {
                OnTouchStart(_input.Touch.Position.ReadValue<Vector2>(), (float)context.startTime);
            }
        }

        private void EndTouchPrimary(InputAction.CallbackContext context)
        {
            if (OnTouchEnd != null)
            {
                OnTouchEnd(_input.Touch.Position.ReadValue<Vector2>(), (float)context.time);
            }
        }
    }
}