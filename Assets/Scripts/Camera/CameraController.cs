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

            CalculateMargins(Data.MarginScreen);

            LevelController.OnRestart += Restart;
        }

        private void Restart()
        {
            c_mainCamera.transform.position = new Vector3(0, 0, Data.ZPosition);

            _currentHeight = 0f;
            _startHeight = 0f;
            _speed = Data.AscendingSpeed;
        }

        public CameraDataSO Data => _data;
        [SerializeField] private CameraDataSO _data;

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
                height_step = GetPlayerHeightDifference() * Data.ReachEdgeSpeed * Time.deltaTime;
            }

            //if (MovementController.Instance.BoundsScreenPosition.max.y < Screen.height - Data.MaxScreenHeightPosition)
            if (MovementController.Instance.ActiveControl.ActiveState.Equals(MovementState.FLOATING))
                height_step = 0;

            current_height += height_step;
        }

        private Vector3 GetCurrentHeightPosition(in float start_height, in float current_height)
        {
            Vector3 camera_position = c_mainCamera.transform.position;
            camera_position.y = start_height + current_height;

            return camera_position;
        }

        private float GetPlayerHeightDifference()
        {
            return MovementController.Instance.Bounds.bounds.max.y -
                    c_mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height - Data.MaxScreenHeightPosition,
                    Vector3.Distance(c_mainCamera.transform.position, MovementController.Instance.transform.position))).y;
        }

        // ##### MARGIN ##### \\

        public static float TopMargin => _topMargin;
        private static float _topMargin;

        public static float BottomMargin => _bottomMargin;
        private static float _bottomMargin;

        public static float LeftMargin => _leftMargin;
        private static float _leftMargin;

        public static float RightMargin => _rightMargin;
        private static float _rightMargin;

        public static bool IsPositionInTopMargin(Vector2 screen_position) => screen_position.y >= TopMargin;
        public static bool IsPositionInBottomMargin(Vector2 screen_position) => screen_position.y <= BottomMargin;
        public static bool IsPositionInLeftMargin(Vector2 screen_position) => screen_position.x <= LeftMargin;
        public static bool IsPositionInRightMargin(Vector2 screen_position) => screen_position.x >= RightMargin;

        private static void CalculateMargins(in Vector2 margin)
        {
            _topMargin = Screen.height - margin.y;
            _bottomMargin = margin.y;

            _leftMargin = margin.x;
            _rightMargin = Screen.width - margin.x;
        }

        // ##### CACHE ##### \\

        private Camera c_mainCamera;

        private void Cache()
        {
            c_mainCamera = Camera.main;
        }
    }
}