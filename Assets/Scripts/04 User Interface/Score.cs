using System.Text;

using UnityEngine;
using UnityEngine.UI;

using JumpMaster.Core;
using JumpMaster.LevelTrackers;

namespace JumpMaster.UI
{
    [RequireComponent(typeof(Text))]
    public class Score : MonoBehaviour
    {
        private StringBuilder _textSB;

        private int _score = 0;
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

            LevelManager.OnRestart += RestartScore;
            LevelManager.OnEndLevel += RemoveText;
        }

        void Update()
        {
            if (!LevelManager.Started)
                return;

            _score = (int)Mathf.Floor(ScoreController.Instance.Score);

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