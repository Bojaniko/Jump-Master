using UnityEngine;

using Studio28.Attributes;

namespace Studio28.Audio.SFX
{
    [CreateAssetMenu(fileName = "Random SFX Info", menuName = "Game/Audio/SFX/Info/Randomized")]
    public class RandomizedSoundEffectInfo : SoundEffectInfo
    {
        [MinMax(0f, 1f)]
        public Vector2 VolumeRange = new Vector2(0.6f, 0.8f);

        [MinMax(-3f, 3f), Tooltip("Default value is 1")] public Vector2 PitchRange = new Vector2(0.8f, 1.2f);

        public override float GetPitch() => Random.Range(PitchRange.x, PitchRange.y);
        public override float GetVolume() => Random.Range(VolumeRange.x, VolumeRange.y);
    }
}
