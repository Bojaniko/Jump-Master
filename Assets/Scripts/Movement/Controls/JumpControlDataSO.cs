using UnityEngine;

namespace JumpMaster.Movement
{
    [CreateAssetMenu(fileName = "Jump Control Data", menuName = "Game/Movement/Jump Data")]
    public class JumpControlDataSO : MovementControlDataSO
    {
        [Header("Normal Jump")]
        [Range(0.5f, 10f)]
        public float Height;
        [Range(2f, 50f)]
        public float Force;
        [Range(0.1f, 2f)]
        public float EndForce = 0.5f;

        [Header("Chain")]
        [Range(1, 10)]
        public int MaxChain = 3;
        [Range(0.5f, 10f)]
        public float ChainPenaltyDuration = 1.5f;
        [Range(0.1f, 1f)]
        public float MinChainHeight = 0.5f;
        [Range(0.1f, 1f)]
        public float MaxCrossChainDistance = 0.9f;
        [Range(0f, 1f)]
        public float CrossChainVelocityPercentage = 0.5f;
    }
}