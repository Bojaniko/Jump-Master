using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpMaster.LevelControllers
{
    public class CameraController : LevelControllerBase
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
        }

        protected override void Pause()
        {
            _ascendingSpeed = 0f;
        }

        protected override void Unpause()
        {
            _ascendingSpeed = AscendingSpeed;
        }

        protected override void PlayerDeath()
        {
            
        }

        protected override void Restart()
        {
            
        }

        public Camera Camera;

        [Range(0.1f, 10f)]
        public float AscendingSpeed = 0.6f;
        [Range(50, 500)]
        public int MaxScreenWidthPosition = 100;
        [Range(50, 500)]
        public int MaxScreenHeightPosition = 100;
        [Range(1f, 10f)]
        public float ReachEdgeSpeed = 2f;

        public bool AscendingStarted { get; private set; } = false;

        private float _ascendingSpeed = 0f;
        private float _ascendingHeight = 0f;
        private float _ascendingStartHeight = 0f;

        private Vector3 _cameraPosition;

        private void Start()
        {
            if (MovementController.Instance != null)
                MovementController.Instance.StateController.OnStateChanged += StartRise;
        }

        private void Update()
        {
            if (!AscendingStarted)
                return;
            if (_ascendingSpeed == 0f)
                return;

            float _dashingWidth = 0f;

            if (MovementController.Instance.StateController.CurrentState.Equals(MovementState.JUMPING) ||
                MovementController.Instance.StateController.CurrentState.Equals(MovementState.FLOATING))
            {
                if (MovementController.Instance.BoundsScreenPosition.max.y > Screen.height - MaxScreenHeightPosition)
                {
                    float playerHeightDifference = LevelController.Instance.PlayerGameObject.GetComponent<Collider>().bounds.max.y -
                        Camera.ScreenToWorldPoint(new Vector3(0, Screen.height - MaxScreenHeightPosition,
                        Vector3.Distance(Camera.transform.position, LevelController.Instance.PlayerGameObject.transform.position))).y;

                    _ascendingHeight += playerHeightDifference * ReachEdgeSpeed * Time.deltaTime;
                }
            }

            if (MovementController.Instance.StateController.CurrentState.Equals(MovementState.FLOATING) != true)
                _ascendingHeight += AscendingSpeed * Time.deltaTime;

            _cameraPosition = Camera.transform.position;
            _cameraPosition.x += _dashingWidth * ReachEdgeSpeed * Time.deltaTime;
            _cameraPosition.y = _ascendingStartHeight + _ascendingHeight;

            Camera.transform.position = _cameraPosition;
        }

        private void StartRise(MovementState state)
        {
            if (!state.Equals(MovementState.JUMPING))
                return;
            AscendingStarted = true;
            _ascendingStartHeight = Camera.transform.position.y;
            MovementController.Instance.StateController.OnStateChanged -= StartRise;
        }
    }
}