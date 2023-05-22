using UnityEngine;

namespace JumpMaster.Damage
{
    public interface IDamageRecord
    {
        public GameObject Target { get;  }
        public Vector2 SourcePosition { get; }
        public float DamageOutput { get; }
        public float Duration { get; }

        public System.Type GetDamageSourceType();
    }
}