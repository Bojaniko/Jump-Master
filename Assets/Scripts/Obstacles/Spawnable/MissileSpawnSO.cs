using UnityEngine;

using GD.MinMaxSlider;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Missile Spawn Data", menuName = "Game/Obstacles/Spawn/Missile")]
    public class MissileSpawnSO : SpawnSO
    {
        [Range(1f, 50f)]
        public float Speed = 10f;

        [MinMaxSlider(2f, 10f)]
        public Vector2 CountdownRange = new(2f, 5f);

        public float GetCountdown()
        {
            return Random.Range(CountdownRange.x, CountdownRange.y);
        }
    }
}