using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    public class PauseButton : UIButton
    {
        protected override string ButtonName { get { return "Pause"; } }

        public static event UIMenusEventHandler OnPause;

        protected override void DetectTap()
        {
            if (LevelController.Paused)
                return;
            if (OnPause != null)
                OnPause();
        }
    }
}