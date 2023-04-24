namespace JumpMaster.Movement
{
    public delegate void ChainEventHandler(int chain, int max_chains);

    public interface IChainable
    {
        public event ChainEventHandler OnChain;
    }
}