using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    public class ResumeButton : UIButton
    {
        protected override string ButtonName { get { return "Resume"; } }

        protected override void DetectTap()
        {
            if (!LevelController.Paused)
                return;
            LevelController.Resume();
        }
    }
}