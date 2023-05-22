using UnityEngine;

namespace JumpMaster.Movement
{
    public class MovementEffectsController : MonoBehaviour
    {
        private static MovementEffectsController s_instance;

        public static MovementEffectsController Instance
        {
            get
            {
                return s_instance;
            }
            private set
            {
                if (s_instance == null)
                    s_instance = value;
                else
                    Debug.LogError("There can only be one Jump Effect Controller in the scene!");
            }
        }

        private void Awake()
        {
            Instance = this;

            MovementController.Instance.GetControlByState(MovementState.JUMPING).OnStart += SpawnJumpCloud;
            MovementController.Instance.GetControlByState(MovementState.DASHING).OnStart += SpawnDashCloud;
            MovementController.Instance.GetControlByState(MovementState.JUMP_CHARGING).OnStart += SpawnJumpCloud;
        }

        public GameObject JumpCloudPrefab;
        public GameObject DashCloudPrefab;

        private void SpawnJumpCloud()
        {
            Vector3 position = MovementController.Instance.transform.position;
            position.y = MovementController.Instance.Bounds.bounds.min.y;
            Instantiate(JumpCloudPrefab, position, Quaternion.identity);
        }

        private void SpawnDashCloud()
        {
            Vector3 position = new Vector3(MovementController.Instance.Bounds.bounds.min.x, MovementController.Instance.transform.position.y, MovementController.Instance.transform.position.z);
            //Quaternion dash_direction = Quaternion.LookRotation(Vector3.right * ControlArgs.Direction.Horizontal, Vector3.up);
            Instantiate(DashCloudPrefab, position, Quaternion.identity);
        }
    }
}