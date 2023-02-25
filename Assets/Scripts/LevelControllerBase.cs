using UnityEngine;

namespace JumpMaster.LevelControllers
{
    public abstract class LevelControllerBase : MonoBehaviour
    {
        protected abstract void Initialize();
        protected abstract void Pause();
        protected abstract void Unpause();
        protected abstract void Death();
        protected abstract void Restart();

        private bool _initialized = false;

        protected bool _paused { get; private set; }

        private void Awake()
        {
            if (_initialized)
                return;

            _paused = true;

            Initialize();

            LevelController.Instance.OnLevelPaused += () => { _paused = true; Pause(); };
            LevelController.Instance.OnLevelStarted += () => { _paused = false; Unpause(); };

            _initialized = true;
        }
    }
}