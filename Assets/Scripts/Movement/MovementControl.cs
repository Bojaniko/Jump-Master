using UnityEngine;

namespace JumpMaster.Movement
{
    public abstract class MovementControl<Data> : IMovementControl
                where Data : MovementControlDataSO
    {
        public MovementControl(MovementController controller, Data data)
        {
            _controller = controller;

            _started = false;
            ControlData = data;

            MovementController.Instance.OnMovementUpdate += OnMovementUpdate;
            ControlArgs = new(Controller.ControlledRigidbody, MovementDirection.Zero, 1f);
        }


        public event ControlActivityEventHandler OnStart;
        public event ControlActivityEventHandler OnExit;

        private bool _started;
        public bool Started { get { return _started; } }

        public MovementControlArgs ControlArgs { get; private set; }
        public readonly Data ControlData;

        private readonly MovementController _controller;
        public MovementController Controller { get { return _controller; } }

        protected abstract void OnMovementUpdate();
        public abstract MovementState ActiveState { get; }
        public abstract bool CanExit();

        public bool CanStart()
        {
            if (_started)
                return false;

            if (Controller.ActiveControl == this)
                return false;

            if (!Controller.ActiveControl.CanExit())
                return false;

            return CanStartControl();
        }
        protected abstract bool CanStartControl();
        
        public abstract void Resume();
        public abstract void Pause();

        public void Start(MovementControlArgs args)
        {
            if (_started)
                return;
            ControlArgs = args;
            StartControl();
            _started = true;
            if (OnStart != null)
                OnStart();
        }
        protected abstract void StartControl();

        public void Exit()
        {
            if (!_started)
                return;
            ExitControl();
            _started = false;
            if (OnExit != null)
                OnExit();
        }
        protected abstract void ExitControl();

        public abstract Vector3 GetCurrentVelocity();

        protected void UpdateDirection(MovementDirection direction)
        {
            ControlArgs = MovementControlArgs.UpdateDirection(ControlArgs, direction);
        }
    }
}