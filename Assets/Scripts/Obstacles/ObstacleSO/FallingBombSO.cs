using UnityEngine;

using Studio28.SFX;

namespace JumpMaster.Obstacles
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "<Pending>")]
    [CreateAssetMenu(fileName = "Falling Bomb Data", menuName = "Game/Obstacles/Falling Bomb")]
    public class FallingBombSO : ObstacleSO
    {
        [Range(10, 500)] public int GameObjectDestroyDelayMS = 150;
        [Range(50, 2000)] public int DetectionShowDurationMS = 500;

        [ColorUsage(false, true)] public Color ArmedLightColor = Color.red;
        [ColorUsage(false, true)] public Color UnarmedLightColor = Color.green;

        public GameObject ExplosionPrefab;

        public SoundEffectInfo ArmingBeepSound;
        public SoundEffectInfo ExplosionSound;
    }
}