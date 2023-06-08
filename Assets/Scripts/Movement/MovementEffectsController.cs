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

        private MovementController _moveRef;

        private void Awake()
        {
            Instance = this;

            _moveRef = MovementController.Instance;

            _moveRef.GetControlByState(MovementState.JUMPING).OnStart += SpawnJumpCloud;
            _moveRef.GetControlByState(MovementState.DASHING).OnStart += SpawnDashCloud;
        }

        public GameObject JumpCloudPrefab;
        public GameObject DashCloudPrefab;

        private void SpawnJumpCloud()
        {
            Vector3 position = _moveRef.transform.position;
            position.y = _moveRef.Bounds.WorldMin.y;
            Instantiate(JumpCloudPrefab, position, Quaternion.identity);
        }

        private void SpawnDashCloud()
        {
            Vector3 position = new Vector3(_moveRef.Bounds.WorldMin.x, _moveRef.transform.position.y, _moveRef.transform.position.z);
            //Quaternion dash_direction = Quaternion.LookRotation(Vector3.right * ControlArgs.Direction.Horizontal, Vector3.up);
            Instantiate(DashCloudPrefab, position, Quaternion.identity);
        }
    }
}