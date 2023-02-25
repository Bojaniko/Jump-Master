using System.Collections.Generic;

using UnityEngine;

namespace Studio28.SFX
{
    public class SFXController : MonoBehaviour
    {
        private static SFXController s_instance;

        public static SFXController Instance
        {
            get
            {
                if (s_instance == null)
                {
                    GameObject tempGameObject = new GameObject("SFX Controller");
                    return tempGameObject.AddComponent<SFXController>();
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

            _sfxSources = new();
        }

        private List<SFXSourceController> _sfxSources;

        public Controller PlayLoopSound<Controller>(SoundEffectInfo info, SFXSourceControllerArgs args) where Controller : SFXSourceController
        {
            Controller controller = SFXSourceController.CreateSFX<Controller>(info, args, true);

            _sfxSources.Add(controller);

            return controller;
        }

        public Controller PlaySound<Controller>(SoundEffectInfo info, SFXSourceControllerArgs args) where Controller : SFXSourceController
        {
            Controller controller = SFXSourceController.CreateSFX<Controller>(info, args, false);

            _sfxSources.Add(controller);

            return controller;
        }

        public DefaultSFXSourceController PlayLoopSound(SoundEffectInfo info, GameObject caller)
        {
            Default_SFX_SC_Args args = new(caller);

            DefaultSFXSourceController controller = SFXSourceController.CreateSFX<DefaultSFXSourceController>(info, args, true);

            _sfxSources.Add(controller);

            return controller;
        }

        public DefaultSFXSourceController PlaySound(SoundEffectInfo info, GameObject caller)
        {
            Default_SFX_SC_Args args = new(caller);

            DefaultSFXSourceController controller = SFXSourceController.CreateSFX<DefaultSFXSourceController>(info, args, false);

            _sfxSources.Add(controller);

            return controller;
        }

        public void RemoveSource(SFXSourceController source)
        {
            if (_sfxSources.Contains(source))
                _sfxSources.Remove(source);
        }
    }
}