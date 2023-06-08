using UnityEngine;

using JumpMaster.Core;
using JumpMaster.Movement;

namespace JumpMaster.CameraControls
{
    public class CameraController : LevelController
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

            LevelManager.OnRestart += Restart;
        }

        private void Restart()
        {
            c_mainCamera.transform.position = new Vector3(0, 0, Data.ZPosition);

            _currentHeight = 0f;
            _startHeight = 0f;

            _reachEdgeHeightFromTop = Screen.height - Data.MaxScreenHeightPosition;
        }

        public CameraDataSO Data => _data;
        [SerializeField] private CameraDataSO _data;

        private float _startHeight;
        private float _currentHeight;

        private float _reachEdgeHeightFromTop;

        private void FixedUpdate()
        {
            if (!LevelManager.Started)
                return;

            if (LevelManager.Ended)
                return;

            if (LevelManager.Paused)
                return;

            ProcessHeightPosition(ref _currentHeight);

            c_mainCamera.transform.position = GetCurrentHeightPosition(_startHeight, _currentHeight);
        }

        private void ProcessHeightPosition(ref float current_height)
        {
            float heightStep;

            if (MovementController.Instance.Bounds.ScreenMax.y > _reachEdgeHeightFromTop)
                heightStep = GetPlayerHeightDifference() * Data.ReachEdgeSpeed * Time.deltaTime;

            else if (MovementController.Instance.ActiveControl.ActiveState.Equals(MovementState.FLOATING))
                heightStep = 0;

            else
                heightStep = Data.AscendingSpeed * Time.deltaTime;

            current_height += heightStep;
        }

        private Vector3 GetCurrentHeightPosition(in float start_height, in float current_height)
        {
            Vector3 camera_position = c_mainCamera.transform.position;
            camera_position.y = start_height + current_height;

            return camera_position;
        }

        private float GetPlayerHeightDifference()
        {
            return Mathf.Clamp(MovementController.Instance.Bounds.ScreenMax.y - _reachEdgeHeightFromTop, 0f, Data.MaxScreenHeightPosition) / Data.MaxScreenHeightPosition;
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