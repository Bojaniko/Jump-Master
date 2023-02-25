using UnityEngine;
using UnityEngine.UI;

using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    public class JumpChainIcon : ChainIcon
    {
        private int _maxJumpChain;

        private void Start()
        {
            _maxJumpChain = MovementController.Instance.MaxJumpChain;
        }

        private void Update()
        {
            _image.fillAmount = 1f - ((float)MovementController.Instance.JumpChain / _maxJumpChain);

            if (MovementController.Instance.HorizontalDirection == -1 || MovementController.Instance.HorizontalDirection == 0)
                _position.x = MovementController.Instance.BoundsScreenPosition.max.x + _offsetPosition.x;
            if (MovementController.Instance.HorizontalDirection == 1)
                _position.x = MovementController.Instance.BoundsScreenPosition.min.x - _offsetPosition.x;
            _position.y = MovementController.Instance.BoundsScreenPosition.min.y + _offsetPosition.y;
            _rect.anchoredPosition = _position;
        }
    }
}