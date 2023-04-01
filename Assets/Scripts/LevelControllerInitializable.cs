using JumpMaster.Structure;

namespace JumpMaster.LevelControllers
{
    public abstract class LevelControllerInitializable : Initializable
    {
        public bool ControllerInitialized { get { return Initialized; } }

        protected abstract override void Initialize();

        private void Awake()
        {
            if (Initialized)
                return;

            Initialize();

            Initialized = true;
        }
    }
}