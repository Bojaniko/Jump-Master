using UnityEngine;

using JumpMaster.Damage;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Falling Bomb Spawn Data", menuName = "Game/Obstacles/Spawn/Falling Bomb")]
    public class FallingBombSpawnSO : SpawnSO
    {
        /// <summary>
        /// The speed at which the bomb falls.
        /// </summary>
        public float FallSpeed => _fallSpeed;
        [SerializeField, Range(0f, 10f), Tooltip("The speed at which the bomb falls.")] private float _fallSpeed = 1.5f;

        /// <summary>
        /// The radius at which the falling bomb detects the player.
        /// </summary>
        public float DetectionRadius => _detectionRadius;
        [SerializeField, Range(0.1f, 20f), Tooltip("The radius at which the falling bomb detects the player.")] private float _detectionRadius = 5f;

        /// <summary>
        /// The distance from the player at which the detection circle is drawn to warn the player.
        /// </summary>
        public float DetectionShowDistance => _detectionShowDistance;
        [SerializeField, Range(1.5f, 10f), Tooltip("The distance from the player at which the detection circle is drawn.")] private float _detectionShowDistance = 6f;

        [Header("Explosion")]

        [SerializeField, Range(100, 2000), Tooltip("The duration (ms) until the bomb explodes, counted twice.")] private int _armingDuration = 800;
        public int ArmingDuration => _armingDuration;

        /// <summary>
        /// The data for the explosion damage.
        /// </summary>
        public ExplosionDataSO ExplosionData => _explosionData;
        [SerializeField, Tooltip("The data for the explosion damage.")] private ExplosionDataSO _explosionData;
    }
}