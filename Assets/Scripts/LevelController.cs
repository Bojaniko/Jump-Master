using UnityEngine;

using JumpMaster.Obstacles;
using JumpMaster.UI;

namespace JumpMaster.LevelControllers
{
    public class LevelController : MonoBehaviour
    {

        private bool _controllersInitialized;

        public static bool LevelLoaded { get { return Instance._controllersInitialized; } }
        public static bool Paused { get; private set; }

        public static float LastPauseTime { get; private set; }

        public delegate void LevelActivityEventHandler();
        public event LevelActivityEventHandler OnLevelStarted;
        public event LevelActivityEventHandler OnLevelPaused;

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

            Physics.gravity = new Vector3(0, -Gravity, 0);

            PauseButton.OnPause += () => { Paused = true; OnLevelPaused(); };
            PauseButton.OnResume += () => { OnLevelStarted(); LastPauseTime = Time.time; Paused = false; };
        }

        private void Update()
        {
            if (!_controllersInitialized)
            {
                StartLevel();
                _controllersInitialized = true;
            }
        }

        public void StartLevel()
        {
            if (OnLevelStarted != null)
                OnLevelStarted();

            Paused = false;
        }

        [Range(1f, 10f)]
        public float Gravity = 9.81f;

        public GameObject PlayerGameObject;

        public ObstacleLevelControllerSO ObstacleLevelControllerData;
    }
}