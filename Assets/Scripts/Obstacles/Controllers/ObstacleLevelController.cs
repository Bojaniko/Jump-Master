using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using JumpMaster.Core;
using JumpMaster.CameraControls;

using Studio28.Probability;

namespace JumpMaster.Obstacles
{
    public class ObstacleLevelController : LevelController
    {
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

            _allObstacles = new();
            _activeObstacles = new();

            InitializeControllers();

            _margins = new(_allObstacles);
            _spawnPoints = new(_data.HorizontalEdgePoints, _data.VerticalEdgePoints, CameraController.Instance.Data.ResolutionAspectChange);
        }

        public static ObstacleSpawnPointTracker SpawnPoints => Instance._spawnPoints;
        private ObstacleSpawnPointTracker _spawnPoints;

        public static ObstacleMarginTracker Margins => Instance._margins;
        private ObstacleMarginTracker _margins;

        public ObstacleLevelControllerSO ObstacleLevelControllerData;
        private ObstacleLevelControllerSO _data;

        private void Update()
        {
            if (!LevelManager.Started)
                return;

            if (LevelManager.Ended)
                return;

            if (LevelManager.Paused)
                return;

            TryStartNewWave();

            if (ActiveObstacles.Length >= _data.MaxObstaclesAtOnce)
                return;

            IObstacleController probed_controller = _controllerProbability.Outcome();
            probed_controller.TrySpawn();

            if (WaveEnded())
                _waveController.EndWave();
        }

        // ##### WAVE ##### \\

        private bool WaveEnded()
        {
            if (!_waveController.Started)
                return true;
            if (ActiveObstacles.Length > 0)
                return false;
            foreach (IObstacleController controller in _controllers)
            {
                if (controller.SpawnsLeft > 0)
                    return false;
            }
            return true;
        }

        private void TryStartNewWave()
        {
            if (_waveController.Started)
                return;

            if (Time.time - _waveController.EndedTime < _data.WaveInterval
                && _waveController.EndedTime != 0f)
                return;

            WaveSO wave_data = null;
            if (_waveController.CurrentWave % _data.BossWaveCount == 0)
                wave_data = _data.GetRandomBossWave();

            if (wave_data == null)
                wave_data = _data.GetRandomNormalWave();

            _waveController.NewWave(wave_data, _controllers.ToArray());
            _waveController.StartWave();

            CalculateAllObstacles();
            GenerateControllerProbabilities();

            _margins.UpdateObstacles(_allObstacles);
        }

        // ##### PROBABILITY ##### \\

        private Weighted<IObstacleController> _controllerProbability;

        private void GenerateControllerProbabilities()
        {
            Dictionary<IObstacleController, int> probabilities = new();
            foreach (IObstacleController controller in _controllers)
            {
                probabilities.Add(controller, controller.SpawnMetrics.SpawnWeight);
            }
            _controllerProbability = new(probabilities);
        }

        // ##### ACTIVE OBSTACLES ##### \\

        private List<IObstacle> _activeObstacles;
        public IObstacle[] ActiveObstacles => _activeObstacles.ToArray();

        private void BindControllerActiveCount()
        {
            foreach (IObstacleController controller in _controllers)
            {
                controller.OnActiveObstaclesChange += CalculateActiveObstacles;
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

        // ##### ALL OBSTACLES ##### \\

        private List<IObstacle> _allObstacles;
        public IObstacle[] AllObstacles => _allObstacles.ToArray();

        private void CalculateAllObstacles()
        {
            _allObstacles.Clear();
            foreach (IObstacleController controller in _controllers)
            {
                _allObstacles.AddRange(controller.AllObstacles);
            }
        }

        // ##### CONTROLLERS ##### \\

        private WaveController _waveController;

        private List<System.Type> _controllerTypes;

        private List<IObstacleController> _controllers;
        public IObstacleController[] Controllers => _controllers.ToArray();

        private void InitializeControllers()
        {
            GetControllerTypes();

            GenerateControllers();

            BindControllerActiveCount();

            _waveController = new();
            _waveController.NewWave(_data.DefaultWaveData, _controllers.ToArray());

            CalculateAllObstacles();
        }

        private void GenerateControllers()
        {
            _controllers = new();
            foreach (System.Type ct in _controllerTypes)
            {
                IObstacleController controller = (IObstacleController)System.Activator.CreateInstance(ct, _data.DefaultWaveData.ControllersData.GetSpawnMetricsForControllerType(ct));
                _controllers.Add(controller);
            }
        }

        private void GetControllerTypes()
        {
            _controllerTypes = new();
            foreach (Assembly asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetName().Name.Equals("JumpMaster.Obstacles"))
                {
                    foreach (System.Type t in asm.GetTypes())
                    {
                        if (t.GetInterface("IObstacleController") != null && !t.IsAbstract)
                            _controllerTypes.Add(t);
                    }
                    return;
                }
            }
        }
    }
}