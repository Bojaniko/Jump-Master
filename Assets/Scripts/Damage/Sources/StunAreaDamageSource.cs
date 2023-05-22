using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpMaster.Damage
{
    public class AreaDamageSourceData : DamageSourceData
    {
        public readonly Vector2 PointA;
        public readonly Vector2 PointB;

        public AreaDamageSourceData(Vector2 point_a, Vector2 point_b, Vector2 origin) : base(origin)
        {
            PointA = point_a;
            PointB = point_b;
        }
    }

    public class StunAreaDamageSource : DamageSource, ITimeable
    {
        public readonly AreaDamageSourceData Data;
        public StunAreaDamageSource(AreaDamageSourceData data, float duration, float max_damage_output, string target_tag) : base(target_tag, max_damage_output)
        {
            Data = data;
            _duration = duration;

            _records = new();
            _hitGameObjects = new();
        }

        public override bool Recorded => Time.time - Data.StartTime >= Duration;

        public float Duration => _duration;
        private readonly float _duration;

        private readonly List<IDamageRecord> _records;
        private readonly List<GameObject> _hitGameObjects;
        public override IDamageRecord[] RecordDamage(int layer_mask)
        {
            Collider2D[] hits;
            hits = Physics2D.OverlapAreaAll(Data.PointA, Data.PointB, layer_mask);

            if (hits.Length == 0f)
                return new IDamageRecord[0];

            _records.Clear();

            float remainingDuration = Duration - (Time.time - Data.StartTime);

            for (int i = 0; i < hits.Length; i++)
            {
                if (!hits[i].gameObject.CompareTag(TargetTag))
                    continue;

                if (_hitGameObjects.Contains(hits[i].gameObject))
                    continue;
                _hitGameObjects.Add(hits[i].gameObject);

                _records.Add(new DamageRecord<StunAreaDamageSource>(hits[i].gameObject, Data.Origin, MaxDamageOutput, remainingDuration));
            }
            return _records.ToArray();
        }
    }
}