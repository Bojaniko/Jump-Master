using UnityEngine;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Wave Data", menuName = "Game/Obstacles/Controllers/Wave Data")]
    public class WaveSO : ScriptableObject
    {
        public WaveType TypeOfWave => _typeOfWave;
        [SerializeField] private WaveType _typeOfWave = WaveType.NORMAL;

        public ObstacleControllersSO ControllersData => _controllersData;
        [SerializeField] private ObstacleControllersSO _controllersData;
    }
}