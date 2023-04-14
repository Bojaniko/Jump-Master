using UnityEngine;

using JumpMaster.LevelControllers;
using JumpMaster.Controls;

namespace JumpMaster.Testing
{
    public class StartLevelTester : MonoBehaviour
    {
        private bool _enabledTest = false;

        void Update()
        {
            if (!LevelController.Loaded)
                return;

            if (_enabledTest)
                return;

            InputController.Instance.OnTap += Instance_OnTap;

            _enabledTest = true;
        }

        private void Instance_OnTap()
        {
            LevelController.StartLevel();

            InputController.Instance.OnTap -= Instance_OnTap;

            enabled = false;
        }
    }
}