using UnityEngine;
using UnityEngine.UIElements;

using JumpMaster.Controls;

namespace JumpMaster.UI
{
    [RequireComponent(typeof(UIMenu))]
    public abstract class UIButton : MonoBehaviour
    {
        protected abstract void DetectTap();
        protected abstract string ButtonName { get; }

        public void BindClickEvent()
        {
            if (c_document == null)
                return;
            if (c_document.visualTreeAsset == null)
                return;
            c_document.rootVisualElement.Q<Button>(ButtonName).clicked += Tap;
        }

        private void Tap()
        {
            DetectTap();
            InputController.Instance.RegisterInputUI();
        }

        // ##### CACHE ##### \\

        private UIDocument c_document;

        private void Awake()
        {
            c_document = GetComponent<UIDocument>();
        }
    }
}