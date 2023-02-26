using UnityEngine;

namespace JumpMaster.Structure
{
    public abstract class Initializable : MonoBehaviour
    {
        protected abstract void Initialize();

        public bool Initialized { get; protected set; }
    }
}