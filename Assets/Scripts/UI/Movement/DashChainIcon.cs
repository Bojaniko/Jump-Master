using JumpMaster.Movement;
using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    public class DashChainIcon : ChainIcon
    {
        private DashControl _dashControl;

        void Start()
        {
            _dashControl = MovementController.Instance.GetControl<DashControl>();

            _dashControl.OnChainUpdate += () =>
            {
                _image.fillAmount = 1f - ((float)_dashControl.Chain / _dashControl.ControlData.MaxChain);
            };
        }

        void Update()
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