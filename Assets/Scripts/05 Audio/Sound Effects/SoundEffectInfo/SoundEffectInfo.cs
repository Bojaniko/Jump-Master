using UnityEngine;

using UnityEngine.Audio;

namespace Studio28.Audio.SFX
{
    /// <summary>
    /// Sound effect data such as the audio clip, volume, pitch...
    /// </summary>
    public abstract class SoundEffectInfo : ScriptableObject
    {

        public AudioClip Clip;

        public AudioMixerGroup MixerGroup;

        public abstract float GetVolume();
        public abstract float GetPitch();
    }
}