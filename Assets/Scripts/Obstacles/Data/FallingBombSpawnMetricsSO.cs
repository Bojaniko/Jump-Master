using UnityEngine;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Falling Bomb Spawn Metrics", menuName = "Game/Obstacles/Metrics/Falling Bomb")]
    public class FallingBombSpawnMetricsSO : SpawnMetricsSO<FallingBombSO, FallingBombSpawnSO>
    {
        /// <summary>
        /// The probability of the bomb being spawned after an interval.
        /// </summary>
        public int SpawnChance => _spawnChance;
        [SerializeField, Range(0, 100), Tooltip("The probability of the bomb being spawned after an interval.")] private int _spawnChance = 20;

        /// <summary>
        /// The interval (s) from when the next bomb can be spawned.
        /// </summary>
        public float Interval => _interval;
        [SerializeField, Range(1f, 50f), Tooltip("The interval (s) from when the next bomb can be spawned.")] private float _interval = 15f;

        /// <summary>
        /// The interval (s) after the first interval if it was unsucessful.
        /// </summary>
        public float SecondaryInterval => _secondaryInterval;
        [SerializeField, Range(1f, 30f), Tooltip("The interval (s) after the first interval if it was unsucessful.")] private float _secondaryInterval = 4f;

        /// <summary>
        /// The positional offset from the screen edge.
        /// </summary>
        public float SpawnOffset => _spawnOffset;
        [SerializeField, Range(0f, 5f), Tooltip("The positional offset from the screen edge.")] private float _spawnOffset = 0.5f;
    }
}