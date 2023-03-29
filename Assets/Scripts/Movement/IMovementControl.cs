using UnityEngine;

namespace JumpMaster.Movement
{
    public delegate void ControlActivityEventHandler();

    public interface IMovementControl
    {
        public event ControlActivityEventHandler OnStart;
        public event ControlActivityEventHandler OnExit;

        public MovementState ActiveState { get; }
        public bool Started { get; }


        public MovementController Controller { get; }

        public MovementControlArgs ControlArgs { get; }

        public bool CanExit();
        public bool CanStart();

        public void Start(MovementControlArgs args);
        public void Exit();
        public void Resume();
        public void Pause();
        public Vector3 GetCurrentVelocity();
    }
}