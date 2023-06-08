using UnityEngine;

namespace JumpMaster.Controls
{
    [CreateAssetMenu(fileName = "Swipe Input Data", menuName = "Game/Input/Processors/Swipe")]
    public class SwipeProcessorDataSO : InputProcessorDataSO
    {
        /// <summary>
        /// The maximum duration for detecting a swipe.
        /// </summary>
        public float MaxTime => _maxTime;
        [SerializeField, Range(0f, 1f), Tooltip("The maximum duration for detecting a swipe.")] private float _maxTime = 0.2f;

        /// <summary>
        /// The minimum swipe distance.
        /// </summary>
        public float MinDistance => _minDistance;
        [SerializeField, Range(10f, 500f), Tooltip("The minimum swipe distance.")] private float _minDistance = 140f;
    }
}