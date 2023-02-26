using UnityEngine;

namespace JumpMaster.LevelControllers
{
    public class JumpEffectsController : MonoBehaviour
    {
        private static JumpEffectsController s_instance;

        public static JumpEffectsController Instance
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

            MovementController.Instance.OnJump += SpawnJumpCloud;
            MovementController.Instance.OnDash += SpawnDashCloud;
        }

        public GameObject JumpCloudPrefab;
        public GameObject DashCloudPrefab;

        private void SpawnJumpCloud(Vector3 position)
        {
            Instantiate(JumpCloudPrefab, position, Quaternion.identity);
        }

        private void SpawnDashCloud(Vector3 position, Quaternion direction)
        {
            Instantiate(DashCloudPrefab, position, direction);
        }
    }
}