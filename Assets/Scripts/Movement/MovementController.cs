using System.Collections.Generic;

using UnityEngine;

using JumpMaster.LevelControllers;
using JumpMaster.Controls;

namespace JumpMaster.Movement
{
    public enum MovementState { STILL, JUMPING, JUMP_CHARGING, DASHING, HANGING, FLOATING, FALLING };

    public class MovementController : LevelControllerInitializable
    {
        public MovementControllerDataSO MovementControllerData;

        // ##### EVENTS #####

        public delegate void activeControlEventHandler(IMovementControl current_control);
        public event activeControlEventHandler OnActiveControlChange;

        public delegate void MovementUpdateEventController();
        public event MovementUpdateEventController OnMovementUpdate;

        // ##### INITIALIZATION #####

        private static MovementController s_instance;
        public static MovementController Instance
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
                    Debug.LogError("There can only be one Movement Controller in the scene!");
            }
        }

        protected override void Initialize()
        {
            Instance = this;

            ControlledRigidbody = GetComponent<Rigidbody>();

            _bounds = GetComponent<BoxCollider>();

            _inputEnabled = false;

            LevelController.OnPause += Pause;
            LevelController.OnEndLevel += Pause;

            LevelController.OnLoad += Resume;
            LevelController.OnResume += Resume;

            LevelController.OnRestart += Restart;

            Restart();
        }

        // ##### GAME STATE #####

        private void Pause()
        {
            ControlledRigidbody.constraints = RigidbodyConstraints.FreezeAll;

            DisableInput();
            PauseControls();
        }
        private void Resume()
        {
            ControlledRigidbody.constraints = RigidbodyConstraints.None;

            EnableInput();
            UnpauseControls();
        }
        private void Restart()
        {
            RegisterControls();
            ResetPlayerPosition();
            BoundsScreenPosition = GetBoundsScreenPosition();

            IMovementControl still_control = GetControl<StillControl>();
            StartControl(still_control, new(ControlledRigidbody));
        }

        public IMovementControl PreviousControl { get; private set; }
        public IMovementControl ActiveControl { get; private set; }

        private List<IMovementControl> _controls;
        public IMovementControl[] Controls { get { return _controls.ToArray(); } }

        public (Vector2 min, Vector2 max) BoundsScreenPosition { get; private set; }
        private BoxCollider _bounds;

        public Rigidbody ControlledRigidbody { get; private set; }

        private bool _inputEnabled = false;

        private void FixedUpdate()
        {
            if (LevelController.Paused)
                return;

            if (ActiveControl == null)
                return;

            ControlledRigidbody.velocity = ActiveControl.GetCurrentVelocity();

            BoundsScreenPosition = GetBoundsScreenPosition();

            if (OnMovementUpdate != null)
                OnMovementUpdate();
        }

        private void TryStartInputControl(IMovementControl control, MovementControlArgs start_args)
        {
            if (LevelController.Paused)
                return;

            TryStartControl(control, start_args);
        }

        private void TryStartControl(IMovementControl control, MovementControlArgs start_args)
        {
            if (!ActiveControl.CanExit())
                return;

            if (!control.CanStart())
                return;

            StartControl(control, start_args);
        }
        private void StartControl(IMovementControl control, MovementControlArgs start_args)
        {
            if (ActiveControl != null)
                ActiveControl.Exit();
            if (PreviousControl == null)
                PreviousControl = control;
            else
                PreviousControl = ActiveControl;
            ActiveControl = control;
            ActiveControl.Start(start_args);

            if (OnActiveControlChange != null)
                OnActiveControlChange(control);
        }

        // Returns a control by type from the controls list.
        public Control GetControl<Control>()
            where Control : IMovementControl
        {
            foreach (IMovementControl control in Controls)
            {
                if (control.GetType().Equals(typeof(Control)))
                    return (Control)control;
            }
            throw new InvalidControlException(typeof(Control).ToString());
        }

        // Returns a control by state from the controls list.
        public IMovementControl GetControlByState(MovementState state)
        {
            foreach (IMovementControl control in Controls)
            {
                if (control.ActiveState.Equals(state))
                    return control;
            }
            throw new InvalidControlOfStateException(state);
        }

        // Sets the player position to sit on the bottom of the screen.
        private void ResetPlayerPosition()
        {
            Vector3 startPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, 0f, PlayerController.Instance.Z_Position));
            startPosition.y += (_bounds.bounds.max.y - _bounds.bounds.min.y) * 0.5f;
            transform.position = startPosition;
        }

        // Calculates the bounds of the player as screen coordinates.
        private (Vector2 min, Vector2 max) GetBoundsScreenPosition()
        {
            Vector2 min = Camera.main.WorldToScreenPoint(_bounds.bounds.min);
            Vector3 max = Camera.main.WorldToScreenPoint(_bounds.bounds.max);
            return (min, max);
        }

        // Register all the different movement controls.
        private void RegisterControls()
        {
            _controls = new();

            JumpControl jump_control = new(this, MovementControllerData.JumpControlData);
            DashControl dash_control = new(this, MovementControllerData.DashControlData);
            StillControl still_control = new(this, MovementControllerData.StillControlData);
            FallControl fall_control = new(this, MovementControllerData.FallControlData);
            FloatControl float_control = new(this, MovementControllerData.FloatControlData);
            HangControl hang_control = new(this, MovementControllerData.HangControlData);
            ChargedJumpControl charged_jump_control = new(this, MovementControllerData.ChargedJumpControlData, jump_control);

            _controls.Add(jump_control);
            _controls.Add(dash_control);
            _controls.Add(still_control);
            _controls.Add(fall_control);
            _controls.Add(float_control);
            _controls.Add(hang_control);
            _controls.Add(charged_jump_control);

            foreach (IMovementControl control in _controls)
            {
                if (control is IExplicitControl explicit_control)
                    explicit_control.OnExplicitDetection += StartControl;
                if (control is ITransitionable transitionable)
                    transitionable.OnTransitionable += StartControl;
            }
        }

        // Enable and disable input detection from inputable controls.

        private void EnableInput()
        {
            if (_inputEnabled)
                return;

            foreach(IMovementControl control in Controls)
            {
                if (control is IInputableControl inputable)
                    inputable.OnInputDetected += TryStartInputControl;
            }

            _inputEnabled = true;
        }
        private void DisableInput()
        {
            if (!_inputEnabled)
                return;

            foreach (IMovementControl control in Controls)
            {
                if (control is IInputableControl inputable)
                    inputable.OnInputDetected -= TryStartInputControl;
            }

            _inputEnabled = false;
        }

        // Pause and upause controls.

        private void PauseControls()
        {
            foreach (IMovementControl control in Controls)
            {
                control.Pause();
            }
        }

        private void UnpauseControls()
        {
            foreach (IMovementControl control in Controls)
            {
                control.Resume();
            }
        }
    }
}