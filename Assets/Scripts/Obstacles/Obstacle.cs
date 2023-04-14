using UnityEngine;

using JumpMaster.Movement;
using JumpMaster.Structure;
using JumpMaster.LevelControllers;

namespace JumpMaster.Obstacles
{
    [RequireComponent(typeof(Rigidbody), typeof(Animator), typeof(SphereCollider))]
    public abstract class Obstacle<ObstacleScriptableObject, ObstacleController, SpawnScriptableObject, SpawnMetricsScriptableObject, SpawnArguments> : Initializable, IObstacle
        where ObstacleScriptableObject : ObstacleSO
        where ObstacleController : IObstacleController
        where SpawnScriptableObject : SpawnSO
        where SpawnMetricsScriptableObject : SpawnMetricsSO<ObstacleScriptableObject, SpawnScriptableObject>
        where SpawnArguments : SpawnArgs
    {
        public ObstacleScriptableObject Data { get; private set; }

        private ObstacleController _controller;
        protected SpawnMetricsScriptableObject SpawnMetrics => (SpawnMetricsScriptableObject)_controller.SpawnMetrics;

        public void Generate(ObstacleSO data, IObstacleController controller)
        {
            if (Initialized)
                return;

            if (!data.GetType().Equals(typeof(ObstacleScriptableObject)))
                return;

            Data = (ObstacleScriptableObject)data;
            _controller = (ObstacleController)controller;

            InitializeBase();
            Initialize();

            Initialized = true;

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

            BoundsScreenPosition = GetBoundsScreenPosition();

            OnUpdate();

            if (IsDespawnable())
                Despawn();
        }

        private void InitializeBase()
        {
            LevelController.OnRestart += Restart;

            CacheComponents();

            transform.localScale = Vector3.one * Data.Scale;
            transform.position = new Vector3(0, 0, Data.Z_Position);

            AlignPlayerDetection();
        }

        private void Restart()
        {
            RestartInstructions();
            Despawn();
        }

        protected abstract void RestartInstructions();
        protected abstract void SpawnInstructions();
        protected abstract void DespawnInstructions();
        protected abstract void OnUpdate();
        protected abstract bool IsDespawnable();

        protected abstract override void Initialize();

        // ##### EVENTS ##### \\
        public event ObstacleStateEventHandler OnSpawn;
        public event ObstacleStateEventHandler OnDespawn;

        // ##### SPAWNING ##### \\
        public bool Spawned { get; private set; }
        public SpawnArguments SpawnArgs { get; private set; }
        public SpawnScriptableObject SpawnData { get; private set; }

        public void Spawn(SpawnSO spawn_data, SpawnArgs spawn_args)
        {
            if (Spawned)
                return;

            if (!spawn_data.GetType().Equals(typeof(SpawnScriptableObject)))
                return;

            if (!spawn_args.GetType().Equals(typeof(SpawnArguments)))
                return;

            Spawn((SpawnScriptableObject)spawn_data, (SpawnArguments)spawn_args);
        }
        private void Spawn(SpawnScriptableObject spawn_data, SpawnArguments spawn_args)
        {
            SpawnData = spawn_data;
            SpawnArgs = spawn_args;

            SpawnInstructions();

            OnSpawn?.Invoke(this);

            Spawned = true;
        }

        public void Despawn()
        {
            if (!Spawned)
                return;

            DespawnInstructions();

            OnDespawn?.Invoke(this);

            gameObject.SetActive(false);

            SpawnData = null;
            SpawnArgs = null;
            Spawned = false;
        }

        // ##### SCREEN POSITION ##### \\
        public Vector2 ScreenPosition { get { return c_camera.WorldToScreenPoint(transform.position); } }
        public (Vector2 min, Vector2 max) BoundsScreenPosition { get; private set; }

        public bool BoundsUnderScreen => (BoundsScreenPosition.max.y < 0);
        public bool BoundsOverScreen => (BoundsScreenPosition.min.y > Screen.height);
        public bool BoundsLeftOfScreen => (BoundsScreenPosition.max.x < 0);
        public bool BoundsRightOfScreen => (BoundsScreenPosition.min.x > Screen.width);

        private (Vector2 min, Vector2 max) GetBoundsScreenPosition()
        {
            Vector2 min = c_camera.WorldToScreenPoint(c_bounds.bounds.min);
            Vector2 max = c_camera.WorldToScreenPoint(c_bounds.bounds.max);
            return (min, max);
        }

        // ##### CACHED COMPONENTS ##### \\
        protected Camera c_camera { get; private set; }
        protected Animator c_animator { get; private set; }
        protected Rigidbody c_rigidbody { get; private set; }
        protected BoxCollider c_bounds { get; private set; }
        protected SphereCollider c_sphereCol { get; private set; }

        private void CacheComponents()
        {
            c_camera = Camera.main;
            c_animator = GetComponent<Animator>();
            c_rigidbody = GetComponent<Rigidbody>();
            c_sphereCol = GetComponent<SphereCollider>();

            CacheBounds();
        }
        private void CacheBounds()
        {
            for (int child = 0; child < transform.childCount; child++)
            {
                if (!transform.GetChild(child).name.Equals("bounds"))
                    continue;
                c_bounds = transform.GetChild(child).GetComponent<BoxCollider>();
            }
            if (c_bounds == null)
            {
                GameObject newBounds = Instantiate(new GameObject(), transform);
                newBounds.transform.SetParent(transform);
                newBounds.name = "bounds";
                newBounds.AddComponent<BoxCollider>();
                newBounds.GetComponent<BoxCollider>().size = Vector3.one * 2f;
                newBounds.GetComponent<BoxCollider>().isTrigger = true;
                c_bounds = newBounds.GetComponent<BoxCollider>();
            }
            c_bounds.isTrigger = true;
        }

        // ##### EXTRACTED FUNCTIONS ##### \\

        private void AlignPlayerDetection()
        {
            Vector3 local_player_position;
            local_player_position = transform.InverseTransformPoint(MovementController.Instance.transform.position);
            c_sphereCol.center = new Vector3(c_sphereCol.center.x, c_sphereCol.center.y, local_player_position.z);
        }
    }
}