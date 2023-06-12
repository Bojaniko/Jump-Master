using UnityEngine;

namespace JumpMaster.Movement
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "<Pending>")]
    [CreateAssetMenu(fileName = "Bounce Control Data", menuName = "Game/Movement/Bounce Data")]
    public class BounceControlDataSO : MovementControlDataSO
    {
        [Range(1f, 100f)]public float BounceForce = 10f;
        [Range(0.1f, 10f)] public float BounceDistance = 3f;

        [Range(0.0f, 1.0f)] public float MinStrengthPercentage = 0.2f;
        [Range(0.1f, 1.0f)] public float MinExitDistancePercentage = 1f;
    }
}