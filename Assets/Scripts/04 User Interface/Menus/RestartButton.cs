using JumpMaster.Core;

namespace JumpMaster.UI
{
    public class RestartButton : UIButton
    {
        protected override string ButtonName { get { return "restart"; } }

        protected override void DetectTap()
        {
            if (gameObject.name.Equals("PAUSE_MENU"))
            {
                if (!LevelManager.Paused)
                    return;
                if (!LevelManager.Loaded)
                    return;
                if (!LevelManager.Started)
                    return;
            }
            LevelManager.Restart();
        }
    }
}