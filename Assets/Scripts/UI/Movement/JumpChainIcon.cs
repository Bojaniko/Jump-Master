using JumpMaster.Movement;
using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    public class JumpChainIcon : ChainIcon
    {
        private JumpControl _jumpControl;
        private DashControl _dashControl;

        private void Start()
        {
            _jumpControl = MovementController.Instance.GetControl<JumpControl>();
            _dashControl = MovementController.Instance.GetControl<DashControl>();

            _jumpControl.OnChainUpdate += () =>
            {
                _image.fillAmount = 1f - ((float)_jumpControl.Chain / _jumpControl.ControlData.MaxChain);
            };
        }

        private void Update()
        {
            if (!LevelController.Started)
                return;

            if (_dashControl.ControlArgs.Direction.Horizontal == -1 || _dashControl.ControlArgs.Direction.Horizontal == 0)
                _position.x = MovementController.Instance.BoundsScreenPosition.max.x + _offsetPosition.x;
            if (_dashControl.ControlArgs.Direction.Horizontal == 1)
                _position.x = MovementController.Instance.BoundsScreenPosition.min.x - _offsetPosition.x;
            _position.y = MovementController.Instance.BoundsScreenPosition.min.y + _offsetPosition.y;
            _rect.anchoredPosition = _position;
        }
    }
}