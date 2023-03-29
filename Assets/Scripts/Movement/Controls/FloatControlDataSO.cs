using UnityEngine;

namespace JumpMaster.Movement {
    [CreateAssetMenu(fileName = "Float Control Data", menuName = "Game/Movement/Float Data")]
    public class FloatControlDataSO : MovementControlDataSO
    {
        [Header("Floating")]
        [Range(0.1f, 2f)]
        public float Force = 0.5f;
        [Range(0.1f, 2f)]
        public float Duration = 1f;
    }
}