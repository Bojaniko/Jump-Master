using UnityEngine;

namespace JumpMaster.Structure
{
    public abstract class Pausable : MonoBehaviour
    {
        protected abstract void Pause();
        protected abstract void Unpause();
        protected abstract void PlayerDeath();
        protected abstract void Restart();
    }
}