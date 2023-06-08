using UnityEngine;

namespace JumpMaster.Controls
{
    internal static class SwipeInputProcessor
    {
        public static Swipe ProcessSwipe(in SwipeInput input, in float max_time, in float min_distance)
        {
            float duration = input.EndTime - input.StartTime;
            float distance = Vector2.Distance(input.StartPosition, input.EndPosition);

            if (!IsSwipeValid(duration, max_time, distance, min_distance))
                return Swipe.Invalid;

            SwipeDirection direction = GetDirection(input.StartPosition, input.EndPosition);
            Swipe swipe = new(direction, input.Delay);

            return swipe;

            //Debug.Log($"Processing swipe: {distance} - distance, {duration} - duration.");
            //Debug.Log($"Swipe delayed: {delayed}, direction is {direction}.");
        }

        private static bool IsSwipeValid(in float duration, in float max_time, in float distance, in float min_distance)
        {
            if (distance < min_distance)
                return false;
            if (duration > max_time)
                return false;
            return true;
        }

        private static SwipeDirection GetDirection(Vector2 x, Vector2 y)
        {
            Vector2 difference = x - y;
            if (Vector2.Angle(Vector2.up, difference.normalized) / 180f >= 0.75f) { return SwipeDirection.UP; }
            if (Vector2.Angle(Vector2.down, difference.normalized) / 180f >= 0.75f) { return SwipeDirection.DOWN; }
            if (Vector2.Angle(Vector2.left, difference.normalized) / 180f >= 0.75f) { return SwipeDirection.LEFT; }
            if (Vector2.Angle(Vector2.right, difference.normalized) / 180f >= 0.75f) { return SwipeDirection.RIGHT; }
            return SwipeDirection.NONE;
        }
    }
}