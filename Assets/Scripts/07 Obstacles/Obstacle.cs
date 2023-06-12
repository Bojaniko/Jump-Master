using UnityEngine;

using JumpMaster.Core;
using JumpMaster.Core.Physics;
using JumpMaster.CameraControls;

namespace JumpMaster.Obstacles
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public abstract class Obstacle<ObstacleScriptableObject, SpawnScriptableObject> : MonoBehaviour, IInitializable, IObstacle
        where ObstacleScriptableObject : ObstacleSO
        where SpawnScriptableObject : SpawnSO
    {
        public ObstacleScriptableObject Data { get; private set; }

        public void Generate(ObstacleSO data)
        {
            if (_initialized)
                return;

            if (!data.GetType().Equals(typeof(ObstacleScriptableObject)))
                return;

            Data = (ObstacleScriptableObject)data;

            InitializeBase();
            Initialize();

            _initialized = true;

            gameObject.SetActive(false);
        }

        private void Awake()
        {
            if (!Initialized)
                gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!Spawned)
                return;

            OnUpdate();

            if (IsDespawnable())
                Despawn();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate();

            CalculateMarginPosition();
        }

        private void InitializeBase()
        {
            LevelManager.OnRestart += Restart;

            Cache();

            transform.localScale = Vector3.one * Data.Scale;
            transform.position = new Vector3(0, 0, Data.Z_Position);
        }

        private void Restart()
        {
            Despawn();
        }

        protected abstract void SpawnInstructions();
        protected abstract void DespawnInstructions();
        protected abstract void OnUpdate();
        protected abstract void OnFixedUpdate();
        protected abstract bool IsDespawnable();

        public bool Initialized => _initialized;
        private bool _initialized;
        protected abstract void Initialize();

        // ##### EVENTS ##### \\

        public event ObstacleStateEventHandler OnSpawn;
        public event ObstacleStateEventHandler OnDespawn;
        public event ObstacleStateEventHandler OnMarginPositionChange;

        // ##### SPAWNING ##### \\
        public bool Spawned { get; private set; }
        public SpawnArgs SpawnArguments { get; private set; }
        public SpawnScriptableObject SpawnData { get; private set; }

        public void Spawn(SpawnSO spawn_data, SpawnArgs spawn_args)
        {
            if (Spawned)
                return;

            if (!spawn_data.GetType().Equals(typeof(SpawnScriptableObject)))
                return;

            Spawn((SpawnScriptableObject)spawn_data, spawn_args);
        }
        private void Spawn(SpawnScriptableObject spawn_data, SpawnArgs spawn_args)
        {
            SpawnData = spawn_data;
            SpawnArguments = spawn_args;

            _topMargin = false;
            _bottomMargin = false;
            _leftMargin = false;
            _rightMargin = false;

            SpawnInstructions();

            gameObject.SetActive(true);

            Spawned = true;

            OnSpawn?.Invoke(this);
        }

        public void Despawn()
        {
            if (!Spawned)
                return;

            DespawnInstructions();

            gameObject.SetActive(false);

            Spawned = false;

            OnDespawn?.Invoke(this);

            SpawnData = null;
            SpawnArguments = null;
        }

        // ##### MARGINS ##### \\

        protected virtual void CalculateMarginPosition()
        {
            bool changed = false;

            bool topMargin = CameraController.IsPositionInTopMargin(Bounding.ScreenPosition);
            if (_topMargin != topMargin)
            {
                _topMargin = topMargin;
                changed = true;
            }

            bool bottomMargin = CameraController.IsPositionInBottomMargin(Bounding.ScreenPosition);
            if (_bottomMargin != bottomMargin)
            {
                _bottomMargin = bottomMargin;
                changed = true;
            }

            bool leftMargin = CameraController.IsPositionInLeftMargin(Bounding.ScreenPosition);
            if (_leftMargin != leftMargin)
            {
                _leftMargin = leftMargin;
                changed = true;
            }

            bool rightMargin = CameraController.IsPositionInRightMargin(Bounding.ScreenPosition);
            if (_rightMargin != rightMargin)
            {
                _rightMargin = rightMargin;
                changed = true;
            }

            if (changed)
                OnMarginPositionChange(this);
        }

        private bool _topMargin;
        private bool _bottomMargin;
        private bool _leftMargin;
        private bool _rightMargin;

        public bool InTopMargin => _topMargin;
        public bool InBottomMargin => _bottomMargin;
        public bool InLeftMargin => _leftMargin;
        public bool InRightMargin => _rightMargin;

        // ##### SCREEN POSITION ##### \\

        public Vector2 ScreenPosition => Bounding.ScreenPosition;
        public bool BoundsUnderScreen => (Bounding.ScreenMax.y < 0);
        public bool BoundsOverScreen => (Bounding.ScreenMin.y > Screen.height);
        public bool BoundsLeftOfScreen => (Bounding.ScreenMax.x < 0);
        public bool BoundsRightOfScreen => (Bounding.ScreenMin.x > Screen.width);

        #region Type data
        public System.Type ObstacleDataType => typeof(ObstacleScriptableObject);
        public System.Type SpawnDataType => typeof(SpawnScriptableObject);
        #endregion

        #region Cached data
        protected float c_invertedScale { get; private set; }

        public BoundingBox Bounding => c_bounding;
        protected BoundingBox c_bounding { get; private set; }

        protected Camera c_camera { get; private set; }
        protected Animator c_animator { get; private set; }
        protected Rigidbody2D c_rigidbody { get; private set; }

        private void Cache()
        {
            c_invertedScale = 1f / Data.Scale;

            c_camera = Camera.main;
            c_animator = GetComponent<Animator>();
            c_bounding = GetComponent<BoundingBox>();
            c_rigidbody = GetComponent<Rigidbody2D>();

            for (int i = 0; i < transform.childCount; i++)
            {
                c_bounding = transform.GetChild(i).GetComponent<BoundingBox>();
            }
        }
        #endregion
    }
}