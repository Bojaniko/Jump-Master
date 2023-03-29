using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;

using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    public delegate void UIMenusEventHandler();
    public enum UILevelStatus { PAUSED, PLAYING, PLAYER_DEATH, RESTARTING, LOADED_LEVEL }

    public class UIController : LevelControllerInitializablePausable
    {
        private static UIController s_instance;

        public static UIController Instance
        {
            get
            {
                return s_instance;
            }
            private set
            {
                if (s_instance == null)
                    s_instance = value;
                else
                    Debug.LogError("There can only be one UI Controller in the scene!");
            }
        }

        protected override void Initialize()
        {
            if (s_instance != null)
                return;
            s_instance = this;
        }

        private void ActivateRegisteredButtons(UIMenu menu)
        {
            foreach (UIButton button in menu.GetComponents<UIButton>())
                button.RegisterClickEvent();
        }

        private void ActivateRegisteredMenus(UILevelStatus status)
        {
            foreach (UIMenu menu in UIMenu.Menus)
            {
                menu.gameObject.SetActive(false);
                if (menu.ActiveStatus.Contains(status))
                {
                    menu.gameObject.SetActive(true);
                    ActivateRegisteredButtons(menu);
                }
            }
        }

        protected override void Pause()
        {
            ActivateRegisteredMenus(UILevelStatus.PAUSED);
        }

        protected override void PlayerDeath()
        {
            ActivateRegisteredMenus(UILevelStatus.PLAYER_DEATH);
        }

        protected override void Restart()
        {
            ActivateRegisteredMenus(UILevelStatus.RESTARTING);
        }

        protected override void Unpause()
        {
            ActivateRegisteredMenus(UILevelStatus.PLAYING);
        }

        protected override void LevelLoaded()
        {
            ActivateRegisteredMenus(UILevelStatus.LOADED_LEVEL);
        }
    }
}