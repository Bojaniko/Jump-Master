using UnityEngine;

namespace JumpMaster.Movement {
    [CreateAssetMenu(fileName = "Float Control Data", menuName = "Game/Movement/Float Data")]
    public class FloatControlDataSO : MovementControlDataSO
    {
        /// <summary>
        /// The slight force the character will face while floating.
        /// </summary>
        public float Force => _force;
        [SerializeField, Range(0f, 5f), Tooltip("The slight force the character will face while floating.")] private float _force;

        /// <summary>
        /// The maximum duration the character can float in mid air.
        /// </summary>
        public float Duration => _duration;
        [SerializeField, Range(0.1f, 10f), Tooltip("The maximum duration the character can float in mid air.")] private float _duration = 1f;
    }
}