using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpMaster.LevelControllers
{
    public class CameraController : LevelControllerInitializablePausable
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

            LevelController.Instance.OnLevelStarted += StartRise;
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
            _cameraPosition = Camera.transform.position;
            _cameraPosition.y -= _ascendingHeight - _ascendingStartHeight;

            _ascendingHeight = 0f;
        }

        protected override void LevelLoaded()
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

        private float _ascendingSpeed = 0f;
        private float _ascendingHeight = 0f;
        private float _ascendingStartHeight = 0f;

        private Vector3 _cameraPosition;

        private void Update()
        {
            if (!LevelController.Started)
                return;
            if (_ascendingSpeed == 0f)
                return;

            if (MovementController.Instance.StateController.CurrentState.Equals(MovementState.JUMPING) ||
                MovementController.Instance.StateController.CurrentState.Equals(MovementState.FLOATING))
            {
                if (MovementController.Instance.BoundsScreenPosition.max.y > Screen.height - MaxScreenHeightPosition)
                {
                    float playerHeightDifference = PlayerController.Instance.GetComponent<Collider>().bounds.max.y -
                        Camera.ScreenToWorldPoint(new Vector3(0, Screen.height - MaxScreenHeightPosition,
                        Vector3.Distance(Camera.transform.position, PlayerController.Instance.transform.position))).y;

                    _ascendingHeight += playerHeightDifference * ReachEdgeSpeed * Time.deltaTime;
                }
            }

            if (MovementController.Instance.StateController.CurrentState.Equals(MovementState.FLOATING) != true)
                _ascendingHeight += AscendingSpeed * Time.deltaTime;

            _cameraPosition = Camera.transform.position;
            _cameraPosition.y = _ascendingStartHeight + _ascendingHeight;

            Camera.transform.position = _cameraPosition;
        }

        private void StartRise()
        {
            _ascendingStartHeight = Camera.transform.position.y;
        }
    }
}