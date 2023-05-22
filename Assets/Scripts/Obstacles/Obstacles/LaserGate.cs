using System.Collections;

using UnityEngine;

using JumpMaster.Structure;
using JumpMaster.Damage;

using Studio28.Audio.SFX;

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

    public class LaserGate : Obstacle<LaserGateSO, LaserGateSpawnSO, LaserGateSpawnMetricsSO, LaserGateSpawnArgs>
    {
        private Vector2 _startBoundingSize;

        protected override void Initialize()
        {
            Cache();

            ActiateLaserGate(false);

            OnSpawn += StartLaserGateLoop;
        }

        protected override void OnUpdate()
        {
            
        }

        protected override void OnFixedUpdate()
        {
            
        }

        // ##### LOOP ##### \\

        private float _countdownInterval;
        private Coroutine _laserGateCoroutine;

        private void StopLaserGateLoop()
        {
            if (_laserGateCoroutine == null)
                return;
            StopCoroutine(_laserGateCoroutine);
            _laserGateCoroutine = null;
        }

        private void StartLaserGateLoop(IObstacle obstacle)
        {
            if (_laserGateCoroutine != null)
                return;

            if (!Spawned)
                return;

            _countdownInterval = SpawnData.CountdownIntervalMS / 1000f;

            _laserGateCoroutine = StartCoroutine("LaserGateLoop");
        }

        private IEnumerator LaserGateLoop()
        {
            yield return new WaitForSecondsPausable(_countdownInterval);

            SetLightMaterialColor("light_0", Data.ActiveColor);

            yield return new WaitForSecondsPausable(_countdownInterval);

            SetLightMaterialColor("light_1", Data.ActiveColor);

            yield return new WaitForSecondsPausable(_countdownInterval);

            SetLightMaterialColor("light_2", Data.ActiveColor);

            yield return new WaitForSecondsPausable(_countdownInterval);

            SetLightMaterialColor("light_main", Data.ActiveColor);

            ActiateLaserGate(true);

            GenerateStunDamageArea();

            yield return new WaitForSecondsPausable(SpawnData.GateHoldTime);

            ResetLightMaterials();

            ActiateLaserGate(false);

            yield return LaserGateLoop();
        }

        private void GenerateStunDamageArea()
        {
            Vector2 a = new Vector2(-SpawnData.GateWidth * 0.5f, -Data.LaserAreaHeight * 0.5f);
            a = transform.TransformPoint(a);
            Vector2 b = new Vector2(SpawnData.GateWidth * 0.5f, Data.LaserAreaHeight * 0.5f);
            b = transform.TransformPoint(b);

            AreaDamageSourceData areaData = new(a, b, transform.position);
            StunAreaDamageSource stunSource = new(areaData, SpawnData.GateHoldTime, SpawnData.Damage, "Player");
            DamageController.Instance.LogDamageSource(stunSource);
        }

        // ##### SPAWNING ##### \\

        protected override void SpawnInstructions()
        {
            transform.position = new Vector3(SpawnArgs.SpawnPosition.x, SpawnArgs.SpawnPosition.y, Data.Z_Position);

            c_bounding.SetSize(new Vector2(_startBoundingSize.x + SpawnData.GateWidth, _startBoundingSize.y));

            ApplyGateWidth(SpawnData.GateWidth);

            CalculateLaserLines();

            ActiateLaserGate(false);

            ResetLightMaterials();
        }

        protected override void DespawnInstructions()
        {
            StopLaserGateLoop();
            ActiateLaserGate(false);
        }

        protected override bool IsDespawnable()
        {
            return BoundsUnderScreen;
        }

        private void ApplyGateWidth(float width)
        {
            c_leftGate.transform.localPosition = new Vector3((-width * c_invertedScale) * 0.5f, 0, 0);
            c_rightGate.transform.localPosition = new Vector3((width * c_invertedScale) * 0.5f, 0, 0);
        }

        // #### LASER LINES ##### \\

        private void ActiateLaserGate(bool active)
        {
            foreach (LineRenderer lineRenderer in c_laserRenderers)
            {
                lineRenderer.enabled = active;
            }
        }

        private void CalculateLaserLines()
        {
            for (int k = 0; k < c_laserRenderers.Length; k++)
            {
                LineRenderer current_laser = c_laserRenderers[k];

                switch (current_laser.name)
                {
                    case "laser_0":
                        current_laser.positionCount = 2;
                        current_laser.SetPositions(new Vector3[2] { c_laserPointsLeft[0].position, c_laserPointsRight[0].position });
                        continue;

                    case "laser_1":
                        current_laser.positionCount = 2;
                        current_laser.SetPositions(new Vector3[2] { c_laserPointsLeft[1].position, c_laserPointsRight[1].position });
                        continue;

                    case "laser_2":
                        current_laser.positionCount = 2;
                        current_laser.SetPositions(new Vector3[2] { c_laserPointsLeft[2].position, c_laserPointsRight[2].position });
                        continue;
                }
            }
        }

        // ##### LIGHT MATERIALS ##### \\

        private void ResetLightMaterials()
        {
            foreach (Material mat in c_gateMaterials)
            {
                if (mat.name.Contains("light"))
                    mat.SetColor("_EmissionColor", Data.InactiveColor);
            }
            ApplyLightMaterials();
        }

        private void SetLightMaterialColor(string material_name, Color color)
        {
            foreach (Material mat in c_gateMaterials)
            {
                if (mat.name.Contains(material_name))
                {
                    mat.SetColor("_EmissionColor", color);
                    break;
                }
            }
            ApplyLightMaterials();
        }

        private void ApplyLightMaterials()
        {
            c_leftGateRenderer.materials = c_gateMaterials;
            c_rightGateRenderer.materials = c_gateMaterials;
        }

        // ##### CACHE ##### \\

        private GameObject c_leftGate;
        private GameObject c_rightGate;

        private Transform[] c_laserPointsLeft;
        private Transform[] c_laserPointsRight;

        private Material[] c_gateMaterials;

        private MeshRenderer c_leftGateRenderer;
        private MeshRenderer c_rightGateRenderer;

        private LineRenderer[] c_laserRenderers;

        private void Cache()
        {
            c_laserRenderers = new LineRenderer[3];

            c_laserPointsLeft = new Transform[3];
            c_laserPointsRight = new Transform[3];

            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject current_child = transform.GetChild(i).gameObject;

                switch (current_child.name)
                {
                    case "gate_left":
                        c_leftGate = current_child;
                        c_leftGateRenderer = c_leftGate.GetComponent<MeshRenderer>();
                        c_gateMaterials = c_leftGateRenderer.materials;

                        for (int k = 0; k < c_leftGate.transform.childCount; k++)
                        {
                            Transform current_point = c_leftGate.transform.GetChild(k);

                            switch (current_point.name)
                            {
                                case "laser_point_left_0":
                                    c_laserPointsLeft[0] = current_point;
                                    continue;

                                case "laser_point_left_1":
                                    c_laserPointsLeft[1] = current_point;
                                    continue;

                                case "laser_point_left_2":
                                    c_laserPointsLeft[2] = current_point;
                                    continue;
                            }
                        }

                        continue;

                    case "gate_right":
                        c_rightGate = current_child;
                        c_rightGateRenderer = c_rightGate.GetComponent<MeshRenderer>();

                        for (int k = 0; k < c_rightGate.transform.childCount; k++)
                        {
                            Transform current_point = c_rightGate.transform.GetChild(k);

                            switch (current_point.name)
                            {
                                case "laser_point_right_0":
                                    c_laserPointsRight[0] = current_point;
                                    continue;

                                case "laser_point_right_1":
                                    c_laserPointsRight[1] = current_point;
                                    continue;

                                case "laser_point_right_2":
                                    c_laserPointsRight[2] = current_point;
                                    continue;
                            }
                        }

                        continue;

                    case "lasers":
                        for (int k = 0; k < current_child.transform.childCount; k++)
                        {
                            LineRenderer current_laser = current_child.transform.GetChild(k).GetComponent<LineRenderer>();

                            AnimationCurve laserWidthCurve = new();
                            laserWidthCurve.AddKey(0f, Data.LaserWidth);

                            switch (current_laser.name)
                            {
                                case "laser_0":
                                    c_laserRenderers[0] = current_laser;
                                    c_laserRenderers[0].widthCurve = laserWidthCurve;
                                    continue;

                                case "laser_1":
                                    c_laserRenderers[1] = current_laser;
                                    c_laserRenderers[1].widthCurve = laserWidthCurve;
                                    continue;

                                case "laser_2":
                                    c_laserRenderers[2] = current_laser;
                                    c_laserRenderers[2].widthCurve = laserWidthCurve;
                                    continue;
                            }
                        }

                        continue;
                }
            }
        }
    }
}