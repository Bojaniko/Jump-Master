using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using JumpMaster.Obstacles;

namespace JumpMaster.LevelControllers.Obstacles
{
    public class ObstacleLevelController : MonoBehaviour
    {
        public delegate void SpawnLoopEventHandler();
        public event SpawnLoopEventHandler OnLoop;

        private bool _initialized = false;

        private float _spawnCheckInterval;
        private Coroutine _controllersCoroutine;

        private ObstacleLevelControllerSO _data;

        private WaveController _waveController;

        private MissileController _missileController;
        private FallingBombController _fallingBombController;
        private LaserGateController _laserGateController;

        private float _lastCheckActiveObstacles = 0f;
        private List<Obstacle> _activeObstacles;
        public Obstacle[] ActiveObstacles
        {
            get
            {
                if (_lastCheckActiveObstacles != Time.time)
                {
                    _activeObstacles.Clear();

                    for (int i = 0; i < _waveController.Controllers.Length; i++)
                    {
                        if (_waveController.Controllers[i].Self.ControllerData.ActiveObstacles.Length > 0)
                            _activeObstacles.AddRange(_waveController.Controllers[i].Self.ControllerData.ActiveObstacles);

                    }
                    /*if (_missileController.ControllerData.Pool.ActiveObstaclesControlled > 0)
                        _activeObstacles.AddRange(_missileController.ControllerData.Pool.ActiveObstacles);
                    if (_fallingBombController.ControllerData.Pool.ActiveObstaclesControlled > 0)
                        _activeObstacles.AddRange(_fallingBombController.ControllerData.Pool.ActiveObstacles);
                    if (_laserGateController.ControllerData.Pool.ActiveObstaclesControlled > 0)
                        _activeObstacles.AddRange(_laserGateController.ControllerData.Pool.ActiveObstacles);*/

                    _lastCheckActiveObstacles = Time.time;
                }

                return _activeObstacles.ToArray();
            }
        }

        private float _topSpawnMarginScreen;
        private float _lastCheckActiveObstaclesTopMargin = 0f;
        private List<Obstacle> _activeObstaclesInTop;
        public Obstacle[] ActiveObstaclesInTopSpawnMargin
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

        private void Awake()
        {
            if (s_instance != null)
            {
                if (s_instance.Equals(this) == false)
                    Destroy(this);
            } else s_instance = this;

            _data = Instantiate(LevelController.Instance.ObstacleLevelControllerData);

            if (_data == null)
            {
                Debug.LogError("There is no data for the Obstacle Controller!");
                enabled = false;
                return;
            }

            _activeObstacles = new();
            _activeObstaclesInTop = new();

            _topSpawnMarginScreen = Screen.height - ((Screen.height / 100f) * _data.TopSpawnMarginPercentage);
            _spawnCheckInterval = _data.SpawnCheckInterval / 1000f;

            InitializeControllers();

            LevelController.Instance.OnLevelStarted += StartControllers;
            LevelController.Instance.OnLevelPaused += PauseControllers;

            _initialized = true;
        }

        private void InitializeControllers()
        {

            if (_missileController == null)
                _missileController = new(_data.DefaultControllersData.Missile);

            if (_fallingBombController == null)
                _fallingBombController = new(_data.DefaultControllersData.FallingBomb);

            if (_laserGateController == null)
                _laserGateController = new(_data.DefaultControllersData.LaserGate);

            if (_waveController == null)
            {
                List<ObstacleController> controllers = new();

                controllers.Add(_missileController);
                controllers.Add(_fallingBombController);
                controllers.Add(_laserGateController);

                _waveController = new(controllers);

                _waveController.NewWave(_data.GetRandomNormalWave());
            }
        }

        private void StartControllers()
        {
            _controllersCoroutine = StartCoroutine(ControllersLoop());
        }

        private void PauseControllers()
        {
            StopCoroutine(_controllersCoroutine);
        }

        private IEnumerator ControllersLoop()
        {
            yield return new WaitForSeconds(_spawnCheckInterval);

            if (ActiveObstacles.Length >= _data.MaxObstaclesAtOnce)
                yield return ControllersLoop();

            if (_waveController.CurrentState.Equals(WaveState.ENDED))
            {
                if (_waveController.StartedTime == 0f)
                    yield return new WaitForSeconds(_data.FirstWaveInterval);
                else
                    yield return new WaitForSeconds(_data.WaveInterval);

                WaveSO wave_data = null;
                if (_waveController.CurrentWave % _data.BossWaveCount == 0)
                    wave_data = _data.GetRandomBossWave();

                if (wave_data == null)
                    wave_data = _data.GetRandomNormalWave();

                _waveController.NewWave(wave_data);
                _waveController.StartWave();
            }

            if (OnLoop != null)
                OnLoop();

            yield return ControllersLoop();
        }
    }
}