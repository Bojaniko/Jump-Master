using UnityEngine;

using JumpMaster.Movement;
using JumpMaster.LevelControllers;

namespace JumpMaster.CameraControls
{
    public class CameraController : LevelControllerInitializable
    {
        private static CameraController s_instance;

        public static CameraController Instance
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
                    Debug.LogError("There can only be one Camera Controller in the scene!");
            }
        }

        protected override void Initialize()
        {
            Instance = this;

            Cache();

            Restart();

            LevelController.OnRestart += Restart;
        }

        private void Restart()
        {
            c_mainCamera.transform.position = new Vector3(0, 0, ZPosition);

            _currentHeight = 0f;
            _startHeight = 0f;
            _speed = AscendingSpeed;
        }

        public float ZPosition = -15f;
        [Range(0.1f, 10f)]
        public float AscendingSpeed = 0.6f;
        [Range(50, 500)]
        public int MaxScreenWidthPosition = 100;
        [Range(50, 500)]
        public int MaxScreenHeightPosition = 100;
        [Range(1f, 10f)]
        public float ReachEdgeSpeed = 2f;

        private float _speed;
        private float _startHeight;
        private float _currentHeight;

        private void FixedUpdate()
        {
            if (!LevelController.Started)
                return;

            if (LevelController.Ended)
                return;

            if (LevelController.Paused)
                return;

            if (_speed == 0f)
                return;

            ProcessHeightPosition(ref _currentHeight);

            c_mainCamera.transform.position = GetCurrentHeightPosition(_startHeight, _currentHeight);
        }

        private void ProcessHeightPosition(ref float current_height)
        {
            float height_step = _speed * Time.deltaTime;

            if (MovementController.Instance.ActiveControl.ActiveState.Equals(MovementState.JUMPING) ||
                MovementController.Instance.ActiveControl.ActiveState.Equals(MovementState.JUMP_CHARGING) ||
                MovementController.Instance.ActiveControl.ActiveState.Equals(MovementState.BOUNCING))
            {
                if (MovementController.Instance.BoundsScreenPosition.max.y < Screen.height - MaxScreenHeightPosition)
                    goto FloatCheck;

                float playerHeightDifference = MovementController.Instance.Bounds.bounds.max.y -
                    c_mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height - MaxScreenHeightPosition,
                    Vector3.Distance(c_mainCamera.transform.position, MovementController.Instance.transform.position))).y;

                height_step = playerHeightDifference * ReachEdgeSpeed * Time.deltaTime;
            }

            FloatCheck:
            if (MovementController.Instance.ActiveControl.ActiveState.Equals(MovementState.FLOATING))
                height_step = 0;

            current_height += height_step;
        }

        private Vector3 GetCurrentHeightPosition(float start_height, float current_height)
        {
            Vector3 camera_position = c_mainCamera.transform.position;
            camera_position.y = start_height + current_height;

            return camera_position;
        }

        // ##### CACHE ##### \\

        private Camera c_mainCamera;

        private void Cache()
        {
            c_mainCamera = Camera.main;
        }
    }
}