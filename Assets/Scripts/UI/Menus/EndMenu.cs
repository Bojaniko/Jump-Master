using UnityEngine;
using UnityEngine.UIElements;

using JumpMaster.LevelControllers;
using JumpMaster.LevelTrackers;

namespace JumpMaster.UI
{
    public class EndMenu : UIMenu
    {
        private void Start()
        {
            LevelController.OnEndLevel += DisplayScore;
        }

        private void DisplayScore()
        {
            gameObject.GetComponent<UIDocument>().rootVisualElement.Q<Label>("score").text = $"SCORE : {((int)Mathf.Floor(ScoreController.Instance.Score)).ToString("00000")}";
        }
    }
}