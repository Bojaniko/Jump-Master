using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using JumpMaster.Controls;

namespace JumpMaster.LevelControllers
{
    public class LevelController : MonoBehaviour
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

        private List<LevelControllerInitializable> _controllers;

        public delegate void LevelActivityEventHandler();

        public static event LevelActivityEventHandler OnLoad;
        public static event LevelActivityEventHandler OnPause;
        public static event LevelActivityEventHandler OnResume;
        public static event LevelActivityEventHandler OnRestart;
        public static event LevelActivityEventHandler OnEndLevel;
        public static event LevelActivityEventHandler OnStartLevel;

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
            Loaded = false;
            Ended = false;
            Started = false;

            StartCoroutine(LevelLoadSequence());

            Physics.gravity = new Vector3(0, -Gravity, 0);
        }

        private void LateUpdate()
        {
            if (s_resumed)
            {
                if (OnResume != null)
                    OnResume();
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

            if (OnStartLevel != null)
                OnStartLevel();
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

            if (OnEndLevel != null)
                OnEndLevel();
        }

        public static void Restart()
        {
            if (!Loaded)
                return;

            if (!Started)
                return;

            Paused = false;
            Started = false;

            if (OnRestart != null)
                OnRestart();
        }

        public static void Pause()
        {
            if (!Loaded)
                return;

            if (Paused)
                return;

            LastPauseStartTime = Time.time;

            Paused = true;

            if (OnPause != null)
                OnPause();
        }

        public static void Resume()
        {
            if (!Loaded)
                return;

            if (!Paused)
                return;

            Debug.Log("Resuming");

            LastPauseEndTime = Time.time;
            LastPauseDuration = LastPauseEndTime - LastPauseStartTime;

            Paused = false;

            s_resumed = true;
        }

        private IEnumerator LevelLoadSequence()
        {
            _controllers = new();
            _controllers.AddRange(FindObjectsOfType<LevelControllerInitializable>());

            for (int i = 0; i < _controllers.Count; i++)
            {
                yield return new WaitUntil(() =>
                {
                    return _controllers[i].ControllerInitialized;
                });
            }

            Loaded = true;
            Paused = false;

            if (OnLoad != null)
                OnLoad();
        }
    }
}