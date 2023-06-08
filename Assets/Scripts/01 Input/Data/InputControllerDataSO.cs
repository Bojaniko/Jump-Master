using UnityEngine;

namespace JumpMaster.Controls
{
    [CreateAssetMenu(fileName = "Input Controller Data", menuName = "Game/Input/Controller Data")]
    public class InputControllerDataSO : ScriptableObject
    {
        public HoldProcessorDataSO HoldData => _holdData;
        [SerializeField] HoldProcessorDataSO _holdData;

        public SwipeProcessorDataSO SwipeData => _swipeData;
        [SerializeField] SwipeProcessorDataSO _swipeData;

        public TapProcessorDataSO TapData => _tapData;
        [SerializeField] TapProcessorDataSO _tapData;
    }
}