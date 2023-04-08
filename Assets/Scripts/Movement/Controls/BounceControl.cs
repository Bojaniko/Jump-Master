using UnityEngine;

using JumpMaster.LevelControllers;

namespace JumpMaster.Movement
{
    public class BounceControl : MovementControl<BounceControlDataSO>, IExplicitControl, ITransitionable
    {
        private float _distance;
        private float _targetDistance;

        private float _strength;
        private Vector3 _direction;

        public BounceControl(MovementController controller, BounceControlDataSO data) : base(controller, data)
        {

        }
        public override MovementState ActiveState => MovementState.BOUNCING;
        public MovementState TransitionState
        {
            get
            {
                if (ControlArgs.Direction.Vertical == 1)
                    return MovementState.FLOATING;
                return MovementState.FALLING;
            }
        }

        public event ExplicitControlEventHandler OnExplicitDetection;
        public event TransitionableControlEventHandler OnTransitionable;

        public override Vector3 GetCurrentVelocity()
        {
            return _direction * ControlData.BounceForce * _strength;
        }

        public override void Pause() { }
        public override void Resume() { }

        public override bool CanExit()
        {
            return (_distance / _targetDistance >= ControlData.MinExitDistancePercentage);
        }
        protected override void ExitControl() { }

        protected override bool CanStartControl() { return true; }
        protected override void StartControl()
        {
            _strength = Mathf.Clamp(ControlArgs.Strength, ControlData.MinStrengthPercentage, 1.0f);
            _direction = (Vector3.up * ControlArgs.Direction.Vertical) + (Vector3.down * ControlArgs.Direction.Horizontal);

            _targetDistance = ControlData.BounceDistance * _strength;
        }

        protected override void OnMovementUpdate()
        {
            if (!Started)
                ExplicitCheck();
            else
                TransitionCheck();
        }

        private void ExplicitCheck()
        {
            if (Controller.ActiveControl.ActiveState.Equals(MovementState.DASHING)
                || Controller.ActiveControl.ActiveState.Equals(MovementState.JUMPING)
                || Controller.ActiveControl.ActiveState.Equals(MovementState.JUMP_CHARGING))
                return;

            if (Controller.BoundsScreenPosition.min.y > 0f)
                return;

            if (OnExplicitDetection == null)
                return;

            OnExplicitDetection(this, new(Controller.ControlledRigidbody, MovementDirection.Up));

            DamageInfo damage_info = new(Controller.transform.position, 1f, 20f);
            DamageController.LogDamage(damage_info);
        }

        private void TransitionCheck()
        {
            _distance = Vector3.Distance(ControlArgs.StartPosition, Controller.transform.position);

            if (_distance < _targetDistance)
                return;

            if (OnTransitionable == null)
                return;

            MovementControlArgs start_args = new(Controller.ControlledRigidbody, MovementDirection.Up);
            OnTransitionable(Controller.GetControlByState(TransitionState), start_args);
        }
    }
}