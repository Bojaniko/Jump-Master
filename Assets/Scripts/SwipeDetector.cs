using System.Collections.Generic;

using UnityEngine;

namespace JumpMaster.Controls {

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

        public delegate void SwipeDetectorEventHandler(SwipeDirection direction);

        public event SwipeDetectorEventHandler OnSwipeDetected;

        public float MaxTime = 1f;
        public float MinDistance = 0.2f;
        [Range(0f, 1f)]
        public float DirectionTreshold = 0.9f;

        private float _startTime;
        private float _endTime;

        private Vector2 _startPosition;
        private Vector2 _endPosition;

        public List<float> DetectedSwipes { get; private set; }

        private void SwipeStart(Vector2 position, float time)
        {
            _startPosition = position;
            _startTime = time;
        }

        private void SwipeEnd(Vector2 position, float time)
        {
            _endPosition = position;
            _endTime = time;

            if (DetectSwipe())
                DetectedSwipes.Add(_startTime);
        }

        private bool DetectSwipe()
        {
            if (Vector2.Distance(_startPosition, _endPosition) >= MinDistance)
            {
                if (_endTime - _startTime <= MaxTime)
                {
                    //Debug.Log("Swipe: distance - " + Vector2.Distance(_startPosition, _endPosition) + ", time - " + (_endTime - _startTime));
                    if (OnSwipeDetected != null)
                        OnSwipeDetected(GetDirection(_startPosition, _endPosition));

                    return true;
                }
            }
            return false;
        }

        private SwipeDirection GetDirection(Vector2 x, Vector2 y)
        {
            Vector2 difference = x - y;

            if (Vector2.Angle(Vector2.up, difference.normalized) / 180f >= DirectionTreshold) { return SwipeDirection.UP; }
            if (Vector2.Angle(Vector2.down, difference.normalized) / 180f >= DirectionTreshold) { return SwipeDirection.DOWN; }
            if (Vector2.Angle(Vector2.left, difference.normalized) / 180f >= DirectionTreshold) { return SwipeDirection.LEFT; }
            if (Vector2.Angle(Vector2.right, difference.normalized) / 180f >= DirectionTreshold) { return SwipeDirection.RIGHT; }

            return SwipeDirection.NONE;
        }
    }
}