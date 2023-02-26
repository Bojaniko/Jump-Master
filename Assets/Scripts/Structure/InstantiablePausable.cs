using UnityEngine;

namespace JumpMaster.Structure
{
    public abstract class InstantiablePausable : MonoBehaviour
    {
        protected abstract void Initialize();
        protected abstract void Pause();
        protected abstract void Unpause();
        protected abstract void PlayerDeath();
        protected abstract void Restart();

        public bool Initialized { get; protected set; }
    }
}