using UnityEngine;

namespace JumpMaster.Damage
{
    public class DamageRegistration<DamageSourceType> : IDamageRegistration
        where DamageSourceType : DamageSource
    {
        public GameObject Target => _target;
        private readonly GameObject _target;

        public DamageDelegate Callback => _callback;
        private readonly DamageDelegate _callback;

        public System.Type GetDamageSourceType() => typeof(DamageSourceType);

        public DamageRegistration(GameObject target, DamageDelegate callback)
        {
            _target = target;
            _callback = callback;
        }
    }
}