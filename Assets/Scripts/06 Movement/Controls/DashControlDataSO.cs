using UnityEngine;

namespace JumpMaster.Movement
{
    [CreateAssetMenu(fileName = "Dash Control Data", menuName = "Game/Movement/Dash Data")]
    public class DashControlDataSO : MovementControlDataSO
    {
        [Header("Dash")]
        [Range(1f, 50f)]
        public float Force = 10f;
        [Range(0f, 10f)]
        public float MinDistance = 2.5f;
        [Range(0f, 10f)]
        public float MaxDistance = 6f;
        [Range(0f, 1f)]
        public float TransitionDistancePercentage = 0.95f;

        public AnimationCurve GravityFalloff;
        public AnimationCurve HorizontalVelocityFalloff;

        [Header("Chain")]
        [Range(1, 10)]
        public int MaxChain = 2;
        [Range(0.5f, 10f)]
        public float ChainPenaltyDuration = 2f;
        [Range(0.1f, 1f)]
        public float MinChainDistance = 0.5f;
        [Range(0f, 20f)]
        public float CrossChainVerticalForce = 2f;
        [Range(0f, 10f)]
        public float MaxCrossChainVerticalDistance = 2f;
    }
}