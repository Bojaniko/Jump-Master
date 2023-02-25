using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Studio28.SFX
{
    public abstract class SFXSourceController : MonoBehaviour
    {

        public AudioSource Source { get; private set; }

        public SoundEffectInfo Info { get; private set; }

        public static Controller CreateSFX<Controller>(SoundEffectInfo info, SFXSourceControllerArgs args, bool loop) where Controller : SFXSourceController
        {
            GameObject sfxGameObject = new GameObject(info.Clip.name);

            sfxGameObject.SetActive(false);

            Controller controller = sfxGameObject.AddComponent<Controller>();

            controller.Info = info;

            controller.Source = sfxGameObject.AddComponent<AudioSource>();

            controller.Source.clip = info.Clip;
            controller.Source.volume = info.GetVolume();
            controller.Source.pitch = info.GetPitch();
            controller.Source.outputAudioMixerGroup = info.MixerGroup;

            if (loop)
                controller.Source.loop = true;

            controller.InitializeSourceController(args);

            sfxGameObject.SetActive(true);

            return controller;
        }

        protected abstract void InitializeSourceController(SFXSourceControllerArgs args);

        private void OnDestroy()
        {
            SFXController.Instance.RemoveSource(this);
        }
    }
}