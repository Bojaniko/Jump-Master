using UnityEngine;

namespace Studio28.Audio.SFX
{
    [CreateAssetMenu(fileName = "Sound Effect Info", menuName = "Game/Audio/SFX/Info/Default")]
    public class DefaultSoundEffectInfo : SoundEffectInfo
    {
        [Range(0f, 1f)] public float Volume = 1f;
        [Range(-3f, 3f)] public float Pitch = 0f;

        public override float GetPitch() => Pitch;
        public override float GetVolume() => Volume;
    }
}
