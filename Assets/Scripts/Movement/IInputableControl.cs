namespace JumpMaster.Movement
{
    public delegate void ControlInputEventHandler(IMovementControl control, MovementControlArgs args);

    public interface IInputableControl
    {
        public event ControlInputEventHandler OnInputDetected;
    }
}