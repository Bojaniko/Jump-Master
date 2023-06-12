namespace JumpMaster.Movement
{
    public delegate void ExplicitControlEventHandler(IMovementControl control, MovementControlArgs args);

    public interface IExplicitControl
    {
        public event ExplicitControlEventHandler OnExplicitDetection;
    }
}