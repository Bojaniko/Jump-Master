using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpMaster.LevelControllers
{
    public class ScoreController : LevelControllerInitializablePausable
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

            _startHeight = PlayerController.Instance.transform.position.y;
        }

        protected override void Pause()
        {

        }

        protected override void Unpause()
        {

        }

        protected override void PlayerDeath()
        {

        }

        protected override void Restart()
        {
            _startHeight = PlayerController.Instance.transform.position.y;
        }

        protected override void LevelLoaded()
        {

        }

        private void Update()
        {
            if (PlayerController.Instance.transform.position.y - _startHeight > Score) Score = PlayerController.Instance.transform.position.y - _startHeight;
        }
    }
}