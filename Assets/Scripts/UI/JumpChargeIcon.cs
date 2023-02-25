using UnityEngine;
using UnityEngine.UI;

using JumpMaster.Controls;
using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    [RequireComponent(typeof(Image))]
    public class JumpChargeIcon : MonoBehaviour
    {
        private RectTransform _rect;

        private Image _image;

        private float _startTime = 0f;
        private float _minHoldTime = 0f;

        private bool _charging = false;

        private Color _defaultColor;
        private Color _updateColor;

        [SerializeField]
        private float _size = 10f;

        [SerializeField]
        private Sprite _emptyCircle, _fullCircle;

        private void Awake()
        {
            if (MovementController.Instance == null)
            {
                gameObject.SetActive(false);
                return;
            }

            _rect = GetComponent<RectTransform>();
            _rect.sizeDelta = Vector2.one * 10f * _size;

            _image = GetComponent<Image>();
            _image.fillAmount = 0f;

            _defaultColor = _image.color;
            _updateColor = _defaultColor;

            InputController.Instance.OnHoldStarted += ChargeStart;
            InputController.Instance.OnHoldPerformed += ChargeEnd;
            InputController.Instance.OnHoldCancelled += ChargeEnd;
        }

        private void Update()
        {
            if (!_charging)
                return;

            if (Time.time - _startTime < _minHoldTime)
            {
                _image.fillAmount = 1f;
                _image.sprite = _fullCircle;

                //float percentage = 1f - ((Time.time - _startTime) / _minHoldTime);
                float percentage = (Time.time - _startTime) / _minHoldTime;

                _rect.localScale = Vector3.one * (1f - percentage);

                _updateColor.a = _defaultColor.a * Mathf.Clamp(percentage, 0.2f, 1f);
                _image.color = _updateColor;
            }
            else
            {
                _rect.localScale = Vector3.one;
                _image.sprite = _emptyCircle;
                _image.color = _defaultColor;
                _image.fillAmount = Mathf.Clamp((Time.time - _startTime - _minHoldTime) / MovementController.Instance.MaxChargeDuration, 0f, 1f);
            }
        }

        private void ChargeStart(Vector2 position, float min_hold_time)
        {
            if (LevelController.Paused)
                return;
            if (!MovementController.Instance.CanJump)
                return;

            _charging = true;
            _startTime = Time.time;
            _minHoldTime = min_hold_time;
            _rect.anchoredPosition = position;
        }

        private void ChargeEnd()
        {
            _image.fillAmount = 0f;
            _charging = false;
        }
    }
}