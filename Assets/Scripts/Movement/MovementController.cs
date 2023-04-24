using System.Collections.Generic;

using UnityEngine;

using JumpMaster.LevelControllers;

namespace JumpMaster.Movement
{
    public enum MovementState { STILL, JUMPING, JUMP_CHARGING, DASHING, HANGING, FLOATING, FALLING, BOUNCING };

    public class MovementController : LevelControllerInitializable
    {
        public MovementControllerDataSO MovementControllerData;

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

            if (MovementControllerData == null)
            {
                enabled = false;
                return;
            }

            Cache();

            LevelController.OnPause += Pause;
            LevelController.OnEndLevel += End;

            LevelController.OnLoad += Resume;
            LevelController.OnResume += Resume;

            LevelController.OnRestart += Restart;

            Restart();
        }

        private void End()
        {
            Constrain(true);
        }
        private void Pause()
        {
            PauseControls();
            Constrain(true);
        }
        private void Resume()
        {
            Constrain(false);

            UnpauseControls();
        }
        private void Restart()
        {
            Constrain(false);

            TryUnregisterControls();

            RegisterControls();
            ResetPlayerPosition();
            BoundsScreenPosition = GetBoundsScreenPosition();

            IMovementControl still_control = GetControl<StillControl>();
            StartControl(still_control, new(this));
        }

        private void FixedUpdate()
        {
            if (!LevelController.Started)
                return;

            if (LevelController.Ended)
                return;

            if (LevelController.Paused)
                return;

            if (ActiveControl == null)
                return;

            ControlledRigidbody.velocity = ActiveControl.GetCurrentVelocity();

            UpdateScreenPosition();

            OnMovementUpdate?.Invoke();
        }

        // ##### EVENTS ##### \\

        public delegate void activeControlEventHandler(IMovementControl current_control);
        public event activeControlEventHandler OnActiveControlChange;

        public delegate void MovementUpdateEventController();
        public event MovementUpdateEventController OnMovementUpdate;

        // ##### CONTROL STARTER ##### \\

        public IMovementControl PreviousControl { get; private set; }
        public IMovementControl ActiveControl { get; private set; }

        private void TryStartControl(IMovementControl control, MovementControlArgs start_args)
        {
            if (LevelController.Paused)
                return;

            if (LevelController.Ended)
                return;

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
            //Debug.Log($"Changed control is {control}.");
            ActiveControl = control;
            ActiveControl.Start(start_args);

            OnActiveControlChange?.Invoke(control);
        }

        // ##### CONTROL GETTERS ##### \\

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

        public IMovementControl GetControlByState(MovementState state)
        {
            foreach (IMovementControl control in Controls)
            {
                if (control.ActiveState.Equals(state))
                    return control;
            }
            throw new InvalidControlOfStateException(state);
        }

        // ##### CONTROL REGISTRATION ##### \\

        private List<IMovementControl> _controls;
        public IMovementControl[] Controls { get { return _controls.ToArray(); } }

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
                    inputable.OnInputDetected += TryStartControl;
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
                    inputable.OnInputDetected -= TryStartControl;
            }

            OnActiveControlChange = null;
            _controls.Clear();
        }

        // ##### POSITION ##### \\

        public Vector2 ScreenPosition { get; private set; }
        public (Vector2 min, Vector2 max) BoundsScreenPosition { get; private set; }

        private void ResetPlayerPosition()
        {
            Vector3 startPosition = c_camera.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, 0f, MovementControllerData.Z_Position));
            startPosition.y += (Bounds.bounds.max.y - Bounds.bounds.min.y) * 0.5f;
            transform.position = startPosition;
        }

        private void UpdateScreenPosition()
        {
            ScreenPosition = c_camera.WorldToScreenPoint(transform.position);
            BoundsScreenPosition = GetBoundsScreenPosition();
        }

        private (Vector2 min, Vector2 max) GetBoundsScreenPosition()
        {
            Vector2 min = Camera.main.WorldToScreenPoint(Bounds.bounds.min);
            Vector3 max = Camera.main.WorldToScreenPoint(Bounds.bounds.max);
            return (min, max);
        }

        // ##### CONSTRAINT ##### \\

        private void Constrain(bool constrain)
        {
            if (constrain)
                c_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            else
                c_rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }

        // ##### PAUSE ##### \\

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

        // ##### CACHE ##### \\

        public BoxCollider Bounds => c_bounds;
        public Rigidbody ControlledRigidbody => c_rigidbody;

        private Camera c_camera;
        private BoxCollider c_bounds;
        private Rigidbody c_rigidbody;

        private void Cache()
        {
            c_camera = Camera.main;
            c_bounds = GetComponent<BoxCollider>();
            c_rigidbody = GetComponent<Rigidbody>();
        }
    }
}