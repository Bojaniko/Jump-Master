using UnityEngine;

using JumpMaster.Structure;

namespace JumpMaster.LevelControllers
{
    public abstract class LevelControllerBase : InitializablePausable
    {
        protected abstract override void Initialize();
        protected abstract override void Pause();
        protected abstract override void Unpause();
        protected abstract override void PlayerDeath();
        protected abstract override void Restart();

        private void Awake()
        {
            if (Initialized)
                return;

            Initialize();

            LevelController.Instance.OnLevelPaused += Pause;
            LevelController.Instance.OnLevelStarted += Unpause;

            Initialized = true;
        }
    }
}