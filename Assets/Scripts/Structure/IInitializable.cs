namespace JumpMaster.Structure
{
    public delegate void InitializationEventHandler();

    public interface IInitializable
    {
        public bool Initialized { get; }
        //public event InitializationEventHandler OnInitialization;
    }
}