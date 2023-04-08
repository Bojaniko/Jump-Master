using UnityEngine;

using JumpMaster.LevelControllers;

namespace JumpMaster.Movement
{
    public sealed class HangControl : MovementControl<HangControlDataSO>, IExplicitControl
    {
        private readonly float _hangFromScreenWidth;

        public event ExplicitControlEventHandler OnExplicitDetection;

        public HangControl(MovementController controller, HangControlDataSO data) : base(controller, data)
        {
            _hangFromScreenWidth = Screen.width - data.StickDistanceScreen;
        }

        public override MovementState ActiveState { get { return MovementState.HANGING; } }

        public override bool CanExit()
        {
            if (Time.time - ControlArgs.StartTime < ControlData.MinDuration)
                return false;
            return true;
        }
        protected override void ExitControl() { }

        protected override bool CanStartControl()
        {
            if (!LevelController.Started)
                return false;
            return true;
        }
        protected override void StartControl()
        {
            Controller.ControlledRigidbody.useGravity = false;
            Controller.ControlledRigidbody.velocity = Vector3.zero;

            Vector3 hang_position = Vector3.zero;
            if (ControlArgs.Direction.Horizontal == 1)
                hang_position = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width - ((Controller.BoundsScreenPosition.max.x - Controller.BoundsScreenPosition.min.x) / 2), 0));
            if (ControlArgs.Direction.Horizontal == -1)
                hang_position = Camera.main.ScreenToWorldPoint(new Vector2((Controller.BoundsScreenPosition.max.x - Controller.BoundsScreenPosition.min.x) / 2, 0));
            hang_position.y = Controller.transform.position.y;
            hang_position.z = Controller.transform.position.z;
            Controller.transform.position = hang_position;

            SwitchDirection();
        }

        public override Vector3 GetCurrentVelocity()
        {
            return Vector3.zero;
        }

        public override void Pause() { }
        public override void Resume() { }
        protected override void OnMovementUpdate()
        {
            if (Started)
                return;

            if (Controller.ActiveControl.ControlArgs.Direction.Horizontal == 0)
                return;

            if (Controller.ActiveControl.ControlArgs.Direction.Horizontal == -1)
            {
                if (Controller.ControlledRigidbody.velocity.x < -ControlData.MinStickVelocity)
                {
                    if (Controller.BoundsScreenPosition.min.x > ControlData.StickDistanceScreen)
                        return;
                }
                else
                {
                    if (Controller.BoundsScreenPosition.min.x >= 0)
                        return;
                }
                OnExplicitDetection(this, new(Controller.ControlledRigidbody, MovementDirection.Left));
            }

            if (Controller.ActiveControl.ControlArgs.Direction.Horizontal == 1)
            {
                if (Controller.ControlledRigidbody.velocity.x > ControlData.MinStickVelocity)
                {
                    if (Controller.BoundsScreenPosition.max.x < _hangFromScreenWidth)
                        return;
                }
                else
                {
                    if (Controller.BoundsScreenPosition.max.x <= Screen.width)
                        return;
                }
                OnExplicitDetection(this, new(Controller.ControlledRigidbody, MovementDirection.Right));
            }
        }

        private void SwitchDirection()
        {
            MovementDirection dir;
            if (ControlArgs.Direction.Horizontal == 1)
                dir = new(0, -1);
            else
                dir = new(0, 1);
            UpdateDirection(dir);
        }
    }
}