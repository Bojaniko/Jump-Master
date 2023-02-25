using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpMaster.LevelControllers
{
    public class ScoreController : MonoBehaviour
    {
        private float _startHeight;

        public float Score { get; private set; } = 0f;

        private LevelController _levelController;

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

        private void Awake()
        {
            Instance = this;

            _levelController = LevelController.Instance;

            _startHeight = _levelController.PlayerGameObject.transform.position.y;
        }

        private void Update()
        {
            if (_levelController.PlayerGameObject.transform.position.y - _startHeight > Score) Score = _levelController.PlayerGameObject.transform.position.y - _startHeight;
        }
    }
}