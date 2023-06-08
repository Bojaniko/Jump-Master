using JumpMaster.Core;

namespace JumpMaster.UI
{
    public class ResumeButton : UIButton
    {
        protected override string ButtonName { get { return "Resume"; } }

        protected override void DetectTap()
        {
            if (!LevelManager.Paused)
                return;
            LevelManager.Resume();
        }
    }
}