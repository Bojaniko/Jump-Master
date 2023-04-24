using UnityEngine;

namespace JumpMaster.Movement
{
    public class StillControl : MovementControl<StillControlDataSO, MovementControlArgs>
    {
        public StillControl(MovementController controller, StillControlDataSO data) : base(controller, data)
        {

        }

        public override MovementState ActiveState { get { return MovementState.STILL; } }

        public override bool CanExit()
        {
            return true;
        }

        protected override bool CanStartControl()
        {
            return true;
        }

        public override Vector3 GetCurrentVelocity()
        {
            return Vector3.zero;
        }

        public override void Pause()
        {

        }

        public override void Resume()
        {

        }

        protected override void OnMovementUpdate()
        {
            
        }

        protected override void StartControl()
        {
            Controller.ControlledRigidbody.useGravity = false;
        }

        protected override void ExitControl()
        {

        }
    }
}