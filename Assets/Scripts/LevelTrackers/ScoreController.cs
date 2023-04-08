using UnityEngine;

using JumpMaster.Movement;
using JumpMaster.LevelControllers;

namespace JumpMaster.LevelTrackers
{
    public class ScoreController : LevelControllerInitializable
    {
        private float _startHeight;

        public float Score { get; private set; } = 0f;

        private static ScoreController s_instance;

        public static ScoreController Instance
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
                    Debug.LogError("You can have only one instance of a Score Controller!");
            }
        }

        protected override void Initialize()
        {
            Instance = this;

            ResetScore();
            LevelController.OnRestart += ResetScore;
        }

        private void ResetScore()
        {
            Score = 0;
            _startHeight = MovementController.Instance.transform.position.y;
        }

        private void Update()
        {
            if (!LevelController.Started)
                return;

            if (MovementController.Instance.transform.position.y - _startHeight > Score) Score = MovementController.Instance.transform.position.y - _startHeight;
        }
    }
}