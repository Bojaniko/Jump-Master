using UnityEngine;

using JumpMaster.LevelControllers;

namespace JumpMaster.Movement
{
    public class BounceControl : MovementControl<BounceControlDataSO, BounceControlArgs>, IExplicitControl, ITransitionable
    {
        private float _distance;
        private float _targetDistance;

        private float _strength;

        public BounceControl(MovementController controller, BounceControlDataSO data) : base(controller, data)
        {
            DamageController.OnPlayerDamageLogged += TrapDamageInput;
        }
        public override MovementState ActiveState => MovementState.BOUNCING;
        public MovementState TransitionState
        {
            get
            {
                if (Vector2.Dot(Vector2.up, _controlArgs.Direction.normalized) > 0f)
                    return MovementState.FLOATING;
                return MovementState.FALLING;
            }
        }

        public event ExplicitControlEventHandler OnExplicitDetection;
        public event TransitionableControlEventHandler OnTransitionable;

        public override Vector3 GetCurrentVelocity()
        {
            return _controlArgs.Direction * ControlData.BounceForce * _strength;
        }

        public override void Pause() { }
        public override void Resume() { }

        public override bool CanExit()
        {
            return (_distance / _targetDistance >= ControlData.MinExitDistancePercentage);
        }
        protected override void ExitControl() { }

        protected override bool CanStartControl() { return false; }
        protected override void StartControl()
        {
            _strength = Mathf.Clamp(ControlArgs.Strength, ControlData.MinStrengthPercentage, 1.0f);
            //_direction = (Vector3.up * ControlArgs.Direction.Vertical) + (Vector3.right * ControlArgs.Direction.Horizontal);

            _targetDistance = ControlData.BounceDistance * _strength;
        }

        protected override void OnMovementUpdate()
        {
            if (Started)
                TransitionCheck();
        }

        private void TrapDamageInput(DamageInfo info)
        {
            if (info.TypeOfDamage.Equals(DamageType.TRAP))
            {
                OnExplicitDetection?.Invoke(this, new BounceControlArgs(new(Controller), info.Direction));
            }
            if (info.TypeOfDamage.Equals(DamageType.EXPLOSION))
            {
                OnExplicitDetection?.Invoke(this, new BounceControlArgs(new (Controller), info.Direction));
            }
        }

        private void TransitionCheck()
        {
            _distance = Vector2.Distance(ControlArgs.StartPosition, Controller.transform.position);

            if (_distance < _targetDistance)
                return;

            MovementControlArgs start_args = new(Controller, 0.5f);
            if (TransitionState.Equals(MovementState.FLOATING))
                OnTransitionable?.Invoke(Controller.GetControlByState(TransitionState), new FloatControlArgs(start_args, MovementDirection.Up));
            else
                OnTransitionable?.Invoke(Controller.GetControlByState(TransitionState), new FallControlArgs(start_args, ControlData.ExplosionVelocityDrag));
        }
    }
}