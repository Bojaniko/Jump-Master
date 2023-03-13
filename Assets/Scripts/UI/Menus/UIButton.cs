using UnityEngine;
using UnityEngine.UIElements;

using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    [RequireComponent(typeof(UIMenu))]
    public abstract class UIButton : MonoBehaviour
    {
        protected abstract void DetectTap();
        protected abstract string ButtonName { get; }

        public void RegisterClickEvent()
        {
            if (GetComponent<UIDocument>() == null)
                return;
            if (GetComponent<UIDocument>().visualTreeAsset == null)
                return;
            GetComponent<UIDocument>().rootVisualElement.Q<Button>(ButtonName).clicked += DetectTap;
        }
    }
}