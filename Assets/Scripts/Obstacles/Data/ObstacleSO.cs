using UnityEngine;

namespace JumpMaster.Obstacles
{
    public abstract class ObstacleSO : ScriptableObject
    {
        /// <summary>
        /// The Z position used to prevent overlapping with other objects in the scene.
        /// </summary>
        public float Z_Position => _zPosition;
        [SerializeField, Range(0f, 100f), Tooltip("The Z position used to prevent overlapping with other objects in the scene.")]private float _zPosition = 3f;

        /// <summary>
        /// The scale of the obstacle.
        /// </summary>
        public float Scale => _scale;
        [SerializeField, Range(0.1f, 2f), Tooltip("The scale of the obstacle.")] private float _scale = 1f;
        
        public GameObject ObstaclePrefab => _obstaclePrefab;
        [SerializeField] private GameObject _obstaclePrefab;
    }
}