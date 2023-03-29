using UnityEngine;

namespace JumpMaster.Movement
{
    public sealed class MovementControlArgs
    {
        public readonly float StartTime;

        public readonly Vector3 StartPosition;

        public readonly MovementDirection Direction;

        public readonly float Strength;

        public MovementControlArgs(Rigidbody controlled_rigidbody, MovementDirection direction, float strength_percentage)
        {
            StartTime = Time.time;

            StartPosition = controlled_rigidbody.transform.position;

            Direction = direction;

            Strength = Mathf.Clamp(strength_percentage, 0.0f, 1.0f);
        }

        public MovementControlArgs(Rigidbody controlled_rigidbody, MovementDirection direction)
        {
            StartTime = Time.time;

            StartPosition = controlled_rigidbody.transform.position;

            Direction = direction;

            Strength = 1f;
        }

        public MovementControlArgs(Rigidbody controlled_rigidbody)
        {
            StartTime = Time.time;

            StartPosition = controlled_rigidbody.transform.position;

            Direction = MovementDirection.Zero;

            Strength = 1f;
        }

        private MovementControlArgs(float start_time, Vector3 start_position, MovementDirection direction, float strength_percentage)
        {
            StartTime = start_time;

            StartPosition = start_position;

            Direction = direction;

            Strength = strength_percentage;
        }

        public static MovementControlArgs UpdateDirection(MovementControlArgs args, MovementDirection new_direction)
        {
            if (args == null)
                return null;
            return new(args.StartTime, args.StartPosition, new_direction, args.Strength);
        }
    }
}