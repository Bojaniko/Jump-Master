using UnityEngine;

namespace JumpMaster.Movement
{
    [CreateAssetMenu(fileName = "Movement Controller Data", menuName = "Game/Movement/Controller Data")]
    public class MovementControllerDataSO : ScriptableObject
    {
        public JumpControlDataSO JumpControlData;

        public DashControlDataSO DashControlData;

        public StillControlDataSO StillControlData;

        public FallControlDataSO FallControlData;

        public FloatControlDataSO FloatControlData;

        public HangControlDataSO HangControlData;

        public ChargedJumpControlDataSO ChargedJumpControlData;
    }
}