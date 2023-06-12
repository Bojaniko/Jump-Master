using UnityEngine;

namespace JumpMaster.Movement
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "<Pending>")]
    [CreateAssetMenu(fileName = "Jump Control Data", menuName = "Game/Movement/Jump Data")]
    public class JumpControlDataSO : MovementControlDataSO
    {
        [Header("Jump")]
        [Range(0.5f, 10f)]
        public float Height = 6f;
        [Range(2f, 50f)]
        public float Force = 20f;

        public AnimationCurve VelocityFalloff;

        [Header("Chain")]
        [Range(1, 50)]
        public int MaxChain = 3;
        [Range(0.1f, 1f)]
        public float MinChainHeight = 0.5f;
        [Range(0.5f, 10f)]
        public float ChainPenaltyDuration = 1.5f;
        [Range(0f, 20f)]
        public float CrossChainHorizontalForce = 10f;
    }
}