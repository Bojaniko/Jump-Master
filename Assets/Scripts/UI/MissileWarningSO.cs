using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpMaster.UI.Data
{
    [CreateAssetMenu(fileName = "Missile Warning Data", menuName = "Game/UI/Missile Warning")]
    public class MissileWarningSO : ScriptableObject
    {
        public GameObject Prefab;

        [Range(50, 1000)]
        public int FlashIntervalMS = 200;

        [Range(0f, 200f)]
        public float ScreenMargin = 10f;
    }
}