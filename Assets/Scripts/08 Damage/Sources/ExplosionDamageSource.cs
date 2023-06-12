using System.Collections.Generic;
using UnityEngine;

using JumpMaster.LevelTrackers;

namespace JumpMaster.Damage
{
    public sealed class ExplosionDamageRecord : DamageRecord<ExplosionDamageSource>
    {
        public float RemainingDuration => _remainingDuration;
        private readonly float _remainingDuration;

        public ExplosionDamageRecord(GameObject target, Vector2 source_position, float damage_output, float remaining_duration)
            : base(target, source_position, damage_output)
        {
            _remainingDuration = remaining_duration;
        }
    }

    public sealed class ExplosionData : DamageSourceData
    {
        public float Duration => _duration;
        private readonly float _duration;

        public float Radius => _radius;
        private readonly float _radius;

        public ExplosionData(float radius, float duration, Vector2 origin, float max_damage_output, string target_tag) : base(origin, max_damage_output, target_tag)
        {
            _duration = duration;
            _radius = radius;
        }
    }

    public class ExplosionDamageSource : DamageSource
    {
        public ExplosionData ExplosionData => (ExplosionData)Data;

        public ExplosionDamageSource(ExplosionData data) : base(data)
        {
            _hitGameObjects = new();
            _records = new();

            TimeTracker.Instance.StartTimeTracking(EndDamageSource, ExplosionData.Duration, Data.StartTime);
        }

        private void EndDamageSource() =>
            OnRecorded?.Invoke(this);

        public override event DamageSourceRecording OnRecorded;

        private readonly List<GameObject> _hitGameObjects;
        private readonly List<IDamageRecord> _records;

        public override IDamageRecord[] RecordDamage(int layer_mask)
        {
            Collider2D[] hits;
            hits = Physics2D.OverlapCircleAll(Data.Origin, ExplosionData.Radius, layer_mask);

            if (hits.Length == 0f)
                return new IDamageRecord[0];

            _records.Clear();

            float remainingTime = ExplosionData.Duration - (Time.time - Data.StartTime); 
            float timeMultiplier = 1f - ((Time.time - Data.StartTime) / ExplosionData.Duration);

            float distance;
            float damage;

            for (int i = 0; i < hits.Length; i++)
            {
                if (!hits[i].gameObject.CompareTag(Data.TargetTag))
                    continue;

                if (_hitGameObjects.Contains(hits[i].gameObject))
                    continue;
                _hitGameObjects.Add(hits[i].gameObject);

                distance = Vector2.Distance(Data.Origin, hits[i].transform.position);
                damage = Data.MaxDamageOutput * (distance / ExplosionData.Radius) * timeMultiplier;

                _records.Add(new ExplosionDamageRecord(hits[i].gameObject, Data.Origin, damage, remainingTime));
            }
            return _records.ToArray();
        }
    }
}