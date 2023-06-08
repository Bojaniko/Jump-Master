using UnityEngine;

using Studio28.Attributes;

namespace Studio28.Audio.SFX
{
    [CreateAssetMenu(fileName = "Min Max SFX Info", menuName = "Game/Audio/SFX/Info/Min-Max")]
    public class MinMaxSoundEffectInfo : SoundEffectInfo
    {
        [Range(0f, 1f)] public float Volume;
        [Range(-3, 3f)] public float Pitch;

        [MinMax(0f, 1f)] public Vector2 VolumeRange;
        [MinMax(-3, 3f)] public Vector2 PitchRange;

        public override float GetPitch() => Pitch;
        public override float GetVolume() => Volume;
    }
}