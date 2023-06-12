using UnityEngine;

using JumpMaster.CameraControls;

namespace JumpMaster.Obstacles
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "<Pending>")]
    [CreateAssetMenu(fileName = "Obstacle Controller Data", menuName = "Obstacle Controller")]
    public class ObstacleLevelControllerSO : ScriptableObject
    {
        public Vector2[] EdgePointsTop => _pointsTop;
        private Vector2[] _pointsTop;

        public Vector2[] EdgePointsBottom => _pointsBottom;
        private Vector2[] _pointsBottom;

        public Vector2[] EdgePointsLeft => _pointsLeft;
        private Vector2[] _pointsLeft;

        public Vector2[] EdgePointsRight => _pointsRight;
        private Vector2[] _pointsRight;

        /// <summary>
        /// The default wave data for initializing the obstacle controllers.
        /// </summary>
        public WaveSO DefaultWaveData => _defaultWaveData;
        [SerializeField, Tooltip("The default wave data for initializing the obstacle controllers. Must contain spawn data for all the controllers.")] private WaveSO _defaultWaveData;

        /// <summary>
        /// The amount of points on the vertical edge of the screen.
        /// </summary>
        public int VerticalEdgePoints => _verticalEdgePoints;
        [SerializeField, Range(1, 20), Tooltip("The amount of points on the vertical edge of the screen.")] private int _verticalEdgePoints = 10;

        /// <summary>
        /// The amount of points on the horizontal edge of the screen.
        /// </summary>
        public int HorizontalEdgePoints => _horizontalEdgePoints;
        [SerializeField, Range(1, 20), Tooltip("The amount of points on the horizontal edge of the screen.")] private int _horizontalEdgePoints = 3;

        [Header("Waves")]

        /// <summary>
        /// The interval, in seconds, between any two waves.
        /// </summary>
        [Range(1f, 50f), Tooltip("The interval, in seconds, between any two waves.")] public float WaveInterval = 10f;

        /// <summary>
        /// The interval, in waves, at which a boss battle starts.
        /// </summary>
        [Range(1, 20), Tooltip("The interval, in waves, at which a boss battle starts.")] public int BossWaveCount = 3;

        /// <summary>
        /// The maximum amount of spawned obstacles at once.
        /// </summary>
        [Range(1, 30), Tooltip("The maximum amount of spawned obstacles at once.")] public int MaxObstaclesAtOnce = 10;

        public WaveSO[] NormalWaveData;
        public WaveSO[] BossWaveData;

        public WaveSO GetRandomBossWave()
        {
            if (BossWaveData.Length == 0)
                return null;
            return BossWaveData[Random.Range(0, BossWaveData.Length)];
        }

        public WaveSO GetRandomNormalWave()
        {
            if (NormalWaveData.Length == 0)
                return null;
            return NormalWaveData[Random.Range(0, NormalWaveData.Length)];
        }

        private void Awake()
        {
            if (!Application.isPlaying)
                return;
            CalculateEdgePoints();
        }

        private void CalculateEdgePoints()
        {
            _verticalEdgePoints = (int)Mathf.Min(CameraController.Instance.Data.ResolutionAspectChange.y * _verticalEdgePoints);
            _horizontalEdgePoints = (int)Mathf.Min(CameraController.Instance.Data.ResolutionAspectChange.x * _horizontalEdgePoints);

            _pointsTop = new Vector2[_horizontalEdgePoints];
            _pointsBottom = new Vector2[_horizontalEdgePoints];
            for (int i = 0; i < _horizontalEdgePoints; i++)
            {
                _pointsTop[i] = new Vector2((Screen.width / _horizontalEdgePoints) * (i + 1), Screen.height);
                _pointsBottom[i] = new Vector2((Screen.width / _horizontalEdgePoints) * (i + 1), 0f);
            }

            _pointsLeft = new Vector2[_verticalEdgePoints];
            _pointsRight = new Vector2[_verticalEdgePoints];
            for (int k = 0; k < _verticalEdgePoints; k++)
            {
                _pointsLeft[k] = new Vector2(0, (Screen.height / _verticalEdgePoints) * (k + 1));
                _pointsRight[k] = new Vector2(Screen.width, (Screen.height / _verticalEdgePoints) * (k + 1));
            }
        }
    }
}