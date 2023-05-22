using UnityEngine;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Wave Data", menuName = "Game/Obstacles/Controllers/Wave Data")]
    public class WaveSO : ScriptableObject
    {
        public WaveType TypeOfWave = WaveType.NORMAL;

        public ObstacleControllersSO ControllersData;
    }
}