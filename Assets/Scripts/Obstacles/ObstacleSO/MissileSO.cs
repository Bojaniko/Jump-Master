using UnityEngine;

using Studio28.SFX;

using JumpMaster.UI.Data;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Missile Data", menuName = "Game/Obstacles/Missile")]
    public class MissileSO : ObstacleSO
    {
        [Range(10, 500)]
        public int GameObjectDestroyDelayMS = 150;

        [Range(0f, 10f)]
        public float DistanceFromScreenDestroy = 3f;

        public MissileWarningSO WarningInfo;

        public SoundEffectInfo ThrustSFX;
        public SoundEffectInfo ExplosionSFX;

        public GameObject ExplosionPrefab;
    }
}