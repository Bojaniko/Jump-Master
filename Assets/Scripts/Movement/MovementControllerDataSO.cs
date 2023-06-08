using System.Reflection;

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

        public LevitationControlDataSO LevitationControlData;

        public BounceControlDataSO BounceControlData;

        public MovementControlDataSO GetControlDataForControlType(System.Type control_type)
        {
            if (control_type.IsSubclassOf(typeof(IMovementControl)))
                return null;

            System.Type controlDataType = typeof(Object);
            foreach (FieldInfo fi in GetType().GetFields())
            {
                foreach (System.Type gs in control_type.BaseType.GetGenericArguments())
                {
                    if (gs.IsSubclassOf(typeof(MovementControlDataSO)))
                    {
                        controlDataType = gs;
                        break;
                    }
                }
                if (controlDataType.Equals(typeof(Object)))
                    return null;

                if (fi.FieldType.Equals(controlDataType))
                    return (MovementControlDataSO)fi.GetValue(this);
            }
            return null;
        }
    }
}