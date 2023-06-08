using UnityEngine;

namespace JumpMaster.Core
{
    /// <summary>
    /// A level controller base class inheriting from MonoBehaviour with a IInitializable contract.
    /// The LevelManager class controls all objects that inherit from LevelController.
    /// </summary>
    public abstract class LevelController : MonoBehaviour, IInitializable
    {
        public bool Initialized => _initialized;
        private bool _initialized;

        /// <summary>
        /// Called on Awake. Do not call Awake method.
        /// </summary>
        protected abstract void Initialize();

        private void Awake()
        {
            if (_initialized)
                return;

            Initialize();

            _initialized = true;
        }
    }
}