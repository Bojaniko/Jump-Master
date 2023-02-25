using System.Collections;

using UnityEngine;

using JumpMaster.LevelControllers;
using JumpMaster.LevelControllers.Obstacles;

using Studio28.SFX;

namespace JumpMaster.Obstacles
{
    public sealed class LaserGateSpawnArgs : SpawnArgs
    {
        public readonly Vector3 SpawnPosition;
        public LaserGateSpawnArgs(Vector3 screen_position, Vector3 spawn_position) : base(screen_position)
        {
            SpawnPosition = spawn_position;
        }
    }

    public class LaserGate : Obstacle, ISpawnable<LaserGate, LaserGateSpawnSO, LaserGateSpawnArgs>, IObstacle<LaserGateSO, LaserGateController>
    {
        private ObstacleSpawnController<LaserGate, LaserGateSpawnSO, LaserGateSpawnArgs, ISpawnable<LaserGate, LaserGateSpawnSO, LaserGateSpawnArgs>> _spawnController;
        public ObstacleSpawnController<LaserGate, LaserGateSpawnSO, LaserGateSpawnArgs, ISpawnable<LaserGate, LaserGateSpawnSO, LaserGateSpawnArgs>> SpawnController
        {
            get
            {
                return _spawnController;
            }
        }
        public LaserGateSO Data
        {
            get
            {
                return _data as LaserGateSO;
            }
        }
        public LaserGateController Controller
        {
            get
            {
                return _controller as LaserGateController;
            }
        }
        public LaserGate ObstacleSelf
        {
            get
            {
                return this;
            }
        }

        private float _countdownInterval;

        private LineRenderer[] _laserRenderers;

        private GameObject _lasers;

        private GameObject _leftGate;
        private GameObject _rightGate;

        private MeshRenderer _leftGateRenderer;
        private MeshRenderer _rightGateRenderer;

        private Material[] _gateMaterials;

        private Transform[] _laserPointsLeft;
        private Transform[] _laserPointsRight;

        private Coroutine _laserGateCoroutine;

        private IEnumerator LaserGateLoop()
        {
            yield return new WaitForSeconds(_countdownInterval);

            SetMaterialColor("light_0", _spawnController.SpawnData.ActiveColor);

            yield return new WaitForSeconds(_countdownInterval);

            SetMaterialColor("light_1", _spawnController.SpawnData.ActiveColor);

            yield return new WaitForSeconds(_countdownInterval);

            SetMaterialColor("light_2", _spawnController.SpawnData.ActiveColor);

            yield return new WaitForSeconds(_countdownInterval);

            SetMaterialColor("light_main", _spawnController.SpawnData.ActiveColor);

            LaserLinesSetActive(true);

            yield return new WaitForSeconds(_spawnController.SpawnData.GateHoldTime);

            ResetMaterials();

            LaserLinesSetActive(false);

            yield return LaserGateLoop();
        }

        protected override void Spawn()
        {
            transform.position = new Vector3(_spawnController.SpawnArgs.SpawnPosition.x, _spawnController.SpawnArgs.SpawnPosition.y, transform.position.z);

            _countdownInterval = _spawnController.SpawnData.CountdownIntervalMS / 1000f;

            BoxCollider lasersCollider = _lasers.GetComponent<BoxCollider>();
            lasersCollider.size = new Vector3(lasersCollider.size.y, _spawnController.SpawnData.GateWidth, lasersCollider.size.z);

            _leftGate.transform.localPosition = new Vector3(0, 0, _spawnController.SpawnData.GateWidth * 0.5f);
            _rightGate.transform.localPosition = new Vector3(0, 0, -_spawnController.SpawnData.GateWidth * 0.5f);

            CalculateLaserLines();

            LaserLinesSetActive(false);

            ResetMaterials();

            gameObject.SetActive(true);

            _laserGateCoroutine = StartCoroutine("LaserGateLoop");
        }

        protected override void Despawn<ObstacleType, SpawnScriptableObject, SpawnArguments>(ISpawnable<ObstacleType, SpawnScriptableObject, SpawnArguments> spawn)
        {
            StopCoroutine(_laserGateCoroutine);

            gameObject.SetActive(false);
        }

        protected override bool IsDespawnable()
        {
            if (!_spawnController.Spawned)
                return false;
            return BoundsUnderScreen;
        }

