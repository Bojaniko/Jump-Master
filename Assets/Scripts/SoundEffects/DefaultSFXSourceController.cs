using UnityEngine;

namespace Studio28.SFX
{
    public class DefaultSFXSourceController : SFXSourceController
    {

        private Default_SFX_SC_Args _data;

        protected override void InitializeSourceController(SFXSourceControllerArgs args)
        {
            _data = (Default_SFX_SC_Args)args;
        }

        private void Awake()
        {
            Source.Play();
        }

        private void Update()
        {
            if (Source.isPlaying == false)
                Destroy(gameObject);
        }
    }

    public class Default_SFX_SC_Args : SFXSourceControllerArgs
    {

        public Default_SFX_SC_Args(GameObject caller) : base(caller)
        {

        }
    }
}