using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    public class Score : MonoBehaviour
    {
        private int _score = 0;
        private Text _text;
        private ScoreController _scoreController;

        private void Awake()
        {
            _scoreController = ScoreController.Instance;

            _text = GetComponent<Text>();
        }

        void Update()
        {
            _score = (int)Mathf.Floor(_scoreController.Score);

            _text.text = "HEIGHT " + _score.ToString("00000");
        }
    }
}