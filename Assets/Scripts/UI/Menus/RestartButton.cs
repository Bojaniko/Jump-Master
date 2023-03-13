using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    public class RestartButton : UIButton
    {
        protected override string ButtonName { get { return "Restart"; } }

        public static event UIMenusEventHandler OnRestart;

        protected override void DetectTap()
        {
            if (!LevelController.Paused)
                return;
            if (!LevelController.Loaded)
                return;
            if (!LevelController.Started)
                return;
            if (OnRestart != null)
                OnRestart();
        }
    }
}