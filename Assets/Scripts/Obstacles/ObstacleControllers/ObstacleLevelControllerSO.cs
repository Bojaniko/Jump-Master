using UnityEngine;

using JumpMaster.LevelControllers.Obstacles;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Obstacle Controller Data", menuName = "Obstacle Controller")]
    public class ObstacleLevelControllerSO : ScriptableObject
    {
        [Range(1f, 50f)]
        public float WaveInterval = 10f;
        [Range(1f, 50f)]
        public float FirstWaveInterval = 5f;
        [Range(1, 20)]
        public int BossWaveCount = 3;

        [Range(100, 5000)]
        public int SpawnCheckInterval = 500;

        [Range(1, 30)]
        public int MaxObstaclesAtOnce = 10;

        [Range(0f, 100f)]
        public float TopSpawnMarginPercentage = 10f;

        public ObstacleControllersSO DefaultControllersData;

        public WaveSO[] NormalWaveData;
        public WaveSO[] BossWaveData;

        public WaveSO GetRandomBossWave()
        {
            if (BossWaveData.Length == 0)
                return null;
            return BossWaveData[Random.Range(0, BossWaveData.Length)];
        }

        public WaveSO GetRandomNormalWave()
        {
            if (NormalWaveData.Length == 0)
                return null;
            return NormalWaveData[Random.Range(0, NormalWaveData.Length)];
        }
    }
}