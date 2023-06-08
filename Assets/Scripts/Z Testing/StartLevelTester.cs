using UnityEngine;

using JumpMaster.Core;
using JumpMaster.Controls;

namespace JumpMaster.Testing
{
    public class StartLevelTester : MonoBehaviour
    {
        private void Awake() =>
            LevelManager.OnLoad += LevelLoaded;

        private void LevelLoaded() =>
            InputController.Instance.RegisterInputPerformedListener<TapProcessor>(StartLevel, this);

        private void StartLevel(IInputPerformedEventArgs args)
        {
            LevelManager.StartLevel();
            InputController.Instance.UnregisterInputPerformedListener<TapProcessor>(this);
        }
    }
}