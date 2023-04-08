using System.Collections.Generic;

using UnityEngine;

using JumpMaster.LevelControllers;

namespace JumpMaster.Movement
{
    public enum MovementState { STILL, JUMPING, JUMP_CHARGING, DASHING, HANGING, FLOATING, FALLING, BOUNCING };

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

            Bounds = GetComponent<BoxCollider>();

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

            PauseControls();
        }
        private void Resume()
        {
            ControlledRigidbody.constraints = RigidbodyConstraints.None;

            UnpauseControls();
        }
        private void Restart()
        {
            ControlledRigidbody.constraints = RigidbodyConstraints.None;

            TryUnregisterControls();

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

        public Vector2 ScreenPosition { get; private set; }
        public (Vector2 min, Vector2 max) BoundsScreenPosition { get; private set; }
        public BoxCollider Bounds { get; private set; }

        public Rigidbody ControlledRigidbody { get; private set; }

        private void FixedUpdate()
        {
            if (!LevelController.Started)
                return;

            if (LevelController.Paused)
                return;

            if (ActiveControl == null)
                return;

            ControlledRigidbody.velocity = ActiveControl.GetCurrentVelocity();

            ScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
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
            Vector3 startPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, 0f, MovementControllerData.Z_Position));
            startPosition.y += (Bounds.bounds.max.y - Bounds.bounds.min.y) * 0.5f;
            transform.position = startPosition;
        }

        // Calculates the bounds of the player as screen coordinates.
        private (Vector2 min, Vector2 max) GetBoundsScreenPosition()
        {
            Vector2 min = Camera.main.WorldToScreenPoint(Bounds.bounds.min);
            Vector3 max = Camera.main.WorldToScreenPoint(Bounds.bounds.max);
            return (min, max);
        }

        // Register all the different movement controls.
        private void RegisterControls()
        {
            if (_controls == null)
                _controls = new();

            JumpControl jump_control = new(this, MovementControllerData.JumpControlData);
            DashControl dash_control = new(this, MovementControllerData.DashControlData);
            StillControl still_control = new(this, MovementControllerData.StillControlData);
            FallControl fall_control = new(this, MovementControllerData.FallControlData);
            FloatControl float_control = new(this, MovementControllerData.FloatControlData);
            HangControl hang_control = new(this, MovementControllerData.HangControlData);
            ChargedJumpControl charged_jump_control = new(this, MovementControllerData.ChargedJumpControlData, jump_control);
            BounceControl bounce_control = new(this, MovementControllerData.BounceControlData);

            _controls.Add(jump_control);
            _controls.Add(dash_control);
            _controls.Add(still_control);
            _controls.Add(fall_control);
            _controls.Add(float_control);
            _controls.Add(hang_control);
            _controls.Add(charged_jump_control);
            _controls.Add(bounce_control);

            foreach (IMovementControl control in _controls)
            {
                if (control is IExplicitControl explicit_control)
                    explicit_control.OnExplicitDetection += StartControl;
                if (control is ITransitionable transitionable)
                    transitionable.OnTransitionable += StartControl;
                if (control is IInputableControl inputable)
                    inputable.OnInputDetected += TryStartInputControl;
            }
        }

        private void TryUnregisterControls()
        {
            if (_controls == null || _controls.Count == 0)
                return;

            foreach (IMovementControl control in _controls)
            {
                if (control is IExplicitControl explicit_control)
                    explicit_control.OnExplicitDetection -= StartControl;
                if (control is ITransitionable transitionable)
                    transitionable.OnTransitionable -= StartControl;
                if (control is IInputableControl inputable)
                    inputable.OnInputDetected -= TryStartInputControl;
            }

            OnActiveControlChange = null;
            _controls.Clear();
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