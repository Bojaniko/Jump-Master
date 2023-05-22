using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Laser Gate Spawn Metrics", menuName = "Game/Obstacles/Metrics/Laser Gate")]
    public class LaserGateSpawnMetricsSO : SpawnMetricsSO<LaserGateSO, LaserGateSpawnSO>
    {
        [Range(1f, 50f)]
        public float Interval = 10f;

        [Range(1f, 30f)]
        public float MinimumDistance = 5f;
    }
}