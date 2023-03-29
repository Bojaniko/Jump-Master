using UnityEngine;

namespace JumpMaster.Movement
{
    [CreateAssetMenu(fileName = "Charged Jump Control Data", menuName = "Game/Movement/Charged Jump Data")]
    public class ChargedJumpControlDataSO : MovementControlDataSO
    {
        [Header("Charged Jump")]
        [Range(0.5f, 10f)]
        public float ChargedHeight;
        [Range(2f, 50f)]
        public float ChargedForce;
        [Range(0.1f, 1f)]
        public float MinChargePercentage = 0.3f;
        [Range(0.5f, 2f)]
        public float MaxChargeDuration;
        [Range(0.1f, 2f)]
        public float EndForce = 0.5f;
        [Range(0.1f, 1f)]
        public float MinChainHeight = 0.5f;
    }
}
