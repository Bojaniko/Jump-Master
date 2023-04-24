using UnityEngine;

namespace JumpMaster.Movement
{
    public class MovementControlArgs
    {
        public readonly float StartTime;

        public readonly Vector3 StartPosition;

        public readonly float Strength;

        public MovementControlArgs(MovementController controller, float strength_percentage)
        {
            StartTime = Time.time;

            StartPosition = controller.ControlledRigidbody.transform.position;

            Strength = Mathf.Clamp(strength_percentage, 0.0f, 1.0f);
        }

        public MovementControlArgs(MovementController controller)
        {
            StartTime = Time.time;

            StartPosition = controller.ControlledRigidbody.transform.position;

            Strength = 1f;
        }

        public MovementControlArgs(MovementControlArgs args)
        {
            StartTime = args.StartTime;

            StartPosition = args.StartPosition;

            Strength = args.Strength;
        }
    }
}