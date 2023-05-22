using UnityEngine;

using Studio28.Attributes;

namespace Studio28.Audio
{
    [CreateAssetMenu(fileName ="Audio Settings", menuName = "Game/Audio/Settings")]
    public class AudioSettingsSO : ScriptableObject
    {
        [Header("Sound Effects")]

        [Range(10f, 500f), Tooltip("Minimum time between any two sound effects."), InspectorName("Minimum Interval"), SerializeField] private float _minimumIntervalSoundEffect;
        public float MinimumIntervalSFX => _minimumIntervalSoundEffect;

        [Tooltip("Delay if true / don't play if false"), InspectorName("Delay Overlapping"), SerializeField] private bool _delayOverlappingSoundEffects;
        public bool DelayOverlappingSFX => _delayOverlappingSoundEffects;

        [MinMax(0f, 1f)] public Vector2 test_atr1;
    }
}