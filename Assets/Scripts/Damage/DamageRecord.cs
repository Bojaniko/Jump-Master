using UnityEngine;

namespace JumpMaster.Damage
{
    public struct DamageRecord<DamageSourceType> : IDamageRecord
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

        /// <summary>
        /// The remaining duration of the damage output.
        /// </summary>
        public float Duration => _duration;
        private readonly float _duration;

        public System.Type GetDamageSourceType() => typeof(DamageSourceType);

        /// <summary>
        /// Create a damage record.
        /// </summary>
        /// <param name="target">The gameobject which has taken damage.</param>
        /// <param name="damage_output">Amount of damage to deal.</param>
        /// <param name="source_position">The source position of the damage output.</param>
        public DamageRecord(GameObject target, Vector2 source_position, float damage_output)
        {
            _target = target;
            _damageOutput = damage_output;
            _source = source_position;
            _duration = 0f;
        }

        public DamageRecord(GameObject target, Vector2 source_position, float damage_output, float duration)
        {
            _target = target;
            _damageOutput = damage_output;
            _source = source_position;
            _duration = duration;
        }
    }

    /*public abstract class DamageRecord
    {
        public readonly float RecordingTime;

        public float Damage => _damage;
        private float _damage;

        protected void UpdateDamageOutput(float damage)
        {
            _damage = damage;
        }

        protected DamageRecord(float damage)
        {
            RecordingTime = Time.time;
            _damage = damage;
        }
    }

    public class ProjectileDamageRecord : DamageRecord
    {
        public readonly GameObject Target;

        public ProjectileDamageRecord(float damage, float distance, int layer_mask, string target_tag, Vector2 origin, Vector2 direction) : base(damage)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, layer_mask);

            if (hit.collider.gameObject.CompareTag(target_tag))
                Target = hit.collider.gameObject;
        }
    }

    public class CollisionDamageRecord : DamageRecord
    {
        public readonly Vector2 Direction;
        public readonly GameObject Target;

        public CollisionDamageRecord(float damage, Vector2 direction, GameObject target) : base(damage)
        {
            Direction = direction;
            Target = target;
        }
    }

    public class StunDamageRecord : DamageRecord
    {
        public readonly float Duration;
        public readonly GameObject Target;

        public StunDamageRecord(float damage, float duration, GameObject target) : base(damage)
        {
            Duration = duration;
            Target = target;
        }
    }

    public class ExplosionDamageRecord : DamageRecord
    {
        public readonly float Radius;
        public readonly Vector2 Origin;

        public ExplosionDamageRecord(float damage, float radius, Vector2 origin, int layer_mask) : base(damage)
        {
            Radius = radius;
            Origin = origin;

            //RaycastHit2D[] targets = Physics2D.CircleCastAll(origin, radius, layer_mask);
        }
    }*/
}