using System.Collections.Generic;
using UnityEngine;

namespace JumpMaster.Damage
{
    public sealed class ProjectileDamageRecord : DamageRecord<ProjectileDamageSource>
    {
        public Vector2 Direction => _direction;
        private readonly Vector2 _direction;

        public ProjectileDamageRecord(GameObject target, Vector2 source_position, Vector2 direction, float damage_output)
            : base(target, source_position, damage_output)
        {
            _direction = direction;
        }
    }

    public sealed class ProjectileDamageData : DamageSourceData
    {
        public float ProjectileSize => _projectileSize;
        private readonly float _projectileSize;

        /// <summary>
        /// An angle between 0 and 180 degrees.
        /// </summary>
        public float MaxCollisionAngle => _maxCollisionAngle;
        private readonly float _maxCollisionAngle;

        public Vector2 Direction => _direction;
        private Vector2 _direction;

        public ProjectileDamageData(Vector2 initial_direction, float projectile_size, float max_collision_angle, Vector2 initial_origin, float max_damage_output, string target_tag)
            : base(initial_origin, max_damage_output, target_tag)
        {
            _direction = initial_direction;
            _projectileSize = projectile_size;
            _maxCollisionAngle = Mathf.Clamp(max_collision_angle, 0f, 180f);
        }

        public void ChangeDirection(Vector2 new_direction) =>
            _direction = new_direction;
    }

    public class ProjectileDamageSource : DamageSource
    {
        public override event DamageSourceRecording OnRecorded;
        public ProjectileDamageData ProjectileData => (ProjectileDamageData)Data;

        private readonly float _maxAngleDot;
        public ProjectileDamageSource(ProjectileDamageData data) : base(data)
        {
            _maxAngleDot = (1f - (ProjectileData.MaxCollisionAngle / 180f)) - (ProjectileData.MaxCollisionAngle / 180f);
        }

        public override IDamageRecord[] RecordDamage(int layer_mask)
        {
            Collider2D[] hits;
            hits = Physics2D.OverlapCircleAll(Data.Origin, ProjectileData.ProjectileSize, layer_mask);

            if (hits.Length == 0f)
                return new IDamageRecord[0];

            List<IDamageRecord> records = new();

            foreach (Collider2D col in hits)
            {
                if (!col.CompareTag(Data.TargetTag))
                    continue;
                if (Vector2.Dot(ProjectileData.Direction, Data.Origin - (Vector2)col.transform.position) < _maxAngleDot)
                    continue;
                records.Add(new ProjectileDamageRecord(col.gameObject, Data.Origin, ProjectileData.Direction, Data.MaxDamageOutput));
            }

            OnRecorded?.Invoke(this);

            return records.ToArray();
        }
    }
}