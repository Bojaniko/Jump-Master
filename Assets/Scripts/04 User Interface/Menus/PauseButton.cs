using JumpMaster.Core;

namespace JumpMaster.UI
{
    public class PauseButton : UIButton
    {
        protected override string ButtonName { get { return "pause"; } }

        protected override void DetectTap()
        {
            LevelManager.Pause();
        }
    }
}