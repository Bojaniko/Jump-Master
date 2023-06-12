using System.Collections.Generic;
using UnityEngine;

using JumpMaster.LevelTrackers;

namespace JumpMaster.Damage
{
    public sealed class StunAreaDamageRecord : DamageRecord<StunAreaDamageSource>
    {
        public float RemainingDuration => _remainingDuration;
        private readonly float _remainingDuration;

        public StunAreaDamageRecord(GameObject target, Vector2 source_position, float damage_output, float remaining_duration) :
            base(target, source_position, damage_output)
        {
            _remainingDuration = remaining_duration;
        }
    }

    public sealed class StunAreaDamageSourceData : DamageSourceData
    {
        public Vector2 PointA => _pointA;
        private readonly Vector2 _pointA;

        public Vector2 PointB => _pointB;
        private readonly Vector2 _pointB;

        public float Duration => _duration;
        private readonly float _duration;

        public StunAreaDamageSourceData(Vector2 point_a, Vector2 point_b, float duration, Vector2 origin, float max_damage_output, string target_tag) : base(origin, max_damage_output, target_tag)
        {
            _pointA = point_a;
            _pointB = point_b;

            _duration = duration;
        }
    }

    public class StunAreaDamageSource : DamageSource
    {
        public StunAreaDamageSourceData AreaData => (StunAreaDamageSourceData)Data;

        public StunAreaDamageSource(StunAreaDamageSourceData data) : base(data)
        {
            _records = new();
            _hitGameObjects = new();

            TimeTracker.Instance.StartTimeTracking(EndSource, AreaData.Duration);
        }

        private void EndSource() =>
            OnRecorded?.Invoke(this);
        public override event DamageSourceRecording OnRecorded;

        private readonly List<IDamageRecord> _records;
        private readonly List<GameObject> _hitGameObjects;
        public override IDamageRecord[] RecordDamage(int layer_mask)
        {
            Collider2D[] hits;
            hits = Physics2D.OverlapAreaAll(AreaData.PointA, AreaData.PointB, layer_mask);

            if (hits.Length == 0f)
                return new IDamageRecord[0];

            _records.Clear();

            float remainingDuration = AreaData.Duration - (Time.time - Data.StartTime);

            for (int i = 0; i < hits.Length; i++)
            {
                if (!hits[i].gameObject.CompareTag(Data.TargetTag))
                    continue;

                if (_hitGameObjects.Contains(hits[i].gameObject))
                    continue;
                _hitGameObjects.Add(hits[i].gameObject);

                _records.Add(new StunAreaDamageRecord(hits[i].gameObject, Data.Origin, Data.MaxDamageOutput, remainingDuration));
            }
            return _records.ToArray();
        }
    }
}