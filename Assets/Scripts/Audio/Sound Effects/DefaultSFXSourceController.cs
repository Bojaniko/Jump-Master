using UnityEngine;

namespace Studio28.Audio.SFX
{
    public class DefaultSFXSourceController : SFXSourceController
    {
        protected override void InitializeSourceController(SFXSourceControllerArgs args) { }

        private void Awake()
        {
            Source.Play();
        }

        private void Update()
        {
            if (!Source.isPlaying)
                OnSoundEffectEnded?.Invoke(this);
        }

        public override event SoundEffectEventHandler OnSoundEffectEnded;
    }
}