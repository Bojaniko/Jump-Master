using UnityEngine;

using Studio28.Audio.SFX;

using JumpMaster.UI.Data;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Missile Data", menuName = "Game/Obstacles/Data/Missile")]
    public class MissileSO : ObstacleSO
    {
        [Range(10, 500)]
        public int GameObjectDestroyDelayMS = 150;

        [Range(0f, 10f)]
        public float DistanceFromScreenDestroy = 3f;

        public MissileWarningSO WarningData;

        public MinMaxSoundEffectInfo ThrustSFX;
        public RandomizedSoundEffectInfo ExplosionSFX;

        public GameObject ExplosionPrefab;
    }
}