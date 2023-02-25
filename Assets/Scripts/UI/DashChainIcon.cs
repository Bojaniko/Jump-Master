using UnityEngine;
using UnityEngine.UI;

using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    public class DashChainIcon : ChainIcon
    {
        private int _maxDashChain;

        void Start()
        {
            _maxDashChain = MovementController.Instance.MaxDashChain;
        }

        void Update()
        {
            _image.fillAmount = 1f - ((float)MovementController.Instance.DashChain / _maxDashChain);

            if (MovementController.Instance.HorizontalDirection == -1 || MovementController.Instance.HorizontalDirection == 0)
                _position.x = MovementController.Instance.BoundsScreenPosition.max.x + _offsetPosition.x;
            if (MovementController.Instance.HorizontalDirection == 1)
                _position.x = MovementController.Instance.BoundsScreenPosition.min.x - _offsetPosition.x;
            _position.y = MovementController.Instance.BoundsScreenPosition.min.y + _offsetPosition.y;
            _rect.anchoredPosition = _position;
        }
    }
}