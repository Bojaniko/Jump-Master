using UnityEngine;
using UnityEngine.UI;

using JumpMaster.Movement;
using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    public class ChainIcon : MonoBehaviour
    {
        private bool _started = false;

        private Vector2 _position;

        [SerializeField] private float _iconSize;
        [SerializeField] private Vector2 _jumpOffset;
        [SerializeField] private Vector2 _dashOffset;

        private void Awake()
        {
            Cache();

            Restart();

            LevelController.OnEndLevel += EndLevel;
            LevelController.OnRestart += Restart;
        }

        private void StartIcons()
        {
            _started = true;

            c_jumpControl.OnStart -= StartIcons;

            c_dashControl.OnStart += UpdateDash;
            c_dashControl.OnChain += UpdateDashChain;
            c_jumpControl.OnChain += UpdateJumpChain;

            c_dashImage.enabled = true;
            c_jumpImage.enabled = true;
        }

        private void EndLevel()
        {
            c_dashControl.OnStart += UpdateDash;
            c_dashControl.OnChain -= UpdateDashChain;
            c_jumpControl.OnChain -= UpdateJumpChain;

            c_dashImage.enabled = false;
            c_jumpImage.enabled = false;
        }

        private void Restart()
        {
            _started = false;

            c_jumpControl.OnStart += StartIcons;

            c_dashImage.enabled = false;
            c_jumpImage.enabled = false;

            c_dashImage.fillAmount = 1f;
            c_jumpImage.fillAmount = 1f;

            c_dashRect.sizeDelta = Vector2.one * _iconSize * 10f;
            c_jumpRect.sizeDelta = Vector2.one * _iconSize * 10f;

            c_dashRect.anchoredPosition = _dashOffset;
            c_jumpRect.anchoredPosition = _jumpOffset;
        }

        private void Update()
        {
            if (!_started)
                return;

            _position.x = MovementController.Instance.ScreenPosition.x;
            _position.y = MovementController.Instance.BoundsScreenPosition.min.y;
            c_rect.anchoredPosition = _position;
        }

        // ##### JUMP & DASH CONTROL UPDATES ##### \\

        private void UpdateJumpChain(int chain, int max_chain)
        {
            c_jumpImage.fillAmount = 1f - ((float)chain / max_chain);
        }

        private void UpdateDashChain(int chain, int max_chain)
        {
            c_dashImage.fillAmount = 1f - ((float)chain / max_chain);
        }

        private void UpdateDash()
        {
            DashControlArgs dashArgs = (DashControlArgs)c_dashControl.ControlArgs;

            if (dashArgs.Direction.Horizontal == -1 || dashArgs.Direction.Horizontal == 0)
            {
                c_dashRect.anchoredPosition = _dashOffset;
                c_jumpRect.anchoredPosition = _jumpOffset;
            }

            if (dashArgs.Direction.Horizontal == 1)
            {
                c_dashRect.anchoredPosition = new Vector2(-_dashOffset.x, _dashOffset.y);
                c_jumpRect.anchoredPosition = new Vector2(-_jumpOffset.x, _jumpOffset.y);
            }
        }

        // ##### CACHE ##### \\

        private Image c_dashImage;
        private Image c_jumpImage;

        private RectTransform c_rect;
        private RectTransform c_dashRect;
        private RectTransform c_jumpRect;

        private DashControl c_dashControl;
        private JumpControl c_jumpControl;

        private void Cache()
        {
            c_rect = GetComponent<RectTransform>();

            c_dashControl = MovementController.Instance.GetControl<DashControl>();
            c_jumpControl = MovementController.Instance.GetControl<JumpControl>();

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name.Equals("DASH"))
                {
                    c_dashRect = transform.GetChild(i).GetComponent<RectTransform>();
                    c_dashImage = transform.GetChild(i).GetComponent<Image>();
                }
                if (transform.GetChild(i).name.Equals("JUMP"))
                {
                    c_jumpRect = transform.GetChild(i).GetComponent<RectTransform>();
                    c_jumpImage = transform.GetChild(i).GetComponent<Image>();
                }
            }
        }
    }
}