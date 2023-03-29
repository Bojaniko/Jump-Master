using UnityEngine;
using UnityEngine.UI;

using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    [RequireComponent(typeof(Image))]
    public class ChainIcon : MonoBehaviour
    {
        protected Vector2 _position;
        [SerializeField]
        protected Vector2 _offsetPosition;

        [SerializeField]
        protected float _iconSize;

        protected Image _image;

        protected RectTransform _rect;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _rect.sizeDelta = Vector2.one * _iconSize * 10f;

            _image = GetComponent<Image>();
            _image.fillAmount = 1f;
            _image.enabled = false;

            LevelController.Instance.OnLevelStarted += () => _image.enabled = true;
        }
    }
}