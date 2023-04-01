using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

namespace JumpMaster.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class UIMenu : MonoBehaviour
    {
        public UILevelStatus[] ActiveStatus;

        public static UIMenu[] Menus
        {
            get
            {
                return RegisteredMenus.ToArray();
            }
        }

        private static List<UIMenu> RegisteredMenus;

        private void Awake()
        {
            if (RegisteredMenus == null)
                RegisteredMenus = new();
            RegisteredMenus.Add(this);
        }
    }
}