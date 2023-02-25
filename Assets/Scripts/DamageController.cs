using UnityEngine;

namespace JumpMaster.LevelControllers
{
    public struct DamageInfo
    {
        public readonly Vector2 Position;
        public readonly float Duration;
        public readonly float Radius;
        public readonly float Amount;

        public readonly float StartTime;

        public DamageInfo(Vector2 position, float duration, float radius, float amount)
        {
            Position = position;
            Duration = duration;
            Radius = radius;
            Amount = amount;

            StartTime = Time.time;
        }
    }

    public static class DamageController
    {
        public delegate void DamageEventController(DamageInfo damage);
        public static DamageEventController OnDamageLogged;

        public static void LogDamage(DamageInfo damage)
        {
            if (OnDamageLogged != null)
                OnDamageLogged(damage);
        }
    }
}