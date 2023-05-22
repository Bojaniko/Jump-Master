namespace JumpMaster.Movement
{
    public delegate void ChainEventHandler(int chain, int max_chains);

    /// <summary>
    /// Defines a control that can be chained together.
    /// </summary>
    public interface IChainable
    {
        public int Chain { get; }
        public event ChainEventHandler OnChain;
    }
}