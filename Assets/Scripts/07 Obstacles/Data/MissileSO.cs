using UnityEngine;

using Studio28.Audio.SFX;

using JumpMaster.UI.Data;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Missile Data", menuName = "Game/Obstacles/Data/Missile")]
    public class MissileSO : ObstacleSO
    {
        public MissileWarningSO WarningData => _warningData;
        [SerializeField] private MissileWarningSO _warningData;

        /// <summary>
        /// The positional offset in the specified direction when the missile is spawned.
        /// </summary>
        public float SpawnOffset => _spawnOffset;
        [SerializeField, Range(0f, 5f), Tooltip("The positional offset in the specified direction when the missile is spawned.")] private float _spawnOffset = 0.5f;

        [Header("Sound Effects")]

        [SerializeField] private MinMaxSoundEffectInfo _thrustSFX;
        public MinMaxSoundEffectInfo ThrustSFX => _thrustSFX;

        public RandomizedSoundEffectInfo ExplosionSFX => _explosionSFX;
        [SerializeField] private RandomizedSoundEffectInfo _explosionSFX;

        [Header("Explosion")]

        [SerializeField, Tooltip("The effect which is instantiated when the missile explodes.")] private GameObject _explosionPrefab;
        /// <summary>
        /// The effect which is instantiated when the missile explodes.
        /// </summary>
        public GameObject ExplosionPrefab => _explosionPrefab;

        /// <summary>
        /// The delay (ms) at which the missile is destroyed after it explodes.
        /// </summary>
        public int ExplosionDestroyDelayMS => _explosionDestroyDelayMS;
        [SerializeField, Range(10, 500), Tooltip("The delay (ms) at which the missile is destroyed after it explodes.")] private int _explosionDestroyDelayMS = 150;
    }
}