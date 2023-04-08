using System.Text;

using UnityEngine;
using UnityEngine.UI;

using JumpMaster.LevelTrackers;
using JumpMaster.LevelControllers;

namespace JumpMaster.UI
{
    [RequireComponent(typeof(Text))]
    public class Score : MonoBehaviour
    {
        private StringBuilder _textSB;

        private int _score = 0;
        private int _previousScore = 0;
        private Text _text;

        private void Awake()
        {
            _text = GetComponent<Text>();
            if (_text == null)
            {
                gameObject.SetActive(false);
                return;
            }

            _textSB = new();
            _textSB.Append("Jump!");
            _text.text = _textSB.ToString();

            LevelController.OnRestart += RestartScore;
            LevelController.OnEndLevel += RemoveText;
        }

        void Update()
        {
            if (!LevelController.Started)
                return;

            _score = (int)Mathf.Floor(ScoreController.Instance.Score);

            if (_score == _previousScore)
                return;

            _previousScore = _score;
            UpdateText();
        }

        private void RemoveText()
        {
            _textSB.Clear();
            _text.text = _textSB.ToString();
        }

        private void RestartScore()
        {
            _score = 0;
            _previousScore = 0;

            _textSB.Clear();
            _textSB.Append("Jump!");
            _text.text = _textSB.ToString();
        }

        private void UpdateText()
        {
            _textSB.Clear();
            _textSB.Append($"HEIGHT {_score.ToString("00000")}");
            _text.text = _textSB.ToString();
        }
    }
}