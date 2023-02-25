using UnityEngine;

using UnityEngine.Audio;

using GD.MinMaxSlider;

namespace Studio28.SFX
{
    [CreateAssetMenu(fileName = "Sound Effect Info", menuName = "Game/SFX")]
    public class SoundEffectInfo : ScriptableObject
    {

        public AudioClip Clip;

        public AudioMixerGroup MixerGroup;

        [MinMaxSlider(0f, 1f)]
        public Vector2 VolumeRange = new Vector2(0.6f, 0.8f);

        [MinMaxSlider(-3f, 3f)]
        public Vector2 PitchRange = new Vector2(0.8f, 1.2f);

        public float GetVolume()
        {
            return Random.Range(VolumeRange.x, VolumeRange.y);
        }

        public float GetPitch()
        {
            return Random.Range(PitchRange.x, PitchRange.y);
        }
    }
}