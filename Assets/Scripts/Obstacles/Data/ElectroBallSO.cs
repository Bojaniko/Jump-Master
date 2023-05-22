using UnityEngine;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Electro Ball Data", menuName = "Game/Obstacles/Data/Electro Ball")]
    public class ElectroBallSO : ObstacleSO
    {
        /// <summary>
        /// The vertical distance the ball stops from the player.
        /// </summary>
        public float StopPositionFromPlayer => _stopPositionFromPlayer;
        [SerializeField, Range(0f, 5f), Tooltip("The vertical distance the ball stops from the player.")] private float _stopPositionFromPlayer;

        /// <summary>
        /// The speed at which the ball escapes the screen when finished intercepting.
        /// </summary>
        public float EscapeMovementSpeed => _escapeMovementSpeed;
        [SerializeField, Range(1f, 20f), Tooltip("The speed at which the ball escapes the screen when finished intercepting.")] private float _escapeMovementSpeed;
    }
}