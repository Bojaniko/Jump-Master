using UnityEngine;

using Studio28.SFX;

using JumpMaster.LevelControllers;
using JumpMaster.LevelControllers.Obstacles;

namespace JumpMaster.Obstacles
{
    [RequireComponent(typeof(Rigidbody), typeof(Animator), typeof(SphereCollider))]
    public abstract class Obstacle : MonoBehaviour
    {
        protected ObstacleSO _data;
        protected ObstacleController _controller;

        public Vector2 ScreenPosition { get { return Camera.main.WorldToScreenPoint(transform.position); } }
        public (Vector2 min, Vector2 max) BoundsScreenPosition { get; private set; }

        protected SFXController _sfxController { get; private set; }

        protected BoxCollider _bounds { get; private set; }

        protected Rigidbody _rigidbody { get; private set; }

        protected Animator _animator { get; private set; }

        protected SphereCollider _sphereCol { get; private set; }

        private bool _initialized = false;
        public bool Initialized { get { return _initialized; } }

        public delegate void InitializationEventHandler();
        public event InitializationEventHandler OnInitialized;

        public delegate void DespawnableEventHandler();
        public event DespawnableEventHandler OnDespawnable;

        public void Generate(ObstacleSO data, ObstacleController controller)
        {
            if (_initialized)
                return;

            _data = data;
            _controller = controller;

            InitializeBase();

            Initialize();

            _initialized = true;

            if (OnInitialized != null)
                OnInitialized();

            gameObject.SetActive(false);
        }

        private void Awake()
        {
            if (!_initialized)
                gameObject.SetActive(false);
        }

        private void Update()
        {
            BoundsScreenPosition = GetBoundsScreenPosition();

            OnUpdate();

            if (IsDespawnable())
            {
                if (OnDespawnable != null)
                    OnDespawnable();
            }
        }

        private void InitializeBase()
        {
            _sfxController = SFXController.Instance;

            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _sphereCol = GetComponent<SphereCollider>();

            transform.localScale = Vector3.one * _data.Scale;
            transform.position = new Vector3(0, 0, _data.Z_Position);
            Vector3 local_player_position = transform.InverseTransformPoint(LevelController.Instance.PlayerGameObject.transform.position);
            _sphereCol.center = new Vector3(_sphereCol.center.x, _sphereCol.center.y, local_player_position.z);

            for (int child = 0; child < transform.childCount; child++)
            {
                if (transform.GetChild(child).name.Equals("bounds"))
                {
                    _bounds = transform.GetChild(child).GetComponent<BoxCollider>();

                    if (_bounds == null)
                    {
                        _bounds = transform.GetChild(child).gameObject.AddComponent<BoxCollider>();
                        _bounds.size = Vector3.one * 2f;
                    }
                    _bounds.isTrigger = true;
                }
            }
            if (_bounds == null)
            {
                GameObject newBounds = Instantiate(new GameObject(), transform);
                newBounds.transform.SetParent(transform);
                newBounds.name = "bounds";
                newBounds.AddComponent<BoxCollider>();
                newBounds.GetComponent<BoxCollider>().size = Vector3.one * 2f;
                newBounds.GetComponent<BoxCollider>().isTrigger = true;
                _bounds = newBounds.GetComponent<BoxCollider>();
            }
        }

        protected abstract void Initialize();
        protected abstract void OnUpdate();
        protected abstract void Spawn();
        protected abstract void Despawn<ObstacleType, SpawnScriptableObject, SpawnArguments>(ISpawnable<ObstacleType, SpawnScriptableObject, SpawnArguments> spawn)
            where SpawnScriptableObject : SpawnSO
            where SpawnArguments : SpawnArgs
            where ObstacleType : Obstacle;
        protected abstract bool IsDespawnable();

        public bool BoundsUnderScreen
        {
            get
            {
                return (BoundsScreenPosition.max.y < 0);
            }
        }
        public bool BoundsOverScreen
        {
            get
            {
                return (BoundsScreenPosition.min.y > Screen.height);
            }
        }
        public bool BoundsLeftOfScreen
        {
            get
            {
                return (BoundsScreenPosition.max.x < 0);
            }
        }
        public bool BoundsRightOfScreen
        {
            get
            {
                return (BoundsScreenPosition.min.x > Screen.width);
            }
        }

        private (Vector2 min, Vector2 max) GetBoundsScreenPosition()
        {
            Vector2 min = Camera.main.WorldToScreenPoint(_bounds.bounds.min);
            Vector2 max = Camera.main.WorldToScreenPoint(_bounds.bounds.max);
            return (min, max);
        }
    }
}