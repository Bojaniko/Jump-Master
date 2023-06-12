using UnityEngine;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Electro Ball Spawn", menuName = "Game/Obstacles/Spawn/Electro Ball")]
    public class ElectroBallSpawnSO : SpawnSO
    {
        /// <summary>
        /// The speed at which the ball moves to it's target position to intercept the player.
        /// </summary>
        public float MovementSpeed => _movementSpeed;
        [SerializeField, Tooltip("The speed at which the ball moves to it's target position to intercept the player.")] private float _movementSpeed;

        /// <summary>
        /// The number of times the ball tries to intercept the player.
        /// </summary>
        public int Interceptions => _interceptions;
        [SerializeField, Range(1, 10), Tooltip("The number of times the ball tries to intercept the player.")]private int _interceptions;

        /// <summary>
        /// The duration (s) the ball stays open to shoot a laser in between.
        /// </summary>
        public float LaserHoldDuration => _laserHoldDuration;
        [SerializeField, Range(1f, 20f), Tooltip("The duration (s) the ball stays open to shoot a laser in between.")] private float _laserHoldDuration;

        /// <summary>
        /// The damage caused to the player when shocked by the laser.
        /// </summary>
        public float LaserDamage => _laserDamage;
        [SerializeField, Range(1f, 100f), Tooltip("The damage caused to the player when shocked by the laser.")]private float _laserDamage;
    }
}