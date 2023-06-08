using UnityEngine;
using UnityEngine.UIElements;

using JumpMaster.Core;
using JumpMaster.LevelTrackers;

namespace JumpMaster.UI
{
    public class EndMenu : UIMenu
    {
        private void Start()
        {
            LevelManager.OnEndLevel += DisplayScore;
        }

        private void DisplayScore()
        {
            gameObject.GetComponent<UIDocument>().rootVisualElement.Q<Label>("score").text = $"SCORE : {((int)Mathf.Floor(ScoreController.Instance.Score)).ToString("00000")}";
        }
    }
}