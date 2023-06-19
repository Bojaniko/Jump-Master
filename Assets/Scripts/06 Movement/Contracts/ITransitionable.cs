namespace JumpMaster.Movement
{
    public delegate void TransitionableControlEventHandler(IMovementControl sender_control, IMovementControl transition_control, MovementControlArgs args);

    public interface ITransitionable
    {
        public event TransitionableControlEventHandler OnTransitionable;

        public MovementState TransitionState { get; }
    }
}