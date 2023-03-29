using UnityEngine;

namespace JumpMaster.Structure
{
    public abstract class Initializable : MonoBehaviour//, IInitializable
    {
        protected abstract void Initialize();

        public bool Initialized { get; protected set; }

        //public event InitializationEventHandler OnInitialization;
    }
}