using System.Collections.Generic;

using UnityEngine;

namespace JumpMaster.Controls
{
    public enum SwipeDirection { UP, DOWN, LEFT, RIGHT, NONE }

    public class SwipeDetector : MonoBehaviour
    {
        private static SwipeDetector _instance;
        public static SwipeDetector Instance
        {
            get
            {
                return _instance;
            }
            private set
            {
                if (_instance == null)
                    _instance = value;
                else
                    Debug.LogError("You can have only one instance of Swipe Detector!");
            }
        }

        private void Awake()
        {
            Instance = this;

            DetectedSwipes = new();

            InputController.Instance.OnTouchStart += SwipeStart;
            InputController.Instance.OnTouchEnd += SwipeEnd;
        }

        public float MaxTime = 1f;
        public float MinDistance = 0.2f;

        public bool IsSwipeDetected(float time)
        {
            foreach (Swipe swipe in DetectedSwipes)
            {
                if (swipe.StartTime == time)
                    return true;
            }
            return false;
        }

        // ##### INPUT ##### \\

        private float _startTime;
        private Vector2 _startPosition;
        private void SwipeStart(Vector2 position, float time)
        {
            _startPosition = position;
            _startTime = time;
        }

        private void SwipeEnd(Vector2 position, float time)
        {
            ProcessSwipe(_startTime, time, _startPosition, position);
        }

        // ##### SWIPE PROCESSING ##### \\

        public delegate void SwipeDetectorEventHandler(Swipe swipe);
        public event SwipeDetectorEventHandler OnSwipeDetected;

        public List<Swipe> DetectedSwipes { get; private set; }

        private void ProcessSwipe(float start_time, float end_time, Vector2 start_position, Vector2 end_position)
        {
            float duration = end_time - start_time;
            float distance = Vector2.Distance(start_position, end_position);

            //Debug.Log($"Processing swipe: {distance} - distance, {duration} - duration.");

            if (!IsSwipeValid(duration, distance))
                return;

            SwipeDirection direction = GetDirection(start_position, end_position);

            Swipe swipe = new(direction, distance, start_time);

            DetectedSwipes.Add(swipe);

            OnSwipeDetected?.Invoke(swipe);
        }

        private bool IsSwipeValid(float duration, float distance)
        {
            if (distance < MinDistance)
                return false;

            if (duration > MaxTime)
                return false;

            return true;
        }

        private SwipeDirection GetDirection(Vector2 x, Vector2 y)
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