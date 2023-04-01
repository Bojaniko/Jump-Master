using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    public class PauseButton : UIButton
    {
        protected override string ButtonName { get { return "Pause"; } }

        protected override void DetectTap()
        {
            if (LevelController.Paused)
                return;
            LevelController.Pause();
        }
    }
}