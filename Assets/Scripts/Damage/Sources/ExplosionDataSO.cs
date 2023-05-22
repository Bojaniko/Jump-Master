using UnityEngine;

namespace JumpMaster.Damage
{
    [CreateAssetMenu(fileName = "Explosion Data", menuName = "Game/Damage/Explosion Data")]
    public class ExplosionDataSO : ScriptableObject
    {
        [Range(0f, 100f)] public float Damage = 30f;

        [Range(0.1f, 20f)] public float Radius = 10f;

        [Range(0.1f, 10f)] public float Duration = 1.5f;
    }
}