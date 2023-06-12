namespace JumpMaster.Damage
{
    public delegate void DamageSourceRecording(DamageSource sender);

    public abstract class DamageSource
    {
        public DamageSourceData Data => _data;
        private readonly DamageSourceData _data;

        /// <summary>
        /// A damage source is considered recorded when it has finished it's lifetime.
        /// </summary>
        public abstract event DamageSourceRecording OnRecorded;

        protected DamageSource(DamageSourceData data) =>
            _data = data;

        /// <summary>
        /// This method is used by the Damage Controller to process
        /// all the damage outputs.
        /// </summary>
        /// <param name="layer_mask">The collision layer mask.</param>
        /// <returns></returns>
        public abstract IDamageRecord[] RecordDamage(int layer_mask);
    }
}