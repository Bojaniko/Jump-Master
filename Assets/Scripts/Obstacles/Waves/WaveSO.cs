using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using JumpMaster.Obstacles;

namespace JumpMaster.LevelControllers.Obstacles
{
    [CreateAssetMenu(fileName = "Wave Data", menuName = "Game/Obstacles/Controllers/Wave Data")]
    public class WaveSO : ScriptableObject
    {
        public WaveType TypeOfWave = WaveType.NORMAL;

        public ObstacleControllersSO ControllersData;
    }
}