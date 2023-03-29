using UnityEngine;

namespace JumpMaster.Structure
{
    public abstract class InitializablePausable : MonoBehaviour//, IInitializable
    {
        protected abstract void Initialize();
        protected abstract void Pause();
        protected abstract void Unpause();
        protected abstract void PlayerDeath();
        protected abstract void Restart();

        public bool Initialized { get; protected set; }

        //public event InitializationEventHandler OnInitialization;
    }
}