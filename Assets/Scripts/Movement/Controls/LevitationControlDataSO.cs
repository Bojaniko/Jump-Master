using UnityEngine;

namespace JumpMaster.Movement
{
    [CreateAssetMenu(fileName = "Levitation Data", menuName = "Game/Movement/Levitation Data")]
    public class LevitationControlDataSO : MovementControlDataSO
    {
        /// <summary>
        /// The slight gravitational force the character will face while levitating.
        /// </summary>
        public float GravityForce => _gravityForce;
        [SerializeField, Range(0f, 5f), Tooltip("The slight gravitational force the character will face while levitating.")] private float _gravityForce;

        /// <summary>
        /// The maximum duration the character can levitate in mid air.
        /// </summary>
        public float Duration => _duration;
        [SerializeField, Range(0.1f, 10f), Tooltip("The maximum duration the character can float in mid air.")] private float _duration = 1f;

        /// <summary>
        /// The cooldown until the character can levitate again.
        /// </summary>
        public float Cooldown => _cooldown;
        [SerializeField, Range(0.1f, 10f), Tooltip("The cooldown until the character can levitate again.")] private float _cooldown = 1f;
    }
}