using UnityEngine;

namespace JumpMaster.Damage
{
    public abstract class DamageRecord<DamageSourceType> : IDamageRecord
        where DamageSourceType : DamageSource
    {
        /// <summary>
        /// The gameobject which has taken damage.
        /// </summary>
        public GameObject Target => _target;
        private readonly GameObject _target;

        /// <summary>
        /// The source position of the damage output.
        /// </summary>
        public Vector2 SourcePosition => _source;
        private readonly Vector2 _source;

        /// <summary>
        /// The final, total damage output.
        /// </summary>
        public float DamageOutput => _damageOutput;
        private readonly float _damageOutput;

        public System.Type GetDamageSourceType() => typeof(DamageSourceType);

        /// <summary>
        /// Create a damage record.
        /// </summary>
        /// <param name="target">The gameobject which has taken damage.</param>
        /// <param name="damage_output">Amount of damage to deal.</param>
        /// <param name="source_position">The source position of the damage output.</param>
        protected DamageRecord(GameObject target, Vector2 source_position, float damage_output)
        {
            _target = target;
            _damageOutput = damage_output;
            _source = source_position;
        }
    }
}