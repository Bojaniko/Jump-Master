using UnityEngine;

using Studio28.Attributes;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Electro Ball Metrics", menuName = "Game/Obstacles/Metrics/Electro Ball")]
    public class ElectroBallSpawnMetricsSO : SpawnMetricsSO<ElectroBallSO, ElectroBallSpawnSO>
    {
        [SerializeField, MinMax(1f, 50f), Tooltip("The interval range for the random time delay to spawn an electro ball.")] private Vector2 _intervalRange;

        /// <summary>
        /// Get a random interval for spawning an electro ball.
        /// </summary>
        public float GetInterval()
        {
            return Random.Range(_intervalRange.x, _intervalRange.y);
        }
    }
}