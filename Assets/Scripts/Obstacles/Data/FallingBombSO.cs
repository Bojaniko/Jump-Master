using UnityEngine;

using Studio28.Audio.SFX;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Falling Bomb Data", menuName = "Game/Obstacles/Data/Falling Bomb")]
    public class FallingBombSO : ObstacleSO
    {
        /// <summary>
        /// The duration (ms) it takes the bombs detection field to appear/disappear.
        /// </summary>
        public int DetectionShowDuration => _detectionShowDuration;
        [SerializeField, Range(50, 2000), Tooltip("The duration (ms) it takes the bombs detection field to appear/disappear.")]
        private int _detectionShowDuration = 500;

        /// <summary>
        /// The color of the bombs light when armed (default: red)
        /// </summary>
        public Color ArmedLightColor => _armedLightColor;
        [SerializeField, ColorUsage(false, true), Tooltip("The color of the bombs light when armed (default: red)")]
        private Color _armedLightColor = Color.red;

        /// <summary>
        /// The color of the bombs light when not armed (default: green)
        /// </summary>
        public Color UnarmedLightColor => _unarmedLightColor;
        [SerializeField, ColorUsage(false, true), Tooltip("The color of the bombs light when not armed (default: green)")]
        private Color _unarmedLightColor = Color.green;

        [Header("Explosion")]

        [SerializeField, Tooltip("The explosion visual effect.")] private GameObject _explosionPrefab;
        /// <summary>
        /// The explosion visual effect.
        /// </summary>
        public GameObject ExplosionPrefab => _explosionPrefab;

        /// <summary>
        /// The delay (ms) until the bomb is despawned after exploding.
        /// </summary>
        public int ExplosionDestroyDelay => _explosionDestroyDelay;
        [SerializeField, Range(10, 500), Tooltip("The delay (ms) until the bomb is despawned after exploding.")]
        private int _explosionDestroyDelay = 150;

        [Header("Sound Effects")]

        [SerializeField, Tooltip("The beep sound when arming.")] private SoundEffectInfo _armingBeepSound;
        /// <summary>
        /// The beep sound when arming.
        /// </summary>
        public SoundEffectInfo ArmingBeepSound => _armingBeepSound;

        /// <summary>
        /// The explosion sound.
        /// </summary>
        public SoundEffectInfo ExplosionSound => _explosionSound;
        [SerializeField, Tooltip("The explosion sound.")] private SoundEffectInfo _explosionSound;
    }
}