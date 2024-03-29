using UnityEngine;

namespace JumpMaster.Movement
{
    public abstract class MovementControl<Data, Args> : IMovementControl
        where Data : MovementControlDataSO
        where Args : MovementControlArgs
    {
        protected MovementControl(MovementController controller, Data data)
        {
            _controller = controller;

            _started = false;
            ControlData = data;

            MovementController.Instance.OnMovementUpdate += OnMovementUpdate;
        }


        public event ControlActivityEventHandler OnStart;
        public event ControlActivityEventHandler OnExit;

        /// <summary>
        /// Is this control started?
        /// </summary>
        public bool Started => _started;
        private bool _started;

        protected Args _controlArgs;
        public MovementControlArgs ControlArgs => _controlArgs;

        public readonly Data ControlData;

        private readonly MovementController _controller;
        public MovementController Controller => _controller;

        protected abstract void OnMovementUpdate();
        public abstract MovementState ActiveState { get; }
        
        public abstract void Resume();
        public abstract void Pause();

        public bool CanStart()
        {
            return CanStartControl();
        }
        protected abstract bool CanStartControl();
        public void Start(MovementControlArgs args)
        {
            if (_started)
                return;

            if (!args.GetType().Equals(typeof(Args)))
                throw new InvalidControlArgumentsTypeException();
            _controlArgs = (Args)args;

            StartControl();
            _started = true;

            OnStart?.Invoke();
        }
        protected abstract void StartControl();

        public abstract bool CanExit(IMovementControl exit_control);
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

        public abstract Vector2 GetCurrentVelocity();
    }
}