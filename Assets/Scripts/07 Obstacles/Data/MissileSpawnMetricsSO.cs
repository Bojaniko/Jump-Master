using UnityEngine;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Missile Spawn Metrics", menuName = "Game/Obstacles/Metrics/Missile Spawn")]
    public class MissileSpawnMetricsSO : SpawnMetricsSO<MissileSO, MissileSpawnSO>
    {
        /// <summary>
        /// The probability of a missile spawning.
        /// </summary>
        public int SpawnChance => _spawnChance;
        [SerializeField, Range(0, 100), Tooltip("The probability of a missile spawning.")] private int _spawnChance = 20;

        /// <summary>
        /// The interval between each missile try spawn.
        /// </summary>
        public float Interval => _interval;
        [SerializeField, Range(0f, 30f), Tooltip("The interval between each missile try spawn.")] private float _interval = 5f;
    }
}