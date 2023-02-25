using System.Collections.Generic;
using UnityEngine;

using Studio28.Utility;

namespace JumpMaster.LevelControllers.Obstacles
{
    public enum WaveState { STARTED, ENDED }
    public enum WaveType { NORMAL, BOSS }

    public class WaveController
    {
        public int CurrentWave { get; private set; }
        public float StartedTime { get; private set; }
        public float EndedTime { get; private set; }

        private WaveSO _data;

        private StateMachine<WaveState> _stateController;

        public WaveState CurrentState
        {
            get
            {
                return _stateController.CurrentState;
            }
        }

        private List<ObstacleController> _controllers;
        public ObstacleController[] Controllers
        {
            get
            {
                return _controllers.ToArray();
            }
        }
        public WaveController(List<ObstacleController> controllers)
        {
            CurrentWave = 0;
            StartedTime = 0f;
            EndedTime = 0f;

            _controllers = controllers;
            _stateController = new("Wave", WaveState.ENDED);

            ObstacleLevelController.Instance.OnLoop += UpdateWave;
        }

        public void NewWave(WaveSO data)
        {
            if (_stateController.CurrentState.Equals(WaveState.STARTED))
                return;
            _data = data;
            foreach (ObstacleController controller in _controllers)
            {
                var controller_type = controller.GetType();

                if (controller_type == typeof(FallingBombController))
                    controller.Self.ControllerData.UpdateData(_data.ControllersData.FallingBomb);
                if (controller_type == typeof(LaserGateController))
                    controller.Self.ControllerData.UpdateData(_data.ControllersData.LaserGate);
                if (controller_type == typeof(MissileController))
                    controller.Self.ControllerData.UpdateData(_data.ControllersData.Missile);
            }
        }

        public void StartWave()
        {
            if (_stateController.CurrentState.Equals(WaveState.STARTED))
                return;

            if (_data == null)
                return;

            _stateController.SetState(WaveState.STARTED);

            StartedTime = Time.time;

            CurrentWave++;

            Debug.Log("Wave " + CurrentWave);
        }

        public void EndWave()
        {
            if (_stateController.CurrentState.Equals(WaveState.ENDED))
                return;

            _stateController.SetState(WaveState.ENDED);

            EndedTime = Time.time;

            _data = null;
        }

        private void UpdateWave()
        {
            if (_stateController.CurrentState.Equals(WaveState.ENDED))
                return;

            bool ended = true;

            ObstacleController controller;
            for (int i = 0; i < _controllers.Count; i++)
            {
                controller = _controllers[i];
                controller.TrySpawn();
                if (ended == true && controller.SpawnsLeft > 0)
                    ended = false;
            }

            if (ended == true && ObstacleLevelController.Instance.ActiveObstacles.Length > 0)
                ended = false;

            if (ended)
                EndWave();
        }
    }
}