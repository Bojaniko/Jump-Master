using UnityEngine;
using UnityEngine.UI;

using JumpMaster.Core;
using JumpMaster.LevelTrackers;

namespace JumpMaster.CameraControls
{
    [RequireComponent(typeof(RawImage))]
    public class BackgroundParallax : MonoBehaviour
    {
        [SerializeField, Range(0f, 1000f)] private float _zPosition = 100f;
        [SerializeField, Range(1f, 10f)] private float _scale = 1f;
        [SerializeField] private float _endHeight = 1000f;
        [SerializeField] private Texture _texture;

        private Vector2 _parallaxPosition;

        private RawImage _backgroundImage;
        private RectTransform _rectTransform;

        private void Awake()
        {
            if (_texture == null)
                return;

            _backgroundImage = GetComponent<RawImage>();
            _backgroundImage.texture = _texture;

            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height) * _scale;
            _rectTransform.transform.position = new Vector3(0f, 0f, _zPosition);
            _rectTransform.anchoredPosition = Vector2.zero;

            _parallaxPosition = Vector2.zero;
        }

        private void Update()
        {
            if (_scale == 1f)
                return;

            if (!LevelManager.Started)
                return;

            if (LevelManager.Paused)
                return;

            float heightPercentage = Mathf.Clamp((ScoreController.Instance.Score / _endHeight), 0.0f, 1.0f);

            _parallaxPosition.y = -(heightPercentage * Screen.height);
            _rectTransform.anchoredPosition = _parallaxPosition;
        }
    }
}