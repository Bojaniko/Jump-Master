using UnityEngine;

namespace JumpMaster.LevelControllers
{
    public delegate void DamageEventController(DamageInfo damage);
    public enum DamageType { EXPLOSION, STUN, PROJECTILE, TRAP }

    public readonly struct DamageInfo
    {
        public readonly DamageType TypeOfDamage;
        public readonly Vector2 Position;
        public readonly Vector2 Direction;
        public readonly float Radius;
        public readonly float Amount;

        public DamageInfo(Vector2 position, Vector2 direction, float radius, float amount, DamageType damage_type)
        {
            Position = position;
            Direction = direction.normalized;
            Radius = radius;
            Amount = amount;
            TypeOfDamage = damage_type;
        }
    }

    public static class DamageController
    {
        public static DamageEventController OnDamageLogged;
        public static DamageEventController OnPlayerDamageLogged;

        public static void LogDamage(DamageInfo damage)
        {
            OnDamageLogged?.Invoke(damage);
        }

        public static void LogPlayerDamage(DamageInfo damage)
        {
            OnPlayerDamageLogged?.Invoke(damage);
        }
    }
}