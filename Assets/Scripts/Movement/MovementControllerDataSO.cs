using UnityEngine;

namespace JumpMaster.Movement
{
    [CreateAssetMenu(fileName = "Movement Controller Data", menuName = "Game/Movement/Controller Data")]
    public class MovementControllerDataSO : ScriptableObject
    {
        [Range(0f, 20f)] public float Z_Position = 3f;

        [Header("Controls data")]

        public JumpControlDataSO JumpControlData;

        public DashControlDataSO DashControlData;

        public StillControlDataSO StillControlData;

        public FallControlDataSO FallControlData;

        public FloatControlDataSO FloatControlData;

        public HangControlDataSO HangControlData;

        public ChargedJumpControlDataSO ChargedJumpControlData;

        public BounceControlDataSO BounceControlData;
    }
}