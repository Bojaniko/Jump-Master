using UnityEngine;

namespace JumpMaster.Movement
{
    [CreateAssetMenu(fileName = "Fall Control Data", menuName = "Game/Movement/Fall Data")]
    public class FallControlDataSO : MovementControlDataSO
    {
        [Range(0f, 50f)]
        public float Drag = 20f;
    }
}