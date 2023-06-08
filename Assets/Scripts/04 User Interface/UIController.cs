using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;

using JumpMaster.Core;

namespace JumpMaster.UI
{
    public delegate void UIMenusEventHandler();
    public enum UILevelStatus { PAUSED, PLAYING, ENDED }

    public class UIController : LevelController
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

            LevelManager.OnPause += () => { ActivateRegisteredMenus(UILevelStatus.PAUSED); };

            LevelManager.OnResume += () => { ActivateRegisteredMenus(UILevelStatus.PLAYING); };

            LevelManager.OnEndLevel += () => { ActivateRegisteredMenus(UILevelStatus.ENDED); };

            LevelManager.OnRestart += () => { ActivateRegisteredMenus(UILevelStatus.PLAYING); };

            LevelManager.OnLoad += () => { ActivateRegisteredMenus(UILevelStatus.PLAYING); };
        }

        private void BindMenuButtons(UIMenu menu)
        {
            foreach (UIButton button in menu.GetComponents<UIButton>())
                button.BindClickEvent();
        }

        private void ActivateRegisteredMenus(UILevelStatus status)
        {
            foreach (UIMenu menu in UIMenu.Menus)
            {
                menu.gameObject.SetActive(false);
                if (menu.ActiveStatus.Contains(status))
                {
                    menu.gameObject.SetActive(true);
                    BindMenuButtons(menu);
                }
            }
        }
    }
}