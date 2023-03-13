using UnityEngine;

namespace JumpMaster.LevelControllers
{
    public readonly struct DamageInfo
    {
        public readonly Vector2 Position;
        public readonly float Radius;
        public readonly float Amount;

        public DamageInfo(Vector2 position, float radius, float amount)
        {
            Position = position;
            Radius = radius;
            Amount = amount;
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