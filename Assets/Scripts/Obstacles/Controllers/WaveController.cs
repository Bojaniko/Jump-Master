using UnityEngine;

using JumpMaster.LevelControllers;

namespace JumpMaster.Obstacles
{
    public enum WaveType { NORMAL, BOSS }

    public class WaveController
    {
        public bool Started { get; private set; }

        public int CurrentWave { get; private set; }
        public float StartedTime { get; private set; }
        public float EndedTime { get; private set; }

        private WaveSO _data;

        public WaveController()
        {
            Started = false;

            CurrentWave = 0;
            StartedTime = 0f;
            EndedTime = 0f;

            LevelController.OnRestart += Restart;
        }

        public void NewWave(WaveSO data, IObstacleController[] controllers)
        {
            if (Started)
                return;
            _data = data;
            foreach (IObstacleController controller in controllers)
            {
                controller.UpdateData(_data.ControllersData.GetSpawnMetricsForController(in controller));
            }
        }

        public void StartWave()
        {
            if (Started)
                return;

            if (_data == null)
                return;

            Started = true;

            StartedTime = Time.time;

            CurrentWave++;
        }

        public void EndWave()
        {
            if (!Started)
                return;

            Started = false;

            EndedTime = Time.time;

            _data = null;
        }

        private void Restart()
        {
            EndWave();

            CurrentWave = 0;

            EndedTime = 0f;
            StartedTime = 0f;
        }
    }
}