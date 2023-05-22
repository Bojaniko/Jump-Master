using UnityEngine;

namespace JumpMaster.Movement
{
    public delegate void ControlActivityEventHandler();

    /// <summary>
    /// The movement control contract.
    /// </summary>
    public interface IMovementControl
    {
        public event ControlActivityEventHandler OnStart;
        public event ControlActivityEventHandler OnExit;

        public MovementState ActiveState { get; }
        public bool Started { get; }


        public MovementController Controller { get; }

        public MovementControlArgs ControlArgs { get; }

        public bool CanExit(IMovementControl exit_control);
        public bool CanStart();

        public void Start(MovementControlArgs args);
        public void Exit();
        public void Resume();
        public void Pause();
        public Vector2 GetCurrentVelocity();
    }
}