using UnityEngine;

using JumpMaster.Structure;

namespace JumpMaster.LevelControllers
{
    public abstract class LevelControllerInitializablePausable : InitializablePausable, ILevelController
    {
        public bool ControllerInitialized { get { return Initialized; } }

        protected abstract override void Initialize();
        protected abstract override void Pause();
        protected abstract override void Unpause();
        protected abstract override void PlayerDeath();
        protected abstract override void Restart();
        protected abstract void LevelLoaded();

        private void Awake()
        {
            if (Initialized)
                return;

            Initialize();

            LevelController.Instance.OnLevelPaused += Pause;
            LevelController.Instance.OnLevelLoaded += LevelLoaded;
            LevelController.Instance.OnLevelReset += Restart;
            LevelController.Instance.OnLevelResume += Unpause;

            Initialized = true;
        }
    }
}