using UnityEngine;

namespace JumpMaster.Movement
{
    [CreateAssetMenu(fileName = "Dash Control Data", menuName = "Game/Movement/Dash Data")]
    public class DashControlDataSO : MovementControlDataSO
    {
        [Header("Dash")]

        [SerializeField, Range(1f, 50f), Tooltip("The dashing force.")] private float _force = 15f;
        public float Force => _force;

        /// <summary>
        /// The distance the player will dash.
        /// </summary>
        public float MaxDistance => _maxDistance;
        [SerializeField, Range(0f, 10f), Tooltip("The distance the player will dash.")]
        private float _maxDistance = 6f;

        /// <summary>
        /// The slight force the player with get vertically when dashing.
        /// </summary>
        public float VerticalForce => _verticalForce;
        [SerializeField, Range(0f, 5f), Tooltip("The slight force the player with get vertically when dashing.")]
        private float _verticalForce = 0.5f;

        // Curves

        /// <summary>
        /// The curve repesenting the applied force of gravity over the duration of the distance dashed.
        /// </summary>
        public AnimationCurve GravityFalloff => _gravityFallof;
        [SerializeField, Tooltip("The curve repesenting the applied force of gravity over the distance dashed.")] private AnimationCurve _gravityFallof;

        /// <summary>
        /// The curve representing the applied dash force over the distance dashed.
        /// </summary>
        public AnimationCurve HorizontalVelocityFalloff => _horizontalVelocityFallof;
        [SerializeField, Tooltip("The curve representing the applied dash force over the distance dashed.")] AnimationCurve _horizontalVelocityFallof;

        [Header("Chain")]

        [SerializeField, Range(1, 10), Tooltip("The amount of times the player can chain a dash.")] private int _maxChain = 4;
        /// <summary>
        /// The amount of times the player can chain a dash.
        /// </summary>
        public int MaxChain => _maxChain;

        /// <summary>
        /// The cooldown duration for reseting a chain.
        /// </summary>
        public float ChainPenaltyDuration => _chainPenaltyDuration;
        [SerializeField, Range(0.1f, 10f), Tooltip("The cooldown duration for reseting a chain.")] private float _chainPenaltyDuration = 2f;

        /// <summary>
        /// The minimum distance the player needs to travel in order to chain with another dash.
        /// </summary>
        public float MinChainDistance => _minChainDistance;
        [SerializeField, Range(0.1f, 1f), Tooltip("The minimum distance the player needs to travel in order to chain with another dash.")] private float _minChainDistance = 0.2f;

        /// <summary>
        /// The vertical velocity for chaining with certain controls like jump.
        /// </summary>
        public float CrossChainVerticalForce => _crossChainVerticalForce;
        [SerializeField, Range(0f, 20f), Tooltip("The vertical velocity for chaining with certain controls like jump.")] private float _crossChainVerticalForce = 6f;

        public float MaxCrossChainVerticalDistance => _maxCrossChainVerticalDistance;
        [SerializeField, Range(0f, 10f)] private float _maxCrossChainVerticalDistance = 1.5f;
    }
}