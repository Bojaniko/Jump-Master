using UnityEngine;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Laser Gate Spawn Data", menuName = "Game/Obstacles/Spawn/Laser Gate")]
    public class LaserGateSpawnSO : SpawnSO
    {
        [Range(100, 2500)]
        public int CountdownIntervalMS = 800;

        [Range(0.5f, 10f)]
        public float GateHoldTime = 1.5f;

        [Range(6f, 20f)]
        public float GateWidth = 8f;
    }
}