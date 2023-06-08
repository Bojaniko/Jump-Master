using UnityEngine;

namespace JumpMaster.Controls
{
    [CreateAssetMenu(fileName = "Tap Input Data", menuName = "Game/Input/Processors/Tap")]
    public class TapProcessorDataSO : InputProcessorDataSO
    {
        /// <summary>
        /// The maximum touch duration for it to be a tap.
        /// </summary>
        public float MaxDuration => _maxDuration;
        [SerializeField, Range(0f, 1f), Tooltip("The maximum touch duration (s) for it to be a tap.")] private float _maxDuration = 0.15f;
    }
}