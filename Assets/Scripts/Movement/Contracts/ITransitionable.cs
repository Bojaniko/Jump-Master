namespace JumpMaster.Movement
{
    public delegate void TransitionableControlEventHandler(IMovementControl transition_to_control, MovementControlArgs args);

    public interface ITransitionable
    {
        public event TransitionableControlEventHandler OnTransitionable;

        public MovementState TransitionState { get; }
    }
}