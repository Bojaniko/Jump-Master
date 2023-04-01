using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;

using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    public delegate void UIMenusEventHandler();
    public enum UILevelStatus { PAUSED, PLAYING, ENDED }

    public class UIController : LevelControllerInitializable
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

            LevelController.OnPause += () => { ActivateRegisteredMenus(UILevelStatus.PAUSED); };

            LevelController.OnResume += () => { ActivateRegisteredMenus(UILevelStatus.PLAYING); };

            LevelController.OnEndLevel += () => { ActivateRegisteredMenus(UILevelStatus.ENDED); };

            LevelController.OnRestart += () => { ActivateRegisteredMenus(UILevelStatus.PLAYING); };

            LevelController.OnLoad += () => { ActivateRegisteredMenus(UILevelStatus.PLAYING); };
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
    }
}