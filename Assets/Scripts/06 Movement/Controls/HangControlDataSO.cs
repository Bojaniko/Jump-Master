using UnityEngine;

namespace JumpMaster.Movement
{
    [CreateAssetMenu(fileName = "Hang Control Data", menuName = "Game/Movement/Hang Data")]
    public class HangControlDataSO : MovementControlDataSO
    {
        [Header("Hanging")]
        [Range(0f, 5f)]
        public float MinDuration = 0.2f;
        [Range(0, 100)]
        public int StickDistanceScreen = 20;
        [Range(0f, 10f)]
        public float MinStickVelocity = 1f;
    }
}