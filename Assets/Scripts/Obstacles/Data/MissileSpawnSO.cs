using UnityEngine;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Missile Spawn Data", menuName = "Game/Obstacles/Spawn/Missile")]
    public class MissileSpawnSO : SpawnSO
    {
        /// <summary>
        /// The movement speed of the missile.
        /// </summary>
        public float Speed => _speed;
        [SerializeField, Range(1f, 50f), Tooltip("The movement speed of the missile.")] private float _speed = 10f;
    }
}