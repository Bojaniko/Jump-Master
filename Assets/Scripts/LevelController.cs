using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using JumpMaster.Obstacles;
using JumpMaster.UI;

namespace JumpMaster.LevelControllers
{
    public class LevelController : MonoBehaviour
    {
        [Range(1f, 10f)]
        public float Gravity = 9.81f;
        public ObstacleLevelControllerSO ObstacleLevelControllerData;

        public static bool Loaded { get; private set; }
        public static bool Started { get; private set; }
        public static bool Paused { get; private set; }
        public static float LastPauseStartTime { get; private set; }
        public static float LastPauseEndTime { get; private set; }
        public static float LastPauseDuration { get; private set; }

        private List<ILevelController> _controllers;

        public delegate void LevelActivityEventHandler();
        public event LevelActivityEventHandler OnLevelStarted;
        public event LevelActivityEventHandler OnLevelLoaded;
        public event LevelActivityEventHandler OnLevelResume;
        public event LevelActivityEventHandler OnLevelPaused;
        public event LevelActivityEventHandler OnPlayerDeath;
        public event LevelActivityEventHandler OnLevelReset;

        private static LevelController s_instance;

        public static LevelController Instance
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
                    Debug.LogError("There can only be one Level Controller in the scene!");
            }
        }

        private void Awake()
        {
            Instance = this;

            Paused = true;
            Started = false;

            _controllers = new();
            _controllers.AddRange(FindObjectsOfType<LevelControllerInitializable>());
            _controllers.AddRange(FindObjectsOfType<LevelControllerInitializablePausable>());

            StartCoroutine(InitializationLoop());

            Physics.gravity = new Vector3(0, -Gravity, 0);

            PauseButton.OnPause += PauseLevel;
            ResumeButton.OnResume += ResumeLevel;
            RestartButton.OnRestart += ResetLevel;
        }

        private void LoadedLevel()
        {
            if (OnLevelLoaded != null)
                OnLevelLoaded();

            Loaded = true;
            Paused = true;
            Started = false;

            if (MovementController.Instance != null)
                MovementController.Instance.StateController.OnStateChanged += DetectLevelStart;

            ResumeLevel();
        }

        private void StartLevel()
        {
            if (OnLevelStarted != null)
                OnLevelStarted();

            Started = true;
            Paused = false;
        }

        private void ResetLevel()
        {
            Started = false;
            Paused = true;

            if (OnLevelReset != null)
                OnLevelReset();

            if (MovementController.Instance != null)
                MovementController.Instance.StateController.OnStateChanged += DetectLevelStart;
        }

        private void PauseLevel()
        {
            Paused = true;

            if (OnLevelPaused != null)
                OnLevelPaused();

            LastPauseStartTime = Time.time;
        }

        private void ResumeLevel()
        {
            if (OnLevelResume != null)
                OnLevelResume();

            Paused = false;
            LastPauseEndTime = Time.time;
            LastPauseDuration = LastPauseEndTime - LastPauseStartTime;
        }

        private IEnumerator InitializationLoop()
        {
            for (int i = 0; i < _controllers.Count; i++)
            {
                yield return new WaitUntil(() =>
                {
                    return _controllers[i].ControllerInitialized;
                });
            }

            LoadedLevel();
        }

        private void DetectLevelStart(MovementState state)
        {
            if (!state.Equals(MovementState.JUMPING))
                return;

            if (Started)
                return;

            StartLevel();

            MovementController.Instance.StateController.OnStateChanged -= DetectLevelStart;
        }
    }
}