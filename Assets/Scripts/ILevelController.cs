using System;

namespace JumpMaster.LevelControllers
{
    public delegate void LevelControllerEventHandler(ILevelController sender, LevelControllerInitializationArgs e);

    public class LevelControllerInitializationArgs : EventArgs
    {

    }

    public interface ILevelController
    {
        public bool ControllerInitialized { get; }
    }
}