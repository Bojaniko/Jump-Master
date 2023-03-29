using UnityEngine;

namespace JumpMaster.Movement
{
    [CreateAssetMenu(fileName = "Hang Control Data", menuName = "Game/Movement/Hang Data")]
    public class HangControlDataSO : MovementControlDataSO
    {
        [Header("Hanging")]
        [Range(0.5f, 10f)]
        public float MinDuration = 0.5f;
        [Range(0, 100)]
        public int StickDistanceScreen = 20;
        [Range(0f, 2f)]
        public float MinStickVelocity = 1f;
    }
}