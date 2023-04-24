using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    public class PauseButton : UIButton
    {
        protected override string ButtonName { get { return "pause"; } }

        protected override void DetectTap()
        {
            LevelController.Pause();
        }
    }
}