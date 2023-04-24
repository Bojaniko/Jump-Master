using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    public class RestartButton : UIButton
    {
        protected override string ButtonName { get { return "restart"; } }

        protected override void DetectTap()
        {
            if (gameObject.name.Equals("PAUSE_MENU"))
            {
                if (!LevelController.Paused)
                    return;
                if (!LevelController.Loaded)
                    return;
                if (!LevelController.Started)
                    return;
            }
            LevelController.Restart();
        }
    }
}