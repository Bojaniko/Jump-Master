using UnityEngine;

namespace Studio28.Audio.SFX
{
    public class SFXSourceControllerArgs
    {
        public readonly GameObject Caller;

        public SFXSourceControllerArgs(GameObject caller)
        {
            Caller = caller;
        }
    }
}