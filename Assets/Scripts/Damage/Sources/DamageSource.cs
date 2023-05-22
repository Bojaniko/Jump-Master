using UnityEngine;

namespace JumpMaster.Damage
{
    /// <summary>
    /// A base class for all DamageSource data.
    /// </summary>
    public abstract class DamageSourceData
    {
        public Vector2 Origin => _origin;
        private readonly Vector2 _origin;

        public float StartTime => _startTime;
        private readonly float _startTime;

        protected DamageSourceData(Vector2 origin)
        {
            _origin = origin;
            _startTime = Time.time;
        }
    }

    public abstract class DamageSource
    {
        public readonly string TargetTag;
        public readonly float MaxDamageOutput;

        protected DamageSource(string target_tag, float max_damage_output)
        {
            TargetTag = target_tag;
            MaxDamageOutput = max_damage_output;
        }

        /// <summary>
        /// This boolean tells the Damage Controller if the source has been recorded,
        /// if it has then the source no longer exists.
        /// </summary>
        public abstract bool Recorded { get; }

        /// <summary>
        /// This method is used by the Damage Controller to process
        /// all the damage outputs.
        /// </summary>
        /// <param name="layer_mask">The collision layer mask.</param>
        /// <returns></returns>
        public abstract IDamageRecord[] RecordDamage(int layer_mask);
    }
}