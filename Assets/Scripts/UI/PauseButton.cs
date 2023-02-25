using UnityEngine;

using JumpMaster.LevelControllers;
using JumpMaster.Controls;

namespace JumpMaster.UI
{
    public class PauseButton : MonoBehaviour
    {
        public delegate void PauseButtonEventHandler();
        public static event PauseButtonEventHandler OnPause;
        public static event PauseButtonEventHandler OnResume;

        private RectTransform _rect;

        private Bounds _bounds;

        private void Start()
        {
            _rect = GetComponent<RectTransform>();

            Vector2 anchored_position = new();
            anchored_position.x = Screen.width + _rect.anchoredPosition.x;
            anchored_position.y = _rect.anchoredPosition.y;

            _bounds = new Bounds(anchored_position, _rect.sizeDelta);

            InputController.Instance.OnTapUI += DetectPauseTap;
        }

        private void DetectPauseTap(Vector2 position)
        {
            if (_bounds.Contains(position) == false)
                return;

            if (LevelController.Paused)
            {
                if (OnResume != null)
                    OnResume();
                return;
            }
            if (!LevelController.Paused)
            {
                if (OnPause != null)
                    OnPause();
                return;
            }
        }
    }
}