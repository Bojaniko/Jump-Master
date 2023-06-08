using UnityEngine;

namespace JumpMaster.Controls
{
    [CreateAssetMenu(fileName = "Hold Input Data", menuName = "Game/Input/Processors/Hold")]
    public class HoldProcessorDataSO : InputProcessorDataSO
    {
        /// <summary>
        /// The minimum time (s) needed to touch the screen for the hold processor to be performed.
        /// </summary>
        public float MinDuration => _minDuration;
        [SerializeField, Range(0f, 1f), Tooltip("The minimum time (s) needed to touch the screen for the hold processor to be performed.")] private float _minDuration = 0.3f;
    }
}