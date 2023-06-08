using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

using Studio28.Audio.SFX;

namespace Studio28.Audio
{
    public class AudioController : MonoBehaviour
    {
        private static AudioController s_instance;

        public static AudioController Instance
        {
            get
            {
                if (s_instance == null)
                {
                    GameObject tempGameObject = new GameObject("SFX Controller");
                    s_instance = tempGameObject.AddComponent<AudioController>();
                }
                return s_instance;
            }
            private set
            {
                if (s_instance == null)
                    s_instance = value;
                else
                    Debug.LogError("You can have only one instance of an SFX Controller!");
            }
        }

        private void Awake()
        {
            Instance = this;
            _sfxSources = new(100);
        }

        // ##### SOURCE REGISTRATION ##### \\

        private List<SFXSourceController> _sfxSources;
        private void RegisterSoundEffect(SFXSourceController source_controller)
        {
            _sfxSources.Add(source_controller);
            source_controller.OnSoundEffectEnded += EndRegisteredSoundEffect;
        }
        private void EndRegisteredSoundEffect(SFXSourceController source_controller)
        {
            source_controller.OnSoundEffectEnded -= EndRegisteredSoundEffect;
            _sfxSources.Remove(source_controller);
            Destroy(source_controller.gameObject);
        }

        // ##### PLAY SOUNDS ##### \\

        public Controller PlayLoopSound<Controller>(SoundEffectInfo info, SFXSourceControllerArgs args) where Controller : SFXSourceController
        {
            Controller controller = SFXSourceController.CreateSFX<Controller>(info, args, true);

            RegisterSoundEffect(controller);

            return controller;
        }
        public DefaultSFXSourceController PlayLoopSound(SoundEffectInfo info, GameObject caller)
        {
            SFXSourceControllerArgs args = new(caller);

            DefaultSFXSourceController controller = SFXSourceController.CreateSFX<DefaultSFXSourceController>(info, args, true);

            RegisterSoundEffect(controller);

            return controller;
        }

        public Controller PlaySound<Controller>(SoundEffectInfo info, SFXSourceControllerArgs args) where Controller : SFXSourceController
        {
            Controller controller = SFXSourceController.CreateSFX<Controller>(info, args, false);

            RegisterSoundEffect(controller);

            return controller;
        }
        public DefaultSFXSourceController PlaySound(SoundEffectInfo info, GameObject caller)
        {
            SFXSourceControllerArgs args = new(caller);

            DefaultSFXSourceController controller = SFXSourceController.CreateSFX<DefaultSFXSourceController>(info, args, false);

            RegisterSoundEffect(controller);

            return controller;
        }
    }
}