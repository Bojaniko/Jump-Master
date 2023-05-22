using UnityEngine;

namespace JumpMaster.Damage
{
    public interface IDamageRegistration
    {
        public GameObject Target { get; }
        public DamageDelegate Callback { get; }
        public System.Type GetDamageSourceType();
    }
}