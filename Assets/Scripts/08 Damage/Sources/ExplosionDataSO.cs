using UnityEngine;

namespace JumpMaster.Damage
{
    [CreateAssetMenu(fileName = "Explosion Data", menuName = "Game/Damage/Explosion Data")]
    public class ExplosionDataSO : ScriptableObject
    {
        public float Damage => _damage;
        [SerializeField, Range(0f, 100f)] private float _damage = 30f;

        public float Radius => _radius;
        [SerializeField, Range(0.1f, 20f)] private float _radius = 10f;

        public float Duration => _duration;
        [SerializeField, Range(0.1f, 10f)] private float _duration = 1.5f;
    }
}