        protected override void Initialize()
        {
            _laserRenderers = new LineRenderer[3];

            _laserPointsLeft = new Transform[3];
            _laserPointsRight = new Transform[3];

            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject current_child = transform.GetChild(i).gameObject;

                switch(current_child.name)
                {
                    case "gate_left":
                        _leftGate = current_child;
                        _leftGateRenderer = _leftGate.GetComponent<MeshRenderer>();
                        _gateMaterials = _leftGateRenderer.materials;

                        for (int k = 0; k < _leftGate.transform.childCount; k++)
                        {
                            Transform current_point = _leftGate.transform.GetChild(k);

                            switch(current_point.name)
                            {
                                case "laser_point_left_0":
                                    _laserPointsLeft[0] = current_point;
                                    continue;

                                case "laser_point_left_1":
                                    _laserPointsLeft[1] = current_point;
                                    continue;

                                case "laser_point_left_2":
                                    _laserPointsLeft[2] = current_point;
                                    continue;
                            }
                        }

                        continue;

                    case "gate_right":
                        _rightGate = current_child;
                        _rightGateRenderer = _rightGate.GetComponent<MeshRenderer>();

                        for (int k = 0; k < _rightGate.transform.childCount; k++)
                        {
                            Transform current_point = _rightGate.transform.GetChild(k);

                            switch (current_point.name)
                            {
                                case "laser_point_right_0":
                                    _laserPointsRight[0] = current_point;
                                    continue;

                                case "laser_point_right_1":
                                    _laserPointsRight[1] = current_point;
                                    continue;

                                case "laser_point_right_2":
                                    _laserPointsRight[2] = current_point;
                                    continue;
                            }
                        }

                        continue;

                    case "lasers":
                        _lasers = current_child;

                        for (int k = 0; k < _lasers.transform.childCount; k++)
                        {
                            LineRenderer current_laser = _lasers.transform.GetChild(k).GetComponent<LineRenderer>();

                            AnimationCurve laserWidthCurve = new();
                            laserWidthCurve.AddKey(0f, Data.LaserWidth);

                            switch (current_laser.name)
                            {
                                case "laser_0":
                                    _laserRenderers[0] = current_laser;
                                    _laserRenderers[0].widthCurve = laserWidthCurve;
                                    continue;

                                case "laser_1":
                                    _laserRenderers[1] = current_laser;
                                    _laserRenderers[1].widthCurve = laserWidthCurve;
                                    continue;

                                case "laser_2":
                                    _laserRenderers[2] = current_laser;
                                    _laserRenderers[2].widthCurve = laserWidthCurve;
                                    continue;
                            }
                        }

                        continue;
                }
            }

            _spawnController = new(this, this);
            _spawnController.OnSpawn += Spawn;
            _spawnController.OnDespawn += Despawn;
        }

        protected override void OnUpdate()
        {
            
        }

        private void LaserLinesSetActive(bool active)
        {
            foreach(LineRenderer lineRenderer in _laserRenderers)
            {
                lineRenderer.enabled = active;
            }
        }

        private void CalculateLaserLines()
        {
            for (int k = 0; k < _laserRenderers.Length; k++)
            {
                LineRenderer current_laser = _laserRenderers[k];

                switch (current_laser.name)
                {
                    case "laser_0":

                        current_laser.positionCount = 2;
                        current_laser.SetPositions(new Vector3[2] { _laserPointsLeft[0].position, _laserPointsRight[0].position });
                        continue;

                    case "laser_1":

                        current_laser.positionCount = 2;
                        current_laser.SetPositions(new Vector3[2] { _laserPointsLeft[1].position, _laserPointsRight[1].position });
                        continue;

                    case "laser_2":

                        current_laser.positionCount = 2;
                        current_laser.SetPositions(new Vector3[2] { _laserPointsLeft[2].position, _laserPointsRight[2].position });
                        continue;
                }
            }
        }

        private void ResetMaterials()
        {
            foreach(Material mat in _gateMaterials)
            {
                if (mat.name.Contains("light"))
                    mat.SetColor("_BaseColor", _spawnController.SpawnData.InactiveColor);
            }
            ApplyMaterials();
        }

        private void SetMaterialColor(string material_name, Color color)
        {
            foreach(Material mat in _gateMaterials)
            {
                if (mat.name.Contains(material_name))
                {
                    mat.SetColor("_BaseColor", color);
                    break;
                }
            }
            ApplyMaterials();
        }

        private void ApplyMaterials()
        {
            _leftGateRenderer.materials = _gateMaterials;
            _rightGateRenderer.materials = _gateMaterials;
        }
    }
}