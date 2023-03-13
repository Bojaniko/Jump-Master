using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    public class ResumeButton : UIButton
    {
        protected override string ButtonName { get { return "Resume"; } }

        public static event UIMenusEventHandler OnResume;

        protected override void DetectTap()
        {
            if (!LevelController.Paused)
                return;
            if (OnResume != null)
                OnResume();
        }
    }
}