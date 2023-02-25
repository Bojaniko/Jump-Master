using UnityEngine;

namespace Studio28.SFX
{
    public abstract class SFXSourceControllerArgs
    {
        public readonly GameObject Caller;

        protected SFXSourceControllerArgs(GameObject caller)
        {
            Caller = caller;
        }
    }
}