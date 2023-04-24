using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using JumpMaster.LevelControllers;

namespace JumpMaster.Obstacles
{
    public class ObstacleLevelController : LevelControllerInitializable
    {
        public ObstacleLevelControllerSO ObstacleLevelControllerData;

        private static ObstacleLevelController s_instance;
        public static ObstacleLevelController Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new GameObject().AddComponent<ObstacleLevelController>();
                return s_instance;
            }
        }

        private void InitializeControllers()
        {

            if (_missileController == null)
                _missileController = new(_data.DefaultControllersData.Missile);

            if (_fallingBombController == null)
                _fallingBombController = new(_data.DefaultControllersData.FallingBomb);

            if (_laserGateController == null)
                _laserGateController = new(_data.DefaultControllersData.LaserGate);

            _controllers = new();

            _controllers.Add(_missileController);
            _controllers.Add(_fallingBombController);
            _controllers.Add(_laserGateController);

            foreach (IObstacleController controller in _controllers)
            {
                controller.OnActiveObstaclesChange += CalculateActiveObstacles;
            }

            if (_waveController == null)
            {
                _waveController = new();
                _waveController.NewWave(_data.GetRandomNormalWave());
            }
        }

        private void CalculateActiveObstacles()
        {
            _activeObstacles.Clear();

            foreach (IObstacleController controller in _controllers)
            {
                _activeObstacles.AddRange(controller.ActiveObstacles);
            }
        }

        protected override void Initialize()
        {
            if (s_instance != null)
            {
                if (s_instance.Equals(this) == false)
                    Destroy(this);
            }
            else s_instance = this;

            _data = ObstacleLevelControllerData;

            if (_data == null)
            {
                Debug.LogError("There is no data for the Obstacle Controller!");
                enabled = false;
                return;
            }

            _activeObstacles = new();
            _activeObstaclesInTop = new();

            _topSpawnMarginScreen = Screen.height - ((Screen.height / 100f) * _data.TopSpawnMarginPercentage);

            InitializeControllers();
        }

        public delegate void SpawnLoopEventHandler();
        public event SpawnLoopEventHandler OnLoop;

        private ObstacleLevelControllerSO _data;

        private WaveController _waveController;

        private MissileController _missileController;
        private FallingBombController _fallingBombController;
        private LaserGateController _laserGateController;

        private List<IObstacleController> _controllers;
        public IObstacleController[] Controllers => _controllers.ToArray();

        private List<IObstacle> _activeObstacles;
        public IObstacle[] ActiveObstacles => _activeObstacles.ToArray();

        private float _topSpawnMarginScreen;
        private float _lastCheckActiveObstaclesTopMargin = 0f;
        private List<IObstacle> _activeObstaclesInTop;
        public IObstacle[] ActiveObstaclesInTopSpawnMargin
        {
            get
            {
                if (_lastCheckActiveObstaclesTopMargin != Time.time)
                {
                    _activeObstaclesInTop.Clear();
                    _lastCheckActiveObstaclesTopMargin = Time.time;

                    for (int i = 0; i < ActiveObstacles.Length; i++)
                    {
                        if (ActiveObstacles[i].BoundsScreenPosition.max.y > _topSpawnMarginScreen)
                        {
                            _activeObstaclesInTop.Add(ActiveObstacles[i]);
                        }
                    }
                }
                return _activeObstaclesInTop.ToArray();
            }
        }

        private void Update()
        {
            if (!LevelController.Started)
                return;

            if (LevelController.Ended)
                return;

            if (LevelController.Paused)
                return;

            if (ActiveObstacles.Length >= _data.MaxObstaclesAtOnce)
                return;

            if (_waveController.CurrentState.Equals(WaveState.ENDED))
            {
                if (_waveController.EndedTime != 0f && Time.time - _waveController.EndedTime < _data.WaveInterval)
                    return;

                WaveSO wave_data = null;
                if (_waveController.CurrentWave % _data.BossWaveCount == 0)
                    wave_data = _data.GetRandomBossWave();

                if (wave_data == null)
                    wave_data = _data.GetRandomNormalWave();

                _waveController.NewWave(wave_data);
                _waveController.StartWave();
            }

            OnLoop?.Invoke();
        }
    }
}