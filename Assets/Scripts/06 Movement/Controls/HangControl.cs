using UnityEngine;

using JumpMaster.Core;

namespace JumpMaster.Movement
{
    public sealed class HangControl : MovementControl<HangControlDataSO, HangControlArgs>, IPrimaryControl, IExplicitControl, IDirectional
    {
        private readonly float _hangFromScreenWidth;

        public event ExplicitControlEventHandler OnExplicitDetection;

        public MovementDirection Direction
        {
            get
            {
                if (_controlArgs == null)
                    return MovementDirection.Zero;
                return _controlArgs.Direction;
            }
        }

        public HangControl(MovementController controller, HangControlDataSO data) : base(controller, data)
        {
            _hangFromScreenWidth = Screen.width - data.StickDistanceScreen;
        }

        public override MovementState ActiveState { get { return MovementState.HANGING; } }

        public override bool CanExit(IMovementControl exit_control)
        {
            if (exit_control.ActiveState.Equals(MovementState.FLOATING))
                return false;
            if (Time.time - ControlArgs.StartTime < ControlData.MinDuration)
                return false;
            return true;
        }
        protected override void ExitControl() { }

        protected override bool CanStartControl()
        {
            if (!LevelManager.Started)
                return false;
            return true;
        }
        protected override void StartControl()
        {
            Controller.ControlledRigidbody.gravityScale = 0f;
            Controller.ControlledRigidbody.velocity = Vector2.zero;

            Vector3 hang_position = Vector3.zero;
            if (_controlArgs.Direction.Horizontal == 1)
                hang_position = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width - ((Controller.Bounds.ScreenMax.x - Controller.Bounds.ScreenMin.x) / 2), 0));
            if (_controlArgs.Direction.Horizontal == -1)
                hang_position = Camera.main.ScreenToWorldPoint(new Vector2((Controller.Bounds.ScreenMax.x - Controller.Bounds.ScreenMin.x) / 2, 0));
            hang_position.y = Controller.transform.position.y;
            hang_position.z = Controller.transform.position.z;
            Controller.transform.position = hang_position;

            SwitchDirection();
        }

        public override Vector2 GetCurrentVelocity()
        {
            return Vector2.zero;
        }

        public override void Pause() { }
        public override void Resume() { }
        protected override void OnMovementUpdate()
        {
            if (Started)
                return;

            if (Controller.ControlledRigidbody.velocity.x < 0f)
            {
                if (Controller.ControlledRigidbody.velocity.x < -ControlData.MinStickVelocity)
                {
                    if (Controller.Bounds.ScreenMin.x > ControlData.StickDistanceScreen)
                        return;
                }
                else
                {
                    if (Controller.Bounds.ScreenMin.x >= 0)
                        return;
                }
                OnExplicitDetection?.Invoke(this, new HangControlArgs(new(Controller), MovementDirection.Left));
            }

            if (Controller.ControlledRigidbody.velocity.x > 0f)
            {
                if (Controller.ControlledRigidbody.velocity.x > ControlData.MinStickVelocity)
                {
                    if (Controller.Bounds.ScreenMax.x < _hangFromScreenWidth)
                        return;
                }
                else
                {
                    if (Controller.Bounds.ScreenMax.x <= Screen.width)
                        return;
                }
                OnExplicitDetection?.Invoke(this, new HangControlArgs(new(Controller), MovementDirection.Right));
            }
        }

        private void SwitchDirection()
        {
            if (_controlArgs.Direction.Equals(MovementDirection.Left))
                _controlArgs.Direction = MovementDirection.Right;
            else
                _controlArgs.Direction = MovementDirection.Left;
        }
    }
}