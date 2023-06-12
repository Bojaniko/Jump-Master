using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using JumpMaster.Core;
using JumpMaster.Core.Physics;

namespace JumpMaster.Movement
{
    public enum MovementState { STILL, JUMPING, DASHING, HANGING, FLOATING, FALLING, BOUNCING, LEVITATING };

    [RequireComponent(typeof(Rigidbody2D))]
    public class MovementController : LevelController
    {
        [SerializeField] private MovementControllerDataSO _data;

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

            if (_data == null)
            {
                enabled = false;
                return;
            }

            Cache();

            LevelManager.OnPause += Pause;
            LevelManager.OnEndLevel += End;

            LevelManager.OnLoad += Resume;
            LevelManager.OnResume += Resume;

            LevelManager.OnRestart += Restart;

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

            RegisterControls();
            ResetPlayerPosition();

            IMovementControl still_control = GetControl<StillControl>();
            StartControl(still_control, new(this));
        }

        private void FixedUpdate()
        {
            if (!LevelManager.Started)
                return;

            if (LevelManager.Ended)
                return;

            if (LevelManager.Paused)
                return;

            if (ActiveControl == null)
                return;

            ControlledRigidbody.velocity = ActiveControl.GetCurrentVelocity();

            OnMovementUpdate?.Invoke();
        }

        // ##### EVENTS ##### \\

        public delegate void activeControlEventHandler(IMovementControl current_control);
        public event activeControlEventHandler OnActiveControlChange;

        public delegate void MovementUpdateEventController();
        public event MovementUpdateEventController OnMovementUpdate;

        // ##### CONTROL STARTER ##### \\

        public IMovementControl PreviousPrimaryControl { get; private set; }
        public IMovementControl PreviousControl { get; private set; }
        public IMovementControl ActiveControl { get; private set; }

        private void TryStartControl(IMovementControl control, MovementControlArgs start_args)
        {
            if (LevelManager.Paused)
                return;

            if (LevelManager.Ended)
                return;

            if (!ActiveControl.CanExit(control))
                return;

            if (!control.CanStart())
                return;

            StartControl(control, start_args);
        }

        private void ForceStartControl(IMovementControl control, MovementControlArgs start_args)
        {
            if (LevelManager.Paused)
                return;

            if (LevelManager.Ended)
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

            if (ActiveControl is IPrimaryControl)
                PreviousPrimaryControl = ActiveControl;

            ActiveControl = control;
            ActiveControl.Start(start_args);

            OnActiveControlChange?.Invoke(control);

            //Debug.Log($"Changed control is {control}.");
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
            GetControlTypes();

            GenerateControls();

            RegisterContracts();
        }

        private void RegisterContracts()
        {
            foreach (IMovementControl control in _controls)
            {
                if (control is IExplicitControl explicit_control)
                    explicit_control.OnExplicitDetection += ForceStartControl;
                if (control is ITransitionable transitionable)
                    transitionable.OnTransitionable += ForceStartControl;
                if (control is IInputableControl inputable)
                    inputable.OnInputDetected += TryStartControl;
            }
        }

        private void GenerateControls()
        {
            _controls = new();
            foreach (System.Type tControl in _controlTypes)
            {
                IMovementControl control = (IMovementControl)System.Activator.CreateInstance(tControl, this, _data.GetControlDataForControlType(tControl));
                _controls.Add(control);
            }
        }

        private List<System.Type> _controlTypes;
        private void GetControlTypes()
        {
            _controlTypes = new();
            foreach (Assembly asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetName().Name.Equals("JumpMaster.Movement"))
                {
                    foreach (System.Type t in asm.GetTypes())
                    {
                        if (t.GetInterface("IMovementControl") != null && !t.IsAbstract)
                            _controlTypes.Add(t);
                    }
                    return;
                }
            }
        }

        // ##### POSITION ##### \\

        private void ResetPlayerPosition()
        {
            Vector3 startPosition = c_camera.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, 0f, _data.Z_Position));
            startPosition.y += (Bounds.WorldMax.y - Bounds.WorldMin.y) * 0.5f;
            transform.position = startPosition;
        }

        // ##### CONSTRAINT ##### \\

        private void Constrain(bool constrain)
        {
            if (constrain)
                c_rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            else
                c_rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
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

        private Camera c_camera;

        public BoundingBox Bounds => c_bounds;
        private BoundingBox c_bounds;

        public Rigidbody2D ControlledRigidbody => c_rigidbody;
        private Rigidbody2D c_rigidbody;

        private void Cache()
        {
            c_camera = Camera.main;
            c_rigidbody = GetComponent<Rigidbody2D>();

            for (int c = 0; c < transform.childCount; c++)
            {
                c_bounds = transform.GetChild(c).GetComponent<BoundingBox>();
            }
        }
    }
}