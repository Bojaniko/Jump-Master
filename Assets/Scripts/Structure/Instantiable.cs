using UnityEngine;

namespace JumpMaster.Structure
{
    public abstract class Instantiable : MonoBehaviour
    {
        protected abstract void Initialize();

        public bool Initialized { get; protected set; }
    }
}