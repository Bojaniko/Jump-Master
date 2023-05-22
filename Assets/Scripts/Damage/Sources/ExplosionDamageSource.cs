using System.Collections.Generic;
using UnityEngine;

namespace JumpMaster.Damage
{
    public class ExplosionData : DamageSourceData
    {
        public readonly float Radius;

        public ExplosionData(float radius, Vector2 origin) : base(origin)
        {
            Radius = radius;
        }
    }

    public class ExplosionDamageSource : DamageSource, ITimeable
    {
        public float Duration => _duration;
        private readonly float _duration;

        public readonly ExplosionData Data;

        public ExplosionDamageSource(ExplosionData data, float duration, float max_damage_output, string target_tag) : base(target_tag, max_damage_output)
        {
            Data = data;
            _duration = duration;

            _hitGameObjects = new();
            _records = new();
        }

        public override bool Recorded => Time.time - Data.StartTime >= Duration;

        private readonly List<GameObject> _hitGameObjects;
        private readonly List<IDamageRecord> _records;
        public override IDamageRecord[] RecordDamage(int layer_mask)
        {
            Collider2D[] hits;
            hits = Physics2D.OverlapCircleAll(Data.Origin, Data.Radius, layer_mask);

            if (hits.Length == 0f)
                return new IDamageRecord[0];

            _records.Clear();

            float time_multiplier = 1f - ((Time.time - Data.StartTime) / Duration);

            float distance;
            float damage;

            for (int i = 0; i < hits.Length; i++)
            {
                if (!hits[i].gameObject.CompareTag(TargetTag))
                    continue;

                if (_hitGameObjects.Contains(hits[i].gameObject))
                    continue;
                _hitGameObjects.Add(hits[i].gameObject);

                distance = Vector2.Distance(Data.Origin, hits[i].transform.position);
                damage = MaxDamageOutput * (distance / Data.Radius) * time_multiplier;

                _records.Add(new DamageRecord<ExplosionDamageSource>(hits[i].gameObject, Data.Origin, damage));
            }
            return _records.ToArray();
        }
    }
}