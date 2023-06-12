using UnityEngine;

namespace JumpMaster.Damage
{
    /// <summary>
    /// A base class for all DamageSource data.
    /// </summary>
    public abstract class DamageSourceData
    {
        public string TargetTag => _targetTag;
        private readonly string _targetTag;

        public float MaxDamageOutput => _maxDamageOutput;
        private readonly float _maxDamageOutput;

        public Vector2 Origin => _origin;
        private Vector2 _origin;

        public float StartTime => _startTime;
        private readonly float _startTime;

        protected DamageSourceData(Vector2 origin, float max_damage_output, string target_tag)
        {
            _targetTag = target_tag;
            _maxDamageOutput = max_damage_output;
            _origin = origin;
            _startTime = Time.time;
        }

        public void ChangeOrigin(Vector2 new_origin) =>
            _origin = new_origin;
    }
}
