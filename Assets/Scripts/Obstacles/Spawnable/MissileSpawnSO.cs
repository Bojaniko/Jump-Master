using UnityEngine;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Missile Spawn Data", menuName = "Game/Obstacles/Spawn/Missile")]
    public class MissileSpawnSO : SpawnSO
    {
        [Range(1f, 50f)] public float Speed = 10f;
    }
}