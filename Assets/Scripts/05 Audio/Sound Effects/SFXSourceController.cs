using UnityEngine;

namespace Studio28.Audio.SFX
{
    public delegate void SoundEffectEventHandler(SFXSourceController source_controller);

    public abstract class SFXSourceController : MonoBehaviour
    {
        public AudioSource Source { get; private set; }

        public SoundEffectInfo Info { get; private set; }


        /// <summary>
        /// This method is used by the SFX Controller to instantiate sound effect sources.
        /// Don't call this method.
        /// </summary>
        public static SourceController CreateSFX<SourceController>(SoundEffectInfo info, SFXSourceControllerArgs args, bool loop) where SourceController : SFXSourceController
        {
            if (info.Clip == null)
                return null;

            GameObject sfxGameObject = new GameObject(info.Clip.name);

            sfxGameObject.SetActive(false);

            SourceController controller = sfxGameObject.AddComponent<SourceController>();

            controller.Info = info;

            controller.Source = sfxGameObject.AddComponent<AudioSource>();

            controller.Source.clip = info.Clip;
            controller.Source.volume = info.GetVolume();
            controller.Source.pitch = info.GetPitch();
            controller.Source.outputAudioMixerGroup = info.MixerGroup;
            controller.Source.playOnAwake = false;

            if (loop)
                controller.Source.loop = true;

            controller.InitializeSourceController(args);

            sfxGameObject.SetActive(true);

            return controller;
        }

        protected abstract void InitializeSourceController(SFXSourceControllerArgs args);
        public abstract event SoundEffectEventHandler OnSoundEffectEnded;
    }
}