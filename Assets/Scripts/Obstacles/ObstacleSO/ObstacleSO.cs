using UnityEngine;

namespace JumpMaster.Obstacles
{
    public abstract class ObstacleSO : ScriptableObject
    {
        [Range(0f, 100f)]
        public float Z_Position = 3f;

        [Range(0.1f, 2f)]
        public float Scale = 1f;

        public GameObject ObstaclePrefab;
    }
}