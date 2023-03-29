namespace JumpMaster.Movement
{
    public delegate void TransitionableControlEventHandler(IMovementControl control, MovementControlArgs args);

    public interface ITransitionable
    {
        public event TransitionableControlEventHandler OnTransitionable;

        public MovementState TransitionState { get; }
    }
}