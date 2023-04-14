using System.Collections.Generic;
using UnityEngine;

using Studio28.Utility;

namespace JumpMaster.Obstacles
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

        public WaveController()
        {
            CurrentWave = 0;
            StartedTime = 0f;
            EndedTime = 0f;

            _stateController = new("Wave", WaveState.ENDED);

            ObstacleLevelController.Instance.OnLoop += UpdateWave;
        }

        public void NewWave(WaveSO data)
        {
            if (_stateController.CurrentState.Equals(WaveState.STARTED))
                return;
            _data = data;
            foreach (IObstacleController controller in ObstacleLevelController.Instance.Controllers)
            {
                var controller_type = controller.GetType();

                if (controller_type == typeof(FallingBombController))
                    controller.UpdateData(_data.ControllersData.FallingBomb);
                if (controller_type == typeof(LaserGateController))
                    controller.UpdateData(_data.ControllersData.LaserGate);
                if (controller_type == typeof(MissileController))
                    controller.UpdateData(_data.ControllersData.Missile);
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

            Debug.Log($"Ended wave {CurrentWave}");

            _stateController.SetState(WaveState.ENDED);

            EndedTime = Time.time;

            _data = null;
        }

        private void UpdateWave()
        {
            if (_stateController.CurrentState.Equals(WaveState.ENDED))
                return;

            ControllersTrySpawn();

            if (Ended())
                EndWave();
        }

        private void ControllersTrySpawn()
        {
            foreach (IObstacleController controller in ObstacleLevelController.Instance.Controllers)
            {
                controller.TrySpawn();
            }
        }

        private bool Ended()
        {
            if (ObstacleLevelController.Instance.ActiveObstacles.Length > 0)
                return false;
            foreach (IObstacleController controller in ObstacleLevelController.Instance.Controllers)
            {
                if (controller.SpawnsLeft > 0)
                    return false;
            }
            return true;
        }
    }
}