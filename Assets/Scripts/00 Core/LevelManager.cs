using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace JumpMaster.Core
{
    public class LevelManager : MonoBehaviour
    {
        [Range(1f, 10f)]
        public float Gravity = 9.81f;

        private static bool s_resumed = false;

        public static bool Loaded { get; private set; }
        public static bool Started { get; private set; }
        public static bool Ended { get; private set; }
        public static bool Paused { get; private set; }
        public static float LastPauseStartTime { get; private set; }
        public static float LastPauseEndTime { get; private set; }
        public static float LastPauseDuration { get; private set; }

        private List<LevelController> _controllers;

        public delegate void LevelActivityEventHandler();

        public static event LevelActivityEventHandler OnLoad;
        public static event LevelActivityEventHandler OnPause;
        public static event LevelActivityEventHandler OnResume;
        public static event LevelActivityEventHandler OnRestart;
        public static event LevelActivityEventHandler OnEndLevel;
        public static event LevelActivityEventHandler OnStartLevel;

        private static LevelManager s_instance;

        public static LevelManager Instance
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
            Loaded = false;
            Ended = false;
            Started = false;

            StartCoroutine(LevelLoadSequence());

            Physics2D.gravity = new Vector3(0, -Gravity, 0);
        }

        private void LateUpdate()
        {
            if (s_resumed)
            {
                OnResume?.Invoke();
                s_resumed = false;
            }
        }

        public static void StartLevel()
        {
            if (!Loaded)
                return;

            if (Started)
                return;

            Started = true;
            Paused = false;

            OnStartLevel?.Invoke();
        }

        public static void EndLevel()
        {
            if (!Loaded)
                return;

            if (!Started)
                return;

            if (Ended)
                return;

            Ended = true;

            OnEndLevel?.Invoke();
        }

        public static void Restart()
        {
            if (!Loaded)
                return;

            if (!Started)
                return;

            Paused = false;
            Started = false;
            Ended = false;

            OnRestart?.Invoke();
        }

        public static void Pause()
        {
            if (!Loaded)
                return;

            if (Paused)
                return;

            LastPauseStartTime = Time.time;

            Paused = true;

            OnPause?.Invoke();
        }

        public static void Resume()
        {
            if (!Loaded)
                return;

            if (!Paused)
                return;

            LastPauseEndTime = Time.time;
            LastPauseDuration = LastPauseEndTime - LastPauseStartTime;

            Paused = false;

            s_resumed = true;
        }

        private IEnumerator LevelLoadSequence()
        {
            _controllers = new();
            _controllers.AddRange(FindObjectsOfType<LevelController>());

            for (int i = 0; i < _controllers.Count; i++)
            {
                yield return new WaitUntil(() =>
                {
                    return _controllers[i].Initialized;
                });
            }

            Loaded = true;
            Paused = false;

            if (OnLoad != null)
                OnLoad();
        }
    }
}