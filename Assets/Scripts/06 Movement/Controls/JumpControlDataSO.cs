using UnityEngine;

namespace JumpMaster.Movement
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "<Pending>")]
    [CreateAssetMenu(fileName = "Jump Control Data", menuName = "Game/Movement/Jump Data")]
    public class JumpControlDataSO : MovementControlDataSO
    {
        [Header("Jump")]
        [SerializeField, Range(0.5f, 10f)] private float _height = 5f;
        public float Height => _height;

        public float Force => _force;
        [SerializeField, Range(2f, 50f)] private float _force = 20f;

        public AnimationCurve VelocityFalloff => _velocityFallof;
        [SerializeField] private AnimationCurve _velocityFallof;

        [Header("Chain")]
        [SerializeField, Range(1, 50)] private int _maxChain = 10;
        public int MaxChain => _maxChain;

        /// <summary>
        /// The minimum jump distance percentage for chaining a jump.
        /// </summary>
        public float MinChainHeight => _minChainHeight;
        [SerializeField, Range(0.1f, 1f), Tooltip("The minimum jump distance percentage for chaining a jump.")]
        private float _minChainHeight = 0.75f;

        /// <summary>
        /// The cooldown duration after chaining a jump.
        /// </summary>
        public float ChainPenaltyDuration => _chainPenaltyDuration;
        [SerializeField, Range(0.5f, 10f), Tooltip("The cooldown duration after chaining a jump.")]
        private float _chainPenaltyDuration = 2.5f;

        /// <summary>
        /// The horizontal force applied if the jump is chained from a horizontal based control.
        /// </summary>
        public float CrossChainHorizontalForce => _crossChainHorizontalForce;
        [SerializeField, Range(0f, 20f), Tooltip("The horizontal force applied if the jump is chained from a horizontal based control.")]
        private float _crossChainHorizontalForce = 10f;
    }
}